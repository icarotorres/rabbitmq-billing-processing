// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Processing.Eventual.Domain.Models;

namespace Processing.Eventual.Application.Abstractions
{
    public interface IBillingsRepository
    {
        Task<List<Billing>> GetCustomerPendingBillingsAsync(ulong cpf, CancellationToken token);
        Task InsertAsync(Billing entity, CancellationToken token);
        Task UpdateManyProcessedAsync(IEnumerable<Billing> entities, CancellationToken token);
        Task RemoveManyConfirmedAsync(IEnumerable<Billing> entities, CancellationToken token);
    }
}
