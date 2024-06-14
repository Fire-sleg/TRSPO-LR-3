using LR_3.Models;
using LR_3.Models.Dto;
using System.Linq.Expressions;

namespace LR_3.Repository.IRepository
{
    public interface IUserRepository : IRepository<LocalUser>
    {
        bool IsUniqueUser(string email);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO);
    }
}
