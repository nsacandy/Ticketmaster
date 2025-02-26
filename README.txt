----------------------------------------------------------------------------------

DATABASE ACCESS INSTRUCTIONS

Step 1: Verify that LocalDB is installed

Open cmd and run:

	sqllocaldb info

If installed, it will list instances like:

	MSSQLLocalDB

If MSSQLLocalDB isn't listed, start it via:

	sqllocaldb start MSSQLLocalDB

----------------------------------------------------------------------------------

Step 2: In Visual Studio's nuget package manager, run the following commands:

	Install-Package Microsoft.EntityFrameworkCore
	Install-Package Microsoft.EntityFrameworkCore.SqlServer
	Install-Package Microsoft.EntityFrameworkCore.Tools
	Install-Package Microsoft.EntityFrameworkCore.Design
	Install-Package Microsoft.EntityFrameworkCore.Relational

----------------------------------------------------------------------------------

Step 3: Verify the database exists

Open cmd as an admin and type:

	sqlcmd -S (localdb)\MSSQLLocalDB -E

Once the SQL query prompt comes up, type:

	SELECT name FROM sys.databases;
	GO

Make sure your database is listed. If not, open Nuget package manager in visual studio, 
and run the following command:

	Update-Database

----------------------------------------------------------------------------------

Step 4: Add yourself as an admin.

	sqlcmd -S (localdb)\MSSQLLocalDB -E

Once the SQL query prompt comes up, type:

	
	CREATE LOGIN [insert computer name here\insert computer user here] FROM WINDOWS;
	ALTER SERVER ROLE sysadmin ADD MEMBER [insert computer name here\insert computer user here];
	exit

----------------------------------------------------------------------------------

Step 5: Restart the DB

	sqllocaldb stop MSSQLLocalDB
	sqllocaldb start MSSQLLocalDB

----------------------------------------------------------------------------------
