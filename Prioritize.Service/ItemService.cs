using Prioritize.Data;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using Prioritize.Models;

namespace Prioritize.Service
{
    public class ItemService
    {
        private IHttpContextAccessor _httpContextAccessor;
        private ItemRepository _itemRepository;
        private PriorityLevelService _priorityLevelService;
        private AppConfig _appConfig;
        private EmailService _emailService;
        private List<LPriorityLevel> PriorityLevelTexts;
        private List<LCoder> Coders;
        private UserService _userService;

        public ItemService(UserService userService, PriorityLevelService priorityLevelService, IHttpContextAccessor httpContextAccessor, ItemRepository itemRepository, AppConfigService appConfigService, EmailService emailService)
        {
            _userService = userService;
            _priorityLevelService = priorityLevelService;
            _emailService = emailService;
            _appConfig = appConfigService.AppConfig;
            _itemRepository = itemRepository;
            _httpContextAccessor = httpContextAccessor;
            PriorityLevelTexts = _priorityLevelService.GetLPriorityLevels().Where(c => c.Active).ToList();
            Coders = _userService.GetCoders();
        }

        public List<DItem> GetItemsByView(List<string> view, List<int> statuses)
        {
            return _itemRepository.GetByView(GetViewItemsWhereClause(view, statuses, _httpContextAccessor.HttpContext.User.Identity.Name)).ToList();
        }

        public DItem GetItemById(int Id)
        {
            return _itemRepository.GetById(Id);
        }
        public int GetNextPriorityNumber()
        {
            return _itemRepository.GetNextPriorityNumber();
        }

        public void SaveOrder(IEnumerable<DItem> Items)
        {
            var currentItems = _itemRepository.GetAll(false).Where(c => Items.Select(d => d.Id).Contains(c.Id)).ToList();

            foreach (var item in Items)
            {
                var currentItem = currentItems.SingleOrDefault(c => c.Id == item.Id);
                if (currentItem != null)
                {
                    currentItem.PriorityNumber = item.PriorityNumber;
                }
            }
            _itemRepository.SaveChanges();
        }


        public void SaveItems(List<DItem> Items)
        {
            var currentItems = _itemRepository.GetAll(false).Where(c => Items.Select(d => d.Id).Contains(c.Id)).ToList();
            var needAnnouncing = new Dictionary<int, List<int>>();



            foreach (var item in Items)
            {

                var currentItem = currentItems.SingleOrDefault(c => c.Id == item.Id);

                if (item.CoderId.HasValue && item.CoderId != currentItem.CoderId)
                {
                    if (needAnnouncing.Keys.Contains(item.CoderId.Value))
                    {
                        if (needAnnouncing[item.CoderId.Value] == null)
                        {
                            needAnnouncing[item.CoderId.Value] = new List<int>()
                            {
                                item.Id
                            };
                        }
                        else
                        {
                            needAnnouncing[item.CoderId.Value].Add(item.Id);
                        }
                    }
                    else
                    {
                        needAnnouncing.Add(item.CoderId.Value, new List<int>());
                        needAnnouncing[item.CoderId.Value].Add(item.Id);
                    }
                }


                if (currentItem != null)
                {
                    currentItem.PriorityNumber = item.PriorityNumber;
                    currentItem.CoderId = item.CoderId;
                    currentItem.Active = item.Active;
                    currentItem.StatusId = item.StatusId;
                    currentItem.PriorityLevelId = item.PriorityLevelId;
                    currentItem.DueDate = item.DueDate;
                }
            }

            var index = 1;

            foreach (var ditem in _itemRepository.GetAll(false).Where(c => c.Active).OrderBy(c => c.PriorityNumber))
            {
                if (ditem.StatusId == 1)
                {
                    ditem.PriorityNumber = index++;
                }
                else
                {
                    ditem.PriorityNumber = 0;
                }
            }

            _itemRepository.SaveChanges();
            SendUpdatedEmail(needAnnouncing);




        }

        private void SendUpdatedEmail(Dictionary<int, List<int>> needAnnouncing)
        {
            foreach (var coderId in needAnnouncing.Keys)
            {
                var Subject = $"A new CRB Task has been assigned.";
                var Body = $"A new CRB Task has been assigned.<p/>";

                if (needAnnouncing[coderId].Count > 1)
                {
                    Subject = $"New CRB Tasks have been assigned.";
                    Body = $"New CRB Tasks have been assigned.<p/>";
                }

                var items = _itemRepository.GetAll(false).Where(c => needAnnouncing[coderId].Contains(c.Id)).ToList();

                Body = $"{Body}{ConvertItemToHTML(items)}";

                _emailService.SendEmail(new List<string>() { Coders.Single(c=> c.Id == items[0].CoderId).Email}, Subject, Body);
            }
        }

        public DItem UpdateItem(DItem item)
        {
            var PriorityLevelTexts = _priorityLevelService.GetLPriorityLevels().Where(c => c.Active);
            var originalCoderId = _itemRepository.GetById(item.Id)?.CoderId;

            if (item.CoderId != originalCoderId)
            {
                var PriorityLevelText = PriorityLevelTexts.Single(c => c.Id == item.PriorityLevelId).Text;

                var Subject = $"A CRB Task has been Assigned. Priority Level of {PriorityLevelText}.";
                var Body = $"A CRB Task has been Assigned.<p/>{ConvertItemToHTML(item)}";

                _emailService.SendEmail(Coders.Single(c => c.Id == item.CoderId).Email, Subject, Body);
            }

            return _itemRepository.UpdateItem(item);
        }

        public DItem CreateItem(DItem item)
        {
            if (item.StatusId != 1) item.PriorityNumber = 0;

            var returnItem = _itemRepository.CreateItem(item);
            SendCreatedEmail(returnItem);
            return returnItem;
        }

        private void SendCreatedEmail(DItem item)
        {
            var Subject = $"A new CRB Task has been created. Priority Level of {item.PriorityLevel.Text}.";
            var Body = $"A new CRB Task has been created.<p/>{ConvertItemToHTML(item)}";

            _emailService.SendEmail(_appConfig.NewItemCreatedEmail, Subject, Body);

        }

        private string ConvertItemToHTML(List<DItem> items)
        {
            string body = string.Empty;
            foreach (var item in items)
            {
                body = body + ConvertItemToHTML(item);
            }
            return body;

        }
        private string ConvertItemToHTML(DItem item)
        {
            string body = string.Empty;
            var PriorityLevelText = PriorityLevelTexts.Single(c => c.Id == item.PriorityLevelId).Text;
            body = body + $"" +
                $"Priority Level: {PriorityLevelText} <br/>" +
                $"Priority Number: {item.PriorityNumber}<br/>" +
                $"Due Date: {item.DueDate?.ToShortDateString()}<br/>" +
                $"Description: {item.Description} <br/>" +
                $"Click <a href='{_appConfig.HostUrl}/{item.Id}'>Here</a> to open the Task<br/>" +
                $"<hr/>";

            return body;

        }


        private Expression<Func<DItem, bool>> GetViewItemsWhereClause(List<string> view, List<int> statuses, string userName)
        {
            if (view[0].Contains("View"))
            {
                switch (view[0])
                {
                    case "ViewAll":
                        return c => c.Active && (statuses.Contains(c.StatusId) || statuses.Contains(0));
                    case "ViewMine":
                        return c => c.Active && c.Coder.UserName == userName && (statuses.Contains(c.StatusId) || statuses.Contains(0));
                    case "ViewUnassigned":
                        return c => c.Active && c.Coder == null && (statuses.Contains(c.StatusId) || statuses.Contains(0));
                    default:
                        return c => c.Active && (statuses.Contains(c.StatusId) || statuses.Contains(0));
                }
            }
            else
            {
                return c => c.Active && (c.CoderId.HasValue && view.Contains(c.CoderId.Value.ToString())) && (statuses.Contains(c.StatusId) || statuses.Contains(0));
            }

        }

    }
}
