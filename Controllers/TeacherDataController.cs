using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ScoolProject.Models;
using MySql.Data.MySqlClient;
using System.Xml;
using System.Diagnostics;

namespace SchoolProject.Controllers
{
    /// <summary>
    /// Receive a teacher's data. Returns the list of teachers. Filters by article title matching a search key.
    /// </summary>
    /// <returns>
    /// List of teachers.
    /// </returns>
    /// <param name="SearchKey">
    /// The search key for our teachers
    /// </param>
    /// <example>
    /// /// GET api/teacherData/listTeachers/Linda ->"employee number: T382", "hire date: 2015-08-22T00:00:00", "sarary: 60.22", "first name: Linda", "last name: Chan" 
    /// </example>
    public class TeacherDataController : ApiController
    {
        private SchoolDbContext School = new SchoolDbContext();
        [HttpGet]
        [Route ("api/teacherData/listTeachers/{SearchKey}")]
        public List<teacher> ListTeachers(string SearchKey)
        {
            MySqlConnection Conn = School.AccessDatabase();

            Debug.WriteLine("Data access Search Key of " + SearchKey);

            Conn.Open();
            string query = "Select * from teachers where teacherfname like @SearchKey or teacherlname like @SearchKey or teacherid like @SearchKey or employeenumber like @SearchKey or hiredate like @SearchKey or salary like @SearchKey";
            MySqlCommand Cmd = Conn.CreateCommand();
            Cmd.CommandText = query;
            Cmd.Parameters.AddWithValue("@SearchKey","%"+SearchKey + "%");
            Cmd.Prepare();
            MySqlDataReader ResultSet = Cmd.ExecuteReader();
            List<teacher> Teachers = new List<teacher>();

            while (ResultSet.Read())
            {
                int TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                string TeacherfName = ResultSet["teacherfname"].ToString();
                string TeacherlName = ResultSet["teacherlname"].ToString();
                string EmployeeNum = ResultSet["employeenumber"].ToString();
                DateTime HireDate = Convert.ToDateTime(ResultSet["HireDate"]);
                double Salary = Convert.ToDouble(ResultSet["Salary"]);

                teacher TeacherInfo = new teacher();
                TeacherInfo.TeacherId = TeacherId;
                TeacherInfo.TeacherfName = TeacherfName;
                TeacherInfo.TeacherlName = TeacherlName;
                TeacherInfo.EmployeeNum = EmployeeNum;
                TeacherInfo.HireDate = HireDate;
                TeacherInfo.Salary = Salary;
                Teachers.Add(TeacherInfo);
            }
            Conn.Close();
            return Teachers;
        }
        /// <summary>
        /// Receive a teacher Id. Returns the teacher matching ID
        /// </summary>
        /// <param name="TeacherId">The teacher ID primary key</param>
        /// <returns>
        /// An instance of a teacher object
        /// </returns>
        /// <example>
        /// Get localhost:xx/api/teacherdata/findteacher/3 -> {"teacherId":"3", teacherfName:"Linda", teacherlName:"Chan", employnumber:"T382", salary:"60.22"}
        ///</example>
        [HttpGet]
        [Route("api/TeacherData/FindTeacher/{TeacherId}")]
        public teacher FindTeacher(int TeacherId)
        {
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();
            MySqlCommand Cmd = Conn.CreateCommand();
            string query = "SELECT teachers.teacherid, teacherfname, teacherlname, employeenumber, hiredate, salary, classname From teachers LEFT JOIN classes ON teachers.teacherid = classes.teacherid where teachers.teacherid =" + TeacherId;
            Cmd.CommandText = query;
            MySqlDataReader ResultSet = Cmd.ExecuteReader();
            teacher NewTeacher = new teacher();
            while (ResultSet.Read())
            {
                NewTeacher.TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                NewTeacher.TeacherfName = ResultSet["teacherfname"].ToString();
                NewTeacher.TeacherlName = ResultSet["teacherlname"].ToString();
                NewTeacher.EmployeeNum = ResultSet["employeenumber"].ToString();
                NewTeacher.HireDate = Convert.ToDateTime(ResultSet["HireDate"]);
                NewTeacher.Salary = Convert.ToDouble(ResultSet["Salary"]);
                NewTeacher.className = ResultSet["classname"].ToString();
            }
            return NewTeacher;
        }
        //Create Teacher
        /// <summary>
        /// Recieve teacher information and enters it into the database.
        /// </summary>
        /// <returns>
        /// Adding teacher list
        /// </returns>
        /// <example>
        /// Post: api/TeacherData/AddTeacher
        /// FORM DATA / REQUEST first name and last name / POST teacher name
        /// {
        /// "teacherfname": "Daniel"
        /// "teacherlname": "Smith"
        /// }
        /// </example>
        [HttpPost]
        [Route("api/TeacherData/AddTeacher")]
        public void AddTeacher([FromBody]teacher NewTeacher)
        {
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();

            Debug.WriteLine("API for adding a teacher");
            Debug.WriteLine(NewTeacher.TeacherfName);

            MySqlCommand Cmd = Conn.CreateCommand();

            string query = "insert into teachers (teacherid, teacherfname, teacherlname, employeenumber, hiredate, salary) values (@teacherid, @teacherfname, @teacherlname, @employeenumber, CURRENT_DATE(), @Salary); insert INTO classes(classname, teacherid) values (@className, @teacherid)";
            //I gave up inserting teacherid into classes. Could you teach me how to do that?
            
            Cmd.CommandText = query;
            Cmd.Parameters.AddWithValue("@teacherid", NewTeacher.TeacherId);
            Cmd.Parameters.AddWithValue("@teacherfname", NewTeacher.TeacherfName);
            Cmd.Parameters.AddWithValue("@teacherlname", NewTeacher.TeacherlName);
            Cmd.Parameters.AddWithValue("@employeenumber", NewTeacher.EmployeeNum);
            Cmd.Parameters.AddWithValue("@salary", NewTeacher.Salary);
            Cmd.Parameters.AddWithValue("@className", NewTeacher.className);
            Cmd.ExecuteNonQuery();
            Conn.Close();   
        }

        //Delete Teacher
        /// <summary>
        /// Delete a teacher in the system
        /// </summary>
        /// <param name="TeacherId">The Teacher Id</param>
        /// <returns>
        /// Deleting teachers.
        /// </returns>
        /// <example>
        /// POST: api/TeacherData/DeleteTeacher/{TeacherId}
        /// </example>
        [HttpPost]
        [Route("api/TeacherData/DeleteTeacher/{TeacherId}")]
        public void DeleteTeacher(int TeacherId)
        {
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();
            string query = "delete teachers, classes from teachers LEFT JOIN classes ON teachers.teacherid = classes.teacherid where teachers.teacherid = @teacherid";
            MySqlCommand Cmd = Conn.CreateCommand();
            Cmd.CommandText = query;
            Cmd.Parameters.AddWithValue("@teacherid", TeacherId);
            Cmd.Prepare();

            Cmd.ExecuteNonQuery();

            Conn.Close();
        }
        ///practice
        //Update Teacher
        /// <summary>
        /// Receive some teachers data and a teacher ID and update the teachers in the database corresponding to that ID
        /// </summary>
        /// <returns>
        /// updating teacher list
        /// </returns>
        /// <example>
        /// POST api/TeacherData/UpdateTeacher/11
        /// POST last name / first name
        /// {
        /// "teacherfname"; "John",
        /// "teacherlname"; "Lenon"
        /// }
        /// </example>
        /// <example>
        /// C:\Users\temot\Downloads\SchoolDbContext\SchoolDbContext\testdata>curl -d @testdata.json -H "Content-TYpe: application/json" http://localhost:55371/api/TeacherData/UpdateTeacher/11
        /// </example>
        [HttpPost]
        [Route("api/TeacherData/UpdateTeacher/{TeacherId}")]
        public void UpdateTeacher(int TeacherId, [FromBody]teacher updatedTeacher)
        {
            Debug.WriteLine("Updating teacher information with an id of " + TeacherId);

            string query = "update teachers set teacherfname = @teacherfname, teacherlname = @teacherlname, salary = @Salary, employeenumber = @employeenumber where teacherid = @teacherid";
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();
            MySqlCommand Cmd = Conn.CreateCommand();
            Cmd.CommandText = query;
            Cmd.Parameters.AddWithValue("@teacherid", updatedTeacher.TeacherId);
            Cmd.Parameters.AddWithValue("@teacherfname", updatedTeacher.TeacherfName);
            Cmd.Parameters.AddWithValue("@teacherlname", updatedTeacher.TeacherlName);
            Cmd.Parameters.AddWithValue("@employeenumber", updatedTeacher.EmployeeNum);
            Cmd.Parameters.AddWithValue("@salary", updatedTeacher.Salary);
            Cmd.Prepare() ;
            Cmd.ExecuteNonQuery();

            Conn.Close();
        }
    }
}
