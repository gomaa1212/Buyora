namespace BuyoraBusiness.Services.Interfaces
{
    public interface IEmailTemplateService
    {
        /// <summary>
        /// يبني إيميل HTML بهوية Buyora. كل النصوص بتتعمل لها HTML-encode جوه الدالة.
        /// </summary>
        /// <param name="subject">عنوان الإيميل (بيظهر كعنوان كبير)</param>
        /// <param name="badge">الشارة الصغيرة فوق العنوان (مثلًا: Account Verification)</param>
        /// <param name="greetingName">اسم المستخدم في التحية</param>
        /// <param name="message">نص الرسالة الأساسي</param>
        /// <param name="buttonText">نص الزرار</param>
        /// <param name="link">الرابط الكامل (Absolute URL)</param>
        /// <param name="footerNote">الملاحظة الصغيرة تحت الرابط</param>
        string Build(
            string subject,
            string badge,
            string greetingName,
            string message,
            string buttonText,
            string link,
            string footerNote);
    }
}