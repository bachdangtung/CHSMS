using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class GetAllSuppliersTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public GetAllSuppliersTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void GetAllSuppliers_ReturnsAllSuppliers()
        {
            // Arrange
            var suppliers = new List<Supplier>
            {
                TestHelper.CreateSupplier(1, "Supplier1"),
                TestHelper.CreateSupplier(2, "Supplier2")
            };
            _medicineRepositoryMock.Setup(repo => repo.GetAllSuppliers()).Returns(suppliers);

            // Act
            var result = _service.GetAllSuppliers();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Supplier1", result[0].Name);
            Assert.Equal(2, result[1].SupplierId);
        }

        [Fact]
        public void GetAllSuppliers_ReturnsEmptyListWhenNoSuppliers()
        {
            // Arrange
            _medicineRepositoryMock.Setup(repo => repo.GetAllSuppliers()).Returns(new List<Supplier>());

            // Act
            var result = _service.GetAllSuppliers();

            // Assert
            Assert.Empty(result);
        }
    }
}
