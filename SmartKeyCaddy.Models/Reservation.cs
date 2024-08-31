namespace SmartKeyCaddy.Models;

//Change this to reservation
public class Reservation
{
    public Guid ReservationId { get; set; }
    public string PmsReservationId { get; set; }
    public Guid PropertyId { get; set; }
    public Guid ChainId { get; set; }
    public Guid GuestId { get; set; }
    public int AccountId { get; set; }
    public DateTime CheckinDate { get; set; }
    public DateTime CheckoutDate { get; set; }
    public string RoomId { get; set; }
    public string RoomName { get; set; }
    public string KeyPinCode { get; set; }
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; }
    public string Status { get; set; }
    public Guest Guest { get; set; }
}