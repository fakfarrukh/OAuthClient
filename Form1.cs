using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OAuthClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string oAuthInfo = GetAuthorizeToken().Result;

                // Process response access token info.  

                // Call REST Web API method with authorize access token.  
                GetInfo(oAuthInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
            
        }

        public async Task<string> GetAuthorizeToken()
        {
            // Initialization.  
            string responseObj = string.Empty;

            // Posting.  

            string user = "admin";
            var client = new RestClient(txturl.Text+"/Token");
            var request = new RestRequest(Method.POST);
            user = txtUser.Text;
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "grant_type=password&username="+user+"&password=admin", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                if (!string.IsNullOrEmpty(response.Content))
                {

                    Token tok = JsonConvert.DeserializeObject<Token>(response.Content);
                    responseObj = tok.AccessToken;
                    txtToken.Text = responseObj;
                }
            }


            return responseObj;
        }

        public void GetInfo(string authorizeToken)
        {
            // Initialization.  
            string responseObj = string.Empty;

            // HTTP GET.  
            var client = new RestClient(txturl.Text + "/api/values");
            var request = new RestRequest(Method.GET);
            request.AddHeader("postman-token", "f58802e1-3c35-9867-5833-7c33f0a46732");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", "Bearer " + authorizeToken);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                txtResponse.Text = response.Content;
            }
        }
    }

    internal class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
