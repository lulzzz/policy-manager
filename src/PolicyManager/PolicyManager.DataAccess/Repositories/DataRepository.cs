using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolicyManager.DataAccess.Repositories
{
    public interface IDataRepository<T>
    {
        Task<IEnumerable<T>> FetchAllAsync();
    }

    public class DataRepository<T>
        : IDataRepository<T>
    {
        public async Task<IEnumerable<T>> FetchAllAsync()
        {
            return await Task.FromResult(new List<T>().AsEnumerable());
        }
    }
}
