using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Prioritize.Data;
using Prioritize.Models;
using Prioritize.Service;
using Prioritize.Web.Models;


namespace Prioritize.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ItemService _itemService;
        private readonly PriorityLevelService _priorityLevelService;
        private readonly UserService _userService;
        private readonly StatusService _statusService;
        private readonly AppConfig _appConfig;
        private readonly EmailService _emailService;

        public HomeController(AppConfigService appconfigservice, EmailService emailService, ItemService itemService, PriorityLevelService priorityLevelService, UserService userService, StatusService statusService)
        {
            _emailService = emailService;
            _appConfig = appconfigservice.AppConfig;
            _statusService = statusService;
            _userService = userService;
            _priorityLevelService = priorityLevelService;
            _itemService = itemService;
        }


        public IActionResult Index(int id, string view, string status)
        {

            ViewBag.IsIndex = true;
            ViewBag.EditItem = id;
            ViewBag.CoderId = _userService.GetCoderByUserName(User.Identity.Name)?.Id;
            ViewBag.HostUrl = _appConfig.HostUrl;
            ViewBag.User = _userService.GetEmployeeByUserName(User.Identity.Name);
            return View();
        }

        [HttpPost]
        [Route("CreateItem")]
        public IActionResult CreateItem() {


            try
            {
                ViewBag.Coders = GetCodersSelectList();
                ViewBag.PriorityLevel = GetPriorityLevelsSelectList();
                ViewBag.Statuses = GetStatusesSelectList();
                ViewBag.CanAssign = CanAssign();

                var vm = new DItem();
                vm.StatusId = 1;
                vm.Status = _statusService.GetStatusById(1);
                vm.PriorityNumber = _itemService.GetNextPriorityNumber();
                return View("_EditPartial", vm);
            }
            catch (Exception ex)
            {
                _emailService.SendEmail(new List<string>() { "" }, "PrioritizeIt Error", ex.Message);
                return Json(new { Success = false, Message = $"That didn't go as planned.  Here's your Error Message: </br>{ex.ToString()}" });
                throw;
            }

        }

        [HttpPost]
        public IActionResult EditItem(int Id)
        {
            try
            {
                ViewBag.Coders = GetCodersSelectList();
                ViewBag.PriorityLevel = GetPriorityLevelsSelectList();
                ViewBag.Statuses = GetStatusesSelectList();
                ViewBag.CanAssign = CanAssign();
                var vm = _itemService.GetItemById(Id);
                return View("_EditPartial", vm);

            }
            catch (Exception ex)
            {
                _emailService.SendEmail(new List<string>() { "" }, "PrioritizeIt Error", ex.Message);
                return Json(new { Success = false, Message = $"That didn't go as planned.  Here's your Error Message: </br>{ex.ToString()}" });
                throw;
            }

        }

        [HttpPost]
        [Models.RequestSizeLimit(valueCountLimit: int.MaxValue)]
        public JsonResult SavePage([FromBody]SavePageViewModel savePageViewModel)
        {
            try
            {
                
                _itemService.SaveItems(savePageViewModel.Items.Select(c => new DItem()
                {
                    Id = c.Id,
                    Active = c.Active,
                    PriorityNumber = c.PriorityNumber,
                    StatusId = c.StatusId,
                    PriorityLevelId = c.PriorityLevelId,
                    CoderId = c.CoderId,
                    DueDate = c.DueDate,
                    
                }).ToList());

                return GetView(savePageViewModel.View, savePageViewModel.Status, "Changes Saved", true);
            }
            catch (Exception ex)
            {
                _emailService.SendEmail(new List<string>() { "" }, "PrioritizeIt Error", ex.Message);
                return Json(new { Success = false, Message = $"That didn't go as planned.  Here's your Error Message: </br>{ex.ToString()}" });
                throw;
            }
        }

        [HttpPost]
        [Models.RequestSizeLimit(valueCountLimit: int.MaxValue)]
        public JsonResult SaveOrder([FromBody]SavePageViewModel savePageViewModel)
        {
           
            try
            {
                
                _itemService.SaveOrder(savePageViewModel.Items.Select(c => new DItem()
                {
                    PriorityNumber = c.PriorityNumber,
                }));

                return Json(new { Success = true, Message = "Reordering Saved" });
            }
            catch (Exception ex)
            {
                _emailService.SendEmail(new List<string>() { "" }, "PrioritizeIt Error", ex.Message);
                return Json(new { Success = false, Message = $"Welp. That didn't work out. One Error Message Coming up!<br/>{ex.Message}" });
                throw;
            }

        }


        [HttpPost]
        public JsonResult GetView(string view, List<int> statuses, string message, bool success)
        {
            try
            {

                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.None,
                    TypeNameHandling = TypeNameHandling.None,
                };
                var vm = new DTViewViewModel()
                {
                    Coders = GetCodersSelectList(),
                    CanAssign = CanAssign(),
                    PriorityList = GetPriorityLevelsSelectList(),
                    Statuses = GetStatusesSelectList(),
                    items = _itemService.GetItemsByView(view, statuses),
                    View = view,
                    ViewText = view.Replace("View", ""),
                    Status = statuses,
                    Success = string.IsNullOrEmpty(message) ? true: success,
                    Message = message
                };

                return Json(vm, settings);
            }
            catch (Exception ex)
            {
                _emailService.SendEmail(new List<string>() { "" }, "Prioritize It Error", ex.Message);
                return Json( new { Success = false, Message = $"Your Car has been Towed.  Ok, not really but something did go wrong.  I present to you your Error Message<br/>{ex.Message}"});
                throw;
            }

        }


        [HttpPost]
        public JsonResult SaveItem(DItem item)
        {
            try
            {
               
                var eventAction = string.Empty;
                var eventType = string.Empty;


                if (ModelState.IsValid)
                {
                    var newItem = new DItem();

                    if (item.Id == 0)
                    {
                        eventType = "Created";
                        eventAction = "Creating";
                        newItem = _itemService.CreateItem(item);
                    }
                    else
                    {
                        eventType = "Updated";
                        eventAction = "Updating";
                        newItem = _itemService.UpdateItem(item);
                    }

                    return Json(new { Success = true, Data = newItem, Message = $"Item {newItem.Id} has been  {eventType} with a Priority of {newItem.PriorityNumber} and a Status of {newItem.Status.Text}" });
                }

                var errors = $"<ul>{ModelState.Values.Select(c => c.Errors.Select(d => $"<li class='errorItem'>{d.ErrorMessage}</li>")).ToList()}</ul>";

                return Json(new { Success = false, ErrorType = "ModelInvalid", Message = $"The following Errors occurred while {eventAction} your Item: <br>{errors}" });
            }
            catch (Exception ex)
            {
                _emailService.SendEmail(new List<string>() { "" }, "PrioritizeIt Error", ex.Message);
                return Json(new { Success = false, ErrorType = "Fatal" , Message = $"This is the part where I say something witty about how this didn't work. I've run out of witty things so  &lt;Insert Witty Comment Here&gt;  &lt;Insert Your Assumed Chuckle Here&gt; and here's your Error Message:<br/>{ex.Message}" });
                throw;
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<SelectListItem> GetCodersSelectList()
        {
            return _userService.GetCoders().Select(c => new SelectListItem()
            {
                Text = c.DisplayNameReversed,
                Value = c.Id.ToString()
            }).ToList();
        }

        private List<SelectListItem> GetStatusesSelectList()
        {
            return _statusService.GetStatuses().Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Text }).ToList();
        }

        private List<SelectListItem> GetPriorityLevelsSelectList()
        {
            return _priorityLevelService.GetLPriorityLevels().Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Text }).ToList();
        }

        private bool CanAssign() {
            return true;
        }
    }
}
