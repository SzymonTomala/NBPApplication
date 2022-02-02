using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.NbpServices
{
    public interface IDataSavingService
    {
        Task<bool> SaveData(string table);
    }
}
