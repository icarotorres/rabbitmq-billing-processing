// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Processing.Scheduled.Worker;
using Processing.Scheduled.Worker.Models;
using Processing.Scheduled.Worker.Services;
using Processing.Scheduled.Worker.Workers;
using Xunit;

namespace UnitTests.Scheduled.Worker.Workers
{
    [Trait("processing", "scheduled-worker")]
    public class ScheduledProcessorWorkerTests
    {
        private readonly Mock<IBatchClient> _batchClient;
        private readonly Mock<ScheduledProcessorSettings> _settingsMock;
        private readonly ScheduledProcessorWorker _sut;

        public ScheduledProcessorWorkerTests()
        {
            _batchClient = new Mock<IBatchClient>();
            _settingsMock = new Mock<ScheduledProcessorSettings>();
            _sut = new ScheduledProcessorWorker(_batchClient.Object, null, null, _settingsMock.Object, null);
        }

        [Theory]
        [InlineData(10)]
        public async Task DoExecute_Should_ExecuteProcess_ResetBatchId_And_ReturnProcessedBatch(int retries)
        {
            // arrange
            _settingsMock.Setup(x => x.Retries).Returns(retries);
            _batchClient.Setup(x => x.FetchBatchAsync(It.IsAny<PairedBatch>())).Throws<Exception>();

            // act
            await _sut.DoExecute();

            // assert
            _sut.ErrorsCount.Should().Be(retries);
        }
    }
}
