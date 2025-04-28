using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Moq;

namespace CHSMS.API.Tests
{
    public class GetAllMedicalSupplyConsumptionsAsyncTests
    {
        private readonly Mock<IUseMedicalSupplyRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly IUseMedicalSupplyService _service;

        public GetAllMedicalSupplyConsumptionsAsyncTests()
        {
            var mocks = TestHelper.CreateMocks();
            _repositoryMock = mocks.Repository;
            _contextMock = mocks.Context;
            _service = new UseMedicalSupplyService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetAllMedicalSupplyConsumptionsAsync_ReturnsList()
        {
            // Arrange
            var umsmsc = new UseMedicalSuppliesMedicalSupplyConsumption
            {
                UseMedicalSupplieId = 1,
                MsconsumptionId = 1,
                TotalPrice = 50,
                Msconsumption = TestHelper.CreateMedicalSupplyConsumption(1, 1, 5)
            };
            umsmsc.Msconsumption.MedicalSupplyInventory = TestHelper.CreateMedicalSupplyInventory(1, 1, 20);
            umsmsc.Msconsumption.MedicalSupplyInventory.MedicalSupply = new MedicalSupply
            {
                MedicalSupplyId = 1,
                MedicalSupplyName = "Test Supply",
                MedicalSupplyCode = "TS001",
                UnitOfMeasure = "Unit"
            };
            umsmsc.Msconsumption.MedicalSupplyInventory.BatchNumber = "BN001";
            umsmsc.Msconsumption.MedicalSupplyInventory.TransactionDate = DateTime.Now.AddDays(-2);
            umsmsc.Msconsumption.MedicalSupplyInventory.ExpiryDate = DateTime.Now.AddYears(1);

            var umsmscList = new List<UseMedicalSuppliesMedicalSupplyConsumption> { umsmsc };

            _repositoryMock.Setup(r => r.GetAllMedicalSupplyConsumptionsAsync()).ReturnsAsync(umsmscList);

            // Act
            var result = await _service.GetAllMedicalSupplyConsumptionsAsync();

            // Assert
            Assert.Single(result);
            var dto = result[0];
            Assert.Equal(1, dto.MedicalSupplyConsumptionId);
            Assert.Equal(1, dto.MedicalSupplyInventoryId);
            Assert.Equal("Test Supply", dto.MedicalSupplyName);
            Assert.Equal("TS001", dto.MedicalSupplyCode);
            Assert.Equal("Unit", dto.UnitOfMeasure);
            Assert.Equal(5, dto.Amount);
            Assert.Equal(50, dto.TotalPrice);
            Assert.Equal("BN001", dto.BatchNumber);
            Assert.Equal(1, dto.UseMedicalSupplieId);
        }

        [Fact]
        public async Task GetAllMedicalSupplyConsumptionsAsync_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllMedicalSupplyConsumptionsAsync()).ReturnsAsync(new List<UseMedicalSuppliesMedicalSupplyConsumption>());

            // Act
            var result = await _service.GetAllMedicalSupplyConsumptionsAsync();

            // Assert
            Assert.Empty(result);
        }
    }
}