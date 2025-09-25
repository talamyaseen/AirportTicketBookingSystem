using AirportTicketBookingSystem.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportTicketBookingSystem.Models
{
    public class Booking
    {
        [Required]
        public string BookingId { get; set; }

        [Required]
        public Flight Flight { get; set; }

        [Required]
        public Passenger Passenger { get; set; }

        [Required]
        public FlightClass Class { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        public decimal PricePaid { get; set; }
    }
}
