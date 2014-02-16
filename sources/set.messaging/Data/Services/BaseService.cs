using System.Data.Entity;

namespace set.messaging.Data.Services
{
    public class BaseService
    {
        public readonly SetMessagingDBContext Context;

        public BaseService(SetMessagingDBContext context = null)
        {
            if (context == null)
            {
                context = new SetMessagingDBContext();
            }

            Context = context;
        }
    }
}