using System;

namespace AirportTicketBookingSystem.Helpers
{
    public static class ValidationPrinter
    {
        public static void PrintValidationRules<T>(string modelName)
        {
            Console.WriteLine($"{modelName} Validation Rules:");
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