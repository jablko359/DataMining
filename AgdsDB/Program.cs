using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgdsDB
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=IGOR-KOMPUTER;Initial Catalog=NORTHWND;Integrated Security=SSPI;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                DbDecompositor decompositor = new DbDecompositor(conn);
                AgdsBuilder builder = new AgdsBuilder(decompositor);
                try
                {
                    builder.BuildGraph();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
               
                //conn.Open();
                //using (SqlCommand command = conn.CreateCommand())
                //{
                //    command.CommandText = "Select * from Users";
                //    var reader = command.ExecuteReader();
                //    while (reader.Read())
                //    {
                //        Console.WriteLine();
                //    }
                //    var test = reader.GetSchemaTable();
                //}
            }
        }
    }
}
