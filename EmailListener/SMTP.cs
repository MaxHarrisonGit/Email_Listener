using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmailListener
{
    class SMTP
    {
        public bool Email(List<string>Who,string Body)
        {
            MailMessage msg = new MailMessage();
            foreach(string person in Who)
                msg.To.Add(new MailAddress(person));
            msg.From = new MailAddress("ODTTAutomation@homeretailgroup.com", "ODTT Automation");
            msg.Subject = "This is a Automation Email";
            msg.Body = Body;
            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();

            client.Port = 25;
            client.Host = "";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                client.Send(msg);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }
    }
}
