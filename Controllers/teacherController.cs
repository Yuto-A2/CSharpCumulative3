using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Mysqlx.Datatypes;
using ScoolProject.Models;

namespace SchoolProject.Controllers
{ 
public class TeacherController : Controller
{
        [HttpGet]
        // GET: localhost:xx/teacher/List -> returns a page listing teachers in the system
        //navigate to View/Article/List.cshtml
        public ActionResult List(string SearchKey)
    {
        Debug.WriteLine("Recieved a Search Key of " + SearchKey);
        List<teacher> Teachers = new List<teacher>();
        TeacherDataController controller = new TeacherDataController();
        Teachers = controller.ListTeachers(SearchKey);

        return View(Teachers);
    }
        //GET: localhost:xx/teacher/show/{id} -> Show a particular article matching that ID
        public ActionResult Show(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            teacher SelectedTeacher = controller.FindTeacher(id);
            //navigate to View/Article/Show.cshtml
            return View(SelectedTeacher);
        }

        //Get: localhost:xx/teacher/new -> Show a new teacher.
        public ActionResult New()
        {
           //navigate to views/teacher/new.cshtml
            return View();
        }
        //Post: localhost:xx/teacher/create -> List.cshtml
        [HttpPost]
        public ActionResult Create(string lastName, string firstName, string employee, double salary)
        {
            Debug.WriteLine("We recieved information");
            Debug.WriteLine(lastName);
            Debug.WriteLine(firstName);

            TeacherDataController teacherController = new TeacherDataController();

            teacher NewTeacher = new teacher();
            NewTeacher.TeacherfName = firstName;
            NewTeacher.TeacherlName = lastName;
            NewTeacher.EmployeeNum = employee; 
            NewTeacher.Salary = salary;


            teacherController.AddTeacher(NewTeacher);

            return RedirectToAction("List");
        }
        //Get: /Teacher/DeleteConfirm/{teacherid} -> a website which lets the user confirm their choice to delete
        public ActionResult DeleteConfirm(int id)
        {
            TeacherDataController teacherController = new TeacherDataController();
            teacher SelectedTeacher = 
                teacherController.FindTeacher(id);
            return View(SelectedTeacher);
        }
        //POST: /teacher/Delete/{id} -> the teacher list page
        [HttpPost]
        public ActionResult Delete(int id)
        {
            TeacherDataController teacherController = new TeacherDataController();
            teacherController.DeleteTeacher(id);
            return RedirectToAction("List");
        }
        public ActionResult Update(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            teacher SelectedTeacher = controller.FindTeacher(id);
            return View(SelectedTeacher);
            //GET /teacher/Update/{id} -> A webpage asking the user to update a teacher
            //POST /teacher/Edit/{id} -> Receiving the teacher information to update
        }
        public ActionResult Edit(int id, string lastName, string firstName, string EmployeeNum, double salary)
        {
            TeacherDataController Controller = new TeacherDataController();
            teacher updatedTeacher = new teacher();
            updatedTeacher.TeacherId = id;
            updatedTeacher.TeacherfName = firstName;
            updatedTeacher.TeacherlName = lastName;
            updatedTeacher.EmployeeNum = EmployeeNum;
            updatedTeacher.Salary = salary;

            Controller.UpdateTeacher(id, updatedTeacher);
            return RedirectToAction("Show/" + id);
        
        }
    }
}