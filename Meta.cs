using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reseda
{
    /// <summary>
    /// Contains constants values used in META column in CSV file.
    /// </summary>
    public static class Meta
    {
        /// <summary>
        /// Means resource item is not used (will be omitted during processing).
        /// </summary>
        public const string UNUSED         = "-";

        /// <summary>
        /// Means resource item should be processed as a comment.
        /// </summary>
        public const string COMMENT        = "#";

        /// <summary>
        /// Means resource item is an array item.
        /// Combination with # symbol means it's a comment for an array.
        /// </summary>
        public const string ARRAY          = "a";

        /// <summary>
        /// Means resource item should is unFormatted (Android res attribute 'formatted="false"')
        /// </summary>
        public const string FORMATTED      = "f";

        /// <summary>
        /// Means resource item should is unTranslatable (Android res attribute 'transatable="false"')
        /// </summary>
        public const string TRANSLATABLE   = "t";
    }
}
