﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magnuszewski.LaptopsApp.Interfaces
{
    public interface IProducer
    {
        int Id { get; set; }
        string Name { get; set; }
    }
}
