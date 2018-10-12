// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HandlerFactoryExample.cs" company="Naos">
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

    using Naos.MessageBus.Domain;

    /// <summary>
    /// Factory builder to provide logic to resolve the appropriate <see cref="IHandleMessages" /> for a dispatched <see cref="IMessage" /> implementation.
    /// </summary>
#if NaosMessageBusHangfireConsole
    public static partial class HandlerFactory
#else
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static partial class HandlerFactoryExample
#endif
    {
        /*----------------------------- CHANGE HERE ---------------------------------*
         * Can specify the map directly or instead use the example function below to *
         * discover your handlers in 1 or many assemblies.                           *
         *---------------------------------------------------------------------------*/

        /// <summary>
        /// Map of the message type to the intended handler type.  Must have a parameterless constructor and implement <see cref="IHandleMessages" />,
        /// however deriving from <see cref="MessageHandlerBase{T}" /> is recommended as it's more straightforward and easier to write.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "This is used in real usage, keeping here as a reference example to make it easier to get started.")]
        private static readonly IReadOnlyDictionary<Type, Type> MessageTypeToHandlerTypeMap = HandlerFactory.DiscoverHandlersInAssemblies(new[] { typeof(ExampleMessage).Assembly, typeof(ExampleMessageHandler).Assembly, });
    }
}