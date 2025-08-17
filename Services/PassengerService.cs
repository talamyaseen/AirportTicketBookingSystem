using System;
using System.Linq;
using AirportTicketBookingSystem.Enums;
using AirportTicketBookingSystem.Helpers;
using AirportTicketBookingSystem.Models;

namespace AirportTicketBookingSystem.Services
{
    public class PassengerService
    {
        private readonly IFlightService _flightService;
        private readonly IBookingService _bookingService;

        public PassengerService(IFlightService flightService, IBookingService bookingService)
        {
            _flightService = flightService;
            _bookingService = bookingService;
        }

        public void Start()
        {
            Console.WriteLine("\nPassenger Options:");
            while (true)
            {
                Console.WriteLine("1: Search Flights");
                Console.WriteLine("2: Book a Flight");
                Console.WriteLine("3: View My Bookings");
                Console.WriteLine("4: Cancel a Booking");
                Console.WriteLine("0: Back");
                Console.Write("Select: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": SearchFlights(); break;
                    case "2": BookFlight(); break;
                    case "3": ViewMyBookings(); break;
                    case "4": CancelBooking(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
        }

        private void SearchFlights()
        {
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
            DateTime? date = null; if (DateTime.TryParse(Console.ReadLine(), out var d)) date = d;

            var flights = _flightService.Search(flightNumber, departureCountry, destinationCountry, departureAirport, arrivalAirport, date);
            if (!flights.Any()) Console.WriteLine("No flights found.");
            else FlightPrinter.PrintFlights(flights.ToList());
        }

        private void BookFlight()
        {
            // passenger info
            Console.Write("Passenger ID: "); var pid = Console.ReadLine() ?? string.Empty;
            Console.Write("Full Name: "); var pname = Console.ReadLine() ?? string.Empty;
            Console.Write("Email (optional): "); var email = Console.ReadLine();
            Console.Write("Phone (optional): "); var phone = Console.ReadLine();
            var passenger = new Passenger { Id = pid, FullName = pname, Email = email, PhoneNumber = phone };

            Console.Write("Enter Flight Number: ");
            var flightNumber = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(flightNumber)) { Console.WriteLine("Flight number is required."); return; }
            var flight = _flightService.GetByNumber(flightNumber);
            if (flight is null) { Console.WriteLine("Flight not found."); return; }

            Console.Write("Class (Economy, Business, FirstClass): ");
            if (!Enum.TryParse<FlightClass>(Console.ReadLine(), true, out var cls)) { Console.WriteLine("Invalid class."); return; }

            try
            {
                var booking = _bookingService.CreateBooking(passenger, flight, cls);
                Console.WriteLine($"\nBooking Confirmed!\nBookingId: {booking.BookingId}\nPassenger: {booking.Passenger.FullName}\nFlight: {booking.Flight.FlightNumber}\nClass: {booking.Class}\nPaid: {booking.PricePaid:C}\nDate: {booking.BookingDate:yyyy-MM-dd HH:mm}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Booking failed: {ex.Message}");
            }
        }

        private void ViewMyBookings()
        {
            Console.Write("Passenger ID: ");
            var pid = Console.ReadLine() ?? string.Empty;
            var my = _bookingService.ForPassenger(pid).ToList();
            if (!my.Any()) Console.WriteLine("No bookings.");
            else BookingPrinter.PrintBookings(my);
        }

        private void CancelBooking()
        {
            Console.Write("Enter BookingId: ");
            var id = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(id)) { Console.WriteLine("BookingId required."); return; }
            var ok = _bookingService.CancelBooking(id);
            Console.WriteLine(ok ? "Booking cancelled." : "Booking not found.");
        }
    }
}
