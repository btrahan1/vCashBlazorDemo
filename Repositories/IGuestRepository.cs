using System.Collections.Generic;
using System.Threading.Tasks;
using vCash.Data.Models;

namespace vCash.Data.Repositories
{
    public interface IGuestRepository
    {
        Task<IEnumerable<Guest>> SearchAsync(string query);
        Task<Guest?> GetByIdAsync(int id);
        Task<Guest?> GetByCardNumberAsync(string cardNumber);
        Task<int> AddAsync(Guest guest);
        Task UpdateAsync(Guest guest);
        Task SaveImageAsync(int guestId, byte[] imageBytes);
        Task SavePhotoIdAsync(int guestId, byte[] frontImage, byte[] rearImage = null);
        Task<byte[]> GetGuestPictureAsync(int guestId);
        Task<byte[]> GetGuestPhotoIdAsync(int guestId);
    }
}
