using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using AvansApp.Models;
using AvansApp.ViewModels;
using AvansApp.Models.ServerModels;
using AvansApp.Models.Enums;

namespace AvansApp.Services.Pages
{
    public class EmployeeService
    {
        private const string base_url = "https://publicapi.avans.nl/oauth";


        public async Task<List<EmployeeVM>> GetEmployees(string name)
        {
            var _employees = new List<EmployeeVM>();

            List<Employee> result = await OAuth.GetInstance().RequestJSON<List<Employee>>(base_url + "/medewerkersgids/search/" + name, new List<string>(), HttpMethod.GET);

            if (result != null && result.Count > 0)
            {
                _employees = result.Select(e => new EmployeeVM(e)).ToList();
                //_employees = _employees.OrderBy(n => n.DisplayName).ToList(); // Do not order by name..
            }

            return _employees;
        }

        public async Task GetEmployeeImage(EmployeeVM employee)
        {
            if (employee == null)
                return;

            employee.ProfilePicture = new EmployeeImage() { path = "../Assets/StoreLogo.png" };
            /*EmployeeImage employeeImage = await OAuth.GetInstance().RequestJSON<EmployeeImage>(base_url + "/medewerkersgids/image/" + employee.Login, new List<string>(), HttpMethod.GET);

            if (employeeImage != null)
            {
                employee.ProfilePicture = employeeImage;
            }*/

            string userdetails = await OAuth.GetInstance().RequestRaw(base_url + "/medewerkersgids/image/" + employee.Login, new List<string>(), HttpMethod.GET);

            if (!string.IsNullOrEmpty(userdetails))
            {
                try
                {
                    userdetails = userdetails.Substring(userdetails.IndexOf('{'));

                    EmployeeImage employeeImage = Newtonsoft.Json.JsonConvert.DeserializeObject<EmployeeImage>(userdetails);
                    employee.ProfilePicture = employeeImage;

                }
                catch (System.Exception Error)
                {
                    System.Diagnostics.Debug.WriteLine(Error.Message);
                }
            }
        }
    }
}
