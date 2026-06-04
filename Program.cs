using System.Text.Json;
using System.Reflection;
using UCDRosettaAPI;


//Initiate Rosetta API Worker
RosettaAPIWorker rosettaAPIWrkr = new();

//Testing Loop 
for(int i = 0; i < 1;i++)
{

    //Query People by Login ID
    List<RosettaPerson> lRosettaPeople = rosettaAPIWrkr.GetPeopleBySearchTerm(RosettaAPIWorker.SearchBy.loginid,"lmarcu");

    //Loop Through Returned Rosetta People Listing
    foreach(RosettaPerson rosettaPrsn in lRosettaPeople)
    {
        //For Readability
        Console.WriteLine(" ");

        //Loop Through Rosetta Person Class and Display Each Property Value
        foreach (PropertyInfo property in rosettaPrsn.GetType().GetProperties())
        {
            Console.WriteLine($"{property.Name}: {property.GetValue(rosettaPrsn)}");
        }

        //For Readability
        Console.WriteLine(" ");

    }//End of lRosettaPeople Listing

    // //Display List Count
    // Console.WriteLine(lRosettaPeople.Count.ToString());
    
    // //Display Expires 
    // Console.WriteLine("Expires in ticks: " + rosettaAPIWrkr.Expires_in_Ticks.ToString());
    
    // //Wait for 4 Seconds
    // await Task.Delay(TimeSpan.FromSeconds(4));    

}





