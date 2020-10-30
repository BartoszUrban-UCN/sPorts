using Newtonsoft.Json.Bson;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public class Marina
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Facilities { get; set; }

        public MarinaOwner MarinaOwner { get; set; }

        public Address Address { get; set; }

        public List<Review> Reviews { get; set; }

        public List<Spot> Spots { get; set; }

        public void AddSpot(Spot s)
        {
            if(Spots == null)
            {
                Spots = new List<Spot>();
                Spots.Add(s);
            }
            else
            {
                Spots.Add(s);
            }
        }
        public void AddReview(Review r)
        {
            if (Reviews == null)
            {
                Reviews = new List<Review>();
                Reviews.Add(r);
            }
            else
            {
                Reviews.Add(r);
            }
        }
    }
}