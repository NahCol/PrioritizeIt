using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace Prioritize.Data
{
    public class ItemRepository
    {
        private PrioritizeDatabaseContext _prioritizeDatabaseContext;

        public ItemRepository(PrioritizeDatabaseContext prioritizeDatabaseContext)
        {           
            _prioritizeDatabaseContext = prioritizeDatabaseContext;
        }

        public IEnumerable<DItem> GetByView(Expression<Func<DItem, bool>> whereClause)
        {

            var coders = GetCoders().Where(c => c.DItems.Any()).Select(c=> new LCoder() {
                DisplayName =c.DisplayName,
                Email = c.Email,
                Id = c.Id,
                UserName = c.UserName
            }).ToList();
            var priorityLevels = _prioritizeDatabaseContext.LPriorityLevels.Where(c => c.Active && c.DItems.Any()).Select(c => new LPriorityLevel()
            {
                Active = c.Active,
                SortOrder = c.SortOrder,                
                Id = c.Id,
                Text = c.Text
            }).ToList();
            var statuses = _prioritizeDatabaseContext.LStatuses.Where(c => c.DItems.Any()).Select(c => new LStatus()
            {                
                Id = c.Id,
                Text = c.Text
            }).ToList();

            var returnobj = _prioritizeDatabaseContext.DItems.Where(whereClause)
                .Select(c=> new DItem() {
                    Action = c.Action,
                    Active = c.Active,
                    Board = c.Board,
                    CardNumber = c.CardNumber,
                    Coder = coders.SingleOrDefault(d=> d.UserName == c.Coder.UserName),
                    CoderId = c.CoderId,
                    Description = c.Description,
                    Id = c.Id,
                    Link = c.Link,
                    List = c.List,
                    PriorityLevel = priorityLevels.SingleOrDefault(d=> d.Id == c.PriorityLevel.Id),
                    PriorityLevelId = c.PriorityLevelId,
                    PriorityNumber = c.PriorityNumber,
                    Requirement = c.Requirement,
                    Status = statuses.SingleOrDefault(d=> d.Id == c.Status.Id),
                    StatusId = c.StatusId,
                    DueDate = c.DueDate
                })
                .OrderBy(c => c.PriorityNumber);

            return returnobj;
        }
        private IEnumerable<LCoder> GetCoders()
        {
            return _prioritizeDatabaseContext.LCoders
                .Join(_prioritizeDatabaseContext.DEQEmps,
                Coder => Coder.UserName,
                DEQEmp => DEQEmp.UserName, (Coder, DEQEmp) => new LCoder()
                {
                    Id = Coder.Id,
                    UserName = Coder.UserName,
                    Email = DEQEmp.Email,
                    DisplayName = DEQEmp.GoBy,
                    DItems = Coder.DItems
                });
        }
        public int GetNextPriorityNumber()
        {
            return _prioritizeDatabaseContext.DItems.Where(c=> c.StatusId== 1).Max(c => c.PriorityNumber)+1;
        }
        public IEnumerable<DItem> GetAll(bool includeInactive)
        {
            return _prioritizeDatabaseContext.DItems.Where(c => c.Active || includeInactive).OrderBy(c => c.PriorityNumber);
        }

        public DItem CreateItem(DItem item)
        {
            item.Active = true;
            _prioritizeDatabaseContext.DItems.Add(item);
            var Index = 1;

            foreach (var ditem in _prioritizeDatabaseContext.DItems.Where(c=>c.Active).OrderBy(c => c.PriorityNumber).ThenByDescending(c => c.Id))
            {
                ditem.PriorityNumber = Index++;
            }
            SaveChanges();

            return GetById(item.Id);
        }

        public DItem UpdateItem(DItem item)
        {
            var oldItem = _prioritizeDatabaseContext.DItems.Single(c => c.Id == item.Id);

            oldItem.List = item.List;
            oldItem.PriorityLevelId = item.PriorityLevelId;
            oldItem.PriorityNumber = item.PriorityNumber;
            oldItem.Requirement = item.Requirement;
            oldItem.Link = item.Link;
            oldItem.Action = item.Action;
            oldItem.Board = item.Board;
            oldItem.CardNumber = item.CardNumber;
            oldItem.CoderId = item.CoderId;
            oldItem.StatusId = item.StatusId;
            oldItem.DueDate = item.DueDate;
            oldItem.Description = item.Description;

            SaveChanges();
            return GetById(oldItem.Id);
        }


        public DItem GetById(int Id)
        {
            var coders = GetCoders().Where(c => c.DItems.Any()).Select(c => new LCoder()
            {
                DisplayName = c.DisplayName,
                Email = c.Email,
                Id = c.Id,
                UserName = c.UserName
            }).ToList();
            var priorityLevels = _prioritizeDatabaseContext.LPriorityLevels.Where(c => c.Active && c.DItems.Any()).Select(c => new LPriorityLevel()
            {
                Active = c.Active,
                SortOrder = c.SortOrder,
                Id = c.Id,
                Text = c.Text
            }).ToList();
            var statuses = _prioritizeDatabaseContext.LStatuses.Where(c => c.DItems.Any()).Select(c => new LStatus()
            {
                Id = c.Id,
                Text = c.Text
            }).ToList();


            var returnobj = _prioritizeDatabaseContext.DItems.Where(c => c.Id == Id)
                .Select(c => new DItem()
                {
                    Action = c.Action,
                    Active = c.Active,
                    Board = c.Board,
                    CardNumber = c.CardNumber,
                    Coder = coders.SingleOrDefault(d => d.UserName == c.Coder.UserName),
                    CoderId = c.CoderId,
                    Description = c.Description,
                    Id = c.Id,
                    Link = c.Link,
                    List = c.List,
                    PriorityLevel = priorityLevels.SingleOrDefault(d => d.Id == c.PriorityLevel.Id),
                    PriorityLevelId = c.PriorityLevelId,
                    PriorityNumber = c.PriorityNumber,
                    Requirement = c.Requirement,
                    Status = statuses.SingleOrDefault(d => d.Id == c.Status.Id),
                    StatusId = c.StatusId,
                    DueDate = c.DueDate
                }).Single();

            return returnobj;
        }

        public int SaveChanges() {
            return _prioritizeDatabaseContext.SaveChanges();
        }


    }
}
