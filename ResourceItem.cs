using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda
{
    /// <summary>
    /// Contains info about single resource item. 
    /// Each item is a row in CSV file.
    /// </summary>
    public class ResourceItem
    {
        /// <summary>
        /// Resource name.
        /// Usually it's Android resource item name or comment text.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Resource item value. 
        /// It can be a string resource item value or array item value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Value indicating the resource item is formatted.
        /// </summary>
        public bool IsFormatted { get; set; }

        /// <summary>
        /// Value indicating the resource item is translatable.
        /// </summary>
        public bool IsTranslatable { get; set; }

        /// <summary>
        /// Documentation text for the resource item (Android xml res attribute "documentation")
        /// </summary>
        public string Documentation { get; set; }

        /// <summary>
        /// Value indicating the resource item is comment.
        /// </summary>
        public bool IsComment { get; set; }

        /// <summary>
        /// Value indicating the resource item is an array item.
        /// </summary>
        public bool IsArrayItem { get; set; }
    }
}
