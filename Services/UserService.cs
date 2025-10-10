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
        Task<UserGetDTO> ForgotUpdateUserPasswordAsync(ForgotUserPutPasswordDTO UserDTO);
        Task<UserGetDTO> UpdateUserPasswordAsync(int id, UserPutPasswordDTO UserDTO);
        Task AssignUserWarehouseAsync(UserAssignWarehouseDTO UserDTO);
        Task DeleteUserFromWarehouseAsync(UserAssignWarehouseDTO UserDTO);

    }
    public class UserService : IUserService
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly IEmailService _emailService;
        public UserService(AppDbContext _context, IMapper _mapper, IEmailService emailService)
        {
            context = _context;
            mapper = _mapper;
            _emailService = emailService;
        }

        public async Task<List<UserGetDTO>> GetUsersAsync()
        {
            var users = await context.Users.Include(u => u.Warehouses).ToListAsync();
            return mapper.Map<List<UserGetDTO>>(users);
        }

        public async Task<UserGetDTO> GetUserByIdAsync(int id)
        {
            var user = await context.Users
            .Include(u => u.Warehouses) 
            .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return mapper.Map<UserGetDTO>(user);
        }

        public async Task<UserGetDTO> RegisterAsync(UserRegisterDTO UserDTO)
        {
            User? u = await context.Users.FirstOrDefaultAsync(x => x.Email == UserDTO.Email);

            if (u != null)
            {
                if (u.Email == UserDTO.Email) throw new ArgumentException("Email");
            }

            if (UserDTO.FirstName.Length > 75 || UserDTO.LastName.Length > 75)
            {
                throw new ArgumentOutOfRangeException("First and Lastname must be less than 75 characters");
            }

            var user = mapper.Map<User>(UserDTO);
            user.PasswordHash = Argon2.Hash(UserDTO.Password);

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            string templatePath = $"{Directory.GetCurrentDirectory()}/Email/Templates/Welcome.cshtml";
            await _emailService.SendEmail(user.Email, "Üdvözlünk a RakLapnál!", templatePath);

            return mapper.Map<UserGetDTO>(user);
        }

        public async Task<string> LoginAsync(UserLoginDTO UserDTO)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == UserDTO.Email);
            if (user == null || !(Argon2.Verify(user.PasswordHash, UserDTO.Password)))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            return "Logged In";
            //return await GenerateToken(user);
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

            if (UserUpdateDTO.Firstname != null && UserUpdateDTO.Firstname.Length > 75 || UserUpdateDTO.Lastname != null && UserUpdateDTO.Lastname.Length > 75)
            {
                throw new ArgumentOutOfRangeException("First and Lastname must be less than 75 characters");
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

        public async Task<UserGetDTO> ForgotUpdateUserPasswordAsync(ForgotUserPutPasswordDTO UserDTO)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == UserDTO.Email);
            if (user == null)
            {
                throw new KeyNotFoundException("This user does not exists.");
            }
            if (Argon2.Verify(user.PasswordHash, UserDTO.Password))
            {
                return mapper.Map<UserGetDTO>(user);
            }
            user.PasswordHash = Argon2.Hash(UserDTO.Password);
            await context.SaveChangesAsync();

            return mapper.Map<UserGetDTO>(user);
        }

        public async Task<UserGetDTO> UpdateUserPasswordAsync(int id, UserPutPasswordDTO UserDTO)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("This user does not exists.");
            }
            if (!(Argon2.Verify(user.PasswordHash, UserDTO.Password)))
            {
                throw new Exception("Passwords does not match");
            }
            user.PasswordHash = Argon2.Hash(UserDTO.NewPassword);
            await context.SaveChangesAsync();

            return mapper.Map<UserGetDTO>(user);
        }

        public async Task AssignUserWarehouseAsync(UserAssignWarehouseDTO UserDTO)
        {
            var user = await context.Users
            .Include(u => u.Warehouses)
            .FirstOrDefaultAsync(u => u.Id == UserDTO.userID);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var warehouse = await context.Warehouses
                .Include(w => w.Users)
                .FirstOrDefaultAsync(w => w.Id == UserDTO.warehouseID);

            if (warehouse == null)
                throw new KeyNotFoundException("Warehouse not found");

            user.Warehouses.Add(warehouse);
            warehouse.Users.Add(user);

            await context.SaveChangesAsync();
        }

        public async Task DeleteUserFromWarehouseAsync(UserAssignWarehouseDTO UserDTO)
        {
            var user = await context.Users
             .Include(u => u.Warehouses)
             .FirstOrDefaultAsync(u => u.Id == UserDTO.userID);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var warehouse = await context.Warehouses
                .Include(w => w.Users)
                .FirstOrDefaultAsync(w => w.Id == UserDTO.warehouseID);

            if (warehouse == null)
                throw new KeyNotFoundException("Warehouse not found");

            user.Warehouses.Remove(warehouse);
            warehouse.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }
}
