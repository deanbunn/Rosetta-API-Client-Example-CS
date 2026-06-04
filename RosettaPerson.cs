
namespace UCDRosettaAPI;

public class RosettaPerson
{
    public string? IAM_ID {get;set;}
    public string? Login_ID {get;set;}
    public string? Mothra_ID {get;set;}
    public string? Employee_ID {get;set;}
    public string? Mail_ID {get;set;}
    public string? Email_Primary {get;set;}
    public string? Email_Work {get;set;}
    public string? DisplayName {get;set;}
    public string ProvisioningStatus_Primary {get;set;}
    public string ProvisioningStatus_Employee {get;set;}
    public string ProvisioningStatus_Faculty {get;set;}
    public string ProvisioningStatus_Student {get;set;}
    public bool Affiliation_Employee {get;set;}
    public bool Affiliation_Faculty {get;set;}
    public bool Affiliation_Temporary_Affiliate {get;set;}
    public bool Affiliation_Student {get;set;}
    public bool Affiliation_Student_Applicant {get;set;}
    public bool Affiliation_Health_Affiliate {get;set;}
    public bool Employment_Is_Academic {get;set;}
    public bool Employment_Is_Academic_Senate {get;set;}
    public bool Employment_Is_Academic_Federation {get;set;}
    public bool Employment_Is_Faculty {get;set;}
    public bool Employment_Is_Teaching_Faculty {get;set;}
    public bool Employment_Is_Ladder_Rank {get;set;}
    public bool Employment_Is_Without_Salary {get;set;}
    public bool Employment_Is_MSP {get;set;}
    public bool Employment_Is_SSP {get;set;}
    public bool Employment_Is_Manager {get;set;}
    public bool Employment_Is_Campus_Employee {get;set;}
    public bool Employment_Is_Health_Employee {get;set;}
  
    
    public RosettaPerson()
    {
        
        ProvisioningStatus_Employee = string.Empty;
        ProvisioningStatus_Faculty = string.Empty;
        ProvisioningStatus_Primary = string.Empty;
        ProvisioningStatus_Student = string.Empty;
        Affiliation_Employee = false;
        Affiliation_Faculty = false;
        Affiliation_Temporary_Affiliate = false;
        Affiliation_Student = false;
        Affiliation_Student_Applicant = false;
        Affiliation_Health_Affiliate = false;
        Employment_Is_Academic = false;
        Employment_Is_Academic_Senate = false;
        Employment_Is_Academic_Federation = false;
        Employment_Is_Faculty = false;
        Employment_Is_Teaching_Faculty = false;
        Employment_Is_Ladder_Rank = false;
        Employment_Is_Without_Salary = false;
        Employment_Is_MSP = false;
        Employment_Is_SSP = false;
        Employment_Is_Manager = false;
        Employment_Is_Campus_Employee = false;
        Employment_Is_Health_Employee = false;

    }
    
}