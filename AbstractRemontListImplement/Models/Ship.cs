﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractRemontListImplement.Models
{
    /// <summary>
    /// Изделие, изготавливаемое в кондитерской
    /// </summary>
    public class Ship
    {
        public int Id { get; set; }
        public string ShipName { get; set; }
        public decimal Price { get; set; }
    }
}
