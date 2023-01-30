namespace Processing.Scheduled.Worker
{

    public class ScheduledProcessorSettings
    {
        public virtual int MillisecondsScheduledTime { get; set; }
        public virtual int MaxErrorCount { get; set; }
    }
}
