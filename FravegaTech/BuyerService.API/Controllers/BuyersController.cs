using BuyerService.Application.Services;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Dtos;

namespace BuyerService.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BuyersController : ControllerBase
    {
        private readonly IBuyerService _buyerService;

        public BuyersController(IBuyerService buyerService)
        {
            _buyerService = buyerService ?? throw new ArgumentNullException(nameof(buyerService));
        }

        /// <summary>
        /// Gets buyer dto by id
        /// </summary>
        /// <param name="buyerId">Buyer id.</param>
        /// <returns>Buyer dto object.</returns>
        [HttpGet("{buyerId}")]
        public async Task<IActionResult> GetAsync(string buyerId)
        {
            BuyerDto buyerDto = await _buyerService.GetBuyerByIdAsync(buyerId);

            if (buyerDto is not null)
                return Ok(buyerDto);
            else
                return NotFound();
        }

        /// <summary>
        /// Adds new buyer
        /// </summary>
        /// <param name="buyerDto">Buyer dto object.</param>
        /// <returns>Added buyer id.</returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] BuyerDto buyerDto)
        {
            string? buyerId = await _buyerService.AddBuyerAsync(buyerDto);

            if (buyerId is not null)
                return Ok(buyerId);
            else
                return BadRequest();
        }
    }
}
