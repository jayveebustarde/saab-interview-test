﻿using System;
using System.Data.SqlClient;
using TicketManagementSystem.Models;
using TicketManagementSystem.Repositories.Interfaces;

namespace TicketManagementSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private bool _disposed;

        public User GetUser(string username)
        {
            // Assume this method does not need to change and is connected to a database with users populated.
            try
            {
                var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["database"].ConnectionString;

                string sql = "SELECT TOP 1 FROM Users u WHERE u.Username == @p1";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var command = new SqlCommand(sql, connection)
                    {
                        CommandType = System.Data.CommandType.Text,
                    };

                    command.Parameters.Add("@p1", System.Data.SqlDbType.NVarChar).Value = username;

                    return (User)command.ExecuteScalar();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public User GetAccountManager()
        {
            // Assume this method does not need to change.
            return GetUser("Sarah");
        }

        public void Dispose()
        {
            // Free up resources
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // dispose managed state (managed objects).
            }

            //  free unmanaged resources (unmanaged objects) and override a finalizer below.
            //  set large fields to null.

            _disposed = true;
        }
    }
}
