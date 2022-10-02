using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using u17004111_HW05.Models;
using u17004111_HW05.Models.ViewModels;

namespace u17004111_HW05.Controllers
{
    public class HomeController : Controller
    {
        DataService DS = new DataService();

        [HttpGet]
        public ActionResult Index()
        {
            BooksVM bvm = new BooksVM();

            return View(bvm);
        }

        [HttpPost]
        public ActionResult Index(string bname, int authorId, int typeId)
        {
            BooksVM bvm = new BooksVM();
            bvm = DS.Search(bname, authorId, typeId);

            
            //The view make suse of BooksVM model type so using that here again.
            return View(bvm);
        }

        public ActionResult BookDetails(int id)
        {
            BorrowsVM borrowVm = new BorrowsVM(id);

            return View(borrowVm);
        }

        [HttpGet]
        public ActionResult Students(int id)
        {
            StudentsVM studentVm = new StudentsVM(id);

            return View(studentVm);
        }

        [HttpPost]
        public ActionResult Students(string sname, string cname, int id)
        {
            StudentsVM studs = new StudentsVM(id);
            List<Student> studsList = new List<Student>();

            if(sname != null && sname != "")
            {
                if(cname != "0" && cname != "")
                {
                    //Both search parameters
                    foreach (var stud in studs.Students)
                    {
                        if (stud.Name.ToLower().Contains(sname.ToLower()) && stud.Class == cname)
                        {
                            studsList.Add(stud);
                        }
                    }
                }
                else
                {
                    //Only the student name search parameter
                    foreach (var stud in studs.Students)
                    {
                        if (stud.Name.ToLower().Contains(sname.ToLower()))
                        {
                            studsList.Add(stud);
                        }
                    }
                }
            }
            else
            {
                if (cname != "0" && cname != "")
                {
                    //Only the classname search parameter
                    foreach (var stud in studs.Students)
                    {
                        if (stud.Class == cname)
                        {
                            studsList.Add(stud);
                        }
                    }
                }
                else
                {
                    //No search parameters here then.
                }
            }

            studs.Students = studsList;

            return View(studs);
        }

        public ActionResult BorrowBook(int studid, int bookid)
        {
            DS.BorrowBook(studid, bookid);

            return RedirectToAction("BookDetails", new { id = bookid});
        }

        public ActionResult ReturnBook(int id, int bookid)
        {
            DS.ReturnBook(id);

            return RedirectToAction("BookDetails", new { id = bookid });
        }
    }
}