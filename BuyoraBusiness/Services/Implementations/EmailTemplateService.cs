using System.Net;
using BuyoraBusiness.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BuyoraBusiness.Services.Implementations
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly string _logoUrl;

        public EmailTemplateService(IConfiguration configuration)
        {
            // العنوان بييجي من الـ config (مش من Request.Host) عشان نمنع Host Header Injection
            var baseUrl = configuration["App:BaseUrl"]?.TrimEnd('/') ?? string.Empty;
            _logoUrl = $"{baseUrl}/Logo.png";
        }

        public string Build(
            string subject,
            string badge,
            string greetingName,
            string message,
            string buttonText,
            string link,
            string footerNote)
        {
            // أي نص ممكن يجي من مستخدم لازم يتعمل له encode قبل ما يدخل في الـ HTML
            var eSubject = WebUtility.HtmlEncode(subject);
            var eBadge = WebUtility.HtmlEncode(badge);
            var eName = WebUtility.HtmlEncode(greetingName);
            var eMessage = WebUtility.HtmlEncode(message);
            var eButton = WebUtility.HtmlEncode(buttonText);
            var eLink = WebUtility.HtmlEncode(link);
            var eFooterNote = WebUtility.HtmlEncode(footerNote);
            var eLogo = WebUtility.HtmlEncode(_logoUrl);
            var year = DateTime.UtcNow.Year;

            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>{eSubject}</title>
</head>
<body style='margin:0; padding:0; background-color:#F8FAFC; font-family:Segoe UI, Tahoma, Geneva, Verdana, sans-serif; color:#1E293B;'>
    <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0' style='background-color:#F8FAFC; padding:40px 15px;'>
        <tr>
            <td align='center'>
                <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0' style='max-width:600px; background-color:#ffffff; border-radius:20px; overflow:hidden; box-shadow:0 12px 32px rgba(7,31,78,0.08); border:1px solid #E2E8F0;'>

                    <tr>
                        <td align='center' style='background:linear-gradient(135deg, #071f4e 0%, #0038A8 100%); padding:35px 25px; border-bottom:4px solid #FF6B00;'>
                            <img src='{eLogo}' alt='Buyora' width='72' height='72' style='display:block; border-radius:14px; margin-bottom:10px; object-fit:contain;' />
                            <h1 style='margin:0; color:#ffffff; font-size:28px; font-weight:800; letter-spacing:-0.5px;'>Buyora</h1>
                            <p style='margin:5px 0 0 0; color:#FFA800; font-size:13px; font-weight:600; letter-spacing:0.5px;'>Shop Smarter. Live Better.</p>
                        </td>
                    </tr>

                    <tr>
                        <td style='padding:40px 35px 30px 35px;'>
                            <span style='display:inline-block; background-color:#FFF7ED; color:#FF6B00; font-size:12px; font-weight:700; padding:6px 14px; border-radius:50px; border:1px solid #FFEDD5; margin-bottom:16px;'>
                                {eBadge}
                            </span>

                            <h2 style='margin:0 0 16px 0; color:#071f4e; font-size:24px; font-weight:800;'>
                                {eSubject}
                            </h2>

                            <p style='margin:0 0 14px 0; font-size:16px; line-height:1.6; color:#334155;'>
                                Hello <strong style='color:#0038A8;'>{eName}</strong>,
                            </p>

                            <p style='margin:0 0 28px 0; font-size:15px; line-height:1.7; color:#64748B;'>
                                {eMessage}
                            </p>

                            <table role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin:30px 0;'>
                                <tr>
                                    <td align='center'>
                                        <a href='{eLink}' target='_blank' style='display:inline-block; background:linear-gradient(135deg, #FF6B00 0%, #FFA800 100%); color:#ffffff; text-decoration:none; font-size:16px; font-weight:700; padding:15px 38px; border-radius:12px; box-shadow:0 8px 20px rgba(255,107,0,0.3);'>
                                            {eButton}
                                        </a>
                                    </td>
                                </tr>
                            </table>

                            <div style='background-color:#F8FAFC; border:1px solid #E2E8F0; border-radius:12px; padding:16px; margin-top:25px;'>
                                <p style='margin:0 0 8px 0; font-size:12px; font-weight:700; color:#475569;'>
                                    If the button above doesn't work, copy and paste this link into your browser:
                                </p>
                                <p style='margin:0; font-size:12px; word-break:break-all; color:#0038A8;'>
                                    <a href='{eLink}' style='color:#0038A8; text-decoration:underline;'>{eLink}</a>
                                </p>
                            </div>

                            <p style='margin:25px 0 0 0; font-size:13px; color:#94A3B8; line-height:1.5;'>
                                {eFooterNote}
                            </p>
                        </td>
                    </tr>

                    <tr>
                        <td align='center' style='background-color:#071f4e; padding:22px 25px; color:#94A3B8; font-size:12px;'>
                            <p style='margin:0 0 6px 0; color:#ffffff; font-weight:700;'>Buyora E-Commerce</p>
                            <p style='margin:0;'>&copy; {year} Buyora - By <span style='color:#FFA800; font-weight:600;'>Abdelrahman Mohamed</span>. All rights reserved.</p>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}