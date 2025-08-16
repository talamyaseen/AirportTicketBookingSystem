using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Services;
using AirportTicketBookingSystem.Helpers;

namespace AirportTicketBookingSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Welcome to the Airport Ticket Booking System (Manager Mode)\n");

            ValidationPrinter.PrintValidationRules<Flight>();

            Console.Write("Enter the path to the CSV file (or just the file name if it's in the same folder): ");
            var filePath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("File path is required.");
                return;
            }

            var importer = new CsvFlightImporter();
            var (flights, errors) = importer.ImportFlightsFromCsv(filePath);

            FlightPrinter.PrintErrors(errors);

            if (flights.Any())
            {
                FlightPrinter.PrintFlights(flights);
                Console.WriteLine("\nSystem ready. You can now continue with booking, searching, etc...");
            }
            else
            {
                Console.WriteLine("No valid flights were imported.");
            }
        }
    }
}
