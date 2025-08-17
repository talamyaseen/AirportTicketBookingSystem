
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportTicketBookingSystem.Enums
{
    public enum FlightClass
    {
        [Display(Name = "Economy Class")]
        Economy,

        [Display(Name = "Business Class")]
        Business,

        [Display(Name = "First Class")]
        FirstClass
    }
}

