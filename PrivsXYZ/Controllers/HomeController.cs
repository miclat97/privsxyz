using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrivsXYZ.Models;
using PrivsXYZ.Services;

namespace PrivsXYZ.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly IPhotoService _photoService;
        private readonly IFileService _fileService;

        private const int MaxFileSizeMB = 5 * 1024 * 1024;

        public HomeController(IMessageService messageService, IPhotoService photoService, IFileService fileService)
        {
            _messageService = messageService;
            _photoService = photoService;
            _fileService = fileService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Photo")]
        public IActionResult Photo()
        {
            return View();
        }

        [HttpGet("File")]
        public IActionResult File()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("DecryptPhoto/{messageAndKey}")]
        public async Task<IActionResult> DecryptPhoto([FromRoute] string messageAndKey)
        {
            ViewBag.LinkOK = true;
            ViewBag.DecryptSure = $"privs.xyz/PhotoDecryptSure/{messageAndKey}";

            return View();
        }

        [HttpGet("DecryptFile/{messageAndKey}")]
        public async Task<IActionResult> DecryptFile([FromRoute] string messageAndKey)
        {
            ViewBag.LinkOK = true;
            ViewBag.DecryptSure = $"privs.xyz/FileDecryptSure/{messageAndKey}";

            return View();
        }

        [HttpGet("Decrypt/{messageAndKey}")]
        public async Task<IActionResult> DecryptMessage([FromRoute] string messageAndKey)
        {
            ViewBag.LinkOK = true;
            ViewBag.DecryptSure = $"privs.xyz/DecryptSure/{messageAndKey}";

            return View();
        }

        [HttpGet("PhotoDecryptSure/{photoAndKey}")]
        public async Task<IActionResult> PhotoDecryptSure([FromRoute] string photoAndKey)
        {
            var ipAddressv4 = HttpContext.Connection.RemoteIpAddress.MapToIPv4();
            var ipAddressv6 = HttpContext.Connection.RemoteIpAddress.MapToIPv6();

            string ipV4InString = ipAddressv4?.ToString();
            string ipV6InString = ipAddressv6?.ToString();

            string hostname = ipAddressv4?.ToString();

            try
            {
                var hostEntry = Dns.GetHostEntry(ipAddressv4?.ToString())?.HostName;
                hostname = hostEntry;
                ViewBag.host = hostEntry;
            }
            catch
            {
                hostname = ipAddressv4?.ToString();
            }

            try
            {
                string photoKeyId = photoAndKey.Split('@')[0];
                string photoKey = photoAndKey.Split('@')[1];

                ViewBag.Image = await _photoService.DeleteAndDecryptPhoto(photoKeyId, photoKey, ipV4InString, ipV6InString, hostname);
            }
            catch (Exception e)
            {
                ViewBag.Image = "Wrong address.";
            }

            return View();
        }

        [HttpGet("FileDecryptSure/{fileAndKey}")]
        public async Task<IActionResult> FileDecryptSure([FromRoute] string fileAndKey)
        {
            var ipAddressv4 = HttpContext.Connection.RemoteIpAddress.MapToIPv4();
            var ipAddressv6 = HttpContext.Connection.RemoteIpAddress.MapToIPv6();

            string ipV4InString = ipAddressv4?.ToString();
            string ipV6InString = ipAddressv6?.ToString();

            string hostname = ipAddressv4?.ToString();

            try
            {
                var hostEntry = Dns.GetHostEntry(ipAddressv4?.ToString())?.HostName;
                hostname = hostEntry;
                ViewBag.host = hostEntry;
            }
            catch
            {
                hostname = ipAddressv4?.ToString();
            }

            byte[] decryptedFile = new byte[0];
            string fileName = "decryptedFile";

            try
            {
                string fileKeyId = fileAndKey.Split('@')[0];
                string fileKey = fileAndKey.Split('@')[1];

                fileName = await _fileService.GetFileName(fileKeyId);
                decryptedFile = await _fileService.DeleteAndDecryptFile(fileKeyId, fileKey, ipV4InString, ipV6InString, hostname);
            }
            catch (Exception e)
            {
                ViewBag.Image = "Wrong address.";
            }

            return File(decryptedFile, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpGet("DecryptSure/{messageAndKey}")]
        public async Task<IActionResult> DecryptSureMessage([FromRoute] string messageAndKey)
        {
            var ipAddressv4 = HttpContext.Connection.RemoteIpAddress.MapToIPv4();
            var ipAddressv6 = HttpContext.Connection.RemoteIpAddress.MapToIPv6();

            string ipV4InString = ipAddressv4?.ToString();
            string ipV6InString = ipAddressv6?.ToString();

            string hostname = ipAddressv4?.ToString();

            try
            {
                var hostEntry = Dns.GetHostEntry(ipAddressv4?.ToString())?.HostName;
                hostname = hostEntry;
                ViewBag.host = hostEntry;
            }
            catch
            {
                hostname = ipAddressv4.ToString();
            }

            try
            {
                string messageKeyId = messageAndKey.Split('@')[0];
                string messageKey = messageAndKey.Split('@')[1];

                ViewBag.DecryptedMessage = await _messageService.DeleteAndDecryptMessage(messageKeyId, messageKey, ipV4InString, ipV6InString, hostname);
            }
            catch (Exception e)
            {
                ViewBag.DecryptedMessage = "Wrong address.";
            }

            return View();
        }

        [HttpPost("SendPhoto")]
        [RequestSizeLimit(MaxFileSizeMB)]
        public async Task<IActionResult> SendPhoto(List<IFormFile> file)
        {
            try
            {
                var ipAddressv4 = HttpContext.Connection.RemoteIpAddress.MapToIPv4();
                var ipAddressv6 = HttpContext.Connection.RemoteIpAddress.MapToIPv6();

                string ipV4InString = ipAddressv4?.ToString();
                string ipV6InString = ipAddressv6?.ToString();

                string hostname = ipAddressv4?.ToString();

                try
                {
                    var hostEntry = Dns.GetHostEntry(ipAddressv4?.ToString())?.HostName;
                    hostname = hostEntry;
                    ViewBag.host = hostEntry;
                }
                catch
                {
                    hostname = ipAddressv4.ToString();
                }

                var photoEntity = file.ElementAt(0);

                if (photoEntity.Length > MaxFileSizeMB)
                {
                    return View("TooLargeFile");
                }

                byte[] encryptedBytes = new byte[photoEntity.Length];
                if (photoEntity.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        photoEntity.CopyTo(ms);
                        encryptedBytes = ms.ToArray();
                    }
                }
                var endOfLink = await _photoService.CreateAndEncryptPhoto(encryptedBytes, ipV4InString, ipV6InString, hostname);

                ViewBag.Link = $"https://privs.xyz/decryptPhoto/{endOfLink}";

                return View();
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost("SendFile")]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSizeMB)]
        [RequestSizeLimit(MaxFileSizeMB)]
        public async Task<IActionResult> SendFile(List<IFormFile> file)
        {
            try
            {
                var ipAddressv4 = HttpContext.Connection.RemoteIpAddress.MapToIPv4();
                var ipAddressv6 = HttpContext.Connection.RemoteIpAddress.MapToIPv6();

                string ipV4InString = ipAddressv4?.ToString();
                string ipV6InString = ipAddressv6?.ToString();

                string hostname = ipAddressv4?.ToString();

                try
                {
                    var hostEntry = Dns.GetHostEntry(ipAddressv4?.ToString())?.HostName;
                    hostname = hostEntry;
                    ViewBag.host = hostEntry;
                }
                catch
                {
                    hostname = ipAddressv4.ToString();
                }

                var fileEntity = file.ElementAt(0);

                if (fileEntity.Length > MaxFileSizeMB)
                {
                    return View("TooLargeFile");
                }

                byte[] encryptedBytes = new byte[fileEntity.Length];
                if (fileEntity.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        fileEntity.CopyTo(ms);
                        encryptedBytes = ms.ToArray();
                    }
                }
                var endOfLink = await _fileService.CreateAndEncryptFile(encryptedBytes, ipV4InString, ipV6InString, hostname, fileEntity.FileName);

                ViewBag.Link = $"https://privs.xyz/decryptFile/{endOfLink}";

                return View();
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage(MessageFormModel formModel)
        {
            try
            {
                var ipAddressv4 = HttpContext.Connection.RemoteIpAddress.MapToIPv4();
                var ipAddressv6 = HttpContext.Connection.RemoteIpAddress.MapToIPv6();

                string ipV4InString = ipAddressv4?.ToString();
                string ipV6InString = ipAddressv6?.ToString();

                string hostname = "zdlajYYGg1BlfxWWSUQ5nOuubLxcNN7tIG7oJxuq";

                try
                {
                    var hostEntry = Dns.GetHostEntry(ipAddressv4?.ToString())?.HostName;
                    hostname = hostEntry;
                    ViewBag.host = hostEntry;
                }
                catch
                {
                    hostname = ipAddressv4.ToString();
                }

                var endOfLink = await _messageService.CreateAndEncryptMessage(formModel.Message, ipV4InString, ipV6InString, hostname);

                ViewBag.Link = $"https://privs.xyz/decrypt/{endOfLink}";

                return View();
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet("ip")]
        public IActionResult Ip()
        {
            var ipAddressv4 = HttpContext.Connection.RemoteIpAddress.MapToIPv4();
            var ipAddressv6 = HttpContext.Connection.RemoteIpAddress.MapToIPv6();

            ViewBag.ipv4 = ipAddressv4?.ToString();
            ViewBag.ipv6 = ipAddressv6?.ToString();

            string hostname = "zdlajYYGg1BlfxWWSUQ5nOuubLxcNN7tIG7oJxuq";

            try
            {
                var hostEntry = Dns.GetHostEntry(ipAddressv4?.ToString())?.HostName;
                hostname = hostEntry;
                ViewBag.host = hostEntry;
            }
            catch
            {
                ViewBag.host = ipAddressv4.ToString();
            }

            return View();
        }

        [HttpGet("tip")]
        public IActionResult TIP()
        {
            var ipAddressv4 = HttpContext.Connection.RemoteIpAddress.MapToIPv4();
            var ipAddressv6 = HttpContext.Connection.RemoteIpAddress.MapToIPv6();

            ViewBag.ipv4 = ipAddressv4?.ToString();
            ViewBag.ipv6 = ipAddressv6?.ToString();

            string hostname = "zdlajYYGg1BlfxWWSUQ5nOuubLxcNN7tIG7oJxuq";

            try
            {
                var hostEntry = Dns.GetHostEntry(ipAddressv4?.ToString())?.HostName;
                hostname = hostEntry;
                ViewBag.host = hostEntry;
            }
            catch
            {
                ViewBag.host = ipAddressv4.ToString();
            }

            return View();
        }

        [HttpPost("ShowMessage")]
        public IActionResult ShowMessage(MessageFormModel formModel)
        {
            ViewBag.messageText = formModel.Message;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
