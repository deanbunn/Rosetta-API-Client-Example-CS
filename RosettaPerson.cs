
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
    public string? Provisioning_Status_Primary {get;set;}
    public string? Provisioning_Status_Employee {get;set;}
    public bool Affiliation_Employee {get;set;}
    public List<string> Employment_Status {get;set;}
    
    public RosettaPerson()
    {
        Employment_Status = [];
        Affiliation_Employee = false;
    }
    
}