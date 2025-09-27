using System;
using System.IO;
using AirportTicketBookingSystem.Services;
using FluentAssertions;
using Xunit;

namespace AirportTicketBookingSystem.Tests
{
    public class CsvFlightImporterTests : IDisposable
    {
        private readonly string _path;
        public CsvFlightImporterTests()
        {
            _path = Path.Combine(Path.GetTempPath(), $"flights_{Guid.NewGuid():N}.csv");
        }

        public void Dispose()
        {
            if (File.Exists(_path)) File.Delete(_path);
        }

        [Fact]
        public void ImportFlightsFromCsv_ValidCsv_ReturnsFlights()
        {
            var csv = "FlightNumber,DepartureCountry,DestinationCountry,DepartureAirport,ArrivalAirport,DepartureDate,EconomyPrice,BusinessPrice,FirstClassPrice,EconomySeats,BusinessSeats,FirstClassSeats\n" +
                      "T1,USA,UK,JFK,LHR,2025-12-01T08:00:00,300.5,600.75,1200.99,100,50,20";
            File.WriteAllText(_path, csv);

            var importer = new CsvFlightImporter();
            var (flights, errors) = importer.ImportFlightsFromCsv(_path);

            errors.Should().BeEmpty();
            flights.Should().HaveCount(1);
            flights[0].FlightNumber.Should().Be("T1");
        }

        [Fact]
        public void ImportFlightsFromCsv_InvalidPath_ReturnsError()
        {
            var importer = new CsvFlightImporter();
            var (flights, errors) = importer.ImportFlightsFromCsv("does_not_exist.csv");
            flights.Should().BeEmpty();
            errors.Should().NotBeEmpty();
        }

        [Fact]
        public void ImportFlightsFromCsv_InvalidRecord_ReportsError()
        {
            var csv = "FlightNumber,DepartureCountry,DestinationCountry,DepartureAirport,ArrivalAirport,DepartureDate,EconomyPrice,BusinessPrice,FirstClassPrice,EconomySeats,BusinessSeats,FirstClassSeats\n" +
                      ",USA,UK,JFK,LHR,2025-12-01T08:00:00,300.5,600.75,1200.99,100,50,20";
            File.WriteAllText(_path, csv);

            var importer = new CsvFlightImporter();
            var (flights, errors) = importer.ImportFlightsFromCsv(_path);

            flights.Should().BeEmpty();
            errors.Should().NotBeEmpty();
        }
    }
}
