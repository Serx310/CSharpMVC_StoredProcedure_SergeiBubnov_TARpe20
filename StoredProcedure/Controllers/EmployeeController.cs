using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StoredProcedure.Data;
using StoredProcedure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoredProcedure.Controllers
{
    public class EmployeeController : Controller
    {
        public StoredProcedureDBContext _context;
        public IConfiguration _config { get; }
        public EmployeeController
            (
            StoredProcedureDBContext context, IConfiguration config
            )
        {
            _context = context;
            _config = config;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IEnumerable<Employee> SearchResult() {
            var result = _context.Employee
                .FromSqlRaw<Employee>("dbo.SearchEmployees")
                .ToList();

            return result;
        }

        public IActionResult DynamicSQL()
        {
            string connectionStr = _config.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "dbo.SearchEmployees";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                List<Employee> model = new List<Employee>();
                while (sdr.Read())
                {
                    var details = new Employee();
                    details.FirstName = sdr["FirstName"].ToString();
                    details.LastName = sdr["LastName"].ToString();
                    details.Gender = sdr["Gender"].ToString();
                    details.Salary = Convert.ToInt32(sdr["Salary"]);
                    model.Add(details);
                }
                return View(model);
            }
            
        }
    }
}
