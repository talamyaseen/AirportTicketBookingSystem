using System;
using System.Collections.Generic;
using System.Linq;
using AirportTicketBookingSystem.Enums;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Storage;

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

    public class FlightService : IFlightService
    {
        private readonly IStorage<List<Flight>> _storage;
        private readonly List<Flight> _flights;
        private readonly Dictionary<string, Flight> _byNumber;

        public FlightService(IStorage<List<Flight>> storage)
        {
            _storage = storage;
            _flights = storage.Load();
            _byNumber = _flights
                .GroupBy(f => f.FlightNumber)
                .ToDictionary(g => g.Key, g => g.First());
        }

        public IEnumerable<Flight> All() => _flights;

        public IEnumerable<Flight> Search(string? flightNumber = null, string? departureCountry = null, string? destinationCountry = null, string? departureAirport = null, string? arrivalAirport = null, DateTime? departureDate = null)
        {
            var q = _flights.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(flightNumber)) q = q.Where(f => f.FlightNumber.Equals(flightNumber, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(departureCountry)) q = q.Where(f => f.DepartureCountry.Equals(departureCountry, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(destinationCountry)) q = q.Where(f => f.DestinationCountry.Equals(destinationCountry, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(departureAirport)) q = q.Where(f => f.DepartureAirport.Equals(departureAirport, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(arrivalAirport)) q = q.Where(f => f.ArrivalAirport.Equals(arrivalAirport, StringComparison.OrdinalIgnoreCase));
            if (departureDate.HasValue) q = q.Where(f => f.DepartureDate.Date == departureDate.Value.Date);
            return q.ToList();
        }

        public Flight? GetByNumber(string flightNumber) => _byNumber.TryGetValue(flightNumber, out var f) ? f : null;

        public bool TryReserveSeat(Flight flight, FlightClass @class)
        {
            switch (@class)
            {
                case FlightClass.Economy:
                    if (flight.EconomySeats <= 0) return false;
                    flight.EconomySeats--; return true;
                case FlightClass.Business:
                    if (flight.BusinessSeats <= 0) return false;
                    flight.BusinessSeats--; return true;
                case FlightClass.FirstClass:
                    if (flight.FirstClassSeats <= 0) return false;
                    flight.FirstClassSeats--; return true;
                default: return false;
            }
        }

        public void ReleaseSeat(Flight flight, FlightClass @class)
        {
            switch (@class)
            {
                case FlightClass.Economy: flight.EconomySeats++; break;
                case FlightClass.Business: flight.BusinessSeats++; break;
                case FlightClass.FirstClass: flight.FirstClassSeats++; break;
            }
        }

        public void AddFlights(IEnumerable<Flight> flights)
        {
            foreach (var f in flights)
            {
                if (_byNumber.ContainsKey(f.FlightNumber))
                {
                    _byNumber[f.FlightNumber] = f; // replace existing
                    var idx = _flights.FindIndex(x => x.FlightNumber.Equals(f.FlightNumber, StringComparison.OrdinalIgnoreCase));
                    if (idx >= 0) _flights[idx] = f;
                }
                else
                {
                    _flights.Add(f);
                    _byNumber[f.FlightNumber] = f;
                }
            }
        }

        public void Save() => _storage.Save(_flights);
    }
}
