using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentValidation;

namespace BlackDragonAIAPI.Models.Validation
{
    public class CommandValidator : AbstractValidator<CommandDetails>
    {
        public CommandValidator()
        {
            this.RuleFor(c => c.Command)
                .NotNull()
                .WithMessage("The command name should not be null")
                .Length(1, 255)
                .WithMessage("The command name must be longer than 1 character")
                .Matches(new Regex("![^ \\n]+"))
                .WithMessage("Invalid command name");

            this.RuleFor(c => c.Message)
                .NotNull()
                .WithMessage("The message should not be null")
                .Length(1, 500)
                .WithMessage("The message should be at least 1 character and no more than 500 characters.");

            this.RuleFor(c => c.Permission)
                .NotNull()
                .WithMessage("A permission should be given")
                .IsInEnum()
                .WithMessage("Invalid enum value");

            this.RuleFor(c => c.Timer)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Timer should be greater than or equal to 0");
        }
    }
}
