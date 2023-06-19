using System.Collections.Generic;

namespace Histosonics.AzureQueryApp.Models
{
    public class ChangeSetResult
    {
        public int count { get; set; }
        public List<ChangeSet> value { get; set; }
    }

    public class ChangeSet
    {
        public int changesetId { get; set; }
        public Author author { get; set; }
    }

    public class Author
    {
        public string displayName { get; set; }
    }
}
