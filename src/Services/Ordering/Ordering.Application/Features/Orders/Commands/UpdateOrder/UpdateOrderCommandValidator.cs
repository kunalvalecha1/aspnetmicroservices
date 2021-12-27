using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(p => p.UserName).NotEmpty().WithMessage("{UserName} is required")
                 .NotNull()
                 .MaximumLength(50).WithMessage("lenght exceeded");
            RuleFor(p => p.EmailAddress).NotEmpty().WithMessage("Email is requred");

            RuleFor(p => p.TotalPrice)
                .NotEmpty().WithMessage("Can't calculate total price")
                .GreaterThan(0).WithMessage("Total amount should be greater than zero");
        }
    }
}
