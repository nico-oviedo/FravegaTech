using BuyerService.Application.Services;
using BuyerService.Domain;
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
            Buyer? buyer = await _buyerService.GetBuyerByIdAsync(buyerId);

            if (buyer is not null)
                return Ok(buyer);
            else
                return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] BuyerDto buyerDto)
        {
            string? buyerId = await _buyerService.GetOrInsertNewBuyerAsync(buyerDto);

            if (buyerId is not null)
                return Ok(buyerId);
            else
                return BadRequest();
        }
    }
}
