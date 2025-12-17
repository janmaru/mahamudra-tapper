using System.Data;

namespace Mahamudra.Tapper.Interfaces
{
    public abstract class ITransaction
    {
        // Volatile data can be read but not modified during the transaction.
        // New data can be added during the transaction.
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.RepeatableRead;
    }
}