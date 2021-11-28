using Common.Data;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elastic.DBSetUp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started creating database using Elastic Search");
            CreateIndex();
            CreateMappings();
            Console.WriteLine("ElasticSearch Database Created");
        }        

        //SQL Server => Databases => Tables => Columns / Rows
        //Elastic search => Indices => Types => Documents with Properties

        //In the given example, I have created 2 projects under the same solution, DBSetUp
        //It is a console application which will create your Index(Database)
        //and Type(table) in the Elastic Search by using a common library.
        //MVC project (Web application)
        //To perform the crud operation in Elastic search, the application is created by using the common library.
        //NEST package should be added to both the projects.
        //While working with Elastic Search,
        //please ensure that Elastic Search is installed on your system and running properly.
        //Please have a look at the below screen as the Elastic Search Engine is running.

        private static void CreateIndex()
        {
            const string elasticIndexName = "employeestore";
            ConnectionSettings settings = new ConnectionSettings(new Uri("http://localhost:9200"));
            settings.DefaultIndex(elasticIndexName);

            ElasticClient client = new ElasticClient(settings);
            client.Indices.Delete(elasticIndexName);
            
            var indexSettings = client.Indices.Exists(elasticIndexName);
            if (!indexSettings.Exists)
            {
                var response = client.Indices.Create(elasticIndexName);
            }
            else 
            {
                Console.WriteLine(elasticIndexName + "index already exist");
            }
        }

        private static void CreateMappings()
        {
            const string elasticIndexName = "employeestore";
            ConnectionSettings settings = new ConnectionSettings(new Uri("http://localhost:9200"));
            settings.DefaultIndex(elasticIndexName);
            ElasticClient esClient = new ElasticClient(settings);

            esClient.Map<Employee>(m =>
            {
                var putMappingDescriptor = m.Index(Indices.Index(elasticIndexName)).AutoMap();
                return putMappingDescriptor;
            });
        }


    }
}
