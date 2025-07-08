using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLink.Core.models
{
    public class JobFilter
    {
        public string? Search { get; set; }
        public string? Distance { get; set; }
        public string? StartPlace { get; set; }
        public string? EndPlace { get; set; }
    }
}
