using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AirportTicketBookingSystem.Services
{
    public class BookingService
    {
        private readonly List<Booking> _bookings;

        public BookingService(List<Booking> bookings)
        {
            _bookings = bookings;
        }

        public IEnumerable<Booking> FilterBookings(
            string? passengerName = null,
            string? flightNumber = null,
            string? departureCountry = null,
            string? destinationCountry = null,
            string? departureAirport = null,
            string? arrivalAirport = null,
            DateTime? departureDate = null,
            FlightClass? flightClass = null,
            decimal? maxPrice = null
        )
        {
            var filteredBookings = _bookings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(passengerName))
                filteredBookings = filteredBookings.Where(b => b.Passenger.Name.Contains(passengerName, StringComparison.OrdinalIgnoreCase));

            filteredBookings = ApplyFilter(filteredBookings, b => flightNumber, b => b.Flight.FlightNumber);
            filteredBookings = ApplyFilter(filteredBookings, b => departureCountry, b => b.Flight.DepartureCountry);
            filteredBookings = ApplyFilter(filteredBookings, b => destinationCountry, b => b.Flight.DestinationCountry);
            filteredBookings = ApplyFilter(filteredBookings, b => departureAirport, b => b.Flight.DepartureAirport);
            filteredBookings = ApplyFilter(filteredBookings, b => arrivalAirport, b => b.Flight.ArrivalAirport);

            if (departureDate.HasValue)
                filteredBookings = filteredBookings.Where(b => b.Flight.DepartureDate.Date == departureDate.Value.Date);

            if (flightClass.HasValue)
                filteredBookings = filteredBookings.Where(b => b.Class == flightClass.Value);

            if (maxPrice.HasValue)
                filteredBookings = filteredBookings.Where(b => b.PricePaid <= maxPrice.Value);

            return filteredBookings.ToList();
        }
        private static IQueryable<Booking> ApplyFilter(IQueryable<Booking> source, Func<Booking, string?> getFilterValue, Func<Booking, string> getProperty)
        {
            var value = getFilterValue(source.FirstOrDefault() ?? null);
            if (!string.IsNullOrWhiteSpace(value))
                return source.Where(b => getProperty(b).Equals(value, StringComparison.OrdinalIgnoreCase));
            return source;
        }

    }
}
