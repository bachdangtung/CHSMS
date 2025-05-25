using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Moq;

namespace CHSMS.API.Test.PrescriptionServiceTest
{
    public class GetAllMedicinesInInventoryAsyncTests
    {
        private readonly Mock<IPrescriptionRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly IPrescriptionService _service;

        public GetAllMedicinesInInventoryAsyncTests()
        {
            _repositoryMock = new Mock<IPrescriptionRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _service = new PrescriptionService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetAllMedicinesInInventoryAsync_EmptyInventory_ReturnsEmptyList()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAvailableMedicinesAsync())
                           .ReturnsAsync(new List<MedicineInventory>());

            // Act
            var result = await _service.GetAllMedicinesInInventoryAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllMedicinesInInventoryAsync_ValidInventory_ReturnsMappedList()
        {
            // Arrange
            var inventories = new List<MedicineInventory>
            {
                new MedicineInventory
                {
                    MedicineInventoryId = 1,
                    MedicineId = 1,
                    Quantity = 100,
                    ExpiryDate = DateTime.Now.AddDays(30),
                    Medicine = new Medicine { MedicineName = "Paracetamol", ActiveIngredient = "Paracetamol", Dosage = "500mg", DosageForm = "Tablet", IsBhyt = true }
                }
            };
            _repositoryMock.Setup(r => r.GetAvailableMedicinesAsync())
                           .ReturnsAsync(inventories);

            // Act
            var result = await _service.GetAllMedicinesInInventoryAsync();

            // Assert
            Assert.Single(result);
            var dto = result[0];
            Assert.Equal(1, dto.MedicineId);
            Assert.Equal("Paracetamol", dto.MedicineName);
            Assert.Equal(100, dto.Quantity);
            Assert.True(dto.IsBhyt);
        }
    }
}