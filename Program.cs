using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Services;
using System;
using System.Collections.Generic;

namespace AirportTicketBookingSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Welcome to the Airport Ticket Booking System\n");

            var bookings = new List<Booking>(); // Stored in-memory for now

            Console.WriteLine("Select mode:");
            Console.WriteLine("1: Manager");
            Console.WriteLine("2: Passenger");
            Console.Write("Choice: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    var manager = new ManagerService(bookings);
                    manager.Start();
                    break;
                case "2":
                    Console.WriteLine("Passenger mode not implemented yet.");
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }
}
