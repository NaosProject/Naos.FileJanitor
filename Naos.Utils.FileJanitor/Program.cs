// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Naos LLC">
//   Copyright 2014 Naos LLC
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Utils.FileJanitor
{
    using System;

    using CLAP;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The program entry point.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static void Main(string[] args)
        {
            WriteAsciiArt();
            Parser.Run<FileJanitorConsoleHarness>(args);
        }

        /// <summary>
        /// Write ASCII art.
        /// </summary>
        private static void WriteAsciiArt()
        {
            Console.WriteLine(@"______________________________________________________________________________");
            Console.WriteLine(@"|                                                                            |");
            Console.WriteLine(@"|                                  . ...  .                                  |");
            Console.WriteLine(@"|                                    OI$7~$7.                                |");
            Console.WriteLine(@"|                                ..8DND. +NDO$.                              |");
            Console.WriteLine(@"|                                 DD?     . N8O                              |");
            Console.WriteLine(@"|                                .DN.       .N8                              |");
            Console.WriteLine(@"|                                ID+.       .~DD                             |");
            Console.WriteLine(@"|                                 DO         IDO                             |");
            Console.WriteLine(@"|                                .N8O       .DD                              |");
            Console.WriteLine(@"|                                  N8OI. ..8DD                               |");
            Console.WriteLine(@"|                                   .MD888DNZ  .                             |");
            Console.WriteLine(@"|                                      88?                                   |");
            Console.WriteLine(@"|                                    . 88?                                   |");
            Console.WriteLine(@"|                                     $D87NO8 ..          .OZ,.8.            |");
            Console.WriteLine(@"|                                    O.88?   =DOO. ..  ..$8D:DD.             |");
            Console.WriteLine(@"|                                    8788?    .   ~7OMNN8OD8DO .             |");
            Console.WriteLine(@"|                                  ...NOZ$ .   .  . .OZO? ....               |");
            Console.WriteLine(@"|                                     .MD+=8OOO  ~O7D, .                     |");
            Console.WriteLine(@"|                                     .NN? .8OOZ7N .                         |");
            Console.WriteLine(@"|                                      88?7O?8ND .                           |");
            Console.WriteLine(@"|                                     .OO7N.7                                |");
            Console.WriteLine(@"|                                  ,O$888Z                                   |");
            Console.WriteLine(@"|                          .. . 7O7D .888O,.                                 |");
            Console.WriteLine(@"|                          . OOZN. . =OD.DZ                                  |");
            Console.WriteLine(@"|             DDD..     .,OZ8= ..   .88   88                                 |");
            Console.WriteLine(@"|          ..8888888~ 7OOD,..      . 8$   DZ                                 |");
            Console.WriteLine(@"|          .=O888888DD8 .           :8    88 .                               |");
            Console.WriteLine(@"|           OOOOD8888888D           Z8    ?N.                                |");
            Console.WriteLine(@"|           OOOOOOOD88Z888$..       88     8.                                |");
            Console.WriteLine(@"|          ...OOOOOOOD88888         88    .8~                                |");
            Console.WriteLine(@"|              .OOOOOOO8DD,         MN    .8I.                               |");
            Console.WriteLine(@"|                 OOZOOZZ.         .M$     8Z                                |");
            Console.WriteLine(@"|                .  OZZZZ:.        .8~     8D.                               |");
            Console.WriteLine(@"|                   . .ZZZ         =O,    .88                                |");
            Console.WriteLine(@"|                                  =8D     NOO.                              |");
            Console.WriteLine(@"|                                   M8.     88..                             |");
            Console.WriteLine(@"|                                   .       .  .                             |");
            Console.WriteLine(@"|                                                                            |");
            Console.WriteLine(@"|----------------------------------------------------------------------------|");
            Console.WriteLine(@"|                                                                            |");
            Console.WriteLine(@"| ,------.,--.,--.            ,--.                ,--.  ,--.                 |");
            Console.WriteLine(@"| |  .---'`--'|  | ,---.      |  | ,--,--.,--,--, `--',-'  '-. ,---. ,--.--. |");
            Console.WriteLine(@"| |  `--, ,--.|  || .-. :,--. |  |' ,-.  ||      \,--.'-.  .-'| .-. ||  .--' |");
            Console.WriteLine(@"| |  |`   |  ||  |\   --.|  '-'  /\ '-'  ||  ||  ||  |  |  |  ' '-' '|  |    |");
            Console.WriteLine(@"| `--'    `--'`--' `----' `-----'  `--`--'`--''--'`--'  `--'   `---' `--'    |");
            Console.WriteLine(@"|                                                                            |");
            Console.WriteLine(@"|____________________________________________________________________________|");
            Console.WriteLine();
        }
    }
}
