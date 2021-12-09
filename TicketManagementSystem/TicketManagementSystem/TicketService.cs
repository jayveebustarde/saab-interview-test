using System;
using System.Linq;
using EmailService;
using TicketManagementSystem.Common;
using TicketManagementSystem.Exceptions;
using TicketManagementSystem.Models;
using TicketManagementSystem.Repositories;

namespace TicketManagementSystem
{
    public class TicketService
    {
        protected readonly IEmailService _emailService;

        public TicketService() 
            : this(new EmailServiceProxy())
        {
            // attempt to implement DI but blocked by test restriction on Program.cs file
        }

        public TicketService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public int CreateTicket(string title, Priority priority, string assignedTo, string desc, DateTime createdDate, bool isPayingCustomer)
        {
            if(string.IsNullOrEmpty(title) || string.IsNullOrEmpty(desc))
                throw new InvalidTicketException(TicketConstants.InvalidTicketException);
            
            User user = GetUser(assignedTo); 
            
            // update the ticket priority based on request details
            priority = SetPriority(title, priority, createdDate);

            if (priority == Priority.High)
            {
                _emailService.SendEmailToAdministrator(title, assignedTo);
            }

            double payingCustomerPrice = priority == Priority.High ? 100 : 50;

            var ticket = new Ticket()
            {
                Title = title,
                AssignedUser = user,
                Priority = priority,
                Description = desc,
                Created = createdDate,
                PriceDollars = isPayingCustomer ? payingCustomerPrice : 0,
                AccountManager = isPayingCustomer ? GetAccountManager() : null
            };

            // Return the int id response from CreateTicket
            return TicketRepository.CreateTicket(ticket);
        }

        public void AssignTicket(int id, string username)
        {
            User user = GetUser(username, false);
            
            var ticket = TicketRepository.GetTicket(id);

            if (ticket is null)
                throw new TicketNotFoundException(string.Format(TicketConstants.TicketNotFoundException, id));
            
            ticket.AssignedUser = user;

            TicketRepository.UpdateTicket(ticket);
        }

        private User GetUser(string username, bool includeNameInException = true)
        {
            User user = null;
            using (var userRepo = new UserRepository())
            {
                user = !string.IsNullOrWhiteSpace(username) ? userRepo.GetUser(username) : null;
            }

            if (user is null)
            {
                // use different exception messages in order to follow old implementation 
                // which have slight difference in exception message value
                string exceptionName = includeNameInException ? username : string.Empty;
                throw new UnknownUserException(string.Format(TicketConstants.UnknownUserException, exceptionName));
            }

            return user;
        }

        private User GetAccountManager()
        {
            User acctManager = null;
            using (var userRepo = new UserRepository())
            {
                acctManager = userRepo.GetAccountManager();
            }
            return acctManager;
        }

        private Priority SetPriority(string title, Priority priority, DateTime createdDate)
        {
            string[] raisedKeywords = { TicketConstants.Crash, TicketConstants.Important, TicketConstants.Failure };
            bool hasRaisedKeywords = raisedKeywords.Any(title.Contains);
            bool isTicketDelayed = createdDate < DateTime.UtcNow - TimeSpan.FromHours(1);

            //Update the priority only if the ticket is delayed or has raised keywords
            if(isTicketDelayed || hasRaisedKeywords)
            {
                if (priority == Priority.Low)
                {
                    priority = Priority.Medium;
                }
                else if (priority == Priority.Medium)
                {
                    priority = Priority.High;
                }
            }
            return priority;
        }
    }
}
