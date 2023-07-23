using Serilog.Sinks.RichTextBox.Themes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rpa_robot.Classes
{
    internal class LoggerTheme
    {
        public static RichTextBoxConsoleTheme Theme { get; } = new RichTextBoxConsoleTheme
        (
            new Dictionary<RichTextBoxThemeStyle, RichTextBoxConsoleThemeStyle>
            {
                [RichTextBoxThemeStyle.Text] = new RichTextBoxConsoleThemeStyle { Foreground = ColorToHtmlColorCode(Color.CornflowerBlue) },
                [RichTextBoxThemeStyle.SecondaryText] = new RichTextBoxConsoleThemeStyle { Foreground = ColorToHtmlColorCode(Color.Gray) },
                [RichTextBoxThemeStyle.TertiaryText] = new RichTextBoxConsoleThemeStyle { Foreground = ColorToHtmlColorCode(Color.DarkGray) },
                [RichTextBoxThemeStyle.Invalid] = new RichTextBoxConsoleThemeStyle { Foreground = ColorToHtmlColorCode(Color.Yellow) },
                [RichTextBoxThemeStyle.Null] = new RichTextBoxConsoleThemeStyle { Foreground = ColorToHtmlColorCode(Color.Blue) },
                [RichTextBoxThemeStyle.Name] = new RichTextBoxConsoleThemeStyle { Foreground = ColorToHtmlColorCode(Color.Gray) },
                [RichTextBoxThemeStyle.String] = new RichTextBoxConsoleThemeStyle { Foreground = ColorToHtmlColorCode(Color.Cyan) },
                [RichTextBoxThemeStyle.Number] = new RichTextBoxConsoleThemeStyle { Foreground = ColorToHtmlColorCode(Color.Magenta) },
                [RichTextBoxThemeStyle.Boolean] = new RichTextBoxConsoleThemeStyle { Foreground = ColorToHtmlColorCode(Color.Blue) },
                [RichTextBoxThemeStyle.Scalar] = new RichTextBoxConsoleThemeStyle { Foreground = ColorToHtmlColorCode(Color.Green) },
                [RichTextBoxThemeStyle.LevelVerbose] = new RichTextBoxConsoleThemeStyle { Foreground = ColorToHtmlColorCode(Color.Gray) },
                [RichTextBoxThemeStyle.LevelDebug] = new RichTextBoxConsoleThemeStyle { Foreground = ColorToHtmlColorCode(Color.Gray) },
                [RichTextBoxThemeStyle.LevelInformation] = new RichTextBoxConsoleThemeStyle { Foreground = ColorToHtmlColorCode(Color.Gray) },
                [RichTextBoxThemeStyle.LevelWarning] = new RichTextBoxConsoleThemeStyle { Foreground = ColorToHtmlColorCode(Color.Yellow) },
                [RichTextBoxThemeStyle.LevelError] = new RichTextBoxConsoleThemeStyle { Foreground = ColorToHtmlColorCode(Color.White), Background = ColorToHtmlColorCode(Color.Red) },
                [RichTextBoxThemeStyle.LevelFatal] = new RichTextBoxConsoleThemeStyle { Foreground = ColorToHtmlColorCode(Color.White), Background = ColorToHtmlColorCode(Color.Red) },
            }
        );
        public static string ColorToHtmlColorCode(Color color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}
