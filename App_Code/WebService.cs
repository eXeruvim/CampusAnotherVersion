using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

[WebService(Namespace = "http://blah.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService
{

    [WebMethod]
    [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
    public void GetMarks(string numberBook)
    {
        HttpContext.Current.Response.Write(Marks.getGradeSubject(numberBook));
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
    public void GetAttestation(string numberBook)
    {
        HttpContext.Current.Response.Write(Current_attestation.getAttestation(numberBook));
    }
}