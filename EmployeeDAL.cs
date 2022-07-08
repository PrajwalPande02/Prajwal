using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// comment by onkar
using System.Data;
using MySql.Data.MySqlClient;
using BOL;

namespace DAL
{
	//Connected Data Access
	//Connection is kept open till operation get completed.
	//Explicitly connectin object has been managed with opening and closing by code
	//comd.ExecuteScalar :::::::::::::for Aggrdate functions with SELECT
	//comd.ExecuteNonQuery:::::::::::::for DML  (insert, update or delete command)
	//comd.ExecuteReader::::::::::::::::for SQL (Select command)
// sumit patil
	//Helper class
	public class EmployeeDAL
	{
		public static string conenctionString = @"server=localhost;user=root;database=Knowitdb;password='mr.cricket02'";
		public static List<Employee> GetAll()
		{
			List<Employee> employees = new List<Employee>();
			IDbConnection conn = new MySqlConnection(conenctionString);
			try
			{
				conn.Open();
				string query = "SELECT * From employee";
				IDbCommand cmd = new MySqlCommand(query, conn as MySqlConnection);
				cmd.CommandType = CommandType.Text;   // 
				IDataReader reader = cmd.ExecuteReader();  //ResultSet
				while (reader.Read())
				{
					Employee emp = new Employee();
					emp.ID = int.Parse(reader["ID"].ToString());  //int.Parse, float.Parse, double.Parse
					emp.Name = reader["Name"].ToString();
					emp.Designation = reader["Designation"].ToString();
					emp.Salary = double.Parse(reader["Salary"].ToString());
					employees.Add(emp);
				}
				reader.Close();
			}
			catch (MySqlException excpetion)
			{
				string error = excpetion.Message;
			}
			finally
			{
				conn.Close();
			}
			return employees;
		}
		public static BOL.Employee GetByID(int id)
		{
			BOL.Employee emp = new BOL.Employee();
			List<Employee> employees = new List<Employee>();
			IDbConnection conn = new MySqlConnection(conenctionString);
			try
			{
				conn.Open();
				string query = "SELECT * From employee where ID=" + id;
				IDbCommand cmd = new MySqlCommand(query, conn as MySqlConnection);
				cmd.CommandType = CommandType.Text;   // 
				IDataReader reader = cmd.ExecuteReader();  //ResultSet

				if (reader.Read())
				{
					emp.ID = int.Parse(reader["ID"].ToString());  //int.Parse, float.Parse, double.Parse
					emp.Name = reader["Name"].ToString();
					emp.Designation = reader["Designation"].ToString();
					emp.Salary = double.Parse(reader["Salary"].ToString());
				}
				reader.Close();
			}
			catch (MySqlException excpetion)
			{
				string error = excpetion.Message;
			}
			finally
			{
				conn.Close();
			}
			return emp;
		}
		public static bool Delete(int id)
		{
			bool status = false;
			IDbConnection conn = new MySqlConnection(conenctionString);
			try
			{
				conn.Open();
				string query = "DELETE From employee where ID=" + id;
				IDbCommand cmd = new MySqlCommand(query, conn as MySqlConnection);
				cmd.CommandType = CommandType.Text;   // 
				int rowsAffected = cmd.ExecuteNonQuery(); // for DML Operations
				if (rowsAffected > 0)
				{
					status = true;
				}
			}
			catch (MySqlException excpetion)
			{
				string error = excpetion.Message;
			}
			finally
			{
				conn.Close();
			}
			return status;
		}
		public static bool Insert(Employee emp)
		{
			bool status = false;
			IDbConnection con = new MySqlConnection(conenctionString);
			try
			{
				con.Open();
				MySqlCommand cmd = new MySqlCommand();
				cmd.CommandType = CommandType.Text;
				string query = "INSERT INTO employee (ID,Name, Designation, Salary) values(" +
								emp.ID + ",'" + emp.Name + "','" + emp.Designation + "'," + emp.Salary + ")";
				int rowsAffected = cmd.ExecuteNonQuery();
				if (rowsAffected > 0)
				{
					status = true;
				}
			}
			catch (Exception ex)
			{
				string exeMessage = ex.Message;
			}
			finally
			{
				con.Close();
			}
			return status;
		}
		public static bool InsertUsingParameters(Employee emp)
		{
			bool status = false;
			IDbConnection con = new MySqlConnection(conenctionString);
			try
			{
				con.Open();
				MySqlCommand cmd = new MySqlCommand();
				cmd.CommandType = CommandType.Text;
				//To avoid SQL Injection , we use parameterized Query using Parameters collection with command object
				string query = "INSERT INTO employee (ID,Name, Designation, Salary)  values(@id, @empName, @empDesignation,@empSalary)";
				cmd.CommandText = query;
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@id", emp.ID);
				cmd.Parameters.AddWithValue("@empName", emp.Name);
				cmd.Parameters.AddWithValue("@empDesignation", emp.Designation);
				cmd.Parameters.AddWithValue("@empSalary", emp.Salary);
				int rowsAffected = cmd.ExecuteNonQuery();
				if (rowsAffected > 0)
				{
					status = true;
				}
			}
			catch (Exception ex)
			{
				string exeMessage = ex.Message;
			}
			finally
			{
				con.Close();
			}
			return status;
		}
		public static bool Update(Employee emp)
		{
			bool status = false;
			MySqlConnection con = new MySqlConnection(conenctionString);
			try
			{
				MySqlCommand cmd = new MySqlCommand();
				cmd.Connection = con;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "UPDATE employee  SET Name='" + emp.Name + "',Designation='"
									+ emp.Designation + "', Salary=" + emp.Salary + " WHERE ID=" + emp.ID;
				con.Open();
				int rowsAffected = cmd.ExecuteNonQuery();
				if (rowsAffected > 0)
				{
					status = true;
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
			}
			finally
			{
				con.Close();
			}
			return status;
		}
		public static bool CreateStoredProcedureAndTable()
		{
			bool status = false;
			MySqlConnection con = new MySqlConnection(conenctionString);
			try
			{
				MySqlCommand cmd = new MySqlCommand();
				cmd.Connection = con;
				con.Open();

				cmd.CommandText = "DROP  PROCEDURE IF EXISTS add_people";
				cmd.ExecuteNonQuery();

				cmd.CommandText = "DROP  TABLE  IF  EXISTS people";
				cmd.ExecuteNonQuery();

				cmd.CommandText = "CREATE TABLE people ( " +
								  "id INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY, first_name VARCHAR(20)," +
								  "last_name VARCHAR(20), birthdate DATE)";
				cmd.ExecuteNonQuery();

				cmd.CommandText = "CREATE PROCEDURE add_people(" +
								  "IN fname VARCHAR(20), IN lname VARCHAR(20), IN bday DATETIME, OUT id INT)" +
								  "BEGIN INSERT INTO people(first_name, last_name, birthdate) " +
								  "VALUES(fname, lname, DATE(bday)); SET id = LAST_INSERT_ID(); END";

				cmd.ExecuteNonQuery();
				status = true;

			}
			catch (MySqlException exp)
			{

			}
			finally
			{
				if (con != null)
				{
					con.Close();
				}
			}
			return status;
		}
		public static bool InvokeStoredProcedure()
		{
			bool status = false;
			MySqlConnection con = new MySqlConnection(conenctionString);
			try
			{
				MySqlCommand cmd = new MySqlCommand();
				cmd.Connection = con;
				cmd.CommandText = "add_people";
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue("@fname", "Kamal");
				cmd.Parameters["@fname"].Direction = ParameterDirection.Input;

				cmd.Parameters.AddWithValue("@lname", "Hasan");
				cmd.Parameters["@lname"].Direction = ParameterDirection.Input;


				cmd.Parameters.AddWithValue("@bday", "1999-09-09");
				cmd.Parameters["@bday"].Direction = ParameterDirection.Input;

				cmd.Parameters.Add("@id", MySqlDbType.Int32);
				cmd.Parameters["@id"].Direction = ParameterDirection.Output;

				con.Open();
				cmd.ExecuteNonQuery();
				status = true;

			}
			catch (MySqlException exp)
			{
				string message = exp.Message;
			}
			finally
			{
				con.Close();
			}
			return status;
		}
	}
}
