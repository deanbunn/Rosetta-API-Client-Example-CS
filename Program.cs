using System.Text.Json;
using System.Reflection;
using UCDRosettaAPI;


//Submitted Argument Check
if(args.Length > 0 && string.IsNullOrEmpty(args[0]) == false)
{
    
    switch(args[0].ToLower())
    {
        case "people":
            PeopleSearching(args[1]);
            break;

        case "employee":
            EmployeeSearching(args[1]);
            break;

        case "department":
            ShowDepartments();
            break;

    }
}


//###############################
// People Searching
//###############################

static void PeopleSearching(string searchTerm)
{

    //Initiate Rosetta API Worker
    RosettaAPIWorker rosettaAPIWrkr = new();

    //Query People by Login ID
    List<RosettaPerson> lRosettaPeople = rosettaAPIWrkr.GetPeopleBySearchTerm(RosettaAPIWorker.PeopleSearchBy.loginid,searchTerm.Trim());

    //Loop Through Returned Rosetta People Listing
    foreach(RosettaPerson rosettaPrsn in lRosettaPeople)
    {

        //For Readability
        Console.WriteLine(" ");

        //Loop Through Rosetta Person Class and Display Each Property Value
        foreach (PropertyInfo property in rosettaPrsn.GetType().GetProperties())
        {
            if(property.Name != "lEmployeeAssociations" && property.Name != "lStudentAssociations")
            {
                Console.WriteLine($"{property.Name}: {property.GetValue(rosettaPrsn)}");
            }
            
        }

        //For Readability
        Console.WriteLine(" ");
        
        //Display Employee Associations
        if(rosettaPrsn.lEmployeeAssociations.Count > 0)
        {
            Console.WriteLine("Employee Associations:");
            Console.WriteLine(" ");

            foreach(RosettaEmployeeAssociation rea in rosettaPrsn.lEmployeeAssociations)
            {
                foreach(PropertyInfo reaProp in rea.GetType().GetProperties())
                {
                    Console.WriteLine($"{reaProp.Name}: {reaProp.GetValue(rea)}");
                }

                Console.WriteLine(" ");
            }

        }//End of Employee Associations
        
        //Display Student Associations
        if(rosettaPrsn.lStudentAssociations.Count > 0)
        {
            Console.WriteLine("Student Associations:");
            Console.WriteLine(" ");

            foreach(RosettaStudentAssociationShort rsa in rosettaPrsn.lStudentAssociations)
            {
                foreach(PropertyInfo rsaProp in rsa.GetType().GetProperties())
                {
                    Console.WriteLine($"{rsaProp.Name}: {rsaProp.GetValue(rsa)}");
                }

                Console.WriteLine(" ");
            }

        }//End of Student Associations

        //For Readability
        Console.WriteLine(" ");

    }//End of lRosettaPeople Listing
}


//###############################
// Employee-Associations
//###############################

static void EmployeeSearching(string searchTerm)
{
    Console.WriteLine("More codes");
}


//###############################
// Show Rosetta Departments
//###############################
static void ShowDepartments()
{
    //Initiate Rosetta API Worker
    RosettaAPIWorker rosettaAPIWrkr = new();

    List<RosettaDepartment> lRosettaDepartments = rosettaAPIWrkr.GetRosettaDepartments();

    foreach(RosettaDepartment rosettaDept in lRosettaDepartments)
    {

        //For Readability
        Console.WriteLine(" ");

        //Loop Through Rosetta Department Class and Display Each Property Value
        foreach(PropertyInfo deptProp in rosettaDept.GetType().GetProperties())
        {
            Console.WriteLine($"{deptProp.Name}: {deptProp.GetValue(rosettaDept)}");
        }

        //For Readability
        Console.WriteLine(" ");
    }

    Console.WriteLine("Departments Count: " + lRosettaDepartments.Count.ToString());

}


//############################
// Testing Snippets
//############################

//Wait for 4 Seconds
//await Task.Delay(TimeSpan.FromSeconds(4));  

// //Testing Loop 
// for(int i = 0; i < 1;i++)
// {
 

// }
