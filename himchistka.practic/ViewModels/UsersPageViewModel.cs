using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using himchistka.practic.Models;
using himchistka.practic.Services;

namespace himchistka.practic.ViewModels;

public class UsersPageViewModel : BaseViewModel
{
    private int _nextPositionId = 4;
    private JobPosition? _selectedPosition;
    private string _positionName = string.Empty;
    private string _positionDescription = string.Empty;

    public UsersPageViewModel(NavigationService navigationService)
    {
        BackCommand = new RelayCommand(() => navigationService.GoBack(), () => navigationService.CanGoBack);
        AddPositionCommand = new RelayCommand(AddPosition);
        UpdatePositionCommand = new RelayCommand(UpdatePosition, () => SelectedPosition is not null);
        DeletePositionCommand = new RelayCommand(DeletePosition, () => SelectedPosition is not null);

        Positions =
        [
            new() { Id = 1, Name = "Технолог", Description = "Контроль качества и подбор химии" },
            new() { Id = 2, Name = "Приёмщик", Description = "Оформление заказов и выдача" },
            new() { Id = 3, Name = "Курьер", Description = "Доставка и забор вещей" }
        ];

        StageOptions = ["Принят", "В работе", "Готов", "Выдан"];

        Orders =
        [
            new() { Id = 101, Client = "Иван Иванов", Services = "Пальто x1", CreatedAt = DateTime.Today.AddDays(-2), Stage = "В работе" },
            new() { Id = 102, Client = "Анна Смирнова", Services = "Пуховик x1, Шторы x1", CreatedAt = DateTime.Today.AddDays(-1), Stage = "Принят" },
            new() { Id = 103, Client = "Максим Петров", Services = "Костюм x2", CreatedAt = DateTime.Today, Stage = "Готов" }
        ];
    }

    public ICommand BackCommand { get; }
    public ICommand AddPositionCommand { get; }
    public ICommand UpdatePositionCommand { get; }
    public ICommand DeletePositionCommand { get; }

    public ObservableCollection<JobPosition> Positions { get; }
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
}
