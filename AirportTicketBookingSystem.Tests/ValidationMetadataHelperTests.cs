using System.Linq;
using FluentAssertions;
using Xunit;
using AirportTicketBookingSystem.Helpers;

namespace AirportTicketBookingSystem.Tests
{
    public class ValidationMetadataHelperTests
    {
        [Fact]
        public void GetValidationMetadata_ReturnsRequiredForFlightFields()
        {
            var meta = ValidationMetadataHelper.GetValidationMetadata<AirportTicketBookingSystem.Models.Flight>();
            var flightNumber = meta.FirstOrDefault(m => m.FieldName == "FlightNumber");
            flightNumber.Should().NotBeNull();
            flightNumber!.Constraints.Should().Contain("Required");
        }

        [Fact]
        public void FieldValidationInfo_HasTypesAndConstraints()
        {
            var meta = ValidationMetadataHelper.GetValidationMetadata<AirportTicketBookingSystem.Models.Passenger>();
            var id = meta.FirstOrDefault(m => m.FieldName == "Id");
            id.Should().NotBeNull();
            id!.FieldType.Should().NotBeNullOrEmpty();
            id.Constraints.Should().Contain("Required");
        }
    }
}
