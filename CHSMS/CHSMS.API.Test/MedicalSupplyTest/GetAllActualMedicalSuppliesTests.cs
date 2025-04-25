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
        }

        [Fact]
        public void GetAllActualMedicalSupplies_ReturnsDTOList_WhenDateIsProvided()
        {
            // Arrange
            DateTime date = DateTime.Now;
            var medicalSupplies = GetSampleMedicalSupplies();
            _mockRepository.Setup(repo => repo.GetAllMedicalSupplies()).Returns(medicalSupplies);
            _mockRepository.Setup(repo => repo.GetActualMSQuantity(1, date)).Returns(50.0);
            _mockRepository.Setup(repo => repo.GetActualMSQuantity(2, date)).Returns(100.0);

            // Act
            var result = _service.GetAllActualMedicalSupplies(date);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Supply1", result[0].MedicalSupplyName);
            Assert.Equal(50.0, result[0].Quantity);
            Assert.Equal("Supply2", result[1].MedicalSupplyName);
            Assert.Equal(100.0, result[1].Quantity);
            _mockRepository.Verify(repo => repo.GetAllMedicalSupplies(), Times.Once());
            _mockRepository.Verify(repo => repo.GetActualMSQuantity(It.IsAny<int>(), date), Times.Exactly(2));
        }

        [Fact]
        public void GetAllActualMedicalSupplies_ReturnsAllSupplies_WhenDateIsNull()
        {
            // Arrange
            DateTime? date = null;
            var medicalSupplies = GetSampleMedicalSupplies();
            _mockRepository.Setup(repo => repo.GetAllMedicalSupplies()).Returns(medicalSupplies);

            // Act
            var result = _service.GetAllActualMedicalSupplies(date);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Supply1", result[0].MedicalSupplyName);
            Assert.Null(result[0].Quantity); // Quantity not set when date is null
            _mockRepository.Verify(repo => repo.GetAllMedicalSupplies(), Times.Once());
            _mockRepository.Verify(repo => repo.GetActualMSQuantity(It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never());
        }

        [Fact]
        public void GetAllActualMedicalSupplies_ReturnsEmptyList_WhenNoSuppliesExist()
        {
            // Arrange
            DateTime date = DateTime.Now;
            var emptyList = new List<MedicalSupply>();
            _mockRepository.Setup(repo => repo.GetAllMedicalSupplies()).Returns(emptyList);

            // Act
            var result = _service.GetAllActualMedicalSupplies(date);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(repo => repo.GetAllMedicalSupplies(), Times.Once());
            _mockRepository.Verify(repo => repo.GetActualMSQuantity(It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never());
        }

        private List<MedicalSupply> GetSampleMedicalSupplies()
        {
            return new List<MedicalSupply>
            {
                new MedicalSupply
                {
                    MedicalSupplyId = 1,
                    MedicalSupplyName = "Supply1",
                    SupplyType = "Type1",
                    UnitOfMeasure = "Unit1",
                    SupplierId = 101,
                    Status = true,
                    ImportPrice = 10.0,
                    SellingPrice = 15.0,
                    BidNumber = 1001
                },
                new MedicalSupply
                {
                    MedicalSupplyId = 2,
                    MedicalSupplyName = "Supply2",
                    SupplyType = "Type2",
                    UnitOfMeasure = "Unit2",
                    SupplierId = 102,
                    Status = true,
                    ImportPrice = 20.0,
                    SellingPrice = 30.0,
                    BidNumber = 1002
                }
            };
        }
    }
}