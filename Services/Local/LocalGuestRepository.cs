using Blazored.LocalStorage;
using vCash.Data.Models;
using vCash.Data.Repositories;

namespace vCashBlazorDemo.Services.Local
{
    public class LocalGuestRepository : IGuestRepository
    {
        private readonly ILocalStorageService _localStorage;
        private const string Key = "vCash_Guests";

        public LocalGuestRepository(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        private async Task<List<Guest>> LoadGuestsAsync()
        {
            var guests = await _localStorage.GetItemAsync<List<Guest>>(Key);
            if (guests == null || guests.Count == 0)
            {
                guests = SeedGuests();
                await _localStorage.SetItemAsync(Key, guests);
            }
            return guests;
        }

        private List<Guest> SeedGuests()
        {
            return new List<Guest>
            {
                new Guest { GuestId = 1, FirstName = "John", LastName = "Doe", SSN="***-**-1234", City="New York", State="NY", AccountLimit=1000, DateLastVisit=DateTime.Now.AddDays(-2) },
                new Guest { GuestId = 2, FirstName = "Jane", LastName = "Smith", SSN="***-**-5678", City="Los Angeles", State="CA", AccountLimit=5000, DateLastVisit=DateTime.Now.AddDays(-10) }
            };
        }

        public async Task<int> AddAsync(Guest guest)
        {
            var guests = await LoadGuestsAsync();
            guest.GuestId = guests.Any() ? guests.Max(g => g.GuestId) + 1 : 1;
            guests.Add(guest);
            await _localStorage.SetItemAsync(Key, guests);
            return guest.GuestId;
        }

        public async Task<Guest?> GetByCardNumberAsync(string cardNumber)
        {
            // For demo, we treat SSN or Phone as lookup since we don't have card swipe
            // Or just return the first guest for easy testing
            var guests = await LoadGuestsAsync();
            return guests.FirstOrDefault(); 
        }

        public async Task<Guest?> GetByIdAsync(int id)
        {
            var guests = await LoadGuestsAsync();
            return guests.FirstOrDefault(g => g.GuestId == id);
        }

        public async Task<IEnumerable<Guest>> SearchAsync(string query)
        {
            var guests = await LoadGuestsAsync();
            if (string.IsNullOrWhiteSpace(query)) return guests;
            
            return guests.Where(g => 
                (g.FirstName + " " + g.LastName).Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (g.SSN != null && g.SSN.Contains(query)));
        }

        public async Task UpdateAsync(Guest guest)
        {
            var guests = await LoadGuestsAsync();
            var existing = guests.FirstOrDefault(g => g.GuestId == guest.GuestId);
            if (existing != null)
            {
                guests.Remove(existing);
                guests.Add(guest);
                await _localStorage.SetItemAsync(Key, guests);
            }
        }

        // Image Handling: Store separately to avoid List bloat
        public async Task<byte[]> GetGuestPhotoIdAsync(int guestId)
        {
            var key = $"vCash_GuestId_{guestId}_Front";
            var data = await _localStorage.GetItemAsync<string>(key);
            return !string.IsNullOrEmpty(data) ? Convert.FromBase64String(data) : Array.Empty<byte>();
        }

        public async Task<byte[]> GetGuestPictureAsync(int guestId)
        {
            var key = $"vCash_GuestPic_{guestId}";
            var data = await _localStorage.GetItemAsync<string>(key);
            return !string.IsNullOrEmpty(data) ? Convert.FromBase64String(data) : Array.Empty<byte>();
        }

        public async Task SaveImageAsync(int guestId, byte[] imageBytes)
        {
            var key = $"vCash_GuestPic_{guestId}";
            await _localStorage.SetItemAsync(key, Convert.ToBase64String(imageBytes));
        }

        public async Task SavePhotoIdAsync(int guestId, byte[] frontImage, byte[] rearImage = null)
        {
             var keyFront = $"vCash_GuestId_{guestId}_Front";
             await _localStorage.SetItemAsync(keyFront, Convert.ToBase64String(frontImage));
             
             if (rearImage != null)
             {
                 var keyRear = $"vCash_GuestId_{guestId}_Rear";
                 await _localStorage.SetItemAsync(keyRear, Convert.ToBase64String(rearImage));
             }
        }
    }
}
