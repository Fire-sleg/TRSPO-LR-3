using AutoMapper;
using LR_3.Controllers;
using LR_3.Models;
using LR_3.Models.Dto;
using LR_3.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    [TestClass]
    public class UserControllerTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private UserController _userController;

        [TestInitialize]
        public void Initialize()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _userController = new UserController(_userRepositoryMock.Object, _mapperMock.Object);
        }

        [TestMethod]
        public async Task Login_WithValidModel_ReturnsOk()
        {
            // Arrange
            var loginRequestDTO = new LoginRequestDTO { /* initialize with valid data */ };
            var loginResponse = new LoginResponseDTO { User = new LocalUser(), Token = "token" };
            _userRepositoryMock.Setup(repo => repo.Login(loginRequestDTO)).ReturnsAsync(loginResponse);

            // Act
            var result = await _userController.Login(loginRequestDTO);

            // Assert
            var actionResult = result as OkObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task Login_WithInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var loginRequestDTO = new LoginRequestDTO { /* initialize with invalid data */ };
            var loginResponse = new LoginResponseDTO { User = null, Token = null };
            _userRepositoryMock.Setup(repo => repo.Login(loginRequestDTO)).ReturnsAsync(loginResponse);

            // Act
            var result = await _userController.Login(loginRequestDTO);

            // Assert
            var actionResult = result as BadRequestObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task Register_WithUniqueEmail_ReturnsOk()
        {
            // Arrange
            var registrationRequestDTO = new RegistrationRequestDTO { /* initialize with valid data */ };
            _userRepositoryMock.Setup(repo => repo.IsUniqueUser(registrationRequestDTO.Email)).Returns(true);
            _userRepositoryMock.Setup(repo => repo.Register(registrationRequestDTO)).ReturnsAsync(new LocalUser());

            // Act
            var result = await _userController.Register(registrationRequestDTO);

            // Assert
            var actionResult = result as OkObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task Register_WithExistingEmail_ReturnsBadRequest()
        {
            // Arrange
            var registrationRequestDTO = new RegistrationRequestDTO { /* initialize with valid data */ };
            _userRepositoryMock.Setup(repo => repo.IsUniqueUser(registrationRequestDTO.Email)).Returns(false);

            // Act
            var result = await _userController.Register(registrationRequestDTO);

            // Assert
            var actionResult = result as BadRequestObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task Register_ErrorWhileRegistering_ReturnsBadRequest()
        {
            // Arrange
            var registrationRequestDTO = new RegistrationRequestDTO { /* initialize with valid data */ };
            _userRepositoryMock.Setup(repo => repo.IsUniqueUser(registrationRequestDTO.Email)).Returns(true);
            _userRepositoryMock.Setup(repo => repo.Register(registrationRequestDTO)).ReturnsAsync((LocalUser)null);

            // Act
            var result = await _userController.Register(registrationRequestDTO);

            // Assert
            var actionResult = result as BadRequestObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, actionResult.StatusCode);
        }
    }
}
