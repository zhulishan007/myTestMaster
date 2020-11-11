using MARS_Repository.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MARS_Web.Helper
{
    public static class GridHelper
    {
        public static AppGridWidthModel GetApplicationwidth(List<GridViewModel> grids, long Id = 0)
        {
            AppGridWidthModel appGridWidth = new AppGridWidthModel();
            appGridWidth.GridId = Id;
            foreach (var item in grids)
            {
                if (item.GridColomn.Trim() == "Name")
                {
                    appGridWidth.NameId = (long)item.GridId;
                    appGridWidth.Name = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Description")
                {
                    appGridWidth.DescriptionId = (long)item.GridId;
                    appGridWidth.Description = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Version")
                {
                    appGridWidth.VersionId = (long)item.GridId;
                    appGridWidth.Version = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Extra Requirement")
                {
                    appGridWidth.ExtraRequirementId = (long)item.GridId;
                    appGridWidth.ExtraRequirement = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Mode")
                {
                    appGridWidth.ModeId = (long)item.GridId;
                    appGridWidth.Mode = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "MARS Explorer Bits")
                {
                    appGridWidth.ExplorerBitsId = (long)item.GridId;
                    appGridWidth.ExplorerBits = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Actions")
                {
                    appGridWidth.ActionsId = (long)item.GridId;
                    appGridWidth.Actions = item.GridSize.Trim();
                }
            }
            return appGridWidth;
        }

        public static ProjectGridWidthModel GetProjectwidth(List<GridViewModel> grids, long Id = 0)
        {
            ProjectGridWidthModel proGridWidth = new ProjectGridWidthModel();
            proGridWidth.GridId = Id;
            foreach (var item in grids)
            {
                if (item.GridColomn.Trim() == "Name")
                {
                    proGridWidth.NameId = (long)item.GridId;
                    proGridWidth.Name = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Description")
                {
                    proGridWidth.DescriptionId = (long)item.GridId;
                    proGridWidth.Description = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Application")
                {
                    proGridWidth.ApplicationId = (long)item.GridId;
                    proGridWidth.Application = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Status")
                {
                    proGridWidth.StatusId = (long)item.GridId;
                    proGridWidth.Status = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Actions")
                {
                    proGridWidth.ActionsId = (long)item.GridId;
                    proGridWidth.Actions = item.GridSize.Trim();
                }
            }
            return proGridWidth;
        }

        public static KeywordGridWidthModel GetKeywordwidth(List<GridViewModel> grids, long Id = 0)
        {
            KeywordGridWidthModel keywordGridWidth = new KeywordGridWidthModel();
            keywordGridWidth.GridId = Id;
            foreach (var item in grids)
            {
                if (item.GridColomn.Trim() == "Name")
                {
                    keywordGridWidth.NameId = (long)item.GridId;
                    keywordGridWidth.Name = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Control Type")
                {
                    keywordGridWidth.ControlTypeId = (long)item.GridId;
                    keywordGridWidth.ControlType = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Entry in Data File")
                {
                    keywordGridWidth.EntryDataId = (long)item.GridId;
                    keywordGridWidth.EntryData = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Actions")
                {
                    keywordGridWidth.ActionsId = (long)item.GridId;
                    keywordGridWidth.Actions = item.GridSize.Trim();
                }
            }
            return keywordGridWidth;
        }

        public static TestSuiteGridWidthModel GetTestSuitewidth(List<GridViewModel> grids, long Id = 0)
        {
            TestSuiteGridWidthModel TSGridWidth = new TestSuiteGridWidthModel();
            TSGridWidth.GridId = Id;
            foreach (var item in grids)
            {
                if (item.GridColomn.Trim() == "Name")
                {
                    TSGridWidth.NameId = (long)item.GridId;
                    TSGridWidth.Name = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Description")
                {
                    TSGridWidth.DescriptionId = (long)item.GridId;
                    TSGridWidth.Description = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Application")
                {
                    TSGridWidth.ApplicationId = (long)item.GridId;
                    TSGridWidth.Application = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Project")
                {
                    TSGridWidth.ProjectId = (long)item.GridId;
                    TSGridWidth.Project = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Actions")
                {
                    TSGridWidth.ActionsId = (long)item.GridId;
                    TSGridWidth.Actions = item.GridSize.Trim();
                }
            }
            return TSGridWidth;
        }

        public static TestCaseGridWidthModel GetTestCasewidth(List<GridViewModel> grids, long Id = 0)
        {
            TestCaseGridWidthModel objGridWidth = new TestCaseGridWidthModel();
            objGridWidth.GridId = Id;
            foreach (var item in grids)
            {
                if (item.GridColomn.Trim() == "Name")
                {
                    objGridWidth.NameId = (long)item.GridId;
                    objGridWidth.Name = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Description")
                {
                    objGridWidth.DescriptionId = (long)item.GridId;
                    objGridWidth.Description = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Application")
                {
                    objGridWidth.ApplicationId = (long)item.GridId;
                    objGridWidth.Application = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "TestSuite")
                {
                    objGridWidth.TestSuiteId = (long)item.GridId;
                    objGridWidth.TestSuite = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Actions")
                {
                    objGridWidth.ActionsId = (long)item.GridId;
                    objGridWidth.Actions = item.GridSize.Trim();
                }
            }
            return objGridWidth;
        }

        public static ObjectGridWidthModel GetObjectwidth(List<GridViewModel> grids, long Id = 0)
        {
            ObjectGridWidthModel objGridWidth = new ObjectGridWidthModel();
            objGridWidth.GridId = Id;
            foreach (var item in grids)
            {
                if (item.GridColomn.Trim() == "Object Name")
                {
                    objGridWidth.NameId = (long)item.GridId;
                    objGridWidth.Name = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Internal Access")
                {
                    objGridWidth.InternalAccessId = (long)item.GridId;
                    objGridWidth.InternalAccess = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Type")
                {
                    objGridWidth.TypeId = (long)item.GridId;
                    objGridWidth.Type = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Parent Pegwindow")
                {
                    objGridWidth.PegwindowId = (long)item.GridId;
                    objGridWidth.Pegwindow = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Actions")
                {
                    objGridWidth.ActionsId = (long)item.GridId;
                    objGridWidth.Actions = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Select All")
                {
                    objGridWidth.SelectId = (long)item.GridId;
                    objGridWidth.Select = item.GridSize.Trim();
                }
            }
            return objGridWidth;
        }

        public static VariableGridWidthModel GetVariblewidth(List<GridViewModel> grids, long Id = 0)
        {
            VariableGridWidthModel variableGridWidth = new VariableGridWidthModel();
            variableGridWidth.GridId = Id;
            foreach (var item in grids)
            {
                if (item.GridColomn.Trim() == "Name")
                {
                    variableGridWidth.NameId = (long)item.GridId;
                    variableGridWidth.Name = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Type")
                {
                    variableGridWidth.TypeId = (long)item.GridId;
                    variableGridWidth.Type = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Value")
                {
                    variableGridWidth.ValueId = (long)item.GridId;
                    variableGridWidth.Value = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Status")
                {
                    variableGridWidth.StatusId = (long)item.GridId;
                    variableGridWidth.Status = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Actions")
                {
                    variableGridWidth.ActionsId = (long)item.GridId;
                    variableGridWidth.Actions = item.GridSize.Trim();
                }
            }
            return variableGridWidth;
        }

        public static UserGridWidthModel GetUserwidth(List<GridViewModel> grids, long Id = 0)
        {
            UserGridWidthModel userGridWidth = new UserGridWidthModel();
            userGridWidth.GridId = Id;
            foreach (var item in grids)
            {
                if (item.GridColomn.Trim() == "First Name")
                {
                    userGridWidth.FNameId = (long)item.GridId;
                    userGridWidth.FName = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Middle Name")
                {
                    userGridWidth.MNameId = (long)item.GridId;
                    userGridWidth.MName = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Last Name")
                {
                    userGridWidth.LNameId = (long)item.GridId;
                    userGridWidth.LName = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "User Name")
                {
                    userGridWidth.NameId = (long)item.GridId;
                    userGridWidth.Name = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Email Address")
                {
                    userGridWidth.EmailId = (long)item.GridId;
                    userGridWidth.Email = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Company Name")
                {
                    userGridWidth.CompanyId = (long)item.GridId;
                    userGridWidth.Company = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Status")
                {
                    userGridWidth.StatusId = (long)item.GridId;
                    userGridWidth.Status = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Actions")
                {
                    userGridWidth.ActionsId = (long)item.GridId;
                    userGridWidth.Actions = item.GridSize.Trim();
                }
            }
            return userGridWidth;
        }

        public static TestcasePqGridWidthModel GetTestCasePqgridwidth(List<GridViewModel> grids, long Id = 0)
        {
            TestcasePqGridWidthModel TCPGridWidth = new TestcasePqGridWidthModel();
            TCPGridWidth.GridId = Id;
            foreach (var item in grids)
            {
                if (item.GridColomn.Trim() == "Keyword")
                {
                    TCPGridWidth.KeywordId = (long)item.GridId;
                    TCPGridWidth.Keyword = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Object")
                {
                    TCPGridWidth.ObjectId = (long)item.GridId;
                    TCPGridWidth.Object = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Parameters")
                {
                    TCPGridWidth.ParametersId = (long)item.GridId;
                    TCPGridWidth.Parameters = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Comment")
                {
                    TCPGridWidth.CommentId = (long)item.GridId;
                    TCPGridWidth.Comment = item.GridSize.Trim();
                }
            }
            return TCPGridWidth;
        }

        public static StoryboardPqGridWidthModel GetStoryboardPqgridwidth(List<GridViewModel> grids, long Id = 0)
        {
            StoryboardPqGridWidthModel SPGridWidth = new StoryboardPqGridWidthModel();
            SPGridWidth.GridId = Id;
            foreach (var item in grids)
            {
                if (item.GridColomn.Trim() == "Action")
                {
                    SPGridWidth.ActionId = (long)item.GridId;
                    SPGridWidth.Action = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Steps")
                {
                    SPGridWidth.StepsId = (long)item.GridId;
                    SPGridWidth.Steps = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Test Suite")
                {
                    SPGridWidth.TestSuiteId = (long)item.GridId;
                    SPGridWidth.TestSuite = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Test Case")
                {
                    SPGridWidth.TestCaseId = (long)item.GridId;
                    SPGridWidth.TestCase = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Dataset")
                {
                    SPGridWidth.DatasetId = (long)item.GridId;
                    SPGridWidth.Dataset = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Base Line Data Result")
                {
                    SPGridWidth.BResultId = (long)item.GridId;
                    SPGridWidth.BResult = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Base Line Data Error cause")
                {
                    SPGridWidth.BErrorCauseId = (long)item.GridId;
                    SPGridWidth.BErrorCause = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Base Line Data Script Start")
                {
                    SPGridWidth.BScriptStartId = (long)item.GridId;
                    SPGridWidth.BScriptStart = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Base Line Data Script Duration")
                {
                    SPGridWidth.BScriptDurationId = (long)item.GridId;
                    SPGridWidth.BScriptDuration = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Comparison Line Data Result")
                {
                    SPGridWidth.CResultId = (long)item.GridId;
                    SPGridWidth.CResult = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Comparison Line Data Error cause")
                {
                    SPGridWidth.CErrorCauseId = (long)item.GridId;
                    SPGridWidth.CErrorCause = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Comparison Line Data Script Start")
                {
                    SPGridWidth.CScriptStartId = (long)item.GridId;
                    SPGridWidth.CScriptStart = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Comparison Line Data Script Duration")
                {
                    SPGridWidth.CScriptDurationId = (long)item.GridId;
                    SPGridWidth.CScriptDuration = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Dependency")
                {
                    SPGridWidth.DependencyId = (long)item.GridId;
                    SPGridWidth.Dependency = item.GridSize.Trim();
                }
                else if (item.GridColomn.Trim() == "Description")
                {
                    SPGridWidth.DescriptionId = (long)item.GridId;
                    SPGridWidth.Description = item.GridSize.Trim();
                }
            }
            return SPGridWidth;
        }

        public static LeftPanelPqGridWidthModel GetLeftpanelgridwidth(List<GridViewModel> grids, long Id = 0)
        {
            LeftPanelPqGridWidthModel LRModel = new LeftPanelPqGridWidthModel();
            LRModel.GridId = Id;
            foreach (var item in grids)
            {
                if (item.GridColomn.Trim() == "Resize Left Panel")
                {
                    LRModel.ResizeId = (long)item.GridId;
                    LRModel.Resize = item.GridSize.Trim();
                }
            }
            return LRModel;
        }
    }
}