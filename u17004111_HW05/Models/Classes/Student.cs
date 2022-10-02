using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u17004111_HW05.Models
{
    public class Student
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthdate { get; set; }
        public string Gender { get; set; }
        public string Class { get; set; }
        public int Point { get; set; }

    }
}