using Core.Services.UserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return Ok(result);
        }

        [HttpGet]
        [Route("{code}/{date}")]
        public async Task<IActionResult> GetCurrentExchangeRate([FromRoute] string code, [FromRoute] DateTime date)
        {
            var result = await _userService.GetHistoricalExchangeRate(code, date);
            return Ok(result);
        }

        [HttpGet]
        [Route("recalculateFromPln/{code}/{amount}")]
        public async Task<IActionResult> RecalculateCurrencyFromPln([FromRoute] string code, [FromRoute] decimal amount)
        {
            var result = await _userService.RecalculateCurrencyFromPln(code, amount);
            return Ok(result);
        }

        [HttpGet]
        [Route("recalculateToPln/{code}/{amount}")]
        public async Task<IActionResult> RecalculateCurrencyToPln([FromRoute] string code, [FromRoute] decimal amount)
        {
            var result = await _userService.RecalculateCurrencyToPln(code, amount);
            return Ok(result);
        }
    }
}
