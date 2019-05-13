
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Prioritize.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prioritize.Service
{
    public class AppConfigService 
    {
        public AppConfig AppConfig { get;  set; }

        public AppConfigService(IOptionsSnapshot<AppConfig> values, IHostingEnvironment Environment)
        {
            AppConfig = values.Value;
            AppConfig.Environment = Environment.EnvironmentName;
        }


    }
}
