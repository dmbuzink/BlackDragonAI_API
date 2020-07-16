using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace BlackDragonAIAPI.Models.Validation
{
    public class TimedMessageValidator : AbstractValidator<TimedMessage>
    {
        public TimedMessageValidator()
        {
            this.RuleFor(tm => tm.Command)
                .NotNull()
                .WithMessage("The command should not be null")
                .Length(1, 255)
                .WithMessage("The command should be between 1 and 255 characters (inclusive)")
                .Matches("![^ \\n]")
                .WithMessage("Invalid command name");

            this.RuleFor(tm => tm.IntervalInMinutes)
                .GreaterThanOrEqualTo(0)
                .WithMessage("The interval has to been greater than or equal to 0");

            this.RuleFor(tm => tm.OffsetInMinutes)
                .GreaterThanOrEqualTo(0)
                .WithMessage("The offset has to been greater than or equal to 0");
        }
    }
}
