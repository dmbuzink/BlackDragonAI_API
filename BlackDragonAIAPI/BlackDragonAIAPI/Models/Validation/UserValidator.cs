using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace BlackDragonAIAPI.Models.Validation
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            this.RuleFor(u => u.Username)
                .NotNull()
                .WithMessage("A username should be given")
                .Length(3, 39)
                .WithMessage("Username should be between 3 and 39 characters");

            this.RuleFor(u => u.Password)
                .NotNull()
                .WithMessage("Password should not be null")
                .Length(1, 255)
                .WithMessage("Password should be between 1 and 255 character (inclusive)");
        }
    }
}
