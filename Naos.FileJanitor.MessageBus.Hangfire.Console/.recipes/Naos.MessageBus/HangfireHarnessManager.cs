﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HangfireHarnessManager.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in Naos.MessageBus.Hangfire.Bootstrapper source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

#if NaosMessageBusHangfireConsole
namespace Naos.MessageBus.Hangfire.Console
#else
namespace Naos.MessageBus.Hangfire.Bootstrapper
#endif
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using global::Hangfire;
    using global::Hangfire.Logging;
    using global::Hangfire.SqlServer;

    using Its.Log.Instrumentation;

    using Naos.Compression.Domain;
    using Naos.Diagnostics.Domain;
    using Naos.MessageBus.Core;
    using Naos.MessageBus.Domain;
    using Naos.MessageBus.Hangfire.Sender;
    using Naos.MessageBus.Persistence;
    using Naos.Serialization.Factory;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Class to run Hangfire harness.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Hangfire", Justification = "Spelling/name is correct.")]
#if !NaosMessageBusHangfireConsole
    [System.Diagnostics.DebuggerStepThrough]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("Naos.MessageBus.Hangfire.Bootstrapper", "See package version number")]
#endif
    public static class HangfireHarnessManager
    {
        /// <summary>
        /// Launch method that will monitor channels until there is no activity and the <see cref="MessageBusLaunchConfiguration.TimeToLive" /> has expired or indefinitely if not configured.
        /// </summary>
        /// <param name="messageBusConnectionConfiguration">Persistence configuration.</param>
        /// <param name="launchConfig">Launch settings to use.</param>
        /// <param name="handlerFactory">Handler factory to build correct <see cref="IHandleMessages" /> for various types of <see cref="IMessage" />'s.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Hangfire", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Prefer the procedural layout here.")]
        public static void Launch(
            MessageBusConnectionConfiguration messageBusConnectionConfiguration,
            MessageBusLaunchConfiguration launchConfig,
            IHandlerFactory handlerFactory)
        {
            new { messageBusConnectionConfiguration }.Must().NotBeNull().OrThrowFirstFailure();
            new { launchConfig }.Must().NotBeNull().OrThrowFirstFailure();
            new { handlerFactory }.Must().NotBeNull().OrThrowFirstFailure();

            if (launchConfig.ChannelsToMonitor.Any(_ => _.GetType() != typeof(SimpleChannel)))
            {
                throw new NotSupportedException(Invariant($"Only {nameof(SimpleChannel)}'s are supported as the implementation of {nameof(IChannel)} for {nameof(launchConfig.ChannelsToMonitor)}."));
            }

            var assembliesToRecord = new[] { typeof(HangfireHarnessManager).Assembly }.ToList();
            if (handlerFactory is ReflectionHandlerFactory reflectionHandlerFactory)
            {
                assembliesToRecord.AddRange(reflectionHandlerFactory.FilePathToAssemblyMap.Values);
            }

            var assemblyDetails = assembliesToRecord.Select(SafeFetchAssemblyDetails).ToList();
            var machineDetails = MachineDetails.Create();
            var harnessStaticDetails = new HarnessStaticDetails
                                           {
                                               MachineDetails = machineDetails,
                                               ExecutingUser = Environment.UserDomainName + "\\" + Environment.UserName,
                                               Assemblies = assemblyDetails,
                                           };

            var serializerFactory = SerializerFactory.Instance;
            var compressorFactory = CompressorFactory.Instance;

            var logProvider = new ItsLogPassThroughProvider();
            LogProvider.SetCurrentLogProvider(logProvider);

            var activeMessageTracker = new InMemoryActiveMessageTracker();

            var envelopeMachine = new EnvelopeMachine(
                PostOffice.MessageSerializationDescription,
                serializerFactory,
                compressorFactory,
                launchConfig.TypeMatchStrategyForMessageResolution);

            var courier = new HangfireCourier(messageBusConnectionConfiguration.CourierPersistenceConnectionConfiguration, envelopeMachine);
            var parcelTrackingSystem = new ParcelTrackingSystem(
                courier,
                envelopeMachine,
                messageBusConnectionConfiguration.EventPersistenceConnectionConfiguration,
                messageBusConnectionConfiguration.ReadModelPersistenceConnectionConfiguration);

            var postOffice = new PostOffice(parcelTrackingSystem, HangfireCourier.DefaultChannelRouter, envelopeMachine);

            HandlerToolshed.InitializePostOffice(() => postOffice);
            HandlerToolshed.InitializeParcelTracking(() => parcelTrackingSystem);
            HandlerToolshed.InitializeSerializerFactory(() => serializerFactory);
            HandlerToolshed.InitializeCompressorFactory(() => compressorFactory);

            var shareManager = new ShareManager(
                serializerFactory,
                compressorFactory,
                launchConfig.TypeMatchStrategyForMatchingSharingInterfaces);

            var handlerSharedStateMap = new ConcurrentDictionary<Type, object>();

            var dispatcher = new MessageDispatcher(
                handlerFactory,
                handlerSharedStateMap,
                launchConfig.ChannelsToMonitor,
                harnessStaticDetails,
                parcelTrackingSystem,
                activeMessageTracker,
                postOffice,
                envelopeMachine,
                shareManager);

            // configure hangfire to use the DispatcherFactory for getting IDispatchMessages calls
            GlobalConfiguration.Configuration.UseActivator(new DispatcherFactoryJobActivator(dispatcher));
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = launchConfig.MessageDeliveryRetryCount });

            var executorOptions = new BackgroundJobServerOptions
                                      {
                                          Queues = launchConfig.ChannelsToMonitor.OfType<SimpleChannel>().Select(_ => _.Name).ToArray(),
                                          SchedulePollingInterval = launchConfig.PollingInterval,
                                          WorkerCount = launchConfig.ConcurrentWorkerCount,
                                      };

            GlobalConfiguration.Configuration.UseSqlServerStorage(
                messageBusConnectionConfiguration.CourierPersistenceConnectionConfiguration.ToSqlServerConnectionString(),
                new SqlServerStorageOptions());

            var launchConfigTimeToLive = launchConfig.TimeToLive;
            if (launchConfigTimeToLive == default(TimeSpan))
            {
                launchConfigTimeToLive = TimeSpan.MaxValue;
            }

            var timeout = DateTime.UtcNow.Add(launchConfigTimeToLive);

            // ReSharper disable once UnusedVariable - good reminder that the server object comes back and that's what is disposed in the end...
            using (var server = new BackgroundJobServer(executorOptions))
            {
                Console.WriteLine("Hangfire Server started. Will terminate when there are no active jobs after: " + timeout);
                Log.Write(() => new { LogMessage = "Hangfire Server launched. Will terminate when there are no active jobs after: " + timeout });

                // once the timeout has been achieved with no active jobs the process will exit (this assumes that a scheduled task will restart the process)
                //    the main impetus for this was the fact that Hangfire won't reconnect correctly so we must periodically initiate an entire reconnect.
                while (activeMessageTracker.ActiveMessagesCount != 0 || (DateTime.UtcNow < timeout))
                {
                    Thread.Sleep(launchConfig.PollingInterval);
                }

                Log.Write(() => new { ex = "Hangfire Server terminating. There are no active jobs and current time if beyond the timeout: " + timeout });
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching and swallowing on purpose.")]
        private static AssemblyDetails SafeFetchAssemblyDetails(Assembly assembly)
        {
            new { assembly }.Must().NotBeNull().OrThrowFirstFailure();

            // get a default
            var ret = new AssemblyDetails { FilePath = assembly.Location };

            try
            {
                ret = AssemblyDetails.CreateFromAssembly(assembly);
            }
            catch (Exception)
            {
                /* no-op - swallow this because we will just get what we get... */
            }

            return ret;
        }
    }
}