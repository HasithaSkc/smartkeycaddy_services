namespace SmartKeyCaddy.Models;

public class EmailTemplate
{
    public Guid EmailTemplateId { get; set; }
    public string EmailTemplateCode { get; set; }
    public string EmailTemplateDescription { get; set; }
    public string EmailSubject { get; set; }
    public string SenderEmailAddress { get; set; }
    public string EmailBody { get; set; }
    public string EmailSignatureImagePath { get; set; }
    public bool IsActive { get; set; }
    public Guid PropertyId { get; set; }
    public DateTime CreatedDate { get; set; }
}