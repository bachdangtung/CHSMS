using CHSMS.API.DTOs.Medicine;
using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;
using CHSMS.API.Repositories;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CHSMS.API.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly SEP_TestContext _context; // For transaction support
        private readonly IMedicineRepository _medicineRepository;

        public MedicineService(IMedicineRepository medicineRepository, SEP_TestContext context)
        {
            _medicineRepository = medicineRepository;
            _context = context;
        }

        public List<MedicineDTO> GetAll()
        {
            List<MedicineDTO> medicineDTOs = new List<MedicineDTO>();
            foreach (var medicine in _medicineRepository.GetAllMedicine())
            {
                var medicineDTO = new MedicineDTO
                {
                    MedicineId = medicine.MedicineId,
                    MedicineName = medicine.MedicineName,
                    ActiveIngredient = medicine.ActiveIngredient,
                    Dosage = medicine.Dosage,
                    DosageForm = medicine.DosageForm,
                    ImportPrice = medicine.ImportPrice,
                    SellingPrice = medicine.SellingPrice,
                    Quantity = _medicineRepository.GetMedicineQuantity(medicine.MedicineId),
                    ShelfLife = medicine.ShelfLife,
                    BatchNumber = medicine.MedicineInventories.FirstOrDefault()?.BatchNumber,
                    BidNumber = medicine.BidNumber,
                    SupplierName = medicine.MedicineInventories.FirstOrDefault()?.Supplier?.Name,
                    ManufacturingDate = medicine.MedicineInventories.FirstOrDefault()?.ManufacturingDate,
                    ExpiryDate = medicine.MedicineInventories.FirstOrDefault()?.ExpiryDate
                };
                medicineDTOs.Add(medicineDTO);
            }
            return medicineDTOs;
        }
        //Get one medical supply
        public MedicineDTO? GetMedicineById(int medicineId)
        {
            var medicine = _medicineRepository.GetMedicine(medicineId);
            if (medicine == null)
                return null;
            var medicineDTO = new MedicineDTO
            {
                MedicineId = medicine.MedicineId,
                MedicineName = medicine.MedicineName,
                ActiveIngredient = medicine.ActiveIngredient,
                Dosage = medicine.Dosage,
                DosageForm = medicine.DosageForm,
                ImportPrice = medicine.ImportPrice,
                SellingPrice = medicine.SellingPrice,
                Quantity = _medicineRepository.GetMedicineQuantity(medicine.MedicineId),
                ShelfLife = medicine.ShelfLife,
                BatchNumber = medicine.MedicineInventories.FirstOrDefault()?.BatchNumber,
                BidNumber = medicine.BidNumber,
                SupplierName = medicine.MedicineInventories.FirstOrDefault()?.Supplier?.Name,
                ManufacturingDate = medicine.MedicineInventories.FirstOrDefault()?.ManufacturingDate,
                ExpiryDate = medicine.MedicineInventories.FirstOrDefault()?.ExpiryDate
            };
            return medicineDTO;
        }

        //Get medicine supply detail
        public List<MedicineInventoryDTO> MedicineDetail(int medicineId)
        {
            List<MedicineInventoryDTO> medicineInventoryDTOs = new List<MedicineInventoryDTO>();
            List<MedicineInventory> medicineInventories = _medicineRepository.GetMedicineDetail(medicineId);
            foreach (var medicineInventory in medicineInventories)
            {
                var medicineInventoryDTO = new MedicineInventoryDTO
                {
                    MedicineInventoryId = medicineInventory.MedicineInventoryId,
                    MedicineId = medicineInventory.MedicineId,
                    Quantity = medicineInventory.Quantity,
                    CertificateNumber = medicineInventory.CertificateNumber,
                    ManufacturingDate = medicineInventory.ManufacturingDate,
                    TransactionDate = medicineInventory.TransactionDate,
                    ExpiryDate = medicineInventory.ExpiryDate,
                    Note = medicineInventory.Note,
                    ReceiverId = medicineInventory.ReceiverId,
                    TransactionType = medicineInventory.TransactionType,
                    BatchNumber = medicineInventory.BatchNumber,
                    BidNumber = medicineInventory.Medicine.BidNumber,
                };
                medicineInventoryDTOs.Add(medicineInventoryDTO);
            }
            return medicineInventoryDTOs;
        }

        public bool AddMedicineInventory(MedicineInventoryAddDTO medicineInventoryDTO)
        {
            var medicine = new MedicineInventory
            {
                MedicineId = medicineInventoryDTO.MedicineId.Value,
                Quantity = medicineInventoryDTO.Quantity,
                CertificateNumber = medicineInventoryDTO.CertificateNumber,
                ManufacturingDate = medicineInventoryDTO.ManufacturingDate,
                TransactionDate = medicineInventoryDTO.TransactionDate,
                //ExpiryDate = _medicineRepository.CalculateExpiryDate(medicineInventoryDTO.ExpiryDate),
                Note = medicineInventoryDTO.Note,
                ReceiverId = medicineInventoryDTO.ReceiverId,
                TransactionType = medicineInventoryDTO.TransactionType,
                BatchNumber = medicineInventoryDTO.BatchNumber,
                SupplierId = medicineInventoryDTO.SupplierId
            };
            if (!_medicineRepository.AddMedicineInventory(medicine)) return false;
            return true;
        }

        public bool UpdateMedicineInventory(MedicineInventoryDTO medicineInventoryDTO)
        {
            var medicineInventory = new MedicineInventory
            {
                MedicineId = medicineInventoryDTO.MedicineId.Value,
                Quantity = medicineInventoryDTO.Quantity,
                CertificateNumber = medicineInventoryDTO.CertificateNumber,
                ManufacturingDate = medicineInventoryDTO.ManufacturingDate,
                TransactionDate = medicineInventoryDTO.TransactionDate,
                ExpiryDate = medicineInventoryDTO.ExpiryDate,
                Note = medicineInventoryDTO.Note,
                ReceiverId = medicineInventoryDTO.ReceiverId,
                TransactionType = medicineInventoryDTO.TransactionType,
                BatchNumber = medicineInventoryDTO.BatchNumber,
            };
            if (!_medicineRepository.UpdateMedicineInventory(medicineInventory)) return false;
            return true;
        }

        public async Task<List<MedicineSuggestionDTO>> GetMedicineSuggestions(string query)
        {
            // Lấy dữ liệu thuốc từ repository (Google Sheets)
            var data = await _medicineRepository.SearchMedicinesData(query);

            var suggestions = new List<MedicineSuggestionDTO>();

            // Lọc và ánh xạ dữ liệu vào DTO
            foreach (var row in data.table.rows)
            {
                var medicineName = GetValueFromJToken(row.c[1]);

                // Skip header row and empty rows
                if (string.IsNullOrEmpty(medicineName) || medicineName.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    continue;

                // Chỉ lấy những thuốc bắt đầu bằng từ khóa tìm kiếm (không phân biệt hoa thường)
                if (string.IsNullOrEmpty(query) || medicineName.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                {
                    var suggestion = new MedicineSuggestionDTO
                    {
                        MedicineName = medicineName,
                        ActiveIngredient = GetValueFromJToken(row.c[2]),
                        Dosage = GetValueFromJToken(row.c[3]),
                        DosageForm = GetValueFromJToken(row.c[6]),
                        UnitPrice = GetValueFromJToken(row.c[12]),
                        ShelfLife = GetValueFromJToken(row.c[14])
                    };
                    suggestions.Add(suggestion);
                }
            }

            // Sắp xếp kết quả theo tên thuốc để trả về danh sách có tổ chức
            return suggestions.OrderBy(s => s.MedicineName).ToList();
        }

        private string GetValueFromJToken(JToken token)
        {
            if (token == null)
                return "";

            // If the token is a JValue
            if (token is JValue jValue)
                return jValue.Value?.ToString() ?? "";

            // If the token is a JObject with 'v' property
            if (token["v"] != null)
                return token["v"].ToString();

            // As a last resort, try direct ToString()
            return token.ToString() ?? "";
        }
    }
}
