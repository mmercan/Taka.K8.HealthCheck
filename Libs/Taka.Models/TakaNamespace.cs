using System;
using System.Collections.Generic;

namespace Taka.Models
{
    public class TakaNamespace
    {
        public string Uid { get; set; }
        public string Name { get; set; }
        public List<TakaLabel> Labels { get; set; }
        public DateTime CreationTime { get; set; }
        public string Status { get; set; }
        public DateTime LatestSyncDateUTC { get; set; }

        public int DeploymentCount { get; set; }
        public int ServiceCount { get; set; }
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }
    }
}