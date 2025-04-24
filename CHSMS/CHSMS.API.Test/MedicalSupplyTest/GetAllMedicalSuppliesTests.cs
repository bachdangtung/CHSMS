using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class GetAllMedicalSuppliesTests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;

        public GetAllMedicalSuppliesTests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);
        }

        [Fact]
        public void GetAllMedicalSupplies_ReturnsListOfMedicalSupplyDTOs_WithQuantities()
        {
            // Arrange
            var supplies = GetSampleMedicalSupplies();

            _mockRepository.Setup(repo => repo.GetAllMedicalSupplies()).Returns(supplies);
            _mockRepository.Setup(repo => repo.GetMSQantityByID(1)).Returns(50.0);
            _mockRepository.Setup(repo => repo.GetMSQantityByID(2)).Returns(100.0);

            // Act
            var result = _service.GetAllMedicalSupplies();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].MedicalSupplyId);
            Assert.Equal("Bandage", result[0].MedicalSupplyName);
            Assert.Equal(50.0, result[0].Quantity);

            Assert.Equal(2, result[1].MedicalSupplyId);
            Assert.Equal("Syringe", result[1].MedicalSupplyName);
            Assert.Equal(100.0, result[1].Quantity);
        }

        [Fact]
        public void GetAllMedicalSupplies_ReturnsEmptyList_WhenNoDataInDatabase()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllMedicalSupplies())
                           .Returns(new List<MedicalSupply>()); // empty list

            // Act
            var result = _service.GetAllMedicalSupplies();

            // Assert
            Assert.NotNull(result); // still should return a list, just empty
            Assert.Empty(result);   // verify it's empty
        }


        private List<MedicalSupply> GetSampleMedicalSupplies()
        {
            return new List<MedicalSupply>
    {
        new MedicalSupply
        {
            MedicalSupplyId = 1,
            MedicalSupplyName = "Bandage",
            SupplyType = "FirstAid",
            UnitOfMeasure = "Box",
            SupplierId = 10,
            Status = true,
            ImportPrice = 5.0,
            SellingPrice = 10.0,
            BidNumber = 1001
        },
        new MedicalSupply
        {
            MedicalSupplyId = 2,
            MedicalSupplyName = "Syringe",
            SupplyType = "Injection",
            UnitOfMeasure = "Piece",
            SupplierId = 11,
            Status = true,
            ImportPrice = 1.5,
            SellingPrice = 3.0,
            BidNumber = 1002
        }
    };
        }

    }
}
