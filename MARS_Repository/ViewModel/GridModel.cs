using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class GridModel
    {
        public decimal GridId { get; set; }
        public string GridName { get; set; }
    }

    public class GridViewModel
    {
        public decimal GridId { get; set; }
        public string GridName { get; set; }
        public string GridColomn { get; set; }
        public string GridSize { get; set; }
    }

    public class AppGridWidthModel
    {
        public long GridId { get; set; }
        public long NameId { get; set; }
        public string Name { get; set; }
        public long DescriptionId { get; set; }
        public string Description { get; set; }
        public long VersionId { get; set; }
        public string Version { get; set; }
        public long ExtraRequirementId { get; set; }
        public string ExtraRequirement { get; set; }
        public long ModeId { get; set; }
        public string Mode { get; set; }
        public long ExplorerBitsId { get; set; }
        public string ExplorerBits { get; set; }
        public long ActionsId { get; set; }
        public string Actions { get; set; }
    }

    public class ProjectGridWidthModel
    {
        public long GridId { get; set; }
        public long NameId { get; set; }
        public string Name { get; set; }
        public long DescriptionId { get; set; }
        public string Description { get; set; }
        public long ApplicationId { get; set; }
        public string Application { get; set; }
        public long StatusId { get; set; }
        public string Status { get; set; }
        public long ActionsId { get; set; }
        public string Actions { get; set; }
    }

    public class KeywordGridWidthModel
    {
        public long GridId { get; set; }
        public long NameId { get; set; }
        public string Name { get; set; }
        public long ControlTypeId { get; set; }
        public string ControlType { get; set; }
        public long EntryDataId { get; set; }
        public string EntryData { get; set; }
        public long ActionsId { get; set; }
        public string Actions { get; set; }
    }

    public class TestSuiteGridWidthModel
    {
        public long GridId { get; set; }
        public long NameId { get; set; }
        public string Name { get; set; }
        public long DescriptionId { get; set; }
        public string Description { get; set; }
        public long ApplicationId { get; set; }
        public string Application { get; set; }
        public long ProjectId { get; set; }
        public string Project { get; set; }
        public long ActionsId { get; set; }
        public string Actions { get; set; }
    }

    public class TestCaseGridWidthModel
    {
        public long GridId { get; set; }
        public long NameId { get; set; }
        public string Name { get; set; }
        public long DescriptionId { get; set; }
        public string Description { get; set; }
        public long ApplicationId { get; set; }
        public string Application { get; set; }
        public long TestSuiteId { get; set; }
        public string TestSuite { get; set; }
        public long ActionsId { get; set; }
        public string Actions { get; set; }
    }

    public class ObjectGridWidthModel
    {
        public long GridId { get; set; }
        public long NameId { get; set; }
        public string Name { get; set; }
        public long InternalAccessId { get; set; }
        public string InternalAccess { get; set; }
        public long TypeId { get; set; }
        public string Type { get; set; }
        public long PegwindowId { get; set; }
        public string Pegwindow { get; set; }
        public long ActionsId { get; set; }
        public string Actions { get; set; }
        public long SelectId { get; set; }
        public string Select { get; set; }
    }

    public class VariableGridWidthModel
    {
        public long GridId { get; set; }
        public long NameId { get; set; }
        public string Name { get; set; }
        public long TypeId { get; set; }
        public string Type { get; set; }
        public long ValueId { get; set; }
        public string Value { get; set; }
        public long StatusId { get; set; }
        public string Status { get; set; }
        public long ActionsId { get; set; }
        public string Actions { get; set; }
    }

    public class UserGridWidthModel
    {
        public long GridId { get; set; }
        public long FNameId { get; set; }
        public string FName { get; set; }
        public long MNameId { get; set; }
        public string MName { get; set; }
        public long LNameId { get; set; }
        public string LName { get; set; }
        public long NameId { get; set; }
        public string Name { get; set; }
        public long EmailId { get; set; }
        public string Email { get; set; }
        public long CompanyId { get; set; }
        public string Company { get; set; }
        public long StatusId { get; set; }
        public string Status { get; set; }
        public long ActionsId { get; set; }
        public string Actions { get; set; }
    }

    public class TestcasePqGridWidthModel
    {
        public long GridId { get; set; }
        public long KeywordId { get; set; }
        public string Keyword { get; set; }
        public long ObjectId { get; set; }
        public string Object { get; set; }
        public long ParametersId { get; set; }
        public string Parameters { get; set; }
        public long CommentId { get; set; }
        public string Comment { get; set; }
    }

    public class StoryboardPqGridWidthModel
    {
        public long GridId { get; set; }
        public long ActionId { get; set; }
        public string Action { get; set; }
        public long StepsId { get; set; }
        public string Steps { get; set; }
        public long TestSuiteId { get; set; }
        public string TestSuite { get; set; }
        public long TestCaseId { get; set; }
        public string TestCase { get; set; }
        public long DatasetId { get; set; }
        public string Dataset { get; set; }
        public long BResultId { get; set; }
        public string BResult { get; set; }
        public long BErrorCauseId { get; set; }
        public string BErrorCause { get; set; }
        public long BScriptStartId { get; set; }
        public string BScriptStart { get; set; }
        public long BScriptDurationId { get; set; }
        public string BScriptDuration { get; set; }
        public long CResultId { get; set; }
        public string CResult { get; set; }
        public long CErrorCauseId { get; set; }
        public string CErrorCause { get; set; }
        public long CScriptStartId { get; set; }
        public string CScriptStart { get; set; }
        public long CScriptDurationId { get; set; }
        public string CScriptDuration { get; set; }
        public long DependencyId { get; set; }
        public string Dependency { get; set; }
        public long DescriptionId { get; set; }
        public string Description { get; set; }
    }

    public class LeftPanelPqGridWidthModel
    {
        public long GridId { get; set; }
        public long ResizeId { get; set; }
        public string Resize { get; set; }
       
    }

    public class GridNameList
    {
        public const string ApplicationList = "Application List";
        public const string ProjectList = "Project List";
        public const string TestSuiteList = "Test Suite List";
        public const string TestCaseList = "Test Case List";
        public const string VaribleList = "Variable List";
        public const string KeywordList = "Keyword List";
        public const string ObjectList = "Object List";
        public const string UserList = "User List";
        public const string TestCasePage = "Test Case Page";
        public const string StoryboradPage = "Storyboard Page";
        public const string ResizeLeftPanel = "Resize Left Panel";
    }
}
