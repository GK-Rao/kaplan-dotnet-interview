using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using WebApiTest.Data;
using WebApiTest.Models;

namespace WebApiTest.Services
{
    public class OrderItemsService : IOrderItemsService
    {
        public OrderItemsService( TestDbContext dbContext )
        {
            testDbContext = dbContext;
        }

        public OrderItemsModel Get( int orderID )
        {
            int result = testDbContext.Orders.Where(o => o.OrderID == orderID).Count();
            if (result == 0)
            {

                throw new System.Exception("Entered OrderId not found");
            }
                IEnumerable<OrderItem> orderItems = testDbContext.OrderItems.Where(oi => oi.OrderID == orderID);

                return new OrderItemsModel
                {
                    OrderID = orderID,
                    Items = orderItems.Select(oi => new OrderItemModel
                    {
                        LineNumber = oi.LineNumber,
                        ProductID = oi.ProductID,
                        StudentPersonID = oi.StudentPersonID,
                        Price = oi.Price
                    })
                };

            
        }

        public async Task<short> AddAsync( int orderID, OrderItemModel item )
        {
            if ( item.LineNumber != 0 )
            {
                throw new ValidationException( "LineNumber is generated and cannot be specified" );
            }

            // Changed 0 to 1 to get new unique lineNumber for every new order posted.
            var lineNumber = (short)( testDbContext.OrderItems.Where( oi => oi.OrderID == orderID ).Max( oi => oi.LineNumber ) + 1 );

            await testDbContext.OrderItems.AddAsync( new OrderItem
                                                         {
                                                             OrderID = orderID,
                                                             LineNumber = lineNumber,
                                                             Price = item.Price,
                                                             ProductID = item.ProductID,
                                                             StudentPersonID = item.StudentPersonID
                                                         } );

            await testDbContext.SaveChangesAsync();

            return lineNumber;
        }

        public async  Task<string> DeleteAsync(int orderID)
        {

            var orders = new Order
            {
                OrderID = orderID
            };

            int existsOrderId = testDbContext.Orders.Where(oi => oi.OrderID == orderID).Count();

            if (existsOrderId != 0)
            {
                testDbContext.Orders.Remove(orders);
            }

            int result = await testDbContext.SaveChangesAsync();

            if (result != 0)
            {
                return $"{orderID} Deleted Successfully";
            }

            return $"{orderID} Not Found";



        }

        private readonly TestDbContext testDbContext;
    }
}
