using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Moq;

namespace CHSMS.API.Tests
{
    public class EditUseMedicalSupplyForPharmacistAsyncTests
    {
        private readonly Mock<IUseMedicalSupplyRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly IUseMedicalSupplyService _service;

        public EditUseMedicalSupplyForPharmacistAsyncTests()
        {
            var mocks = TestHelper.CreateMocks();
            _repositoryMock = mocks.Repository;
            _contextMock = mocks.Context;
            _service = new UseMedicalSupplyService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task EditUseMedicalSupplyForPharmacistAsync_ValidInput_UpdatesSuccessfully()
        {
            // Arrange
            var dto = TestHelper.CreateValidEditPharmacistDTO(1);
            var useMedicalSupply = TestHelper.CreateUseMedicalSupply(1, 1, 1);
            useMedicalSupply.IssueDate = DateTime.UtcNow.Date;
            var consumption = TestHelper.CreateMedicalSupplyConsumption(1, 1, 5);
            var inventory = TestHelper.CreateMedicalSupplyInventory(1, 1, 20);
            var umsmsc = new UseMedicalSuppliesMedicalSupplyConsumption { UseMedicalSupplieId = 1, MsconsumptionId = 1, TotalPrice = 0 };

            _repositoryMock.Setup(r => r.GetUseMedicalSupplyByIdAsync(1)).ReturnsAsync(useMedicalSupply);
            _repositoryMock.Setup(r => r.GetMedicalSupplyConsumptionByIdAsync(1)).ReturnsAsync(consumption);
            _repositoryMock.Setup(r => r.GetMedicalSupplyInventoryByIdAsync(1)).ReturnsAsync(inventory);
            _repositoryMock.Setup(r => r.GetUseMedicalSuppliesMedicalSupplyConsumptionByConsumptionIdAsync(1)).ReturnsAsync(umsmsc);
            _repositoryMock.Setup(r => r.UpdateMedicalSupplyConsumptionAsync(It.IsAny<MedicalSupplyConsumption>())).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdateMedicalSupplyInventoryAsync(It.IsAny<MedicalSupplyInventory>())).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdateUseMedicalSuppliesMedicalSupplyConsumptionAsync(It.IsAny<UseMedicalSuppliesMedicalSupplyConsumption>())).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.UpdateUseMedicalSupplyAsync(It.IsAny<UseMedicalSupply>())).Returns(Task.CompletedTask);

            // Act
            await _service.EditUseMedicalSupplyForPharmacistAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.UpdateMedicalSupplyConsumptionAsync(It.IsAny<MedicalSupplyConsumption>()), Times.Once());
            _repositoryMock.Verify(r => r.UpdateMedicalSupplyInventoryAsync(It.IsAny<MedicalSupplyInventory>()), Times.Once());
            _repositoryMock.Verify(r => r.UpdateUseMedicalSuppliesMedicalSupplyConsumptionAsync(It.IsAny<UseMedicalSuppliesMedicalSupplyConsumption>()), Times.Once());
            _repositoryMock.Verify(r => r.UpdateUseMedicalSupplyAsync(It.IsAny<UseMedicalSupply>()), Times.Once());
            _contextMock.Verify(c => c.Database.BeginTransactionAsync(default), Times.Once());
        }

        [Fact]
        public async Task EditUseMedicalSupplyForPharmacistAsync_NonExistentUseMedicalSupply_ThrowsException()
        {
            // Arrange
            var dto = TestHelper.CreateValidEditPharmacistDTO(1);
            _repositoryMock.Setup(r => r.GetUseMedicalSupplyByIdAsync(1)).ReturnsAsync((UseMedicalSupply)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.EditUseMedicalSupplyForPharmacistAsync(dto));
            Assert.Equal("Lỗi khi chỉnh sửa trạng thái đơn vật tư: Không tìm thấy đơn vật tư với ID: 1", exception.Message);
        }

        [Fact]
        public async Task EditUseMedicalSupplyForPharmacistAsync_DifferentIssueDate_ThrowsException()
        {
            // Arrange
            var dto = TestHelper.CreateValidEditPharmacistDTO(1);
            var useMedicalSupply = TestHelper.CreateUseMedicalSupply(1, 1, 1);
            useMedicalSupply.IssueDate = DateTime.UtcNow.AddDays(-1);

            _repositoryMock.Setup(r => r.GetUseMedicalSupplyByIdAsync(1)).ReturnsAsync(useMedicalSupply);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.EditUseMedicalSupplyForPharmacistAsync(dto));
            Assert.Equal("Lỗi khi chỉnh sửa trạng thái đơn vật tư: Chỉ được chỉnh sửa trạng thái đơn vật tư trong ngày phát hành đơn vật tư!", exception.Message);
        }

        [Fact]
        public async Task EditUseMedicalSupplyForPharmacistAsync_InsufficientInventory_ThrowsException()
        {
            // Arrange
            var dto = TestHelper.CreateValidEditPharmacistDTO(1);
            var useMedicalSupply = TestHelper.CreateUseMedicalSupply(1, 1, 1);
            useMedicalSupply.IssueDate = DateTime.UtcNow.Date;
            var consumption = TestHelper.CreateMedicalSupplyConsumption(1, 1, 25);
            var inventory = TestHelper.CreateMedicalSupplyInventory(1, 1, 20);

            _repositoryMock.Setup(r => r.GetUseMedicalSupplyByIdAsync(1)).ReturnsAsync(useMedicalSupply);
            _repositoryMock.Setup(r => r.GetMedicalSupplyConsumptionByIdAsync(1)).ReturnsAsync(consumption);
            _repositoryMock.Setup(r => r.GetMedicalSupplyInventoryByIdAsync(1)).ReturnsAsync(inventory);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.EditUseMedicalSupplyForPharmacistAsync(dto));
            Assert.Equal("Lỗi khi chỉnh sửa trạng thái đơn vật tư: Số lượng tồn kho không đủ để phát vật tư!", exception.Message);
        }
    }
}