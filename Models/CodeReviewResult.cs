using System.Collections.Generic;

namespace Histosonics.AzureQueryApp.Models
{
    public class CodeReviewResult
    {
        public List<WorkItem> workItems { get; set; }
    }

    public class WorkItem
    {
        public int id { get; set; }
        public string url { get; set; }
    }

    public class WorkItemQ
    {
        public string workItemType { get; set; }
        public string title { get; set; }
    }
}
