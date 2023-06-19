using System.Collections.Generic;

namespace Histosonics.AzureQueryApp.Models
{
    public class WorkItemResult
    {
        public int count { get; set; }
        public List<WorkItemQ> value { get; set; }
    }
}
