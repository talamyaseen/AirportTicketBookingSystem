using System;
using System.Collections.Generic;
using System.IO;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Storage;
using FluentAssertions;
using Xunit;

namespace AirportTicketBookingSystem.Tests
{
    public class JsonStorageTests : IDisposable
    {
        private readonly string _file;
        public JsonStorageTests()
        {
            _file = Path.Combine(Path.GetTempPath(), $"storage_{Guid.NewGuid():N}.json");
            if (File.Exists(_file)) File.Delete(_file);
        }

        [Fact]
        public void SaveAndLoad_WorksForListOfFlights()
        {
            var flights = new List<Flight> { SampleData.CreateFlight("J1") };
            var storage = new JsonStorage<List<Flight>>(_file, new List<Flight>());

            storage.Save(flights);
            var loaded = storage.Load();
            loaded.Should().HaveCount(1);
            loaded[0].FlightNumber.Should().Be("J1");
        }

        [Fact]
        public void Load_NonExistingFile_ReturnsDefault()
        {
            var defaultList = new List<Flight>();
            var storage = new JsonStorage<List<Flight>>(_file, defaultList);
            var loaded = storage.Load();
            loaded.Should().BeSameAs(defaultList);
        }

        public void Dispose()
        {
            if (File.Exists(_file)) File.Delete(_file);
        }
    }
}
