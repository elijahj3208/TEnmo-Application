using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient
{
    public class AccountAPIService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly static string ACCOUNT_URL = API_BASE_URL + "account";
        private readonly RestClient client = new RestClient();

        public AccountAPIService()
        {
        }

        public decimal GetBalance(string token)
        {
            decimal output = 0.00M;
            RestRequest request = new RestRequest(ACCOUNT_URL + "/balance");
            client.Authenticator = new JwtAuthenticator(token);
            IRestResponse<decimal> response = client.Get<decimal>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                ProcessErrorResponse(response);
            }
            else if (!response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            } 
            else
            {
                output = response.Data;
            }

            return output;
        }

        public void ProcessErrorResponse(IRestResponse response)
        {
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
                else
                {
                    throw new Exception();
                }
            }
        }
    }
}
