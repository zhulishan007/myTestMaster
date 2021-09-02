using MARS_Repository.Entities;
using MARS_Repository.ViewModel;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARS_Repository.Repositories
{
    public class ChartRepository
    {
        Logger logger = LogManager.GetLogger("Log");
        Logger ELogger = LogManager.GetLogger("ErrorLog");
        DBEntities enty = Helper.GetMarsEntitiesInstance();
        public string Username = string.Empty;
        public ChartViewModel CreateThreeDChart(DataSet dataSet, int x_index, int y_index, int z_index)
        {
            ChartViewModel chartViewModel = new ChartViewModel();
            try
            {
                if (dataSet != null)
                {

                    DataTable dataTable = dataSet.Tables[0];
                    var dt_enum = dataTable.AsEnumerable();
                    var x_axis = (from r in dt_enum
                                  select r[x_index]).Distinct().ToArray();
                    var y_axis = (from r in dt_enum
                                  select r[y_index]).Distinct().ToArray();

                    object[,] z_axis = new object[y_axis.Length, x_axis.Length];


                    for (int y = 0; y < y_axis.Length; y++)
                    {
                        y_axis[y] = y_axis[y].ToString().Trim();
                        for (int x = 0; x < x_axis.Length; x++)
                        {
                            x_axis[x] = x_axis[x].ToString().Trim();
                            z_axis[y, x] = dt_enum.Where(s => s[x_index].ToString().Trim().Equals(x_axis[x].ToString().Trim()))
                                .Where(s => s[y_index].ToString().Trim().Equals(y_axis[y].ToString().Trim()))
                                .Select(s => s[z_index]).FirstOrDefault();
                            if (z_axis[y, x] == null)
                                z_axis[y, x] = 0;
                        }
                    }

                    chartViewModel.XAxis = x_axis;
                    chartViewModel.YAxis = y_axis;
                    chartViewModel.ZAxis = z_axis;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Chart for CreateThreeDChart method | DataSet : {0} | xIndex: {1} | yIndex : {2} | zIndex : {3} | Username : {4} ", dataSet, x_index, y_index, z_index, Username));
                ELogger.ErrorException(string.Format("Error occured in Chart for CreateThreeDChart method | DataSet : {0} | xIndex: {1} | yIndex : {2} | zIndex : {3} | Username : {4} ", dataSet, x_index, y_index, z_index, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Chart for CreateThreeDChart method | DataSet : {0} | xIndex: {1} | yIndex : {2} | zIndex : {3} | Username : {4} ", dataSet, x_index, y_index, z_index, Username), ex.InnerException);
                throw;
            }
            return chartViewModel;
        }

        public string CreateBarChart(DataSet dataSet, int x_index, int y_index, int z_index)
        {
            ChartViewModel chartViewModel = new ChartViewModel();
            string str_json = null;
            try
            {

                if (dataSet != null)
                {

                    DataTable dataTable = dataSet.Tables[0];
                    var dt_enum = dataTable.AsEnumerable();
                    var x_axis = (from r in dt_enum
                                  select r[x_index]).Distinct().ToArray();
                    var y_axis = (from r in dt_enum
                                  select r[y_index]).Distinct().ToArray();

                    object[,] z_axis = new object[x_axis.Length, y_axis.Length];
                    str_json = "[";
                    for (int x = 0; x < x_axis.Length; x++)
                    {
                        x_axis[x] = x_axis[x].ToString().Trim();
                        str_json += "{\"key\": \"" + x_axis[x] + "\", \"values\": [";
                        for (int y = 0; y < y_axis.Length; y++)
                        {
                            y_axis[y] = y_axis[y].ToString().Trim();
                            z_axis[x, y] = dt_enum.Where(s => s[x_index].ToString().Trim().Equals(x_axis[x].ToString().Trim()))
                                .Where(s => s[y_index].ToString().Trim().Equals(y_axis[y].ToString().Trim()))
                                .Select(s => s[z_index]).FirstOrDefault();
                            if (z_axis[x, y] != null)
                            {
                                str_json += "{\"key\": \"" + y_axis[y] + "\"" +
                                            ", \"value\": \"" + z_axis[x, y] + "\"},";
                            }

                        }
                        str_json = str_json.Substring(0, str_json.Length - 1);
                        str_json += "]},";
                    }
                    str_json = str_json.Substring(0, str_json.Length - 1);
                    str_json += "]";


                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Chart for CreateBarChart method | DataSet : {0} | xIndex: {1} | yIndex : {2} | zIndex : {3} | Username : {4} ", dataSet, x_index, y_index, z_index, Username));
                ELogger.ErrorException(string.Format("Error occured in Chart for CreateBarChart method | DataSet : {0} | xIndex: {1} | yIndex : {2} | zIndex : {3} | Username : {4} ", dataSet, x_index, y_index, z_index, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Chart for CreateBarChart method | DataSet : {0} | xIndex: {1} | yIndex : {2} | zIndex : {3} | Username : {4} ", dataSet, x_index, y_index, z_index, Username), ex.InnerException);
                throw;
            }
            return str_json;
        }

        public string CreatePieChart(DataSet dataSet, int x_index, int z_index)
        {
            //ChartViewModel chartViewModel = new ChartViewModel();
            string str_json = null;
            try
            {
                if (dataSet != null)
                {

                    DataTable dataTable = dataSet.Tables[0];
                    var dt_enum = dataTable.AsEnumerable();
                    var x_axis = (from r in dt_enum
                                  select r[x_index]).Distinct().ToArray();

                    // var y_axis = (from r in dt_enum
                    //              select r[1]).Distinct().ToArray();

                    var y_axis = new object();
                    str_json = "[";

                    for (int x = 0; x < x_axis.Length; x++)
                    {

                        x_axis[x] = x_axis[x].ToString().Trim();


                        //   y_axis[y] = y_axis[y].ToString().Trim();
                        y_axis = dt_enum.Where(s => s[x_index].ToString().Trim().Equals(x_axis[x].ToString().Trim()))
                            .Select(s => s[z_index]).FirstOrDefault();
                        if (y_axis != null)
                        {
                            str_json += "{\"name\": \"" + x_axis[x] + "\",  \"y\": " + y_axis + "},";
                        }
                    }
                    str_json = str_json.Substring(0, str_json.Length - 1);
                    str_json += "]";


                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Chart for CreatePieChart method | DataSet : {0} | xIndex: {1} | zIndex : {2} | Username : {3} ", dataSet, x_index, z_index, Username));
                ELogger.ErrorException(string.Format("Error occured in Chart for CreatePieChart method | DataSet : {0} |  xIndex: {1} | zIndex : {2} | Username : {3} ", dataSet, x_index, z_index, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Chart for CreatePieChart method | DataSet : {0} |  xIndex: {1} | zIndex : {2} | Username : {3} ", dataSet, x_index, z_index, Username), ex.InnerException);
                throw;
            }
            return str_json;
        }

        public string CreateBasicLineChart(DataSet dataSet, int x_index, int y_index, int axis_label)
        {
            //ChartViewModel chartViewModel = new ChartViewModel();
            string str_json = null;
            try
            {
                if (dataSet != null)
                {

                    DataTable dataTable = dataSet.Tables[0];
                    var dt_enum = dataTable.AsEnumerable();

                    var label_names = (from r in dt_enum
                                       select r[axis_label]).Distinct().ToArray();

                    var x_axis = (from r in dt_enum
                                  orderby r[x_index]
                                  select r[x_index]).Distinct().ToArray();

                    // var y_axis = (from r in dt_enum
                    //              select r[1]).Distinct().ToArray();

                    var y_axis = new object();
                    str_json = "[";

                    for (int l = 0; l < label_names.Length; l++)
                    {

                        label_names[l] = label_names[l].ToString().Trim();

                        str_json += "{\"name\": \"" + label_names[l] + "\", \"data\" : [";
                        //   y_axis[y] = y_axis[y].ToString().Trim();
                        foreach (var item_xaxis in x_axis)
                        {
                            y_axis = dt_enum.Where(s => s[x_index].ToString().Trim().Equals(item_xaxis.ToString().Trim()))
                            .Where(s => s[axis_label].ToString().Trim().Equals(label_names[l].ToString().Trim()))
                            .Select(s => s[y_index]).FirstOrDefault();
                            if (y_axis != null)
                            {
                                 str_json += y_axis.ToString().Replace("/", "//") + ",";
                            }
                            else
                            {
                                str_json += "0,";
                            }
                        }
                        str_json = str_json.Substring(0, str_json.Length - 1);


                        str_json += "]},";
                    }
                    str_json = str_json.Substring(0, str_json.Length - 1);
                    str_json += "]";


                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Chart for CreatePieChart method | DataSet : {0} | xIndex: {1} | zIndex : {2} | Username : {3} ", dataSet, x_index, y_index, Username));
                ELogger.ErrorException(string.Format("Error occured in Chart for CreatePieChart method | DataSet : {0} |  xIndex: {1} | zIndex : {2} | Username : {3} ", dataSet, x_index, y_index, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Chart for CreatePieChart method | DataSet : {0} |  xIndex: {1} | zIndex : {2} | Username : {3} ", dataSet, x_index, y_index, Username), ex.InnerException);
                throw;
            }
            return str_json;
        }

        public string CreateSplineChart(DataSet dataSet, int x_index, int y_index, int axis_label)
        {
            //ChartViewModel chartViewModel = new ChartViewModel();
            string str_json = null;
            try
            {
                if (dataSet != null)
                {

                    DataTable dataTable = dataSet.Tables[0];
                    var dt_enum = dataTable.AsEnumerable();

                    var label_names = (from r in dt_enum
                                       select r[axis_label]).Distinct().ToArray();

                    var x_axis = (from r in dt_enum
                                  orderby r[x_index]
                                  select r[x_index]).Distinct().ToArray();

                    var y_axis = new object();
                    str_json = "[";

                    for (int l = 0; l < label_names.Length; l++)
                    {

                        label_names[l] = label_names[l].ToString().Trim();

                        str_json += "{\"name\": " + label_names[l] + ", \"data\" :[";
                        //   y_axis[y] = y_axis[y].ToString().Trim();
                        foreach (DateTime item_xaxis in x_axis)
                        {

                            //str_json += "[Date.UTC(" + item_xaxis.Year + "," + item_xaxis.Month + "," + item_xaxis.Day + "),";
                            str_json += "[" + item_xaxis.ToUniversalTime() + ",";
                            y_axis = dt_enum.Where(s => s[x_index].ToString().Trim().Equals(item_xaxis.ToString().Trim()))
                            .Where(s => s[axis_label].ToString().Trim().Equals(label_names[l].ToString().Trim()))
                            .Select(s => s[y_index]).FirstOrDefault();
                            if (y_axis != null)
                            {
                                str_json += "\"" + y_axis.ToString().Replace("/", "//") + "\"],";
                            }
                            else
                            {
                                str_json += "\"0\"],";
                            }

                        }
                        str_json = str_json.Substring(0, str_json.Length - 1);


                        str_json += "]},";
                    }
                    str_json = str_json.Substring(0, str_json.Length - 1);
                    str_json += "]";


                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Chart for CreatePieChart method | DataSet : {0} | xIndex: {1} | zIndex : {2} | Username : {3} ", dataSet, x_index, y_index, Username));
                ELogger.ErrorException(string.Format("Error occured in Chart for CreatePieChart method | DataSet : {0} |  xIndex: {1} | zIndex : {2} | Username : {3} ", dataSet, x_index, y_index, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Chart for CreatePieChart method | DataSet : {0} |  xIndex: {1} | zIndex : {2} | Username : {3} ", dataSet, x_index, y_index, Username), ex.InnerException);
                throw;
            }
            return str_json;
        }

        public bool SaveAxisData(AxisDataViewModel axisDataViewModel)
        {
            try
            {
                logger.Info(string.Format("Add axis start | Query: {0} | Username: {1}", axisDataViewModel.queryId, Username));
                var flag = false;

                if (!CheckAxisDataExists(axisDataViewModel.queryId, axisDataViewModel.chartType))
                {
                    var RegisterTbl = new T_AXIS_LIST();
                    RegisterTbl.AXIS_LIST_ID = Helper.NextTestSuiteId("SEQ_T_AXIS_LIST"); ;
                    RegisterTbl.QUERY_ID = axisDataViewModel.queryId;
                    RegisterTbl.COMMAND_TYPE_ID = axisDataViewModel.chartType;
                    RegisterTbl.X_AXIS = axisDataViewModel.xAxis;
                    RegisterTbl.Y_AXIS = axisDataViewModel.yAxis;
                    RegisterTbl.Z_AXIS = axisDataViewModel.zAxis;
                    enty.T_AXIS_LIST.Add(RegisterTbl);
                    enty.SaveChanges();

                    flag = true;
                }
                else
                {
                    var RegisterTbl = enty.T_AXIS_LIST.Find(axisDataViewModel.axisId);
                    RegisterTbl.X_AXIS = axisDataViewModel.xAxis;
                    RegisterTbl.Y_AXIS = axisDataViewModel.yAxis;
                    RegisterTbl.Z_AXIS = axisDataViewModel.zAxis;
                    enty.SaveChanges();
                    flag = true;
                }
                logger.Info(string.Format("Add axis end | Query: {0} | Username: {1}", axisDataViewModel.queryId, Username));
                return flag;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Chart for SaveAxisData method | Query: {0} | Username: {1}", axisDataViewModel.queryId, Username));
                ELogger.ErrorException(string.Format("Error occured in Chart for SaveAxisData method | Query: {0} | Username: {1}", axisDataViewModel.queryId, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Chart for SaveAxisData method | Query: {0} | Username: {1}", axisDataViewModel.queryId, Username), ex.InnerException);
                throw;
            }
        }

        public AxisDataViewModel AxisDataExists(long? queryId, short? chartType)
        {
            try
            {
                logger.Info(string.Format("AxisDataExists start | Query: {0} | ChartType: {1} | Username: {2}", queryId, chartType, Username));
                AxisDataViewModel model = (from q in enty.T_AXIS_LIST
                                           where q.QUERY_ID == queryId
                                           where q.COMMAND_TYPE_ID == chartType
                                           select new AxisDataViewModel
                                           {
                                               axisId = q.AXIS_LIST_ID,
                                               queryId = q.QUERY_ID,
                                               chartType = q.COMMAND_TYPE_ID,
                                               xAxis = q.X_AXIS,
                                               yAxis = q.Y_AXIS,
                                               zAxis = q.Z_AXIS
                                           }).FirstOrDefault();

                logger.Info(string.Format("AxisDataExists end | Query: {0} | ChartType: {1} |Username: {2}", queryId, chartType, Username));
                return model;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Chart for AxisDataExists method | Query Id: {0} | Chart Type : {1} | Username: {1}", queryId, chartType, Username));
                ELogger.ErrorException(string.Format("Error occured in Chart for AxisDataExists method | Query Id: {0} | Chart Type : {1} | Username: {1}", queryId, chartType, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Chart for AxisDataExists method | Query Id: {0} | Chart Type : {1} | Username: {1}", queryId, chartType, Username), ex.InnerException);
                throw;
            }

        }

        //checks if data exists
        public bool CheckAxisDataExists(long? queryId, short? chartType)
        {
            try
            {
                logger.Info(string.Format("AxisDataExists start | Query: {0} | ChartType: {1} | Username: {2}", queryId, chartType, Username));
                bool result = enty.T_AXIS_LIST.Any(q => q.QUERY_ID == queryId && q.COMMAND_TYPE_ID == chartType);
                logger.Info(string.Format("AxisDataExists end | Query: {0} | ChartType: {1} |Username: {2}", queryId, chartType, Username));
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Chart for CheckAxisDataExists method | Query Id : {0} | Chart Type: {1} | UserName: {2}", queryId, chartType, Username));
                ELogger.ErrorException(string.Format("Error occured in Chart for CheckAxisDataExists method | Query Id : {0} | Chart Type: {1} | UserName: {2}", queryId, chartType, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Chart for CheckAxisDataExists method | Query Id : {0} | Chart Type: {1} | UserName: {2}", queryId, chartType, Username), ex.InnerException);
                throw;
            }

        }

        public string CreateBasicColumnChart(DataSet dataSet, int x_index, int y_index, int axis_label)
        {
            //ChartViewModel chartViewModel = new ChartViewModel();
            string str_json = null;
            try
            {
                if (dataSet != null)
                {

                    DataTable dataTable = dataSet.Tables[0];
                    var dt_enum = dataTable.AsEnumerable();

                    var label_names = (from r in dt_enum
                                       select r[axis_label]).Distinct().ToArray();

                    var x_axis = (from r in dt_enum
                                  orderby r[x_index]
                                  select r[x_index]).Distinct().ToArray();

                    var y_axis = new object();
                    str_json = "[";

                    //
                    //for (int l = 0; l < label_names.Length; l++)
                    //{

                    //    label_names[l] = label_names[l].ToString().Trim();

                    //    str_json += "{\"name\": " + label_names[l] + ", \"data\" :[";
                    //    //   y_axis[y] = y_axis[y].ToString().Trim();
                    //    foreach (DateTime item_xaxis in x_axis)
                    //    {

                    //        //str_json += "[Date.UTC(" + item_xaxis.Year + "," + item_xaxis.Month + "," + item_xaxis.Day + "),";
                    //        str_json += "[" + item_xaxis.ToUniversalTime() + ",";
                    //        y_axis = dt_enum.Where(s => s[x_index].ToString().Trim().Equals(item_xaxis.ToString().Trim()))
                    //        .Where(s => s[axis_label].ToString().Trim().Equals(label_names[l].ToString().Trim()))
                    //        .Select(s => s[y_index]).FirstOrDefault();
                    //        if (y_axis != null)
                    //        {
                    //            str_json += y_axis.ToString().Replace("/", "//") + "],";
                    //        }
                    //        else
                    //        {
                    //            str_json += "0],";
                    //        }

                    //    }
                    //    str_json = str_json.Substring(0, str_json.Length - 1);


                    //    str_json += "]},";
                    //}

                    for (int l = 0; l < x_axis.Length; l++)
                    {

                        x_axis[l] = x_axis[l].ToString().Trim();

                        str_json += "{\"name\": \"" + x_axis[l] + "\",\"data\" :[";
                        foreach (var item in label_names)
                        {
                            y_axis = dt_enum.Where(s => s[x_index].ToString().Trim().Equals(x_axis[l].ToString().Trim()))
                            .Where(s => s[axis_label].ToString().Trim().Equals(item.ToString().Trim()))
                            .Select(s => s[y_index]).FirstOrDefault();
                            if (y_axis != null)
                            {
                                str_json += y_axis.ToString().Replace("/", "//") + ",";
                            }
                            else
                            {
                                str_json += "0,";
                            }
                        }
                        str_json = str_json.Substring(0, str_json.Length - 1);
                        str_json += "]},";
                    }

                    str_json = str_json.Substring(0, str_json.Length - 1);
                    str_json += "]";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Chart for CreateBasicColumnChart method | DataSet : {0} | xIndex: {1} | zIndex : {2} | Username : {3} ", dataSet, x_index, y_index, Username));
                ELogger.ErrorException(string.Format("Error occured in Chart for CreateBasicColumnChart method | DataSet : {0} |  xIndex: {1} | zIndex : {2} | Username : {3} ", dataSet, x_index, y_index, Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Chart for CreateBasicColumnChart method | DataSet : {0} |  xIndex: {1} | zIndex : {2} | Username : {3} ", dataSet, x_index, y_index, Username), ex.InnerException);
                throw;
            }
            return str_json;
        }


        public string CreateBasicColumnChartcetegories(DataSet dataSet, int axis_label)
        {
            //ChartViewModel chartViewModel = new ChartViewModel();
            string str = String.Empty;
            try
            {
                if (dataSet != null)
                {

                    DataTable dataTable = dataSet.Tables[0];
                    var dt_enum = dataTable.AsEnumerable();

                    var label_names = (from r in dt_enum
                                       select r[axis_label]).Distinct().ToArray();

                    str = "[\"" + string.Join("\",\"", label_names) + "\"]";
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error occured in Chart for CreateBasicColumnChartcetegories method | DataSet : {0} | Username : {1} ", dataSet, Username));
                ELogger.ErrorException(string.Format("Error occured in Chart for CreateBasicColumnChartcetegories method | DataSet : {0} | Username : {1} ", dataSet,  Username), ex);
                if (ex.InnerException != null)
                    ELogger.ErrorException(string.Format("InnerException : Error occured in Chart for CreateBasicColumnChartcetegories method | DataSet : {0} | Username : {1} ", dataSet, Username), ex);
                throw;
            }
            return str;
        }

    }
}
