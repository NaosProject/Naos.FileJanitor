// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineAbstractionBase.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in Naos.Recipes.Console.Bootstrapper source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Console
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using CLAP;

    using Its.Configuration;
    using Its.Log.Instrumentation;

    using Naos.Diagnostics.Domain;
    using Naos.Logging.Domain;
    using Naos.Recipes.Configuration.Setup;

    using OBeautifulCode.Collection.Recipes;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Abstraction for use with <see cref="CLAP" /> to provide basic command line interaction.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "Cannot be static for command line contract.")]
    public abstract class CommandLineAbstractionBase
    {
        /// <summary>
        /// Entry point to simulate a failure.
        /// </summary>
        /// <param name="debug">Optional indication to launch the debugger from inside the application (default is false).</param>
        /// <param name="message">Message to use when creating a SimulatedFailureException.</param>
        /// <param name="environment">Optional value to use when setting the Its.Configuration precedence to use specific settings.</param>
        [Verb(Aliases = "", IsDefault = false, Description = "Throws an exception with provided message to simulate an error and confirm correct setup;\r\n            example usage: [Harness].exe fail /message='My Message.'\r\n                           [Harness].exe fail /message='My Message.' /debug=true\r\n                           [Harness].exe fail /message='My Message.' /environment=ExampleDevelopment\r\n                           [Harness].exe fail /message='My Message.' /environment=ExampleDevelopment /debug=true\r\n")]
        public static void Fail(
            [Aliases("")] [Description("Launches the debugger.")] [DefaultValue(false)] bool debug,
            [Aliases("")] [Required] [Description("Message to use when creating a SimulatedFailureException.")] string message,
            [Aliases("")] [Description("Sets the Its.Configuration precedence to use specific settings.")] [DefaultValue(null)] string environment)
        {
            /*---------------------------------------------------------------------------*
             * Any method should run this logic to debug, setup config & logging, etc.   *
             *---------------------------------------------------------------------------*/
            CommonSetup(debug, environment);

            /*---------------------------------------------------------------------------*
             * This is not necessary but often very useful to print out the arguments.   *
             *---------------------------------------------------------------------------*/
            PrintArguments(new { message, environment });

            /*---------------------------------------------------------------------------*
             * Throw an exception after all logging is setup which will excercise the    *
             * top level error handling and can ensure correct setup.  Uses the type     *
             * SimulatedFailureException from Naos.Diagnostics.Domain so they can easily *
             * be filtered out if needed to avoid panic or triggering alarms.            *
             *---------------------------------------------------------------------------*/
            throw new SimulatedFailureException(message);
        }

        /// <summary>
        /// Entry point to log a message and exit gracefully.
        /// </summary>
        /// <param name="debug">Optional indication to launch the debugger from inside the application (default is false).</param>
        /// <param name="message">Message to log.</param>
        /// <param name="environment">Optional value to use when setting the Its.Configuration precedence to use specific settings.</param>
        [Verb(Aliases = "", IsDefault = false, Description = "Logs the provided message to confirm correct setup;\r\n            example usage: [Harness].exe pass /message='My Message.'\r\n                           [Harness].exe pass /message='My Message.' /debug=true\r\n                           [Harness].exe pass /message='My Message.' /environment=ExampleDevelopment\r\n                           [Harness].exe pass /message='My Message.' /environment=ExampleDevelopment /debug=true\r\n")]
        public static void Pass(
            [Aliases("")] [Description("Launches the debugger.")] [DefaultValue(false)] bool debug,
            [Aliases("")] [Required] [Description("Message to log.")] string message,
            [Aliases("")] [Description("Sets the Its.Configuration precedence to use specific settings.")] [DefaultValue(null)] string environment)
        {
            /*---------------------------------------------------------------------------*
             * Any method should run this logic to debug, setup config & logging, etc.   *
             *---------------------------------------------------------------------------*/
            CommonSetup(debug, environment);

            /*---------------------------------------------------------------------------*
             * This is not necessary but often very useful to print out the arguments.   *
             *---------------------------------------------------------------------------*/
            PrintArguments(new { message, environment });

            /*---------------------------------------------------------------------------*
             * Write message after all logging is setup which will confirm setup.        *
             *---------------------------------------------------------------------------*/
            Log.Write(() => message);
        }

        /// <summary>
        /// Error method to call from CLAP; a 1 will be returned as the exit code if this is entered since an exception was thrown.
        /// </summary>
        /// <param name="context">Context provided with details.</param>
        [Error]
#pragma warning disable CS3001 // Argument type is not CLS-compliant - needed for CLAP
        public static void Error(ExceptionContext context)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            new { context }.Must().NotBeNull().OrThrowFirstFailure();

            // change color to red
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            // parser exception or
            if (context.Exception is CommandLineParserException)
            {
                Console.WriteLine("Failure parsing command line arguments.  Run the exe with the 'help' command for usage.");
                Console.WriteLine("   " + context.Exception.Message);
            }
            else
            {
                Console.WriteLine("Failure during execution.");
                Console.WriteLine("   " + context.Exception.Message);
                Console.WriteLine(string.Empty);
                Console.WriteLine("   " + context.Exception);
                Log.Write(context.Exception);
            }

            // restore color
            Console.WriteLine();
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Help method to call from CLAP.
        /// </summary>
        /// <param name="helpText">Generated help text to display.</param>
        [Empty]
        [Help(Aliases = "h,?,-h,-help")]
        [Verb(IsDefault = true)]
        public static void Help(string helpText)
        {
            new { helpText }.Must().NotBeNull().OrThrowFirstFailure();

            Console.WriteLine("   Usage");
            Console.Write("   -----");

            // strip out the usage info about help, it's confusing
            helpText = helpText.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Skip(3).ToNewLineDelimited();
            Console.WriteLine(helpText);
            Console.WriteLine();
        }

        /// <summary>
        /// Runs common setup logic to prepare for <see cref="Its.Log" /> and <see cref="Its.Configuration" />, also will launch the debugger if the debug flag is provided.
        /// </summary>
        /// <param name="debug">A value indicating whether or not to launch the debugger.</param>
        /// <param name="environment">Optional environment name that will set the <see cref="Its.Configuration" /> precedence instead of the default which is reading the App.Config value.</param>
        /// <param name="logProcessorSettings">Optional <see cref="LogProcessorSettings" /> to use instead of the default found in <see cref="Its.Configuration" />.</param>
        /// <param name="configuredAndManagedLogProcessors">Optional set of pre-configured and externally managed <see cref="LogProcessorBase" /> to use.</param>
        /// <param name="announcer">Optional announcer; DEFAULT is null which will go to <see cref="Console.WriteLine(string)" />.<see cref="Console.WriteLine(string)" />.</param>
        protected static void CommonSetup(
            bool debug,
            string environment = null,
            LogProcessorSettings logProcessorSettings = null,
            IReadOnlyCollection<LogProcessorBase> configuredAndManagedLogProcessors = null,
            Action<string> announcer = null)
        {
            var localAnnouncer = announcer ?? Console.WriteLine;

            /*---------------------------------------------------------------------------*
             * Useful for launching the debugger from the command line and making sure   *
             * it is connected to the instance of the IDE you want to use.               *
             *---------------------------------------------------------------------------*/
            if (debug)
            {
                Debugger.Launch();
            }

            /*---------------------------------------------------------------------------*
             * Setup deserialization logic for any use of Its.Configuration reading      *
             * config files from the '.config' directory with 'environment' sub folders  *
             * the chain of responsibility is set in the App.config file using the       *
             * 'Its.Configuration.Settings.Precedence' setting.  You can override the    *
             * way this is used by specifying a different diretory for the config or     *
             * providing additonal precedence values using                               *
             * ResetConfigureSerializationAndSetValues.                                  *
             *---------------------------------------------------------------------------*/
            if (!string.IsNullOrWhiteSpace(environment))
            {
                Config.ResetConfigureSerializationAndSetValues(environment, announcer: localAnnouncer);
            }
            else
            {
                Config.ConfigureSerialization(localAnnouncer);
            }

            /*---------------------------------------------------------------------------*
             * Initialize logging; this sets up Its.Log which is what gets used through- *
             * out the code.  All logging will also get sent through it.  This  can be   *
             * swapped out to send all Its.Log messages to another logging framework if  *
             * there is already one in place.                                            *
             *---------------------------------------------------------------------------*/
            var localLogProcessorSettings = logProcessorSettings ?? Settings.Get<LogProcessorSettings>() ?? new LogProcessorSettings();
            if (localLogProcessorSettings == null)
            {
                localAnnouncer("No LogProcessorSettings provided or found in config; using Null Object susbstitue.");
                localLogProcessorSettings = new LogProcessorSettings();
            }

            LogProcessing.Instance.Setup(localLogProcessorSettings, localAnnouncer, configuredAndManagedLogProcessors);
        }

        /// <summary>
        /// Masks a string to be printed, very useful for things like passwords or certificate keys.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="percentageOfStringToMask">Optional percentage of string to leave; e.g. 20, 50, etc. (must be between 1-99); default is 70.</param>
        /// <returns>Masked string.</returns>
        protected static string MaskString(string input, int percentageOfStringToMask = 70)
        {
            new { percentageOfStringToMask }.Must().BeInRange(1, 99).OrThrowFirstFailure();

            var localInput = input ?? string.Empty;
            int indexToMaskUntil = (int)(localInput.Length * (percentageOfStringToMask / 100d));
            var maskedString = new string('*', indexToMaskUntil) + string.Join(string.Empty, localInput.Skip(indexToMaskUntil));
            return maskedString;
        }

        /// <summary>
        /// Prints the arguments as provided via an anonymous object. e.g. PrintArguments(new { argumentOne, argumentTwo, maskedArgumentThree = MaskString(argumentThree), totalDays = argumentFour.TotalDays }, nameof(MyMethod));
        /// </summary>
        /// <example>PrintArguments(new { argumentOne, argumentTwo, maskedArgumentThree = MaskString(argumentThree), totalDays = argumentFour.TotalDays }, nameof(MyMethod));</example>
        /// <param name="anonymousObjectWithArguments">Object to reflect over properties of.</param>
        /// <param name="method">Optional name of method being called, if null then it will construct a <see cref="StackTrace" /> to retrieve it; this takes about 10-15 milliseconds so it's not free but is relatively cheap to have a smaller mouth to feed.</param>
        /// <param name="announcer">Optional announcer; DEFAULT is null which will go to <see cref="Console.WriteLine(string)" />.<see cref="Console.WriteLine(string)" />.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1720:IdentifiersShouldNotContainTypeNames",
            MessageId = "object",
            Justification = "Prefer this name for clarity.")]
        protected static void PrintArguments(object anonymousObjectWithArguments = null, string method = null, Action<string> announcer = null)
        {
            var localAnnouncer = announcer ?? Console.WriteLine;
            var localMethod = method ?? new StackTrace().GetFrame(1).GetMethod().Name;
            var lines = new List<string>();

            var propertyInfos = anonymousObjectWithArguments?.GetType()
                .GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).ToList();
            var firstLine = Invariant($"Called '{localMethod}' with no provided arguments");

            if (propertyInfos != null && propertyInfos.Any())
            {
                firstLine = Invariant($"Called '{localMethod}' with the following arguments:");

                int longestLeftOffset = 3 + propertyInfos.Max(_ => _.Name.Length);

                foreach (var propertyInfo in propertyInfos)
                {
                    var value = propertyInfo.GetValue(anonymousObjectWithArguments) ?? "<null>";
                    var propertyName = propertyInfo.Name;
                    var paddingSize = longestLeftOffset - propertyName.Length;
                    var padding = new string(' ', paddingSize);
                    var line = Invariant($"{padding}{propertyName}: {value}");
                    lines.Add(line);
                }
            }

            lines.Add(string.Empty);

            localAnnouncer(firstLine);
            lines.ForEach(_ => localAnnouncer(_));
        }
    }

    /// <summary>
    /// Example of how to extend the base class to add your custom functionality.  It's recommeneded that each method take
    /// optional environment name AND debug boolean paramters and then call the <see cref="CommandLineAbstractionBase.CommonSetup" /> but not necessary.
    /// The common setup also allows for provided the <see cref="LogProcessorSettings" /> directly instead of the default
    /// loading from <see cref="Its.Configuration" />.
    /// </summary>
    public class ExampleCommandLineAbstraction : CommandLineAbstractionBase
    {
        /// <summary>
        /// Example of a custom data processing job that might need to be run as a cron job.
        /// </summary>
        /// <param name="debug">A value indicating whether or not to launch the debugger.</param>
        /// <param name="environment">Optional environment name that will set the <see cref="Its.Configuration" /> precedence instead of the default which is reading the App.Config value.</param>
        /// <param name="filePathToProcess">Example of a directory that needs to be checked for files to process.</param>
        [Verb(Aliases = "Example", Description = "Example of a custom data processing job that might need to be run as a cron job.")]
        public static void Example(
            [Aliases("")] [Description("Launches the debugger.")] [DefaultValue(false)] bool debug,
            [Aliases("")] [Description("Sets the Its.Configuration precedence to use specific settings.")] [DefaultValue(null)] string environment,
            [Required] [Aliases("")] [Description("Example of a directory that needs to be checked for files to process.")] string filePathToProcess)
        {
            /*---------------------------------------------------------------------------*
             * Normally this would just be done from the Its.Configuration file but the  *
             * we're overriding to only use the Console for demonstration purposes.      *
             *---------------------------------------------------------------------------*/
            var logProcessorSettingsOverride = new LogProcessorSettings(new[] { new LogConfigurationConsole(LogContexts.All, LogContexts.AllErrors) });

            /*---------------------------------------------------------------------------*
             * Any method should run this logic to debug, setup config & logging, etc.   *
             *---------------------------------------------------------------------------*/
            CommonSetup(debug, environment, logProcessorSettingsOverride);

            /*---------------------------------------------------------------------------*
             * This is not necessary but often very useful to print out the arguments.   *
             *---------------------------------------------------------------------------*/
            PrintArguments(new { filePathToProcess, environment });

            Log.Write(() => Invariant($"Processed files at: {filePathToProcess}"));
        }
    }
}