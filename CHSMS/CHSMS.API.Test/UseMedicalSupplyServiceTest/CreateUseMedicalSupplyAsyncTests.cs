using CHSMS.API.DTOs.MedicalSupplyConsumption;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Moq;

namespace CHSMS.API.Tests
{
    public class CreateUseMedicalSupplyAsyncTests
    {
        private readonly Mock<IUseMedicalSupplyRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly IUseMedicalSupplyService _service;

        public CreateUseMedicalSupplyAsyncTests()
        {
            var mocks = TestHelper.CreateMocks();
            _repositoryMock = mocks.Repository;
            _contextMock = mocks.Context;
            _service = new UseMedicalSupplyService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task CreateUseMedicalSupplyAsync_ValidInput_ReturnsId()
        {
            // Arrange
            var dto = TestHelper.CreateValidUseMedicalSupplyDTO();
            var inventory = TestHelper.CreateMedicalSupplyInventory(1, 1, 20);
            var createdUseMedicalSupply = TestHelper.CreateUseMedicalSupply(1, 1, 1);
            var createdConsumption = TestHelper.CreateMedicalSupplyConsumption(1, 1, 5);
            var createdUmsmsc = new UseMedicalSuppliesMedicalSupplyConsumption
            {
                UseMedicalSupplieId = 1,
                MsconsumptionId = 1,
                TotalPrice = 0
            };

            _repositoryMock.Setup(r => r.GetMedicalSupplyInventoryByIdAsync(1)).ReturnsAsync(inventory);
            _repositoryMock.Setup(r => r.CreateUseMedicalSupplyAsync(It.IsAny<UseMedicalSupply>())).ReturnsAsync(createdUseMedicalSupply);
            _repositoryMock.Setup(r => r.CreateMedicalSupplyConsumptionAsync(It.IsAny<MedicalSupplyConsumption>())).ReturnsAsync(createdConsumption);
            _repositoryMock.Setup(r => r.CreateUseMedicalSuppliesMedicalSupplyConsumptionAsync(It.IsAny<UseMedicalSuppliesMedicalSupplyConsumption>())).ReturnsAsync(createdUmsmsc);

            // Act
            var result = await _service.CreateUseMedicalSupplyAsync(1, 1, dto);

            // Assert
            Assert.Equal(1, result);
            _contextMock.Verify(c => c.Database.BeginTransactionAsync(default), Times.Once());
            _repositoryMock.Verify(r => r.CreateUseMedicalSupplyAsync(It.IsAny<UseMedicalSupply>()), Times.Once());
            _repositoryMock.Verify(r => r.CreateMedicalSupplyConsumptionAsync(It.IsAny<MedicalSupplyConsumption>()), Times.Once());
            _repositoryMock.Verify(r => r.CreateUseMedicalSuppliesMedicalSupplyConsumptionAsync(It.IsAny<UseMedicalSuppliesMedicalSupplyConsumption>()), Times.Once());
        }

        [Fact]
        public async Task CreateUseMedicalSupplyAsync_FutureIssueDate_ThrowsException()
        {
            var dto = TestHelper.CreateValidUseMedicalSupplyDTO();
            dto.IssueDate = DateTime.Now.AddDays(1);

            var exception = await Assert.ThrowsAsync<Exception>(() => _service.CreateUseMedicalSupplyAsync(1, 1, dto));

            Assert.Contains("Ngày phát hành không được là ngày trong tương lai!", exception.Message);
        }

        [Fact]
        public async Task CreateUseMedicalSupplyAsync_MoreThanTenMedicalSupplies_ThrowsException()
        {
            var dto = TestHelper.CreateValidUseMedicalSupplyDTO();
            dto.MedicalSupplyConsumptions = Enumerable.Range(1, 11).Select(i => new MedicalSupplyConsumptionDTO
            {
                MedicalSupplyInventoryId = i,
                Amount = 5,
                ConsumptionDate = DateTime.Now.AddDays(-1)
            }).ToList();

            _repositoryMock.Setup(r => r.GetMedicalSupplyInventoryByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => TestHelper.CreateMedicalSupplyInventory(id, id, 20));

            var exception = await Assert.ThrowsAsync<Exception>(() => _service.CreateUseMedicalSupplyAsync(1, 1, dto));

            Assert.Contains("Một đơn vật tư không được chứa quá 10 loại vật tư!", exception.Message);
        }

        [Fact]
        public async Task CreateUseMedicalSupplyAsync_DuplicateInventoryIds_ThrowsException()
        {
            var dto = TestHelper.CreateValidUseMedicalSupplyDTO();
            dto.MedicalSupplyConsumptions.Add(new MedicalSupplyConsumptionDTO
            {
                MedicalSupplyInventoryId = 1,
                Amount = 5,
                ConsumptionDate = DateTime.Now.AddDays(-1)
            });

            _repositoryMock.Setup(r => r.GetMedicalSupplyInventoryByIdAsync(1))
                .ReturnsAsync(TestHelper.CreateMedicalSupplyInventory(1, 1, 20));

            var exception = await Assert.ThrowsAsync<Exception>(() => _service.CreateUseMedicalSupplyAsync(1, 1, dto));

            Assert.Contains("Có vật tư bị trùng trong đơn vật tư. Vui lòng kiểm tra lại!", exception.Message);
        }

        [Fact]
        public async Task CreateUseMedicalSupplyAsync_InsufficientInventory_ThrowsException()
        {
            var dto = TestHelper.CreateValidUseMedicalSupplyDTO();
            dto.MedicalSupplyConsumptions[0].Amount = 25;
            var inventory = TestHelper.CreateMedicalSupplyInventory(1, 1, 20);

            _repositoryMock.Setup(r => r.GetMedicalSupplyInventoryByIdAsync(1)).ReturnsAsync(inventory);

            var exception = await Assert.ThrowsAsync<Exception>(() => _service.CreateUseMedicalSupplyAsync(1, 1, dto));

            Assert.Contains("Số lượng yêu cầu vượt quá tồn kho", exception.Message);
        }

        [Fact]
        public async Task CreateUseMedicalSupplyAsync_ExpiredInventory_ThrowsException()
        {
            var dto = TestHelper.CreateValidUseMedicalSupplyDTO();
            var inventory = TestHelper.CreateMedicalSupplyInventory(1, 1, 20, DateTime.Now.AddDays(-2));

            _repositoryMock.Setup(r => r.GetMedicalSupplyInventoryByIdAsync(1)).ReturnsAsync(inventory);

            var exception = await Assert.ThrowsAsync<Exception>(() => _service.CreateUseMedicalSupplyAsync(1, 1, dto));

            Assert.Contains("Ngày sử dụng vượt quá hạn sử dụng", exception.Message);
        }

        [Fact]
        public async Task CreateUseMedicalSupplyAsync_BelowMinimumQuantity_ThrowsException()
        {
            var dto = TestHelper.CreateValidUseMedicalSupplyDTO();
            dto.MedicalSupplyConsumptions[0].Amount = 15;
            var inventory = TestHelper.CreateMedicalSupplyInventory(1, 1, 20);

            _repositoryMock.Setup(r => r.GetMedicalSupplyInventoryByIdAsync(1)).ReturnsAsync(inventory);

            var exception = await Assert.ThrowsAsync<Exception>(() => _service.CreateUseMedicalSupplyAsync(1, 1, dto));

            Assert.Contains($"Số lượng tồn kho của vật tư ID {dto.MedicalSupplyConsumptions[0].MedicalSupplyInventoryId} sẽ dưới ngưỡng tối thiểu (10) sau khi tạo đơn vật tư!", exception.Message);
        }

    }
}