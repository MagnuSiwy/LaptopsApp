using System.Collections.Generic;
using System.Linq;
using Magnuszewski.LaptopsApp.Core;
using Magnuszewski.LaptopsApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Magnuszewski.LaptopsApp.DAO
{
    public class LaptopStorage : ILaptopStorage
    {
        private readonly LaptopContext context;

        public LaptopStorage()
        {
            context = new LaptopContext();
            context.Database.EnsureCreated();
        }

        public IEnumerable<ILaptop> GetLaptops()
        {
            return context.Laptops
                .Include(l => l.Producer)
                .AsEnumerable()
                .Cast<ILaptop>()
                .ToList();
        }

        public void AddLaptop(ILaptop laptop)
        {
            context.Laptops.Add((Laptop)laptop); // Cast to the correct type
            context.SaveChanges();
        }

        public void UpdateLaptop(ILaptop laptop)
        {
            context.Laptops.Update((Laptop)laptop); // Cast to the correct type
            context.SaveChanges();
        }

        public void DeleteLaptop(int id)
        {
            var laptop = context.Laptops.Find(id);
            if (laptop != null)
            {
                context.Laptops.Remove(laptop);
                context.SaveChanges();
            }
        }

        public ILaptop GetLaptopById(int id)
        {
            return context.Laptops
                .Include(l => l.Producer)
                .FirstOrDefault(l => l.Id == id);
        }

        public IEnumerable<ILaptop> GetLaptopsByType(LaptopType type)
        {
            return context.Laptops
                .Include(l => l.Producer)
                .Where(l => l.Type == type)
                .AsEnumerable()
                .Cast<ILaptop>()
                .ToList();
        }
    }
}