using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.AccountDTO
{
    public class CreateAccountDTO
    {
        public int AccountId { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Role { get; set; } = null!;
    }
}
