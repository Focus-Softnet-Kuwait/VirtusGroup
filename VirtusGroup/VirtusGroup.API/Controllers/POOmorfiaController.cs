using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using VirtusGroup.API.Auth;
using VirtusGroup.API.Models;
using static VirtusGroup.API.Models.ApiResponse;
namespace VirtusGroup.API.Controllers
{
    public class POOmorfiaController : ApiController
    {
        public string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public string ServerIp = ConfigurationManager.AppSettings["serverIp"].ToString();
        public string Username = ConfigurationManager.AppSettings["username"].ToString();
        public string Password = ConfigurationManager.AppSettings["password"].ToString();
        public string CompanyCode = ConfigurationManager.AppSettings["companyCode"].ToString();
        
        public string GetSessionId(string company, string user, string pass)
        {
            try
            {
                HashData objHashRequest = new HashData();
                Hashtable objHashLogin = new Hashtable();

                objHashLogin.Add("Username", Username);
                objHashLogin.Add("Password", Password);
                objHashLogin.Add("CompanyId", company);

                List<Hashtable> lstHash = new List<Hashtable>();
                lstHash.Add(objHashLogin);

                objHashRequest.data = lstHash;
                objHashRequest.message = "";
                objHashRequest.result = 1;

                string sContent = JsonConvert.SerializeObject(objHashRequest);
                var serverIp = ServerIp;
                HashData objHashResponse = new HashData();
                using (var client = new WebClient())
                {
                    client.Headers.Add("Content-Type", "application/json");
                    string sUrl = "Http://" + serverIp + "/Focus8API/login";
                    var strResponse = client.UploadString(sUrl, sContent);
                    WriteLog.writeLog("Session ID: " + strResponse);
                    objHashResponse = JsonConvert.DeserializeObject<HashData>(strResponse);
                }
                return objHashResponse.data[0]["fSessionId"].ToString();
            }
            catch (Exception)
            {


                throw;
            }
        }
        private int Convertdate(DateTime Dt)
        {
            return ((Convert.ToDateTime(Dt).Year) * 65536) + (Convert.ToDateTime(Dt).Month) * 256 + (Convert.ToDateTime(Dt).Day);
        }
        private int ConvertTime(DateTime Dt)
        {
            return (Dt.Hour * 65536) + (Dt.Minute * 256) + Dt.Second;
        }
        public static bool IsNumeric(char o)
        {
            double result;
            //return o != null && Double.TryParse(o.ToString(), out result);
            return Double.TryParse(o.ToString(), out result);
        }
        private static int getCompCodeVal(char cCode)
        {
            int iRet = 0;
            char[] sLetters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            for (int i = 0; i < sLetters.Length; i++)
            {
                if (sLetters[i] == cCode)
                {
                    iRet = i;
                    break;
                }
            }
            return iRet + 10;
        }
        public static int GetCompID(string CompCode)
        {
            int iRet = 0;
            string sCompCode = CompCode;
            if (IsNumeric(sCompCode[0]))
            {
                iRet = (36 * 36) * int.Parse(sCompCode[0].ToString());
            }
            else
            {
                iRet = (36 * 36) * getCompCodeVal(sCompCode[0]);
            }
            if (IsNumeric(sCompCode[1]))
            {
                iRet += (36) * int.Parse(sCompCode[1].ToString());
            }
            else
            {
                iRet += (36) * getCompCodeVal(sCompCode[1]);
            }

            if (IsNumeric(sCompCode[2]))
            {
                iRet += (36 * 0) * int.Parse(sCompCode[2].ToString());
            }
            else
            {
                iRet += (36 * 0) * getCompCodeVal(sCompCode[2]);
            }
            return iRet;

        }

        //[BasicAuth]
        [Authorize]
        [HttpPost]
        public IHttpActionResult PostPOOmorfia([FromBody] POOmorfia jvo)
        {
            string clientIp = HttpContext.Current.Request.UserHostAddress;
            var request = HttpContext.Current.Request;
            
            WriteLog.writeLog("Incoming Client Request from IP: " + clientIp + "" + Environment.NewLine + "Action:POOmorfia/PostPOOmorfia");
            List<PostVoucherResponse> getData = new List<PostVoucherResponse>();
            try
            {
                string Comp = Convert.ToString(GetCompID(CompanyCode));
                string SessionId = GetSessionId(Comp, Username, Password);
                string voucherno = jvo.Header.DocNo;
                string MyVNo = voucherno;
                DateTime dt = new DateTime(Convert.ToDateTime(jvo.Header.Date).Year, Convert.ToDateTime(jvo.Header.Date).Month, Convert.ToDateTime(jvo.Header.Date).Day);
                int vDate = Convertdate(Convert.ToDateTime(dt));
                HashData objHashRequest = new HashData();
                Hashtable objHeader = new Hashtable();
                objHeader.Add("Doc", MyVNo);
                objHeader.Add("Date", vDate);
                objHeader.Add("VendorAC__Name", jvo.Header.VendorACName);
                objHeader.Add("Currency__Id", 110);                     // KWD
                objHeader.Add("ExchangeRate", "1.0000000000");
                objHeader.Add("Entity__Id", 10);                        // Omorfia Life Co. WLL
                objHeader.Add("Business Unit__Id", 16);                 // COR-SC
                objHeader.Add("sNarration", jvo.Header.Narration);
                List<Dictionary<string, object>> lstBody = new List<Dictionary<string, object>>();

                for (int i = 0; i < jvo.Body.Count; i++)
                {
                    Dictionary<string, object> objBody = new Dictionary<string, object>();

                    objBody.Add("Item__Id", 3622);                           // SERV0001
                    objBody.Add("Description", jvo.Body[i].Description);
                    objBody.Add("Unit__Name", "Each");
                    objBody.Add("Quantity", jvo.Body[i].Quantity);
                    objBody.Add("Rate", jvo.Body[i].Rate);
                    objBody.Add("sRemarks", jvo.Body[i].Remarks);

                    lstBody.Add(objBody);
                }

                Hashtable objHash = new Hashtable();
                objHash.Add("Header", objHeader);
                objHash.Add("Body", lstBody);

                List<Hashtable> lstHash = new List<Hashtable>();
                lstHash.Add(objHash);
                objHashRequest.data = lstHash;
                objHashRequest.url = "http://" + ServerIp + "/Focus8API/Transactions/2564/" + MyVNo + "/";
                objHashRequest.result = 1;
                string sContent = JsonConvert.SerializeObject(objHashRequest);
                using (var client = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
                                    | SecurityProtocolType.Tls12
                                    | SecurityProtocolType.Tls11
                                    | SecurityProtocolType.Tls;
                    client.Encoding = System.Text.Encoding.UTF8;
                    client.Headers.Add("fSessionId", SessionId);
                    client.Headers.Add("Content-Type", "application/json");
                    WriteLog.writeLog(sContent.ToString());
                    var response = client.UploadString("http://" + ServerIp + "/Focus8API/Transactions/2564", sContent);
                    WriteLog.writeLog(response.ToString());
                    if (response != null)
                    {
                        var responseData = JsonConvert.DeserializeObject<ApiResponse.PostResponse>(response);
                        if (responseData.result == 1)
                        {
                            getData.Add(new ApiResponse.PostVoucherResponse { VoucherNo = Convert.ToString(responseData.data[0]["VoucherNo"]), result = responseData.result, message = "Voucher Posted Successfully" });
                        }
                        if (responseData.result == -1)
                        {
                            WriteLog.writeLog(Convert.ToString(responseData.data[0]["VoucherNo"]));
                            getData.Add(new ApiResponse.PostVoucherResponse { VoucherNo = Convert.ToString(responseData.data[0]["VoucherNo"]), result = 0, message = "Voucher Not Posted : " + Convert.ToString(responseData.message) + "" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                getData.Add(new PostVoucherResponse { result = 0, message = "Voucher Not Posted {" + ex.Message + "}" });
            }
            return Ok(getData);
        }
        
    }
}