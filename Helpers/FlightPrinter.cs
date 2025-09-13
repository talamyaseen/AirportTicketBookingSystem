using System;
using System.Collections.Generic;
using AirportTicketBookingSystem.Models;

namespace AirportTicketBookingSystem.Helpers
{
    public static class FlightPrinter
    {
        public static void PrintFlights(List<Flight> flights)
        {
            Console.WriteLine($"\nSuccessfully imported {flights.Count} flight(s):\n");
            foreach (var flight in flights)
            {
                Console.WriteLine(
                    $"Flight {flight.FlightNumber} from {flight.DepartureCountry} to {flight.DestinationCountry} on {flight.DepartureDate}"
                );
            }
        }
    }
}
