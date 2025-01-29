using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magnuszewski.LaptopsApp.Core;

namespace Magnuszewski.LaptopsApp.Interfaces
{
    public interface ILaptop
    {
        int Id { get; set; }
        string Model { get; set; }
        IProducer Producer { get; set; }
        LaptopType Type { get; set; }
        double Price { get; set; }
    }
}
