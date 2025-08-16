using AirportTicketBookingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportTicketBookingSystem.Presentation
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

        public static void PrintErrors(List<string> errors)
        {
            foreach (var error in errors)
            {
                Console.WriteLine($"{error}");
            }
        }
    }
}
