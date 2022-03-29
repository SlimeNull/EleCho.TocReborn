using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace NullLib.TocReborn.Pro.Helper
{
    internal static class VerifyCodeHelper
    {
        static readonly MailAddress MailSender = new MailAddress("slimenull@outlook");
        public static void SendVerificationCode(string mailto, string account, string code)
        {
            MailMessage mail = new MailMessage();
            mail.From = MailSender;
            mail.To.Add(mailto);
            mail.Subject = "TOC PRO Verification Code";
            mail.Body = $"You Verification code is {code} for account {account}, it's valid in 5 minutes";

            SmtpClient smtp = new SmtpClient();
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(MailSender.Address, "ms548426.");
            smtp.Host = "outlook.com";
            smtp.Send(mail);
        }
    }
}
