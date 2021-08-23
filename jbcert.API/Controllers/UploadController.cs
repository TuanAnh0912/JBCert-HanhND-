using jbcert.API.Middleware;
using jbcert.DATA.Provider;
using jbcert.DATA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace jbcert.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ClientIpCheckActionFilter))]
    public class UploadController : ControllerBase
    {
        NguoiDungProvider _nguoiDungProvider = new NguoiDungProvider();
        [Route("ImportHocSinhExcelFile")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult ImportHocSinhExcelFile()
        {
            try
            {
                var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "ImportHocSinhExcel", user.DonViId.Value.ToString());
                bool exists = System.IO.Directory.Exists(folderPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(folderPath);

                var formFiles = Request.Form.Files;
                if (formFiles != null && formFiles.Count() > 0)
                {
                    var files = new List<FileDinhKemYeuCauViewModel>();
                    foreach (var formFile in formFiles)
                    {
                        string fileId = Guid.NewGuid().ToString();
                        string guid = DateTime.Now.ToString("yyyy-MM-dd") + "_" + fileId + "_";
                        var fileName = guid + formFile.FileName;
                        var filePath = folderPath + "/" + fileName;
                        string ext = Path.GetExtension(filePath);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            formFile.CopyTo(stream);
                        }
                        string url = "/Upload/ImportHocSinhExcel/" + user.DonViId.Value + "/" + fileName;
                        string path_e = GetVirtualPath(filePath);
                        var obj = new FileDinhKemYeuCauViewModel();
                        obj.FileId = fileId;
                        obj.Url = url;
                        obj.TenFile = formFile.FileName;
                        obj.NguoiTao = user.NguoiDungId;
                        obj.NgayTao = DateTime.Now;
                        obj.DonViId = user.PhongBan.DonViId;
                        obj.Ext = ext;
                        obj.FileKey = fileId;
                        switch (obj.Ext.ToLower())
                        {
                            case ".jpg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".jpeg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".png":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/png.png";
                                break;

                            case ".pdf":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/pdf.png";
                                break;

                            case ".xlsx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".xls":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".doc":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".docx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".ppt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/ppt.png";
                                break;

                            case ".txt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/txt.png";
                                break;

                            case ".zip":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/zip.png";
                                break;

                            default:
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/unknow.png";
                                break;
                        }
                        files.Add(obj);
                    }
                    return Ok(files);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("ImportSoGocFile")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult ImportSoGocFile()
        {
            try
            {
                var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "SoGoc", user.DonViId.Value.ToString());
                bool exists = System.IO.Directory.Exists(folderPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(folderPath);

                var formFiles = Request.Form.Files;
                if (formFiles != null && formFiles.Count() > 0)
                {
                    var files = new List<FileDinhKemYeuCauViewModel>();
                    foreach (var formFile in formFiles)
                    {
                        string fileId = Guid.NewGuid().ToString();
                        string guid = DateTime.Now.ToString("yyyy-MM-dd") + "_" + fileId + "_";
                        var fileName = guid + formFile.FileName;
                        var filePath = folderPath + "/" + fileName;
                        string ext = Path.GetExtension(filePath);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            formFile.CopyTo(stream);
                        }
                        string url = "/Upload/SoGoc/" + user.DonViId.Value + "/" + fileName;
                        string path_e = GetVirtualPath(filePath);
                        var obj = new FileDinhKemYeuCauViewModel();
                        obj.FileId = fileId;
                        obj.Url = url;
                        obj.TenFile = formFile.FileName;
                        obj.NguoiTao = user.NguoiDungId;
                        obj.NgayTao = DateTime.Now;
                        obj.DonViId = user.PhongBan.DonViId;
                        obj.Ext = ext;
                        obj.FileKey = fileId;
                        switch (obj.Ext.ToLower())
                        {
                            case ".jpg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".jpeg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".png":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/png.png";
                                break;

                            case ".pdf":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/pdf.png";
                                break;

                            case ".xlsx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".xls":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".doc":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".docx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".ppt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/ppt.png";
                                break;

                            case ".txt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/txt.png";
                                break;

                            case ".zip":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/zip.png";
                                break;

                            default:
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/unknow.png";
                                break;
                        }
                        files.Add(obj);
                    }
                    return Ok(files);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <returns>File Directory</returns>
        [Route("UploadFileDinhKemYeuCau")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UploadFileDinhKemYeuCau()
        {
            try
            {
                var formFiles = Request.Form.Files;
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (formFiles != null && formFiles.Count() > 0)
                {
                    var files = new List<FileDinhKemYeuCauViewModel>();
                    foreach (var formFile in formFiles)
                    {
                        string fileId = Guid.NewGuid().ToString();
                        string guid = DateTime.Now.ToString("yyyy-MM-dd") + "_" + fileId + "_";
                        var fileName = guid + formFile.FileName;
                        string url = "/Upload/Request/" + fileName;
                        var filePath = Directory.GetCurrentDirectory() + "/Upload/Request/" + fileName;
                        string ext = Path.GetExtension(filePath);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            formFile.CopyTo(stream);
                        }

                        string path_e = GetVirtualPath(filePath);
                        var obj = new FileDinhKemYeuCauViewModel();
                        obj.FileId = fileId;
                        obj.Url = url;
                        obj.TenFile = formFile.FileName;
                        obj.NguoiTao = user.NguoiDungId;
                        obj.NgayTao = DateTime.Now;
                        obj.DonViId = user.PhongBan.DonViId;
                        obj.Ext = ext;
                        obj.FileKey = fileId;
                        switch (obj.Ext.ToLower())
                        {
                            case ".jpg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".jpeg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".png":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/png.png";
                                break;

                            case ".pdf":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/pdf.png";
                                break;

                            case ".xlsx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".xls":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".doc":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".docx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".ppt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/ppt.png";
                                break;

                            case ".txt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/txt.png";
                                break;

                            case ".zip":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/zip.png";
                                break;

                            default:
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/unknow.png";
                                break;
                        }
                        files.Add(obj);
                    }
                    return Ok(files);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <returns>File Directory</returns>
        [Route("UploadFileHocSinhDuXetTotNghiep")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UploadFileHocSinhDuXetTotNghiep()
        {
            try
            {
                var formFiles = Request.Form.Files;
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (formFiles != null && formFiles.Count() > 0)
                {
                    var files = new List<FileDinhKemYeuCauViewModel>();
                    foreach (var formFile in formFiles)
                    {
                        string fileId = Guid.NewGuid().ToString();
                        string guid = DateTime.Now.ToString("yyyy-MM-dd") + "_" + fileId + "_";
                        var fileName = guid + formFile.FileName;
                        string url = "/Upload/FileHocSinhDuXetTotNghiep/" + fileName;
                        var filePath = Directory.GetCurrentDirectory() + "/Upload/FileHocSinhDuXetTotNghiep/" + fileName;
                        string ext = Path.GetExtension(filePath);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            formFile.CopyTo(stream);
                        }

                        string path_e = GetVirtualPath(filePath);
                        var obj = new FileDinhKemYeuCauViewModel();
                        obj.FileId = fileId;
                        obj.Url = url;
                        obj.TenFile = formFile.FileName;
                        obj.NguoiTao = user.NguoiDungId;
                        obj.NgayTao = DateTime.Now;
                        obj.DonViId = user.PhongBan.DonViId;
                        obj.Ext = ext;
                        obj.FileKey = fileId;
                        switch (obj.Ext.ToLower())
                        {
                            case ".jpg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".jpeg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".png":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/png.png";
                                break;

                            case ".pdf":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/pdf.png";
                                break;

                            case ".xlsx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".xls":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".doc":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".docx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".ppt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/ppt.png";
                                break;

                            case ".txt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/txt.png";
                                break;

                            case ".zip":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/zip.png";
                                break;

                            default:
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/unknow.png";
                                break;
                        }
                        files.Add(obj);
                    }
                    return Ok(files);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Upload file đính kèm của học sinh
        /// </summary>
        /// <returns></returns>
        [Route("UploadFileDinhKemHocSinh")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UploadFileDinhKemHocSinh()
        {
            try
            {
                var formFiles = Request.Form.Files;
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (formFiles != null && formFiles.Count() > 0)
                {
                    var files = new List<HocSinhFileDinhKemViewModel>();
                    foreach (var formFile in formFiles)
                    {
                        string fileId = Guid.NewGuid().ToString();
                        string guid = DateTime.Now.ToString("yyyy-MM-dd") + "_" + fileId + "_";
                        var fileName = guid + formFile.FileName;
                        string url = "/Upload/HocSinhFile/" + fileName;
                        var filePath = Directory.GetCurrentDirectory() + "/Upload/HocSinhFile/" + fileName;
                        string ext = Path.GetExtension(filePath);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            formFile.CopyTo(stream);
                        }

                        string path_e = GetVirtualPath(filePath);
                        var obj = new HocSinhFileDinhKemViewModel();
                        obj.FileId = fileId;
                        obj.Url = url;
                        obj.TenFile = formFile.FileName;
                        obj.NguoiTao = user.NguoiDungId;
                        obj.NgayTao = DateTime.Now;
                        obj.DonViId = user.PhongBan.DonViId;
                        obj.Ext = ext;
                        switch (obj.Ext.ToLower())
                        {
                            case ".jpg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".jpeg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".png":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/png.png";
                                break;

                            case ".pdf":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/pdf.png";
                                break;

                            case ".xlsx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".xls":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".doc":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".docx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".ppt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/ppt.png";
                                break;

                            case ".txt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/txt.png";
                                break;

                            case ".zip":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/zip.png";
                                break;

                            default:
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/unknow.png";
                                break;
                        }
                        files.Add(obj);
                    }
                    return Ok(files);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Upload file đính kèm của học sinh
        /// </summary>
        /// <returns></returns>
        [Route("UploadFileDinhKemPhoi")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UploadFileDinhKemPhoi()
        {
            try
            {
                var formFiles = Request.Form.Files;
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (formFiles != null && formFiles.Count() > 0)
                {
                    var files = new List<PhoiFileDinhKemViewModel>();
                    foreach (var formFile in formFiles)
                    {
                        string fileId = Guid.NewGuid().ToString();
                        string guid = DateTime.Now.ToString("yyyy-MM-dd") + "_" + fileId + "_";
                        var fileName = guid + formFile.FileName;
                        string url = "/Upload/AnhPhoi/" + fileName;
                        var filePath = Directory.GetCurrentDirectory() + "/Upload/AnhPhoi/" + fileName;
                        string ext = Path.GetExtension(filePath);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            formFile.CopyTo(stream);
                        }

                        string path_e = GetVirtualPath(filePath);
                        var obj = new PhoiFileDinhKemViewModel();
                        obj.FileId = fileId;
                        obj.Url = url;
                        obj.TenFile = formFile.FileName;
                        obj.NguoiTao = user.NguoiDungId;
                        obj.NgayTao = DateTime.Now;
                        obj.DonViId = user.PhongBan.DonViId;
                        obj.Ext = ext;
                        switch (obj.Ext.ToLower())
                        {
                            case ".jpg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".jpeg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".png":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/png.png";
                                break;

                            case ".pdf":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/pdf.png";
                                break;

                            case ".xlsx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".xls":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".doc":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".docx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".ppt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/ppt.png";
                                break;

                            case ".txt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/txt.png";
                                break;

                            case ".zip":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/zip.png";
                                break;

                            default:
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/unknow.png";
                                break;
                        }
                        files.Add(obj);
                    }
                    return Ok(files);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Upload file đính kèm của học sinh
        /// </summary>
        /// <returns></returns>
        [Route("UploadAnhLoaiBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UploadAnhLoaiBang()
        {
            try
            {
                var formFiles = Request.Form.Files;
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (formFiles != null && formFiles.Count() > 0)
                {
                    var files = new List<LoaiBangFileDinhKemViewModel>();
                    foreach (var formFile in formFiles)
                    {
                        string fileId = Guid.NewGuid().ToString();
                        string guid = DateTime.Now.ToString("yyyy-MM-dd") + "_" + fileId + "_";
                        var fileName = guid + formFile.FileName;
                        string url = "/Upload/AnhLoaiBang/" + fileName;
                        var filePath = Directory.GetCurrentDirectory() + "/Upload/AnhLoaiBang/" + fileName;
                        string ext = Path.GetExtension(filePath);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            formFile.CopyTo(stream);
                        }

                        string path_e = GetVirtualPath(filePath);
                        var obj = new LoaiBangFileDinhKemViewModel();
                        obj.FileId = fileId;
                        obj.Url = url;
                        obj.TenFile = formFile.FileName;
                        obj.NguoiTao = user.NguoiDungId;
                        obj.NgayTao = DateTime.Now;
                        obj.DonViId = user.PhongBan.DonViId;
                        obj.Ext = ext;
                        switch (obj.Ext.ToLower())
                        {
                            case ".jpg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".jpeg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".png":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/png.png";
                                break;

                            case ".pdf":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/pdf.png";
                                break;

                            case ".xlsx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".xls":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".doc":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".docx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".ppt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/ppt.png";
                                break;

                            case ".txt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/txt.png";
                                break;

                            case ".zip":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/zip.png";
                                break;

                            default:
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/unknow.png";
                                break;
                        }
                        files.Add(obj);
                    }
                    return Ok(files);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UploadAnhVanBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UploadAnhVanBang()
        {
            try
            {
                var formFiles = Request.Form.Files;
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (formFiles != null && formFiles.Count() > 0)
                {
                    var files = new List<AnhVanBangViewModel>();
                    foreach (var formFile in formFiles)
                    {
                        string fileId = Guid.NewGuid().ToString();
                        string guid = DateTime.Now.ToString("yyyy-MM-dd") + "_" + fileId + "_";
                        var fileName = guid + formFile.FileName;
                        string url = "/Upload/AnhVanBang/" + fileName;
                        var filePath = Directory.GetCurrentDirectory() + "/Upload/AnhVanBang/" + fileName;
                        string ext = Path.GetExtension(filePath);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            formFile.CopyTo(stream);
                        }

                        string path_e = GetVirtualPath(filePath);
                        var obj = new AnhVanBangViewModel();
                        obj.FileId = fileId;
                        obj.Url = url;
                        obj.TenFile = formFile.FileName;
                        obj.NguoiTao = user.NguoiDungId;
                        obj.NgayTao = DateTime.Now;
                        obj.DonViId = user.PhongBan.DonViId;
                        obj.Ext = ext;
                        switch (obj.Ext.ToLower())
                        {
                            case ".jpg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".jpeg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".png":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/png.png";
                                break;

                            case ".pdf":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/pdf.png";
                                break;

                            case ".xlsx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".xls":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".doc":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".docx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".ppt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/ppt.png";
                                break;

                            case ".txt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/txt.png";
                                break;

                            case ".zip":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/zip.png";
                                break;

                            default:
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/unknow.png";
                                break;
                        }
                        files.Add(obj);
                    }
                    return Ok(files);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("UploadQuyetDinhCaiChinh")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UploadQuyetDinhCaiChinh()
        {
            try
            {
                var formFiles = Request.Form.Files;
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (formFiles != null && formFiles.Count() > 0)
                {
                    var files = new List<FileDinhKemCaiChinhViewModel>();
                    foreach (var formFile in formFiles)
                    {
                        string fileId = Guid.NewGuid().ToString();
                        string guid = DateTime.Now.ToString("yyyy-MM-dd") + "_" + fileId + "_";
                        var fileName = guid + formFile.FileName;
                        string url = "/Upload/QDCaiChinh/" + fileName;
                        var filePath = Directory.GetCurrentDirectory() + "/Upload/QDCaiChinh/" + fileName;
                        string ext = Path.GetExtension(filePath);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            formFile.CopyTo(stream);
                        }

                        string path_e = GetVirtualPath(filePath);
                        var obj = new FileDinhKemCaiChinhViewModel();
                        obj.FileId = fileId;
                        obj.Url = url;
                        obj.TenFile = formFile.FileName;
                        obj.NguoiTao = user.NguoiDungId;
                        obj.NgayTao = DateTime.Now;
                        obj.DonViId = user.PhongBan.DonViId;
                        obj.Ext = ext;
                        switch (obj.Ext.ToLower())
                        {
                            case ".jpg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".jpeg":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                                break;

                            case ".png":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/png.png";
                                break;

                            case ".pdf":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/pdf.png";
                                break;

                            case ".xlsx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".xls":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                                break;

                            case ".doc":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".docx":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                                break;

                            case ".ppt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/ppt.png";
                                break;

                            case ".txt":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/txt.png";
                                break;

                            case ".zip":
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/zip.png";
                                break;

                            default:
                                obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/unknow.png";
                                break;
                        }
                        files.Add(obj);
                    }
                    return Ok(files);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private string GetVirtualPath(string physicalPath)
        {
            string rootpath = Directory.GetCurrentDirectory();
            physicalPath = physicalPath.Replace(rootpath, "");
            physicalPath = physicalPath.Replace("\\", "/");
            return "/" + physicalPath;
        }
    }
}