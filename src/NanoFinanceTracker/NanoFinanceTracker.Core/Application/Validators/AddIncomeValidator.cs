using FluentValidation;
using NanoFinanceTracker.Core.Application.Dtos.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoFinanceTracker.Core.Application.Validators
{
    public class AddIncomeValidator : AbstractValidator<AddIncome>
    {
        public AddIncomeValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Category).NotEmpty();
        }
    }
}
