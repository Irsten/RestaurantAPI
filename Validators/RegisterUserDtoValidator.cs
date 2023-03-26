using FluentValidation;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;

namespace RestaurantAPI.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        
        public RegisterUserDtoValidator(RestaurantDbContext dbContext)
        {
            RuleFor(r => r.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(r => r.Password)
                .MinimumLength(6);

            RuleFor(r => r.ConfirmPassword)
                .Equal(p => p.Password);

            RuleFor(r => r.Email)
                .Custom((value, context) =>
                {
                    var emailInUse = dbContext.Users.Any(u => u.Email == value);
                    if (emailInUse)
                    {
                        context.AddFailure("Email", "That email is taken.");
                    }
                });
        }
    }
}
