using System.Data.Entity;

namespace set.messaging.Data.Services
{
    public class BaseService
    {
        public readonly DbContext Context;

        public BaseService(DbContext context = null)
        {
            if (context == null)
            {
                context = new SetMessagingDBContext();
            }

            Context = context;
        }
    }
}