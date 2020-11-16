namespace WebApplication.BusinessLogic
{
    internal interface IPDFService<T>
    {
        void CreatePDFFile(T fileToMakePDFFrom);

        void DeleteBookingFiles(int bookingReferenceNo);

    }
}
