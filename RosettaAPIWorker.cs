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

    public RosettaPerson ParseRosettaPersonJson(JsonElement jePeople)
    {

        //Initialize Person to Return
        RosettaPerson rosettaPerson = new();

        //Retrieve Display Name
        if(jePeople.TryGetProperty("displayname",out JsonElement jeDisplayName))
        {
            rosettaPerson.DisplayName = jeDisplayName.GetString() ?? "";
        }

        //Retrieve IAM ID
        if(jePeople.TryGetProperty("iam_id",out JsonElement jeIAMID))
        {
            rosettaPerson.IAM_ID = jeIAMID.GetString() ?? "";
        }

        //Retrieve IDs
        if(jePeople.TryGetProperty("id",out JsonElement jeIDs))
        {

            //Retrieve IAM ID
            if(jeIDs.TryGetProperty("iam_id",out JsonElement jeIDsIAM))
            {
                rosettaPerson.IAM_ID = jeIDsIAM.GetString() ?? "";
            }
            
            //Retrieve Login ID
            if(jeIDs.TryGetProperty("login_id",out JsonElement jeIDsLogin))
            {
                rosettaPerson.Login_ID = jeIDsLogin.GetString() ?? "";
            }

            //Retrieve Mothra ID
            if(jeIDs.TryGetProperty("mothra_id",out JsonElement jeIDsMothra))
            {
                rosettaPerson.Mothra_ID = jeIDsMothra.GetString() ?? "";
            }

            //Retrieve Employee ID
            if(jeIDs.TryGetProperty("employee_id",out JsonElement jeIDsEmployee))
            {
                rosettaPerson.Employee_ID = jeIDsEmployee.GetString() ?? "";
            }

            //Retrieve Mail IDs
            if(jeIDs.TryGetProperty("mail_id",out JsonElement jeIDsMail))
            {
                //Check for Campus Mail ID
                if(jeIDsMail.TryGetProperty("campus",out JsonElement jeIDsMailCampus))
                {
                    rosettaPerson.Mail_ID_Campus = jeIDsMailCampus.GetString() ?? "";
                }

                //Check for Health Mail ID
                if(jeIDsMail.TryGetProperty("health",out JsonElement jeIDsMailHealth))
                {
                    rosettaPerson.Mail_ID_Health = jeIDsMailHealth.GetString() ?? "";
                }

            }
            
        }//End of IDs 

        //Retrieve Names
        if(jePeople.TryGetProperty("name",out JsonElement jeNames))
        {
            //Check for Lived First Name
            if(jeNames.TryGetProperty("lived_first_name",out JsonElement jeNamesLivedFirst))
            {
                rosettaPerson.Lived_First_Name = jeNamesLivedFirst.GetString() ?? "";
            }

            //Check for Lived Last Name
            if(jeNames.TryGetProperty("lived_last_name",out JsonElement jeNamesLivedLast))
            {
                rosettaPerson.Lived_Last_Name = jeNamesLivedLast.GetString() ?? "";
            }

        }//End of Names Checks

        //Retrieve Email Addresses
        if(jePeople.TryGetProperty("email",out JsonElement jeEmailAddress))
        {

            //Check for Campus Email Address
            if(jeEmailAddress.TryGetProperty("campus",out JsonElement jeEmailAddressCampus))
            {
                rosettaPerson.Email_Address_Campus = jeEmailAddressCampus.GetString() ?? "";
            }

            //Check for Health Email Address
            if(jeEmailAddress.TryGetProperty("health",out JsonElement jeEmailAddressHealth))
            {
                rosettaPerson.Email_Address_Health = jeEmailAddressHealth.GetString() ?? "";
            }

        }
        
        //Retrieve Provisioning Statuses
        if(jePeople.TryGetProperty("provisioning_status",out JsonElement jeProvisioningStatus))
        {

            //Retrieve Primary Provisioning Status
            if(jeProvisioningStatus.TryGetProperty("primary",out JsonElement jeProvisioningStatusPrimary))
            {
                rosettaPerson.ProvisioningStatus_Primary = jeProvisioningStatusPrimary.GetString() ?? "";
            }
            
            //Retrieve Employee Provisioning Status
            if(jeProvisioningStatus.TryGetProperty("employee",out JsonElement jeProvisioningStatusEmployee))
            {
                rosettaPerson.ProvisioningStatus_Employee = jeProvisioningStatusEmployee.GetString() ?? "";
            }

            //Retrieve Faculty Provisioning Status
            if(jeProvisioningStatus.TryGetProperty("faculty",out JsonElement jeProvisioningStatusFaculty))
            {
                rosettaPerson.ProvisioningStatus_Faculty = jeProvisioningStatusFaculty.GetString() ?? "";
            }

            //Retrieve Student Provisioning Status
            if(jeProvisioningStatus.TryGetProperty("student",out JsonElement jeProvisioningStatusStudent))
            {
                rosettaPerson.ProvisioningStatus_Student = jeProvisioningStatusStudent.GetString() ?? "";
            }
            
        }//End of Provisioning Status

        //Retrieve Affiliation 
        if(jePeople.TryGetProperty("affiliation",out JsonElement jeAffiliation))
        {
            //Loop Through Each Affiliation
            foreach(JsonElement jeAffil in jeAffiliation.EnumerateArray())
            {

                switch(jeAffil.GetString())
                {

                    case "employee":
                        rosettaPerson.Affiliation_Employee = true;
                        break;
                    
                    case "faculty":
                        rosettaPerson.Affiliation_Faculty = true;
                        break;
                    
                    case "temporary_affiliate":
                        rosettaPerson.Affiliation_Temporary_Affiliate = true;
                        break;

                    case "student":
                        rosettaPerson.Affiliation_Student = true;
                        break;

                    case "student_applicant":
                        rosettaPerson.Affiliation_Student_Applicant = true;
                        break;

                    case "health_affiliate":
                        rosettaPerson.Affiliation_Health_Affiliate = true;
                        break;

                }//End of jeAffil Switch Statement

            }//End of Affiliation Enumerate Array

        }//End of Affiliations 

        //Retrieve Employment Status 
        if(jePeople.TryGetProperty("employment_status",out JsonElement jeEmploymentStatus))
        {
            //Loop Through Each Employment Status
            foreach(JsonElement jeEmplStatus in jeEmploymentStatus.EnumerateArray())
            {

                switch(jeEmplStatus.GetString())
                {

                    case "is_academic":
                        rosettaPerson.Employment_Is_Academic = true;
                        break;
                    
                    case "is_academic_senate":
                        rosettaPerson.Employment_Is_Academic_Senate = true;
                        break;
                    
                    case "is_academic_federation":
                        rosettaPerson.Employment_Is_Academic_Federation = true;
                        break;

                    case "is_faculty":
                        rosettaPerson.Employment_Is_Faculty = true;
                        break;

                    case "is_teaching_faculty":
                        rosettaPerson.Employment_Is_Teaching_Faculty = true;
                        break;

                    case "is_ladder_rank":
                        rosettaPerson.Employment_Is_Ladder_Rank = true;
                        break;

                    case "is_without_salary":
                        rosettaPerson.Employment_Is_Without_Salary = true;
                        break;

                    case "is_msp":
                        rosettaPerson.Employment_Is_MSP = true;
                        break;

                    case "is_ssp":
                        rosettaPerson.Employment_Is_SSP = true;
                        break;

                    case "is_manager":
                        rosettaPerson.Employment_Is_Manager = true;
                        break;

                    case "is_campus_employee":
                        rosettaPerson.Employment_Is_Campus_Employee = true;
                        break;

                    case "is_health_employee":
                        rosettaPerson.Employment_Is_Health_Employee = true;
                        break;

                }//End of jeEmplStatus Switch Statement

            }//End of Employment Status Enumerate Array

        }//End of Employment Statuses

        return rosettaPerson;
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

                                        //Add Rosetta Person to Returned People List
                                        lRosettaPeople.Add(ParseRosettaPersonJson(element));

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