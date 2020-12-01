namespace WebApplication.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public byte Stars { get; set; }
        public string Comment { get; set; }

        public int MarinaId { get; set; }
        public Marina Marina { get; set; }
    }
}
