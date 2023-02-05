// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Billings.Domain.Models;

namespace Billings.Application.Abstractions
{
    /// <summary>
    /// A null-free memory abstraction for I/O database operations related to <see cref="Billing"/>
    /// </summary>
    public interface IBillingRepository
    {
        /// <summary>
        /// Inserts a new record to database
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task InsertAsync(Billing entity, CancellationToken token);

        /// <summary>
        /// Gets a list of <see cref="Billing"/> records from database filtered by cpf and/or month and year.
        /// </summary>
        /// <returns></returns>
        Task<List<Billing>> GetManyAsync(ulong cpf, byte month, ushort year, CancellationToken token);

        /// <summary>
        /// Gets all <see cref="Billing"/> records pending Its amount processing
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<List<Billing>> GetPendingAsync(CancellationToken token);

        /// <summary>
        /// Updates a batch of processed <see cref="Billing"/> records at once
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task UpdateProcessedBatchAsync(IEnumerable<Billing> entities, CancellationToken token = default);
    }
}
