using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Magnuszewski.LaptopsApp.DAO;
using Magnuszewski.LaptopsApp.Core;
using Magnuszewski.LaptopsApp.Interfaces;

namespace Magnuszewski.LaptopsApp.DAOFile
{
    public class FileLaptopStorage : ILaptopStorage
    {
        private readonly string filePath = @"C:\Users\student\Documents\LaptopsApp\laptops.csv";
        private List<ILaptop> laptops;
        private List<IProducer> producers;

        public FileLaptopStorage()
        {
            if (File.Exists(filePath))
            {
                var csvLines = File.ReadAllLines(filePath);
                laptops = csvLines.Skip(1).Select(line => ParseLaptop(line)).ToList();
            }
            else
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
        }

        public IEnumerable<ILaptop> GetLaptops() => laptops;

        public void AddLaptop(ILaptop laptop)
        {
            laptop.Id = laptops.Any() ? laptops.Max(l => l.Id) + 1 : 1;
            laptops.Add(laptop);
            Save();
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
                Save();
            }
        }

        public void DeleteLaptop(int id)
        {
            var laptop = laptops.FirstOrDefault(l => l.Id == id);
            if (laptop != null)
            {
                laptops.Remove(laptop);
                Save();
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
            }
        }

        public IEnumerable<IProducer> GetProducers() => producers;

        private void Save()
        {
            var csv = new StringBuilder();
            csv.AppendLine("Id,Model,ProducerId,ProducerName,Type,Price");
            foreach (var laptop in laptops)
            {
                csv.AppendLine($"{laptop.Id},{laptop.Model},{laptop.Producer.Id},{laptop.Producer.Name},{laptop.Type},{laptop.Price}");
            }
            File.WriteAllText(filePath, csv.ToString());
        }

        private ILaptop ParseLaptop(string csvLine)
        {
            var values = csvLine.Split(',');
            return new Laptop
            {
                Id = int.Parse(values[0]),
                Model = values[1],
                Producer = new Producer { Id = int.Parse(values[2]), Name = values[3] },
                Type = Enum.Parse<LaptopType>(values[4]),
                Price = double.Parse(values[5])
            };
        }
    }
}