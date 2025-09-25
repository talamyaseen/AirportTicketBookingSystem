using AirportTicketBookingSystem.Enums;
using AirportTicketBookingSystem.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AirportTicketBookingSystem.Extensions
{
    public static class FlightClassExtensions
    {
        public static decimal GetPrice(this FlightClass flightClass, Flight flight)
        {
            return flightClass switch
            {
                FlightClass.Economy => flight.EconomyPrice,
                FlightClass.Business => flight.BusinessPrice,
                FlightClass.FirstClass => flight.FirstClassPrice,
                _ => throw new ArgumentOutOfRangeException(nameof(flightClass), "Invalid flight class")
            };
        }

        public static string? GetDisplayName(this FlightClass flightClass)
        {
            var type = flightClass.GetType();
            var memInfo = type.GetMember(flightClass.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
            return attributes.Length > 0 ? ((DisplayAttribute)attributes[0]).Name : flightClass.ToString();
        }
    }
}
