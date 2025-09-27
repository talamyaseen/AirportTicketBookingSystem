using AirportTicketBookingSystem.Enums;
using AirportTicketBookingSystem.Extensions;
using FluentAssertions;
using Xunit;

namespace AirportTicketBookingSystem.Tests
{
    public class FlightClassExtensionsTests
    {
        [Fact]
        public void GetPrice_ReturnsCorrectValue()
        {
            var flight = SampleData.CreateFlight("X1");
            flight.EconomyPrice = 123m;
            flight.BusinessPrice = 456m;
            flight.FirstClassPrice = 789m;

            FlightClass.Economy.GetPrice(flight).Should().Be(123m);
            FlightClass.Business.GetPrice(flight).Should().Be(456m);
            FlightClass.FirstClass.GetPrice(flight).Should().Be(789m);
        }

        [Fact]
        public void GetDisplayName_UsesDisplayAttribute()
        {
            var e = FlightClass.Economy;
            var display = e.GetDisplayName();
            display.Should().Be("Economy Class");
        }
    }
}
