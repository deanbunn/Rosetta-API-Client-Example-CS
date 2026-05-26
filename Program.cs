using System.Text.Json;
using UCDRosettaAPI;


//Initiate Rosetta API Worker
RosettaAPIWorker rosettaAPIWrkr = new();

//Testing Loop 
for(int i = 0; i < 25;i++)
{

    RosettaPerson rpTester = rosettaAPIWrkr.GetPersonBySearchTerm(RosettaAPIWorker.SearchBy.loginid,"dbunn");

    Console.WriteLine(rpTester.DisplayName);

    //Display Expires 
    Console.WriteLine(rosettaAPIWrkr.Expires_in_Ticks.ToString());
    
    //Wait for One Minute
    //await Task.Delay(TimeSpan.FromMinutes(1));

    //Wait for 10 Seconds
    await Task.Delay(TimeSpan.FromSeconds(10));    

}





