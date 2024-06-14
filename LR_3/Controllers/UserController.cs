using AutoMapper;
using LR_3.Models;
using LR_3.Models.Dto;
using LR_3.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LR_3.Controllers
{
    [Route("api/v{version:apiVersion}/UsersAuth")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class UserController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IUserRepository _dbUser;
        private readonly IMapper _mapper;

        public UserController(IUserRepository dbUser, IMapper mapper)
        {
            _dbUser = dbUser;
            _mapper = mapper;
            this._response = new();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _dbUser.Login(model);
            if (loginResponse == null || loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorsMessages.Add("Email or password is incorrect");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            bool ifUserEmailUnique = _dbUser.IsUniqueUser(model.Email);
            if (!ifUserEmailUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorsMessages.Add("Email is already exists");
                return BadRequest(_response);
            }
            var user = await _dbUser.Register(model);
            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorsMessages.Add("Error while registering");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            //_response.Result = user;
            return Ok(_response);
        }
    }
}
