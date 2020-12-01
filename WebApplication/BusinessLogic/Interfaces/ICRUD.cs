using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication.BusinessLogic
{
    public interface ICRUD<T>
    {
        Task<int> Create(T objectToCreate);
        Task<T> GetSingle(int? id);
        Task<IEnumerable<T>> GetAll();
        T Update(T objectToUpdate);
        Task Delete(int? id);
        Task<bool> Exists(int? id);
        Task<int> Save();
    }
}
