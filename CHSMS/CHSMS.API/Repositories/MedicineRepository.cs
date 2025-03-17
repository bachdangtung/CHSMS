using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace CHSMS.API.Repositories
{
    public class MedicineRepository : IMedicineRepository
    {
        private readonly SEP_TestContext _context;
        private readonly HttpClient _httpClient;
        private const string SheetID = "12aVbY2ZHYXkXg1CnZ_V_y_Rrw4i8BEoH"; // ID của Google Sheets
        private const string SheetURL = $"https://docs.google.com/spreadsheets/d/{SheetID}/gviz/tq?tqx=out:json"; // URL API Google Sheets
        public MedicineRepository(HttpClient httpClient, SEP_TestContext context)
        {
            _context = context;
            _httpClient = httpClient;
        }

        // Get all medicines
        public List<Medicine> GetAllMedicine()
        {
            var medicines = _context.Medicines
                .Include(m => m.MedicineInventories)
                .ThenInclude(mi => mi.Supplier) // Nếu muốn lấy luôn nhà cung cấp
                .ToList();
            return medicines;
        }

        public Medicine? GetMedicine(int medicineId)
        {
            var result = _context.Medicines
                                 .Include(m => m.MedicineInventories) // Eager load MedicineInventories
                                 .ThenInclude(mi => mi.Supplier)     // Eager load Supplier for each MedicineInventory
                                 .FirstOrDefault(m => m.MedicineId == medicineId);

            return result;
        }


        // Get one medicine by ID
        public List<MedicineInventory> GetMedicineDetail(int medicineId)
        {
            return _context.MedicineInventories
                .Where(x => x.MedicineId == medicineId && x.Quantity > 0 && x.ExpiryDate > DateTime.Now)
                .ToList();
        }
        // Get list medicine by ID
        public List<MedicineInventory> GetAvailableMedicineInventory(int medicineId)
        {
            return _context.MedicineInventories
                .Where(x => x.MedicineId == medicineId && x.Quantity > 0 && x.ExpiryDate > DateTime.Now)
                .OrderBy(x => x.ExpiryDate)
                .ToList();
        }

        // Get total quantity of a medicine
        public double GetMedicineQuantity(int medicineId)
        {
            return _context.MedicineInventories
                .Where(x => x.MedicineId == medicineId && x.Quantity > 0 && x.ExpiryDate > DateTime.Now)
                .Sum(x => x.Quantity) ?? 0;
        }

        // Add medicine inventory
        public bool AddMedicineInventory(MedicineInventory medicineInventory)
        {
            var medicineExists = _context.Medicines.Any(m => m.MedicineId == medicineInventory.MedicineId);
            if (!medicineExists)
            {
                throw new Exception("Thuốc không tồn tại trong hệ thống.");
            }

            _context.MedicineInventories.Add(medicineInventory);
            return _context.SaveChanges() > 0;
        }

        // Update medicine inventory
        public bool UpdateMedicineInventory(MedicineInventory medicineInventory)
        {
            _context.MedicineInventories.Update(medicineInventory);
            return _context.SaveChanges() > 0;
        }

        // Calculate expiry date of a medicine inventory
        public DateTime? CalculateExpiryDate(MedicineInventory inventory)
        {
            return inventory.ExpiryDate
                ?? (inventory.ManufacturingDate.HasValue
                    ? inventory.ManufacturingDate.Value.AddMonths(inventory.Medicine?.ShelfLife ?? 0)
                    : null);
        }

        // Search for medicines
        public async Task<dynamic> SearchMedicinesData(string query)
        {
            // For searching, we'll still get all data and filter it in the service
            var response = await _httpClient.GetStringAsync(SheetURL);
            return ParseGoogleSheetsResponse(response);
        }
        // Parse Google Sheets API response
        private dynamic ParseGoogleSheetsResponse(string response)
        {
            // Google Sheets API returns data wrapped in a callback function, we need to extract the JSON
            string jsonString = Regex.Match(response, @"(?<=\().*(?=\);)").Value;
            return JObject.Parse(jsonString);
        }
    }
}
