using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class UserModel
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
        public string COMPANY_NAME { get; set; }
        public decimal? IsDeleted { get; set; }
        public decimal? UserMappingId { get; set; }
        public decimal? STATUS { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string RoleIds { get; set; }
    }

    public class LoginCookieModel
    {
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string Dbconnection { get; set; }
    }

    public class UserActiveModel
    {
        public decimal ACTIVE_ID { get; set; }
        public decimal USER_ID { get; set; }
        public string UserName { get; set; }
        public string PageName { get; set; }
    }

    public class TestCaseTestSuiteModel
    {
        public Nullable<long> TestSuiteId { get; set; }
        public string TestCaseName { get; set; }
    }
}
