using System.Collections.Generic;

namespace set.messaging.Models
{
    public class PageModel<T> where T : BaseModel
    {
        public int PageNumber { get; set; }
        public int PageCount { get; set; }

        public long ItemCount { get; set; }

        public bool HasPrevPage { get; set; }
        public bool HasNextPage { get; set; }

        public List<T> Items { get; set; }
    }
}