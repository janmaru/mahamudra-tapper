using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Mahamudra.Tapper.Interfaces
{ 
    public interface ICommand<T>
    {
        Task<T> Execute(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string schema = null);
    }
}