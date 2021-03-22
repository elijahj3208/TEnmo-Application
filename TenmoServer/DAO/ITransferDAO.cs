using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {

        bool ReduceBalance(decimal amount, int userId);
        bool IncreaseBalance(decimal amount, int userId);
        bool CreateTransfer(CreateTransfer createTransfer, int transferType, int status);
        List<Transfer> GetTransferForUser(int userId, int type);
        bool UpdateRequest(int transferId, int status);
        public RawTransferData GetTransferFromId(int transferId);
    }
}
