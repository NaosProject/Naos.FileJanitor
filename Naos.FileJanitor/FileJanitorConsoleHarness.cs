// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileJanitorConsoleHarness.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor
{
    using System;
    using System.Linq;

    using CLAP;

    using OBeautifulCode.Collection;

    public class FileJanitorConsoleHarness
    {
        [Verb(Aliases = "Cleanup", Description = "Removes old files")]
        public static void Cleanup(
            [Required] [Aliases("")] [Description("The root path to evaluate (must be a directory).")] string rootPath,
            [Required] [Aliases("")] [Description("The time to retain files (in format dd:hh:mm).")] string retentionWindow,
            [DefaultValue(true)] [Aliases("")] [Description("Whether or not to evaluate files recursively on the path.")] bool recursive,
            [DefaultValue(false)] [Aliases("")] [Description("Whether or not to delete directories that are or become empty during cleanup.")] bool deleteEmptyDirectories,
            [DefaultValue(FileJanitor.DateRetrievalStrategy.LastUpdateDate)] [Aliases("")] [Description("The date retrieval strategy to use.")] FileJanitor.DateRetrievalStrategy dateRetrievalStrategy)
        {
            var retentionWindowTimeSpan = FileJanitorConsoleHarness.GetTimeSpanFromDayHourMinuteColonDelimited(retentionWindow);

            Console.WriteLine("PARAMETERS:");
            Console.WriteLine("                  rootPath: " + rootPath);
            Console.WriteLine("           retentionWindow: " + retentionWindow);
            Console.WriteLine(" retentionWindow (IN DAYS): " + retentionWindowTimeSpan.TotalDays);
            Console.WriteLine("    deleteEmptyDirectories: " + deleteEmptyDirectories);
            Console.WriteLine("                 recursive: " + recursive);
            Console.WriteLine("     dateRetrievalStrategy: " + dateRetrievalStrategy);
            Console.WriteLine(string.Empty);

            FileJanitor.Cleanup(
                rootPath,
                retentionWindowTimeSpan,
                recursive,
                deleteEmptyDirectories,
                dateRetrievalStrategy);
        }

        public static TimeSpan GetTimeSpanFromDayHourMinuteColonDelimited(string textToParse)
        {
            var argException = new ArgumentException("Value: " + (textToParse ?? string.Empty) + " isn't a valid time, please use format dd:hh:mm.", textToParse);
            if (string.IsNullOrEmpty(textToParse))
            {
                throw argException;
            }

            var split = textToParse.Split(':');
            if (split.Length != 3)
            {
                throw argException;
            }

            var daysRaw = split[0];
            int days;
            if (!int.TryParse(daysRaw, out days))
            {
                throw argException;
            }

            var hoursRaw = split[1];
            int hours;
            if (!int.TryParse(hoursRaw, out hours))
            {
                throw argException;
            }

            var minutesRaw = split[2];
            int minutes;
            if (!int.TryParse(minutesRaw, out minutes))
            {
                throw argException;
            }

            return new TimeSpan(days, hours, minutes, 0);
        }

        [Empty]
        [Help(Aliases = "h,?,-h,-help")]
        [Verb(IsDefault = true)]
        public static void Help(string help)
        {
            Console.WriteLine("   Usage");
            Console.Write("   -----");

            // strip out the usage info about help, it's confusing
            help = help.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Skip(3).ToNewLineDelimited();
            Console.WriteLine(help);
            Console.WriteLine();
        }

        [Error]
        public static void Error(ExceptionContext context)
        {
            // change color to red
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            // parser exception or 
            if (context.Exception is CommandLineParserException)
            {
                Console.WriteLine("I don't understand.  Run the exe with the 'help' command for usage.");
                Console.WriteLine("   " + context.Exception.Message);
            }
            else
            {
                Console.WriteLine("Something broke while performing janitorial duties.");
                Console.WriteLine("   " + context.Exception.Message);
                Console.WriteLine(string.Empty);
                Console.WriteLine("   " + context.Exception);
            }

            // restore color
            Console.WriteLine();
            Console.ForegroundColor = originalColor;
        }
    }
}