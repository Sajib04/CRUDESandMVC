using Common.Data;
using CRUDESandMVC.Models;
using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRUDESandMVC.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Registration(UserModel userModel)
        {
            const string elasticIndexName = "employeestore";

            //ConnectionSettings settings = new ConnectionSettings(new Uri("http://localhost:9200"));
            //settings.DefaultIndex(elasticIndexName);


            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
            var settings = new ConnectionSettings(pool)
                // configure the client with authentication credentials
                .BasicAuthentication("user", "password");
            var esClient = new ElasticClient(settings);





            //ElasticClient esClient = new ElasticClient(settings);
            Employee employee = new Employee
            {
                EmployeeID = userModel.ID,
                EmployeeName = userModel.Name,
                Address = userModel.Address
            };

            var response = await esClient.IndexAsync<Employee>(employee, i => i
                                              .Index("employeestore")
                                              //.GetType(TypeName.From<Employee>())
                                              .Id(userModel.ID)
                                              .Refresh(Elasticsearch.Net.Refresh.True));

            return RedirectToAction("GetUsers");
        }

        public async Task<ActionResult> GetUsers()
        {
            const string elasticIndexName = "employeestore";
            Employee employee = null;

            ConnectionSettings settings = new ConnectionSettings(new Uri("http://localhost:9200"));
            settings.DefaultIndex(elasticIndexName);

            ElasticClient esClient = new ElasticClient(settings);
            var response = await esClient.SearchAsync<Employee>(s => s.Query(q => q.MatchAll()));
            if (!response.IsValid)
            {
                throw new Exception("Elasticsearch response error. " + response.DebugInformation);
            }
            var employee1 = response.Hits.ToList();
            List<Employee> empList = new List<Employee>();
            foreach (var empItem in employee1)
            {
                employee = new Employee
                {
                    EmployeeID = empItem.Source.EmployeeID,
                    EmployeeName = empItem.Source.EmployeeName,
                    Address = empItem.Source.Address
                };

                empList.Add(employee);
            }

            List<UserModel> userList = new List<UserModel>();
            foreach (var item in empList)
            {
                UserModel um = new UserModel
                {
                    ID = item.EmployeeID,
                    Name = item.EmployeeName,
                    Address = item.Address
                };

                userList.Add(um);
            }

            return View(userList);
        }

        public async Task<ActionResult> Delete(int id)
        {
            const string elasticIndexName = "employeestore";
            ConnectionSettings settings = new ConnectionSettings(new Uri("http://localhost:9200"));
            settings.DefaultIndex(elasticIndexName);

            ElasticClient esClient = new ElasticClient(settings);
            var indexName = elasticIndexName;

            var deleteRequest = new Nest.DeleteRequest(indexName, id);
            deleteRequest.Refresh = Elasticsearch.Net.Refresh.True;

            var response = await esClient.DeleteAsync(new DocumentPath<Employee>(new Id(id)));
            if (!response.IsValid)
            {
                throw new Exception("Elasticsearch response error. " + response.DebugInformation);
            };

            //var response = esClient.DeleteByQuery<Employee>(q => q.Query );

            //            .Match(m => m.OnField(f => f.Guid).Equals(someObject.Guid))
            //);

    //        var deleteByQueryResponse = esClient.DeleteByQuery<object>(d => d
    //                    .Index(indexName).Type("Employee")
    //.Query(q => q
    //    .Term("orgId", "1234")
//    )
//);



            return RedirectToAction("GetUsers");
        }

        public async Task<ActionResult> Edit(int id)
        {
            const string elasticIndexName = "employeestore";
            Employee emp = null;

            ConnectionSettings settings = new ConnectionSettings(new Uri("http://localhost:9200"));
            settings.DefaultIndex(elasticIndexName);

            ElasticClient esClient = new ElasticClient(settings);


            var response = await esClient.SearchAsync<Employee>(s => s.Query(
                        q => q.Term(field => field.EmployeeID, id)
                ));
            if (!response.IsValid)
            {
                throw new Exception("Elasticsearch response error. " + response.DebugInformation);
            }
            
            if (response != null)
            {
                var employee = response.Hits.FirstOrDefault();
                emp = new Employee
                {
                    EmployeeID = employee.Source.EmployeeID,
                    EmployeeName = employee.Source.EmployeeName,
                    Address = employee.Source.Address
                };
            }

            UserModel um = new UserModel()
            {
                ID = emp.EmployeeID,
                Name = emp.EmployeeName,
                Address = emp.Address
            };

            return View(um);            
        }

        [HttpPost]
        public async Task<ActionResult> Edit(UserModel um)
        {
            const string elasticIndexName = "employeestore";
            ConnectionSettings settings = new ConnectionSettings(new Uri("http://localhost:9200"));
            settings.DefaultIndex(elasticIndexName);

            ElasticClient esClient = new ElasticClient(settings);
            Employee emp = new Employee
            {
                EmployeeID = um.ID,
                EmployeeName = um.Name,
                Address = um.Address
            };

            var response = await esClient.IndexAsync<Employee>(emp, i => i
                        .Index(elasticIndexName)
                        //.Type(TypeName.From<Employee>())
                        .Id(um.ID)
                        .Refresh(Elasticsearch.Net.Refresh.True)
            );
            return RedirectToAction("GetUsers");
        }

        public async Task<ActionResult> Details(int id)
        {
            const string elasticIndexName = "employeestore";
            Employee emp = null;

            ConnectionSettings settings = new ConnectionSettings(new Uri("http://localhost:9200"));
            settings.DefaultIndex(elasticIndexName);

            ElasticClient esClient = new ElasticClient(settings);
            var response = await esClient.SearchAsync<Employee>(s => s.Query(
                        q => q.Term(field => field.EmployeeID,  id)
                ));

            if (response != null)
            {
                var employee = response.Hits.FirstOrDefault();
                emp = new Employee
                {
                    EmployeeID = employee.Source.EmployeeID,
                    EmployeeName = employee.Source.EmployeeName,
                    Address = employee.Source.Address
                };
            }

            UserModel um = new UserModel()
            {
                ID = emp.EmployeeID,
                Name = emp.EmployeeName,
                Address = emp.Address
            };

            return View(um);
        }



    }
}