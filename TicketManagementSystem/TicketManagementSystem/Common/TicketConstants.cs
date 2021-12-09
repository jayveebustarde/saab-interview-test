namespace TicketManagementSystem.Common
{
    internal static class TicketConstants
    {
        internal const string InvalidTicketException = "Title or description were null";
        internal const string TicketNotFoundException = "No ticket found for id {0}";
        internal const string UnknownUserException = "User {0} not found";

        internal const string Crash = "Crash";
        internal const string Important = "Important";
        internal const string Failure = "Failure";
    }
}
