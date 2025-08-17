using AirportTicketBookingSystem.Models;
using System.Text.Json;

namespace AirportTicketBookingSystem.Helpers
{
    public static class BookingStorage
    {
        private const string FileName = "bookings.json";

        public static List<Booking> LoadBookings()
        {
            if (!File.Exists(FileName)) return new List<Booking>();

            var json = File.ReadAllText(FileName);
            return JsonSerializer.Deserialize<List<Booking>>(json) ?? new List<Booking>();
        }

        public static void SaveBookings(List<Booking> bookings)
        {
            var json = JsonSerializer.Serialize(bookings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FileName, json);
        }
    }
}
