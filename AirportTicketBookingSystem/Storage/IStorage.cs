using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportTicketBookingSystem.Storage
{
    public interface IStorage<T>
    {
        T Load();
        void Save(T data);
    }
}