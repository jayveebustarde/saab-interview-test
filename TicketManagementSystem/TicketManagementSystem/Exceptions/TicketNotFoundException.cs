using System;

namespace TicketManagementSystem.Exceptions
{
    public class TicketNotFoundException : Exception
    {
        public TicketNotFoundException(string message) : base(message)
        {

        }
    }
}
