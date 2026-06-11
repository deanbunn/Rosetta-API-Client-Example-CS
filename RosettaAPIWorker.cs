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

    public enum PeopleSearchBy
    {
        iamid,
        loginid,
        email,
        employeeid,
        studentid,
        mailid,
        department
    }

    public enum EmployeeSearchBy
    {
        iamid,
        departmentid,
        divisionid,
        subdivisionid,
        subdivisionl4id,
        organizationid
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

    public RosettaStudentAssociationShort ParseRosettaStudentAssocShortJson(JsonElement jeStudentAssocShrt)
    {
        //Initialize Student Association to Return
        RosettaStudentAssociationShort rosettaStudentAssoc = new();

        //Retrieve College Code
        if(jeStudentAssocShrt.TryGetProperty("college_code",out JsonElement jeCollegeCode))
        {
            rosettaStudentAssoc.College_Code = jeCollegeCode.GetString() ?? "";
        }

        //Retrieve College Title 
        if(jeStudentAssocShrt.TryGetProperty("college_title",out JsonElement jeCollegeTitle))
        {
            rosettaStudentAssoc.College_Title = jeCollegeTitle.GetString() ?? "";
        }

        //Retrieve Major Code
        if(jeStudentAssocShrt.TryGetProperty("major_code",out JsonElement jeMajorCode))
        {
            rosettaStudentAssoc.Major_Code = jeMajorCode.GetString() ?? "";
        }

        //Retrieve Major Title
        if(jeStudentAssocShrt.TryGetProperty("major_title",out JsonElement jeMajorTitle))
        {
            rosettaStudentAssoc.Major_Title = jeMajorTitle.GetString() ?? "";
        }

        //Retrieve Academic Level
        if(jeStudentAssocShrt.TryGetProperty("academic_level",out JsonElement jeAcademicLvl))
        {
            rosettaStudentAssoc.Academic_Level = jeAcademicLvl.GetString() ?? "";
        }

        //Retrieve Class Level
        if(jeStudentAssocShrt.TryGetProperty("class_level",out JsonElement jeClassLvl))
        {
            rosettaStudentAssoc.Class_Level = jeClassLvl.GetString() ?? "";
        }

        return rosettaStudentAssoc;
    }

    public RosettaStudentAssociation ParseRosettaStudentAssocJson(JsonElement jeStudentAssoc)
    {

        //Initialize Student Association to Return
        RosettaStudentAssociation rosettaStudentAssoc = new();

        //Retrieve IAM ID
        if(jeStudentAssoc.TryGetProperty("iam_id",out JsonElement jeIAMID))
        {
            rosettaStudentAssoc.IAM_ID = jeIAMID.GetString() ?? "";
        }

        //Retrieve Student ID
        if(jeStudentAssoc.TryGetProperty("student_id",out JsonElement jeStudentID))
        {
            rosettaStudentAssoc.Student_ID = jeStudentID.GetString() ?? "";
        }

        //Retrieve PIDM
        if(jeStudentAssoc.TryGetProperty("pidm",out JsonElement jePIDM))
        {
            rosettaStudentAssoc.PIDM = jePIDM.GetString() ?? "";
        }

        //Retrieve College Code
        if(jeStudentAssoc.TryGetProperty("college_code",out JsonElement jeCollegeCode))
        {
            rosettaStudentAssoc.College_Code = jeCollegeCode.GetString() ?? "";
        }

        //Retrieve College Title 
        if(jeStudentAssoc.TryGetProperty("college_title",out JsonElement jeCollegeTitle))
        {
            rosettaStudentAssoc.College_Title = jeCollegeTitle.GetString() ?? "";
        }

        //Retrieve Major Code
        if(jeStudentAssoc.TryGetProperty("major_code",out JsonElement jeMajorCode))
        {
            rosettaStudentAssoc.Major_Code = jeMajorCode.GetString() ?? "";
        }

        //Retrieve Major Title
        if(jeStudentAssoc.TryGetProperty("major_title",out JsonElement jeMajorTitle))
        {
            rosettaStudentAssoc.Major_Title = jeMajorTitle.GetString() ?? "";
        }

        //Retrieve Level Affiliation Code
        if(jeStudentAssoc.TryGetProperty("lvl_affiliation_code",out JsonElement jeLvlAfflCode))
        {
            rosettaStudentAssoc.Level_Affiliation_Code = jeLvlAfflCode.GetString() ?? "";
        }

        //Retrieve Class Affiliation Code
        if(jeStudentAssoc.TryGetProperty("cls_affiliation_code",out JsonElement jeClsAfflCode))
        {
            rosettaStudentAssoc.Class_Affiliation_Code = jeClsAfflCode.GetString() ?? "";
        }

        //Retrieve Rank
        if(jeStudentAssoc.TryGetProperty("rank",out JsonElement jeRank))
        {
            rosettaStudentAssoc.Rank = jeRank.GetString() ?? "";
        }

        return rosettaStudentAssoc;
    }

    public RosettaEmployeeAssociation ParseRosettaEmployeeAssocJson(JsonElement jeEmploymentAssoc)
    {
        //Initialize Employee Association to Return
        RosettaEmployeeAssociation rosettaEmplAssoc = new();

        //Retrieve IAM ID
        if(jeEmploymentAssoc.TryGetProperty("iam_id",out JsonElement jeIAMID))
        {
            rosettaEmplAssoc.IAM_ID = jeIAMID.GetString() ?? "";
        }

        //Retrieve Employee Record
        if(jeEmploymentAssoc.TryGetProperty("employee_record",out JsonElement jeEmployeeRecord))
        {
            rosettaEmplAssoc.Employee_Record = jeEmployeeRecord.GetString() ?? "";
        }

        //Retrieve Employee ID
        if(jeEmploymentAssoc.TryGetProperty("employee_id",out JsonElement jeEmployeeID))
        {
            rosettaEmplAssoc.Employee_ID = jeEmployeeID.GetString() ?? "";
        }

        //Retrieve Position Number
        if(jeEmploymentAssoc.TryGetProperty("position_number",out JsonElement jePositionNumber))
        {
            rosettaEmplAssoc.Position_Number = jePositionNumber.GetString() ?? "";
        }

        //Retrieve Position Title
        if(jeEmploymentAssoc.TryGetProperty("position_title",out JsonElement jePositionTitle))
        {
            rosettaEmplAssoc.Position_Title = jePositionTitle.GetString() ?? "";
        }

        //Retrieve Relationship to Organization
        if(jeEmploymentAssoc.TryGetProperty("relationship_to_organization",out JsonElement jeRelationToOrg))
        {
            rosettaEmplAssoc.Relationship_To_Organization = jeRelationToOrg.GetString() ?? "";
        }

        //Retrieve Employee Classification
        if(jeEmploymentAssoc.TryGetProperty("employee_classification",out JsonElement jeEmplClassifction))
        {
            rosettaEmplAssoc.Employee_Classification = jeEmplClassifction.GetString() ?? "";
        }

        //Retrieve Employee Classification Description
        if(jeEmploymentAssoc.TryGetProperty("employee_classification_description",out JsonElement jeEmplClassifctionDescp))
        {
            rosettaEmplAssoc.Employee_Classification_Description = jeEmplClassifctionDescp.GetString() ?? "";
        }

        //Retrieve Status
        if(jeEmploymentAssoc.TryGetProperty("status",out JsonElement jeStatus))
        {
            rosettaEmplAssoc.Status = jeStatus.GetString() ?? "";
        }

        //Retrieve Hire Date
        if(jeEmploymentAssoc.TryGetProperty("hire_date",out JsonElement jeHireDate))
        {
            rosettaEmplAssoc.Hire_Date = jeHireDate.GetString() ?? "";
        }

        //Retrieve Start Date
        if(jeEmploymentAssoc.TryGetProperty("start_date",out JsonElement jeStartDate))
        {
            rosettaEmplAssoc.Start_Date = jeStartDate.GetString() ?? "";
        }

        //Retrieve FTE Percentage
        if(jeEmploymentAssoc.TryGetProperty("fte_percentage",out JsonElement jeFTEPercentage))
        {
            rosettaEmplAssoc.FTE_Percentage = jeFTEPercentage.GetString() ?? "";
        }

        //Retrieve Joy Type ID
        if(jeEmploymentAssoc.TryGetProperty("job_type_id",out JsonElement jeJobTypeID))
        {
            rosettaEmplAssoc.Job_Type_ID = jeJobTypeID.GetString() ?? "";
        }

        //Retrieve Job Type Description
        if(jeEmploymentAssoc.TryGetProperty("job_type_description",out JsonElement jeJobTypeDesc))
        {
            rosettaEmplAssoc.Job_Type_Description = jeJobTypeDesc.GetString() ?? "";
        }

        //Retrieve Organization ID
        if(jeEmploymentAssoc.TryGetProperty("organization_id",out JsonElement jeOrganizationID))
        {
            rosettaEmplAssoc.Organization_ID = jeOrganizationID.GetString() ?? "";
        }

        //Retrieve Organization Title
        if(jeEmploymentAssoc.TryGetProperty("organization_title",out JsonElement jeOrgTitle))
        {
            rosettaEmplAssoc.Organization_Title = jeOrgTitle.GetString() ?? "";
        }

        //Retrieve Division ID
        if(jeEmploymentAssoc.TryGetProperty("division_id",out JsonElement jeDivisionID))
        {
            rosettaEmplAssoc.Division_ID = jeDivisionID.GetString() ?? "";
        }

        //Retrieve Division Title
        if(jeEmploymentAssoc.TryGetProperty("division_title",out JsonElement jeDivisionTitle))
        {
            rosettaEmplAssoc.Division_Title = jeDivisionTitle.GetString() ?? "";
        }

        //Retrieve Subdivision ID
        if(jeEmploymentAssoc.TryGetProperty("subdivision_id",out JsonElement jeSubDivID))
        {
            rosettaEmplAssoc.Subdivision_ID = jeSubDivID.GetString() ?? "";
        }

        //Retrieve Subdivision Title
        if(jeEmploymentAssoc.TryGetProperty("subdivision_title",out JsonElement jeSudDivTitle))
        {
            rosettaEmplAssoc.Subdivision_Title = jeSudDivTitle.GetString() ?? "";
        }

        //Retrieve Subdivision L4 ID
        if(jeEmploymentAssoc.TryGetProperty("subdivision_l4_id",out JsonElement jeSubDivL4ID))
        {
            rosettaEmplAssoc.Subdivision_L4_ID = jeSubDivL4ID.GetString() ?? "";
        }

        //Retrieve Subdivision L4 Title
        if(jeEmploymentAssoc.TryGetProperty("subdivision_l4_title",out JsonElement jeSubDivL4Title))
        {
            rosettaEmplAssoc.Subdivision_L4_Title = jeSubDivL4Title.GetString() ?? "";
        }

        //Retrieve Business Unit ID
        if(jeEmploymentAssoc.TryGetProperty("business_unit_id",out JsonElement jeBusinessUnitID))
        {
            rosettaEmplAssoc.Business_Unit_ID = jeBusinessUnitID.GetString() ?? "";
        }

        //Retrieve Business Unit Title
        if(jeEmploymentAssoc.TryGetProperty("business_unit_title",out JsonElement jeBusinessUnitTitle))
        {
            rosettaEmplAssoc.Business_Unit_Title = jeBusinessUnitTitle.GetString() ?? "";
        }

        //Retrieve Department ID
        if(jeEmploymentAssoc.TryGetProperty("department_id",out JsonElement jeDepartmentID))
        {
            rosettaEmplAssoc.Department_ID = jeDepartmentID.GetString() ?? "";
        }

        //Retrieve Department Title
        if(jeEmploymentAssoc.TryGetProperty("department_title",out JsonElement jeDepartmentTitle))
        {
            rosettaEmplAssoc.Department_Title = jeDepartmentTitle.GetString() ?? "";
        }

        //Retrieve Department Short Title
        if(jeEmploymentAssoc.TryGetProperty("department_short_title",out JsonElement jeDepartmentShortTitle))
        {
            rosettaEmplAssoc.Department_Short_Title = jeDepartmentShortTitle.GetString() ?? "";
        }

        //Retrieve Reports to Position
        if(jeEmploymentAssoc.TryGetProperty("reports_to_position",out JsonElement jeReportsToPosition))
        {
            rosettaEmplAssoc.Reports_To_Position = jeReportsToPosition.GetString() ?? "";
        }

        //Retrieve Reports To IAM ID
        if(jeEmploymentAssoc.TryGetProperty("reports_to_iam_id",out JsonElement jeReportsToIAMID))
        {
            rosettaEmplAssoc.Reports_To_IAM_ID = jeReportsToIAMID.GetString() ?? "";
        }

        //Retrieve Reports to Employee ID
        if(jeEmploymentAssoc.TryGetProperty("reports_to_employee_id",out JsonElement jeReportsToEmpID))
        {
            rosettaEmplAssoc.Reports_To_Employee_ID = jeReportsToEmpID.GetString() ?? "";
        }

        //Retrieve Is Health Position
        if(jeEmploymentAssoc.TryGetProperty("is_health_position",out JsonElement jeIsHealthPos))
        {
            rosettaEmplAssoc.Is_Health_Position = jeIsHealthPos.GetString() ?? "";
        }

        //Retrieve Is Campus Position
        if(jeEmploymentAssoc.TryGetProperty("is_campus_position",out JsonElement jeIsCampusPos))
        {
            rosettaEmplAssoc.Is_Campus_Position = jeIsCampusPos.GetString() ?? "";
        }

        return rosettaEmplAssoc;
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
                rosettaPerson.Provisioning_Status_Primary = jeProvisioningStatusPrimary.GetString() ?? "";
            }
            
            //Retrieve Employee Provisioning Status
            if(jeProvisioningStatus.TryGetProperty("employee",out JsonElement jeProvisioningStatusEmployee))
            {
                rosettaPerson.Provisioning_Status_Employee = jeProvisioningStatusEmployee.GetString() ?? "";
            }

            //Retrieve Faculty Provisioning Status
            if(jeProvisioningStatus.TryGetProperty("faculty",out JsonElement jeProvisioningStatusFaculty))
            {
                rosettaPerson.Provisioning_Status_Faculty = jeProvisioningStatusFaculty.GetString() ?? "";
            }

            //Retrieve Student Provisioning Status
            if(jeProvisioningStatus.TryGetProperty("student",out JsonElement jeProvisioningStatusStudent))
            {
                rosettaPerson.Provisioning_Status_Student = jeProvisioningStatusStudent.GetString() ?? "";
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

        //Check for Employment Associations
        if(jePeople.TryGetProperty("employee_association",out JsonElement jeEmploymentAssociations))
        {
            //Loop Through Each Employment Association
            foreach(JsonElement jeEmplAssociation in jeEmploymentAssociations.EnumerateArray())
            {
                rosettaPerson.lEmployeeAssociations.Add(ParseRosettaEmployeeAssocJson(jeEmplAssociation));
            }

            //Update Employee Associations with IAM ID
            if(string.IsNullOrEmpty(rosettaPerson.IAM_ID) == false && rosettaPerson.lEmployeeAssociations.Count > 0)
            {
                foreach(RosettaEmployeeAssociation rea in rosettaPerson.lEmployeeAssociations)
                {
                    rea.IAM_ID = rosettaPerson.IAM_ID;
                }
            }

        }//End of Employment Associations

        //Check for Student Associations
        if(jePeople.TryGetProperty("student_association",out JsonElement jeStudentAssociations))
        {
            //Loop Through Each Student Association
            foreach(JsonElement jeStdtAssociation in jeStudentAssociations.EnumerateArray())
            {
                rosettaPerson.lStudentAssociations.Add(ParseRosettaStudentAssocShortJson(jeStdtAssociation));
            }

        }//End of Student Associations

        return rosettaPerson;
    }

    public List<RosettaPerson> GetPeopleBySearchTerm(PeopleSearchBy searchBy, string searchTerm)
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

    public List<RosettaEmployeeAssociation> GetEmployeeAssociationsBySearchTerm(EmployeeSearchBy searchBy, string searchTerm)
    {
        //Var for List to Return
        List<RosettaEmployeeAssociation> lEmployeeAssociations = new();

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

                //Initiate Http Client to Get Employee Association Information
                using(var client = new HttpClient())
                {
                    //Var for Bearer Token
                    string bearerToken = "Bearer " + _OAuth_Token;

                    //Add Required Header Values
                    client.DefaultRequestHeaders.Add("Authorization",bearerToken);

                    //Var for Employee Association Url
                    string employeeURL = Base_Url + "employee-association?"+ searchBy.ToString() + "=" + searchTerm + "&offset=" + nSrchRsltOffset.ToString() + "&limit=" + nSrchRsltLimit.ToString() + "&count=true";

                    //Get to People Endpoint
                    HttpResponseMessage response = client.GetAsync(employeeURL).Result;

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
                                using(JsonDocument jdEmplAssoc = JsonDocument.Parse(responsebody))
                                {

                                    //Access the Array at the Root
                                    JsonElement root = jdEmplAssoc.RootElement;

                                    //Iterate Through the Array
                                    foreach(JsonElement element in root.EnumerateArray())
                                    {

                                        //Add Rosetta Person to Returned People List
                                        lEmployeeAssociations.Add(ParseRosettaEmployeeAssocJson(element));

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

        return lEmployeeAssociations;
    }
    public RosettaDepartment ParseRosettaDepartmentJson(JsonElement jeDept)
    {
        //Intialize Rosetta Department to Return
        RosettaDepartment rosettaDept = new();

        //Retrieve Department ID
        if(jeDept.TryGetProperty("department_id",out JsonElement jeDepartmentID))
        {
            rosettaDept.Department_ID = jeDepartmentID.GetString() ?? "";
        }

        //Retrieve Department Title
        if(jeDept.TryGetProperty("department_title",out JsonElement jeDepartmentTitle))
        {
            rosettaDept.Department_Title = jeDepartmentTitle.GetString() ?? "";
        }

        //Retrieve Department Short Title
        if(jeDept.TryGetProperty("department_short_title",out JsonElement jeDepartmentShortTitle))
        {
            rosettaDept.Department_Short_Title = jeDepartmentShortTitle.GetString() ?? "";
        }

        //Retrieve Subdivision ID
        if(jeDept.TryGetProperty("subdivision_id",out JsonElement jeSubdivID))
        {
            rosettaDept.Subdivision_ID = jeSubdivID.GetString() ?? "";
        }

        //Retrieve Subdivision Title
        if(jeDept.TryGetProperty("subdivision_title",out JsonElement jeSubdivTitle))
        {
            rosettaDept.Subdivision_Title = jeSubdivTitle.GetString() ?? "";
        }

        //Retrieve Subdivision L4 ID
        if(jeDept.TryGetProperty("subdivision_l4_id",out JsonElement jeSubdivL4ID))
        {
            rosettaDept.Subdivision_L4_ID = jeSubdivL4ID.GetString() ?? "";
        }

        //Retrieve Subdivision L4 Title
        if(jeDept.TryGetProperty("subdivision_l4_title",out JsonElement jeSubdivL4Title))
        {
            rosettaDept.Subdivision_L4_Title = jeSubdivL4Title.GetString() ?? "";
        }

        //Retrieve Division ID
        if(jeDept.TryGetProperty("division_id",out JsonElement jeDivisionID))
        {
            rosettaDept.Division_ID = jeDivisionID.GetString() ?? "";
        }

        //Retrieve Division Title
        if(jeDept.TryGetProperty("division_title",out JsonElement jeDivisionTitle))
        {
            rosettaDept.Division_Title = jeDivisionTitle.GetString() ?? "";
        }

        //Retrieve Organization ID
        if(jeDept.TryGetProperty("organization_id",out JsonElement jeOrgID))
        {
            rosettaDept.Organization_ID = jeOrgID.GetString() ?? "";
        }

        //Retrieve Organization Title
        if(jeDept.TryGetProperty("organization_title",out JsonElement jeOrgTitle))
        {
            rosettaDept.Organization_Title = jeOrgTitle.GetString() ?? "";
        }

        return rosettaDept;
    }

    public List<RosettaDepartment> GetRosettaDepartments()
    {
        
        //Initialize List to Return
        List<RosettaDepartment> lDepartments = new();

        //Var for Result Limit
        int nRsltLimit = 2000;

        //Check OAuth Token
        if(CheckOAuthToken() == true)
        {
            //Initiate Http Client to Get Department Information
            using(var client = new HttpClient())
            {
                //Var for Bearer Token
                string bearerToken = "Bearer " + _OAuth_Token;

                //Add Required Header Values
                client.DefaultRequestHeaders.Add("Authorization",bearerToken);

                //Var for Departments Url
                string departmentsURL = Base_Url + "employee-association/departments?" + "limit=" + nRsltLimit.ToString();

                //Get to Employee Association Departments Endpoint
                HttpResponseMessage response = client.GetAsync(departmentsURL).Result;

                //Check Response Status Code
                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //Read Response Body
                    string responsebody = response.Content.ReadAsStringAsync().Result;

                    //Parse the Response Body
                    using(JsonDocument jdDepartments = JsonDocument.Parse(responsebody))
                    {

                        //Access the Array at the Root
                        JsonElement root = jdDepartments.RootElement;

                        //Iterate Through the Array
                        foreach(JsonElement element in root.EnumerateArray())
                        {
                            //Add Rosetta Department to Returned Departments List
                            lDepartments.Add(ParseRosettaDepartmentJson(element));

                        }//End of Root Enumerate Array

                    }//End Parse Response Body

                }//End of Status Code Check

            }

        }//End of Check OAuth Token

        return lDepartments;
    }

}