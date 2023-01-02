using ImageToQRCodeWebSite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.IO;


namespace ImageToQRCodeWebSite.Controllers
{
    public class ConvertedImageController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Index(ImgToQRCodeConverter imgToQRCodeConverter)
        {
            IFormFileCollection files = Request.Form.Files;
            MemoryStream memoryStream = new MemoryStream();
            await files[0].CopyToAsync(memoryStream);
            imgToQRCodeConverter.imageWay = files[0].FileName;
            imgToQRCodeConverter.imageBytes = memoryStream.ToArray();
            return View(imgToQRCodeConverter);
        }
    }
}
