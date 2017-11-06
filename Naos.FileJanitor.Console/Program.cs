// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Hangfire.Console
{
    using System;

    using CLAP;

    using Its.Log.Instrumentation;

    using Naos.FileJanitor.Console;

    /// <summary>
    /// Main entry point of the application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args">Arguments for application.</param>
        /// <returns>Exit code.</returns>
        public static int Main(string[] args)
        {
            try
            {
                WriteAsciiArt(Console.WriteLine);

                /*---------------------------------------------------------------------------*
                 * This is just a pass through to the CLAP implementation of the harness,    *
                 * it will parse the command line arguments and provide multiple entry       *
                 * points as configured.  It is easiest to derive from the abstract class    *
                 * 'CommandLinAbstractionBase' as 'ExampleCommandLineAbstraction' does which *
                 * provides an example of the minimum amount of work to get started.  It is  *
                 * installed as a recipe for easy reference and covers help, errors, etc.    *
                 *---------------------------------------------------------------------------*
                 * For an example of config files you can install the package                *
                 * 'Naos.Recipes.Console.ExampleConfig' which has examples of the directory  *
                 * structure, 'LogProcessorSettings' settings for console and file, as well  *
                 * as an App.Config it not using the environment name as a parameter.        *
                 *---------------------------------------------------------------------------*
                 * Must update the code below to use your custom abstraction class.          *
                 *---------------------------------------------------------------------------*/
                var exitCode = Parser.Run<ConsoleAbstraction>(args);
                return exitCode;
            }
            catch (Exception ex)
            {
                /*---------------------------------------------------------------------------*
                 * This should never be reached but is here as a last ditch effort to ensure *
                 * errors are not lost.                                                      *
                 *---------------------------------------------------------------------------*/
                Console.WriteLine(string.Empty);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(string.Empty);
                Log.Write(ex);

                return 1;
            }
        }

        private static void WriteAsciiArt(Action<string> announcer)
        {
            announcer(@"______________________________________________________________________________");
            announcer(@"|                                                                            |");
            announcer(@"|                                  . ...  .                                  |");
            announcer(@"|                                    OI$7~$7.                                |");
            announcer(@"|                                ..8DND. +NDO$.                              |");
            announcer(@"|                                 DD?     . N8O                              |");
            announcer(@"|                                .DN.       .N8                              |");
            announcer(@"|                                ID+.       .~DD                             |");
            announcer(@"|                                 DO         IDO                             |");
            announcer(@"|                                .N8O       .DD                              |");
            announcer(@"|                                  N8OI. ..8DD                               |");
            announcer(@"|                                   .MD888DNZ  .                             |");
            announcer(@"|                                      88?                                   |");
            announcer(@"|                                    . 88?                                   |");
            announcer(@"|                                     $D87NO8 ..          .OZ,.8.            |");
            announcer(@"|                                    O.88?   =DOO. ..  ..$8D:DD.             |");
            announcer(@"|                                    8788?    .   ~7OMNN8OD8DO .             |");
            announcer(@"|                                  ...NOZ$ .   .  . .OZO? ....               |");
            announcer(@"|                                     .MD+=8OOO  ~O7D, .                     |");
            announcer(@"|                                     .NN? .8OOZ7N .                         |");
            announcer(@"|                                      88?7O?8ND .                           |");
            announcer(@"|                                     .OO7N.7                                |");
            announcer(@"|                                  ,O$888Z                                   |");
            announcer(@"|                          .. . 7O7D .888O,.                                 |");
            announcer(@"|                          . OOZN. . =OD.DZ                                  |");
            announcer(@"|             DDD..     .,OZ8= ..   .88   88                                 |");
            announcer(@"|          ..8888888~ 7OOD,..      . 8$   DZ                                 |");
            announcer(@"|          .=O888888DD8 .           :8    88 .                               |");
            announcer(@"|           OOOOD8888888D           Z8    ?N.                                |");
            announcer(@"|           OOOOOOOD88Z888$..       88     8.                                |");
            announcer(@"|          ...OOOOOOOD88888         88    .8~                                |");
            announcer(@"|              .OOOOOOO8DD,         MN    .8I.                               |");
            announcer(@"|                 OOZOOZZ.         .M$     8Z                                |");
            announcer(@"|                .  OZZZZ:.        .8~     8D.                               |");
            announcer(@"|                   . .ZZZ         =O,    .88                                |");
            announcer(@"|                                  =8D     NOO.                              |");
            announcer(@"|                                   M8.     88..                             |");
            announcer(@"|                                   .       .  .                             |");
            announcer(@"|                                                                            |");
            announcer(@"|----------------------------------------------------------------------------|");
            announcer(@"|                                                                            |");
            announcer(@"| ,------.,--.,--.            ,--.                ,--.  ,--.                 |");
            announcer(@"| |  .---'`--'|  | ,---.      |  | ,--,--.,--,--, `--',-'  '-. ,---. ,--.--. |");
            announcer(@"| |  `--, ,--.|  || .-. :,--. |  |' ,-.  ||      \,--.'-.  .-'| .-. ||  .--' |");
            announcer(@"| |  |`   |  ||  |\   --.|  '-'  /\ '-'  ||  ||  ||  |  |  |  ' '-' '|  |    |");
            announcer(@"| `--'    `--'`--' `----' `-----'  `--`--'`--''--'`--'  `--'   `---' `--'    |");
            announcer(@"|                                                                            |");
            announcer(@"|____________________________________________________________________________|");
            announcer(string.Empty);
        }
    }
}