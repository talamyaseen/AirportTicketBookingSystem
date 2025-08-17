using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
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

            try
            {
                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    IgnoreBlankLines = true,
                    TrimOptions = TrimOptions.Trim
                });

                var records = csv.GetRecords<Flight>();
                foreach (var flight in records)
                {
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
            }
            catch (Exception ex)
            {
                errors.Add($"Error reading CSV: {ex.Message}");
            }

            return (flights, errors);
        }
    }
}
