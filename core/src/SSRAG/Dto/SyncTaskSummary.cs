using System.Collections.Generic;

namespace SSRAG.Dto
{
    public class SyncTaskSummary
    {
        public SyncTaskSummary(List<SyncTaskSummaryItem> tasks, int count)
        {
            Tasks = tasks;
            Count = count;
        }

        public List<SyncTaskSummaryItem> Tasks { get; }

        public int Count { get; }
    }
}
