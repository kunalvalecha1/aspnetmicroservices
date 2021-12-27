using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContext> logger)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.Orders.AddRange(GetPreconfigureOrders());
                await orderContext.SaveChangesAsync();
            }
        }

        private static IEnumerable<Order> GetPreconfigureOrders()
        {
            return new List<Order>
         {
             new Order() {
                 UserName="admin", 
                 FirstName ="admin", 
                 LastName="admin", 
                 EmailAddress="admin@admin.com",
                 TotalPrice=100,
             }
         };  
        }
    }
}
