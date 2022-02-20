using Core.DataAccessLayer;
using Core.Entities;
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

        public async Task<decimal?> GetCurrentExchangeRate(string currencyCode)
        {
            var currentCurrencyRate = await _context.CurrencyRates
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync(x => x.Code == currencyCode.ToUpper());

            if (currentCurrencyRate is null)
                return null;

            return currentCurrencyRate.Value;
        }

        public async Task<decimal?> GetHistoricalExchangeRate(string currencyCode, DateTime date)
        {
            var historicalCurrencyRate = await _context.CurrencyRates
                .FirstOrDefaultAsync(x => x.Date == date.Date && x.Code == currencyCode.ToUpper());

            if (historicalCurrencyRate is null)
                return null;

            return historicalCurrencyRate.Value;
        }

        public async Task<decimal?> RecalculateCurrencyFromPln(string currencyCode, decimal amount)
        {
            var currentCurrencyRate = await _context.CurrencyRates
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync(x => x.Code == currencyCode.ToUpper());

            if (currentCurrencyRate is null)
                return null;

            return Math.Round(amount / currentCurrencyRate.Value, 2);
        }

        public async Task<decimal?> RecalculateCurrencyToPln(string currencyCode, decimal amount)
        {
            var currentCurrencyRate = await _context.CurrencyRates
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync(x => x.Code == currencyCode.ToUpper());

            if (currentCurrencyRate is null)
                return null;

            return Math.Round(amount * currentCurrencyRate.Value, 2);
        }

        public async Task<decimal?> RecalculateTwoCurrencies(string firstCurrencyCode, string secondCurrencyCode)
        {
            var firstCurrencyRate = await _context.CurrencyRates
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync(x => x.Code == firstCurrencyCode.ToUpper());

            var secondCurrencyRate = await _context.CurrencyRates
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync(x => x.Code == secondCurrencyCode.ToUpper());

            return firstCurrencyRate is null || secondCurrencyRate is null ?  null :
                Math.Round(firstCurrencyRate.Value / secondCurrencyRate.Value, 2);
        }

        public async Task<string> GetCsvFile(string currencyCode, DateTime dateFrom, DateTime dateTo)
        {
            var currencyRates = await _context.CurrencyRates
                .Where(x => x.Code == currencyCode.ToUpper() && x.Date >= dateFrom.Date && x.Date <= dateTo)
                .ToListAsync();

            return currencyRates.Count > 0 ? TransferToCsv(currencyRates) : null;
        }

        private static string TransferToCsv(List<CurrencyRate> currencyRates)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Date,Value");

            foreach (var rate in currencyRates)
            {
                builder.AppendLine(rate.Date.ToString().Substring(0, 10)
                    + "," + rate.Value.ToString().Replace(',', '.'));
            }

            return builder.ToString();
        }
    }
}
