using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class API_Transfer
    {
        //public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
        public decimal Amount { get; set; }
        public int AccountFrom { get; set; }


    }

    public class Transfer
    {
        public int TransferId { get; set; }
        public string TransferType { get; set; }
        public string TransferStatus { get; set; }
        public string AccountFromUserName { get; set; }
        public string AccountToUserName { get; set; }
        public decimal Amount { get; set; }
    }

    public class TransferNumber
    {
        public int TransferId { get; set; }
    }
}
