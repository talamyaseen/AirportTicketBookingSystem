using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportTicketBookingSystem.Models
{
    public class Flight
    {
        [Required]
        public string FlightNumber { get; set; }

        [Required]
        public string DepartureCountry { get; set; }

        [Required]
        public string DestinationCountry { get; set; }

        [Required]
        public string DepartureAirport { get; set; }

        [Required]
        public string ArrivalAirport { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DepartureDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal BasePrice { get; set; }

        public int EconomySeats { get; set; }
        public int BusinessSeats { get; set; }
        public int FirstClassSeats { get; set; }
    }
}
