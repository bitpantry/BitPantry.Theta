using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPantry.Theta
{
    class Constants
    {
        public static readonly char[] ELEMENT_PREFIXES = new char[]
        {
            ELEMENT_PREFIX_NAMED_PARAMETER_SWITCH,
            ELEMENT_PREFIX_BOOLEAN_SWITCH
        };

        public const char ELEMENT_PREFIX_NAMED_PARAMETER_SWITCH = '-';
        public const char ELEMENT_PREFIX_BOOLEAN_SWITCH = '/';
        public const string TAB = "     ";

        public static readonly System.Drawing.Font FONT_STANDARD = new System.Drawing.Font("Consolas", 9);

        public static readonly System.Drawing.Color COLOR_BACKGROUND = System.Drawing.Color.FromArgb(37, 37, 38);

        public static System.Drawing.Color COLOR_STANDARD_FOREGROUND = System.Drawing.Color.FromArgb(241, 241, 241);
        public static System.Drawing.Color COLOR_WARNING_FOREGROUND = System.Drawing.Color.Yellow;
        public static System.Drawing.Color COLOR_ERROR_FOREGROUND = System.Drawing.Color.Red;
        public static System.Drawing.Color COLOR_DEBUG_FOREGROUND = System.Drawing.Color.FromArgb(241, 241, 241);
        public static System.Drawing.Color COLOR_VERBOSE_FOREGROUND = System.Drawing.Color.FromArgb(128, 128, 128);

        public static System.Drawing.Color COLOR_ACCENT1_FOREGROUND = System.Drawing.Color.FromArgb(241, 241, 241); // used for table row accents
        public static System.Drawing.Color COLOR_ACCENT2_FOREGROUND = System.Drawing.Color.FromArgb(241, 241, 241); // used for highlights
        public static System.Drawing.Color COLOR_ACCENT3_FOREGROUND = System.Drawing.Color.Empty;
        public static System.Drawing.Color COLOR_ACCENT4_FOREGROUND = System.Drawing.Color.CornflowerBlue;
        public static System.Drawing.Color COLOR_ACCENT5_FOREGROUND = System.Drawing.Color.Orange;
        

        public static System.Drawing.Color COLOR_VERBOSE_HIGHLIGHT = System.Drawing.Color.Empty;
        public static System.Drawing.Color COLOR_DEBUG_HIGHLIGHT = System.Drawing.Color.Empty;
        public static System.Drawing.Color COLOR_ERROR_HIGHLIGHT = System.Drawing.Color.Empty;
        public static System.Drawing.Color COLOR_WARNING_HIGHLIGHT = System.Drawing.Color.Empty;
        public static System.Drawing.Color COLOR_STANDARD_HIGHLIGHT = System.Drawing.Color.Empty;

        public static System.Drawing.Color COLOR_ACCENT1_HIGHLIGHT = System.Drawing.Color.FromArgb(47, 47, 48);
        public static System.Drawing.Color COLOR_ACCENT2_HIGHLIGHT = System.Drawing.Color.FromArgb(17, 61, 111);
        public static System.Drawing.Color COLOR_ACCENT3_HIGHLIGHT = System.Drawing.Color.Empty;
        public static System.Drawing.Color COLOR_ACCENT4_HIGHLIGHT = System.Drawing.Color.Empty;
        public static System.Drawing.Color COLOR_ACCENT5_HIGHLIGHT = System.Drawing.Color.Empty;
    }
}
