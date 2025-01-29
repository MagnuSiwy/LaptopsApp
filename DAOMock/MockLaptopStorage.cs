using System.Collections.Generic;
using System.Linq;
using Magnuszewski.LaptopsApp.Core;
using Magnuszewski.LaptopsApp.DAO;
using Magnuszewski.LaptopsApp.Interfaces;

namespace Magnuszewski.LaptopsApp.DAOMock
{
    public class MockLaptopStorage : ILaptopStorage
    {
        private readonly List<ILaptop> laptops;
        private readonly List<IProducer> producers;

        public MockLaptopStorage()
        {
            producers = new List<IProducer>
                {
                    new Producer { Id = 1, Name = "Lenovo" },
                    new Producer { Id = 2, Name = "Dell" },
                    new Producer { Id = 3, Name = "Asus" }
                };

            laptops = new List<ILaptop>
                {
                    new Laptop
                    {
                        Id = 1,
                        Model = "ThinkPad T14s gen 1 (AMD)",
                        Producer = (Producer)producers[0],
                        Type = LaptopType.Business,
                        Price = 400
                    },
                    new Laptop
                    {
                        Id = 2,
                        Model = "ThinkPad x270",
                        Producer = (Producer)producers[0],
                        Type = LaptopType.Business,
                        Price = 100
                    },
                    new Laptop
                    {
                        Id = 3,
                        Model = "Latitude 3550",
                        Producer = (Producer)producers[1],
                        Type = LaptopType.Business,
                        Price = 700
                    },
                    new Laptop
                    {
                        Id = 4,
                        Model = "G15 5530",
                        Producer = (Producer)producers[1],
                        Type = LaptopType.Gaming,
                        Price = 1700
                    },
                    new Laptop
                    {
                        Id = 5,
                        Model = "IdeaPad Gaming 3 15ARH05",
                        Producer = (Producer)producers[0],
                        Type = LaptopType.Gaming,
                        Price = 700
                    },
                    new Laptop
                    {
                        Id = 6,
                        Model = "Vivobook 15",
                        Producer = (Producer)producers[2],
                        Type = LaptopType.Business,
                        Price = 500
                    }
                };
        }

        public IEnumerable<ILaptop> GetLaptops() => laptops;

        public void AddLaptop(ILaptop laptop)
        {
            laptop.Id = laptops.Any() ? laptops.Max(l => l.Id) + 1 : 1;
            laptops.Add(laptop);
        }

        public void UpdateLaptop(ILaptop laptop)
        {
            var existing = laptops.FirstOrDefault(l => l.Id == laptop.Id);
            if (existing != null)
            {
                existing.Model = laptop.Model;
                existing.Producer = laptop.Producer;
                existing.Type = laptop.Type;
                existing.Price = laptop.Price;
            }
        }

        public void DeleteLaptop(int id)
        {
            var laptop = laptops.FirstOrDefault(l => l.Id == id);
            if (laptop != null)
            {
                laptops.Remove(laptop);
            }
        }

        public ILaptop GetLaptopById(int id) => laptops.FirstOrDefault(l => l.Id == id);

        public IEnumerable<ILaptop> GetLaptopsByType(LaptopType type) => laptops.Where(l => l.Type == type);

        public void AddProducer(IProducer producer)
        {
            producer.Id = producers.Any() ? producers.Max(p => p.Id) + 1 : 1;
            producers.Add(producer);
        }

        public void DeleteProducer(int id)
        {
            var producer = producers.FirstOrDefault(p => p.Id == id);
            if (producer != null)
            {
                producers.Remove(producer);
                foreach (var laptop in laptops.Where(l => l.Producer.Id == id))
                {
                    laptop.Producer = null;
                }
            }
        }

        public IEnumerable<IProducer> GetProducers() => producers;
    }
}
