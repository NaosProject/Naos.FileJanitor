// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineAbstraction.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Console
{
    using System;

    using CLAP;

    using Naos.FileJanitor.Core;
    using Naos.FileJanitor.Domain;

    /// <summary>
    /// Abstraction for use with <see cref="CLAP" /> to provide basic command line interaction.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "Cannot be static for command line contract.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Hangfire", Justification = "Spelling/name is correct.")]
    public class CommandLineAbstraction : CommandLineAbstractionBase
    {
        /// <summary>
        /// Removes old files.
        /// </summary>
        /// <param name="debug">Launches the debugger.</param>
        /// <param name="rootPath">The root path to evaluate (must be a directory).</param>
        /// <param name="retentionWindow">The time to retain files (in format dd:hh:mm).</param>
        /// <param name="recursive">Whether or not to evaluate files recursively on the path.</param>
        /// <param name="deleteEmptyDirectories">Whether or not to delete directories that are or become empty during cleanup.</param>
        /// <param name="dateRetrievalStrategy">The date retrieval strategy to use on files.</param>
        /// <param name="environment">Sets the Its.Configuration precedence to use specific settings.</param>
        [Verb(Aliases = "Cleanup", Description = "Removes old files.")]
        public static void Cleanup(
            [Aliases("")] [Description("Launches the debugger.")] [DefaultValue(false)] bool debug,
            [Required] [Aliases("")] [Description("The root path to evaluate (must be a directory).")] string rootPath,
            [Required] [Aliases("")] [Description("The time to retain files (in format dd:hh:mm).")] string retentionWindow,
            [DefaultValue(true)] [Aliases("")] [Description("Whether or not to evaluate files recursively on the path.")] bool recursive,
            [DefaultValue(false)] [Aliases("")] [Description("Whether or not to delete directories that are or become empty during cleanup.")] bool deleteEmptyDirectories,
            [DefaultValue(DateRetrievalStrategy.LastUpdateDate)] [Aliases("")] [Description("The date retrieval strategy to use on files.")] DateRetrievalStrategy dateRetrievalStrategy,
            [Aliases("")] [Description("Sets the Its.Configuration precedence to use specific settings.")] [DefaultValue(null)] string environment)
        {
            CommonSetup(debug, environment);

            var retentionWindowTimeSpan = GetTimeSpanFromDayHourMinuteColonDelimited(retentionWindow);

            PrintArguments(
                new
                    {
                        rootPath,
                        retentionWindowAsDayHourMinute = retentionWindow,
                        retentionWindowInDays = retentionWindowTimeSpan.TotalDays,
                        deleteEmptyDirectories,
                        recursive,
                        dateRetrievalStrategy,
                    });

            FilePathJanitor.Cleanup(
                rootPath,
                retentionWindowTimeSpan,
                recursive,
                deleteEmptyDirectories,
                dateRetrievalStrategy);
        }

        /// <summary>
        /// Parses a colon delimited day hour minute string into a valid <see cref="TimeSpan" />.
        /// </summary>
        /// <param name="textToParse">Text to parse.</param>
        /// <returns>Parsed <see cref="TimeSpan" />.</returns>
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
            if (!int.TryParse(daysRaw, out int days))
            {
                throw argException;
            }

            var hoursRaw = split[1];
            if (!int.TryParse(hoursRaw, out int hours))
            {
                throw argException;
            }

            var minutesRaw = split[2];
            if (!int.TryParse(minutesRaw, out int minutes))
            {
                throw argException;
            }

            return new TimeSpan(days, hours, minutes, 0);
        }
    }
}