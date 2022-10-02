using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u17004111_HW05.Models.ViewModels
{
    public class BorrowsVM
    {
        public string Book { get; set; }
        public int BookId { get; set; }
        public List<Borrow> Borrows { get; set; }
        public List<Student> Students { get; set; }
        public bool BookStatus { get; set; }

        //Use this default constructor to avoid having to populate in controller.
        public BorrowsVM(int id)
        {
            DataService DS = new DataService();
            Borrows = DS.GetBorrows(id);
            BookId = id;

            List<Student> tempStud = DS.GetAllStudents();
            //This logic returns only the students to whom a borrows is linked for the given book.
            foreach(Student student in DS.GetAllStudents())
            {
                foreach(Borrow borrow in Borrows)
                {
                    if(borrow.StudentID == student.ID)
                    {
                        tempStud.Add(student);
                    }
                }
            }

            Students = tempStud;

            //This is not resource efficient at all.
            foreach (Book book in DS.GetAllBooks())
            {
                if (book.ID == id)
                {
                    Book = book.Name;
                }
            }

            if (DBNull.Value.Equals(Borrows.FirstOrDefault()))
            {
                BookStatus = false;
            }
            else
            {
                BookStatus = true;
            }
        }

    }
}