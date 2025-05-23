using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Services;
using SharedKernel.Dtos;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetAsync(int orderId)
        {
            return Ok(await _orderService.GetAndTranslateOrderAsync(orderId));
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync([FromQuery] int orderId, [FromQuery] string documentNumber, [FromQuery] string status,
            [FromQuery] string createdOnFrom, [FromQuery] string createdOnTo)
        {
            return Ok(await _orderService.SearchOrdersAsync(orderId, documentNumber, status, createdOnFrom, createdOnTo));
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] OrderDto orderDto)
        {
            string? orderId = await _orderService.InsertNewOrderAsync(orderDto);

            if (orderId is not null)
                return Ok(orderId);
            else
                return BadRequest();
        }

        [HttpPost("{orderId}/events")]
        public async Task<IActionResult> AddEventAsync(int orderId, [FromBody] EventDto eventDto)
        {
            return Ok(await _orderService.AddEventAsync(orderId, eventDto));
        }
    }
}
