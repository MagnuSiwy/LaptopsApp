using Magnuszewski.LaptopsApp.ViewModels;
using System.Windows;

namespace Magnuszewski.LaptopsApp
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        // Parameterless constructor for XAML
        public MainWindow() : this(null) { }
    }
}