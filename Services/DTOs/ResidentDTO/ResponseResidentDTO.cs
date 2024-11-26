using Repositories.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.ResidentDTO
{
    public class ResponseResidentDTO
    {
        public string ResidentId { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? RelationShipWithOwner { get; set; }

        public DateOnly? MoveInDate { get; set; }

        public string Apartment { get; set; } = "";
    }
}
