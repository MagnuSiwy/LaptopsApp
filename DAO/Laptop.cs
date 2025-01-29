using Magnuszewski.LaptopsApp.Core;
using Magnuszewski.LaptopsApp.Interfaces;

namespace Magnuszewski.LaptopsApp.DAO
{
    public class Laptop : ILaptop
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public int ProducerId { get; set; }
        public Producer Producer { get; set; } // Use concrete type for EF Core
        IProducer ILaptop.Producer
        {
            get => Producer;
            set => Producer = (Producer)value;
        }
        public LaptopType Type { get; set; }
        public double Price { get; set; }
    }
}