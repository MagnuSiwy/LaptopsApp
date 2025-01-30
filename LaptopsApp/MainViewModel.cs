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
using Magnuszewski.LaptopsApp.Views;

namespace Magnuszewski.LaptopsApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ILaptopStorage laptopStorage;
        private Laptop selectedLaptop;
        private Laptop newLaptop;
        private string errorMessage;
        private string searchQuery;
        private List<int> availableIds;
        private int nextId;

        public MainViewModel(ILaptopStorage laptopStorage)
        {
            this.laptopStorage = laptopStorage ?? throw new ArgumentNullException(nameof(laptopStorage));
            Laptops = new ObservableCollection<ILaptop>(laptopStorage.GetLaptops());
            FilteredLaptops = new ObservableCollection<ILaptop>(Laptops);
            Producers = new ObservableCollection<IProducer>(laptopStorage.GetProducers() ?? new List<IProducer>());
            LaptopTypes = Enum.GetValues(typeof(LaptopType));

            availableIds = new List<int>();
            nextId = Laptops.Any() ? Laptops.Max(l => l.Id) + 1 : 1;

            AddLaptopCommand = new RelayCommand(AddLaptop);
            SaveLaptopCommand = new RelayCommand(SaveLaptop, CanSaveLaptop);
            DeleteLaptopCommand = new RelayCommand(DeleteLaptop, CanModifyLaptop);
            OpenAddProducerCommand = new RelayCommand(OpenAddProducer);
        }

        public ILaptopStorage LaptopStorage => laptopStorage;

        public ObservableCollection<ILaptop> Laptops { get; }
        public ObservableCollection<ILaptop> FilteredLaptops { get; }
        public ObservableCollection<IProducer> Producers { get; }
        public Array LaptopTypes { get; }

        public Laptop SelectedLaptop
        {
            get => selectedLaptop;
            set
            {
                if (selectedLaptop != null)
                {
                    selectedLaptop.PropertyChanged -= Laptop_PropertyChanged;
                }

                selectedLaptop = value;

                if (selectedLaptop != null)
                {
                    selectedLaptop.PropertyChanged += Laptop_PropertyChanged;
                }

                // RefreshLaptops();

                IsLaptopValid(selectedLaptop);
                OnPropertyChanged();
                ((RelayCommand)DeleteLaptopCommand).RaiseCanExecuteChanged();
                ((RelayCommand)SaveLaptopCommand).RaiseCanExecuteChanged();
            }
        }

        public Laptop NewLaptop
        {
            get => newLaptop;
            set
            {
                if (newLaptop != null)
                {
                    newLaptop.PropertyChanged -= Laptop_PropertyChanged;
                }

                newLaptop = value;

                if (newLaptop != null)
                {
                    newLaptop.PropertyChanged += Laptop_PropertyChanged;
                }

                //RefreshLaptops();

                IsLaptopValid(newLaptop);
                OnPropertyChanged();
                ((RelayCommand)SaveLaptopCommand).RaiseCanExecuteChanged();
            }
        }

        private void RefreshLaptops()
        {
            var updatedLaptops = laptopStorage.GetLaptops();
            Laptops.Clear();

            foreach (var laptop in updatedLaptops)
            {
                Laptops.Add(laptop);
            }

            FilterLaptops();
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

        public string SearchQuery
        {
            get => searchQuery;
            set
            {
                searchQuery = value;
                OnPropertyChanged();
                FilterLaptops();
            }
        }

        public ICommand AddLaptopCommand { get; }
        public ICommand SaveLaptopCommand { get; }
        public ICommand DeleteLaptopCommand { get; }
        public ICommand OpenAddProducerCommand { get; }

        private void AddLaptop()
        {
            int id = availableIds.Any() ? availableIds.First() : nextId;
            if (availableIds.Any())
            {
                availableIds.RemoveAt(0);
            }

            NewLaptop = new Laptop
            {
                Id = id,
                Producer = (Producer)(Producers.FirstOrDefault() ?? new Producer())
            };
            SelectedLaptop = NewLaptop;
            ErrorMessage = string.Empty;
        }

        private void SaveLaptop()
        {
            if (NewLaptop != null)
            {
                if (!IsLaptopValid(NewLaptop)) return;

                NewLaptop.Id = availableIds.Any() ? availableIds.First() : nextId++;
                if (availableIds.Any())
                {
                    availableIds.RemoveAt(0);
                }

                laptopStorage.AddLaptop(NewLaptop);
                Laptops.Add(NewLaptop);
                FilterLaptops();
                NewLaptop = null;
            }
            else if (SelectedLaptop != null)
            {
                if (!IsLaptopValid(SelectedLaptop)) return;

                laptopStorage.UpdateLaptop(SelectedLaptop);
                FilterLaptops();
            }

            ErrorMessage = string.Empty;
        }

        private bool IsLaptopValid(ILaptop laptop)
        {
            if (laptop == null)
            {
                ErrorMessage = string.Empty;
                return true;
            }

            if (string.IsNullOrWhiteSpace(laptop.Model))
            {
                ErrorMessage = "Model field must be filled.";
                return false;
            }
            else if (string.IsNullOrWhiteSpace(laptop.Producer.Name))
            {
                ErrorMessage = "Producer field must be filled.";
                return false;
            }
            else if (laptop.Price <= 0)
            {
                ErrorMessage = "Price must be greater than 0.";
                return false;
            }

            ErrorMessage = string.Empty;
            return true;
        }

        private void Laptop_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsLaptopValid(sender as ILaptop);
        }

        private bool CanSaveLaptop() => NewLaptop != null || SelectedLaptop != null;

        private void DeleteLaptop()
        {
            if (SelectedLaptop != null)
            {
                laptopStorage.DeleteLaptop(SelectedLaptop.Id);
                availableIds.Add(SelectedLaptop.Id);
                Laptops.Remove(SelectedLaptop);
                FilterLaptops();
                SelectedLaptop = null;
            }
        }

        private bool CanModifyLaptop() => SelectedLaptop != null;

        private void OpenAddProducer()
        {
            var addProducerWindow = new AddProducerWindow
            {
                DataContext = new AddProducerViewModel(this)
            };
            addProducerWindow.ShowDialog();
        }

        private void FilterLaptops()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                FilteredLaptops.Clear();
                foreach (var laptop in Laptops)
                {
                    FilteredLaptops.Add(laptop);
                }
            }
            else
            {
                var filtered = Laptops.Where(l => l.Model.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                FilteredLaptops.Clear();
                foreach (var laptop in filtered)
                {
                    FilteredLaptops.Add(laptop);
                }
            }
        }

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