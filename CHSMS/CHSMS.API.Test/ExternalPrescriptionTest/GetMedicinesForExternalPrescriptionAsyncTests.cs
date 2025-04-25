using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace CHSMS.API.Test.ExternalPrescriptionTest
{
    public class GetMedicinesForExternalPrescriptionAsyncTests : IDisposable
    {
        private readonly Mock<IExternalPrescriptionRepository> _repositoryMock;
        private readonly SEP_TestContext _dbContext;
        private readonly ExternalPrescriptionService _service;

        public GetMedicinesForExternalPrescriptionAsyncTests()
        {
            _repositoryMock = new Mock<IExternalPrescriptionRepository>();

            var options = new DbContextOptionsBuilder<SEP_TestContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _dbContext = new SEP_TestContext(options);
            _service = new ExternalPrescriptionService(_repositoryMock.Object, _dbContext);
        }

        [Fact]
        public async Task GetMedicinesForExternalPrescriptionAsync_ReturnsValidMedicines()
        {
            // Arrange
            var expectedMedicines = new List<Medicine>
            {
                new Medicine { MedicineId = 1, MedicineName = "Medicine A", Status = true, IsBhyt = true },
                new Medicine { MedicineId = 2, MedicineName = "Medicine B", Status = true, IsBhyt = true }
            };

            _repositoryMock.Setup(r => r.GetMedicinesForExternalPrescriptionAsync())
                .ReturnsAsync(expectedMedicines);

            // Act
            var result = await _service.GetMedicinesForExternalPrescriptionAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, m => Assert.True(m.Status));
            Assert.Contains(result, m => m.MedicineId == 1 && m.MedicineName == "Medicine A");
            Assert.Contains(result, m => m.MedicineId == 2 && m.MedicineName == "Medicine B");
            _repositoryMock.Verify(r => r.GetMedicinesForExternalPrescriptionAsync(), Times.Once());
        }

        [Fact]
        public async Task GetMedicinesForExternalPrescriptionAsync_NoMedicines_ReturnsEmptyList()
        {
            // Arrange
            var expectedMedicines = new List<Medicine>();

            _repositoryMock.Setup(r => r.GetMedicinesForExternalPrescriptionAsync())
                .ReturnsAsync(expectedMedicines);

            // Act
            var result = await _service.GetMedicinesForExternalPrescriptionAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _repositoryMock.Verify(r => r.GetMedicinesForExternalPrescriptionAsync(), Times.Once());
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}