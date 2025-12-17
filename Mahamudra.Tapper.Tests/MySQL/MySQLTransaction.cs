using Mahamudra.Tapper.Interfaces;
using System.Data;

namespace Mahamudra.Tapper.Tests.MySQL;
public class MySQLTransaction: ITransaction
{
    public MySQLTransaction()
    {
        IsolationLevel = IsolationLevel.ReadCommitted;
    }
} 