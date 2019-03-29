using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingSample_BLL.Services
{
    using System.Data.Entity;

    using LoggingSample_BLL.Helpers;
    using LoggingSample_BLL.Models;

    using LoggingSample_DAL.Context;

    public class OrderService : IDisposable
    {
        private readonly AppDbContext _context = new AppDbContext();

        public Task<OrderModel> GetOrder(int customerId, int orderId)
        {
            return _context.Orders.Where(o => o.CustomerId == customerId)
                .SingleOrDefaultAsync(o => o.Id == orderId).ContinueWith(t => t.Result?.Map());
        }

        public Task<List<OrderModel>> GetOrders(int customerId)
        {
            return _context.Orders.Where(o => o.CustomerId == customerId).ToListAsync()
                .ContinueWith(t => t.Result.Select(item => item?.Map()).ToList());
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
