using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrivsXYZ.Helpers;
using PrivsXYZ.Models;
using PrivsXYZ.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PrivsXYZ.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly IPhotoService _photoService;
        private readonly IFileService _fileService;
        private readonly IEntryCounterService _entryCounterService;

        private readonly IUserDataHelper _userDataHelper;

        private const int MaxFileSizeMB = 5 * 1024 * 1024;

        public HomeController(IMessageService messageService, IPhotoService photoService, IFileService fileService,
            IEntryCounterService entryCounterService, IUserDataHelper userDataHelper)
        {
            _messageService = messageService;
            _photoService = photoService;
            _fileService = fileService;
            _entryCounterService = entryCounterService;

            _userDataHelper = userDataHelper;
        }

        public async Task<IActionResult> Index()
        {
            var userData = _userDataHelper.GetUserData();
            await _entryCounterService.RegisterSiteEnter(userData.Item1, userData.Item2, userData.Item3, "Index");

            return View();
        }

        [HttpGet("GroupMessage")]
        public async Task<IActionResult> GroupMessage()
        {
            var userData = _userDataHelper.GetUserData();
            await _entryCounterService.RegisterSiteEnter(userData.Item1, userData.Item2, userData.Item3, "GroupMessage");


            return View();
        }

        [HttpGet("Photo")]
        public async Task<IActionResult> Photo()
        {
            var userData = _userDataHelper.GetUserData();
            await _entryCounterService.RegisterSiteEnter(userData.Item1, userData.Item2, userData.Item3, "Photo");

            return View();
        }

        [HttpGet("File")]
        public async Task<IActionResult> File()
        {
            var userData = _userDataHelper.GetUserData();
            await _entryCounterService.RegisterSiteEnter(userData.Item1, userData.Item2, userData.Item3, "GroupMessage");

            return View();
        }

        [HttpGet("DecryptPhoto/{messageAndKey}")]
        public async Task<IActionResult> DecryptPhoto([FromRoute] string messageAndKey)
        {
            var userData = _userDataHelper.GetUserData();
            await _entryCounterService.RegisterSiteEnter(userData.Item1, userData.Item2, userData.Item3, "DecryptPhoto");

            ViewBag.LinkOK = true;
            ViewBag.DecryptSure = $"privs.xyz/PhotoDecryptSure/{messageAndKey}";

            return View();
        }

        [HttpGet("DecryptFile/{messageAndKey}")]
        public async Task<IActionResult> DecryptFile([FromRoute] string messageAndKey)
        {
            var userData = _userDataHelper.GetUserData();
            await _entryCounterService.RegisterSiteEnter(userData.Item1, userData.Item2, userData.Item3, "DecryptFile");

            ViewBag.LinkOK = true;
            ViewBag.DecryptSure = $"privs.xyz/FileDecryptSure/{messageAndKey}";

            return View();
        }

        [HttpGet("Decrypt/{messageAndKey}")]
        public async Task<IActionResult> DecryptMessage([FromRoute] string messageAndKey)
        {
            var userData = _userDataHelper.GetUserData();
            await _entryCounterService.RegisterSiteEnter(userData.Item1, userData.Item2, userData.Item3, "DecryptMessage");

            ViewBag.LinkOK = true;
            ViewBag.DecryptSure = $"privs.xyz/DecryptSure/{messageAndKey}";

            return View();
        }

        [HttpGet("PhotoDecryptSure/{photoAndKey}")]
        public async Task<IActionResult> PhotoDecryptSure([FromRoute] string photoAndKey)
        {
            var userData = _userDataHelper.GetUserData();
            await _entryCounterService.RegisterSiteEnter(userData.Item1, userData.Item2, userData.Item3, "PhotoDecryptSure");

            try
            {
                string photoKeyId = photoAndKey.Split('@')[0];
                string photoKey = photoAndKey.Split('@')[1];

                ViewBag.Image = await _photoService.DeleteAndDecryptPhoto(photoKeyId, photoKey, userData.Item1,
                    userData.Item2, userData.Item3);
            }
            catch (Exception)
            {
                ViewBag.Image = "Wrong address.";
            }

            return View();
        }

        [HttpGet("FileDecryptSure/{fileAndKey}")]
        public async Task<IActionResult> FileDecryptSure([FromRoute] string fileAndKey)
        {
            var userData = _userDataHelper.GetUserData();
            await _entryCounterService.RegisterSiteEnter(userData.Item1, userData.Item2, userData.Item3, "PhotoDecryptSure");

            byte[] decryptedFile = new byte[0];
            string fileName = "decryptedFile";

            try
            {
                string fileKeyId = fileAndKey.Split('@')[0];
                string fileKey = fileAndKey.Split('@')[1];

                fileName = await _fileService.GetFileName(fileKeyId);
                decryptedFile = await _fileService.DeleteAndDecryptFile(fileKeyId, fileKey, userData.Item1, userData.Item2, userData.Item3);
            }
            catch (Exception)
            {
                ViewBag.Image = "Wrong address.";
            }

            return File(decryptedFile, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpGet("DecryptSure/{messageAndKey}")]
        public async Task<IActionResult> DecryptSureMessage([FromRoute] string messageAndKey)
        {
            var userData = _userDataHelper.GetUserData();
            await _entryCounterService.RegisterSiteEnter(userData.Item1, userData.Item2, userData.Item3, "MessageDecryptSure");

            try
            {
                string messageKeyId = messageAndKey.Split('@')[0];
                string messageKey = messageAndKey.Split('@')[1];

                ViewBag.DecryptedMessage = await _messageService.DeleteAndDecryptMessage(messageKeyId, messageKey,
                    userData.Item1, userData.Item2, userData.Item3);
            }
            catch (Exception)
            {
                ViewBag.DecryptedMessage = "Wrong address.";
            }

            return View();
        }

        [HttpPost("SendPhoto")]
        [RequestSizeLimit(MaxFileSizeMB)]
        public async Task<IActionResult> SendPhoto(List<IFormFile> file)
        {
            var userData = _userDataHelper.GetUserData();
            await _entryCounterService.RegisterSiteEnter(userData.Item1, userData.Item2, userData.Item3, "SendPhoto");

            try
            {
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
                var endOfLink = await _photoService.CreateAndEncryptPhoto(encryptedBytes, userData.Item1, userData.Item2, userData.Item3);

                ViewBag.Link = $"https://privs.xyz/decryptPhoto/{endOfLink}";

                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost("SendFile")]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSizeMB)]
        [RequestSizeLimit(MaxFileSizeMB)]
        public async Task<IActionResult> SendFile(List<IFormFile> file)
        {
            var userData = _userDataHelper.GetUserData();
            await _entryCounterService.RegisterSiteEnter(userData.Item1, userData.Item2, userData.Item2, "SendFile");

            try
            {
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
                var endOfLink = await _fileService.CreateAndEncryptFile(encryptedBytes, userData.Item1, userData.Item2, userData.Item3,
                    fileEntity.FileName);

                ViewBag.Link = $"https://privs.xyz/decryptFile/{endOfLink}";

                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage(MessageFormModel formModel)
        {
            try
            {
                var userData = _userDataHelper.GetUserData();
                await _entryCounterService.RegisterSiteEnter(userData.Item1, userData.Item2, userData.Item2, "SendFile");

                var endOfLink = await _messageService.CreateAndEncryptMessage(formModel.Message, userData.Item1, userData.Item2,
                    userData.Item3);

                ViewBag.Link = $"https://privs.xyz/decrypt/{endOfLink}";

                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet("ip")]
        public async Task<IActionResult> Ip()
        {
            var userData = _userDataHelper.GetUserData();
            await _entryCounterService.RegisterSiteEnter(userData.Item1, userData.Item2, userData.Item3, "Ip");

            ViewBag.ipv4 = userData.Item1;
            ViewBag.ipv6 = userData.Item2;
            ViewBag.host = userData.Item3;

            return View();
        }

        [HttpGet("tip")]
        public async Task<IActionResult> TIP()
        {
            var userData = _userDataHelper.GetUserData();
            await _entryCounterService.RegisterSiteEnter(userData.Item1, userData.Item2, userData.Item3, "Ip");

            ViewBag.ipv4 = userData.Item1;
            ViewBag.ipv6 = userData.Item2;
            ViewBag.host = userData.Item3;

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
