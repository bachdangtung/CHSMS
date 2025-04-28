using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Moq;

namespace CHSMS.API.Tests
{
    public class GetUseMedicalSuppliesByMedicalRecordHistoryIdAsyncTests
    {
        private readonly Mock<IUseMedicalSupplyRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly IUseMedicalSupplyService _service;

        public GetUseMedicalSuppliesByMedicalRecordHistoryIdAsyncTests()
        {
            var mocks = TestHelper.CreateMocks();
            _repositoryMock = mocks.Repository;
            _contextMock = mocks.Context;
            _service = new UseMedicalSupplyService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetUseMedicalSuppliesByMedicalRecordHistoryIdAsync_ReturnsList()
        {
            // Arrange
            var useMedicalSupplies = new List<UseMedicalSupply>
            {
                TestHelper.CreateUseMedicalSupply(1, 1, 1)
            };
            _repositoryMock.Setup(r => r.GetUseMedicalSuppliesByMedicalRecordHistoryIdAsync(1)).ReturnsAsync(useMedicalSupplies);

            // Act
            var result = await _service.GetUseMedicalSuppliesByMedicalRecordHistoryIdAsync(1);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].UseMedicalSupplyId);
        }

        [Fact]
        public async Task GetUseMedicalSuppliesByMedicalRecordHistoryIdAsync_NoRecords_ThrowsException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetUseMedicalSuppliesByMedicalRecordHistoryIdAsync(999)).ReturnsAsync(new List<UseMedicalSupply>());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetUseMedicalSuppliesByMedicalRecordHistoryIdAsync(999));
        }
    }
}