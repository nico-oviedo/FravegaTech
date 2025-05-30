using BuyerService.Application.Services;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Dtos;
using SharedKernel.Exceptions;

namespace BuyerService.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BuyersController : ControllerBase
    {
        private readonly IBuyerService _buyerService;
        private readonly ILogger<BuyersController> _logger;

        public BuyersController(IBuyerService buyerService, ILogger<BuyersController> logger)
        {
            _buyerService = buyerService ?? throw new ArgumentNullException(nameof(buyerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets buyer dto by id
        /// </summary>
        /// <param name="buyerId">Buyer id.</param>
        /// <returns>Buyer dto object.</returns>
        [HttpGet("{buyerId}")]
        public async Task<IActionResult> GetAsync(string buyerId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(buyerId))
                    return BadRequest("Id del comprador es requerido.");

                _logger.LogInformation($"START endpoint call {GetType().Name}:{nameof(GetAsync)}.");
                BuyerDto buyerDto = await _buyerService.GetBuyerByIdAsync(buyerId);

                _logger.LogInformation($"END endpoint call {GetType().Name}.{nameof(GetAsync)}.");
                return Ok(buyerDto);
            }
            catch (NotFoundException)
            {
                return NotFound("Comprador no fue encontrado.");
            }
            catch (DataAccessException)
            {
                return StatusCode(500, "Ocurrió un error al obtener los datos del comprador.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Un error interno ha ocurrido.");
            }
        }

        /// <summary>
        /// Gets buyer id by document number
        /// </summary>
        /// <param name="documentNumber">Buyer document number.</param>
        /// <returns>Buyer id.</returns>
        [HttpGet("documentnumber/{documentNumber}")]
        public async Task<IActionResult> GetByDocumentNumberAsync(string documentNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(documentNumber))
                    return BadRequest("Número de documento del comprador es requerido.");

                _logger.LogInformation($"START endpoint call {GetType().Name}:{nameof(GetByDocumentNumberAsync)}.");
                string? buyerId = await _buyerService.GetBuyerIdByDocumentNumberAsync(documentNumber);

                _logger.LogInformation($"END endpoint call {GetType().Name}.{nameof(GetByDocumentNumberAsync)}.");
                return Ok(buyerId);
            }
            catch (DataAccessException)
            {
                return StatusCode(500, "Ocurrió un error al obtener el id del comprador.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Un error interno ha ocurrido.");
            }
        }

        /// <summary>
        /// Adds new buyer
        /// </summary>
        /// <param name="buyerDto">Buyer dto object.</param>
        /// <returns>Added buyer id.</returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] BuyerDto buyerDto)
        {
            try
            {
                _logger.LogInformation($"START endpoint call {GetType().Name}:{nameof(PostAsync)}.");
                string buyerId = await _buyerService.AddBuyerAsync(buyerDto);

                _logger.LogInformation($"END endpoint call {GetType().Name}.{nameof(PostAsync)}.");
                return Ok(buyerId);
            }
            catch (DataAccessException)
            {
                return StatusCode(500, "Ocurrió un error al ingresar un nuevo comprador en el sistema.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Un error interno ha ocurrido.");
            }
        }
    }
}
