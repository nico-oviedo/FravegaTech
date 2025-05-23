using BuyerService.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;

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


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BuyerDto buyerDto)
        {
            string? buyerId = await _buyerService.GetOrInsertNewBuyer(buyerDto);

            if (buyerId is not null)
                return Ok(buyerId);
            else
                return BadRequest();
        }
    }
}
