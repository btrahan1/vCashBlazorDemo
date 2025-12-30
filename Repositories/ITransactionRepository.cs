using System.Collections.Generic;
using System.Threading.Tasks;
using vCash.Data.Models;

namespace vCash.Data.Repositories
{
    public interface ITransactionRepository
    {
        Task SaveTransactionAsync(TransactionHeader header, IEnumerable<TransactionDetail> details, IEnumerable<TransactionCheckImage> checkImages);
        Task<IEnumerable<GuestTransactionHistory>> GetHistoryByGuestIdAsync(int guestId);
        Task<IEnumerable<GuestTransactionHistory>> GetHistoryByMakerIdAsync(int makerId);
        Task<Maker?> GetMakerAsync(string routingNumber, string accountNumber, int locationId);
        Task<Bank?> GetBankAsync(string routingNumber);
        Task<IEnumerable<Maker>> GetMakersByLocationAsync(int locationId);
    }
}
