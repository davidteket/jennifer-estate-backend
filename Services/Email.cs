using System;
using System.Net;
using System.Net.Mail;

namespace backend.Services
{
    public class Email
    {
        private string _smtpAddress;
        private int _smtpPort;
        private bool _enableSSL;
        private string _senderAddress; 
        private string _senderPassword;
        private Serializer _serializer;

        public Email()
        {
            _serializer = new Serializer();
            _smtpAddress = _serializer.GetEmailServiceConfiguration("Host");
            _smtpPort = int.Parse(_serializer.GetEmailServiceConfiguration("Port"));
            _enableSSL = bool.Parse(_serializer.GetEmailServiceConfiguration("EnableSSL"));
            _senderAddress = _serializer.GetEmailServiceConfiguration("Sender");
            _senderPassword = _serializer.GetEmailServiceConfiguration("Password");
        }

        // Értesítés regisztrációról.
        //
        public bool NotifyRegistration(string to, string name, string username, string tmpPass, string role)
        {
            bool result = false;
            
            string companyName = _serializer.GetApplication("Name");
            string url = _serializer.GetApplication("URL");
            string subject = _serializer.GetEmail("Registration", "Subject", companyName, url);
            string body = _serializer.GetEmail("Registration", "Body", name, username, tmpPass, role, url, companyName);

            using (MailMessage email = new MailMessage()) 
            {  
                email.From = new MailAddress(_senderAddress);  
                email.To.Add(to);  
                email.Subject = subject;  
                email.Body = body;  
                email.IsBodyHtml = true;   

                using(SmtpClient smtp = new SmtpClient(_smtpAddress, _smtpPort)) 
                {  
                    smtp.Credentials = new NetworkCredential(_senderAddress, _senderPassword);  
                    smtp.EnableSsl = _enableSSL;  
                    try {
                        smtp.Send(email);
                    } catch (Exception exception) 
                    {
                        Console.WriteLine(exception.Message);
                        result = false;
                    }
                }
            }

            if (result != false)
                result = true;

            return result;            
        }
    }
}