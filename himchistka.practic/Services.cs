using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace himchistka.practic
{
    public static class ValidationService
    {
        public static bool IsValidLogin(string login)
        {
            return !string.IsNullOrWhiteSpace(login) && login.Length >= 4;
        }

        public static bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 6;
        }

        public static bool IsValidOrder(OrderRecord order)
        {
            return order != null
                   && !string.IsNullOrWhiteSpace(order.ClientFullName)
                   && !string.IsNullOrWhiteSpace(order.ServiceName)
                   && order.TotalPrice >= 0;
        }
    }

    public sealed class AuthService
    {
        private static readonly List<UserAccount> Users = new List<UserAccount>
        {
            new UserAccount { Id = 1, FullName = "Администратор", Login = "admin", PasswordHash = Hash("admin123"), Role = UserRole.Admin },
            new UserAccount { Id = 2, FullName = "Менеджер", Login = "manager", PasswordHash = Hash("manager123"), Role = UserRole.Manager },
            new UserAccount { Id = 3, FullName = "Пользователь", Login = "user", PasswordHash = Hash("user123"), Role = UserRole.User }
        };

        public IReadOnlyList<UserAccount> GetUsers()
        {
            return Users.Select(Clone).ToList();
        }

        public UserAccount Register(string fullName, string login, string password, UserRole role)
        {
            if (!ValidationService.IsValidLogin(login))
            {
                throw new InvalidOperationException("Логин должен содержать минимум 4 символа.");
            }

            if (!ValidationService.IsValidPassword(password))
            {
                throw new InvalidOperationException("Пароль должен содержать минимум 6 символов.");
            }

            if (Users.Any(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Пользователь с таким логином уже существует.");
            }

            var account = new UserAccount
            {
                Id = Users.Count == 0 ? 1 : Users.Max(u => u.Id) + 1,
                FullName = string.IsNullOrWhiteSpace(fullName) ? login : fullName,
                Login = login,
                PasswordHash = Hash(password),
                Role = role
            };

            Users.Add(account);
            return Clone(account);
        }

        public void UpdateUser(UserAccount updated)
        {
            var user = Users.FirstOrDefault(x => x.Id == updated.Id);
            if (user == null)
            {
                throw new InvalidOperationException("Пользователь не найден.");
            }

            if (!user.Login.Equals(updated.Login, StringComparison.OrdinalIgnoreCase)
                && Users.Any(x => x.Login.Equals(updated.Login, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Логин уже используется.");
            }

            user.FullName = updated.FullName;
            user.Login = updated.Login;
            user.Role = updated.Role;
        }

        public void DeleteUser(int userId)
        {
            var user = Users.FirstOrDefault(x => x.Id == userId);
            if (user != null && user.Login != "admin")
            {
                Users.Remove(user);
            }
        }

        public UserAccount Login(string login, string password)
        {
            var hash = Hash(password ?? string.Empty);
            var user = Users.FirstOrDefault(x => x.Login.Equals(login ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                                                 && x.PasswordHash == hash);
            return user == null ? null : Clone(user);
        }

        private static UserAccount Clone(UserAccount source)
        {
            return new UserAccount
            {
                Id = source.Id,
                FullName = source.FullName,
                Login = source.Login,
                PasswordHash = source.PasswordHash,
                Role = source.Role
            };
        }

        private static string Hash(string raw)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw ?? string.Empty));
                return Convert.ToBase64String(bytes);
            }
        }
    }

    public sealed class PdfReceiptService
    {
        public string CreateReceipt(string outputDirectory, OrderRecord order, IEnumerable<CartItem> items)
        {
            Directory.CreateDirectory(outputDirectory);
            var filePath = Path.Combine(outputDirectory, $"receipt-{order.Id}-{DateTime.Now:yyyyMMddHHmmss}.pdf");
            var qrPayload = $"ORDER:{order.Id}|SUM:{order.TotalPrice:0.00}|DATE:{order.DateReceived:yyyy-MM-dd}";
            var qrAscii = GeneratePseudoQr(qrPayload);

            var lines = new List<string>
            {
                "Чек химчистки CleanersDB",
                $"Заказ: #{order.Id}",
                $"Клиент: {order.ClientFullName}",
                $"Дата: {order.DateReceived:dd.MM.yyyy HH:mm}",
                "-------------------------------------"
            };

            foreach (var item in items)
            {
                lines.Add($"{item.ServiceName} x{item.Quantity} = {item.TotalPrice:0.00} руб.");
            }

            lines.Add("-------------------------------------");
            lines.Add($"ИТОГО: {order.TotalPrice:0.00} руб.");
            lines.Add("QR:");
            lines.AddRange(qrAscii.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None));

            WritePrimitivePdf(filePath, lines);
            return filePath;
        }

        private static string GeneratePseudoQr(string payload)
        {
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(payload));
                var size = 21;
                var sb = new StringBuilder();
                for (var y = 0; y < size; y++)
                {
                    for (var x = 0; x < size; x++)
                    {
                        var bit = hash[(x + y * size) % hash.Length] % 2 == 0;
                        sb.Append(bit ? "██" : "  ");
                    }

                    sb.AppendLine();
                }

                return sb.ToString();
            }
        }

        private static void WritePrimitivePdf(string filePath, IList<string> lines)
        {
            var contentBuilder = new StringBuilder();
            contentBuilder.AppendLine("BT");
            contentBuilder.AppendLine("/F1 10 Tf");
            contentBuilder.AppendLine("50 780 Td");

            for (var i = 0; i < lines.Count; i++)
            {
                var escaped = lines[i].Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
                if (i > 0)
                {
                    contentBuilder.AppendLine("0 -12 Td");
                }

                contentBuilder.AppendLine($"({escaped}) Tj");
            }

            contentBuilder.AppendLine("ET");

            var content = contentBuilder.ToString();
            var pdf = new StringBuilder();
            var offsets = new List<int>();

            pdf.AppendLine("%PDF-1.4");
            offsets.Add(pdf.Length);
            pdf.AppendLine("1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj");
            offsets.Add(pdf.Length);
            pdf.AppendLine("2 0 obj << /Type /Pages /Kids [3 0 R] /Count 1 >> endobj");
            offsets.Add(pdf.Length);
            pdf.AppendLine("3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >> endobj");
            offsets.Add(pdf.Length);
            pdf.AppendLine("4 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Courier >> endobj");
            offsets.Add(pdf.Length);
            pdf.AppendLine($"5 0 obj << /Length {content.Length} >> stream");
            pdf.Append(content);
            pdf.AppendLine("endstream endobj");

            var xrefPos = pdf.Length;
            pdf.AppendLine("xref");
            pdf.AppendLine("0 6");
            pdf.AppendLine("0000000000 65535 f ");
            foreach (var offset in offsets)
            {
                pdf.AppendLine($"{offset:D10} 00000 n ");
            }

            pdf.AppendLine("trailer << /Size 6 /Root 1 0 R >>");
            pdf.AppendLine("startxref");
            pdf.AppendLine(xrefPos.ToString());
            pdf.AppendLine("%%EOF");

            File.WriteAllText(filePath, pdf.ToString(), Encoding.ASCII);
        }
    }
}
