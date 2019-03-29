using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Routing;
using LoggingSample_BLL.Helpers;
using LoggingSample_BLL.Models;
using LoggingSample_DAL.Context;
using LoggingSample_DAL.Entities;

namespace LoggingSample.Controllers
{
    using System.Collections.Specialized;

    using LoggingSample_BLL.Services;

    using NLog;

    [RoutePrefix("api")]
    public class OrdersController : ApiController
    {
        private readonly AppDbContext _context = new AppDbContext();

        private readonly OrderService _service = new OrderService();

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        [Route("customers/{customerId}/orders", Name = "Orders")]
        public async Task<IHttpActionResult> Get(int customerId)
        {
            Logger.Info($"Start getting orders of customer {customerId}.");

            try
            {
                var orders = (await this._service.GetOrders(customerId)).Select(o => InitOrder(o));

                Logger.Info($"Retrieving orders of customer {customerId}.");
                return Ok(orders);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Some error occured while getting orders of customer {customerId}.");
                throw;
            }
        }

        [Route("customers/{customerId}/orders/{orderId}", Name = "Order")]
        public async Task<IHttpActionResult> Get(int customerId, int orderId)
        {
            Logger.Info($"Start getting order {orderId} of customer {customerId}.");
            try
            {
                var order = await this._service.GetOrder(customerId, orderId);

                if (order == null)
                {
                    Logger.Info($"Customer {customerId} doesn't have order with id {orderId}.");
                    return NotFound();
                }

                Logger.Info($"Retrieving order {orderId} of customer {customerId}.");

                return Ok(InitOrder(order));
            }
            catch (Exception e)
            {
                Logger.Error($"Some error occured while getting order {orderId} of customer {customerId}.");
                throw;
            }
        }

        private object InitOrder(OrderModel model)
        {
            return new
            {
                _self = new UrlHelper(Request).Link("Order", new {customerId = model.CustomerId, orderId = model.Id}),
                customer = new UrlHelper(Request).Link("Customer", new {customerId = model.CustomerId}),
                data = model
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
                this._service.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}