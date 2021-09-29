using MARS_Api.Helper;
using MARS_Repository.Repositories;
using MARS_Repository.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MARS_Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class EntitlementController : ApiController
    {
        //This method will load all the data and filter them
        [Route("api/UserRoleMappingDataLoad")]
        [AcceptVerbs("GET", "POST")]
        public BaseModel DataLoad([FromBody]SearchModel searchModel)
        {
            CommonHelper.SetConnectionString(Request);
            BaseModel baseModel = new BaseModel();
            var data = new List<UserRoleMappingViewModel>();
          
            try
            {
                int colOrderIndex = default(int);
                int recordsTotal = default(int);
                string colDir = string.Empty;
                var colOrder = string.Empty;
                string NameSearch = string.Empty;
                string RoleSearch = string.Empty;
                
                var repentil = new EntitlementRepository();

                string search = searchModel.search.value;
                var draw = searchModel.draw;
                if (searchModel.order.Any())
                {
                    string order = searchModel.order.FirstOrDefault().column.ToString();
                    string orderDir = searchModel.order.FirstOrDefault().dir.ToString();

                    colOrderIndex = searchModel.order.FirstOrDefault().column;
                    colDir = searchModel.order.FirstOrDefault().dir.ToString();
                }

                int startRec = searchModel.start;
                int pageSize = searchModel.length;

                if (searchModel.columns.Any())
                {
                    colOrder = searchModel.columns[colOrderIndex].name;

                    NameSearch = searchModel.columns[0].search.value;
                    RoleSearch = searchModel.columns[1].search.value;
                }
                data = repentil.ListOfUserRoleMapping();

              
                if (!string.IsNullOrEmpty(NameSearch))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.UserName) && x.UserName.ToLower().Trim().Contains(NameSearch.ToLower().Trim())).ToList();
                }
                if (!string.IsNullOrEmpty(RoleSearch))
                {
                    data = data.Where(p => !string.IsNullOrEmpty(p.Roles) && p.Roles.ToString().ToLower().Contains(RoleSearch.ToLower())).ToList();
                }
                if (colDir == "desc")
                {
                    switch (colOrder)
                    {
                        case "User Name":
                            data = data.OrderByDescending(a => a.UserName).ToList();
                            break;
                        case "Roles":
                            data = data.OrderByDescending(a => a.Roles).ToList();
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
                        case "User Name":
                            data = data.OrderBy(a => a.UserName).ToList();
                            break;
                        case "Roles":
                            data = data.OrderBy(a => a.Roles).ToList();
                            break;
                        default:
                            data = data.OrderBy(a => a.UserName).ToList();
                            break;
                    }
                }

                int totalRecords = data.Count();
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search   
                    data = data.Where(p => p.UserName.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Roles.ToString().ToLower().Contains(search.ToLower())
                    ).ToList();
                }
                int recFilter = data.Count();
                data = data.Skip(startRec).Take(pageSize).ToList();

                baseModel.data = data;
                baseModel.status = 1;
                baseModel.message = "Success";
                baseModel.recordsTotal = recordsTotal;
                baseModel.recordsFiltered = recFilter;
                baseModel.draw = draw;
            }
            catch (Exception ex)
            {
                baseModel.data = null;
                baseModel.status = 0;
                baseModel.message = "Error : " + ex.ToString();
            }

            return baseModel;
        }

        //Add/Update User Role objects values
        [Route("api/AddEditUserRoleMapping")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel AddEditUserRoleMapping(UserRoleMappingViewModel model)
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                var _etlrepository = new EntitlementRepository();
                var UserName = _etlrepository.GetUserName(model.UserId);
                var _addeditResult = _etlrepository.AddEditUserRoleMapping(model);
                resultModel.message = "Saved User [" + UserName + "] Role.";
                resultModel.data = _addeditResult;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //Delete the User Role object data by UserId
        [Route("api/DeleteUserRole")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel DeleteUserRole(long UserId)
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                var _etlrepository = new EntitlementRepository();
                var UserName = _etlrepository.GetUserName(UserId);
                var _deleteResult = _etlrepository.DeleteUserRole(UserId);

                resultModel.data = "success";
                resultModel.message = "User [" + UserName + "] Role has been deleted.";
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //Check Role already exist or not
        [Route("api/CheckExistOrNotUser")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel CheckExistOrNotUser(long UserId)
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                var _etlrepository = new EntitlementRepository();
                var result = _etlrepository.CheckExistOrNotUser(UserId);

                resultModel.data = result;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //This method will Create Role
        [Route("api/CreateRole")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel CreateRole(string RoleName)
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                var _etlrepository = new EntitlementRepository();
                var _existResult = _etlrepository.CheckRoleExist(RoleName);
                if (_existResult)
                {
                    resultModel.data = "warning";
                    resultModel.message = "Role [" + RoleName + "]  has already exists.";
                    resultModel.status = 1;
                    return resultModel;
                }
                var _createResult = _etlrepository.CreateRole(RoleName);

                resultModel.data = "success";
                resultModel.message = "Role [" + RoleName + "]  has been created.";
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //This Method Get all Role
        [Route("api/GetAllRoles")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel GetAllRoles()
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                AccountRepository repo = new AccountRepository();
                var allRoles = repo.GetAllRoles().Select(c => new System.Web.Mvc.SelectListItem { Text = c.ROLE_NAME, Value = c.ROLE_ID.ToString() }).OrderBy(x => x.Text).ToList(); ;

                resultModel.data = allRoles;
                resultModel.status = 1;
            }
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }

        //Add/Update PrivilageRoleMapping values
        [Route("api/AddEditPrivilageRoleMapping")]
        [AcceptVerbs("GET", "POST")]
        public ResultModel AddEditPrivilageRoleMapping(PrivilegeRoleMappingViewModel model)
        {
            CommonHelper.SetConnectionString(Request);
            ResultModel resultModel = new ResultModel();
            try
            {
                var _etlrepository = new EntitlementRepository();
                var RoleName = _etlrepository.GetRoleName(model.RoleId);
                var _addeditResult = _etlrepository.AddEditPrivilageRoleMapping(model);
                var _treerepository = new GetTreeRepository();
                resultModel.message = "Saved Privilege Role  [" + RoleName + "].";
                resultModel.data = _addeditResult;
                resultModel.status = 1;
            }  
            catch (Exception ex)
            {
                resultModel.status = 0;
                resultModel.message = ex.Message.ToString();
            }
            return resultModel;
        }
    }
}
