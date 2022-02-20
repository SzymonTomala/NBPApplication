using Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace NBPApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("{code}")]
        public async Task<IActionResult> GetCurrentExchangeRate([FromRoute] string code)
        {
            var result = await _userService.GetCurrentExchangeRate(code);

            return result is not null ? Ok(result) : NotFound();
        }

        [HttpGet]
        [Route("{code}/{date}")]
        public async Task<IActionResult> GetCurrentExchangeRate([FromRoute] string code, [FromRoute] DateTime date)
        {
            var result = await _userService.GetHistoricalExchangeRate(code, date);

            return result is not null ? Ok(result) : NotFound();
        }

        [HttpGet]
        [Route("RecalculateFromPln/{code}/{amount}")]
        public async Task<IActionResult> RecalculateCurrencyFromPln([FromRoute] string code, [FromRoute] decimal amount)
        {
            var result = await _userService.RecalculateCurrencyFromPln(code, amount);

            return result is not null ? Ok(result) : NotFound();
        }

        [HttpGet]
        [Route("RecalculateToPln/{code}/{amount}")]
        public async Task<IActionResult> RecalculateCurrencyToPln([FromRoute] string code, [FromRoute] decimal amount)
        {
            var result = await _userService.RecalculateCurrencyToPln(code, amount);

            return result is not null ? Ok(result) : NotFound();
        }

        [HttpGet]
        [Route("RecalculateTwoCurrencies/{firstCode}/{secondCode}")]
        public async Task<IActionResult> RecalculateTwoCurrencies([FromRoute] string firstCode, [FromRoute] string secondCode)
        {
            var result = await _userService.RecalculateTwoCurrencies(firstCode, secondCode);

            return result is not null ? Ok(result) : NotFound();
        }

        [HttpGet]
        [Route("GetCsvFile/{currencyCode}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetCsvFile([FromRoute] string currencyCode,
            [FromRoute] DateTime dateFrom, [FromRoute] DateTime dateTo)
        {
            var result = await _userService.GetCsvFile(currencyCode, dateFrom, dateTo);

            return result is not null ? Ok(result) : NotFound();
        }

        
    }
}
