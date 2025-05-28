using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Services.Interfaces;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Requests;
using SharedKernel.Dtos.Responses;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        /// <summary>
        /// Gets order dto by id
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>Order translated dto object.</returns>
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetAsync(int orderId)
        {
            OrderTranslatedDto orderTranslatedDto = await _orderService.GetAndTranslateOrderAsync(orderId);

            if (orderTranslatedDto is not null)
                return Ok(orderTranslatedDto);
            else
                return BadRequest();
        }

        /// <summary>
        /// Searchs orders according to given filters
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="documentNumber">Buyer document number.</param>
        /// <param name="status">Order status.</param>
        /// <param name="createdOnFrom">Order created from.</param>
        /// <param name="createdOnTo">Order created to.</param>
        /// <returns>List of order dto objects.</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync([FromQuery] int orderId, [FromQuery] string documentNumber, [FromQuery] string status,
            [FromQuery] string createdOnFrom, [FromQuery] string createdOnTo)
        {
            List<OrderDto> orderDtos = await _orderService.SearchOrdersAsync(orderId, documentNumber, status, createdOnFrom, createdOnTo);

            if (orderDtos is not null)
                return Ok(orderDtos);
            else
                return BadRequest();
        }

        /// <summary>
        /// Adds new order
        /// </summary>
        /// <param name="orderRequestDto">Order request dto object.</param>
        /// <returns>Order created dto object.</returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] OrderRequestDto orderRequestDto)
        {
            OrderCreatedDto orderCreatedDto = await _orderService.AddOrderAsync(orderRequestDto);

            if (orderCreatedDto is not null)
                return Ok(orderCreatedDto);
            else
                return BadRequest();
        }

        /// <summary>
        /// Adds event to an order
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="eventDto">Event dto.</param>
        /// <returns>Event added dto object.</returns>
        [HttpPost("{orderId}/events")]
        public async Task<IActionResult> AddEventAsync(int orderId, [FromBody] EventDto eventDto)
        {
            EventAddedDto eventAddedDto = await _orderService.AddEventToOrderAsync(orderId, eventDto);

            if (eventAddedDto is not null)
                return Ok(eventAddedDto);
            else
                return BadRequest();
        }
    }
}
