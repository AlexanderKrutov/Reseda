using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Reseda
{
    /// <summary>
    /// Resource set for a locale.
    /// </summary>
    public class ResourceSet : List<ResourceItem>
    {
        public ResourceSet(string locale)
        {
            Locale = locale;
        }

        /// <summary>
        /// Locale code, for example, "en" or "ru"
        /// </summary>
        public string Locale { get; private set; }


        public string LocaleName
        {
            get
            {
                return string.IsNullOrEmpty(Locale) ? "Default" : Locale;
            }
        }

        /// <summary>
        /// Number of string resource items in the set
        /// </summary>
        public int StringsCount
        {
            get
            {
                return this.Count(i => !i.IsArrayItem && !i.IsComment);
            }
        }

        /// <summary>
        /// Number of array resources in the set
        /// </summary>
        public int ArraysCount
        {
            get
            {
                return 
                    this.Where(i => i.IsArrayItem && !i.IsComment)
                        .Select(i => i.Name).Distinct().Count();
            }
        }

        public string ValuesFolder
        {
            get
            {
                return $"values{(string.IsNullOrEmpty(Locale) ? "" : "-")}{Locale}";
            }
        }
    }
}
