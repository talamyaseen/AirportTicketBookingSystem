using System;
using System.Linq;
using AirportTicketBookingSystem.Enums;
using AirportTicketBookingSystem.Helpers;
using AirportTicketBookingSystem.Models;

namespace AirportTicketBookingSystem.Services
{
    public class ManagerService
    {
        private readonly IFlightService _flightService;
        private readonly IBookingService _bookingService;

        public ManagerService(IFlightService flightService, IBookingService bookingService)
        {
            _flightService = flightService;
            _bookingService = bookingService;
        }

        public void Start()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.WriteLine("\nManager Options:");
                Console.WriteLine("1: Load Flights from CSV");
                Console.WriteLine("2: Filter Bookings");
                Console.WriteLine("3: List All Flights");
                Console.WriteLine("0: Exit");
                Console.Write("Select an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ValidationPrinter.PrintValidationRules<Flight>("Flight");
                        LoadFlightsFromCsv();
                        break;
                    case "2":
                        FilterBookings();
                        break;
                    case "3":
                        FlightPrinter.PrintFlights(_flightService.All().ToList());
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

            ConsolePrinter.PrintErrors(errors);

            if (newFlights.Any())
            {
                _flightService.AddFlights(newFlights);
                _flightService.Save();
                FlightPrinter.PrintFlights(newFlights);
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

            var filter = new BookingFilter();

            Console.Write("Passenger Name: ");
            filter.PassengerName = Console.ReadLine();

            Console.Write("Flight Number: ");
            filter.FlightNumber = Console.ReadLine();

            Console.Write("Departure Country: ");
            filter.DepartureCountry = Console.ReadLine();

            Console.Write("Destination Country: ");
            filter.DestinationCountry = Console.ReadLine();

            Console.Write("Departure Airport: ");
            filter.DepartureAirport = Console.ReadLine();

            Console.Write("Arrival Airport: ");
            filter.ArrivalAirport = Console.ReadLine();

            Console.Write("Departure Date (yyyy-MM-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out var dt))
                filter.DepartureDate = dt;

            Console.Write("Flight Class (Economy, Business, FirstClass): ");
            if (Enum.TryParse<FlightClass>(Console.ReadLine(), true, out var fc))
                filter.FlightClass = fc;

            Console.Write("Max Price: ");
            if (decimal.TryParse(Console.ReadLine(), out var price))
                filter.MaxPrice = price;

            var results = _bookingService.GetBookings(filter).ToList();

            if (!results.Any())
                Console.WriteLine("No bookings matched your filters.");
            else
                BookingPrinter.PrintBookings(results);
        }
    }
}
