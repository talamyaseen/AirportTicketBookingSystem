using AirportTicketBookingSystem.Models;
using System.Text.Json;

namespace AirportTicketBookingSystem.Helpers
{
    public static class FlightStorage
    {
        private const string FileName = "flights.json";

        public static List<Flight> LoadFlights()
        {
            if (!File.Exists(FileName)) return new List<Flight>();

            var json = File.ReadAllText(FileName);
            return JsonSerializer.Deserialize<List<Flight>>(json) ?? new List<Flight>();
        }

        public static void SaveFlights(List<Flight> flights)
        {
            var json = JsonSerializer.Serialize(flights, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FileName, json);
        }
    }
}
