// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Billings.Application.Models;
using Billings.Domain.Models;
using Billings.Domain.Services;
using FluentValidation;

namespace Billings.Application.Validators
{
    public class BillingRequestValidator : AbstractValidator<BillingRequest>
    {
        public BillingRequestValidator(IModelFactory factory, IValidator<Billing> validator)
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => factory.CreateBilling(x.Cpf, x.Amount, x.DueDate)).SetValidator(validator);
            RuleFor(x => x.DueDate)
                .Must(x => Library.ValueObjects.Date.ValidateFutureDate(x))
                .WithMessage("Vencimento precisa representar uma data válida futura no formato [dd-MM-yyyy]");
        }
    }
}
