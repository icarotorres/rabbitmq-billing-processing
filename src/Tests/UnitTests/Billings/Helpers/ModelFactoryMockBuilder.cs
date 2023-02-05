// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Billings.Domain.Models;
using Billings.Domain.Services;
using Moq;

namespace UnitTests.Billings.Helpers
{
    public sealed class ModelFactoryMockBuilder
    {
        private readonly Mock<IModelFactory> _mock;

        private ModelFactoryMockBuilder()
        {
            _mock = new Mock<IModelFactory>();
        }

        public static ModelFactoryMockBuilder Create()
        {
            return new ModelFactoryMockBuilder();
        }

        public ModelFactoryMockBuilder CreateBilling(string cpfString, double amount, string dueDate, Billing model)
        {
            _mock.Setup(x => x.CreateBilling(cpfString, amount, dueDate)).Returns(model);
            return this;
        }

        public IModelFactory Build()
        {
            return _mock.Object;
        }
    }
}
