using System.ComponentModel;
using System.Runtime.CompilerServices;
using Magnuszewski.LaptopsApp.Core;
using Magnuszewski.LaptopsApp.Interfaces;

namespace Magnuszewski.LaptopsApp.DAO
{
    public class Laptop : ILaptop, INotifyPropertyChanged
    {
        private int id;
        private string model;
        private IProducer producer;
        private LaptopType type;
        private double price;

        public int Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        public string Model
        {
            get => model;
            set
            {
                model = value;
                OnPropertyChanged();
            }
        }

        public int ProducerId { get; set; }

        public Producer Producer
        {
            get => (Producer)producer;
            set
            {
                producer = value;
                OnPropertyChanged();
            }
        }

        IProducer ILaptop.Producer
        {
            get => Producer;
            set => Producer = (Producer)value;
        }

        public LaptopType Type
        {
            get => type;
            set
            {
                type = value;
                OnPropertyChanged();
            }
        }

        public double Price
        {
            get => price;
            set
            {
                price = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}