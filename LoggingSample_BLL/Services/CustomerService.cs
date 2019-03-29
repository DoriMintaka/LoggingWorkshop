using System;
using System.Data.Entity;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using LoggingSample_BLL.Helpers;
using LoggingSample_BLL.Models;
using LoggingSample_DAL.Context;

namespace LoggingSample_BLL.Services
{
    using System.Collections.Generic;
    using System.Linq;

    public class CustomerService : IDisposable
    {
        private readonly AppDbContext _context = new AppDbContext();

        public Task<CustomerModel> GetCustomer(int customerId)
        {
            if (customerId == 56)
            {
                throw new CustomerServiceException("Wrong id has been requested",
                    CustomerServiceException.ErrorType.WrongCustomerId);
            }

            return _context.Customers.SingleOrDefaultAsync(item => item.Id == customerId).ContinueWith(task =>
            {
                var customer = task.Result;

                return customer?.Map();
            });
        }

        public Task<List<CustomerModel>> GetCustomers()
        {
            return this._context.Customers.ToListAsync().ContinueWith(
                t =>
                    {
                        var customers = t.Result.Select(c => c?.Map()).ToList();

                        return customers;
                    });

        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

    public class CustomerServiceException : Exception
    {
        public enum ErrorType
        {
            WrongCustomerId
        }

        public ErrorType Type { get; set; }

        public CustomerServiceException(string message, ErrorType errorType): base(message)
        {
           Type = errorType;
        }
    }
}