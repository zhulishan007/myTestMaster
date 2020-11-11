using System.Collections.Generic;

namespace MARS_Repository.ViewModel
{
    public class RolePrivilegesMappingModel
    {
        public RolePrivilegesMappingModel()
        {
            PrivilegesDataListModel = new List<PrivilegesDataModel>();
        }
        public long PrivilegeRoleMapId { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public List<PrivilegesDataModel> PrivilegesDataListModel { get; set; }
    }

    public class PrivilegesDataModel
    {
        public long PrivilegeId { get; set; }
        public string PrivilegeName { get; set; }
    }
}
