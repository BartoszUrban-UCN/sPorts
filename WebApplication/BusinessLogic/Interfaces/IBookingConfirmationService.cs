using System.Threading.Tasks;

namespace WebApplication.BusinessLogic
{
    public interface IBookingConfirmationService
    {
        Task<bool> ConfirmSpotBooked(int bookingLineId);

        void SendConfirmationMail(int bookingId);
    }
}