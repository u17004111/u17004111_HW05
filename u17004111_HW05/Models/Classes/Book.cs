using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u17004111_HW05.Models
{
    public class Book
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int PageCount { get; set; }
        public int Point { get; set; }
        public int? AuthorID { get; set; }
        public int? TypeID { get; set; }

        //These are not entity properties, but are needed for display purposes.
        public string Author { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

    }
}