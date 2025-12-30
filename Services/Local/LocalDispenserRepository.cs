using Blazored.LocalStorage;
using vCash.Data.Models;
using vCash.Data.Repositories;

namespace vCashBlazorDemo.Services.Local
{
    public class LocalDispenserRepository : IDispenserRepository
    {
        private readonly ILocalStorageService _localStorage;
        
        public LocalDispenserRepository(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        private string GetKey(int locationId) => $"vCash_Dispenser_{locationId}";

        public async Task<Dispenser> GetDispenserAsync(int locationId, DateTime businessDate)
        {
            var key = GetKey(locationId);
            var d = await _localStorage.GetItemAsync<Dispenser>(key);

            if (d == null)
            {
                // Seed
                d = new Dispenser 
                { 
                    DispenserId = 1, 
                    LocationId = locationId, 
                    DeviceName = "Demo Dispenser 01",
                    BusinessDate = businessDate,
                    // Lots of cash for demo :)
                    Current1s = 100,
                    Current5s = 100,
                    Current10s = 100,
                    Current20s = 500, // $10k
                    Current50s = 100,
                    Current100s = 50
                };
                await _localStorage.SetItemAsync(key, d);
            }
            return d;
        }

        public async Task UpdateDispenserCountsAsync(Dispenser dispenser)
        {
            // For demo, we just overwrite the whole object state
            // In SQL we only updated PaidIn/Out, but here Object Persistence is easier
            var key = GetKey(dispenser.LocationId);
            
            // Recalculate currents logic if needed? 
            // For simplicity, we assume the UI passed us the 'Finished' state object or we just save it.
            // Since the Interface only takes the object, let's just save the object.
            await _localStorage.SetItemAsync(key, dispenser);
        }
    }
}
