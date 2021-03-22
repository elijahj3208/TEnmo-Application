using TenmoServer.DAO;
using TenmoServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TransferController : ControllerBase
    {
        private readonly IAccountDAO accountDAO;
        private readonly IUserDAO userDAO;
        private readonly ITransferDAO transferDAO;

        public TransferController(IAccountDAO _accountDAO, IUserDAO _userDAO, ITransferDAO _transferDAO)
        {
            accountDAO = _accountDAO;
            userDAO = _userDAO;
            transferDAO = _transferDAO;
        }

        //wires get request at endpoint /transfer/users to return list of users
        //minus name of current person logged in
        [HttpGet("users")]
        public List<GetUser> ListUsers()
        {
            //gets username of person currently logged in
            string username = User.Identity.Name;
            //returns list of users without the person currently logged in
            return userDAO.GetUserList(username);
        }

        //wires post request at transfer endpoint to create new send
        [HttpPost]
        public ActionResult CreateTransfer(CreateTransfer transfer)
        {
            //get's current user's account
            Account userAccount = accountDAO.GetAccountFromUserId(transfer.AccountFrom);
            //get's current user's balance
            decimal accountBalance = userAccount.Balance;
            //check's to see if person is trying to send money to themselves
            if(transfer.AccountFrom == transfer.AccountTo)
            {
                return BadRequest("Invalid recipient. Cannot send money to yourself.");
            }
            //checks to see if transfer amount is less than money in user's account
            else if (transfer.Amount <= accountBalance)
            {
                //reduces balance of person sending money(user)
                bool reduceSuccess = transferDAO.ReduceBalance(transfer.Amount, transfer.AccountFrom);
                //checks to make sure that there wasn't server communication problems when executing
                if (!reduceSuccess)
                {
                    return StatusCode(500, "Unable to withdraw funds / server issue.");
                }
                //increases balance of recipient's account
                bool increaseSuccess = transferDAO.IncreaseBalance(transfer.Amount, transfer.AccountTo);
                //checks to make sure that there wasn't server communication problems when executing
                if (!increaseSuccess)
                {
                    return StatusCode(500, "Unable to add funds / server issue.");
                
                }
                //creates log of transfer in transfers table
                bool createTransferSuccess = transferDAO.CreateTransfer(transfer, 2, 2);
                //checks to make sure that there wasn't server communication problems when executing
                if (!createTransferSuccess)
                {
                    return StatusCode(500, "Unable to record transaction / server issue.");
                }
                //if reduce, increase and transfer are successful, return success message to client
                return Ok("Transfer successful.");
            }
            else 
            {
                //alerts client that user doesn't have enough money to send
                return BadRequest("Insufficient funds.");
            
            }   
        }

        //wires get request at transfer endpoint to return transfer list for user
        [HttpGet]
        public ActionResult<List<Transfer>> GetTransferList()
        {
            User user = userDAO.GetUser(User.Identity.Name);
            List<Transfer> transferList = transferDAO.GetTransferForUser(user.UserId, 1);
            return Ok(transferList);
        }

        [HttpPost("request")]
        public ActionResult MakeRequest(CreateTransfer transfer)
        {
            if (transfer.AccountFrom == transfer.AccountTo)
            {
                return BadRequest("Invalid recipient. Cannot request money from yourself.");
            }
            else
            {
                bool createTransferSuccess = transferDAO.CreateTransfer(transfer, 1, 1);
                if (!createTransferSuccess)
                {
                    return StatusCode(500, "Unable to record request / server issue.");
                }

                return Ok("Request is pending.");
            }
        }

        [HttpGet("pending")]
        public ActionResult<List<Transfer>> GetPendingTransferList()
        {
            User user = userDAO.GetUser(User.Identity.Name);
            List<Transfer> transferList = transferDAO.GetTransferForUser(user.UserId, 2);
            return Ok(transferList);
        }

        [HttpPut("reject")]
        public ActionResult RejectRequest(TransferNumber transferNumber)
        {
            bool rejectSuccess = transferDAO.UpdateRequest(transferNumber.TransferId, 3);

            if (!rejectSuccess)
            {
                return StatusCode(500, "Unable to reject request / server issue.");
            }
            else
            {
                return Ok("Request rejected.");
            }
        }

        //wires put request at transfer/approve to approve money request for specific request id stored in transfernumber
        [HttpPut("approve")]
        public ActionResult ApproveRequest(TransferNumber transferNumber)
        {
            //gets user id for current user
            int userId = userDAO.GetUser(User.Identity.Name).UserId;
            //gets account from current user's id
            Account userAccount = accountDAO.GetAccountFromUserId(userId);
            //gets balance for current user's account
            decimal accountBalance = userAccount.Balance;
            //creates transfer object for base data of transfer i.e. just ids for types, accounts, no names
            RawTransferData transfer = transferDAO.GetTransferFromId(transferNumber.TransferId);
            //gets amount of transfer from transfer object
            decimal transferAmount = transfer.Amount;
            //gets account for recipient from account number
            Account recipientAccount = accountDAO.GetAccountFromAccountNumber(transfer.AccountTo);
            //checks to prevent user from approving request they made
            if (transfer.AccountTo == userAccount.AccountId)
            {
                return BadRequest("You cannot approve a request to your own account.");
            }
            //checks to make sure person approving has enough money in account to send
            if (accountBalance >= transferAmount)
            {
                bool reduceSuccess = transferDAO.ReduceBalance(transferAmount, userId);
                if (!reduceSuccess)
                {
                    return StatusCode(500, "Unable to withdraw funds / server issue.");
                }
                bool increaseSuccess = transferDAO.IncreaseBalance(transferAmount, recipientAccount.UserId);
                if (!increaseSuccess)
                {
                    return StatusCode(500, "Unable to add funds / server issue.");
                }
                //updates transfer status from "pending" to "approved"
                bool createTransferSuccess = transferDAO.UpdateRequest(transferNumber.TransferId, 2);
                if (!createTransferSuccess)
                {
                    return StatusCode(500, "Unable to record transaction / server issue.");
                }
                //if successful, returns status 200 to client w/ message
                return Ok("Request Approved, transfer successful.");
            }
            else
            {
                return BadRequest("Insufficient funds.");
            }
        }
    }
}
