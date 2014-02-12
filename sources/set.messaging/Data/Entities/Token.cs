namespace set.messaging.Data.Entities
{
    public class Token : BaseEntity
    {
        public string AppId { get; set; }
        public App App { get; set; }
        
        public long UsageCount { get; set; }
        public bool IsAppActive { get; set; }
    }
}