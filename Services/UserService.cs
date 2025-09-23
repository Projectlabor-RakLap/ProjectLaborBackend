using AutoMapper;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectLaborBackend.Dtos.UserDTOs;
using ProjectLaborBackend.Entities;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectLaborBackend.Services
{
    public interface IUserService
    {
        Task<List<UserGetDTO>> GetUsersAsync();
        Task<UserGetDTO> GetUserByIdAsync(int id);
        Task<UserGetDTO> RegisterAsync(UserRegisterDTO UserDTO);
        Task<string> LoginAsync(UserLoginDTO UserDTO);
        Task<UserGetDTO> UpdateProfileAsync(int userId, UserPutDTO UserDTO);
        Task DeleteUser(int id);
    }
    public class UserService : IUserService
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;

        public UserService(AppDbContext _context, IMapper _mapper)
        {
            context = _context;
            mapper = _mapper;
        }

        public async Task<List<UserGetDTO>> GetUsersAsync()
        {
            List<User> users = await context.Users.ToListAsync();
            return mapper.Map<List<UserGetDTO>>(users);
        }

        public async Task<UserGetDTO> GetUserByIdAsync(int id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return mapper.Map<UserGetDTO>(user);
        }

        public async Task<UserGetDTO> RegisterAsync(UserRegisterDTO UserDTO)
        {
            User? u = await context.Users.FirstOrDefaultAsync(x => x.FirstName == UserDTO.FirstName || x.LastName == UserDTO.LastName || x.Email == UserDTO.Email);

            if (u != null)
            {
                string message = "There is already a User with this ";
                if (u.Email == UserDTO.Email) throw new ArgumentException(message + "Email");
                if (u.FirstName == UserDTO.FirstName && u.LastName == UserDTO.LastName) throw new ArgumentException(message + "Username");
            }

            var user = mapper.Map<User>(UserDTO);
            user.PasswordHash = Argon2.Hash(UserDTO.Password);

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return mapper.Map<UserGetDTO>(user);
        }

        public async Task<string> LoginAsync(UserLoginDTO UserDTO)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == UserDTO.Email);
            if (user == null || Argon2.Verify(UserDTO.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            return await GenerateToken(user);
        }

        private async Task<string> GenerateToken(User user)
        {
            return ("Logged In");
        }

        public async Task<UserGetDTO> UpdateProfileAsync(int userId, UserPutDTO UserUpdateDTO)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            if (user.Email != UserUpdateDTO.Email && await context.Users.AnyAsync(u => u.Email == UserUpdateDTO.Email))
            {
                throw new ArgumentException("There is already another User with this email address");
            }

            mapper.Map(UserUpdateDTO, user);

            try
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }

            return mapper.Map<UserGetDTO>(user);
        }

        public async Task DeleteUser(int id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
                return;

            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }
}
