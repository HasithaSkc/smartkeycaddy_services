
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Models;

namespace HotelCheckIn.Domain.Contracts;

public interface IEmailService
{
    Task<bool> SendEmail(KeyAllocation keyAllocation, Reservation reservation, Property property, CommunicationType comunicationType);
}
