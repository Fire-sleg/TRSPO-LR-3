using AutoMapper;
using LR_3;
using LR_3.Controllers;
using LR_3.Models;
using LR_3.Models.Dto;
using LR_3.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;

namespace TestProject
{
    [TestClass]
    public class ProductControllerTests
    {
        private ProductController _controller;
        private Mock<IProductRepository> _mockProductRepository;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IMapper> _mockMapper;

        [TestInitialize]
        public void Setup()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new ProductController(_mockProductRepository.Object, _mockUserRepository.Object, _mockMapper.Object);
        }

        [TestMethod]
        public async Task GetProductsAsync_Returns_OKResult()
        {
            // Arrange
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();

            var expectedProducts = new List<Product>
            {
                new Product { Id = guid1, Name = "Product 1" },
                new Product { Id = guid2, Name = "Product 2" }
            };
            var expectedProductDTOs = new List<ProductDTO>
            {
                new ProductDTO { Id = guid1, Name = "Product 1" },
                new ProductDTO { Id = guid2, Name = "Product 2" }
            };

            _mockProductRepository.Setup(repo => repo.GetAllAsync(null)).ReturnsAsync(expectedProducts);
            _mockMapper.Setup(mapper => mapper.Map<List<ProductDTO>>(expectedProducts)).Returns(expectedProductDTOs);

            // Act
            var result = await _controller.GetProductsAsync();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);

            var response = okResult.Value as APIResponse;
            Assert.IsNotNull(response);
            CollectionAssert.AreEqual(expectedProductDTOs, (System.Collections.ICollection)response.Result);
        }

        [TestMethod]
        public async Task GetProductsAsync_Returns_InternalServerError_When_Exception()
        {
            //It.IsAny<Expression<Func<Event, bool>>>()
            // Arrange
            _mockProductRepository.Setup(repo => repo.GetAllAsync(null)).ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetProductsAsync();

            // Assert
            var objectResult = result.Result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            var response = objectResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
            Assert.IsNotNull(response.ErrorsMessages);
            Assert.AreEqual(1, response.ErrorsMessages.Count);
            Assert.IsTrue(response.ErrorsMessages[0].StartsWith("System.Exception: Test Exception"));
        }

        [TestMethod]
        public async Task GetAsync_WithValidId_ReturnsOk()
        {
            var productEntity = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test"
            };

            var productDTO = new ProductDTO
            {
                Id = productEntity.Id,
                Name = "Test"
            };
            // Arrange
            var productId = Guid.NewGuid();
            _mockProductRepository.Setup(repo => repo.GetAsync(u => u.Id == productId, true)).ReturnsAsync(productEntity);
            _mockMapper.Setup(mapper => mapper.Map<ProductDTO>(productEntity)).Returns(productDTO);

            // Act
            var result = await _controller.GetAsync(productId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);

            var response = okResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.AreEqual(productDTO, response.Result);
        }

        [TestMethod]
        public async Task GetAsync_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var productId = Guid.Empty;
            Debug.WriteLine(productId);
            // Act
            var result = await _controller.GetAsync(productId);
            Debug.WriteLine(result);
            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Debug.WriteLine(badRequestResult);
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            var response = badRequestResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        public async Task GetAsync_WithNonExistingEvent_ReturnsNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockProductRepository.Setup(repo => repo.GetAsync(u => u.Id == productId, true)).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.GetAsync(productId);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

            var response = notFoundResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        public async Task GetAsync_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockProductRepository.Setup(repo => repo.GetAsync(u => u.Id == productId, true)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetAsync(productId);

            // Assert
            var objectResult = result.Result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            var response = objectResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        public async Task CreateProductAsync_WithValidDTO_ReturnsCreated()
        {
            // Arrange
            var createDTO = new ProductCreateDTO
            {
                Name = "New Product",
            };

            _mockProductRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(), true)).ReturnsAsync((Product)null);
            _mockMapper.Setup(mapper => mapper.Map<Product>(createDTO)).Returns(new Product());
            _mockMapper.Setup(mapper => mapper.Map<ProductDTO>(It.IsAny<Product>())).Returns(new ProductDTO());

            // Act
            var result = await _controller.CreateProductAsync(createDTO);

            // Assert
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            Assert.IsNotNull(createdAtRouteResult);
            Assert.AreEqual(StatusCodes.Status201Created, createdAtRouteResult.StatusCode);

            var response = createdAtRouteResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task CreateProductAsync_WithNullModel_ReturnsBadRequest()
        {
            // Arrange
            ProductCreateDTO createDTO = null;

            // Act
            var result = await _controller.CreateProductAsync(createDTO);

            // Assert
            var actionResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateProductAsync_ProductAlreadyExists_ReturnsBadRequest()
        {
            // Arrange
            var createDTO = new ProductCreateDTO { /* initialize with valid data */ };
            var existingProduct = new Product { /* initialize with valid data */ };
            _mockProductRepository.Setup(repo => repo.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower(), true)).ReturnsAsync(existingProduct);

            // Act
            var result = await _controller.CreateProductAsync(createDTO);

            // Assert
            var actionResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateProductAsync_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var createDTO = new ProductCreateDTO { /* initialize with valid data */ };
            _mockProductRepository.Setup(repo => repo.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower(), true)).Throws(new Exception("Simulated error"));

            // Act
            var result = await _controller.CreateProductAsync(createDTO);

            // Assert
            var actionResult = result.Result as ObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteProductAsync_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productEntity = new Product { Id = productId }; // Assuming you have an Product entity class
            _mockProductRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(), true)).ReturnsAsync(productEntity);

            // Act
            var result = await _controller.DeleteProductAsync(productId);

            // Assert
            var noContentResult = result.Result as NoContentResult;

            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }


        [TestMethod]
        public async Task DeleteProductAsync_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var result = await _controller.DeleteProductAsync(invalidId);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            var response = badRequestResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        public async Task DeleteProductAsync_WithNonexistentId_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            _mockProductRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(), true)).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.DeleteProductAsync(nonExistentId);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

            var response = notFoundResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        public async Task DeleteProductAsync_ExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockProductRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(), true)).Throws(new Exception("Simulated error"));

            // Act
            var result = await _controller.DeleteProductAsync(productId);

            // Assert
            var objectResult = result.Result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            var response = objectResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
            Assert.IsNotNull(response.ErrorsMessages);
            Assert.AreEqual(1, response.ErrorsMessages.Count);
            Debug.WriteLine(response.ErrorsMessages[0]);
            Assert.IsTrue(response.ErrorsMessages[0].StartsWith("System.Exception: Simulated error"));
        }


        [TestMethod]
        public async Task UpdateProductAsync_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var updateDTO = new ProductUpdateDTO { Id = productId, /* other properties */ };
            var productModel = new Product { Id = productId };
            _mockMapper.Setup(mapper => mapper.Map<Product>(updateDTO)).Returns(productModel);

            // Act
            var result = await _controller.UpdateProductAsync(productId, updateDTO);

            // Assert
            var actionResult = result.Result as StatusCodeResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status204NoContent, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateProductAsync_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var updateDTO = new ProductUpdateDTO { Id = Guid.NewGuid(), /* other properties */ }; // Invalid ID

            // Act
            var result = await _controller.UpdateProductAsync(productId, updateDTO);

            // Assert
            var actionResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, actionResult.StatusCode);

            var response = actionResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        public async Task UpdateProductAsync_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var updateDTO = new ProductUpdateDTO { Id = productId, /* other properties */ };
            _mockMapper.Setup(mapper => mapper.Map<Product>(updateDTO)).Throws(new Exception("Simulated error"));

            // Act
            var result = await _controller.UpdateProductAsync(productId, updateDTO);

            // Assert
            var actionResult = result.Result as ObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, actionResult.StatusCode);

            var response = actionResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
            Assert.IsNotNull(response.ErrorsMessages);
            Assert.AreEqual(1, response.ErrorsMessages.Count);
            Debug.WriteLine(response.ErrorsMessages[0]);
            Assert.IsTrue(response.ErrorsMessages[0].StartsWith("System.Exception: Simulated error"));
        }


        [TestMethod]
        public async Task UpdatePartialProductAsync_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var patchDocument = new JsonPatchDocument<ProductUpdateDTO>();
            patchDocument.Replace(e => e.Name, "New Value"); // Define patch operation
            var product = new Product { Id = productId };
            var messageDTO = new ProductUpdateDTO { Id = productId };
            var model = new Product { Id = productId };
            _mockMapper.Setup(mapper => mapper.Map<ProductUpdateDTO>(product)).Returns(messageDTO);
            _mockMapper.Setup(mapper => mapper.Map<Product>(messageDTO)).Returns(model);
            _mockProductRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Product, bool>>>(), false)).ReturnsAsync(product);

            // Act
            var result = await _controller.UpdatePartialProductAsync(productId, patchDocument);

            // Assert
            var actionResult = result as NoContentResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status204NoContent, actionResult.StatusCode);

        }

        [TestMethod]
        public async Task UpdatePartialProductAsync_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var productId = Guid.Empty; // Invalid ID
            var patchDocument = new JsonPatchDocument<ProductUpdateDTO>();

            // Act
            var result = await _controller.UpdatePartialProductAsync(productId, patchDocument);

            // Assert
            var actionResult = result as BadRequestObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, actionResult.StatusCode);

            var response = actionResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
        }
    }
}