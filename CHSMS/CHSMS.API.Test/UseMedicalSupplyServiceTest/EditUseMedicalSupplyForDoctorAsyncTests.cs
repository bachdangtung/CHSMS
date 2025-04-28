using CHSMS.API.DTOs.MedicalSupplyConsumption;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Moq;

namespace CHSMS.API.Tests
{
    public class EditUseMedicalSupplyForDoctorAsyncTests
    {
        private readonly Mock<IUseMedicalSupplyRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly IUseMedicalSupplyService _service;

        public EditUseMedicalSupplyForDoctorAsyncTests()
        {
            var mocks = TestHelper.CreateMocks();
            _repositoryMock = mocks.Repository;
            _contextMock = mocks.Context;
            _service = new UseMedicalSupplyService(_repositoryMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task EditUseMedicalSupplyForDoctorAsync_ValidInput_UpdatesSuccessfully()
        {
            // Arrange
            var dto = TestHelper.CreateValidEditDoctorDTO(1);
            var useMedicalSupply = TestHelper.CreateUseMedicalSupply(1, 1, 1);
            var inventory = TestHelper.CreateMedicalSupplyInventory(1, 1, 20);
            var createdConsumption = TestHelper.CreateMedicalSupplyConsumption(1, 1, 5);
            var existingConsumption = TestHelper.CreateMedicalSupplyConsumption(2, 1, 5, false); // Unconfirmed consumption
            var createdUmsmsc = new UseMedicalSuppliesMedicalSupplyConsumption
            {
                UseMedicalSupplieId = 1,
                MsconsumptionId = 1,
                TotalPrice = 0
            };

            _repositoryMock.Setup(r => r.GetUseMedicalSupplyByIdAsync(1)).ReturnsAsync(useMedicalSupply);
            _repositoryMock.Setup(r => r.GetMedicalSupplyConsumptionsByUseMedicalSupplyIdAsync(1)).ReturnsAsync(new List<MedicalSupplyConsumption> { existingConsumption });
            _repositoryMock.Setup(r => r.GetMedicalSupplyInventoryByIdAsync(1)).ReturnsAsync(inventory);
            _repositoryMock.Setup(r => r.UpdateUseMedicalSupplyAsync(It.IsAny<UseMedicalSupply>())).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.CreateMedicalSupplyConsumptionAsync(It.IsAny<MedicalSupplyConsumption>())).ReturnsAsync(createdConsumption);
            _repositoryMock.Setup(r => r.CreateUseMedicalSuppliesMedicalSupplyConsumptionAsync(It.IsAny<UseMedicalSuppliesMedicalSupplyConsumption>())).ReturnsAsync(createdUmsmsc);

            // Act
            await _service.EditUseMedicalSupplyForDoctorAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.UpdateUseMedicalSupplyAsync(It.IsAny<UseMedicalSupply>()), Times.Once());
            _repositoryMock.Verify(r => r.CreateMedicalSupplyConsumptionAsync(It.IsAny<MedicalSupplyConsumption>()), Times.Once());
            _repositoryMock.Verify(r => r.CreateUseMedicalSuppliesMedicalSupplyConsumptionAsync(It.IsAny<UseMedicalSuppliesMedicalSupplyConsumption>()), Times.Once());
            _contextMock.Verify(c => c.Database.BeginTransactionAsync(default), Times.Once());
        }

        [Fact]
        public async Task EditUseMedicalSupplyForDoctorAsync_FutureIssueDate_ThrowsException()
        {
            // Arrange
            var dto = TestHelper.CreateValidEditDoctorDTO(1);
            dto.IssueDate = DateTime.Now.AddDays(1);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.EditUseMedicalSupplyForDoctorAsync(dto));
            Assert.Equal("Lỗi khi chỉnh sửa đơn vật tư: Ngày phát hành không được là ngày trong tương lai!", exception.Message);
        }

        [Fact]
        public async Task EditUseMedicalSupplyForDoctorAsync_NonExistentUseMedicalSupply_ThrowsException()
        {
            // Arrange
            var dto = TestHelper.CreateValidEditDoctorDTO(1);
            _repositoryMock.Setup(r => r.GetUseMedicalSupplyByIdAsync(1)).ReturnsAsync((UseMedicalSupply)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.EditUseMedicalSupplyForDoctorAsync(dto));
            Assert.Equal($"Lỗi khi chỉnh sửa đơn vật tư: Không tìm thấy đơn vật tư với ID: {dto.UseMedicalSupplyId}", exception.Message);
        }

        [Fact]
        public async Task EditUseMedicalSupplyForDoctorAsync_AllConsumptionsConfirmed_ThrowsException()
        {
            // Arrange
            var dto = TestHelper.CreateValidEditDoctorDTO(1);
            var useMedicalSupply = TestHelper.CreateUseMedicalSupply(1, 1, 1);
            var consumptions = new List<MedicalSupplyConsumption> { TestHelper.CreateMedicalSupplyConsumption(1, 1, 5, true) };

            _repositoryMock.Setup(r => r.GetUseMedicalSupplyByIdAsync(1)).ReturnsAsync(useMedicalSupply);
            _repositoryMock.Setup(r => r.GetMedicalSupplyConsumptionsByUseMedicalSupplyIdAsync(1)).ReturnsAsync(consumptions);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.EditUseMedicalSupplyForDoctorAsync(dto));
            Assert.Equal("Lỗi khi chỉnh sửa đơn vật tư: Đơn vật tư đã được xác nhận hoàn tất, không thể chỉnh sửa!", exception.Message);
        }

        [Fact]
        public async Task EditUseMedicalSupplyForDoctorAsync_MoreThanTenConsumptions_ThrowsException()
        {
            // Arrange
            var dto = TestHelper.CreateValidEditDoctorDTO(1);
            dto.MedicalSupplyConsumptionsToAdd = Enumerable.Range(1, 11).Select(i => new MedicalSupplyConsumptionDTO
            {
                MedicalSupplyInventoryId = i,
                Amount = 5,
                ConsumptionDate = DateTime.Now.AddDays(-1)
            }).ToList();

            var useMedicalSupply = TestHelper.CreateUseMedicalSupply(1, 1, 1);
            var existingConsumption = TestHelper.CreateMedicalSupplyConsumption(1, 1, 5, false); // Unconfirmed consumption
            _repositoryMock.Setup(r => r.GetUseMedicalSupplyByIdAsync(1)).ReturnsAsync(useMedicalSupply);
            _repositoryMock.Setup(r => r.GetMedicalSupplyConsumptionsByUseMedicalSupplyIdAsync(1)).ReturnsAsync(new List<MedicalSupplyConsumption> { existingConsumption });
            _repositoryMock.Setup(r => r.GetMedicalSupplyInventoryByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => TestHelper.CreateMedicalSupplyInventory(id, id, 20));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.EditUseMedicalSupplyForDoctorAsync(dto));
            Assert.Equal("Lỗi khi chỉnh sửa đơn vật tư: Một đơn vật tư không được chứa quá 10 loại vật tư!", exception.Message);
        }
    }
}