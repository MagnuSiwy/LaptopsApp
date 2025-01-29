using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Magnuszewski.LaptopsApp.DAO;
using Magnuszewski.LaptopsApp.Interfaces;

namespace Magnuszewski.LaptopsApp.ViewModels
{
    public class AddProducerViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel mainViewModel;
        private string newProducerName;
        private IProducer selectedProducer;

        public AddProducerViewModel(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel ?? throw new ArgumentNullException(nameof(mainViewModel));
            AddProducerCommand = new RelayCommand(AddProducer);
            DeleteProducerCommand = new RelayCommand(DeleteProducer, CanDeleteProducer);
        }

        public ObservableCollection<IProducer> Producers => mainViewModel.Producers;

        public string NewProducerName
        {
            get => newProducerName;
            set
            {
                newProducerName = value;
                OnPropertyChanged();
            }
        }

        public IProducer SelectedProducer
        {
            get => selectedProducer;
            set
            {
                selectedProducer = value;
                OnPropertyChanged();
                ((RelayCommand)DeleteProducerCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddProducerCommand { get; }
        public ICommand DeleteProducerCommand { get; }

        private void AddProducer()
        {
            if (string.IsNullOrWhiteSpace(NewProducerName))
            {
                MessageBox.Show("Producer name cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newProducer = new Producer { Name = NewProducerName };
            mainViewModel.LaptopStorage.AddProducer(newProducer);
            mainViewModel.Producers.Add(newProducer);

            NewProducerName = string.Empty;
        }

        private void DeleteProducer()
        {
            if (SelectedProducer != null)
            {
                if (mainViewModel != null)
                {
                    if (mainViewModel.LaptopStorage != null)
                    {
                        mainViewModel.Producers.Remove(SelectedProducer);
                        mainViewModel.LaptopStorage.DeleteProducer(SelectedProducer.Id);
                        SelectedProducer = null;
                        MessageBox.Show("Producer deleted successfully.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Unable to delete producer. Storage is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Console.WriteLine("LaptopStorage is null.");
                    }
                }
                else
                {
                    MessageBox.Show("MainViewModel is not initialized.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine("MainViewModel is null.");
                }
            }
            else
            {
                MessageBox.Show("No producer selected to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private bool CanDeleteProducer() => SelectedProducer != null;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}