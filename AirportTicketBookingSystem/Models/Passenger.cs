using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportTicketBookingSystem.Models
{
    public class Passenger
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
