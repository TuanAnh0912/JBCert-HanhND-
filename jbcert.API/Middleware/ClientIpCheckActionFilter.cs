using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace jbcert.API.Middleware
{
    public class ClientIpCheckActionFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;

        public ClientIpCheckActionFilter(ILogger logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {

            string timeNow = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
            _logger.LogInformation("Time " + timeNow + " | Remote IpAddress: " + remoteIp);
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                foreach (var routeValue in controllerActionDescriptor.RouteValues)
                {
                    _logger.LogInformation("Time " + timeNow + " | Remote IpAddress: " + remoteIp + " | Route: " + routeValue.Value + " - Type: " + routeValue.Key);
                }
                var actionAttributes = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true);
            }

            base.OnActionExecuting(context);
        }
    }
}
