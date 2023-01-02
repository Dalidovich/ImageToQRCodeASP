using Microsoft.Extensions.FileSystemGlobbing.Internal;
using QRCoder;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

using System.ComponentModel.DataAnnotations;
namespace ImageToQRCodeWebSite.Models
{
    public class ImgToQRCodeConverter
    {
        public string imageWay{get;set;}
        public byte[] imageBytes;
        private MemoryStream _qrCode=new MemoryStream();

        private Dictionary<Enum, int> _ECCLevelBytesDictionary = new Dictionary<Enum, int>()
            {
                { QRCodeGenerator.ECCLevel.L, 2953 },//l <=2953 7% may be lost before recovery is not possible
                { QRCodeGenerator.ECCLevel.M, 2331 },//m <=2331 15% may be lost before recovery is not possible
                { QRCodeGenerator.ECCLevel.Q, 1663 },//q <=1663 25% may be lost before recovery is not possible
                { QRCodeGenerator.ECCLevel.H, 1273 } //h <=1273 30% may be lost before recovery is not possible
            };
        public string QRCodeToBase64(Bitmap QRCode)
        {
            QRCode.Save(_qrCode, System.Drawing.Imaging.ImageFormat.Jpeg);
            //Bitmap a = new Bitmap(_qrCode);
            //a.Save("asd.png");
            return Convert.ToBase64String(_qrCode.ToArray());
        }
        public string imgToBase64()
        {
            string base64String = Convert.ToBase64String(imageBytes.ToArray());
            return base64String;
        }
        public string createPattern()
        {
            return $"data:text/html,<img src=\"data:image/png;base64,{imgToBase64()}\">";
        }
        public QRCodeGenerator.ECCLevel choosingRightLevel(string pattern)
        {
            return (QRCodeGenerator.ECCLevel)_ECCLevelBytesDictionary.Where(x => x.Value > pattern.Length).Last().Key;
        }
        public string getInfoECCLevel(string pattern)
        {
            if (pattern.Length >= _ECCLevelBytesDictionary[QRCodeGenerator.ECCLevel.L])
                return "error: big picture";
            var curECCLevel = (QRCodeGenerator.ECCLevel)_ECCLevelBytesDictionary.Where(x => x.Value > pattern.Length).Last().Key;
            switch (curECCLevel)
            {
                case QRCoder.QRCodeGenerator.ECCLevel.L:
                    return "7% may be lost before recovery is not possible";
                case QRCoder.QRCodeGenerator.ECCLevel.M:
                    return "15% may be lost before recovery is not possible";
                case QRCoder.QRCodeGenerator.ECCLevel.Q:
                    return "25% may be lost before recovery is not possible";
                case QRCoder.QRCodeGenerator.ECCLevel.H:
                    return "30% may be lost before recovery is not possible";
            }
            return "exeption";
        }
        public string createQRCodeAutomatically()
        {
            var pattern = createPattern();
            QRCodeGenerator qrGenerator = new QRCodeGenerator();            
            if (pattern.Length >= _ECCLevelBytesDictionary[QRCodeGenerator.ECCLevel.L])
                return "error: big picture";
            return createQRCode(choosingRightLevel(pattern), pattern);
        }
        private string createQRCode(QRCodeGenerator.ECCLevel level,string pattern)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(pattern, level);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            qrCodeImage.Save("lol.png", ImageFormat.Jpeg);
            return QRCodeToBase64(qrCodeImage);
        }
    }
}
