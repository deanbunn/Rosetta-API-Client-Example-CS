using DotNetEnv;
using RosettaAPIApp;

//Load Environment Variables File
Env.Load();

//Initiate UCD API Info
UCDAPIInfo ucdAPIInfo = new();

//Load API Information
ucdAPIInfo.Client_ID = Environment.GetEnvironmentVariable("ROSETTA_CLIENT_ID");
ucdAPIInfo.Client_Secret = Environment.GetEnvironmentVariable("ROSETTA_CLIENT_SECRET");
ucdAPIInfo.Base_Url = Environment.GetEnvironmentVariable("ROSETTA_BASE_URL");
ucdAPIInfo.Token_Url = Environment.GetEnvironmentVariable("ROSETTA_OAUTH_URL");
ucdAPIInfo.Test_ID = Environment.GetEnvironmentVariable("ROSETTA_TEST_ID");   

//Check for Required Client ID and Secret Before Making API Calls
if(string.IsNullOrEmpty(ucdAPIInfo.Client_ID) == false && string.IsNullOrEmpty(ucdAPIInfo.Client_Secret) == false)
{

    //Initiate Http Client to Get OAuth Token
    using(var client = new HttpClient())
    {
        client.DefaultRequestHeaders.Add("client_id",ucdAPIInfo.Client_ID);
        client.DefaultRequestHeaders.Add("client_secret",ucdAPIInfo.Client_Secret);
        client.DefaultRequestHeaders.Add("grant_type","CLIENT_CREDENTIALS");
        client.DefaultRequestHeaders.Add("scope","read:public");

        HttpResponseMessage response = client.PostAsync(ucdAPIInfo.Token_Url, null).Result;

        //Read Response Body
        string responsebody = response.Content.ReadAsStringAsync().Result;

        Console.WriteLine("Status: " + response.StatusCode);
        Console.WriteLine(responsebody);

    }



    
}//End of Client ID and Client Secret Checks

