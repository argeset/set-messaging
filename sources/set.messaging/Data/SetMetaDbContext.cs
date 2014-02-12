using System.Data.Entity;

using set.messaging.Data.Entities;

namespace set.messaging.Data
{
    public class SetMessagingDBContext : DbContext
    {
        public SetMessagingDBContext(string connectionStringOrName)
            : base(connectionStringOrName)
        {
            Database.SetInitializer(new SetMessagingDBInitializer());
        }

        public SetMessagingDBContext()
            : this("Name=SetMessaging")
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<App> Apps { get; set; }
        public DbSet<RequestLog> RequestLogs { get; set; }
    }
}
