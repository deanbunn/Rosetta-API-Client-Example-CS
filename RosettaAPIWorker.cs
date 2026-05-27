using System.Text.Json;
using DotNetEnv;

namespace UCDRosettaAPI;

public class RosettaAPIWorker
{
    public string? Base_Url {get;set;}
    private string? Token_Url {get;set;}
    private string? _Client_ID {get;set;}
    private string? _Client_Secret {get;set;}
    private string? _OAuth_Token {get;set;}
    private string? _OAuth_Scopes {get;set;}
    public string? Test_ID {get;set;}
    public long Expires_in_Ticks {get;set;}

    public RosettaAPIWorker()
    {
        //Load Environment Variables File
        Env.Load();

        //Load API Information
        _Client_ID = Environment.GetEnvironmentVariable("ROSETTA_CLIENT_ID");
        _Client_Secret = Environment.GetEnvironmentVariable("ROSETTA_CLIENT_SECRET");
        _OAuth_Scopes = Environment.GetEnvironmentVariable("ROSETTA_SCOPES");
        Base_Url = Environment.GetEnvironmentVariable("ROSETTA_BASE_URL");
        Token_Url = Environment.GetEnvironmentVariable("ROSETTA_OAUTH_URL");
        Test_ID = Environment.GetEnvironmentVariable("ROSETTA_TEST_ID"); 
        
        //Configure Inital Ticks Value
        Expires_in_Ticks = 0;

    }

    public enum SearchBy
    {
        iamid,
        loginid,
        email,
        employeeid,
        studentid,
        mailid,
        department
    }

    public bool CheckOAuthToken()
    {
        //Var for Return Value
        bool bTokenStatus = true;

        //Check Ticks
        if(DateTime.Now.AddMinutes(1).Ticks >= Expires_in_Ticks)
        {
            //Initiate Http Client to Get OAuth Token
            using(var client = new HttpClient())
            {

                //Add Required Header Values
                client.DefaultRequestHeaders.Add("client_id",_Client_ID);
                client.DefaultRequestHeaders.Add("client_secret",_Client_Secret);
                client.DefaultRequestHeaders.Add("grant_type","CLIENT_CREDENTIALS");
                client.DefaultRequestHeaders.Add("scope",_OAuth_Scopes);

                //Post Sent to Token Url
                HttpResponseMessage response = client.PostAsync(Token_Url, null).Result;

                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //Read Response Body
                    string responsebody = response.Content.ReadAsStringAsync().Result;

                    //Parse Response Body Json
                    var jnOAuthPayload = JsonDocument.Parse(responsebody);

                    //Get Root Json Element
                    var jnOAuthRoot = jnOAuthPayload.RootElement;

                    //Make Sure Access Token and Expires Values are Included in Payload
                    if(jnOAuthRoot.TryGetProperty("access_token", out JsonElement accessTokenElement) && 
                    jnOAuthRoot.TryGetProperty("expires_in", out JsonElement expiresInElement))
                    {

                        //Load OAuth Token
                        _OAuth_Token = accessTokenElement.GetString();

                        //Determine When It Expires
                        if(expiresInElement.TryGetDouble(out double dblExpiresIn))
                        {
                            Expires_in_Ticks = DateTime.Now.AddSeconds(dblExpiresIn).Ticks;
                        }
                        else
                        {
                            Expires_in_Ticks = 0;
                            bTokenStatus = false;
                        }

                    }
                    else
                    {
                        Expires_in_Ticks = 0;
                        bTokenStatus = false;
                    }//End of Access_Token and Expires_In Checks

                }
                else
                {
                    Expires_in_Ticks = 0;
                    bTokenStatus = false;
                }//End of Status Code Check

            }//End of HttpClient

        }//End of Ticks Check

        return bTokenStatus;

    }

    public List<RosettaPerson> GetPeopleBySearchTerm(SearchBy searchBy, string searchTerm)
    {
        //Var for Return List
        List<RosettaPerson> lRosettaPeople = new();

        //Var for Search Result Limit
        int nSrchRsltLimit = 100;

        //Var for Search Result Offset
        int nSrchRsltOffset = 0;

        //Var for Retrieve More Search Results
        bool bRetrMoreSrchRslts = true;

        do
        {
            //Check OAuth Token
            if(CheckOAuthToken() == true)
            {
                //Initiate Http Client to Get People Information
                using(var client = new HttpClient())
                {
                    //Var for Bearer Token
                    string bearerToken = "Bearer " + _OAuth_Token;

                    //Add Required Header Values
                    client.DefaultRequestHeaders.Add("Authorization",bearerToken);

                    //Var for People Url
                    string peopleURL = Base_Url + "people?"+ searchBy.ToString() + "=" + searchTerm + "&offset=" + nSrchRsltOffset.ToString() + "&limit=" + nSrchRsltLimit.ToString() + "&count=true";

                    //Get to People Endpoint
                    HttpResponseMessage response = client.GetAsync(peopleURL).Result;

                    //Check Response Status Code
                    if(response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        
                        //Pull X-Total-Count and x-response-count
                        if(response.Headers.TryGetValues("x-total-count", out var xtcount) 
                        && response.Headers.TryGetValues("x-response-count", out var xrpcount)
                        && int.TryParse(xtcount.First(),out int nTotalCnt) 
                        && int.TryParse(xrpcount.First(),out int nRspnCnt))
                        {

                            //Check Total and Response Counts are Not Empty
                            if(nTotalCnt > 0 && nRspnCnt > 0)
                            {
                                //Read Response Body
                                string responsebody = response.Content.ReadAsStringAsync().Result;

                                //Parse the Response Body
                                using(JsonDocument jdPeople = JsonDocument.Parse(responsebody))
                                {

                                    //Access the Array at the Root
                                    JsonElement root = jdPeople.RootElement;

                                    //Iterate Through the Array
                                    foreach(JsonElement element in root.EnumerateArray())
                                    {
                                        //Initialize Person to Return
                                        RosettaPerson rosettaPerson = new();

                                        //Pull Display Name
                                        if(element.TryGetProperty("displayname",out JsonElement displayNameElement))
                                        {
                                            rosettaPerson.DisplayName = displayNameElement.GetString();
                                        }

                                        //Many More Coming
                                        //
                                        //
                                        //

                                        //Add Rosetta Person to Returned People List
                                        lRosettaPeople.Add(rosettaPerson);

                                    }//End of Root Enumerate Array

                                }//End Parse Response Body

                                //Increment Offset
                                nSrchRsltOffset += nSrchRsltLimit;

                                //Check Offset to Total Count
                                if(nSrchRsltOffset >= nTotalCnt)
                                {
                                    bRetrMoreSrchRslts = false;
                                }

                            }
                            else
                            {
                                bRetrMoreSrchRslts = false;
                            }//End of nTotalCnt and nRspnCnt Empty Checks

                        }
                        else
                        {
                            bRetrMoreSrchRslts = false;
                        }//End of Return Header Counts Checks

                    }
                    else
                    {
                        bRetrMoreSrchRslts = false;
                    }//End of Status Code Check

                }//End of HttpClient

            }
            else
            {
                bRetrMoreSrchRslts = false;
            }//End of CheckOAuthToken

        }
        while(bRetrMoreSrchRslts == true);

        //Return Person
        return lRosettaPeople;
    }

}