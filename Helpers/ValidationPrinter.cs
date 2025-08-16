using AirportTicketBookingSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportTicketBookingSystem.Presentation
{
    public static class ValidationPrinter
    {
        public static void PrintValidationRules<T>()
        {
            Console.WriteLine("Flight Model Validation Rules:");
            var validations = ValidationMetadataHelper.GetValidationMetadata<T>();
            foreach (var fieldInfo in validations)
            {
                Console.WriteLine($"- {fieldInfo.FieldName} ({fieldInfo.FieldType}):");
                foreach (var constraint in fieldInfo.Constraints)
                {
                    Console.WriteLine($"    * {constraint}");
                }
            }
            Console.WriteLine("\n---------------------------------------------\n");
        }
    }
}
