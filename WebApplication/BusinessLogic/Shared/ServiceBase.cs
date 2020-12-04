using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.BusinessLogic.Shared;

namespace WebApplication.BusinessLogic
{
    public class ServiceBase
    {
        protected readonly SportsContext _context;

        public ServiceBase(SportsContext context)
        {
            context.ThrowIfNull();

            _context = context;
        }

        public async Task<int> Save()
        {
            try
            {
                var result = await _context.SaveChangesAsync();
                if (result < 1)
                {
                    throw new BusinessException(string.Empty, "There were some problems saving changes to the database");
                }
                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new BusinessException(string.Empty, "Database problems, couldn't save changes.\n" + ex.ToString());
            }
            catch (DbUpdateException ex)
            {
                throw new BusinessException(string.Empty, "Concurrency problems, couldn't save change.\n" + ex.ToString());
            }
        }
    }
}
