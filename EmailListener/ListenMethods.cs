using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace EmailListener
{
    class ListenMethods
    {
        AcDataBase db = new AcDataBase();
        SMTP smtp = new SMTP();

        public Rectangle TopBar { get; set; }

        public void UpdateColour(Brush Bru)
        {
            TopBar.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                TopBar.Fill = Bru;
            }));
        }

        public bool Search()
        {
            KeyValuePair<bool, double> Line = new KeyValuePair<bool, double>();
            DataTable dt = db.get("select * from [EmailTasks]");
            foreach(DataRow row in dt.Rows)
            {
                if (row.Field<bool>("SendE"))
                    if ((Line = ConditionMet(row)).Key)
                    {
                        UpdateColour(Brushes.Orange);
                        SendEmail(row, Line);
                        UpdateColour(Brushes.Green);
                    }
            }
            return true;
        }

        private KeyValuePair<bool,double> ConditionMet(DataRow row)
        {
            List<int> List = new List<int>();
            DataTable dt = db.get("Select [Status] from MasterData where [Task] = '" + row.Field<string>("Task") + "'");
            foreach (DataRow row2 in dt.Rows)
                List.Add(row2.Field<int>("Status"));
            double PerComplete = ((double)List.Count(o => o == 3) / List.Count)*100;
            Dictionary<double, bool> EndLi = IsCond(row.Field<string>("Conditions"), row.Field<string>("ComCon"));
            foreach(KeyValuePair<double, bool> item in EndLi)
                if(!item.Value)
                    if(PerComplete >= item.Key)
                        return new KeyValuePair<bool, double>(true, item.Key);

            //List<double> Cond = Conditions(row.Field<string>("Conditions"));
            //List<double> comCon = Conditions(row.Field<string>("ComCon"));
            return new KeyValuePair<bool, double>(false,0);
        }


        private Dictionary<double, bool> IsCond(string Cons, string Comp)
        {
            Dictionary<double, bool> EndLi = (Cons.Split(",".ToCharArray())).Select(o => double.Parse(o)).ToDictionary(o => o, o => false);
            foreach (double item in (Comp.Split(",".ToCharArray())).Select(o => double.Parse(o)).ToList())
                if(EndLi.ContainsKey(item))
                    EndLi[item] = true;
            return EndLi.OrderBy(o => o.Key).ToDictionary((keyItem) => keyItem.Key, (valueitem) => valueitem.Value);
        }

        //private List<double> Conditions(string item)
        //{
        //    return (item.Split(",".ToCharArray())).Select(o=> double.Parse(o)).ToList();
        //}


        private bool SendEmail(DataRow row, KeyValuePair<bool, double> Line)
        {
            string BodyHTML = "";
            if (row.Field<string>("EmailType") == "CC")
                BodyHTML = CCBodyReplace(ConfCheckHTML(row, Line), row);

            if (smtp.Email(EmailTo(row), BodyHTML))
            {
                if (row.Field<string>("EmailType") == "CC")
                    db.sendin(new List<string>() { "1", DateTime.Today.AddDays(1).AddHours(7).ToString("yyyy/MM/dd hh:mm:ss"), row.Field<string>("Task") }, "Update MasterData set [Status] = @1, [RunDT] = @2 where [Task] = @3");
                else
                    db.sendin(new List<string>() { row.Field<string>("ComCon") + "," + Line.Value.ToString() }, "Update EmailTasks Set [ComCon] = @1 Where ID = " + row.Field<int>("ID"));
            }
            return true;
        }

        private string ConfCheckHTML(DataRow row, KeyValuePair<bool, double> Line)
        {
            using (StreamReader reader = new StreamReader((@"A:\EmailTest\ConfidentCheck.html")))
                return reader.ReadToEnd();
            //using (StreamReader reader = new StreamReader((@"C:\Users\HARRIMA\Documents\Visual Studio 2017\Projects\EmailListener\EmailListener\ConfidentCheck.html")))
            //    return reader.ReadToEnd();
        }

        private List<string> EmailTo(DataRow row)
        {
            List<string> EndList = new List<string>();
            Dictionary<int, string> Users = new Dictionary<int, string>();
            DataTable dt = db.get("Select * from EmailAddress");
            foreach (DataRow row2 in dt.Rows)
                Users.Add(row2.Field<int>("ID"), row2.Field<string>("Email"));
            if (row.Field<string>("Email_IDs") != "")
            {
                var List1 = (row.Field<string>("Email_IDs").Split(",".ToCharArray())).Select(o => int.Parse(o));
                foreach (int item in List1)
                    if(Users.ContainsKey(item))
                        EndList.Add(Users[item]);
            }
            if (row.Field<string>("Group_IDs") != "")
            {
                var List1 = (row.Field<string>("Group_IDs").Split(",".ToCharArray())).Select(o => int.Parse(o));
                foreach (int item in List1)
                    if (Users.ContainsKey(item))
                        EndList.Add(Users[item]);
            }
            return EndList;
        }

        public string CCBodyReplace(string Body, DataRow row)
        {
            List<KeyValuePair<string, string>> ObjectList = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("001ConCheck-GTX_1ManDC2H", "'>%GT1%"), new KeyValuePair<string, string>("003ConCheck-WCS_1ManDC2H", "'>%WC1%"), new KeyValuePair<string, string>("002ConCheck-RJ_1ManDC2H", "'>%ST1%"), new KeyValuePair<string,string>("006ConCheck-GTX_2Man", "'>%GT2%"), new KeyValuePair<string, string>("005ConCheck-WCS_2Man", "'>%WC2%"), new KeyValuePair<string, string>("004ConCheck-RJ_2Man", "'>%ST2%"), new KeyValuePair<string, string>("010ConCheck-GTX_1ManH2H", "'>%GTH%"), new KeyValuePair<string, string>("012ConCheck-WCS_1ManH2H", "'>%WCH%"), new KeyValuePair<string, string>("011ConCheck-RJ_1ManH2H", "'>%STH%"), new KeyValuePair<string, string>("013ConCheck-WCS_PrePay", "'>%WCC%") };
            DataTable dt = db.get("select * from Results where [Task] = '" + row.Field<string>("Task") + "' order by ID DESC");
            foreach (KeyValuePair<string, string> Item in ObjectList)
            {
                DataRow NewRow = dt.AsEnumerable().Where(o => o.Field<string>("Test_ID") == Item.Key).Select(o => o).FirstOrDefault();
                if (NewRow.Field<string>("Result") == "Fail")
                    if(File.Exists(@"<a href=""file:///\\actfile1\data\Development\2) Development Testing Programme\9) Automation\UFT Automation\Results\" + Item.Key + @".bmp"))
                        Body = Body.Replace(Item.Value, ";color:red'><b>" + NewRow.Field<string>("Result") + @"</b><br /></span></b><span style='font-family:""Argos Book""'>" + @"<a href=""file:///\\actfile1\data\Development\2) Development Testing Programme\9) Automation\UFT Automation\Results\" + Item.Key + @".bmp""> Click Here </ a > ");
                    else
                        Body = Body.Replace(Item.Value, ";color:red'><b>" + NewRow.Field<string>("Result") + @"</b><br /></span></b><span style='font-family:""Argos Book""'>" + @"<a href=""file:///\\actfile1\data\Development\2) Development Testing Programme\9) Automation\UFT Automation\Results\" + Item.Key + @".png""> Click Here </ a > ");
                else if(NewRow.Field<string>("Order_Number") != "")
                    Body = Body.Replace(Item.Value, ";color:green'><b>" + NewRow.Field<string>("Result") + @"</b><br /></span></b><span style='font-family:""Argos Book""'>" + NewRow.Field<string>("Order_Number"));
                else if (NewRow.Field<string>("Order_Number1") != "")
                    Body = Body.Replace(Item.Value, ";color:green'><b>" + NewRow.Field<string>("Result") + @"</b><br /></span></b><span style='font-family:""Argos Book""'>" + NewRow.Field<string>("Order_Number1"));
                else if (NewRow.Field<string>("Order_Number2") != "")
                    Body = Body.Replace(Item.Value, ";color:green'><b>" + NewRow.Field<string>("Result") + @"</b><br /></span></b><span style='font-family:""Argos Book""'>" + NewRow.Field<string>("Order_Number2"));
                else
                    Body = Body.Replace(Item.Value, NewRow.Field<string>("Result"));
            }

            return Body;
        }
    }
}
