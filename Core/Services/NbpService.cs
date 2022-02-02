using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Core.DTO;
using Newtonsoft.Json;
using Core.Entities;
using Core.DataAccessLayer;

namespace Core.Services
{
    public class NbpSettings
    {
        public string BaseUrl { get; set; }
    }

    public class NbpService : IHostedService, IDisposable
    {
        private const string Table = "A";
        private readonly HttpClient _client;
        private readonly NbpSettings _settings;
        private readonly CurrencyContext _context;
        private readonly ILogger<NbpService> _logger;
        private Timer timer;

        public NbpService(IOptions<NbpSettings> options, HttpClient client,
            ILogger<NbpService> logger, CurrencyContext context)
        {
            _client = client; // fix later
            _context = context;
            _settings = options.Value;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(async o =>
            {
                bool result = await SaveData(Table);

                if (result)
                {
                    _logger.LogInformation("Data saved succesfully");
                }
                else
                {
                    _logger.LogInformation("Failed to get and save the data");
                }

            },
                null, TimeSpan.Zero,
                TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Task stopped");
            return Task.CompletedTask;
        }

        public async Task<bool> SaveData(string table)
        {
            //string url = $"{_settings.BaseUrl}/api/exchangerates/tables/{table}/today/?format=json";
            string url = "http://api.nbp.pl/api/exchangerates/tables/A/today/?format=json";
            string json;
            try
            {
                json = await _client.GetStringAsync(url);
            }
            catch (Exception exception)
            {
                _logger.LogInformation($"Exception: {exception.Message}");
                return true;
            }

            CompleteResponse response;

            try
            {
                response = JsonConvert.DeserializeObject<List<CompleteResponse>>(json).First();
            }
            catch (Exception exception)
            {
                _logger.LogInformation($"Exception: {exception.Message}");
                return false;
            }

            DateTime date = response.EffectiveDate;

            foreach (var rate in response.Rates)
            {
                CurrencyRate currencyRate = new CurrencyRate()
                {
                    Code = rate.Code,
                    Value = rate.Mid,
                    Date = date
                };

                await _context.CurrencyRates.AddAsync(currencyRate);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }

}
