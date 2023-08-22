using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendWithBrevo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Utility;

public class EmailSender : IEmailSender
{

 //   public System.Threading.Tasks.Task SendEmailAsync(string email, string name, string subject)
	//{
 //       // logic to send email

 //       var apiInstance = new TransactionalEmailsApi();
 //       string SenderName = "Restraurant Store";
 //       string SenderEmail = "admin@reststore.com";
 //       SendSmtpEmailSender emailSender = new SendSmtpEmailSender(SenderName, SenderEmail);

 //       SendSmtpEmailTo emailReceiver = new SendSmtpEmailTo(email, name);
 //       List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>();
 //       To.Add(emailReceiver);
        
 //       string HtmlContent = null;
 //       string TextContent = subject;
        
 //       try
 //       {
 //           var sendSmtpEmail = new SendSmtpEmail(emailSender, To, null, null, HtmlContent, TextContent, Subject);
 //           CreateSmtpEmail result = apiInstance.SendTransacEmail(sendSmtpEmail);
 //           Console.WriteLine("Response:\n" + result.ToJson());
 //       }
 //       catch (Exception e)
 //       {
 //           Console.WriteLine(e.Message);
 //       }

 //   }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {//logic to send email
        return Task.CompletedTask;
    }
}
