using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web;

namespace VirtusGroup.API
{
    public class JwtValidationHandler: DelegatingHandler
    {
        private readonly string _jwtKey = ConfigurationManager.AppSettings["Jwt:Key"];
        private readonly string _jwtIssuer = ConfigurationManager.AppSettings["Jwt:Issuer"];
        private readonly string _jwtAudience = ConfigurationManager.AppSettings["Jwt:Audience"];

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Bypass token validation for specific endpoints
            if (request.RequestUri.AbsolutePath.EndsWith("/api/Login", StringComparison.OrdinalIgnoreCase))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            if (request.Headers.Authorization != null && request.Headers.Authorization.Scheme == "Bearer")
            {
                var token = request.Headers.Authorization.Parameter;
                var tokenHandler = new JwtSecurityTokenHandler();

                try
                {
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _jwtIssuer,
                        ValidAudience = _jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),
                        ClockSkew = TimeSpan.Zero
                    };

                    SecurityToken validatedToken;
                    var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                    HttpContext.Current.User = principal;
                    Thread.CurrentPrincipal = principal;
                }
                catch (SecurityTokenException)
                {
                    return request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid token");
                }
                catch (Exception)
                {
                    return request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred while processing the token");
                }
            }
            else
            {
                return request.CreateResponse(HttpStatusCode.Unauthorized, "No token provided");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}