using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Helpers
{
    public static class QRHelper
    {
        public static string QRGenerator(string data)
        {
            data = string.IsNullOrEmpty(data) ? "Không tìm thấy dữ liệu" : @"http://tracuu-backan.jbcert.vn/detail?keyword=" + data.Replace("/", "-");
            string path = Path.Combine("Upload", "QRCode", "QRCodeBang", Guid.NewGuid().ToString() + ".png");
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            qrCodeImage.Save(Path.Combine(Directory.GetCurrentDirectory(), path), System.Drawing.Imaging.ImageFormat.Png);
            return "/" + path;
        }
    }
}
