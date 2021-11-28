An index is like a ‘database’ in a relational database. It has a mapping which defines multiple types.

SQL Server => Databases => Tables => Columns/Rows
Elastic search => Indices => Types => Documents with Properties

These types hold multiple Documents (rows), and each document has Properties or Fields (columns).


To connect Elastic Search using C#, MVC, Web API, or ASP.NET Core, we use .NET Elastic client, i.e., NEST.

In the given example, I have created 2 projects under the same solution,

DBSetUp
It is a console application which will create your Index (Database) and Type (table) in the Elastic Search by using a common library.

MVC project (Web application)
To perform the crud operation in Elastic search, the application is created by using the common library.
NEST package should be added to both the projects.

While working with Elastic Search, please ensure that Elastic Search is installed on your system and running properly.  
Please have a look at the below screen as the Elastic Search Engine is running.
