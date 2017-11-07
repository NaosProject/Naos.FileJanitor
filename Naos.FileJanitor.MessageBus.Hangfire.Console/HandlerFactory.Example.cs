// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HandlerFactory.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in Naos.MessageBus.Hangfire.Bootstrapper source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

#if NaosMessageBusHangfireConsole
namespace Naos.MessageBus.Hangfire.Console
#else
namespace Naos.FileJanitor.MessageBus.Hangfire.Console
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Its.Log.Instrumentation;

    using Naos.MessageBus.Domain;

    using static System.FormattableString;

    /// <summary>
    /// Factory builder to provide logic to resolve the appropriate <see cref="IHandleMessages" /> for a dispatched <see cref="IMessage" /> implementation.
    /// </summary>
    public static partial class HandlerFactory
    {
        /*----------------------------- CHANGE HERE ---------------------------------*
         * Can specify the map directly or instead use the example function below to *
         * discover your handlers in 1 or many assemblies.                           *
         *---------------------------------------------------------------------------*/

        /// <summary>
        /// Map of the message type to the intended handler type.  Must have a parameterless constructor and implement <see cref="IHandleMessages" />,
        /// however deriving from <see cref="MessageHandlerBase{T}" /> is recommended as it's more straightforward and easier to write.
        /// </summary>
        private static readonly IReadOnlyDictionary<Type, Type> MessageTypeToHandlerTypeMap = DiscoverHandlersInAssemblies(new[] { typeof(ExampleMessage).Assembly });
    }

    /// <summary>
    /// Example of an <see cref="IMessage" />.
    /// </summary>
    public class ExampleMessage : IMessage
    {
        /// <inheritdoc cref="IMessage" />
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets an example of a group of work to process.
        /// </summary>
        public string GroupToProcess { get; set; }
    }

    /// <summary>
    /// Handler for <see cref="ExampleMessage" />.
    /// </summary>
    public class ExampleMessageHandler : MessageHandlerBase<ExampleMessage>
    {
        /// <inheritdoc cref="MessageHandlerBase{T}" />
        public override async Task HandleAsync(ExampleMessage message)
        {
            await Task.Run(() => { });

            Log.Write(() => Invariant($"Finished processing group: {message.GroupToProcess}"));
        }
    }
}