using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vCash.Data.Models;
using vCash.Data.Repositories;
using vCashBlazorDemo.Models;

namespace vCashBlazorDemo.Services
{
    public class GuestService
    {
        private readonly IGuestRepository _repository;
        private readonly ITransactionRepository _transactionRepository;

        public GuestService(IGuestRepository repository, ITransactionRepository transactionRepository)
        {
            _repository = repository;
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<GuestModel>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<GuestModel>();

            var results = await _repository.SearchAsync(query);

            // Map Data Entity to UI Model
            return results.Select(g => new GuestModel
            {
                GuestId = g.GuestId, // Map PK
                Id = g.GuestUid == Guid.Empty ? Guid.NewGuid() : g.GuestUid, 
                FirstName = g.FirstName,
                MiddleName = g.MiddleName ?? "",
                LastName = g.LastName,
                Address = g.AddressLine1 ?? "",
                City = g.City ?? "",
                State = g.State ?? "",
                Zip = g.Zip.ToString(),
                DLNumber = g.DrLicNbr ?? "",
                DLState = g.DrLicState ?? "",
                SSN = g.SSN ?? "",
                BirthDate = g.BirthDate,
                Phone1 = g.PhoneNum1 ?? "",
                Phone2 = g.PhoneNum2 ?? "",
                IsBanned = g.Blocked,
                Notes = g.Comment,
                AccountLimit = g.AccountLimit
            });
        }

        public async Task<bool> AddGuestAsync(GuestModel model)
        {
            var entity = new vCash.Data.Models.Guest
            {
                GuestUid = Guid.NewGuid(),
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                AddressLine1 = model.Address,
                City = model.City,
                State = model.State,
                Zip = int.TryParse(model.Zip, out var z) ? z : 0,
                SSN = model.SSN,
                DrLicNbr = model.DLNumber,
                DrLicState = model.DLState,
                Sex = model.Sex,
                BirthDate = model.BirthDate,
                PhoneNum1 = model.Phone1,
                PhoneNum2 = model.Phone2,
                AccountLimit = model.AccountLimit,
                Blocked = model.IsBanned,
                Comment = model.Notes,
                CustomerId = 1 // Hardcoded for single-tenant POC
            };

            var newId = await _repository.AddAsync(entity);
            
            // Handle Photo Persistence
            if (!string.IsNullOrEmpty(model.PhotoUrl) && model.PhotoUrl.Contains(","))
            {
                try
                {
                    // expecting data:image/png;base64,....
                    var base64Data = model.PhotoUrl.Split(',')[1];
                    var imageBytes = Convert.FromBase64String(base64Data);
                    await _repository.SaveImageAsync(newId, imageBytes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to save image for GuestId {newId}: {ex.Message}");
                }
            }
            
            // Handle Photo ID Persistence
            if (!string.IsNullOrEmpty(model.PhotoIdUrl) && model.PhotoIdUrl.Contains(","))
            {
                try
                {
                    var base64Data = model.PhotoIdUrl.Split(',')[1];
                    var imageBytes = Convert.FromBase64String(base64Data);
                    await _repository.SavePhotoIdAsync(newId, imageBytes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to save Photo ID for GuestId {newId}: {ex.Message}");
                }
            }
            
            return true;
        }

        public async Task<bool> UpdateGuestAsync(GuestModel model)
        {
            var entity = new vCash.Data.Models.Guest
            {
                GuestId = model.GuestId, // PK required for Update
                GuestUid = model.Id,
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                AddressLine1 = model.Address,
                City = model.City,
                State = model.State,
                Zip = int.TryParse(model.Zip, out var z) ? z : 0,
                SSN = model.SSN,
                DrLicNbr = model.DLNumber,
                DrLicState = model.DLState,
                Sex = model.Sex,
                BirthDate = model.BirthDate,
                PhoneNum1 = model.Phone1,
                PhoneNum2 = model.Phone2,
                AccountLimit = model.AccountLimit,
                Blocked = model.IsBanned,
                Comment = model.Notes,
                CustomerId = 1
            };

            await _repository.UpdateAsync(entity);
            
            // Handle Photo Update
            if (!string.IsNullOrEmpty(model.PhotoUrl) && model.PhotoUrl.Contains(","))
            {
                try
                {
                    var base64Data = model.PhotoUrl.Split(',')[1];
                    var imageBytes = Convert.FromBase64String(base64Data);
                    await _repository.SaveImageAsync(model.GuestId, imageBytes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to update image for GuestId {model.GuestId}: {ex.Message}");
                }
            }
            
            // Handle Photo ID Update
            if (!string.IsNullOrEmpty(model.PhotoIdUrl) && model.PhotoIdUrl.Contains(","))
            {
                try
                {
                    var base64Data = model.PhotoIdUrl.Split(',')[1];
                    var imageBytes = Convert.FromBase64String(base64Data);
                    await _repository.SavePhotoIdAsync(model.GuestId, imageBytes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to update Photo ID for GuestId {model.GuestId}: {ex.Message}");
                }
            }

            return true;
        }

        public async Task LoadCandidateImagesAsync(GuestModel model)
        {
            if (model == null) return;

            // Load Guest Picture
            var photoBytes = await _repository.GetGuestPictureAsync(model.GuestId);
            if (photoBytes != null && photoBytes.Length > 0)
            {
                var base64 = Convert.ToBase64String(photoBytes);
                model.PhotoUrl = $"data:image/png;base64,{base64}";
            }
            else
            {
                model.PhotoUrl = "";
            }

            // Load Guest ID
            var idBytes = await _repository.GetGuestPhotoIdAsync(model.GuestId);
            if (idBytes != null && idBytes.Length > 0)
            {
                 var base64 = Convert.ToBase64String(idBytes);
                 model.PhotoIdUrl = $"data:image/png;base64,{base64}";
            }
            else
            {
                model.PhotoIdUrl = "";
            }
            
        }

        public async Task<IEnumerable<GuestTransactionHistory>> GetTransactionHistoryAsync(int guestId)
        {
            return await _transactionRepository.GetHistoryByGuestIdAsync(guestId);
        }
    }
}
