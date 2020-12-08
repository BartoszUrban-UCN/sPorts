using System.Threading.Tasks;

namespace WebApplication.BusinessLogic
{
    public interface IPDFService<T>
    {
        Task CreatePDFFile(T fileToMakePDFFrom);

        void DeleteBookingFiles(int bookingReferenceNo);

    }
}
