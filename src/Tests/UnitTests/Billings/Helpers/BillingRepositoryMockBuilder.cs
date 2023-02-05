﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Billings.Application.Abstractions;
using Billings.Domain.Models;
using Moq;

namespace UnitTests.Billings.Helpers
{
    public sealed class BillingRepositoryMockBuilder
    {
        private readonly Mock<IBillingRepository> _mock;

        private BillingRepositoryMockBuilder()
        {
            _mock = new Mock<IBillingRepository>();
        }

        public static BillingRepositoryMockBuilder Create()
        {
            return new BillingRepositoryMockBuilder();
        }

        public BillingRepositoryMockBuilder GetMany(ulong cpf, byte month, ushort year, List<Billing> result)
        {
            _mock.Setup(x => x.GetManyAsync(cpf, month, year, default)).ReturnsAsync(result);
            return this;
        }

        public BillingRepositoryMockBuilder GetPending(List<Billing> result)
        {
            _mock.Setup(x => x.GetPendingAsync(default)).ReturnsAsync(result);
            return this;
        }

        public BillingRepositoryMockBuilder Insert(Billing entity, Task result)
        {
            _mock.Setup(x => x.InsertAsync(entity, default)).Returns(result);
            return this;
        }

        public BillingRepositoryMockBuilder UpdateProcessedBatch(List<Billing> entities, Task result)
        {
            _mock.Setup(x => x.UpdateProcessedBatchAsync(entities, default)).Returns(result);
            return this;
        }

        public IBillingRepository Build()
        {
            return _mock.Object;
        }
    }
}
