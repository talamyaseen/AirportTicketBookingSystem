using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Services;
using AirportTicketBookingSystem.Helpers;
using AirportTicketBookingSystem.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AirportTicketBookingSystem.Services
{
    public class ManagerService
    {
        private List<Flight> _flights;
        private BookingService _bookingService;

        public ManagerService(List<Booking> bookings)
        {
            _flights = FlightStorage.LoadFlights();
            _bookingService = new BookingService(bookings);
        }

        public void Start()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ValidationPrinter.PrintValidationRules<Flight>();

            while (true)
            {
                Console.WriteLine("\nManager Options:");
                Console.WriteLine("1: Load Flights from CSV");
                Console.WriteLine("2: Filter Bookings");
                Console.WriteLine("0: Exit");
                Console.Write("Select an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        LoadFlightsFromCsv();
                        break;
                    case "2":
                        FilterBookings();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        private void LoadFlightsFromCsv()
        {
            Console.Write("Enter CSV file path: ");
            var filePath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("File path is required.");
                return;
            }

            var importer = new CsvFlightImporter();
            var (newFlights, errors) = importer.ImportFlightsFromCsv(filePath);

            FlightPrinter.PrintErrors(errors);

            if (newFlights.Any())
            {
                _flights.AddRange(newFlights); // Append new flights
                FlightPrinter.PrintFlights(newFlights);
                FlightStorage.SaveFlights(_flights); // Save all flights
                Console.WriteLine("Flights loaded and saved successfully.");
            }
            else
            {
                Console.WriteLine("No valid flights imported.");
            }
        }

        private void FilterBookings()
        {
            Console.WriteLine("\nEnter filter parameters (leave blank to skip):");

            Console.Write("Passenger Name: ");
            var passengerName = Console.ReadLine();

            Console.Write("Flight Number: ");
            var flightNumber = Console.ReadLine();

            Console.Write("Departure Country: ");
            var departureCountry = Console.ReadLine();

            Console.Write("Destination Country: ");
            var destinationCountry = Console.ReadLine();

            Console.Write("Departure Airport: ");
            var departureAirport = Console.ReadLine();

            Console.Write("Arrival Airport: ");
            var arrivalAirport = Console.ReadLine();

            Console.Write("Departure Date (yyyy-MM-dd): ");
            DateTime? departureDate = null;
            if (DateTime.TryParse(Console.ReadLine(), out var dt))
                departureDate = dt;

            Console.Write("Flight Class (Economy, Business, FirstClass): ");
            FlightClass? flightClass = null;
            if (Enum.TryParse<FlightClass>(Console.ReadLine(), true, out var fc))
                flightClass = fc;

            Console.Write("Max Price: ");
            decimal? maxPrice = null;
            if (decimal.TryParse(Console.ReadLine(), out var price))
                maxPrice = price;

            var results = _bookingService.FilterBookings(
                passengerName,
                flightNumber,
                departureCountry,
                destinationCountry,
                departureAirport,
                arrivalAirport,
                departureDate,
                flightClass,
                maxPrice
            ).ToList();

            if (!results.Any())
                Console.WriteLine("No bookings matched your filters.");
            else
            {
                Console.WriteLine($"\nFound {results.Count} booking(s):");
                foreach (var b in results)
                {
                    Console.WriteLine($"Booking {b.BookingId}: {b.Passenger.Name}, Flight {b.Flight.FlightNumber}, Class: {b.Class}, Paid: {b.PricePaid}");
                }
            }
        }
    }
}
