using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using System.Threading;
using Core.Services.NbpServices;
using Core.DataAccessLayer;

namespace Infrastructure.Services.NbpApiServices
{

    public class DailyRequestService : IHostedService, IDisposable
    {
        private const string Table = "A";
        private readonly IDataSavingService _dataSavingService;
        private readonly ILogger<DailyRequestService> _logger;
        private Timer timer;

        public DailyRequestService(ILogger<DailyRequestService> logger)
        {
            _dataSavingService = new DataSavingService(new NbpApiService(), new CurrencyContext());
            _logger = logger;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(o =>
            {
                bool result = _dataSavingService.SaveData(Table).Result;

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

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}