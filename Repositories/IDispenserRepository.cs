using System.Threading.Tasks;
using vCash.Data.Models;
using System;

namespace vCash.Data.Repositories
{
    public interface IDispenserRepository
    {
        Task<Dispenser> GetDispenserAsync(int locationId, DateTime businessDate);
        Task UpdateDispenserCountsAsync(Dispenser dispenser);
    }
}
