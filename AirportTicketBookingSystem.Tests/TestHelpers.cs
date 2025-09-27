using System;
using System.Collections.Generic;
using AirportTicketBookingSystem.Storage;
using AirportTicketBookingSystem.Models;

namespace AirportTicketBookingSystem.Tests
{
    public class FakeStorage<T> : IStorage<T>
    {
        private T _data;
        public FakeStorage(T initial) => _data = initial;
        public T Load() => _data;
        public void Save(T data) => _data = data;
        public T Current => _data;
    }

    public static class SampleData
    {
        public static Flight CreateFlight(string num = "F001",
            int econSeats = 2, int busSeats = 1, int firstSeats = 0)
        {
            return new Flight
            {
                FlightNumber = num,
                DepartureCountry = "CountryA",
                DestinationCountry = "CountryB",
                DepartureAirport = "AAA",
                ArrivalAirport = "BBB",
                DepartureDate = new DateTime(2025, 12, 01, 8, 0, 0),
                EconomyPrice = 100m,
                BusinessPrice = 300m,
                FirstClassPrice = 1000m,
                EconomySeats = econSeats,
                BusinessSeats = busSeats,
                FirstClassSeats = firstSeats
            };
        }

        public static Passenger CreatePassenger(string id = "P1", string name = "Test User")
            => new Passenger { Id = id, FullName = name, Email = "a@a.com", PhoneNumber = "123" };
    }
}
