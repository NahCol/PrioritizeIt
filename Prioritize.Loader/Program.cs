using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using Prioritize.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Prioritize.Loader
{
    public class Program
    {
        private const string path = @"c:\temp\TeamPrioritize-JW-TA.xlsx";
        public static IConfigurationRoot configuration;
        public static PrioritizeDatabaseContext _context;
        public static void Main(string[] args)
        {

            // Create service collection
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            _context = serviceProvider.GetService<PrioritizeDatabaseContext>();
            LoadPriorities();

        }
        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Build configuration
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();


            // Add access to generic IConfigurationRoot
            serviceCollection.AddSingleton(configuration);
            serviceCollection.AddDbContext<PrioritizeDatabaseContext>(conf => conf.UseSqlServer(configuration.GetConnectionString("DefaultDatabase")));

        }


        private static void LoadPriorities()
        {
            if (File.Exists(path))
            {
                var PriorityLevelList = GetPriorityLevels();
                var TempList = new List<ItemWrapper>();
                var package = new ExcelPackage(new FileInfo(path));
                ExcelWorksheet sheet = package.Workbook.Worksheets[1];
                var rows = sheet.Cells.Select(cell => cell.Start.Row).Distinct().OrderBy(x => x);

                var dItems = rows.Skip(1).Select(row =>
                {
                    var item = new ItemWrapper();
                    item.TempPriorityNumber = sheet.Cells[row, 5].GetValue<decimal>();
                    item.Board = sheet.Cells[row, 1].GetValue<string>() ?? string.Empty;
                    item.List = sheet.Cells[row, 2].GetValue<string>() ?? string.Empty;
                    item.Action = sheet.Cells[row, 3].GetValue<string>() ?? string.Empty;
                    item.CardNumber = sheet.Cells[row, 4].GetValue<string>() ?? string.Empty;
                    item.PriorityNumber = 0;
                    item.Requirement = sheet.Cells[row, 6].GetValue<string>() ?? string.Empty;
                    item.PriorityLevel = PriorityLevelList.SingleOrDefault(c=> c.Text.Equals(sheet.Cells[row, 7].GetValue<string>()));
                    item.Link = sheet.Cells[row, 8].GetValue<string>();


                    Console.WriteLine($"Adding {item.Board} {item.List} {item.PriorityLevel.Text} {item.PriorityNumber} {item.Board}");

                    return item;

                });

                TempList.AddRange(dItems);

                var dItemsOrder = TempList.OrderBy(c => c.PriorityLevel.SortOrder).ThenBy(c => c.TempPriorityNumber).ToList();

                foreach (var item in dItemsOrder)
                {
                    item.PriorityNumber = dItemsOrder.IndexOf(item) + 1;
                    var dItem = new DItem()
                    {
                        Action = item.Action,
                        Board = item.Board,
                        List = item.List,
                        CardNumber = item.CardNumber,
                        PriorityNumber = item.PriorityNumber,
                        Requirement = item.Requirement,
                        PriorityLevel = item.PriorityLevel,
                        Coder = null,
                        Link = item.Link,
                        Active = true,
                        Description = MakeDescription(item.Link),
                        StatusId = 1
                       
                    };
                    _context.DItems.Add(dItem);


                }
                _context.SaveChanges();
            };


        }


        private static string MakeDescription(string link)
        {
            var description = string.Empty;
            if (!string.IsNullOrEmpty(link) && link.Contains("trello"))
            {
                description = link.Split('/', options: StringSplitOptions.RemoveEmptyEntries).Last();
                description = description.Substring(description.IndexOf("-")).Replace("-", " ");
            }
            return description;
        }

        private static List<LPriorityLevel> GetPriorityLevels()
        {
            return _context.LPriorityLevels.ToList();
        }

        public class Column : System.Attribute
        {
            public int ColumnIndex { get; set; }


            public Column(int column)
            {
                ColumnIndex = column;
            }
        }

    }
}
