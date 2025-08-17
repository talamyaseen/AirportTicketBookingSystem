using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using AirportTicketBookingSystem.Models;

namespace AirportTicketBookingSystem.Services
{
    public class CsvFlightImporter
    {
        public (List<Flight> ValidFlights, List<string> Errors) ImportFlightsFromCsv(string filePath)
        {
            var flights = new List<Flight>();
            var errors = new List<string>();

            if (!File.Exists(filePath))
            {
                errors.Add("File not found: " + filePath);
                return (flights, errors);
            }

            var lines = File.ReadAllLines(filePath);
            if (lines.Length <= 1)
            {
                errors.Add("CSV file is empty or missing headers.");
                return (flights, errors);
            }

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var columns = line.Split(',');

                if (columns.Length != 12)
                {
                    errors.Add($"Invalid data format at line {i + 1}");
                    continue;
                }

                try
                {
                    var flight = new Flight
                    {
                        FlightNumber = columns[0].Trim(),
                        DepartureCountry = columns[1].Trim(),
                        DestinationCountry = columns[2].Trim(),
                        DepartureAirport = columns[3].Trim(),
                        ArrivalAirport = columns[4].Trim(),
                        DepartureDate = DateTime.Parse(columns[5].Trim(), CultureInfo.InvariantCulture),
                        EconomyPrice = decimal.Parse(columns[6].Trim()),
                        BusinessPrice = decimal.Parse(columns[7].Trim()),
                        FirstClassPrice = decimal.Parse(columns[8].Trim()),
                        EconomySeats = int.Parse(columns[9].Trim()),
                        BusinessSeats = int.Parse(columns[10].Trim()),
                        FirstClassSeats = int.Parse(columns[11].Trim())
                    };

                    var context = new ValidationContext(flight);
                    var results = new List<ValidationResult>();

                    if (!Validator.TryValidateObject(flight, context, results, true))
                    {
                        foreach (var error in results)
                        {
                            errors.Add($"Flight {flight.FlightNumber} is invalid: {error.ErrorMessage}");
                        }
                        continue;
                    }

                    flights.Add(flight);
                }
                catch (Exception ex)
                {
                    errors.Add($"Error parsing line {i + 1}: {ex.Message}");
                }
            }

            return (flights, errors);
        }
    }
}
