using Mars.Dto;
using MARS_Repository.Entities;
using MarsEngineSvc.basicReturnDataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MARS_Web.RESTfulApiClient
{
    public class MarsWebRESTfulApiClientExtend : MarsRESTfulApiclient
    {
        public List<T_DATA_SOURCEDTO> GetDataSource(ref bool isOk, ref string strError, ref string strStack)
        {
            string strMainURL = $"MarsEngine\\DataSource?currentDBIdx={currentdBIdx}";
            string strURL = this.BuildURL(strMainURL, ref isOk, ref strError);
            RESTfulDatasources dataSource = GetDataFromURL<RESTfulDatasources>(strURL, ref isOk, ref strError);
            if ((!isOk) || (dataSource == null)||(dataSource.DataSources==null))
            {
                return null;
            }

            isOk = true;            
            return dataSource.DataSources.ToList();
        }
        public MarsWebRESTfulApiClientExtend(string strDBIdx) : base(strDBIdx)
        {

        }
    }
}