using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Moq;

namespace CHSMS.API.Tests
{
    public class GetAllMedicalSuppliesInInventoryAsyncTests
    {
        private readonly Mock<IUseMedicalSupplyRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly IUseMedicalSupplyService _service;

        public GetAllMedicalSuppliesInInventoryAsyncTests()
        {
            var mocks = TestHelper.CreateMocks();
            _repositoryMock = mocks.Repository;
            _contextMock = mocks.Context;
            _service = new UseMedicalSupplyService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetAllMedicalSuppliesInInventoryAsync_ReturnsList()
        {
            // Arrange
            var inventories = new List<MedicalSupplyInventory>
            {
                TestHelper.CreateMedicalSupplyInventory(1, 1, 20)
            };
            _repositoryMock.Setup(r => r.GetAvailableMedicalSuppliesAsync()).ReturnsAsync(inventories);

            // Act
            var result = await _service.GetAllMedicalSuppliesInInventoryAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].MedicalSupplyInventoryId);
        }

        [Fact]
        public async Task GetAllMedicalSuppliesInInventoryAsync_ReturnsEmptyList()
        {
            // Arrange
            var inventories = new List<MedicalSupplyInventory>();
            _repositoryMock.Setup(r => r.GetAvailableMedicalSuppliesAsync()).ReturnsAsync(inventories);

            // Act
            var result = await _service.GetAllMedicalSuppliesInInventoryAsync();

            // Assert
            Assert.Empty(result);
        }
    }
}