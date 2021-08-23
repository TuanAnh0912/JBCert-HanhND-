using jbcert.API.Middleware;
using jbcert.DATA.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace jbcert.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackUpController : ControllerBase
    {
        BackUpProvider _backUpProvider = new BackUpProvider();
        [Route("ExportBangGoc")]
        //[ClaimRequirement("All")]
        [HttpGet]
        public IActionResult ExportBangGoc(int nam, int LoaiBangId)
        {
            try
            {
                return Ok(_backUpProvider.GetAllThongTinSoGocByNam(nam, LoaiBangId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}