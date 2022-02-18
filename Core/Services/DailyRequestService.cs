using Core.DataAccessLayer;
using Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Services
{
    public class DailyRequestService : IHostedService, IDisposable
    {
        private Timer timer;
        private readonly ILogger<DailyRequestService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IServiceScopeFactory _scopeFactory;

        public DailyRequestService(ILogger<DailyRequestService> logger, HttpClient httpClient, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _httpClient = httpClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(o =>
            {
                var result = GetResponse().Result;

                using (var scope = _scopeFactory.CreateScope())
                {
                    CurrencyContext context = scope.ServiceProvider.GetRequiredService<CurrencyContext>();
                    CurrencyRate currency = new CurrencyRate()
                    {
                        Code = "USD",
                        Value = 3.78m,
                        Date = DateTime.Today
                    };
                    context.Add(currency);
                    context.SaveChanges();
                }

                if (string.IsNullOrEmpty(result))
                    _logger.LogInformation("Nie udało sie");
                else
                    _logger.LogInformation(result + "Udało się");

            },
                null, TimeSpan.Zero,
                TimeSpan.FromSeconds(6));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Task stopped");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        private async Task<string> GetResponse()
        {
            string path = "http://api.nbp.pl/api/exchangerates/rates/a/usd/?format=json";
            HttpResponseMessage result = await _httpClient.GetAsync(path);

            string responseBody = await result.Content.ReadAsStringAsync();

            return responseBody;
        }
    }
}
