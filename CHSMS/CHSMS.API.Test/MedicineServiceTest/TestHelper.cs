using CHSMS.API.DTOs.Medicine;
using CHSMS.API.Models;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public static class TestHelper
    {
        public static Medicine CreateMedicine(int id = 1, string name = "TestMedicine", double importPrice = 10.0, bool status = true, bool isBhyt = true)
        {
            return new Medicine
            {
                MedicineId = id,
                MedicineName = name,
                ActiveIngredient = "TestIngredient",
                Dosage = "500mg",
                DosageForm = "Tablet",
                ImportPrice = importPrice,
                SellingPrice = 15.0,
                ShelfLife = 24,
                BidNumber = "BID123",
                Status = status,
                IsBhyt = isBhyt,
                MedicineCode = "MED001",
                MedicineInventories = new HashSet<MedicineInventory>()
            };
        }

        public static MedicineInventory CreateMedicineInventory(int id = 1, int medicineId = 1, double quantity = 100, string batchNumber = "BATCH001")
        {
            return new MedicineInventory
            {
                MedicineInventoryId = id,
                MedicineId = medicineId,
                Quantity = quantity,
                ImportQuantity = quantity,
                BatchNumber = batchNumber,
                ManufacturingDate = DateTime.Now.AddMonths(-6),
                ExpiryDate = DateTime.Now.AddMonths(18),
                TransactionDate = DateTime.Now,
                CertificateNumber = "CERT001",
                TransactionType = true,
                ReceiverId = 1,
                SupplierId = 1,
                Note = "Test Note",
                Medicine = CreateMedicine(medicineId)
            };
        }

        public static MedicineDTO CreateMedicineDTO(int id = 1, string name = "TestMedicine", double quantity = 100)
        {
            return new MedicineDTO
            {
                MedicineId = id,
                MedicineName = name,
                ActiveIngredient = "TestIngredient",
                Dosage = "500mg",
                DosageForm = "Tablet",
                Quantity = quantity,
                ImportPrice = 10.0,
                SellingPrice = 15.0,
                ManufacturingDate = DateTime.Now.AddMonths(-6),
                ExpiryDate = DateTime.Now.AddMonths(18),
                ShelfLife = 24,
                BatchNumber = "BATCH001",
                BidNumber = "BID123",
                Status = true,
                IsBhyt = true,
                MedicineCode = "MED001"
            };
        }

        public static MedicineInventoryGetAllDTO CreateMedicineInventoryGetAllDTO(int id = 1, string name = "TestMedicine", double quantity = 100)
        {
            return new MedicineInventoryGetAllDTO
            {
                MedicineId = id,
                MedicineName = name,
                ActiveIngredient = "TestIngredient",
                Dosage = "500mg",
                DosageForm = "Tablet",
                ImportPrice = 10.0,
                BidNumber = "BID123",
                BatchNumber = "BATCH001",
                Quantity = quantity,
                ManufacturingDate = DateTime.Now.AddMonths(-6),
                ExpiryDate = DateTime.Now.AddMonths(18),
                IsBhyt = true,
                Status = true
            };
        }

        public static User CreateUser(int id = 1, string name = "TestUser")
        {
            return new User
            {
                UserId = id,
                UserName = name
            };
        }

        public static Supplier CreateSupplier(int id = 1, string name = "TestSupplier")
        {
            return new Supplier
            {
                SupplierId = id,
                Name = name
            };
        }

        public static MedicineInventoryDetailDTO CreateMedicineInventoryDetailDTO(int id = 1, int medicineId = 1, double quantity = 100)
        {
            return new MedicineInventoryDetailDTO
            {
                MedicineInventoryId = id,
                MedicineId = medicineId,
                MedicineName = "TestMedicine",
                Quantity = quantity,
                ImportQuantity = quantity,
                TransactionType = true,
                Note = "Test Note",
                CertificateNumber = "CERT001",
                ExpiryDate = DateTime.Now.AddMonths(18),
                ReceiverId = 1,
                ReceiverName = "TestUser",
                TransactionDate = DateTime.Now,
                SupplierId = 1,
                SupplierName = "TestSupplier",
                ManufacturingDate = DateTime.Now.AddMonths(-6),
                BatchNumber = "BATCH001"
            };
        }

        public static MedicineConsumption CreateMedicineConsumption(int id = 1, int inventoryId = 1, double amount = 50)
        {
            return new MedicineConsumption
            {
                MedicineConsumptionId = id,
                MedicineInventoryId = inventoryId,
                Amount = amount,
                ConsumptionDate = DateTime.Now,
                Status = true,
                Note = "Test Consumption"
            };
        }

        public static MedicineInventoryAddDTO CreateMedicineInventoryAddDTO(int medicineId = 1, double quantity = 100)
        {
            return new MedicineInventoryAddDTO
            {
                MedicineId = medicineId,
                CertificateNumber = "CERT001",
                TransactionType = true,
                ImportQuantity = quantity,
                ManufacturingDate = DateTime.Now.AddMonths(-6),
                TransactionDate = DateTime.Now,
                Note = "Test Note",
                BatchNumber = "BATCH001",
                SupplierId = 1
            };
        }

        public static ConsumeMedicineDTO CreateConsumeMedicineDTO(int consumptionId = 1, int inventoryId = 1, double quantity = 50)
        {
            return new ConsumeMedicineDTO
            {
                ConsumeMedicineId = consumptionId,
                MedicineInventoryId = inventoryId,
                Quantity = quantity,
                Status = true,
                Note = "Test Consumption"
            };
        }

        public static MedicineInventoryUpdateDTO CreateMedicineInventoryUpdateDTO(int inventoryId = 1, int medicineId = 1, double quantity = 100)
        {
            return new MedicineInventoryUpdateDTO
            {
                MedicineInventoryId = inventoryId,
                MedicineId = medicineId,
                CertificateNumber = "CERT001",
                TransactionType = true,
                Quantity = quantity,
                ImportQuantity = quantity,
                ManufacturingDate = DateTime.Now.AddMonths(-6),
                ExpiryDate = DateTime.Now.AddMonths(18),
                TransactionDate = DateTime.Now,
                Note = "Updated Note",
                BatchNumber = "BATCH001",
                SupplierId = 1
            };
        }

        public static MedicineInventoryUpdateHistoryDTO CreateMedicineInventoryUpdateHistoryDTO(int inventoryId = 1, int medicineId = 1)
        {
            return new MedicineInventoryUpdateHistoryDTO
            {
                MedicineInventoryId = inventoryId,
                MedicineId = medicineId,
                CertificateNumber = "CERT001",
                ManufacturingDate = DateTime.Now.AddMonths(-6),
                ExpiryDate = DateTime.Now.AddMonths(18),
                TransactionType = true,
                BatchNumber = "BATCH001",
                SupplierId = 1,
                Note = "Test Note",
                ImportQuantity = 100,
                Quantity = 100,
                TransactionDate = DateTime.Now,
                CanEdit = true,
                CanEditNote = true,
                CanEditImportQuantity = true,
                CanEditManufacturingDate = true
            };
        }
    }
}
