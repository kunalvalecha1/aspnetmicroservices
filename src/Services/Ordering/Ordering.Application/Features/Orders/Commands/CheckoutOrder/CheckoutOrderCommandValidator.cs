using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckOutOrderCommandValidator :AbstractValidator<CheckOutOrderCommand>
    {
        public CheckOutOrderCommandValidator()
        {
            RuleFor(p => p.UserName).NotEmpty().WithMessage("{UserName} is required")
                .NotNull()
                .MaximumLength(50).WithMessage("lenght exceeded");
            RuleFor(p => p.EmailAddress).NotEmpty().WithMessage("Email is requred");

            RuleFor(p => p.TotalPrice)
                .NotEmpty().WithMessage("Can't calculate total price")
                .GreaterThan(0).WithMessage("Total amount should be greater than zer0");

            
        }
    }
}
