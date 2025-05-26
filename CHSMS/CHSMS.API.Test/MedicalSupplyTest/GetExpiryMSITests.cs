using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Moq;

namespace CHSMS.API.Test.MedicalSupplyTest
{
    public class GetExpiryMSITests
    {
        private readonly Mock<IMedicalSupplyRepository> _mockRepository;
        private readonly MedicalSupplyService _service;
        private readonly DateTime currentDate = new DateTime(2025, 5, 25);

        public GetExpiryMSITests()
        {
            _mockRepository = new Mock<IMedicalSupplyRepository>();
            _service = new MedicalSupplyService(_mockRepository.Object);

            // Setup precondition: MedicalSupplyInventory with MedicalSupplyId: 1
            var inventory1 = new MedicalSupplyInventory
            {
                SupplyInventoryId = 1,
                MedicalSupplyId = 1,
                Quantity = 10,
                ExpiryDate = new DateTime(2025, 5, 5)
            };
            var inventory2 = new MedicalSupplyInventory
            {
                SupplyInventoryId = 2,
                MedicalSupplyId = 1,
                Quantity = 20,
                ExpiryDate = new DateTime(2025, 10, 5)
            };
            _mockRepository.Setup(r => r.GetMedicalSupplyInventoryByMSID(1))
                .Returns(new List<MedicalSupplyInventory> { inventory1, inventory2 });
        }

        [Fact]
        public void GetExpiryMSI_ValidId_NullFrom_NullTo_ReturnsExpiredQuantity()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetNumberOfExpiredMSI(1, null, null))
                .Returns(10.0); // Only 5/5/2025 is expired as of 25/5/2025

            // Act
            var result = _service.GetExpiryMSI(1, null, null);

            // Assert
            Assert.Equal(10.0, (double)result);
        }

        [Fact]
        public void GetExpiryMSI_ValidId_NullFrom_To6May2025_ReturnsExpiredQuantity()
        {
            // Arrange
            var toDate = new DateTime(2025, 5, 6);
            _mockRepository.Setup(r => r.GetNumberOfExpiredMSI(1, null, toDate))
                .Returns(10.0); // Only 5/5/2025 is expired by 6/5/2025

            // Act
            var result = _service.GetExpiryMSI(1, null, toDate);

            // Assert
            Assert.Equal(10.0, (double)result);
        }

        [Fact]
        public void GetExpiryMSI_ValidId_NullFrom_To4April2025_ReturnsZero()
        {
            // Arrange
            var toDate = new DateTime(2025, 4, 4);
            _mockRepository.Setup(r => r.GetNumberOfExpiredMSI(1, null, toDate))
                .Returns(0.0); // No inventory expired by 4/4/2025

            // Act
            var result = _service.GetExpiryMSI(1, null, toDate);

            // Assert
            Assert.Equal(0.0, (double)result);
        }

        [Fact]
        public void GetExpiryMSI_ValidId_From3March2025_NullTo_ReturnsExpiredQuantity()
        {
            // Arrange
            var fromDate = new DateTime(2025, 3, 3);
            _mockRepository.Setup(r => r.GetNumberOfExpiredMSI(1, fromDate, null))
                .Returns(10.0); // 5/5/2025 is expired as of 25/5/2025

            // Act
            var result = _service.GetExpiryMSI(1, fromDate, null);

            // Assert
            Assert.Equal(10.0, (double)result);
        }

        [Fact]
        public void GetExpiryMSI_ValidId_From6May2025_NullTo_ReturnsZero()
        {
            // Arrange
            var fromDate = new DateTime(2025, 5, 6);
            _mockRepository.Setup(r => r.GetNumberOfExpiredMSI(1, fromDate, null))
                .Returns(0.0); // No inventory expired after 6/5/2025 as of 25/5/2025

            // Act
            var result = _service.GetExpiryMSI(1, fromDate, null);

            // Assert
            Assert.Equal(0.0, (double)result);
        }

        [Fact]
        public void GetExpiryMSI_ValidId_From6May2025_To4April2025_ReturnsZero()
        {
            // Arrange
            var fromDate = new DateTime(2025, 5, 6);
            var toDate = new DateTime(2025, 4, 4);
            _mockRepository.Setup(r => r.GetNumberOfExpiredMSI(1, fromDate, toDate))
                .Returns(0.0); // Invalid range (from > to) returns 0

            // Act
            var result = _service.GetExpiryMSI(1, fromDate, toDate);

            // Assert
            Assert.Equal(0.0, (double)result);
        }

        [Fact]
        public void GetExpiryMSI_InvalidId_NullFrom_NullTo_ReturnsZero()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetNumberOfExpiredMSI(-1, null, null))
                .Returns(0.0); // Invalid ID returns 0

            // Act
            var result = _service.GetExpiryMSI(-1, null, null);

            // Assert
            Assert.Equal(0.0, (double)result);
        }
    }
}