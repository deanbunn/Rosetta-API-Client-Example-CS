using System.Text.Json;
using UCDRosettaAPI;


//Initiate Rosetta API Worker
RosettaAPIWorker rosettaAPIWrkr = new();


//Testing Loop 
for(int i = 0; i < 25;i++)
{

    //Pull OAuth Token
    if(rosettaAPIWrkr.GetAPIToken())
    {
        //Display Expires 
        Console.WriteLine(rosettaAPIWrkr.Expires_in_Ticks.ToString());
    }
    else
    {
        Console.WriteLine("No bueno");
    }

    //Wait for One Minute
    //await Task.Delay(TimeSpan.FromMinutes(1));

}





