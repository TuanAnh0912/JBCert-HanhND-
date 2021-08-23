using jbcert.DATA.Provider;
using jbcert.DATA.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace jbcert.API.Middleware
{
    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(string roleValue) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim("Role", roleValue) };
        }
    }

    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        private readonly Claim _claim;
        private readonly ILogger<ClaimRequirementFilter> _logger;

        public ClaimRequirementFilter(Claim claim, ILogger<ClaimRequirementFilter> logger)
        {
            _claim = claim;
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool flag = false;
            NguoiDungProvider userProvider = new NguoiDungProvider();
            string username = context.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
            {
                context.Result = new BadRequestObjectResult("TOKENTIMEOUT");
            }
            List<string> roles = _claim.Value.Split(',').ToList();
            if (roles.Any(x => x.ToLower() == "all"))
            {
                flag = true;
            }
            ChucNangProvider permissionProvider = new ChucNangProvider();
            var user = userProvider.GetByTenDangNhap(username);
            if (user == null)
            {
                context.Result = new BadRequestObjectResult("TOKENTIMEOUT");
            }
            else
            {
                List<ChucNangViewModel> permissionViewModels = permissionProvider.GetByNhomNguoiDungId(user.NhomNguoiDungId.Value);
                foreach (string role in roles)
                {
                    if (permissionViewModels.Any(x => x.AuthCode != null && x.AuthCode.ToLower().Trim() == role.ToLower().Trim()))
                    {
                        flag = true;
                    }
                }

                if (!flag)
                {
                    context.Result = new BadRequestObjectResult("UNAUTHORIZE");
                }
            }

         
        }
    }
}