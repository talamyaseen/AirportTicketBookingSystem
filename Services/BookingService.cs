using System;
using System.Collections.Generic;
using System.Linq;
using AirportTicketBookingSystem.Enums;
using AirportTicketBookingSystem.Extensions;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Storage;

namespace AirportTicketBookingSystem.Services
{
    public interface IBookingService
    {
        IEnumerable<Booking> FilterBookings(
            string? passengerName = null,
            string? flightNumber = null,
            string? departureCountry = null,
            string? destinationCountry = null,
            string? departureAirport = null,
            string? arrivalAirport = null,
            DateTime? departureDate = null,
            FlightClass? flightClass = null,
            decimal? maxPrice = null);

        Booking CreateBooking(Passenger passenger, Flight flight, FlightClass @class);
        bool CancelBooking(string bookingId);
        IEnumerable<Booking> All();
        IEnumerable<Booking> ForPassenger(string passengerId);
        void Save();
    }

    public class BookingService : IBookingService
    {
        private readonly IStorage<Dictionary<string, Booking>> _storage;
        private readonly Dictionary<string, Booking> _bookings;
        private readonly IFlightService _flightService;

        public BookingService(IStorage<Dictionary<string, Booking>> storage, IFlightService flightService)
        {
            _storage = storage;
            _bookings = storage.Load();
            _flightService = flightService;
        }

        public IEnumerable<Booking> All() => _bookings.Values;
        public IEnumerable<Booking> ForPassenger(string passengerId) => _bookings.Values.Where(b => b.Passenger.Id.Equals(passengerId, StringComparison.OrdinalIgnoreCase));

        public IEnumerable<Booking> FilterBookings(string? passengerName = null, string? flightNumber = null, string? departureCountry = null, string? destinationCountry = null, string? departureAirport = null, string? arrivalAirport = null, DateTime? departureDate = null, FlightClass? flightClass = null, decimal? maxPrice = null)
        {
            var q = _bookings.Values.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(passengerName))
                q = q.Where(b => b.Passenger.FullName.Contains(passengerName, StringComparison.OrdinalIgnoreCase));

            q = ApplyFilter(q, flightNumber, b => b.Flight.FlightNumber);
            q = ApplyFilter(q, departureCountry, b => b.Flight.DepartureCountry);
            q = ApplyFilter(q, destinationCountry, b => b.Flight.DestinationCountry);
            q = ApplyFilter(q, departureAirport, b => b.Flight.DepartureAirport);
            q = ApplyFilter(q, arrivalAirport, b => b.Flight.ArrivalAirport);

            if (departureDate.HasValue)
                q = q.Where(b => b.Flight.DepartureDate.Date == departureDate.Value.Date);

            if (flightClass.HasValue)
                q = q.Where(b => b.Class == flightClass.Value);

            if (maxPrice.HasValue)
                q = q.Where(b => b.PricePaid <= maxPrice.Value);

            return q.ToList();
        }

        private static IEnumerable<Booking> ApplyFilter(IEnumerable<Booking> source, string? filterValue, Func<Booking, string> selector)
        {
            if (string.IsNullOrWhiteSpace(filterValue)) return source;
            return source.Where(b => selector(b).Equals(filterValue, StringComparison.OrdinalIgnoreCase));
        }

        public Booking CreateBooking(Passenger passenger, Flight flight, FlightClass @class)
        {
            if (!_flightService.TryReserveSeat(flight, @class))
                throw new InvalidOperationException("No seats available in selected class.");

            var price = @class.GetPrice(flight);
            var booking = new Booking
            {
                BookingId = Guid.NewGuid().ToString("N"),
                Passenger = passenger,
                Flight = flight,
                Class = @class,
                BookingDate = DateTime.UtcNow,
                PricePaid = price
            };

            _bookings[booking.BookingId] = booking;
            Save();
            _flightService.Save();
            return booking;
        }

        public bool CancelBooking(string bookingId)
        {
            if (_bookings.TryGetValue(bookingId, out var booking))
            {
                _bookings.Remove(bookingId);
                _flightService.ReleaseSeat(booking.Flight, booking.Class);
                Save();
                _flightService.Save();
                return true;
            }
            return false;
        }

        public void Save() => _storage.Save(_bookings);
    }
}
