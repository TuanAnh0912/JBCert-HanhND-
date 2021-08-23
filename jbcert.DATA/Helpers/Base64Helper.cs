using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace jbcert.DATA.Helpers
{
    public static class Base64Helper
    {
        public static string ConvertBase64ToFileAndSave(string base64String, string filename, string folderPath)
        {
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Upload", folderPath, filename);
                File.WriteAllBytes(path, Convert.FromBase64String(base64String));
                return "/Upload/" + folderPath + "/" + filename;
            }
            catch (Exception ex)
            {
                return base64String;
            }

        }
    }
}
