using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Magnuszewski.LaptopsApp.Core;
using Magnuszewski.LaptopsApp.Interfaces;
using Magnuszewski.LaptopsApp.DAO;

namespace Magnuszewski.LaptopsApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ILaptopStorage laptopStorage;
        private ILaptop selectedLaptop;
        private ILaptop newLaptop;
        private string errorMessage;
        private List<int> availableIds;
        private int nextId;

        public MainViewModel(ILaptopStorage laptopStorage)
        {
            this.laptopStorage = laptopStorage ?? throw new ArgumentNullException(nameof(laptopStorage));
            Laptops = new ObservableCollection<ILaptop>(laptopStorage.GetLaptops());
            LaptopTypes = Enum.GetValues(typeof(LaptopType));

            availableIds = new List<int>();
            nextId = Laptops.Any() ? Laptops.Max(l => l.Id) + 1 : 1;

            AddLaptopCommand = new RelayCommand(AddLaptop);
            SaveLaptopCommand = new RelayCommand(SaveLaptop, CanSaveLaptop);
            DeleteLaptopCommand = new RelayCommand(DeleteLaptop, CanModifyLaptop);
        }

        public ObservableCollection<ILaptop> Laptops { get; }
        public Array LaptopTypes { get; }

        public ILaptop SelectedLaptop
        {
            get => selectedLaptop;
            set
            {
                selectedLaptop = value;
                OnPropertyChanged();
                ((RelayCommand)DeleteLaptopCommand).RaiseCanExecuteChanged();
                ((RelayCommand)SaveLaptopCommand).RaiseCanExecuteChanged();
            }
        }

        public ILaptop NewLaptop
        {
            get => newLaptop;
            set
            {
                newLaptop = value;
                OnPropertyChanged();
                ((RelayCommand)SaveLaptopCommand).RaiseCanExecuteChanged();
            }
        }

        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddLaptopCommand { get; }
        public ICommand SaveLaptopCommand { get; }
        public ICommand DeleteLaptopCommand { get; }

        private void AddLaptop()
        {
            int id = availableIds.Any() ? availableIds.First() : nextId++;
            if (availableIds.Any())
            {
                availableIds.RemoveAt(0);
            }

            NewLaptop = new Laptop
            {
                Id = id,
                Producer = new Producer()
            };
            SelectedLaptop = NewLaptop;
            ErrorMessage = string.Empty;
        }

        private void SaveLaptop()
        {
            if (NewLaptop != null)
            {
                if (string.IsNullOrWhiteSpace(NewLaptop.Model))
                {
                    ErrorMessage = "Model field must be filled.";
                    return;
                }
                if (string.IsNullOrWhiteSpace(NewLaptop.Producer.Name))
                {
                    ErrorMessage = "Producer field must be filled.";
                    return;
                }
                if (NewLaptop.Price <= 0)
                {
                    ErrorMessage = "Price must be greater than 0.";
                    return;
                }

                laptopStorage.AddLaptop(NewLaptop);
                Laptops.Add(NewLaptop);
                NewLaptop = null;
            }
            else if (SelectedLaptop != null)
            {
                if (string.IsNullOrWhiteSpace(SelectedLaptop.Model))
                {
                    ErrorMessage = "Model field must be filled.";
                    return;
                }
                if (string.IsNullOrWhiteSpace(SelectedLaptop.Producer.Name))
                {
                    ErrorMessage = "Producer field must be filled.";
                    return;
                }
                if (SelectedLaptop.Price <= 0)
                {
                    ErrorMessage = "Price must be greater than 0.";
                    return;
                }

                laptopStorage.UpdateLaptop(SelectedLaptop);
            }

            ErrorMessage = string.Empty;
        }

        private bool CanSaveLaptop() => NewLaptop != null || SelectedLaptop != null;

        private void DeleteLaptop()
        {
            if (SelectedLaptop != null)
            {
                laptopStorage.DeleteLaptop(SelectedLaptop.Id);
                availableIds.Add(SelectedLaptop.Id);
                Laptops.Remove(SelectedLaptop);
                SelectedLaptop = null;
            }
        }

        private bool CanModifyLaptop() => SelectedLaptop != null;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => canExecute == null || canExecute();

        public void Execute(object parameter) => execute();

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}