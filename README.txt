Ticketmaster Database Setup Instructions

Step 1: Verify LocalDB Installation

1. Open Command Prompt and run:

    sqllocaldb info

2. If installed, it should list instances like:

    MSSQLLocalDB

3. If MSSQLLocalDB isn't listed, start it with:

    sqllocaldb start MSSQLLocalDB

------------------------------------------------------------

Step 2: Install Required Packages

In Visual Studio's Package Manager Console, run:

    Install-Package Microsoft.EntityFrameworkCore
    Install-Package Microsoft.EntityFrameworkCore.SqlServer
    Install-Package Microsoft.EntityFrameworkCore.Tools
    Install-Package Microsoft.EntityFrameworkCore.Design
    Install-Package Microsoft.EntityFrameworkCore.Relational

------------------------------------------------------------

Step 3: Build the Database

1. In Visual Studio's Package Manager Console, run:

    dotnet ef migrations add InitialCreate
    dotnet ef database update

This will create the database and seed the admin user automatically.

------------------------------------------------------------

Step 4: Log in as Admin

Once the database is set up, use these credentials to log in:

    Email: admin@ticketmaster.com
    Password: AdminPass123

------------------------------------------------------------

That’s it! The database should be up and running, with the admin account ready for use.
