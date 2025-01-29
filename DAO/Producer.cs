using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magnuszewski.LaptopsApp.Interfaces;

namespace Magnuszewski.LaptopsApp.DAO
{
    public class Producer : IProducer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
