using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq; 
using CsvHelper;
using CsvHelper.Configuration;
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

            IEnumerable<Flight> records;
            try
            {
                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    IgnoreBlankLines = true,
                    TrimOptions = TrimOptions.Trim
                });

                records = csv.GetRecords<Flight>().ToList(); 
            }
            catch (Exception ex)
            {
                errors.Add($"Error reading CSV: {ex.Message}");
                return (flights, errors);
            }

            foreach (var flight in records)
            {
                var context = new ValidationContext(flight);
                var results = new List<ValidationResult>();
                if (!Validator.TryValidateObject(flight, context, results, true))
                {
                    var groupedErrors = results.GroupBy(r => flight.FlightNumber)
                        .Select(g => $"Flight {g.Key} is invalid: {string.Join("; ", g.Select(r => r.ErrorMessage))}");
                    errors.AddRange(groupedErrors);
                    continue;
                }

                flights.Add(flight);
            }

            return (flights, errors);
        }
    }
}
