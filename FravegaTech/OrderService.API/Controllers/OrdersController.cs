using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Services.Interfaces;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Requests;
using SharedKernel.Dtos.Responses;
using SharedKernel.Exceptions;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets order dto by id
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>Order translated dto object.</returns>
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetAsync(int orderId)
        {
            try
            {
                if (orderId <= 0)
                    return BadRequest("Id de la orden es requerido.");

                _logger.LogInformation($"START endpoint call {GetType().Name}:{nameof(GetAsync)}.");
                OrderTranslatedDto fullOrder = await _orderService.GetFullOrderAsync(orderId);

                _logger.LogInformation($"END endpoint call {GetType().Name}.{nameof(GetAsync)}.");
                return Ok(fullOrder);
            }
            catch (NotFoundException)
            {
                return NotFound("Orden no fue encontrada.");
            }
            catch (DataAccessException)
            {
                return StatusCode(500, "Ocurrió un error al obtener los datos de la orden.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Un error interno ha ocurrido.");
            }
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
        public async Task<IActionResult> SearchAsync([FromQuery] int? orderId, [FromQuery] string? documentNumber, [FromQuery] string? status,
            [FromQuery] DateTime? createdOnFrom, [FromQuery] DateTime? createdOnTo)
        {
            try
            {
                _logger.LogInformation($"START endpoint call {GetType().Name}:{nameof(SearchAsync)}.");
                List<OrderDto> orderDtos = await _orderService.SearchOrdersAsync(orderId, documentNumber, status, createdOnFrom, createdOnTo);

                _logger.LogInformation($"END endpoint call {GetType().Name}.{nameof(SearchAsync)}.");
                return Ok(orderDtos);
            }
            catch (NotFoundException)
            {
                return NotFound("No fueron encontradas Ordenes con los filtros ingresados.");
            }
            catch (DataAccessException)
            {
                return StatusCode(500, "Ocurrió un error al buscar las ordenes.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Un error interno ha ocurrido.");
            }
        }

        /// <summary>
        /// Adds new order
        /// </summary>
        /// <param name="orderRequestDto">Order request dto object.</param>
        /// <returns>Order created dto object.</returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] OrderRequestDto orderRequestDto)
        {
            try
            {
                _logger.LogInformation($"START endpoint call {GetType().Name}:{nameof(PostAsync)}.");
                OrderCreatedDto orderCreatedDto = await _orderService.AddOrderAsync(orderRequestDto);

                _logger.LogInformation($"END endpoint call {GetType().Name}.{nameof(PostAsync)}.");
                return Ok(orderCreatedDto);
            }
            catch (BusinessValidationException)
            {
                return BadRequest("La orden es inválida y no ha sido ingresada en el sistema.");
            }
            catch (DataAccessException)
            {
                return StatusCode(500, "Ocurrió un error al ingresar una nueva orden en el sistema.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Un error interno ha ocurrido.");
            }
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
            try
            {
                if (orderId <= 0)
                    return BadRequest("Id de la orden es requerido.");

                _logger.LogInformation($"START endpoint call {GetType().Name}:{nameof(AddEventAsync)}.");
                EventAddedDto eventAddedDto = await _orderService.AddEventToOrderAsync(orderId, eventDto);

                _logger.LogInformation($"END endpoint call {GetType().Name}.{nameof(AddEventAsync)}.");
                return Ok(eventAddedDto);
            }
            catch (NotFoundException)
            {
                return NotFound("La orden no existe en el sistema.");
            }
            catch (BusinessValidationException)
            {
                return BadRequest("El evento es inválido y no ha sido ingresado en el sistema.");
            }
            catch (DataAccessException)
            {
                return StatusCode(500, "Ocurrió un error al ingresar un nuevo evento en el sistema.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Un error interno ha ocurrido.");
            }
        }
    }
}
