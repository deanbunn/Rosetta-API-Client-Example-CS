using System.Text.Json;
using System.Reflection;
using UCDRosettaAPI;


//Submitted Argument Check
if(args.Length > 0 && string.IsNullOrEmpty(args[0]) == false)
{
    
    switch(args[0].ToLower())
    {

        case "people-login":
            PeopleSearching(RosettaAPIWorker.PeopleSearchBy.loginid,args[1]);
            break;

        case "people-iam":
            PeopleSearching(RosettaAPIWorker.PeopleSearchBy.iamid,args[1]);
            break;

        case "people-employee":
            PeopleSearching(RosettaAPIWorker.PeopleSearchBy.employeeid,args[1]);
            break;

        case "people-student":
            PeopleSearching(RosettaAPIWorker.PeopleSearchBy.studentid,args[1]);
            break;

        case "people-department":
            PeopleSearching(RosettaAPIWorker.PeopleSearchBy.department,args[1]);
            break;

        case "employee-iam":
            EmployeeSearching(RosettaAPIWorker.EmployeeSearchBy.iamid,args[1]);
            break;

        case "employee-department":
            EmployeeSearching(RosettaAPIWorker.EmployeeSearchBy.departmentid,args[1]);
            break;

        case "employee-division":
            EmployeeSearching(RosettaAPIWorker.EmployeeSearchBy.divisionid,args[1]);
            break;

        case "employee-organization":
            EmployeeSearching(RosettaAPIWorker.EmployeeSearchBy.organizationid,args[1]);
            break;

        case "employee-subdivision":
            EmployeeSearching(RosettaAPIWorker.EmployeeSearchBy.subdivisionid,args[1]);
            break;

        case "employee-subdivisionl4":
            EmployeeSearching(RosettaAPIWorker.EmployeeSearchBy.subdivisionl4id,args[1]);
            break;

        case "departments":
            ShowDepartments();
            break;

    }
}
else
{
    ShowArgumentOptions();
}


//###############################
// People Searching
//###############################

static void PeopleSearching(RosettaAPIWorker.PeopleSearchBy peopleSearchBy,string searchTerm)
{

    //Initiate Rosetta API Worker
    RosettaAPIWorker rosettaAPIWrkr = new();

    //Query People by Search Parameters
    List<RosettaPerson> lRosettaPeople = rosettaAPIWrkr.GetPeopleBySearchTerm(peopleSearchBy,searchTerm.Trim());

    //Loop Through Returned Rosetta People Listing
    foreach(RosettaPerson rosettaPrsn in lRosettaPeople)
    {
        //For Readability
        Console.WriteLine(" ");
        Console.WriteLine("=========== " + rosettaPrsn.DisplayName + " =============");
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

        
    }//End of lRosettaPeople Listing
}


//###############################
// Employee-Associations
//###############################

static void EmployeeSearching(RosettaAPIWorker.EmployeeSearchBy employeeSearchBy,string searchTerm)
{

    //Initiate Rosetta API Worker
    RosettaAPIWorker rosettaAPIWrkr = new();

    //Query Employee Associations by Search Parameters
    List<RosettaEmployeeAssociation> lRosettaEmplAssocs = rosettaAPIWrkr.GetEmployeeAssociationsBySearchTerm(employeeSearchBy,searchTerm.Trim());

    //Loop Through Returned Rosetta Employee Associations
    foreach(RosettaEmployeeAssociation rosettaEmplAssoc in lRosettaEmplAssocs)
    {

        //For Readability
        Console.WriteLine(" ");

        //Loop Through Rosetta Employee Association Class and Display Each Property Value
        foreach(PropertyInfo reaProp in rosettaEmplAssoc.GetType().GetProperties())
        {
            Console.WriteLine($"{reaProp.Name}: {reaProp.GetValue(rosettaEmplAssoc)}");
        }

        //For Readability
        Console.WriteLine(" ");
    }

    Console.WriteLine("Employee Associations Count: " + lRosettaEmplAssocs.Count.ToString());

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

//###############################
// Display Agrument Options
//###############################
static void ShowArgumentOptions()
{
    Console.WriteLine(" ");
    Console.WriteLine("Argument Options:");
    Console.WriteLine("=======================");
    Console.WriteLine("people-login <userid>");
    Console.WriteLine("people-iam <iamid>");
    Console.WriteLine("people-employee <employeeid>");
    Console.WriteLine("people-student <studentid>");
    Console.WriteLine("people-department <departmentcode>");
    Console.WriteLine("employee-iam <iamid>");
    Console.WriteLine("employee-department <departmentcode>");
    Console.WriteLine("employee-division <divisionid>");
    Console.WriteLine("employee-organization <organizationid>");
    Console.WriteLine("employee-subdivision <subdivisionid>");
    Console.WriteLine("employee-subdivisionl4 <subdivisionl4id>");
    Console.WriteLine("departments");
    Console.WriteLine(" ");
    Console.WriteLine(" ");
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
