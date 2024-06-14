using AutoMapper;
using LR_3;
using LR_3.Controllers;
using LR_3.Models;
using LR_3.Models.Dto;
using LR_3.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public class RecipeControllerTests
    {
        private RecipeController _controller;
        private Mock<IRecipeRepository> _mockRecipeRepository;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IMapper> _mockMapper;

        [TestInitialize]
        public void Setup()
        {
            _mockRecipeRepository = new Mock<IRecipeRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new RecipeController(_mockRecipeRepository.Object, _mockUserRepository.Object, _mockMapper.Object);
        }

        [TestMethod]
        public async Task GetRecipesAsync_Returns_OKResult()
        {
            // Arrange
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();

            var expectedRecipes = new List<Recipe>
            {
                new Recipe { Id = guid1, Name = "Recipe 1" },
                new Recipe { Id = guid2, Name = "Recipe 2" }
            };
            var expectedRecipeDTOs = new List<RecipeDTO>
            {
                new RecipeDTO { Id = guid1, Name = "Recipe 1" },
                new RecipeDTO { Id = guid2, Name = "Recipe 2" }
            };

            _mockRecipeRepository.Setup(repo => repo.GetAllAsync(null)).ReturnsAsync(expectedRecipes);
            _mockMapper.Setup(mapper => mapper.Map<List<RecipeDTO>>(expectedRecipes)).Returns(expectedRecipeDTOs);

            // Act
            var result = await _controller.GetRecipesAsync();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);

            var response = okResult.Value as APIResponse;
            Assert.IsNotNull(response);
            CollectionAssert.AreEqual(expectedRecipeDTOs, (System.Collections.ICollection)response.Result);
        }

        [TestMethod]
        public async Task GetRecipesAsync_Returns_InternalServerError_When_Exception()
        {
            //It.IsAny<Expression<Func<Event, bool>>>()
            // Arrange
            _mockRecipeRepository.Setup(repo => repo.GetAllAsync(null)).ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.GetRecipesAsync();

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
            var RecipeEntity = new Recipe
            {
                Id = Guid.NewGuid(),
                Name = "Test"
            };

            var RecipeDTO = new RecipeDTO
            {
                Id = RecipeEntity.Id,
                Name = "Test"
            };
            // Arrange
            var RecipeId = Guid.NewGuid();
            _mockRecipeRepository.Setup(repo => repo.GetAsync(u => u.Id == RecipeId, true)).ReturnsAsync(RecipeEntity);
            _mockMapper.Setup(mapper => mapper.Map<RecipeDTO>(RecipeEntity)).Returns(RecipeDTO);

            // Act
            var result = await _controller.GetAsync(RecipeId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);

            var response = okResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.AreEqual(RecipeDTO, response.Result);
        }

        [TestMethod]
        public async Task GetAsync_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var RecipeId = Guid.Empty;
            Debug.WriteLine(RecipeId);
            // Act
            var result = await _controller.GetAsync(RecipeId);
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
        public async Task GetAsync_WithNonExistingRecipe_ReturnsNotFound()
        {
            // Arrange
            var RecipeId = Guid.NewGuid();
            _mockRecipeRepository.Setup(repo => repo.GetAsync(u => u.Id == RecipeId, true)).ReturnsAsync((Recipe)null);

            // Act
            var result = await _controller.GetAsync(RecipeId);

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
            var RecipeId = Guid.NewGuid();
            _mockRecipeRepository.Setup(repo => repo.GetAsync(u => u.Id == RecipeId, true)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetAsync(RecipeId);

            // Assert
            var objectResult = result.Result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            var response = objectResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        public async Task CreateRecipeAsync_WithValidDTO_ReturnsCreated()
        {
            // Arrange
            var createDTO = new RecipeCreateDTO
            {
                Name = "New Recipe",
            };

            _mockRecipeRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), true)).ReturnsAsync((Recipe)null);
            _mockMapper.Setup(mapper => mapper.Map<Recipe>(createDTO)).Returns(new Recipe());
            _mockMapper.Setup(mapper => mapper.Map<RecipeDTO>(It.IsAny<Recipe>())).Returns(new RecipeDTO());

            // Act
            var result = await _controller.CreateRecipeAsync(createDTO);

            // Assert
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            Assert.IsNotNull(createdAtRouteResult);
            Assert.AreEqual(StatusCodes.Status201Created, createdAtRouteResult.StatusCode);

            var response = createdAtRouteResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task CreateRecipeAsync_WithNullModel_ReturnsBadRequest()
        {
            // Arrange
            RecipeCreateDTO createDTO = null;

            // Act
            var result = await _controller.CreateRecipeAsync(createDTO);

            // Assert
            var actionResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateRecipeAsync_RecipeAlreadyExists_ReturnsBadRequest()
        {
            // Arrange
            var createDTO = new RecipeCreateDTO { /* initialize with valid data */ };
            var existingRecipe = new Recipe { /* initialize with valid data */ };
            _mockRecipeRepository.Setup(repo => repo.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower(), true)).ReturnsAsync(existingRecipe);

            // Act
            var result = await _controller.CreateRecipeAsync(createDTO);

            // Assert
            var actionResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateRecipeAsync_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var createDTO = new RecipeCreateDTO { /* initialize with valid data */ };
            _mockRecipeRepository.Setup(repo => repo.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower(), true)).Throws(new Exception("Simulated error"));

            // Act
            var result = await _controller.CreateRecipeAsync(createDTO);

            // Assert
            var actionResult = result.Result as ObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteRecipeAsync_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var RecipeId = Guid.NewGuid();
            var RecipeEntity = new Recipe { Id = RecipeId }; // Assuming you have an Recipe entity class
            _mockRecipeRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), true)).ReturnsAsync(RecipeEntity);

            // Act
            var result = await _controller.DeleteRecipeAsync(RecipeId);

            // Assert
            var noContentResult = result.Result as NoContentResult;

            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }


        [TestMethod]
        public async Task DeleteRecipeAsync_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var result = await _controller.DeleteRecipeAsync(invalidId);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            var response = badRequestResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        public async Task DeleteRecipeAsync_WithNonexistentId_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            _mockRecipeRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), true)).ReturnsAsync((Recipe)null);

            // Act
            var result = await _controller.DeleteRecipeAsync(nonExistentId);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

            var response = notFoundResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        public async Task DeleteRecipeAsync_ExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            var RecipeId = Guid.NewGuid();
            _mockRecipeRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), true)).Throws(new Exception("Simulated error"));

            // Act
            var result = await _controller.DeleteRecipeAsync(RecipeId);

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
        public async Task UpdateRecipeAsync_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var RecipeId = Guid.NewGuid();
            var updateDTO = new RecipeUpdateDTO { Id = RecipeId, /* other properties */ };
            var RecipeModel = new Recipe { Id = RecipeId };
            _mockMapper.Setup(mapper => mapper.Map<Recipe>(updateDTO)).Returns(RecipeModel);

            // Act
            var result = await _controller.UpdateRecipeAsync(RecipeId, updateDTO);

            // Assert
            var actionResult = result.Result as StatusCodeResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status204NoContent, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateRecipeAsync_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var RecipeId = Guid.NewGuid();
            var updateDTO = new RecipeUpdateDTO { Id = Guid.NewGuid(), /* other properties */ }; // Invalid ID

            // Act
            var result = await _controller.UpdateRecipeAsync(RecipeId, updateDTO);

            // Assert
            var actionResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, actionResult.StatusCode);

            var response = actionResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        public async Task UpdateRecipeAsync_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var RecipeId = Guid.NewGuid();
            var updateDTO = new RecipeUpdateDTO { Id = RecipeId, /* other properties */ };
            _mockMapper.Setup(mapper => mapper.Map<Recipe>(updateDTO)).Throws(new Exception("Simulated error"));

            // Act
            var result = await _controller.UpdateRecipeAsync(RecipeId, updateDTO);

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
        public async Task UpdatePartialRecipeAsync_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var RecipeId = Guid.NewGuid();
            var patchDocument = new JsonPatchDocument<RecipeUpdateDTO>();
            patchDocument.Replace(e => e.Name, "New Value"); // Define patch operation
            var Recipe = new Recipe { Id = RecipeId };
            var messageDTO = new RecipeUpdateDTO { Id = RecipeId };
            var model = new Recipe { Id = RecipeId };
            _mockMapper.Setup(mapper => mapper.Map<RecipeUpdateDTO>(Recipe)).Returns(messageDTO);
            _mockMapper.Setup(mapper => mapper.Map<Recipe>(messageDTO)).Returns(model);
            _mockRecipeRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Recipe, bool>>>(), false)).ReturnsAsync(Recipe);

            // Act
            var result = await _controller.UpdatePartialRecipeAsync(RecipeId, patchDocument);

            // Assert
            var actionResult = result as NoContentResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status204NoContent, actionResult.StatusCode);

        }

        [TestMethod]
        public async Task UpdatePartialRecipeAsync_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var RecipeId = Guid.Empty; // Invalid ID
            var patchDocument = new JsonPatchDocument<RecipeUpdateDTO>();

            // Act
            var result = await _controller.UpdatePartialRecipeAsync(RecipeId, patchDocument);

            // Assert
            var actionResult = result as BadRequestObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, actionResult.StatusCode);

            var response = actionResult.Value as APIResponse;
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
        }


        [TestMethod]
        public async Task GetRecommendations_Returns_BadRequest_When_ProductIds_Null()
        {
            // Arrange
            List<Guid> nullProductIds = null;

            // Act
            var result = await _controller.GetRecommendations(nullProductIds);

            // Assert
            var actionResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status400BadRequest, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task GetRecommendations_Returns_NotFound_When_No_Recipes_Found()
        {
            // Arrange
            List<Guid> productIds = new List<Guid> { Guid.NewGuid() }; // Provide valid productIds here
            _mockRecipeRepository.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Recipe, bool>>>())).ReturnsAsync(() => null);

            // Act
            var result = await _controller.GetRecommendations(productIds);

            // Assert
            var actionResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status404NotFound, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task GetRecommendations_Returns_Ok_With_RecipeDTOs_When_Recipes_Found()
        {
            // Arrange
            List<Guid> productIds = new List<Guid> { Guid.NewGuid() }; // Provide valid productIds here

            var id = Guid.NewGuid();

            var recipes = new List<Recipe>() { 
                new Recipe{
                    Id = id
                } 
            };
            var recipesDTO = new List<RecipeDTO>()
            {
                new RecipeDTO{
                    Id = id 
                }

            };
            _mockRecipeRepository.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Recipe, bool>>>())).ReturnsAsync(recipes);
            _mockMapper.Setup(mapper => mapper.Map<List<RecipeDTO>>(recipes)).Returns(recipesDTO);

            // Act 
            var result = await _controller.GetRecommendations(productIds);
            Debug.WriteLine(result.Result);
            // Assert
            var actionResult = result.Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, actionResult.StatusCode);
        }
        
    }
}

