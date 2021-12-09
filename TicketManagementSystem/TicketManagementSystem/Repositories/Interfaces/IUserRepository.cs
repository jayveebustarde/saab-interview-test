using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketManagementSystem.Models;

namespace TicketManagementSystem.Repositories.Interfaces
{
    public interface IUserRepository : IDisposable
    {
        User GetUser(string username);
        User GetAccountManager();
    }
}
