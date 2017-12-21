using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda
{
    /// <summary>
    /// Represents single resource data item.
    /// It can be resource string, comment or even an empty row.
    /// </summary>
    public class DataItem
    {
        /// <summary>
        /// Resource name. Usually it's Android resource item name or comment text.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Meta column. Contains flags which help Reseda to process DataItem properly.
        /// </summary>
        public string Meta { get; set; } = "";

        /// <summary>
        /// Documentation text for the resource item (Android xml res attribute "documentation").
        /// </summary>
        public string Documentation { get; set; }

        /// <summary>
        /// Values dictionary. Key is locale code, value is resource item value for corresponding language. 
        /// </summary>
        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Value indicating the resource item is unused.
        /// </summary>
        public bool IsUnused
        {
            get
            {
                return Meta.Contains(UNUSED);
            }
        }

        /// <summary>
        /// Value indicating the resource item is formatted.
        /// </summary>
        public bool IsFormatted
        {
            get
            {
                return !Meta.Contains(FORMATTED);
            }
        }

        /// <summary>
        /// Value indicating the resource item is translatable.
        /// </summary>
        public bool IsTranslatable
        {
            get
            {
                return !Meta.Contains(TRANSLATABLE);
            }
        }

        /// <summary>
        /// Value indicating the resource item is comment.
        /// </summary>
        public bool IsComment
        {
            get
            {
                return Meta.Contains(COMMENT);
            }
        }

        /// <summary>
        /// Value indicating the resource item is an empty row
        /// </summary>
        public bool IsEmptyRow
        {
            get
            {
                return string.IsNullOrWhiteSpace(Meta) &&
                    Values.Values.All(v => string.IsNullOrWhiteSpace(v)) &&
                    string.IsNullOrWhiteSpace(Documentation);
            }
        }

        /// <summary>
        /// Returns true if the item contains payload data, i.e. not a comment, empty or unused.
        /// </summary>
        public bool IsResource
        {
            get
            {
                return !IsComment && !IsEmptyRow && !IsUnused;
            }
        }

        #region Meta 

        /// <summary>
        /// Means resource item is not used (will be omitted during processing).
        /// </summary>
        public const string UNUSED = "-";

        /// <summary>
        /// Means resource item should be processed as a comment.
        /// </summary>
        public const string COMMENT = "#";

        /// <summary>
        /// Means resource item should is unFormatted (Android res attribute 'formatted="false"')
        /// </summary>
        public const string FORMATTED = "f";

        /// <summary>
        /// Means resource item should is unTranslatable (Android res attribute 'transatable="false"')
        /// </summary>
        public const string TRANSLATABLE = "t";

        #endregion Meta 
    }
}
