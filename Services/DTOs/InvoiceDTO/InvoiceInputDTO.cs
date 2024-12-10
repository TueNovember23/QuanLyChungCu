using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.InvoiceDTO
{
    public class InvoiceInputDTO
    {
        public string ApartmentCode { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int NumberOfPeople { get; set; }
        public double ApartmentArea { get; set; }
        public double Price { get; set; }
    }

}
