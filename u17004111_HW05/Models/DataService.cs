using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using u17004111_HW05.Models.ViewModels;

namespace u17004111_HW05.Models
{
    public class DataService
    {
        private String ConnectionString;

        public DataService()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        //NOTE: With the GET functions, the search function, type filter and author filter must be able to work together and independently.

        /*
        I think I should have one function that handles all possible search possibilities from the searchbar and two filters. The function will change what is sent to the serve based off of which inputs are used. This also makes sense considering all searches must be initiated by the same search button. That way I technically have 1 search function only, for 7 different possible types of search:
        - 0: No parameters passed - invalid search
        - S: Searchbar only 1
        - A: Author filter only 2
        - T: Type filter only 3
        - A+T: Type and author filters 4
        - S+A: Searchbar and author filter 5
        - S+T: Searchbar and type filter 6
        - S+A+T: Searchbar and author and type filters 7

        Use nested if statements to determine what the sqlcommand should be for each of the seven different search possibilities.
        */
        public BooksVM Search(string bookName, int authorId, int typeId)
        {
            BooksVM bookVm = new BooksVM();
            //Would i need a variable and a list of book, author and type in order to add them to the booksVm?
            List<Book> books = new List<Book>();
            Book book = new Book();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = null;
                int searchType = 0;

                //Below is a completely unneccessarily complicated nested if-else decision tree for the different search options. 
                if (bookName == null || bookName == "")
                {
                    //A+T
                    searchType = 4;
                    cmd = new SqlCommand("select * from books where typeId = " + typeId + " AND authorId = " + authorId, connection);

                    if (authorId <= 0)
                    {
                        if (typeId <= 0)
                        {
                            //0 - Invalid search
                            searchType = 0;
                        }
                        else
                        {
                            //T
                            searchType = 3;
                            cmd = new SqlCommand("select * from books where typeId = " + typeId, connection);
                        }
                    }
                    else if (typeId <= 0)
                    {
                        //A
                        searchType = 2;
                        cmd = new SqlCommand("select * from books where authorId = " + authorId, connection);
                    }
                }
                else
                {
                    //S+A+T
                    searchType = 7;
                    cmd = new SqlCommand("select * from books where name LIKE '%" + bookName + "%' AND authorId = " + authorId + " AND typeId = " + typeId, connection);

                    if (authorId <= 0)
                    {
                        if (typeId <= 0)
                        {
                            //S
                            searchType = 1;
                            cmd = new SqlCommand("select * from books where name LIKE '%" + bookName + "%'", connection);
                        }
                        else
                        {
                            //S+T
                            searchType = 6;
                            cmd = new SqlCommand("select * from books where name LIKE '%" + bookName + "%' AND typeId = " + typeId, connection);
                        }
                    }
                    else if (typeId <= 0)
                    {
                        //S+A
                        searchType = 5;
                        cmd = new SqlCommand("select * from books where name LIKE '%" + bookName + "%' AND authorId = " + authorId, connection);
                    }
                }

                using (cmd)
                {
                    bookVm.Authors = GetAllAuthors();
                    bookVm.Types = GetAllTypes();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            book = new Book()
                            {
                                ID = Convert.ToInt32(rdr["bookId"]),
                                Name = Convert.ToString(rdr["name"]),
                                PageCount = Convert.ToInt32(rdr["pagecount"]),
                                Point = Convert.ToInt32(rdr["point"]),
                                AuthorID = Convert.ToInt32(rdr["authorId"]),
                                TypeID = Convert.ToInt32(rdr["typeId"]),
                                Type = Convert.ToString(bookVm.Types.Where(xx => xx.ID == book.TypeID).Select(yy => yy.Name))
                            };
                            List<string> tempauth = (bookVm.Authors.Where(xx => xx.AuthorID == book.AuthorID).Select(yy => yy.Name)).ToList();
                            book.Author = tempauth[0];

                            List<string> temptype = (bookVm.Types.Where(xx => xx.ID == book.TypeID).Select(yy => yy.Name)).ToList();
                            book.Type = temptype[0];

                            books.Add(book);
                        }
                        bookVm.Books = books;
                    }
                }
                connection.Close();
            }
            return bookVm;
        }

        public List<Book> GetAllBooks()
        {
            List<Book> books = new List<Book>();
            Book book;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("select books.bookId, books.name, books.pagecount, books.point, books.authorId, books.typeId, authors.name as [Author], types.name as [Type] from books, authors, types where books.authorId = authors.authorId AND books.typeId = types.typeId", connection);

                using (cmd)
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            book = new Book
                            {
                                ID = Convert.ToInt32(rdr["bookId"]),
                                Name = Convert.ToString(rdr["name"]),
                                PageCount = Convert.ToInt32(rdr["pagecount"]),
                                Point = Convert.ToInt32(rdr["point"]),
                                AuthorID = Convert.ToInt32(rdr["authorId"]),
                                TypeID = Convert.ToInt32(rdr["typeId"]),
                                Author = Convert.ToString(rdr["Author"]),
                                Type = Convert.ToString(rdr["Type"])
                            };

                            books.Add(book);
                        }
                    }
                }
                connection.Close();
            }

            return books;
        }

        public List<Author> GetAllAuthors()
        {
            List<Author> authors = new List<Author>();
            Author author;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("select * from authors", connection);

                using (cmd)
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            author = new Author()
                            {
                                AuthorID = Convert.ToInt32(rdr["authorId"]),
                                Name = Convert.ToString(rdr["name"]),
                                Surname = Convert.ToString(rdr["surname"])
                            };
                            
                            authors.Add(author);
                        }
                    }
                }
                connection.Close();
            }

            return authors;
        }

        public List<Type> GetAllTypes()
        {
            List<Type> types = new List<Type>();
            Type type;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("select * from types", connection);

                using (cmd)
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            type = new Type()
                            {
                                ID = Convert.ToInt32(rdr["typeId"]),
                                Name = Convert.ToString(rdr["name"]),
                            };
                            
                            types.Add(type);
                        }
                    }
                }
                connection.Close();
            }

            return types;
        }

        public List<Student> GetAllStudents()
        {
            List<Student> students = new List<Student>();
            Student student;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("select * from students", connection);

                using (cmd)
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            student = new Student()
                            {
                                ID = Convert.ToInt32(rdr["studentId"]),
                                Name = Convert.ToString(rdr["name"]),
                                Surname = Convert.ToString(rdr["surname"]),
                                Point = Convert.ToInt32(rdr["point"]),
                                Class = Convert.ToString(rdr["class"]),
                                Gender = Convert.ToString(rdr["gender"]),
                                Birthdate = Convert.ToDateTime(rdr["birthdate"])
                            };

                            students.Add(student);
                        }
                    }
                }
                connection.Close();
            }
            return students;
        }

        public List<Borrow> GetBorrows(int id)
        {
            Borrow borrow;
            List<Borrow> borrows = new List<Borrow>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("select * from borrows where borrows.bookId = " + id + " order by borrowId desc", connection);

                using (cmd)
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            borrow = new Borrow
                            {
                                ID = Convert.ToInt32(rdr["borrowId"]),
                                StudentID = Convert.ToInt32(rdr["studentId"]),
                                BookID = Convert.ToInt32(rdr["bookId"]),
                                Taken = Convert.ToString(rdr["takenDate"]),
                                Brought = Convert.ToString(rdr["broughtDate"])
                            };
                            
                            borrows.Add(borrow);
                        }
                    }
                }
                connection.Close();
            }

            return borrows;
        }

        public void BorrowBook(int studid, int bookid)
        {
            int lastID = 0;
            Borrow newBorrow = new Borrow();
            newBorrow.BookID = bookid;
            newBorrow.StudentID = studid;
            newBorrow.Taken = DateTime.Now.ToString();
            newBorrow.Brought = DBNull.Value.ToString();

            //Retrieving only the last/latest borrows from the borrows table in order to assign appropriate borrow id.

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("insert into borrows(studentId, bookId, takenDate) values (" + newBorrow.StudentID + "," + newBorrow.BookID + ",'" + newBorrow.Taken + "')", connection);

                using (cmd)
                {
                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        //The id parameter is the student ID of the student who has taken the book out last and needs to return it still.
        public void ReturnBook(int id)
        {
            string latestBrought = DateTime.Now.ToString();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("update borrows set broughtDate = '" + latestBrought + "' where borrowId=(select max(borrowId) from borrows where studentId = '" + id + "')", connection);

                using (cmd)
                {
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

    }
}