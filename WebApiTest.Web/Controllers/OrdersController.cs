using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using WebApiTest.Models;
using WebApiTest.Services;

namespace WebApiTest.Web.Controllers
{
    [ApiController]
    [Route( "[controller]" )]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderItemsService _orderItemsService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController( IOrderItemsService orderItemsService, ILogger<OrdersController> logger )
        {
            _logger = logger;
            _orderItemsService = orderItemsService;
        }

        /// <summary>
        ///     Get the items on the order
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        [Route("{orderID:int}")]
        [HttpGet]
        public ActionResult<OrderItemsModel> Get( int orderID )
        {
            try
            {
                return _orderItemsService.Get(orderID);
            }
            catch (Exception  ex)
            {
                return NotFound();
            }
            
        }

        /// <summary>
        ///     Adds an item to an order, LineNumber must be zero or unspecified
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="item">The Order Item</param>
        /// <returns></returns>
        [Route("{orderID:int}")]
        [HttpPost]
        public Task<short> Post( int orderID, OrderItemModel item )
        {
            try
            {
                return _orderItemsService.AddAsync( orderID, item );
            }
            catch ( ValidationException ve )
            {
                throw new BadHttpRequestException( ve.Message );
            }
        }

        [Route("{orderID:int}")]
        [HttpDelete]
        public Task<string> Delete (int orderID)
        {

                return  _orderItemsService.DeleteAsync(orderID);
        }
    }
}
