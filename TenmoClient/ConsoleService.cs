using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    public class ConsoleService
    {
        /// <summary>
        /// Prompts for transfer ID to view, approve, or reject
        /// </summary>
        /// <param name="action">String to print in prompt. Expected values are "Approve" or "Reject" or "View"</param>
        /// <returns>ID of transfers to view, approve, or reject</returns>
        public int PromptForTransferID(string action, List<User> userList)
        {
            Console.WriteLine("");
            Console.Write("Please enter user ID to " + action + " (0 to cancel): ");
            bool check = true;
            int output = 0;
            while (check)
            {
                if (!int.TryParse(Console.ReadLine(), out int userId))
                {
                    Console.WriteLine("Invalid input. Only input a number (0 to cancel).");
                    //return 0;
                }
                else if (userId == 0)
                {
                    return 0;
                }
                else if (!ValidUser(userList, userId))
                {
                    Console.WriteLine("Please select a valid user ID (0 to cancel).");
                }
                else
                {
                    output = userId;
                    check = false;
                }
            }
            return output;
        }

        public LoginUser PromptForLogin()
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            string password = GetPasswordFromConsole("Password: ");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        private string GetPasswordFromConsole(string displayMessage)
        {
            string pass = "";
            Console.Write(displayMessage);
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Remove(pass.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine("");
            return pass;
        }

        public void printBalance(decimal balance)
        {
            Console.WriteLine("");
            Console.WriteLine("Your current account balance is: " + balance.ToString("C2"));
            Console.WriteLine("");
            Console.WriteLine("Press enter to continue.");
        }

        public void printUserList(List<User> userList)
        {
            Console.WriteLine();
            Console.WriteLine("----------------------------------");
            Console.WriteLine("Users");
            Console.WriteLine("ID\tName");
            Console.WriteLine("----------------------------------");
            foreach (User user in userList)
            {
                Console.WriteLine($"{user.UserId}\t{user.Username}");
            }
            Console.WriteLine("---------");
        }

        public decimal PromptForTransferAmount(string action)
        {
            Console.WriteLine("");
            Console.Write("Please enter amount to " + action + " (0 to cancel): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                Console.WriteLine("Invalid input. Only input a dollar amount.");
                return 0;
            }
            else if (amount <= 0)
            {
                Console.WriteLine("Nice try Joe. Only positive amounts.");
                return 0;
            }
            else
            {
                return amount;
            }
        }

        public void PromptPrintMessage(string message)
        {
            Console.WriteLine("");
            Console.WriteLine(message);
            Console.WriteLine("");
            Console.WriteLine("Press enter to continue.");

        }

        public static bool ValidUser(List<User> userList, int userId)
        {
            bool output = false;

            foreach (User user in userList)
            {
                if (user.UserId == userId)
                {
                    output = true;
                }
            }

            return output;
        }

        public void printTransfersFromList(List<Transfer> transferList)
        {
            Console.WriteLine();
            Console.WriteLine("----------------------------------");
            Console.WriteLine("Transfers");
            Console.WriteLine("ID\tFrom/To\t\tAmount");
            Console.WriteLine("----------------------------------");
            foreach (Transfer transfer in transferList)
            {
                Console.WriteLine(FilterByFromOrTo(transfer));    
            }
            Console.WriteLine("---------");
        }
        public void printPendingRequestsFromList(List<Transfer> transferList)
        {
            Console.WriteLine();
            Console.WriteLine("----------------------------------");
            Console.WriteLine("Pending Transfers");
            Console.WriteLine("ID\tTo\t\tAmount");
            Console.WriteLine("----------------------------------");
            foreach (Transfer transfer in transferList)
            {
                Console.WriteLine(FilterByFromOrTo(transfer));
            }
            Console.WriteLine("---------");
        }

        private string FilterByFromOrTo(Transfer transfer)
        {
            string output = "";
            if (transfer.AccountFromUserName == UserService.GetUserName())
            {
                output = $"{transfer.TransferId}\tTo:   {transfer.AccountToUserName}\t{transfer.Amount:C2}";
            }
            else
            {
                output = $"{transfer.TransferId}\tFrom: {transfer.AccountFromUserName}\t{transfer.Amount:C2}";
            }
            return output;
        }

        public int PromptForTransferDetails(List<Transfer> transferList)
        {
            Console.WriteLine("");
            Console.Write("Please enter transfer ID to view details (0 to cancel): ");
            bool check = true;
            int output = 0;
            while (check)
            {
                if (!int.TryParse(Console.ReadLine(), out int input))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
                else if (input == 0)
                {
                    return output;
                }
                else 
                {
                    foreach (Transfer transfer in transferList)
                    {
                        if (transfer.TransferId == input)
                        {
                            output = input;
                            return output;
                        }
                    }
                    Console.WriteLine("");
                    Console.WriteLine("Not a valid transfer ID.");
                    Console.WriteLine("Please enter a valid transfer ID or press 0 to exit.");
                }
            }
            return output;
        }

        public void PrintTransferDetails(List<Transfer> transferList, int transferId)
        {
            foreach (Transfer transfer in transferList)
            {
                if (transfer.TransferId == transferId)
                {
                    Console.WriteLine("----------------------------------");
                    Console.WriteLine("Transfer Details");
                    Console.WriteLine("----------------------------------");
                    Console.WriteLine($"ID:\t{transfer.TransferId}");
                    Console.WriteLine($"From:\t{transfer.AccountFromUserName}");
                    Console.WriteLine($"To:\t{transfer.AccountToUserName}");
                    Console.WriteLine($"Type:\t{transfer.TransferType}");
                    Console.WriteLine($"Status:\t{transfer.TransferStatus}");
                    Console.WriteLine($"Amount:\t{transfer.Amount:C2}");
                    Console.WriteLine("");
                    Console.WriteLine("Press enter to continue.");
                }
            }
        }

        public int PromptForIdToApproveReject(List<Transfer> transferList)
        {
            Console.WriteLine("");
            Console.Write("Please enter transfer ID to view approve/reject (0 to cancel): ");
            bool check = true;
            int output = 0;
            while (check)
            {
                if (!int.TryParse(Console.ReadLine(), out int input))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
                else if (input == 0)
                {
                    return output;
                }
                else
                {
                    foreach (Transfer transfer in transferList)
                    {
                        if (transfer.TransferId == input)
                        {
                            output = input;
                            return output;
                        }
                    }
                    Console.WriteLine("");
                    Console.WriteLine("Not a valid transfer ID.");
                    Console.WriteLine("Please enter a valid transfer ID or press 0 to exit.");
                }
            }
            return output;
        }

        public int PromptToApproveOrReject()
        {
            Console.Clear();
            Console.WriteLine("1: Approve");
            Console.WriteLine("2: Reject");
            Console.WriteLine("0: Don't approve or reject");
            Console.WriteLine("---------");
            bool check = true;
            int output = 0;
            while (check)
            {
                if (!int.TryParse(Console.ReadLine(), out int input))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
                else if (input == 0)
                {
                    return output;
                }
                else if (input > 2)
                {
                    Console.WriteLine("Nice try Joe. Please enter a number between 0 and 2.");
                }
                else
                {
                    output = input;
                    return output;
                }
            }
            return output;
        }

        public void PromptToEnter()
        {
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
        }
    }
}
