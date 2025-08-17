using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AirportTicketBookingSystem.Helpers
{
    public static class ValidationMetadataHelper
    {
        public static List<FieldValidationInfo> GetValidationMetadata<T>()
        {
            var type = typeof(T);
            var props = type.GetProperties();
            var list = new List<FieldValidationInfo>();

            foreach (var prop in props)
            {
                var info = new FieldValidationInfo
                {
                    FieldName = prop.Name,
                    FieldType = prop.PropertyType.Name,
                    Constraints = new List<string>()
                };

                var attributes = prop.GetCustomAttributes(true);

                foreach (var attr in attributes)
                {
                    switch (attr)
                    {
                        case RequiredAttribute:
                            info.Constraints.Add("Required");
                            break;
                        case RangeAttribute range:
                            info.Constraints.Add($"Range: {range.Minimum} to {range.Maximum}");
                            break;
                        case StringLengthAttribute strLen:
                            info.Constraints.Add($"Max Length: {strLen.MaximumLength}");
                            break;
                        case DataTypeAttribute dt:
                            info.Constraints.Add($"Data Type: {dt.DataType}");
                            break;
                        case RegularExpressionAttribute regex:
                            info.Constraints.Add($"Regex: {regex.Pattern}");
                            break;
                    }
                }

                if (!info.Constraints.Any())
                {
                    info.Constraints.Add("No constraints");
                }

                list.Add(info);
            }

            return list;
        }
    }

    public class FieldValidationInfo
    {
        public string FieldName { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty;
        public List<string> Constraints { get; set; } = new();
    }
}
