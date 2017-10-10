using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda
{
    public class ResourceItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsFormatted { get; set; }
        public bool IsTranslatable { get; set; }
        public string Documentation { get; set; }
        public bool IsComment { get; set; }
        public bool IsArrayItem { get; set; }
    }
}
