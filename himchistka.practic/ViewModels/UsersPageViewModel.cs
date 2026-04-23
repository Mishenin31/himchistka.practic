using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using himchistka.practic.Models;
using himchistka.practic.Services;
using himchistka.practic.Views;

namespace himchistka.practic.ViewModels;

public class UsersPageViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;
    private readonly SessionService _sessionService;

    private int _nextPositionId = 4;
    private int _nextServiceId = 7;
    private JobPosition? _selectedPosition;
    private Product? _selectedService;
    private string _positionName = string.Empty;
    private string _positionDescription = string.Empty;
    private string _serviceName = string.Empty;
    private string _serviceDescription = string.Empty;
    private decimal _servicePrice;
    private int _serviceStock;

    public UsersPageViewModel(NavigationService navigationService, SessionService sessionService)
    {
        _navigationService = navigationService;
        _sessionService = sessionService;

        BackCommand = new RelayCommand(() => _navigationService.GoBack(), () => _navigationService.CanGoBack);
        AddPositionCommand = new RelayCommand(AddPosition);
        UpdatePositionCommand = new RelayCommand(UpdatePosition, () => SelectedPosition is not null);
        DeletePositionCommand = new RelayCommand(DeletePosition, () => SelectedPosition is not null);

        AddServiceCommand = new RelayCommand(AddService);
        UpdateServiceCommand = new RelayCommand(UpdateService, () => SelectedService is not null);
        DeleteServiceCommand = new RelayCommand(DeleteService, () => SelectedService is not null);

        Positions =
        [
            new() { Id = 1, Name = "Технолог", Description = "Контроль качества и подбор химии" },
            new() { Id = 2, Name = "Приёмщик", Description = "Оформление заказов и выдача" },
            new() { Id = 3, Name = "Курьер", Description = "Доставка и забор вещей" }
        ];

        Services =
        [
            new() { Id = 1, Name = "Химчистка пальто", Description = "Удаление пятен, отпаривание и восстановление ткани", Price = 3200, Stock = 14 },
            new() { Id = 2, Name = "Химчистка костюма", Description = "Деликатная сухая чистка и точечная обработка", Price = 2500, Stock = 20 },
            new() { Id = 3, Name = "Химчистка пуховика", Description = "Глубокая чистка с восстановлением объема утеплителя", Price = 4200, Stock = 9 },
            new() { Id = 4, Name = "Химчистка штор", Description = "Бережная чистка больших текстильных изделий", Price = 5400, Stock = 8 },
            new() { Id = 5, Name = "Чистка свадебного платья", Description = "Премиальный уход с ручной обработкой декора", Price = 7600, Stock = 6 },
            new() { Id = 6, Name = "Экспресс-обновление рубашек", Description = "Быстрая чистка и идеальное глажение", Price = 1100, Stock = 30 }
        ];

        StageOptions = ["Принят", "В работе", "Готов", "Выдан"];

        Orders =
        [
            new() { Id = 101, Client = "Иван Иванов", Services = "Пальто x1", CreatedAt = DateTime.Today.AddDays(-2), Stage = "В работе" },
            new() { Id = 102, Client = "Анна Смирнова", Services = "Пуховик x1, Шторы x1", CreatedAt = DateTime.Today.AddDays(-1), Stage = "Принят" },
            new() { Id = 103, Client = "Максим Петров", Services = "Костюм x2", CreatedAt = DateTime.Today, Stage = "Готов" }
        ];

        EnsureAdminAccess();
    }

    public ICommand BackCommand { get; }
    public ICommand AddPositionCommand { get; }
    public ICommand UpdatePositionCommand { get; }
    public ICommand DeletePositionCommand { get; }
    public ICommand AddServiceCommand { get; }
    public ICommand UpdateServiceCommand { get; }
    public ICommand DeleteServiceCommand { get; }

    public ObservableCollection<JobPosition> Positions { get; }
    public ObservableCollection<Product> Services { get; }
    public ObservableCollection<AdminOrder> Orders { get; }
    public IReadOnlyList<string> StageOptions { get; }

    public JobPosition? SelectedPosition
    {
        get => _selectedPosition;
        set
        {
            if (!SetProperty(ref _selectedPosition, value))
            {
                return;
            }

            PositionName = value?.Name ?? string.Empty;
            PositionDescription = value?.Description ?? string.Empty;

            (UpdatePositionCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeletePositionCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    public Product? SelectedService
    {
        get => _selectedService;
        set
        {
            if (!SetProperty(ref _selectedService, value))
            {
                return;
            }

            ServiceName = value?.Name ?? string.Empty;
            ServiceDescription = value?.Description ?? string.Empty;
            ServicePrice = value?.Price ?? 0;
            ServiceStock = value?.Stock ?? 0;

            (UpdateServiceCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteServiceCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    public string PositionName
    {
        get => _positionName;
        set => SetProperty(ref _positionName, value);
    }

    public string PositionDescription
    {
        get => _positionDescription;
        set => SetProperty(ref _positionDescription, value);
    }

    public string ServiceName
    {
        get => _serviceName;
        set => SetProperty(ref _serviceName, value);
    }

    public string ServiceDescription
    {
        get => _serviceDescription;
        set => SetProperty(ref _serviceDescription, value);
    }

    public decimal ServicePrice
    {
        get => _servicePrice;
        set => SetProperty(ref _servicePrice, value);
    }

    public int ServiceStock
    {
        get => _serviceStock;
        set => SetProperty(ref _serviceStock, value);
    }

    private void EnsureAdminAccess()
    {
        if (_sessionService.CurrentUser?.IsAdmin == true)
        {
            return;
        }

        MessageBox.Show("Админ панель доступна только администратору.");
        _navigationService.Navigate<CatalogPage>();
    }

    private void AddPosition()
    {
        if (string.IsNullOrWhiteSpace(PositionName))
        {
            MessageBox.Show("Введите название должности.");
            return;
        }

        Positions.Add(new JobPosition
        {
            Id = _nextPositionId++,
            Name = PositionName,
            Description = PositionDescription
        });

        PositionName = string.Empty;
        PositionDescription = string.Empty;
    }

    private void UpdatePosition()
    {
        if (SelectedPosition is null)
        {
            return;
        }

        SelectedPosition.Name = PositionName;
        SelectedPosition.Description = PositionDescription;

        var index = Positions.IndexOf(SelectedPosition);
        if (index >= 0)
        {
            Positions[index] = SelectedPosition;
        }
    }

    private void DeletePosition()
    {
        if (SelectedPosition is null)
        {
            return;
        }

        Positions.Remove(SelectedPosition);
        SelectedPosition = null;
    }

    private void AddService()
    {
        if (!ValidateServiceInputs())
        {
            return;
        }

        Services.Add(new Product
        {
            Id = _nextServiceId++,
            Name = ServiceName,
            Description = ServiceDescription,
            Price = ServicePrice,
            Stock = ServiceStock
        });

        ClearServiceInputs();
    }

    private void UpdateService()
    {
        if (SelectedService is null || !ValidateServiceInputs())
        {
            return;
        }

        SelectedService.Name = ServiceName;
        SelectedService.Description = ServiceDescription;
        SelectedService.Price = ServicePrice;
        SelectedService.Stock = ServiceStock;

        var index = Services.IndexOf(SelectedService);
        if (index >= 0)
        {
            Services[index] = SelectedService;
        }
    }

    private void DeleteService()
    {
        if (SelectedService is null)
        {
            return;
        }

        Services.Remove(SelectedService);
        SelectedService = null;
        ClearServiceInputs();
    }

    private bool ValidateServiceInputs()
    {
        if (string.IsNullOrWhiteSpace(ServiceName))
        {
            MessageBox.Show("Введите название услуги.");
            return false;
        }

        if (ServicePrice <= 0)
        {
            MessageBox.Show("Цена услуги должна быть больше 0.");
            return false;
        }

        if (ServiceStock < 0)
        {
            MessageBox.Show("Количество не может быть отрицательным.");
            return false;
        }

        return true;
    }

    private void ClearServiceInputs()
    {
        ServiceName = string.Empty;
        ServiceDescription = string.Empty;
        ServicePrice = 0;
        ServiceStock = 0;
    }
}
