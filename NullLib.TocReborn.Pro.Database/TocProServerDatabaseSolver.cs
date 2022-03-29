using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Net.Mail;

namespace NullLib.TocReborn.Pro
{
    public class TocProServerDatabaseSolver : ITocProServerDataManager
    {
        public string MailAccount { get; set; }
        public string MailPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        public TocProServerDatabaseSolver(IDbConnection conn)
        {
            Conn = conn;
        }

        public IDbConnection Conn { get; }

        public ICollection<TocProUser> Users => throw new NotImplementedException();

        public ICollection<TocProChannel> Channels => throw new NotImplementedException();

        public bool ContainsAccount(string account)
        {
            IDbCommand dbCommand = Conn.CreateCommand();
            dbCommand.CommandText = "SELECT * FROM accounts FULL OUTER JOIN channels ON accounts.account = channels.account";
            return dbCommand.ExecuteNonQuery() > 0;
        }

        public void SendVerificationCode(string mailto, string account, string code)
        {
            MailAddress sender = new MailAddress(MailAccount);

            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = "TOC Pro Register";
            mailMessage.Body = $"Your TOC Pro verification code is '{code}', for account '{account}'";
            mailMessage.From = sender;
            mailMessage.To.Add(mailto);

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Credentials = new NetworkCredential(MailAccount, MailPassword);
            smtpClient.Host = "outlook.com";
            smtpClient.EnableSsl = true;

            smtpClient.Send(mailMessage);
        }
    }
}
