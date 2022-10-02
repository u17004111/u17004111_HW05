using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using u17004111_HW05.Models;

namespace u17004111_HW05.Models.ViewModels
{
    public class BooksVM
    {
        public List<Book> Books { get; set; }
        public List<Author> Authors { get; set; }
        public List<Type> Types { get; set; }
        
        //Use this default constructor to avoid having to populate in controller.
        public BooksVM()
        {
            DataService DS = new DataService();

            Books = DS.GetAllBooks();
            Authors = DS.GetAllAuthors();
            Types = DS.GetAllTypes();
            
            foreach(Book book in Books)
            {
                List<Borrow> borrows = DS.GetBorrows(book.ID);

                if(borrows != null && borrows.Count != 0)
                {
                    var latestB = borrows.FirstOrDefault();
                    var latestDate = latestB.Brought;
                    if(latestDate == null)
                    {
                        book.Status = "Out";
                    }
                    else
                    {
                        book.Status = "Available";
                    }
                }

            }
        }
    }
}