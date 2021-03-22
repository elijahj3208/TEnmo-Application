using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly AccountAPIService accountService = new AccountAPIService();
        private static readonly TransferAPIServices transferService = new TransferAPIServices();
        

        static void Main(string[] args)
        {
            Run();
        }

        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.Clear();
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        API_User user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in. Press any key to continue.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                            Console.ReadLine();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection. Press any key to return to login.");
                    Console.ReadLine();
                }
            }

            MenuSelection();
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            string token = UserService.GetToken();
            while (menuSelection != 0)
            {
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    Console.Clear();
                    //prints the results of the get balance function 
                    consoleService.printBalance(accountService.GetBalance(token));
                    Console.ReadLine();
                }
                else if (menuSelection == 2)
                {
                    Console.Clear();
                    //generates list of transfers for logged in user
                    List<Transfer> transferList = transferService.GetTransfersForUser(token);
                    //prints transfer list to console
                    consoleService.printTransfersFromList(transferList);
                    //prints to console, asks user for transfer to view details, deals w/ bad input
                    int transferId = consoleService.PromptForTransferDetails(transferList);
                    //if they choose zero, bounces out to main menu
                    if (transferId != 0)
                    {
                        Console.Clear();
                        //once they enter valid transfer number, prints details for transfer from the transfer list
                        consoleService.PrintTransferDetails(transferList, transferId);
                        Console.ReadLine();
                    }
                }
                else if (menuSelection == 3)
                {
                    Console.Clear();
                    //generates list of pending transfers for users
                    List<Transfer> transferList = transferService.GetPendingTransfersForUser(token);
                    //prints list of pending transfers
                    consoleService.printPendingRequestsFromList(transferList);
                    //prompts user for ID of request to approve/reject, deals with bad input
                    int transferId = consoleService.PromptForIdToApproveReject(transferList);
                    //storing the result from  promptforid in object to serialize for back end
                    TransferNumber transferNumber = new TransferNumber() { TransferId = transferId };
                    if (transferId != 0)
                    {
                        int userInput = consoleService.PromptToApproveOrReject();
                        if (userInput != 0)
                        {
                            string message = transferService.ApproveOrReject(userInput, transferNumber, token);
                            consoleService.PromptPrintMessage(message);
                            Console.ReadLine();
                        }
                    }

                }
                else if (menuSelection == 4)
                {
                    Console.Clear();
                    List<User> userList = transferService.GetUsers(token);
                    consoleService.printUserList(userList);
                    int transferUserId = consoleService.PromptForTransferID("transfer", userList);
                    if (transferUserId != 0)
                    {
                        decimal transferAmount = consoleService.PromptForTransferAmount("transfer");
                        if (transferAmount != 0)
                        {
                            API_Transfer transfer = transferService.CreateTransfer(transferUserId, transferAmount, UserService.GetUserId());
                            string responseMessage = transferService.Transfer(transfer, token);
                            consoleService.PromptPrintMessage(responseMessage);
                            Console.ReadLine();
                        }
                    }
                    Console.ReadLine();
                }
                else if (menuSelection == 5)
                {
                    Console.Clear();
                    List<User> userList = transferService.GetUsers(token);
                    consoleService.printUserList(userList);
                    int transferUserId = consoleService.PromptForTransferID("request", userList);
                    if (transferUserId != 0)
                    {
                        decimal transferAmount = consoleService.PromptForTransferAmount("request");
                        if (transferAmount != 0)
                        {
                            API_Transfer transfer = transferService.CreateTransfer(UserService.GetUserId(), transferAmount, transferUserId);
                            string responseMessage = transferService.CreateRequest(transfer, token);
                            consoleService.PromptPrintMessage(responseMessage);
                        }
                    }
                    Console.ReadLine();
                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    Run(); //return to entry point
                    
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
