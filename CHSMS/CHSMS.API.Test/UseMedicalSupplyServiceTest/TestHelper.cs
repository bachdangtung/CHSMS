using CHSMS.API.DTOs.MedicalSupplyConsumption;
using CHSMS.API.DTOs.UseMedicalSupply;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace CHSMS.API.Tests
{
    public static class TestHelper
    {
        public static (Mock<IUseMedicalSupplyRepository> Repository, Mock<SEP_TestContext> Context, Mock<DbContextOptions<SEP_TestContext>> Options) CreateMocks()
        {
            var repositoryMock = new Mock<IUseMedicalSupplyRepository>();
            var contextMock = new Mock<SEP_TestContext>();
            var optionsMock = new Mock<DbContextOptions<SEP_TestContext>>();

            var databaseMock = new Mock<DatabaseFacade>(contextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();
            databaseMock.Setup(d => d.BeginTransactionAsync(default)).ReturnsAsync(transactionMock.Object);
            contextMock.Setup(c => c.Database).Returns(databaseMock.Object);

            return (repositoryMock, contextMock, optionsMock);
        }

        public static CreateUseMedicalSupplyDTO CreateValidUseMedicalSupplyDTO()
        {
            return new CreateUseMedicalSupplyDTO
            {
                IssueDate = DateTime.Now.AddDays(-1),
                Note = "Test note",
                MedicalSupplyConsumptions = new List<MedicalSupplyConsumptionDTO>
                {
                    new MedicalSupplyConsumptionDTO
                    {
                        MedicalSupplyInventoryId = 1,
                        Amount = 5,
                        ConsumptionDate = DateTime.Now.AddDays(-1),
                        Note = "Consumption note"
                    }
                }
            };
        }

        public static MedicalSupplyInventory CreateMedicalSupplyInventory(int id, int medicalSupplyId, int quantity, DateTime? expiryDate = null)
        {
            return new MedicalSupplyInventory
            {
                SupplyInventoryId = id,
                MedicalSupplyId = medicalSupplyId,
                Quantity = quantity,
                ExpiryDate = expiryDate ?? DateTime.Now.AddYears(1),
                MedicalSupply = new MedicalSupply { MedicalSupplyId = medicalSupplyId, SellingPrice = 10 }
            };
        }

        public static UseMedicalSupply CreateUseMedicalSupply(int id, int userId, int medicalRecordHistoryId, bool status = false, DateTime? issueDate = null)
        {
            return new UseMedicalSupply
            {
                UseMedicalSupplieId = id,
                UserId = userId,
                MedicalRecordHistoryId = medicalRecordHistoryId,
                IssueDate = issueDate ?? DateTime.Now.AddDays(-1),
                Status = status,
                Note = "Test note"
            };
        }

        public static MedicalSupplyConsumption CreateMedicalSupplyConsumption(int id, int inventoryId, int amount, bool status = false)
        {
            return new MedicalSupplyConsumption
            {
                MsconsumptionId = id,
                MedicalSupplyInventoryId = inventoryId,
                Amount = amount,
                ConsumptionDate = DateTime.Now.AddDays(-1),
                Status = status,
                Note = "Consumption note"
            };
        }

        public static EditUseMedicalSupplyForDoctorDTO CreateValidEditDoctorDTO(int useMedicalSupplyId)
        {
            return new EditUseMedicalSupplyForDoctorDTO
            {
                UseMedicalSupplyId = useMedicalSupplyId,
                MedicalRecordHistoryId = 1,
                UserId = 1,
                IssueDate = DateTime.Now.AddDays(-1),
                Note = "Updated note",
                MedicalSupplyConsumptionIdsToRemove = new List<int>(),
                MedicalSupplyConsumptionsToAdd = new List<MedicalSupplyConsumptionDTO>
                {
                    new MedicalSupplyConsumptionDTO
                    {
                        MedicalSupplyInventoryId = 1,
                        Amount = 5,
                        ConsumptionDate = DateTime.Now.AddDays(-1),
                        Note = "New consumption"
                    }
                }
            };
        }

        public static EditUseMedicalSupplyForPharmacistDTO CreateValidEditPharmacistDTO(int useMedicalSupplyId)
        {
            return new EditUseMedicalSupplyForPharmacistDTO
            {
                UseMedicalSupplyId = useMedicalSupplyId,
                MedicalSupplyConsumptionStatuses = new List<DTOs.UseMedicalSupply.MedicalSupplyConsumptionStatusDTO>
                {
                    new DTOs.UseMedicalSupply.MedicalSupplyConsumptionStatusDTO
                    {
                        MedicalSupplyConsumptionId = 1,
                        Status = true
                    }
                }
            };
        }
    }
}