using BuyerService.Application.Services;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Dtos;

namespace BuyerService.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class BuyersController : ControllerBase
    {
        private readonly IBuyerService _buyerService;

        public BuyersController(IBuyerService buyerService)
        {
            _buyerService = buyerService ?? throw new ArgumentNullException(nameof(buyerService));
        }

        [HttpGet("{buyerId}")]
        public async Task<IActionResult> GetAsync(string buyerId)
        {
            BuyerDto buyerDto = await _buyerService.GetBuyerByIdAsync(buyerId);

            if (buyerDto is not null)
                return Ok(buyerDto);
            else
                return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] BuyerDto buyerDto)
        {
            string? buyerId = await _buyerService.GetBuyerIdOrInsertNewBuyerAsync(buyerDto);

            if (buyerId is not null)
                return Ok(buyerId);
            else
                return BadRequest();
        }
    }
}
