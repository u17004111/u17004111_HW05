using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u17004111_HW05.Models.ViewModels
{
    public class StudentsVM
    {
        private Student currentBStud;

        public List<Student> Students { get; set; }
        public List<string> Classes { get; set; }
        //This is only populated if the last student has NOT RETURNED the book
        public Student CurrentBStud { get => currentBStud; set => currentBStud = value; }
        public int BookID { get; set; }

        public StudentsVM(int bookId)
        {
            DataService DS = new DataService();
            Students = DS.GetAllStudents();
            Classes = new List<string>() { "9A", "9B", "9C", "9D", "9E", "10A", "10B", "10C", "10D", "10E", "11A", "11B", "11C", "11D", "11E", "12A", "12B", "12C", "12D", "12E" };
            BookID = bookId;

            //Logically, we should be able to determine who the current borrowing student is from the last/latest borrows for  given book. So, first find relevant borrows, then look at last borrow, then determine student and availability.
            Borrow lastB = DS.GetBorrows(bookId).First();
            var studId = lastB.StudentID;

            //Only if the book hasn't been returned do we check which student has it.
            if (lastB.Brought == "" || lastB.Brought == null)
            {
                foreach (var stud in DS.GetAllStudents())
                {
                    if (stud.ID == studId)
                    {
                        CurrentBStud = stud;
                    }
                }
            }
            else
            {
                CurrentBStud = null;
            }
        }
    }
}