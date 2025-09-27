using System;
using System.Collections.Generic;
using System.Linq;
using AirportTicketBookingSystem.Enums;
using AirportTicketBookingSystem.Models;
using AirportTicketBookingSystem.Services;
using AirportTicketBookingSystem.Storage;
using FluentAssertions;
using Moq;
using Xunit;

namespace AirportTicketBookingSystem.Tests
{
    public class FlightServiceTests
    {
        [Fact]
        public void AddFlights_AddsAndReplacesFlightsAndSaveCalled()
        {
            var f1 = SampleData.CreateFlight("F1");
            var storageMock = new Mock<IStorage<List<Flight>>>();
            var flights = new List<Flight> { f1 };
            storageMock.Setup(s => s.Load()).Returns(flights);
            storageMock.Setup(s => s.Save(It.IsAny<List<Flight>>())).Verifiable();

            var svc = new FlightService(storageMock.Object);

            var f2 = SampleData.CreateFlight("F2");
            svc.AddFlights(new[] { f2 });
            svc.All().Count().Should().Be(2);

            var f2updated = SampleData.CreateFlight("F2");
            f2updated.DepartureCountry = "NewCountry";
            svc.AddFlights(new[] { f2updated });

            svc.GetByNumber("F2").Should().NotBeNull();
            svc.GetByNumber("F2")!.DepartureCountry.Should().Be("NewCountry");

            svc.Save();
            storageMock.Verify(s => s.Save(It.IsAny<List<Flight>>()), Times.Once);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(0, false)]
        public void TryReserveSeat_Economy_BehavesCorrectly(int econSeats, bool expected)
        {
            // Arrange
            var f = SampleData.CreateFlight("T1", econSeats: econSeats);
            var storageMock = new Mock<IStorage<List<Flight>>>();
            storageMock.Setup(s => s.Load()).Returns(new List<Flight> { f });

            var svc = new FlightService(storageMock.Object);

            // Act
            var flight = svc.GetByNumber("T1")!;
            var res = svc.TryReserveSeat(flight, FlightClass.Economy);

            // Assert
            res.Should().Be(expected);
            if (expected) flight.EconomySeats.Should().Be(econSeats - 1);
            else flight.EconomySeats.Should().Be(0);
        }

        [Fact]
        public void ReleaseSeat_IncrementsAppropriateSeat()
        {
            // Arrange
            var f = SampleData.CreateFlight("T2", econSeats: 0, busSeats: 0, firstSeats: 0);
            var storageMock = new Mock<IStorage<List<Flight>>>();
            storageMock.Setup(s => s.Load()).Returns(new List<Flight> { f });

            var svc = new FlightService(storageMock.Object);
            var flight = svc.GetByNumber("T2")!;

            // Act
            svc.ReleaseSeat(flight, FlightClass.Economy);
            svc.ReleaseSeat(flight, FlightClass.Business);
            svc.ReleaseSeat(flight, FlightClass.FirstClass);

            // Assert
            flight.EconomySeats.Should().Be(1);
            flight.BusinessSeats.Should().Be(1);
            flight.FirstClassSeats.Should().Be(1);
        }

        [Fact]
        public void Search_FiltersByMultipleCriteria()
        {
            // Arrange
            var a = SampleData.CreateFlight("A1"); a.DepartureCountry = "USA";
            var b = SampleData.CreateFlight("B1"); b.DepartureCountry = "Germany"; b.DestinationCountry = "France";
            var storageMock = new Mock<IStorage<List<Flight>>>();
            storageMock.Setup(s => s.Load()).Returns(new List<Flight> { a, b });

            var svc = new FlightService(storageMock.Object);

            // Act
            var r1 = svc.Search(departureCountry: "Germany").ToList();

            // Assert
            r1.Should().HaveCount(1).And.Contain(f => f.FlightNumber == "B1");

            var date = new DateTime(2025, 12, 01);
            var r2 = svc.Search(departureDate: date).ToList();
            r2.Should().NotBeEmpty();
        }
    }
}
