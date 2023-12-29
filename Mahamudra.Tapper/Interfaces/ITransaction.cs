using System.Data;

namespace Mahamudra.Tapper.Interfaces
{
    public abstract class ITransaction
    {   
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;
    }
}