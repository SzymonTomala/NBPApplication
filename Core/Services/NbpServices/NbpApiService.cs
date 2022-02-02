using Core.Services.NbpServices;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infrastructure.Services.NbpApiServices
{

    public class NbpApiService : INbpApiService
    {
        private readonly HttpClient _client;

        public NbpApiService()
        {
            _client = new HttpClient();
        }

        public async Task<string> GetJson(string table)
        {
            string url = CreateUrl(table);
            string response;
            try
            {
                response = await _client.GetStringAsync(url);
            }
            catch
            {
                return null;
            }

            return response;
        }

        private static string CreateUrl(string table)
        {
            return $"http://api.nbp.pl/api/exchangerates/tables/{table}/today/?format=json";
        }
    }
}
