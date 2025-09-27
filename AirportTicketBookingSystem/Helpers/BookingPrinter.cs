using AirportTicketBookingSystem.Models;
using System;
using System.Collections.Generic;

namespace AirportTicketBookingSystem.Helpers
{
    public static class BookingPrinter
    {
        public static void PrintBookings(IEnumerable<Booking> bookings)
        {
            Console.WriteLine($"\nFound {bookings.Count()} booking(s):\n");
            foreach (var b in bookings)
            {
                Console.WriteLine($"BookingId: {b.BookingId}, Passenger: {b.Passenger.FullName}, Flight: {b.Flight.FlightNumber}, Class: {b.Class}, PricePaid: {b.PricePaid:C}, BookingDate: {b.BookingDate}");
            }
        }
    }
}
