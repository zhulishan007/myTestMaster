using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.ViewModel
{
    public class ObjectList
    {
        public decimal ObjectId { get; set; }
        public string ObjectName { get; set; }
    }
    public class Objects
    {
        public decimal ObjectId { get; set; }
        public string ObjectName { get; set; }
        public string ObjectType { get; set; }
        public string Quickaccess { get; set; }
        public string ObjectParent { get; set; }
        public string EnumType { get; set; }
        public long applicationid { get; set; }
        public short? autocheckerror { get; set; }
        public string description { get; set; }
        public string Sql { get; set; }
        public int Totalcount { get; set; }
    }
    public class objectType
    {
        public long typeid { get; set; }
        public string typename { get; set; }
    }
  public class ObjectModel
    {
        public decimal ObjectId { get; set; }
        public string ObjectName { get; set; }
        public long? ObjectType { get; set; }
        public string ObjectTypeName { get; set; }
        public long applicationid { get; set; }
        public string Quickaccess { get; set; }
        public string ObjectParent { get; set; }
        public string description { get; set; }
        public string EnumType { get; set; }
        public short? autocheckerror { get; set; }
    }

    public class ObjectExportModel
    {
        public string OBJECTNAME { get; set; }
        public string TYPE { get; set; }
        public string QUICK_ACCESS { get; set; }
        public string PARENT { get; set; }
        public string COMMENT { get; set; }
        public string ENUM_TYPE { get; set; }
        public string SQL { get; set; }
    }
}
