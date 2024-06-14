using AutoMapper;
using LR_3.Data;
using LR_3.Models;
using LR_3.Models.Dto;
using LR_3.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LR_3.Controllers
{
    [Route("api/v{version:apiVersion}/Recipes")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class RecipeController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IRecipeRepository _dbRecipe;
        private readonly IUserRepository _dbUser;
        private readonly IMapper _mapper;
        public RecipeController(IRecipeRepository dbRecipe, IUserRepository dbUser, IMapper mapper)
        {
            _dbRecipe = dbRecipe;
            _dbUser = dbUser;
            _mapper = mapper;
            this._response = new ();
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetRecipesAsync()
        {
            try
            {
                IEnumerable<Recipe> RecipeList = await _dbRecipe.GetAllAsync();
                _response.Result = _mapper.Map<List<RecipeDTO>>(RecipeList);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorsMessages.Add(ex.ToString());
            }
            return StatusCode(StatusCodes.Status500InternalServerError, _response); 
            
        }


        [HttpGet("{id:Guid}", Name = "GetRecipe")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
                var recipe = await _dbRecipe.GetAsync(u => u.Id == id);
                _response.Result = _mapper.Map<RecipeDTO>(recipe);
                if (recipe != null)
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorsMessages.Add(ex.ToString() );
            }
            return StatusCode(StatusCodes.Status500InternalServerError, _response);

        }

        [HttpPost("recommendations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetRecommendations([FromBody] List<Guid> productIds)
        {
            try
            {
                if (productIds == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
                IEnumerable<Recipe> recipeList = await _dbRecipe.GetAllAsync(u => u.ProductIds.Intersect(productIds).Any());
                _response.Result = _mapper.Map<List<RecipeDTO>>(recipeList);
                if (recipeList != null)
                {
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    return Ok(_response);
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorsMessages.Add(ex.ToString());
            }
            return StatusCode(StatusCodes.Status500InternalServerError, _response);
        }

        //[MapToApiVersion("2.0")]

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateRecipeAsync([FromBody] RecipeCreateDTO createDTO)
        {
            try
            {
                if (createDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.Result = createDTO;
                    return BadRequest(_response);
                }

                if (await _dbRecipe.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorsMessages.Add("Recipe is already exists!");
                    return BadRequest(_response);
                }

                Recipe model = _mapper.Map<Recipe>(createDTO);

                await _dbRecipe.CreateAsync(model);

                _response.Result = _mapper.Map<RecipeDTO>(model); ;
                _response.StatusCode = HttpStatusCode.Created;
                _response.IsSuccess = true; 

                return CreatedAtRoute("GetRecipe", new { id = model.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorsMessages.Add(ex.ToString());
            }
            return StatusCode(StatusCodes.Status500InternalServerError, _response);

        }

        [HttpDelete("{id:Guid}", Name = "DeleteRecipe")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteRecipeAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                var recipe = await _dbRecipe.GetAsync(u => u.Id == id);
                if (recipe == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound; 
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }
                await _dbRecipe.RemoveAsync(recipe);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return NoContent(); //Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorsMessages = new List<string> { ex.ToString() };
            }
            return StatusCode(StatusCodes.Status500InternalServerError, _response);

        }

        [HttpPut("{id:Guid}", Name = "UpdateRecipe")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateRecipeAsync(Guid id, [FromBody] RecipeUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                Recipe model = _mapper.Map<Recipe>(updateDTO);

                await _dbRecipe.UpdateAsync(model);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return NoContent();//Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorsMessages.Add(ex.ToString() );
            }
            return StatusCode(StatusCodes.Status500InternalServerError, _response);
            
        }

        [HttpPatch("{id:Guid}", Name = "UpdatePartialRecipe")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialRecipeAsync(Guid id, JsonPatchDocument<RecipeUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == Guid.Empty)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            /*---AsNoTracking()---*/
            var recipe = await _dbRecipe.GetAsync(u => u.Id == id, tracked:false);

            RecipeUpdateDTO recipeDTO = _mapper.Map<RecipeUpdateDTO>(recipe);

            if (recipe == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }

            patchDTO.ApplyTo(recipeDTO, ModelState);

            Recipe model = _mapper.Map<Recipe>(recipeDTO);


            await _dbRecipe.UpdateAsync(model);

            if (!ModelState.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(ModelState);
            }

            return NoContent();
        }
    }
}
