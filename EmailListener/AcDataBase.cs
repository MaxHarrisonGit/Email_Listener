using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
namespace EmailListener
{
    class AcDataBase
    {
        private OleDbConnection db;

        public AcDataBase()
        {
            //db = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\Users\\Administrator\\Desktop\\EndProject\\EndDatabase.mdb");


            //var item = System.IO.Directory.GetParent("X:\\Development\\2) Development Testing Programme\\9) Automation\\UFT Automation\\Database\\Automation Database1.accdb");
            db = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=X:\\Development\\2) Development Testing Programme\\9) Automation\\UFT Automation\\Database\\Automation Database2.mdb");
            db.Open();
        }

        public bool sendin(List<string> Value, string com)
        {
            var command = new OleDbCommand(com, db);
            int I = 1;
            foreach (string val in Value)
            {
                command.Parameters.AddWithValue("@" + I, val);
                I++;
            }
            return (command.ExecuteNonQuery() > 0);
        }


        public bool sendinNull(List<DBNull> Value, string com)
        {
            var command = new OleDbCommand(com, db);
            int I = 1;
            foreach (DBNull val in Value)
            {
                command.Parameters.AddWithValue("@" + I, val);
                I++;
            }
            return (command.ExecuteNonQuery() > 0);
        }

        public DataTable get(string com)
        {
            var command = new OleDbDataAdapter(com, db);
            DataTable dt = new DataTable();
            command.Fill(dt);
            return dt;
        }

        public void MultiInsert(List<DataSend> DataList)
        {
            OleDbCommand command = new OleDbCommand();
            OleDbTransaction transaction = null;
            command.Connection = db;
            transaction = db.BeginTransaction(IsolationLevel.ReadCommitted);
            command.Connection = db;
            command.Transaction = transaction;

            foreach (DataSend data in DataList)
            {
                command.CommandText = data.com;
                int I = 1;
                foreach (string val in data.values)
                {
                    command.Parameters.AddWithValue("@" + I, val);
                    I++;
                }
                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }
    }

    

    public class DataSend
    {
        public string com { get; set; }
        public List<string> values { get; set; }
    }
}
