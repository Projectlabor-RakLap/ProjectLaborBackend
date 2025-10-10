using AutoMapper;
using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ProjectLaborBackend.Dtos.UserDTOs;
using ProjectLaborBackend.Email;
using ProjectLaborBackend.Entities;

namespace ProjectLaborBackend.Services
{
    public interface IEmailService
    {
        Task SendEmail(string userEmail, string subject, string template);
    }
    public class EmailService : IEmailService
    {
        private IFluentEmail _fluentEmail;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public EmailService(IFluentEmail fluentEmail, AppDbContext context, IMapper mapper)
        {
            _fluentEmail = fluentEmail;
            _context = context;
            _mapper = mapper;
        }

        public async Task SendEmail(string userEmail, string subject, string template)
        {
            var UserDTO = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            
            if (UserDTO == null)
            {
                throw new KeyNotFoundException("User with given email does not exist!");
            }

            var user = _mapper.Map<User>(UserDTO);

            await _fluentEmail
               .To(userEmail)
               .Subject(subject)
               .UsingTemplateFromFile(template, user)
               .SendAsync();
        }
    }
}
