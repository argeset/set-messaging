using System.Collections.Generic;

namespace set.messaging.Data.Entities
{
    public class App : BaseEntity
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public string Description { get; set; }
        public string Url { get; set; }

        public virtual ICollection<Token> Tokens { get; set; }

    }
}