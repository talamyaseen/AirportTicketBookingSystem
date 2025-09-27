using AirportTicketBookingSystem.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportTicketBookingSystem.Services
{
    public class BookingFilter
    {
        public string? PassengerName { get; set; }
        public string? PassengerId { get; set; }
        public string? FlightNumber { get; set; }
        public string? DepartureCountry { get; set; }
        public string? DestinationCountry { get; set; }
        public string? DepartureAirport { get; set; }
        public string? ArrivalAirport { get; set; }
        public DateTime? DepartureDate { get; set; }
        public FlightClass? FlightClass { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
