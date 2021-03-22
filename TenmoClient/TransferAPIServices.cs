using TenmoClient.Data;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;


namespace TenmoClient
{
    class TransferAPIServices
    {

        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly static string TRANSFER_URL = API_BASE_URL + "transfer";
        private readonly RestClient client = new RestClient();

        public List<User> GetUsers(string token)
        {
            List<User> output = new List<User>();
            RestRequest request = new RestRequest(TRANSFER_URL + "/users");
            client.Authenticator = new JwtAuthenticator(token);
            IRestResponse<List<User>> response = client.Get<List<User>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                output = response.Data;
            }

            return output;
        }

        public string ProcessErrorResponse(IRestResponse response)
        {
            string message = response.Content;
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error occurred - unable to reach server.");
            }
            else if (!response.IsSuccessful)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Please Login");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("You do not have the correct permission for the requested action");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return message;
                }
                else
                {
                    throw new Exception();
                }
            }
            return message;
        }

        public API_Transfer CreateTransfer( int accountTo, decimal amount, int accountFrom)
        {
            API_Transfer transfer = new API_Transfer()
            { AccountTo = accountTo, Amount = amount, AccountFrom = accountFrom };
            return transfer;
        }

        public string Transfer(API_Transfer transfer, string token)
        {
            RestRequest request = new RestRequest(TRANSFER_URL);
            request.AddJsonBody(transfer);
            client.Authenticator = new JwtAuthenticator(token);
            IRestResponse response = client.Post(request);
            string message = ProcessErrorResponse(response);
            return message;


        }

        public List<Transfer> GetTransfersForUser(string token)
        {
            RestRequest request = new RestRequest(TRANSFER_URL);
            client.Authenticator = new JwtAuthenticator(token);
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
            return response.Data;
        }

        public List<Transfer> GetPendingTransfersForUser(string token)
        {
            RestRequest request = new RestRequest(TRANSFER_URL + "/pending");
            client.Authenticator = new JwtAuthenticator(token);
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
            return response.Data;
        }

        public string CreateRequest(API_Transfer transfer, string token)
        {
            RestRequest request = new RestRequest(TRANSFER_URL + "/request");
            request.AddJsonBody(transfer);
            client.Authenticator = new JwtAuthenticator(token);
            IRestResponse response = client.Post(request);
            string message = ProcessErrorResponse(response);
            return message;
        }

        public string ApproveOrReject(int userInput, TransferNumber transferNumber, string token)
        {
            if (userInput == 1)
            {
                RestRequest request = new RestRequest(TRANSFER_URL + "/approve");
                request.AddJsonBody(transferNumber);
                //adding an object the server can read, it connects to all the properties in the sql statement
                client.Authenticator = new JwtAuthenticator(token);
                IRestResponse response = client.Put(request);
                string message = ProcessErrorResponse(response);
                return message;
            }
            else
            {
                RestRequest request = new RestRequest(TRANSFER_URL + "/reject");
                request.AddJsonBody(transferNumber);
                client.Authenticator = new JwtAuthenticator(token);
                IRestResponse response = client.Put(request);
                string message = ProcessErrorResponse(response);
                return message;
            }
        }
    }
}
