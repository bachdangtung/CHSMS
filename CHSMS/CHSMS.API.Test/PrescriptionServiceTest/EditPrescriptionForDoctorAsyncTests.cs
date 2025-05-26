using CHSMS.API.DTOs.MedicineConsumption;
using CHSMS.API.DTOs.Prescription;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHSMS.API.Test.PrescriptionServiceTest
{
    public class EditPrescriptionForDoctorAsyncTests
    {
        private readonly Mock<IPrescriptionRepository> _repositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<DatabaseFacade> _databaseFacadeMock;
        private readonly Mock<IDbContextTransaction> _transactionMock;
        private readonly IPrescriptionService _service;

        public EditPrescriptionForDoctorAsyncTests()
        {
            _repositoryMock = new Mock<IPrescriptionRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _databaseFacadeMock = new Mock<DatabaseFacade>(_contextMock.Object);
            _transactionMock = new Mock<IDbContextTransaction>();

            _contextMock.Setup(c => c.Database).Returns(_databaseFacadeMock.Object);
            _databaseFacadeMock.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                              .ReturnsAsync(_transactionMock.Object);

            _service = new PrescriptionService(_repositoryMock.Object, _contextMock.Object);
        }
        [Fact]
        public async Task EditPrescriptionForDoctorAsync_FutureIssueDate_ThrowsException()
        {
            // Arrange
            var dto = new EditPrescriptionForDoctorDTO
            {
                PrescriptionId = 1,
                IssueDate = DateTime.Now.AddDays(1),
                MedicineConsumptionsToAdd = new List<MedicineConsumptionDTO>()
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForDoctorAsync(dto));
            Assert.Contains("Ngày phát hành không được là ngày trong tương lai!", exception.Message);
            _transactionMock.Verify(t => t.RollbackAsync(default), Times.Once());
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_PrescriptionNotFound_ThrowsException()
        {
            // Arrange
            var dto = new EditPrescriptionForDoctorDTO
            {
                PrescriptionId = 1,
                IssueDate = DateTime.Now,
                MedicineConsumptionsToAdd = new List<MedicineConsumptionDTO>()
            };
            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1)).ReturnsAsync((Prescription)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForDoctorAsync(dto));
            Assert.Contains("Không tìm thấy đơn thuốc với ID: 1", exception.Message);
            _transactionMock.Verify(t => t.RollbackAsync(default), Times.Once());
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_AllConsumptionsConfirmed_ThrowsException()
        {
            // Arrange
            var dto = new EditPrescriptionForDoctorDTO
            {
                PrescriptionId = 1,
                IssueDate = DateTime.Now,
                MedicineConsumptionsToAdd = new List<MedicineConsumptionDTO>()
            };
            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                           .ReturnsAsync(new Prescription { PrescriptionId = 1 });
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                           .ReturnsAsync(new List<MedicineConsumption>
                           {
                               new MedicineConsumption { MedicineConsumptionId = 1, Status = true }
                           });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForDoctorAsync(dto));
            Assert.Contains("Đơn thuốc đã được xác nhận hoàn tất, không thể chỉnh sửa!", exception.Message);
            _transactionMock.Verify(t => t.RollbackAsync(default), Times.Once());
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_MoreThanTenMedicines_ThrowsException()
        {
            // Arrange
            var dto = new EditPrescriptionForDoctorDTO
            {
                PrescriptionId = 1,
                IssueDate = DateTime.Now,
                MedicineConsumptionsToAdd = Enumerable.Range(1, 8).Select(i => new MedicineConsumptionDTO
                {
                    MedicineInventoryId = i,
                    Amount = 10,
                    ConsumptionDate = DateTime.Now
                }).ToList()
            };
            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                           .ReturnsAsync(new Prescription { PrescriptionId = 1 });
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                           .ReturnsAsync(new List<MedicineConsumption>
                           {
                               new MedicineConsumption { MedicineConsumptionId = 1, Status = false },
                               new MedicineConsumption { MedicineConsumptionId = 2, Status = false },
                               new MedicineConsumption { MedicineConsumptionId = 3, Status = false }
                           });
            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync((int id) => new MedicineInventory
                           {
                               MedicineInventoryId = id,
                               MedicineId = id,
                               Quantity = 100,
                               ExpiryDate = DateTime.Now.AddDays(30)
                           });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForDoctorAsync(dto));
            Assert.Contains("Một đơn thuốc không được chứa quá 10 loại thuốc!", exception.Message);
            _transactionMock.Verify(t => t.RollbackAsync(default), Times.Once());
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_DuplicateMedicineInventoryIds_ThrowsException()
        {
            // Arrange
            var dto = new EditPrescriptionForDoctorDTO
            {
                PrescriptionId = 1,
                IssueDate = DateTime.Now,
                MedicineConsumptionsToAdd = new List<MedicineConsumptionDTO>
        {
            new MedicineConsumptionDTO { MedicineInventoryId = 1, Amount = 10, ConsumptionDate = DateTime.Now },
            new MedicineConsumptionDTO { MedicineInventoryId = 1, Amount = 5, ConsumptionDate = DateTime.Now }
        }
            };
            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                           .ReturnsAsync(new Prescription { PrescriptionId = 1 });
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                           .ReturnsAsync(new List<MedicineConsumption>
                           {
                       new MedicineConsumption { MedicineConsumptionId = 1, Status = false } // Thêm MedicineConsumption với Status = false
                           });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForDoctorAsync(dto));
            Assert.Contains("Có thuốc bị trùng trong danh sách thêm mới. Vui lòng kiểm tra lại!", exception.Message);
            _transactionMock.Verify(t => t.RollbackAsync(default), Times.Once());
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_NonExistentInventory_ThrowsException()
        {
            // Arrange
            var dto = new EditPrescriptionForDoctorDTO
            {
                PrescriptionId = 1,
                IssueDate = DateTime.Now,
                MedicineConsumptionsToAdd = new List<MedicineConsumptionDTO>
        {
            new MedicineConsumptionDTO { MedicineInventoryId = 1, Amount = 10, ConsumptionDate = DateTime.Now }
        }
            };
            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                           .ReturnsAsync(new Prescription { PrescriptionId = 1 });
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                           .ReturnsAsync(new List<MedicineConsumption>
                           {
                       new MedicineConsumption { MedicineConsumptionId = 1, Status = false } // Thêm để vượt qua kiểm tra trạng thái
                           });
            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(1))
                           .ReturnsAsync((MedicineInventory)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForDoctorAsync(dto));
            Assert.Contains("Không tìm thấy kho thuốc với ID: 1", exception.Message);
            _transactionMock.Verify(t => t.RollbackAsync(default), Times.Once());
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_InsufficientInventory_ThrowsException()
        {
            // Arrange
            var dto = new EditPrescriptionForDoctorDTO
            {
                PrescriptionId = 1,
                IssueDate = DateTime.Now,
                MedicineConsumptionsToAdd = new List<MedicineConsumptionDTO>
        {
            new MedicineConsumptionDTO { MedicineInventoryId = 1, Amount = 200, ConsumptionDate = DateTime.Now }
        }
            };
            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                           .ReturnsAsync(new Prescription { PrescriptionId = 1 });
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                           .ReturnsAsync(new List<MedicineConsumption>
                           {
                       new MedicineConsumption { MedicineConsumptionId = 1, Status = false } // Thêm để vượt qua kiểm tra trạng thái
                           });
            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(1))
                           .ReturnsAsync(new MedicineInventory
                           {
                               MedicineInventoryId = 1,
                               MedicineId = 1,
                               Quantity = 100,
                               ExpiryDate = DateTime.Now.AddDays(30)
                           });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForDoctorAsync(dto));
            Assert.Contains("Số lượng yêu cầu vượt quá tồn kho", exception.Message);
            _transactionMock.Verify(t => t.RollbackAsync(default), Times.Once());
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_ExpiredMedicine_ThrowsException()
        {
            // Arrange
            var dto = new EditPrescriptionForDoctorDTO
            {
                PrescriptionId = 1,
                IssueDate = DateTime.Now,
                MedicineConsumptionsToAdd = new List<MedicineConsumptionDTO>
        {
            new MedicineConsumptionDTO { MedicineInventoryId = 1, Amount = 10, ConsumptionDate = DateTime.Now.AddDays(1) }
        }
            };
            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                           .ReturnsAsync(new Prescription { PrescriptionId = 1 });
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                           .ReturnsAsync(new List<MedicineConsumption>
                           {
                       new MedicineConsumption { MedicineConsumptionId = 1, Status = false } // Thêm để vượt qua kiểm tra trạng thái
                           });
            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(1))
                           .ReturnsAsync(new MedicineInventory
                           {
                               MedicineInventoryId = 1,
                               MedicineId = 1,
                               Quantity = 100,
                               ExpiryDate = DateTime.Now.AddDays(-1)
                           });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.EditPrescriptionForDoctorAsync(dto));
            Assert.Contains("Ngày sử dụng vượt quá hạn sử dụng", exception.Message);
            _transactionMock.Verify(t => t.RollbackAsync(default), Times.Once());
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_UpdateOnlyPrescription_EditsSuccessfully()
        {
            // Arrange
            var dto = new EditPrescriptionForDoctorDTO
            {
                PrescriptionId = 1,
                IssueDate = DateTime.Now,
                MedicalRecordHistoryId = 2,
                UserId = 3,
                Note = "Updated note",
                IsBhyt = true,
                MedicineConsumptionsToAdd = new List<MedicineConsumptionDTO>(),
                MedicineConsumptionIdsToRemove = new List<int>()
            };
            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                           .ReturnsAsync(new Prescription { PrescriptionId = 1 });
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                           .ReturnsAsync(new List<MedicineConsumption>
                           {
                               new MedicineConsumption { MedicineConsumptionId = 1, Status = false }
                           });
            _repositoryMock.Setup(r => r.UpdatePrescriptionAsync(It.IsAny<Prescription>()))
                           .Returns(Task.CompletedTask);

            // Act
            await _service.EditPrescriptionForDoctorAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.UpdatePrescriptionAsync(It.Is<Prescription>(p =>
                p.MedicalRecordHistoryId == 2 &&
                p.UserId == 3 &&
                p.Note == "Updated note" &&
                p.IsBhyt == true)), Times.Once());
            _transactionMock.Verify(t => t.CommitAsync(default), Times.Once());
            _transactionMock.Verify(t => t.RollbackAsync(default), Times.Never());
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_RemoveAndAddConsumption_EditsSuccessfully()
        {
            // Arrange
            var dto = new EditPrescriptionForDoctorDTO
            {
                PrescriptionId = 1,
                IssueDate = DateTime.Now,
                MedicalRecordHistoryId = 2,
                UserId = 3,
                MedicineConsumptionIdsToRemove = new List<int> { 1 },
                MedicineConsumptionsToAdd = new List<MedicineConsumptionDTO>
                {
                    new MedicineConsumptionDTO
                    {
                        MedicineInventoryId = 2,
                        Amount = 10,
                        ConsumptionDate = DateTime.Now,
                        IsSpecialMedicine = false,
                        Note = "New medicine"
                    }
                }
            };
            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                           .ReturnsAsync(new Prescription { PrescriptionId = 1 });
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                           .ReturnsAsync(new List<MedicineConsumption>
                           {
                               new MedicineConsumption { MedicineConsumptionId = 1, Status = false }
                           });
            _repositoryMock.Setup(r => r.GetPrescriptionMedicineConsumptionByConsumptionIdAsync(1))
                           .ReturnsAsync(new PrescriptionMedicineConsumption { PrescriptionId = 1, MedicineConsumtionId = 1 });
            _repositoryMock.Setup(r => r.DeletePrescriptionMedicineConsumptionAsync(1, 1))
                           .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.DeleteMedicineConsumptionAsync(1))
                           .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(2))
                           .ReturnsAsync(new MedicineInventory
                           {
                               MedicineInventoryId = 2,
                               MedicineId = 2,
                               Quantity = 100,
                               ExpiryDate = DateTime.Now.AddDays(30)
                           });
            _repositoryMock.Setup(r => r.CreateMedicineConsumptionAsync(It.IsAny<MedicineConsumption>()))
                           .ReturnsAsync(new MedicineConsumption { MedicineConsumptionId = 2 });
            _repositoryMock.Setup(r => r.CreatePrescriptionMedicineConsumptionAsync(It.IsAny<PrescriptionMedicineConsumption>()))
                           .ReturnsAsync(new PrescriptionMedicineConsumption { PrescriptionId = 1, MedicineConsumtionId = 2 });
            _repositoryMock.Setup(r => r.UpdatePrescriptionAsync(It.IsAny<Prescription>()))
                           .Returns(Task.CompletedTask);

            // Act
            await _service.EditPrescriptionForDoctorAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.DeleteMedicineConsumptionAsync(1), Times.Once());
            _repositoryMock.Verify(r => r.CreateMedicineConsumptionAsync(It.IsAny<MedicineConsumption>()), Times.Once());
            _transactionMock.Verify(t => t.CommitAsync(default), Times.Once());
            _transactionMock.Verify(t => t.RollbackAsync(default), Times.Never());
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_ExactlyTenMedicines_EditsSuccessfully()
        {
            // Arrange
            var dto = new EditPrescriptionForDoctorDTO
            {
                PrescriptionId = 1,
                IssueDate = DateTime.Now,
                MedicalRecordHistoryId = 2,
                UserId = 3,
                MedicineConsumptionsToAdd = Enumerable.Range(6, 5).Select(i => new MedicineConsumptionDTO
                {
                    MedicineInventoryId = i,
                    Amount = 10,
                    ConsumptionDate = DateTime.Now,
                    IsSpecialMedicine = false,
                    Note = $"Medicine {i}"
                }).ToList(),
                MedicineConsumptionIdsToRemove = new List<int>()
            };
            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                           .ReturnsAsync(new Prescription { PrescriptionId = 1 });
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                           .ReturnsAsync(new List<MedicineConsumption>
                           {
                               new MedicineConsumption { MedicineConsumptionId = 1, Status = false },
                               new MedicineConsumption { MedicineConsumptionId = 2, Status = false },
                               new MedicineConsumption { MedicineConsumptionId = 3, Status = false },
                               new MedicineConsumption { MedicineConsumptionId = 4, Status = false },
                               new MedicineConsumption { MedicineConsumptionId = 5, Status = false }
                           });
            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync((int id) => new MedicineInventory
                           {
                               MedicineInventoryId = id,
                               MedicineId = id,
                               Quantity = 100,
                               ExpiryDate = DateTime.Now.AddDays(30)
                           });
            _repositoryMock.Setup(r => r.CreateMedicineConsumptionAsync(It.IsAny<MedicineConsumption>()))
                           .ReturnsAsync((MedicineConsumption mc) => new MedicineConsumption
                           {
                               MedicineConsumptionId = mc.MedicineInventoryId
                           });
            _repositoryMock.Setup(r => r.CreatePrescriptionMedicineConsumptionAsync(It.IsAny<PrescriptionMedicineConsumption>()))
                           .ReturnsAsync((PrescriptionMedicineConsumption pmc) => new PrescriptionMedicineConsumption
                           {
                               PrescriptionId = 1,
                               MedicineConsumtionId = pmc.MedicineConsumtionId
                           });
            _repositoryMock.Setup(r => r.UpdatePrescriptionAsync(It.IsAny<Prescription>()))
                           .Returns(Task.CompletedTask);

            // Act
            await _service.EditPrescriptionForDoctorAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.CreateMedicineConsumptionAsync(It.IsAny<MedicineConsumption>()), Times.Exactly(5));
            _transactionMock.Verify(t => t.CommitAsync(default), Times.Once());
            _transactionMock.Verify(t => t.RollbackAsync(default), Times.Never());
        }

        [Fact]
        public async Task EditPrescriptionForDoctorAsync_MultipleMedicinesAndBHYT_EditsSuccessfully()
        {
            // Arrange
            var dto = new EditPrescriptionForDoctorDTO
            {
                PrescriptionId = 1,
                IssueDate = DateTime.Now,
                MedicalRecordHistoryId = 2,
                UserId = 3,
                Note = "Updated note",
                IsBhyt = true,
                MedicineConsumptionIdsToRemove = new List<int> { 1 },
                MedicineConsumptionsToAdd = new List<MedicineConsumptionDTO>
                {
                    new MedicineConsumptionDTO { MedicineInventoryId = 2, Amount = 10, ConsumptionDate = DateTime.Now, IsSpecialMedicine = false, Note = "Medicine 2" },
                    new MedicineConsumptionDTO { MedicineInventoryId = 3, Amount = 20, ConsumptionDate = DateTime.Now, IsSpecialMedicine = true, Note = "Medicine 3" }
                }
            };
            _repositoryMock.Setup(r => r.GetPrescriptionByIdAsync(1))
                           .ReturnsAsync(new Prescription { PrescriptionId = 1 });
            _repositoryMock.Setup(r => r.GetMedicineConsumptionsByPrescriptionIdAsync(1))
                           .ReturnsAsync(new List<MedicineConsumption>
                           {
                               new MedicineConsumption { MedicineConsumptionId = 1, Status = false },
                               new MedicineConsumption { MedicineConsumptionId = 4, Status = false }
                           });
            _repositoryMock.Setup(r => r.GetPrescriptionMedicineConsumptionByConsumptionIdAsync(1))
                           .ReturnsAsync(new PrescriptionMedicineConsumption { PrescriptionId = 1, MedicineConsumtionId = 1 });
            _repositoryMock.Setup(r => r.DeletePrescriptionMedicineConsumptionAsync(1, 1))
                           .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.DeleteMedicineConsumptionAsync(1))
                           .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.GetMedicineInventoryByIdAsync(It.IsAny<int>()))
                           .ReturnsAsync((int id) => new MedicineInventory
                           {
                               MedicineInventoryId = id,
                               MedicineId = id,
                               Quantity = 100,
                               ExpiryDate = DateTime.Now.AddDays(30)
                           });
            _repositoryMock.Setup(r => r.CreateMedicineConsumptionAsync(It.IsAny<MedicineConsumption>()))
                           .ReturnsAsync((MedicineConsumption mc) => new MedicineConsumption
                           {
                               MedicineConsumptionId = mc.MedicineInventoryId
                           });
            _repositoryMock.Setup(r => r.CreatePrescriptionMedicineConsumptionAsync(It.IsAny<PrescriptionMedicineConsumption>()))
                           .ReturnsAsync((PrescriptionMedicineConsumption pmc) => new PrescriptionMedicineConsumption
                           {
                               PrescriptionId = 1,
                               MedicineConsumtionId = pmc.MedicineConsumtionId
                           });
            _repositoryMock.Setup(r => r.UpdatePrescriptionAsync(It.IsAny<Prescription>()))
                           .Returns(Task.CompletedTask);

            // Act
            await _service.EditPrescriptionForDoctorAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.UpdatePrescriptionAsync(It.Is<Prescription>(p => p.IsBhyt == true)), Times.Once());
            _repositoryMock.Verify(r => r.DeleteMedicineConsumptionAsync(1), Times.Once());
            _repositoryMock.Verify(r => r.CreateMedicineConsumptionAsync(It.IsAny<MedicineConsumption>()), Times.Exactly(2));
            _transactionMock.Verify(t => t.CommitAsync(default), Times.Once());
            _transactionMock.Verify(t => t.RollbackAsync(default), Times.Never());
        }

        

    }
}
