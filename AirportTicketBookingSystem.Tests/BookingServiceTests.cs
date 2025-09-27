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
    public class BookingServiceTests
    {
        [Fact]
        public void CreateBooking_ReservesSeatAndPersistsBooking_usingMockedStorageAndFlightService()
        {
            // Arrange
            var flight = SampleData.CreateFlight("BK1", econSeats: 1);
            var bookingsDict = new Dictionary<string, Booking>();
            var savedRef = (Dictionary<string, Booking>?)null;

            var mockStorage = new Mock<IStorage<Dictionary<string, Booking>>>();
            mockStorage.Setup(s => s.Load()).Returns(bookingsDict);
            mockStorage
                .Setup(s => s.Save(It.IsAny<Dictionary<string, Booking>>()))
                .Callback<Dictionary<string, Booking>>(d => savedRef = d);

            var mockFlightService = new Mock<IFlightService>();
            mockFlightService.Setup(s => s.TryReserveSeat(flight, FlightClass.Economy)).Returns(true);
            mockFlightService.Setup(s => s.Save()).Verifiable();

            var bookingService = new BookingService(mockStorage.Object, mockFlightService.Object);

            var passenger = SampleData.CreatePassenger();

            // Act
            var booking = bookingService.CreateBooking(passenger, flight, FlightClass.Economy);

            // Assert
            booking.Should().NotBeNull();
            mockFlightService.Verify(s => s.TryReserveSeat(flight, FlightClass.Economy), Times.Once);
            mockFlightService.Verify(s => s.Save(), Times.Once);
            savedRef.Should().NotBeNull();
            savedRef!.Should().ContainKey(booking.BookingId);
            savedRef[booking.BookingId].Passenger.FullName.Should().Be(passenger.FullName);
        }

        [Fact]
        public void CreateBooking_ThrowsWhenNoSeats()
        {
            // Arrange
            var flight = SampleData.CreateFlight("BK2", econSeats: 0);
            var bookingsDict = new Dictionary<string, Booking>();
            var mockStorage = new Mock<IStorage<Dictionary<string, Booking>>>();
            mockStorage.Setup(s => s.Load()).Returns(bookingsDict);

            var mockFlightService = new Mock<IFlightService>();
            mockFlightService.Setup(s => s.TryReserveSeat(flight, FlightClass.Economy)).Returns(false);

            var svc = new BookingService(mockStorage.Object, mockFlightService.Object);

            // Act
            Action act = () => svc.CreateBooking(SampleData.CreatePassenger(), flight, FlightClass.Economy);

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("No seats available*");
            mockFlightService.Verify(s => s.TryReserveSeat(flight, FlightClass.Economy), Times.Once);
            mockStorage.Verify(s => s.Save(It.IsAny<Dictionary<string, Booking>>()), Times.Never);
        }

        [Fact]
        public void CancelBooking_RemovesBookingAndReleasesSeat()
        {
            // Arrange
            var flight = SampleData.CreateFlight("BK3", econSeats: 1);
            var bookingsDict = new Dictionary<string, Booking>();
            var mockStorage = new Mock<IStorage<Dictionary<string, Booking>>>();
            mockStorage.Setup(s => s.Load()).Returns(bookingsDict);

            var mockFlightService = new Mock<IFlightService>();
            mockFlightService.Setup(s => s.TryReserveSeat(flight, FlightClass.Economy))
                .Returns(() => {
                    if (flight.EconomySeats > 0) { flight.EconomySeats--; return true; }
                    return false;
                });
            mockFlightService.Setup(s => s.ReleaseSeat(flight, FlightClass.Economy))
                .Callback(() => flight.EconomySeats++);
            mockFlightService.Setup(s => s.Save()).Verifiable();

            var svc = new BookingService(mockStorage.Object, mockFlightService.Object);

            var booking = svc.CreateBooking(SampleData.CreatePassenger("p1", "P1"), flight, FlightClass.Economy);
            flight.EconomySeats.Should().Be(0);

            // Act
            var ok = svc.CancelBooking(booking.BookingId);

            // Assert
            ok.Should().BeTrue();
            mockFlightService.Verify(s => s.ReleaseSeat(flight, FlightClass.Economy), Times.Once);
            mockStorage.Verify(s => s.Save(It.IsAny<Dictionary<string, Booking>>()), Times.AtLeastOnce);
            bookingsDict.Should().BeEmpty();
            flight.EconomySeats.Should().Be(1);
        }


        [Fact]
        public void CreateBooking_UsesFlightService_Moq_VerifyCalls()
        {
            // Arrange
            var mockFlightService = new Mock<IFlightService>();
            var flight = SampleData.CreateFlight("M1", econSeats: 2);
            mockFlightService.Setup(s => s.TryReserveSeat(flight, FlightClass.Economy)).Returns(true);
            mockFlightService.Setup(s => s.Save()).Verifiable();

            var bookingsDict = new Dictionary<string, Booking>();
            var mockStorage = new Mock<IStorage<Dictionary<string, Booking>>>();
            mockStorage.Setup(s => s.Load()).Returns(bookingsDict);
            mockStorage.Setup(s => s.Save(It.IsAny<Dictionary<string, Booking>>()));

            var svc = new BookingService(mockStorage.Object, mockFlightService.Object);

            // Act
            var booking = svc.CreateBooking(SampleData.CreatePassenger(), flight, FlightClass.Economy);

            // Assert
            booking.Should().NotBeNull();
            mockFlightService.Verify(s => s.TryReserveSeat(flight, FlightClass.Economy), Times.Once);
            mockFlightService.Verify(s => s.Save(), Times.Once);
            mockStorage.Verify(s => s.Save(It.IsAny<Dictionary<string, Booking>>()), Times.Once);
        }

        [Fact]
        public void GetBookings_Filtering_WorksForName_Class_MaxPrice()
        {
            var f1 = SampleData.CreateFlight("S1", econSeats: 10);
            var f2 = SampleData.CreateFlight("S2", econSeats: 10);
            var flights = new List<Flight> { f1, f2 };

            var flightStorageMock = new Mock<IStorage<List<Flight>>>();
            flightStorageMock.Setup(s => s.Load()).Returns(flights);
            var flightService = new FlightService(flightStorageMock.Object);

            var bookingStorageDict = new Dictionary<string, Booking>();
            var bookingStorageMock = new Mock<IStorage<Dictionary<string, Booking>>>();
            bookingStorageMock.Setup(s => s.Load()).Returns(bookingStorageDict);

            var bookingService = new BookingService(bookingStorageMock.Object, flightService);

            var p1 = SampleData.CreatePassenger("p1", "Alice");
            var p2 = SampleData.CreatePassenger("p2", "Bob");

            var b1 = bookingService.CreateBooking(p1, f1, FlightClass.Economy); 
            var b2 = bookingService.CreateBooking(p2, f2, FlightClass.Business); 

            var nameResults = bookingService.GetBookings(new BookingFilter { PassengerName = "alice" }).ToList();
            nameResults.Should().HaveCount(1);
            nameResults.First().BookingId.Should().Be(b1.BookingId);

            var maxPrice = bookingService.GetBookings(new BookingFilter { MaxPrice = 150 }).ToList();
            maxPrice.Should().HaveCount(1);
            maxPrice.First().BookingId.Should().Be(b1.BookingId);

            var classResults = bookingService.GetBookings(new BookingFilter { FlightClass = FlightClass.Business }).ToList();
            classResults.Should().HaveCount(1);
            classResults.First().BookingId.Should().Be(b2.BookingId);
        }
    }
}
