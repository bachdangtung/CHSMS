using AutoMapper;
using CHSMS.API.DTOs.User;
using CHSMS.API.Models;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NETCore.MailKit.Core;

namespace CHSMS.API.Tests.AuthServiceTests
{
    public class EditUserProfileAsyncTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly AuthService _authService;

        public EditUserProfileAsyncTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();

            // Only mock dependencies that are actually used
            _authService = new AuthService(
                _userRepositoryMock.Object,
                Mock.Of<IRoleRepository>(),
                Mock.Of<IConfiguration>(),
                Mock.Of<IEmailService>(),
                Mock.Of<IMapper>());
        }

        [Fact]
        public async Task EditUserProfileAsync_ValidUpdate_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var existingUser = TestHelper.CreateTestUser(userId);
            var updateDto = new EditUserProfileDto
            {
                Fullname = "Updated Name",
                Email = "updated@example.com",
                PhoneNumber = "0987654321",
                Address = "Updated Address",
                Gender = "Female",
                Dob = new DateTime(1995, 1, 1)
            };

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId))
                .ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(u => u.GetByEmailAsync(updateDto.Email))
                .ReturnsAsync((User)null);
            _userRepositoryMock.Setup(u => u.GetByPhoneNumber(updateDto.PhoneNumber))
                .ReturnsAsync((User)null);
            _userRepositoryMock.Setup(u => u.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.EditUserProfileAsync(userId, updateDto);

            // Assert
            Assert.True(result);
            Assert.Equal(updateDto.Fullname, existingUser.Fullname);
            Assert.Equal(updateDto.Email, existingUser.Email);
            Assert.Equal(updateDto.PhoneNumber, existingUser.PhoneNumber);
            Assert.Equal(updateDto.Address, existingUser.Address);
            Assert.Equal(updateDto.Gender, existingUser.Gender);
            Assert.Equal(updateDto.Dob, existingUser.Dob);

            _userRepositoryMock.Verify(u => u.UpdateAsync(existingUser), Times.Once);
        }

        [Fact]
        public async Task EditUserProfileAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            var userId = -1;
            var updateDto = new EditUserProfileDto();

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _authService.EditUserProfileAsync(userId, updateDto));
            Assert.Equal("Người dùng không tồn tại", ex.Message);
        }

        [Fact]
        public async Task EditUserProfileAsync_EmailExists_ThrowsException()
        {
            // Arrange
            var userId = 1;
            var existingUser = TestHelper.CreateTestUser(userId);
            var otherUser = TestHelper.CreateTestUser(2);
            var updateDto = new EditUserProfileDto { Email = "test@example.com" };

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId))
                .ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(u => u.GetByEmailAsync(updateDto.Email))
                .ReturnsAsync(otherUser);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _authService.EditUserProfileAsync(userId, updateDto));
            Assert.Equal("Email đã tồn tại", ex.Message);
        }

        [Fact]
        public async Task EditUserProfileAsync_PhoneExists_ThrowsException()
        {
            // Arrange
            var userId = 1;
            var existingUser = TestHelper.CreateTestUser(userId);
            var otherUser = TestHelper.CreateTestUser(2);
            otherUser.PhoneNumber = "9876543210";
            var updateDto = new EditUserProfileDto { PhoneNumber = "9876543210" };

            _userRepositoryMock.Setup(u => u.GetByIdAsync(userId))
                .ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(u => u.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
            _userRepositoryMock.Setup(u => u.GetByPhoneNumber(updateDto.PhoneNumber))
                .ReturnsAsync(otherUser);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _authService.EditUserProfileAsync(userId, updateDto));
            Assert.Equal("Số điện thoại đã tồn tại", ex.Message);
        }
    }
}