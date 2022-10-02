using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u17004111_HW05.Models
{
    public class Borrow
    {
        public int ID { get; set; }
        public int StudentID { get; set; }
        public int BookID { get; set; }
        public String Taken { get; set; }
        public String Brought { get; set; }
    }
}