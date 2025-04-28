using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Moq;

namespace CHSMS.API.Tests
{
    public class GetAllUseMedicalSuppliesAsyncTests
    {
        private readonly Mock<IUseMedicalSupplyRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly IUseMedicalSupplyService _service;

        public GetAllUseMedicalSuppliesAsyncTests()
        {
            var mocks = TestHelper.CreateMocks();
            _repositoryMock = mocks.Repository;
            _contextMock = mocks.Context;
            _service = new UseMedicalSupplyService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetAllUseMedicalSuppliesAsync_ReturnsList()
        {
            // Arrange
            var useMedicalSupplies = new List<UseMedicalSupply>
            {
                TestHelper.CreateUseMedicalSupply(1, 1, 1)
            };
            _repositoryMock.Setup(r => r.GetAllUseMedicalSuppliesAsync()).ReturnsAsync(useMedicalSupplies);

            // Act
            var result = await _service.GetAllUseMedicalSuppliesAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].UseMedicalSupplyId);
        }

        [Fact]
        public async Task GetAllUseMedicalSuppliesAsync_ReturnsEmptyList()
        {
            // Arrange
            var useMedicalSupplies = new List<UseMedicalSupply>
            {
            };
            _repositoryMock.Setup(r => r.GetAllUseMedicalSuppliesAsync()).ReturnsAsync(useMedicalSupplies);

            // Act
            var result = await _service.GetAllUseMedicalSuppliesAsync();

            // Assert
            Assert.Empty(result);
        }
    }
}