using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.DTO;
using Core.Services.NbpServices;
using Core.DataAccessLayer;

namespace Infrastructure.Services.NbpApiServices
{
    public class DataSavingService : IDataSavingService
    {
        private const int TimeZoneHoursDifference = 1;
        private readonly INbpApiService _nbpApiService;
        private readonly CurrencyContext _context;

        public DataSavingService(INbpApiService nbpApiService, CurrencyContext context)
        {
            _nbpApiService = nbpApiService;
            _context = context;
        }

        public async Task<bool> SaveData(string table)
        {
            try
            {
                string json = await _nbpApiService.GetJson(table);

                if (json != null)
                {
                    return true;
                }
                string properJson = json[1..^1];
                CompleteResponse response = JsonConvert.DeserializeObject<CompleteResponse>(properJson);
                DateTime date = response.EffectiveDate;

                List<Rate> rates = response.Rates.ToList();

                foreach (var rate in rates)
                {
                    CurrencyRate newRate = new CurrencyRate()
                    {
                        Value = rate.Mid,
                        Code = rate.Code,
                        Date = date
                    };
                    await _context.CurrencyRates.AddAsync(newRate);
                }
                await _context.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }

        
    }
}