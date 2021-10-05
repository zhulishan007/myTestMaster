using MARS_Web.MarsUtility;
using MarsEngineSvc.basicReturnDataStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace MARS_Web.RESTfulApiClient
{

    
    public class MarsRESTfulApiclient
    {
        private static MarsLog Logger = MarsLog.GetLogger(Mars_LogType._normal, typeof(MarsRESTfulApiclient));
        public static string WebURLPrefix = null;

        protected string currentdBIdx=null;

        public MarsRESTfulApiclient(string strDBIdx)
        {
            currentdBIdx = strDBIdx;
        }

        protected string GetURLData(string strURLWithPara)
        {
            using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
            {
                var response = httpClient.GetAsync(strURLWithPara).GetAwaiter().GetResult();
                var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return result;
            }
        }


        public T GetDataFromURL<T>(string strURLWithPara, ref bool isOk, ref string strError)
        {
            try
            {
                string strData = GetURLData(strURLWithPara);
                return DeserializeToObjectFromResponseString<T>(strData, ref isOk, ref strError);
            }
            catch (Exception e)
            {
                isOk = false;
                strError = string.Format("Exception:[{0}]", e.Message);
                Console.WriteLine("\t{0}\r\n\t{1}", e.Message, e.StackTrace);
                return default(T);
            }

        }

        public RESTfulReturnObjects GetDataFromURL(string strURLWithPara, ref bool isOk, ref string strError)
        {
            try
            {
                string strData = GetURLData(strURLWithPara);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RESTfulReturnObjects));
                MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(strData));

                RESTfulReturnObjects rslt = (RESTfulReturnObjects)serializer.ReadObject(ms);
                if (rslt == null)
                {
                    isOk = false;
                    strError = string.Format("Can't get data from [{0}]", strURLWithPara);
                    return null;
                }
                if ((RESTfulObjectType)rslt.objectType == RESTfulObjectType.error_obj)
                {
                    isOk = false;
                    strError = string.Format("[{0}] return Error, with Error message:[{1}]",
                        strURLWithPara,
                        rslt.ReturnedMessage);
                    return rslt;
                }
                isOk = true;
                return rslt;
            }
            catch (Exception e)
            {
                isOk = false;
                strError = string.Format("Exception:[{0}]", e.Message);
                return null;
            }
        }

        protected string BuildURL(string strAPI, ref bool isOk, ref string strError)
        {
            try
            {
                Logger.LogBegin($"base url:[{WebURLPrefix[WebURLPrefix.Length - 1]}]");
                string strURLWithoutSlash = WebURLPrefix;
                if (WebURLPrefix[WebURLPrefix.Length - 1] != '/')
                {
                    strURLWithoutSlash = WebURLPrefix + "/";
                }
                isOk = true;
                return string.Format("{0}{1}", strURLWithoutSlash, strAPI);
            }
            catch (Exception e)
            {
                isOk = false;
                strError = e.Message;
                return "";
            }


        }

        public T DeserializeToObjectFromResponseString<T>(string strData, ref bool isOk, ref string strError)
        {
            Logger.LogBegin(string.IsNullOrEmpty(strData) ? "strData Len:null or empty" : $"strDataLen:{strData.Length}");
            try
            {
                var settings = new DataContractJsonSerializerSettings
                {
                    DateTimeFormat = new System.Runtime.Serialization.DateTimeFormat("s"),

                };

                var jsSrlzr = new System.Web.Script.Serialization.JavaScriptSerializer();

                jsSrlzr.MaxJsonLength = 200 * 1024 * 1024;  //10 M
                T rslt = jsSrlzr.Deserialize<T>(strData);

                //DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T), settings);
                //MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(strData));
                //T rslt = (T)serializer.ReadObject(ms);
                if (rslt == null)
                {
                    isOk = false;
                    strError = string.Format("Can't convert data to taget object -[{0}]", typeof(T));
                    Logger.Error("DeserializeToObjectFromResponseString", strError);
                    return default(T);
                }
                isOk = true;

                return rslt;
            }
            catch (Exception e)
            {
                Logger.Error(strData, e);
                isOk = false;
                strError = string.Format("Exception:[{0}]", e.Message);
                return default(T);
            }
            finally
            {
                Logger.LogEnd($"return {isOk}");
            }
        }

        protected T DoPut<T>(string strURLPart, T objToSend, ref bool isOk, ref string strError, bool isBSon = false, bool isDebug = false)
        {
            Logger.LogBegin(string.Format("url:{0}, isBSon:{1} object is:{2}", strURLPart, isBSon, objToSend == null ? "N/A" : objToSend.ToString()));
            string strDataReturned = "";
            try
            {
                string strURL = BuildURL(strURLPart, ref isOk, ref strError);
                if (!isOk)
                {
                    return default(T);
                }

                System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
                HttpResponseMessage rsp = null;

                if (isBSon)
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/bson"));
                    
                    System.Net.Http.Formatting.MediaTypeFormatter bsonFormatter = new System.Net.Http.Formatting.BsonMediaTypeFormatter();
                    //MemoryStream ms = new MemoryStream();
                    //BinaryFormatter bf = new BinaryFormatter();
                    //bf.Serialize(ms, objToSend);
                    //ByteArrayContent dataToPut = new ByteArrayContent(ms.ToArray());
                    rsp = httpClient.PostAsync(strURL, objToSend, bsonFormatter).GetAwaiter().GetResult();
                    string strData = rsp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    if (rsp.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        strError = rsp.Content.ToString();
                        isOk = false;
                        return default(T);
                    }
                    T rslt = DeserializeToObjectFromResponseString<T>(strData, ref isOk, ref strError);
                    if ((!isOk) || (rslt.Equals(default(T))))
                    {
                        isOk = false;
                        return default(T);
                    }
                    return rslt;
                }
                else
                {
                    //Logger.Error("doPut", "before Serialize");
                    string strJsonObj = (new System.Web.Script.Serialization.JavaScriptSerializer()).Serialize(objToSend);
                    if (isDebug)
                    {
                        Logger.Info("doPut", $"after JSon converted to:{strJsonObj}");
                    }
                    var httpContent = new StringContent(strJsonObj, Encoding.UTF8, "application/json");
                    if (isDebug)
                    {
                        Logger.Info("doPut", $"created StringContent:{httpClient}");
                    }
                    Logger.Info("doPut", "before PutAsync");
                    //try
                    //{
                    //    var tmp = httpClient.PutAsJsonAsync(strURL, objToSend);
                    //    Logger.Info("doPut test", $"PutAsJsonAsync data returns :{tmp}");
                    //}
                    //catch (Exception e)
                    //{
                    //    Logger.Error("doPut test", e.Message, e);
                    //}

                    //rsp = httpClient.PutAsync(strURL, httpContent).GetAwaiter().GetResult();
                    rsp = httpClient.PostAsync(strURL, httpContent).GetAwaiter().GetResult();
                    Logger.Info("\t", "ReadAsStringAsync before");
                    strDataReturned = rsp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Logger.Info("\t", "DeserializeToObjectFromResponseString before");
                    T rslt = DeserializeToObjectFromResponseString<T>(strDataReturned, ref isOk, ref strError);
                    if ((!isOk) || (rslt.Equals(default(T))))
                    {
                        isOk = false;
                        strError = string.IsNullOrEmpty(strError) ? "No object is returned" : strError;
                        Logger.Error("doPut", $"isOk:{isOk}, {strError}");
                        return default(T);
                    }
                    return rslt;
                }
            }
            catch (Exception e)
            {   
                Logger.Error($"data returned:{strDataReturned}", e);
                strError = string.Format("Exception:[{0}]", e.Message);
                isOk = false;
                return default(T);
            }
            finally
            {
                Logger.LogEnd($"returns {isOk}");
            }
        }
    }
}