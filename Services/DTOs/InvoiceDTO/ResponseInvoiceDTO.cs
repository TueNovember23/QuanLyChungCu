﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.InvoiceDTO
{
    public class ResponseInvoiceDTO
    {
        public int InvoiceId { get; set; }
        public string ApartmentCode { get; set; } = null!;
        public int Month { get; set; }
        public int Year { get; set; }
        public double ElectricityFee { get; set; }
        public double WaterFee { get; set; }
        public double ParkingFee { get; set; }
        public double ManagementFee { get; set; }
        public double TotalAmount { get; set; }
        public string Status { get; set; } = null!;
    }
}
