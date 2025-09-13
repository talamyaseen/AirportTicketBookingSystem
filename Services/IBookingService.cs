using System;
using System.Collections.Generic;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Enums;

namespace AirportTicketBookingSystem.Services
{
    public interface IBookingService
    {
        IEnumerable<Booking> GetBookings(BookingFilter? filter = null);
        Booking CreateBooking(Passenger passenger, Flight flight, FlightClass @class);
        bool CancelBooking(string bookingId);
        void Save();
    }
}