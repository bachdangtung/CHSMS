using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CHSMS.API.Test.MedicineServiceTest
{
    public class GetAllReceiversTests
    {
        private readonly Mock<IMedicineRepository> _medicineRepositoryMock;
        private readonly Mock<SEP_TestContext> _contextMock;
        private readonly Mock<ILogger<MedicineService>> _loggerMock;
        private readonly MedicineService _service;

        public GetAllReceiversTests()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _contextMock = new Mock<SEP_TestContext>();
            _loggerMock = new Mock<ILogger<MedicineService>>();
            _service = new MedicineService(_medicineRepositoryMock.Object, _contextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void GetAllReceivers_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                TestHelper.CreateUser(1, "User1"),
                TestHelper.CreateUser(2, "User2")
            };
            _medicineRepositoryMock.Setup(repo => repo.GetAllUsers()).Returns(users);

            // Act
            var result = _service.GetAllReceivers();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("User1", result[0].UserName);
            Assert.Equal(2, result[1].UserId);
        }

        [Fact]
        public void GetAllReceivers_ReturnsEmptyListWhenNoUsers()
        {
            // Arrange
            _medicineRepositoryMock.Setup(repo => repo.GetAllUsers()).Returns(new List<User>());

            // Act
            var result = _service.GetAllReceivers();

            // Assert
            Assert.Empty(result);
        }
    }
}
