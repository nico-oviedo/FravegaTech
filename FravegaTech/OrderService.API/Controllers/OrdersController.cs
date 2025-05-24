using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Services;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Responses;

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
            OrderTranslatedDto orderTranslatedDto = await _orderService.GetAndTranslateOrderAsync(orderId);

            if (orderTranslatedDto is not null)
                return Ok(orderTranslatedDto);
            else
                return BadRequest();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync([FromQuery] int orderId, [FromQuery] string documentNumber, [FromQuery] string status,
            [FromQuery] string createdOnFrom, [FromQuery] string createdOnTo)
        {
            IEnumerable<OrderDto> orderDtos = await _orderService.SearchOrdersAsync(orderId, documentNumber, status, createdOnFrom, createdOnTo);

            if (orderDtos is not null)
                return Ok(orderDtos);
            else
                return BadRequest();
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
