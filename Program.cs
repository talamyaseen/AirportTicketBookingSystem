using System;
using System.Collections.Generic;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Services;
using AirportTicketBookingSystem.Storage;

namespace AirportTicketBookingSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Welcome to the Airport Ticket Booking System\n");

           
            var flightStorage = new JsonStorage<List<Flight>>("flights.json", new List<Flight>());
            var bookingStorage = new JsonStorage<Dictionary<string, Booking>>("bookings.json", new Dictionary<string, Booking>());

           
            IFlightService flightService = new FlightService(flightStorage);
            IBookingService bookingService = new BookingService(bookingStorage, flightService);

            while (true)
            {
                Console.WriteLine("Select mode:");
                Console.WriteLine("1: Manager");
                Console.WriteLine("2: Passenger");
                Console.WriteLine("0: Exit");
                Console.Write("Choice: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        var manager = new ManagerService(flightService, bookingService);
                        manager.Start();
                        break;
                    case "2":
                        var passenger = new PassengerService(flightService, bookingService);
                        passenger.Start();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
    }
}
