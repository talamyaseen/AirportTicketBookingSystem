using System;
using System.Linq;
using AirportTicketBookingSystem.Enums;
using AirportTicketBookingSystem.Helpers;

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
                        ValidationPrinter.PrintValidationRules<AirportTicketBookingSystem.Models.Flight>("Flight");
                        LoadFlightsFromCsv();
                        break;
                    case "2":
                        FilterBookings();
                        break;
                    case "3":
                        Helpers.FlightPrinter.PrintFlights(_flightService.All().ToList());
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

            Helpers.FlightPrinter.PrintErrors(errors);

            if (newFlights.Any())
            {
                _flightService.AddFlights(newFlights);
                _flightService.Save();
                Helpers.FlightPrinter.PrintFlights(newFlights);
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
                Helpers.BookingPrinter.PrintBookings(results);
        }
    }
}