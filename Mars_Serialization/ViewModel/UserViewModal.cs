using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mars_Serialization.ViewModel
{
    public class UserViewModal
    {
        public decimal TESTER_ID { get; set; }
        public string TESTER_NAME_LAST { get; set; }
        public string TESTER_NAME_M { get; set; }
        public string TESTER_NAME_F { get; set; }
        public string TESTER_LOGIN_NAME { get; set; }
        public string TESTER_PWD { get; set; }
        public Nullable<decimal> AVAILABLE_MARK { get; set; }
        public string TESTER_MAIL { get; set; }
        public string TESTER_NUMBER { get; set; }
        public decimal COMPANY_ID { get; set; }
        public string CREATOR_NAME { get; set; }
        public Nullable<System.DateTime> CREATE_TIME { get; set; }
        public string TESTER_DESC { get; set; }
        public string IS_DELETED { get; set; }
        public List<ProjectByUser> Projects { get; set; }
        public List<UserRoleViewModel> Roles { get; set; }
        public List<UserPrivilegeViewModel> Privileges { get; set; }
    }
    public class ProjectByUser
    {
        public string ProjectName { get; set; }
        public long ProjectId { get; set; }
        public string username { get; set; }
        public decimal userId { get; set; }
        public bool ProjectExists { get; set; }
        public string ProjectDesc { get; set; }
        public long TestSuiteCount { get; set; }
        public long StoryBoardCount { get; set; }
    }
    public class UserPrivilegeViewModel
    {
        public decimal PRIVILEGE_ID { get; set; }
        public string PRIVILEGE_NAME { get; set; }
        public string MODULE { get; set; }
        public string DESCRIPTION { get; set; }
    }
    public class UserRoleViewModel
    {
        public decimal ROLE_ID { get; set; }
        public string ROLE_NAME { get; set; }
    }
}
