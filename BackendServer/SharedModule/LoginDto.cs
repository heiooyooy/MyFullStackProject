using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace SharedModule;

public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleSet("Update", () =>
        {
            RuleFor(dto => dto.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Password can't be empty.");
        });
        
        RuleFor(dto => dto.Email)
            .Cascade(CascadeMode.Stop) 
            .NotEmpty().WithMessage("Email can't be empty.")
            .EmailAddress().WithMessage("Please enter valid email address.");

        RuleFor(dto => dto.Password)
            .Cascade(CascadeMode.Stop) 
            .NotEmpty().WithMessage("Password can't be empty.")
            .Must(BeAStrongPassword).WithMessage("Password is not strong enough.");
    }

    private bool BeAStrongPassword(string password)
    {
        return password.Any(char.IsDigit) && password.Any(char.IsUpper);
    }
}