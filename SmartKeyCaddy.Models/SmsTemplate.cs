namespace SmartKeyCaddy.Models;

public class SmsTemplate
{
    public Guid SmsTemplateUuid { get; set; }
    public string SmsTemplateCode { get; set; }
    public string SenderName { get; set; }
    public string SmsTemplateDescription { get; set; }
    public string SmsBody { get; set; }
    public string SmsSignatureImagePath { get; set; }
    public bool IsActive { get; set; }
    public string PropertyId { get; set; }
    public DateTime CreatedDate { get; set; }
}