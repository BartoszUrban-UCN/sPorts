namespace WebApplication.BusinessLogic
{
    public interface IPDFService<T>
    {
        void CreatePDFFile(T fileToMakePDFFrom);

        void DeleteBookingFiles(int bookingReferenceNo);

    }
}
