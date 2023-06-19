namespace Histosonics.AzureQueryApp.Commands
{
    public class LockCommand
    {
        public bool Lock { get; set; }
        
        public LockCommand(bool lockValue)
        {
            Lock = lockValue;
        }
    }
}
