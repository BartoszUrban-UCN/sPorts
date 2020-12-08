using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.BusinessLogic.Specifications
{
    public class CurrentUserSpecification : BaseSpecification<Marina>
    {
        public CurrentUserSpecification(int marinaOwnerId)
        {
            Criteria = i => i.MarinaOwner.PersonId == marinaOwnerId;
        }
    }
}
