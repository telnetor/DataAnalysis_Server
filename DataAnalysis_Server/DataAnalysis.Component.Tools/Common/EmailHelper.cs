using DataAnalysis.Component.Tools.Log;
using DataAnalysis.Component.Tools.Settings;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Component.Tools.Common
{
    public class EmailHelper
    {
        private static string RECIPIENT = string.Empty;
        private static string Mail_USER = string.Empty;
        private static string Mail_PWD = string.Empty;
        private static string SMTP = string.Empty;
        static EmailHelper()
        {
            RECIPIENT = AppSetting.GetConnection("Email", "Recipient");
            Mail_USER = AppSetting.GetConnection("Email", "MailUser");
            Mail_PWD = AppSetting.GetConnection("Email", "MailPwd");
            SMTP = AppSetting.GetConnection("Email", "Smtp");
        }


        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="message">内容</param>
        public static void SendEmail(string title, string content)
        {
            try
            {
                MimeMessage message = new MimeMessage();
                //发送人
                message.From.Add(new MailboxAddress("SmallHan", Mail_USER));
                string[] array = RECIPIENT.Split(',');
                if (array.Length > 0)
                {
                    foreach (var it in array)
                    {
                        message.To.Add(new MailboxAddress(it));
                    }
                    //标题
                    message.Subject = title;
                    TextPart html = new TextPart(TextFormat.Text)
                    {
                        Text = content
                    };
                    var alternative = new Multipart("alternative");
                    alternative.Add(html);

                    var multipart = new Multipart("mixed");
                    multipart.Add(alternative);
                    message.Body = multipart;
                    using (SmtpClient client = new SmtpClient())
                    {
                        client.Connect(SMTP, 465, true);
                        client.Authenticate(Mail_USER, Mail_PWD);
                        client.Send(message);
                        client.Disconnect(true);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.Error.Debug($"发送邮箱报错:{ex}");
            }
        }
    }
}
