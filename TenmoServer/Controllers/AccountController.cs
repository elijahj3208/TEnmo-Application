using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDAO accountDAO;
        private readonly IUserDAO userDAO;

        public AccountController(IAccountDAO _accountDAO, IUserDAO _userDAO)
        {
            accountDAO = _accountDAO;
            userDAO = _userDAO;
        }
        //wires get request at /account/balance to return current user's account balance
        [HttpGet("balance")]
        public decimal GetBalance()
        {
            //sql call returns user object from current user's name
            User user = userDAO.GetUser(User.Identity.Name);

            //make's sql call to get current user's account and return balance property
            return accountDAO.GetAccountFromUserId(user.UserId).Balance;
        }
    }
}
