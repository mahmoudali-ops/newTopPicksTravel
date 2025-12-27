using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.DTOs.Email;
using TourSite.Core.Servicies.Contract;

namespace TourSite.Service.Services.Emails
{
    using System.Net;
    using System.Net.Mail;

    public class EmailSender : IEmailSender_
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendToAdminAsync(EmailsDto e)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:From"]),
                Subject = "🛎️ New Booking Request",
                IsBodyHtml = true
            };

            // To (Admin)
            mail.To.Add(_config["EmailSettings:AdminEmail"]);

            // Optional: Reply-To
            mail.ReplyToList.Add(new MailAddress("info@toppickstravels.com"));

            mail.Body = $@"
            <h2>New Booking Received</h2>
            <p><b>Name:</b> {e.FullName}</p>
            <p><b>Email:</b> {e.EmailAddress}</p>
            <p><b>Tour:</b> {e.FullTourName}</p>
            <p><b>Hotel:</b> {e.HotelName}</p>
            <p><b>Room:</b> {e.RoomNumber}</p>
            <p><b>Adults:</b> {e.AdultNumber}</p>
            <p><b>Children:</b> {e.ChildernNumber}</p>
            <p><b>Date:</b> {e.BookingDate:yyyy-MM-dd}</p>
            <p><b>Message:</b><br/>{e.Message}</p>
        ";

            using var smtp = new SmtpClient
            {
                Host = _config["EmailSettings:SmtpServer"],
                Port = int.Parse(_config["EmailSettings:Port"]),
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(
                    _config["EmailSettings:Username"],
                    _config["EmailSettings:Password"]
                )
            };

            await smtp.SendMailAsync(mail);
        }
    }
}
