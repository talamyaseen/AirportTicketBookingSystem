using System;
using System.Collections.Generic;
using AirportTicketBookingSystem.Enums;
using AirportTicketBookingSystem.Models;

namespace AirportTicketBookingSystem.Services
{
    public interface IFlightService
    {
        IEnumerable<Flight> Search(
            string? flightNumber = null,
            string? departureCountry = null,
            string? destinationCountry = null,
            string? departureAirport = null,
            string? arrivalAirport = null,
            DateTime? departureDate = null);

        Flight? GetByNumber(string flightNumber);
        bool TryReserveSeat(Flight flight, FlightClass @class);
        void ReleaseSeat(Flight flight, FlightClass @class);
        void AddFlights(IEnumerable<Flight> flights);
        void Save();
        IEnumerable<Flight> All();
    }
}
