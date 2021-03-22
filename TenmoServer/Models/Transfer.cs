using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public string TransferType { get; set; }
        public string TransferStatus { get; set; }
        public string AccountFromUserName { get; set; }
        public string AccountToUserName { get; set; }
        public decimal Amount { get; set; }
    }

    public class CreateTransfer
    { 
        public int AccountTo { get; set; }
        public decimal Amount { get; set; }
        public int AccountFrom { get; set; }
    }

    public class RawTransferData
    {
        public int TransferId { get; set; }
        public int TransferTypeId { get; set; }
        public int TransferStatusId { get; set; }
        public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
        public decimal Amount { get; set; }
    }

    public class TransferNumber
    {
        public int TransferId { get; set; }
    }
}
