using System.Collections.Generic;
using Magnuszewski.LaptopsApp.Core;

namespace Magnuszewski.LaptopsApp.Interfaces
{
    public interface ILaptopStorage
    {
        IEnumerable<ILaptop> GetLaptops();
        void AddLaptop(ILaptop laptop);
        void UpdateLaptop(ILaptop laptop);
        void DeleteLaptop(int id);
        ILaptop GetLaptopById(int id);
        IEnumerable<ILaptop> GetLaptopsByType(LaptopType type);
    }
}