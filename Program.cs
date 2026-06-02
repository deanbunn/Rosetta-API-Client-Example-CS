using System.Text.Json;
using UCDRosettaAPI;


//Initiate Rosetta API Worker
RosettaAPIWorker rosettaAPIWrkr = new();

//Testing Loop 
for(int i = 0; i < 1;i++)
{

    //Query People by Login ID
    List<RosettaPerson> lRosettaPeople = rosettaAPIWrkr.GetPeopleBySearchTerm(RosettaAPIWorker.SearchBy.department,"024070");

    foreach(RosettaPerson rosettaPrsn in lRosettaPeople)
    {
        Console.WriteLine(" ");

        //Show Display Name of Person
        Console.WriteLine(rosettaPrsn.DisplayName);
        Console.WriteLine(rosettaPrsn.IAM_ID);
        Console.WriteLine(rosettaPrsn.Provisioning_Status_Primary);
        Console.WriteLine(rosettaPrsn.Provisioning_Status_Employee);
        Console.WriteLine(rosettaPrsn.Affiliation_Employee.ToString());
        
        Console.WriteLine(" ");
    }

    // //Display List Count
    // Console.WriteLine(lRosettaPeople.Count.ToString());
    
    // //Display Expires 
    // Console.WriteLine("Expires in ticks: " + rosettaAPIWrkr.Expires_in_Ticks.ToString());
    
    // //Wait for 4 Seconds
    // await Task.Delay(TimeSpan.FromSeconds(4));    

}





