using System;
using System.Collections.Generic;

namespace AirportTicketBookingSystem.Helpers
{
    public static class ConsolePrinter
    {
        public static void PrintErrors(List<string> errors)
        {
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
        }
    }
}
