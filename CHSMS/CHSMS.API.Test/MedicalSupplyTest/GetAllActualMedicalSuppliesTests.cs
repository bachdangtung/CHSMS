using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class GetAllActualMedicalSuppliesTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public GetAllActualMedicalSuppliesTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);

            // Setup precondition: MedicalSupplyInventories and MedicalSupplyConsumption
            var medicalSupplies = new List<MedicalSupply>
            {
                new MedicalSupply { MedicalSupplyId = 1, MedicalSupplyName = "Supply1" },
                new MedicalSupply { MedicalSupplyId = 2, MedicalSupplyName = "Supply2" }
            };

            _mockRepository.Setup(r => r.GetAllMedicalSupplies())
                .Returns(medicalSupplies);

            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryByMSID(1))
                .Returns(new List<MedicalSupplyInventory>
                {
                    new MedicalSupplyInventory { SupplyInventoryId = 1, MedicalSupplyId = 1, Quantity = 5 }
                });

            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryByMSID(2))
                .Returns(new List<MedicalSupplyInventory>
                {
                    new MedicalSupplyInventory { SupplyInventoryId = 2, MedicalSupplyId = 2, Quantity = -5 }
                });

            _mockRepository.Setup(r => r.MSConsumptionDetail(1, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Returns(new List<MedicalSupplyConsumption>
                {
                    new MedicalSupplyConsumption
                    {
                        MedicalSupplyInventoryId = 1,
                        Amount = 5,
                        ConsumptionDate = new DateTime(2025, 5, 5)
                    }
                });
        }

        [Fact]
        public void GetAllActualMedicalSupplies_NullDate_ValidInventory_ReturnsCorrectQuantity()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetMSQantityByID(1)).Returns(5.0);
            _mockRepository.Setup(r => r.GetMSQantityByID(2)).Returns(-5.0);

            // Act
            var result = _service.GetAllActualMedicalSupplies(null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].MedicalSupplyId);
            Assert.Equal(5.0, result[0].Quantity);
            Assert.Equal(2, result[1].MedicalSupplyId);
            Assert.Equal(-5.0, result[1].Quantity);
            _mockRepository.Verify(r => r.GetMSQantityByID(1), Times.Once());
            _mockRepository.Verify(r => r.GetMSQantityByID(2), Times.Once());
        }

        [Fact]
        public void GetAllActualMedicalSupplies_NullDate_NegativeQuantityInventory_ReturnsNegativeQuantity()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetMSQantityByID(2)).Returns(-5.0);

            // Act
            var result = _service.GetAllActualMedicalSupplies(null);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, dto => dto.MedicalSupplyId == 2 && dto.Quantity == -5.0);
            _mockRepository.Verify(r => r.GetMSQantityByID(2), Times.Once());
        }

        [Fact]
        public void GetAllActualMedicalSupplies_NullDate_WithConsumption_ReturnsCorrectQuantity()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetMSQantityByID(1)).Returns(5.0);

            // Act
            var result = _service.GetAllActualMedicalSupplies(null);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, dto => dto.MedicalSupplyId == 1 && dto.Quantity == 5.0);
            _mockRepository.Verify(r => r.GetMSQantityByID(1), Times.Once());
            _mockRepository.Verify(r => r.MSConsumptionDetail(It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Never());
        }

        [Fact]
        public void GetAllActualMedicalSupplies_DateProvided_CalculatesActualQuantity()
        {
            // Arrange
            var inputDate = new DateTime(2025, 4, 4);
            _mockRepository.Setup(r => r.GetActualMSQuantity(1, inputDate)).Returns(2.0);
            _mockRepository.Setup(r => r.GetActualMSQuantity(2, inputDate)).Returns(-7.0);

            // Act
            var result = _service.GetAllActualMedicalSupplies(inputDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].MedicalSupplyId);
            Assert.Equal(2.0, result[0].Quantity);
            Assert.Equal(2, result[1].MedicalSupplyId);
            Assert.Equal(-7.0, result[1].Quantity);
            _mockRepository.Verify(r => r.GetActualMSQuantity(1, inputDate), Times.Once());
            _mockRepository.Verify(r => r.GetActualMSQuantity(2, inputDate), Times.Once());
            _mockRepository.Verify(r => r.GetMSQantityByID(It.IsAny<int>()), Times.Never());
        }
    }
}