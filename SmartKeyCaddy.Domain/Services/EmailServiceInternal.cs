
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Models;

namespace SmartKeyCaddy.Domain.Services;

public partial class EmailService
{
    private string GetEmailMessageBody(EmailTemplate emailTemplate, Reservation reservation, Property property, CommunicationType emailType)
    {
        var messageBody = emailTemplate.EmailBody;

        switch (emailType)
        {
            case CommunicationType.QrCode:
                messageBody = messageBody.Replace("[FirstName]", reservation.Guest?.FirstName);
                messageBody = messageBody.Replace("[PropertyName]", property.PropertyName);
                messageBody = messageBody.Replace("[QrCodeUrl]", $"{_qrCodeApiSettings.ImagePath}/{property.Chain.ChainCode}/{reservation.PmsReservationId}.{Constants.QrCodeFileExtension}");
                return messageBody;

            case CommunicationType.KeyPinCode:
                messageBody = messageBody.Replace("[FirstName]", reservation.Guest?.FirstName);
                messageBody = messageBody.Replace("[KeyPinCode]", reservation.KeyPinCode);
                return messageBody;

            case CommunicationType.OnlineChekin:
            case CommunicationType.OnlineChekinReminder:
                messageBody = messageBody.Replace("[FirstName]", reservation.Guest?.FirstName);
                messageBody = messageBody.Replace("[LastName]", reservation.Guest?.LastName);
                messageBody = messageBody.Replace("[ReservationUuid]", reservation.ReservationId.ToString());
                messageBody = messageBody.Replace("[CheckinDate]", reservation.CheckinDate.ToString("yyyy-MM-dd"));
                messageBody = messageBody.Replace("[PropertyName]", property.PropertyName);
                messageBody = messageBody.Replace("[PmsReservationId]", reservation.PmsReservationId);
                return messageBody;
            default: return messageBody;
        }
    }

    private string GetEmailMessageSubject(EmailTemplate emailTemplate, string pmsReservationNo)
    {
        return emailTemplate.EmailSubject.Replace("[ReservationNo]", pmsReservationNo);
    }
}
