using Core.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class UserService : IUserService
    {
        private readonly CurrencyContext _context;

        public UserService(CurrencyContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetCurrentExchangeRate(string currencyCode)
        {
            var currentCurrencyRate = await _context.CurrencyRates
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync(x => x.Code == currencyCode);

            return currentCurrencyRate.Value;
        }

        public async Task<decimal> GetHistoricalExchangeRate(string currencyCode, DateTime date)
        {
            var historicalCurrencyRate = await _context.CurrencyRates
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync(x => x.Code == currencyCode);

            return historicalCurrencyRate.Value;
        }

        public async Task<decimal> RecalculateCurrencyFromPln(string currencyCode, decimal amount)
        {
            var currentCurrencyRate = await _context.CurrencyRates
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync(x => x.Code == currencyCode);

            return Math.Round(amount / currentCurrencyRate.Value, 2);
        }

        public async Task<decimal> RecalculateCurrencyToPln(string currencyCode, decimal amount)
        {
            var currentCurrencyRate = await _context.CurrencyRates
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync(x => x.Code == currencyCode);

            return Math.Round(amount * currentCurrencyRate.Value, 2);
        }
    }
}
