// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Customers.Domain.Models;
using Customers.Domain.Services;
using Moq;

namespace UnitTests.Customers.Helpers
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

        public ModelFactoryMockBuilder CreateCustomer(string cpfString, string name, string state, Customer model)
        {
            _mock.Setup(x => x.CreateCustomer(cpfString, name, state)).Returns(model);
            return this;
        }

        public IModelFactory Build()
        {
            return _mock.Object;
        }
    }
}
