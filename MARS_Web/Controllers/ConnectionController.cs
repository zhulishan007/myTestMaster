using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MARS_Web.Controllers
{
    public class ConnectionController : Controller
    {
        // GET: Connection
        public ActionResult ConnectionList()
        {
            return PartialView("ConnectionList");
        }

        [HttpPost]
        public JsonResult DataLoad()
        {
            try
            {
                var repp = new AccountRepository();

                string search = Request.Form.GetValues("search[value]")[0];
                string draw = Request.Form.GetValues("draw")[0];
                string order = Request.Form.GetValues("order[0][column]")[0];
                string orderDir = Request.Form.GetValues("order[0][dir]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

                string colOrderIndex = Request.Form.GetValues("order[0][column]")[0];
                var colOrder = Request.Form.GetValues("columns[" + colOrderIndex + "][name]").FirstOrDefault();
                string colDir = Request.Form.GetValues("order[0][dir]")[0];

                var data = new List<DBconnectionViewModel>();

                data = repp.GetConnectionList();

                string UserNameSearch = Request.Form.GetValues("columns[0][search][value]")[0];
                string PasswordSearch = Request.Form.GetValues("columns[1][search][value]")[0];
                string HostSearch = Request.Form.GetValues("columns[2][search][value]")[0];
                string PortSearch = Request.Form.GetValues("columns[3][search][value]")[0];
                string SchemaSearch = Request.Form.GetValues("columns[4][search][value]")[0];
                string Service_NameSearch = Request.Form.GetValues("columns[5][search][value]")[0];
                string DatabasenameSearch = Request.Form.GetValues("columns[6][search][value]")[0];


                if (!string.IsNullOrEmpty(UserNameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.UserName) && x.UserName.ToLower().Trim().Contains(UserNameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(PasswordSearch))
                {
                    data = data.Where(p => !string.IsNullOrEmpty(p.Password) && p.Password.ToString().ToLower().Contains(PasswordSearch.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(HostSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Host) && x.Host.ToLower().Trim().Contains(HostSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(PortSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Port) && x.Port.ToLower().Trim().Contains(PortSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(SchemaSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Schema) && x.Schema.ToLower().Trim().Contains(SchemaSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(Service_NameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Service_Name) && x.Service_Name.ToLower().Trim().Contains(Service_NameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(DatabasenameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.Databasename) && x.Databasename.ToLower().Trim().Contains(DatabasenameSearch.ToLower().Trim())).ToList();
                }

                if (colDir == "desc")
                {
                    switch (colOrder)
                    {
                        case "UserName":
                            data = data.OrderByDescending(a => a.UserName).ToList();
                            break;
                        case "Password":
                            data = data.OrderByDescending(a => a.Password).ToList();
                            break;
                        case "Host":
                            data = data.OrderByDescending(a => a.Host).ToList();
                            break;
                        case "Port":
                            data = data.OrderByDescending(a => a.Port).ToList();
                            break;
                        case "Schema":
                            data = data.OrderByDescending(a => a.Schema).ToList();
                            break;
                        case "Service_Name":
                            data = data.OrderByDescending(a => a.Service_Name).ToList();
                            break;
                        case "Databasename":
                            data = data.OrderByDescending(a => a.Databasename).ToList();
                            break;
                        default:
                            data = data.OrderByDescending(a => a.UserName).ToList();
                            break;
                    }
                }
                else
                {
                    switch (colOrder)
                    {
                        case "UserName":
                            data = data.OrderByDescending(a => a.UserName).ToList();
                            break;
                        case "Password":
                            data = data.OrderByDescending(a => a.Password).ToList();
                            break;
                        case "Host":
                            data = data.OrderByDescending(a => a.Host).ToList();
                            break;
                        case "Port":
                            data = data.OrderByDescending(a => a.Port).ToList();
                            break;
                        case "Schema":
                            data = data.OrderByDescending(a => a.Schema).ToList();
                            break;
                        case "Service_Name":
                            data = data.OrderByDescending(a => a.Service_Name).ToList();
                            break;
                        case "Databasename":
                            data = data.OrderByDescending(a => a.Databasename).ToList();
                            break;
                        default:
                            data = data.OrderByDescending(a => a.UserName).ToList();
                            break;
                    }
                }

                int totalRecords = data.Count();

                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    data = data.Where(p => p.UserName.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Password.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Host.ToString().ToLower().Contains(search.ToLower())  ||
                    p.Port.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Schema.ToString().ToLower().Contains(search.ToLower()) || 
                    p.Service_Name.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Databasename.ToString().ToLower().Contains(search.ToLower())
                    ).ToList();
                }

                int recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();


                return Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = totalRecords,
                    recordsFiltered = recFilter,
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}