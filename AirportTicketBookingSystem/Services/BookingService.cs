using AirportTicketBookingSystem.Enums;
using AirportTicketBookingSystem.Extensions;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AirportTicketBookingSystem.Services
{
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

        public IEnumerable<Booking> GetBookings(BookingFilter? filter = null)
        {
            var q = _bookings.Values.AsEnumerable();

            if (filter == null) return q;

            if (!string.IsNullOrWhiteSpace(filter.PassengerName))
                q = q.Where(b => b.Passenger.FullName.Contains(filter.PassengerName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(filter.PassengerId))
                q = q.Where(b => b.Passenger.Id.Equals(filter.PassengerId, StringComparison.OrdinalIgnoreCase));

            q = ApplyFilter(q, filter.FlightNumber, b => b.Flight.FlightNumber);
            q = ApplyFilter(q, filter.DepartureCountry, b => b.Flight.DepartureCountry);
            q = ApplyFilter(q, filter.DestinationCountry, b => b.Flight.DestinationCountry);
            q = ApplyFilter(q, filter.DepartureAirport, b => b.Flight.DepartureAirport);
            q = ApplyFilter(q, filter.ArrivalAirport, b => b.Flight.ArrivalAirport);

            if (filter.DepartureDate.HasValue)
                q = q.Where(b => b.Flight.DepartureDate.Date == filter.DepartureDate.Value.Date);

            if (filter.FlightClass.HasValue)
                q = q.Where(b => b.Class == filter.FlightClass.Value);

            if (filter.MaxPrice.HasValue)
                q = q.Where(b => b.PricePaid <= filter.MaxPrice.Value);

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
