using System;

namespace himchistka.practic.Models;

public class JobPosition
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class AdminOrder
{
    public int Id { get; set; }
    public string Client { get; set; } = string.Empty;
    public string Services { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Stage { get; set; } = "Принят";
}
