using System;
using System.Collections.Generic;

namespace MARS_Repository.ViewModel
{
    public class PrivilegeRoleMappingViewModel
    {
        public PrivilegeRoleMappingViewModel()
        {
            PrivilegeListModel = new List<PrivilegeMapModel>();
        }
        public long RoleId { get; set; }
        public string PrivilegeId { get; set; }
        public long PrivilegeRoleMappingId { get; set; }
        public List<PrivilegeMapModel> PrivilegeListModel { get; set; }
    }

    public class PrivilegeMapModel
    {
        public long PrivilegeId { get; set; }
        public Nullable<short> IsActive { get; set; }
    }

    public class RolePrivilegeMappingViewModel
    {
        public long? PrivilegeRoleMappingId { get; set; }
        public long? RoleId { get; set; }
        public long? PrivilegeId { get; set; }
        public string Name { get; set; }
        public string Module { get; set; }
        public string Desc { get; set; }
        public bool? Selected { get; set; }
    }
}
