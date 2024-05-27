using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace VirtusGroup.API.Auth
{
    public class BasicAuthAttribute:AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            var authorization = request.Headers.Authorization;

            if (authorization == null || authorization.Scheme != "Basic")
            {
                HandleUnauthorized(actionContext);
                return;
            }
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authorization.Parameter)).Split(':');
            var username = credentials[0];
            var password = credentials[1];

            if(!IsValidUser(username, password))
            {
                HandleUnauthorized(actionContext);
            }
        }
        private void HandleUnauthorized(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", "Basic");
        }
        private bool IsValidUser(string username, string password)
        {
            string storedUsername = ConfigurationManager.AppSettings["apiUsername"];
            string storedPassword = ConfigurationManager.AppSettings["apiPassword"];

            return username == storedUsername && password == storedPassword;
        }
    }
}