using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Moq;

namespace CHSMS.API.Test.PrescriptionServiceTest
{
    public class GetAllMedicineConsumptionsAsyncTests
    {
        private readonly Mock<IPrescriptionRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly IPrescriptionService _service;

        public GetAllMedicineConsumptionsAsyncTests()
        {
            _repositoryMock = new Mock<IPrescriptionRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _service = new PrescriptionService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetAllMedicineConsumptionsAsync_NoConsumptions_ReturnsEmptyList()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllMedicineConsumptionsAsync())
                           .ReturnsAsync(new List<PrescriptionMedicineConsumption>());

            // Act
            var result = await _service.GetAllMedicineConsumptionsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllMedicineConsumptionsAsync_ValidConsumptions_ReturnsMappedList()
        {
            // Arrange
            var pmcs = new List<PrescriptionMedicineConsumption>
            {
                new PrescriptionMedicineConsumption
                {
                    PrescriptionId = 1,
                    MedicineConsumtionId = 1,
                    TotalPrice = 100,
                    MedicineConsumtion = new MedicineConsumption
                    {
                        MedicineConsumptionId = 1,
                        MedicineInventoryId = 1,
                        Amount = 10,
                        ConsumptionDate = DateTime.Now,
                        Status = true,
                        MedicineInventory = new MedicineInventory
                        {
                            MedicineInventoryId = 1,
                            Medicine = new Medicine
                            {
                                MedicineName = "Paracetamol",
                                MedicineCode = "P001",
                                ActiveIngredient = "Paracetamol",
                                Dosage = "500mg",
                                DosageForm = "Tablet"
                            },
                            BatchNumber = "B123",
                            ExpiryDate = DateTime.Now.AddDays(30)
                        }
                    }
                }
            };
            _repositoryMock.Setup(r => r.GetAllMedicineConsumptionsAsync())
                           .ReturnsAsync(pmcs);

            // Act
            var result = await _service.GetAllMedicineConsumptionsAsync();

            // Assert
            Assert.Single(result);
            var dto = result[0];
            Assert.Equal(1, dto.MedicineConsumptionId);
            Assert.Equal("Paracetamol", dto.MedicineName);
            Assert.Equal(100, dto.TotalPrice);
        }
    }
}