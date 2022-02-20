using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IUserService
    {
        Task<decimal?> GetCurrentExchangeRate(string currencyCode);
        Task<decimal?> GetHistoricalExchangeRate(string currencyCode, DateTime date);
        Task<decimal?> RecalculateCurrencyToPln(string currencyCode, decimal amount);
        Task<decimal?> RecalculateCurrencyFromPln(string currencyCode, decimal amount);
        Task<decimal?> RecalculateTwoCurrencies(string firstCode, string secondCode);
    }
}
