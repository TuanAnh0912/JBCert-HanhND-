using jbcert.API.Middleware;
using jbcert.API.Service;
using jbcert.DATA.IdentityModels;
using jbcert.DATA.Models;
using jbcert.DATA.Provider;
using jbcert.DATA.ViewModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ClientIpCheckActionFilter))]
    public class AuthenticateController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;

        public AuthenticateController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
                                    IConfiguration configuration,
                                    IEmailSender emailSender, ISmsSender smsSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _smsSender = smsSender;
        }
        [HttpGet]
        [Route("GetThongTinNguoiDung")]
        [ClaimRequirement("All")]
        public IActionResult GetThongTinNguoiDung()
        {
            NguoiDungProvider nguoiDungProvider = new NguoiDungProvider();
            NguoiDung nguoidung = nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

            return Ok(new { fullName = nguoidung.HoTen, userName = nguoidung.TenDangNhap, id = nguoidung.NguoiDungId });
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: false);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    if (user.TwoFactorEnabled)
                    {
                        var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
                        var code = await _userManager.GenerateTwoFactorTokenAsync(user, userFactors.FirstOrDefault().ToString());
                        await _smsSender.SendSmsAsync(user.PhoneNumber, "Your security code is: " + code);
                        return Ok(new { Message = "Yêu cầu nhập mã từ điện thoại", Tokens = model.Username });
                    }
                    else
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);

                        var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                        foreach (var userRole in userRoles)
                        {
                            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                        }

                        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                        var token = new JwtSecurityToken(
                            issuer: _configuration["JWT:ValidIssuer"],
                            audience: _configuration["JWT:ValidAudience"],
                            expires: DateTime.UtcNow.AddDays(6),
                            claims: authClaims,
                            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                            );
                        NguoiDungProvider nguoiDungProvider = new NguoiDungProvider();
                        NguoiDung nguoiDung = nguoiDungProvider.GetByTenDangNhap(model.Username);
                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo,
                            Fullname = nguoiDung.HoTen,
                            UserId = nguoiDung.NguoiDungId,
                            Username = model.Username
                        });
                    }
                }
                return Unauthorized();
            }
            catch(Exception ex)
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("LoginWithCode")]
        public async Task<IActionResult> LoginWithCode([FromBody] VerifyCodeViewModel model)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return Unauthorized("Người dùng không tồn tại");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            if (user != null && userFactors != null && userFactors.Count() >= 0)
            {

                if (await _userManager.VerifyTwoFactorTokenAsync(user, "Phone", model.Code))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );
                    NguoiDungProvider nguoiDungProvider = new NguoiDungProvider();
                    NguoiDung nguoiDung = nguoiDungProvider.GetByTenDangNhap(user.UserName);
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo,
                        Fullname = nguoiDung.HoTen,
                        UserId = nguoiDung.NguoiDungId,
                        Username = user.UserName
                    });
                }
                else
                {
                    return BadRequest("Sai mã code " + model.Code);
                }
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                responseViewModel.Message = "Username đã tồn tại";
                responseViewModel.Data = null;
                responseViewModel.Status = false;
                return BadRequest(responseViewModel.Message);
            }

            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
            {
                responseViewModel.Message = "Email đã tồn tại";
                responseViewModel.Data = null;
                responseViewModel.Status = false;
                return BadRequest(responseViewModel.Message);
            }

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                responseViewModel.Message = "Mật khẩu không đúng định dạng";
                responseViewModel.Data = null;
                responseViewModel.Status = false;
                return Ok(responseViewModel);
            }

            NguoiDungProvider _nguoiDungProvider = new NguoiDungProvider();
            NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            NguoiDungViewModel obj = new NguoiDungViewModel();
            obj.NguoiDungId = Guid.NewGuid();
            obj.TenDangNhap = model.TenDangNhap;
            obj.MatKhau = "Không cần lưu mật khẩu ở đây";
            obj.HoTen = model.HoTen;
            obj.NhomNguoiDungId = model.NhomNguoiDungId;
            obj.PhongBanId = model.PhongBanId;
            obj.NgaySinh = model.NgaySinh;
            obj.DiaChi = model.DiaChi;
            obj.SoCanCuoc = model.SoCanCuoc;
            obj.DienThoai = model.DienThoai;
            obj.Email = model.Email;
            obj.NgayTao = DateTime.Now;
            obj.NguoiTao = nguoiDung != null ? nguoiDung.NguoiDungId : null;
            obj.Active = true;
            obj.IsDelete = false;
            obj.GioiTinhId = model.GioiTinhId;
            obj.LaGiaoVien = model.LaGiaoVien;
            obj.DanTocId = model.DanTocId;
            obj.DonViId = model.DonViId;
            _nguoiDungProvider.InsertUser(obj);

            responseViewModel.Message = "Đăng ký tài khoản thành công";
            responseViewModel.Data = null;
            responseViewModel.Status = true;
            return Ok(responseViewModel);
        }


        [HttpPost]
        [Route("AddPhoneNumber")]
        public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok("Lỗi");
            }
            // Generate the token and send it
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Ok("Không tồn tại tài khoản");
            }
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
            await _smsSender.SendSmsAsync(model.PhoneNumber, "Your security code is: " + code);
            return RedirectToAction(nameof(VerifyPhoneNumber), new { PhoneNumber = model.PhoneNumber });
        }

        [HttpGet]
        [Route("VerifyPhoneNumber")]
        public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Ok("Error");
            }
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            // Send an SMS to verify the phone number
            return phoneNumber == null ? Ok("Error") : Ok(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        [HttpPost]
        [Route("VerifyPhoneNumber")]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok("Fail");
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
                if (result.Succeeded)
                {
                    await _userManager.SetTwoFactorEnabledAsync(user, true);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Ok("Đã bật xác thực 2 yếu tố");
                }
                else
                {
                    return Ok("Sai mã");
                }
            }
            return Ok();
        }

        //[HttpPost]
        //[Route("EnableTwoFactorAuthentication")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EnableTwoFactorAuthentication()
        //{
        //    var user = await GetCurrentUserAsync();
        //    if (user != null)
        //    {
        //        await _userManager.SetTwoFactorEnabledAsync(user, true);
        //        await _signInManager.SignInAsync(user, isPersistent: false);
        //    }
        //    return Ok();
        //}

        private string GenerateJSONWebToken(LoginModel loginModel)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:ValidAudience"],
              _configuration["Jwt:ValidIssuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            AddLoginSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        [Route("RequireAuthentication")]
        [ClaimRequirement("All")]
        [HttpGet]
        public async Task<IActionResult> RequireAuthentication(string password)
        {
            ResponseViewModel<object> responseViewModel = new DATA.ViewModels.ResponseViewModel<object>();
            try
            {
                var user = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Name));
                //var timestamp = DateTime.Now.ToFileTime();
                bool result= await _userManager.CheckPasswordAsync(user, password);
                responseViewModel.Data = result;
                responseViewModel.Status = true;
                responseViewModel.Message = result ? "Xác nhận mật khẩu thành công" : "Xác nhận mật khẩu không thành công";
                return Ok(responseViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("SendCodeAuthentication")]
        [ClaimRequirement("All")]
        [HttpGet]
        public async Task<IActionResult> SendCodeAuthentication()
        {
            var user = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Name));

            var timestamp = DateTime.Now.ToFileTime();

            //await _smsSender.SendSmsAsync(user.PhoneNumber, "Your security code is: " + timestamp);

            return Ok(timestamp);
        }

        [Route("SendLinkForResetPassword")]
        //[ClaimRequirement("All")]
        [HttpPost]
        public async Task<IActionResult> SendLinkForResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
                if (user == null)
                {
                    return BadRequest("Không tìm thấy email!");
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                string callBackUrl = string.Format("{0}?email={1}&code={2}", resetPasswordViewModel.CallBackUrl, user.Email, code);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Template", "ResetPasswordTemplate.Html");
                string content = System.IO.File.ReadAllText(path);
                content = content.Replace("{{link}}", callBackUrl);
                // create email message
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("notification.jbhotel@gmail.com"));
                email.To.Add(MailboxAddress.Parse(resetPasswordViewModel.Email));
                email.Subject = "Khôi phục mật khẩu";
                email.Body = new TextPart(TextFormat.Html) { Text = content };

                // send email
                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("notification.jbhotel@gmail.com", "Vinh3001");
                smtp.Send(email);
                smtp.Disconnect(true);

                return Ok("Đã gửi link cập nhật mật khẩu vào email của bạn!");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("ResetPassword")]
        //[ClaimRequirement("All")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
                if (user == null)
                {
                    return BadRequest("Không tìm thấy tài khoản!");
                }
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Code, resetPasswordViewModel.Password);
                if (result.Succeeded)
                {
                    return Ok("Đổi mật khẩu mới thành công");
                }
                else
                {
                    return BadRequest("Phiên đổi mật khẩu không có hiệu lực");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
