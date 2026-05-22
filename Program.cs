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
    //Initiate 
    Console.WriteLine("Made it here!");






    
}//End of Client ID and Client Secret Checks

