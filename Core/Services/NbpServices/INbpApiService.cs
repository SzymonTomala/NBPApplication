using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.NbpServices
{
    public interface INbpApiService
    {
        Task<string> GetJson(string table);
    }
}
