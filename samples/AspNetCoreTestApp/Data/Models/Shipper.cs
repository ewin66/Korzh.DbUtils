﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreTestApp.Models
{
    public class Shipper
    {
        [Column("ShipperID")]
        public int Id { get; set; }

        public string CompanyName { get; set; }

        public string Phone { get; set; }
    }
}
