// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Processing.Scheduled.Worker
{

    public class ScheduledProcessorSettings
    {
        public virtual int MillisecondsScheduledTime { get; set; }
        public virtual int MaxErrorCount { get; set; }
    }
}
