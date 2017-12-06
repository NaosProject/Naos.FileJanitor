// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbortIfNoNewFileLocationForTopicMessageHandlerTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Test
{
    using System;
    using System.Linq;
    using System.Threading;

    using Naos.Compression.Domain;
    using Naos.FileJanitor.Domain;
    using Naos.FileJanitor.MessageBus.Handler;
    using Naos.FileJanitor.MessageBus.Scheduler;
    using Naos.Logging.Domain;
    using Naos.MessageBus.Domain;
    using Naos.MessageBus.Domain.Exceptions;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Factory;

    using Xunit;

    public class AbortIfNoNewFileLocationForTopicMessageHandlerTest
    {
        private static readonly ISerializeAndDeserialize Serializer = SerializerFactory.Instance.BuildSerializer(FileLocationAffectedItem.ItemSerializationDescription);

        private static readonly InMemoryLogProcessor Logger = new InMemoryLogProcessor(new InMemoryLogConfiguration(LogContexts.All));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Best in static constructor.")]
        static AbortIfNoNewFileLocationForTopicMessageHandlerTest()
        {
            LogProcessing.Instance.Setup(new LogProcessorSettings(), null, new[] { Logger });
            HandlerToolshed.InitializeSerializerFactory(() => SerializerFactory.Instance);
            HandlerToolshed.InitializeCompressorFactory(() => CompressorFactory.Instance);
        }

        [Fact]
        public void Handle_MatchingStatusReportsWithAffectedItemsEqualFileLocation_DoesAbort()
        {
            // arrange
            Logger.PurgeAllLoggedItems();

            var handler = new AbortIfNoNewFileLocationForTopicMessageHandler();
            var topic = new NamedTopic("topic");
            var affectedItem = new FileLocationAffectedItem
                                           {
                                               FileLocationAffectedItemMessage = "did stuff",
                                               FileLocation = new FileLocation { ContainerLocation = "containerLocation", Container = "container", Key = "key" },
                                               FilePath = "path",
                                           };
            var affectedItemId = Serializer.SerializeToString(affectedItem);
            var affectsCompletedDateTimeUtc = DateTime.UtcNow;

            var message = new AbortIfNoNewFileLocationForTopicMessage
                              {
                                  FileLocation = new FileLocation { ContainerLocation = "containerLocation", Container = "container", Key = "key" },
                                  TopicToCheckAffectedItemsFor = topic,
                                  TopicStatusReports =
                                      new[]
                                          {
                                              new TopicStatusReport
                                                  {
                                                      Topic = new AffectedTopic(topic.Name),
                                                      AffectedItems = new[] { new AffectedItem { Id = affectedItemId } },
                                                      AffectsCompletedDateTimeUtc = affectsCompletedDateTimeUtc,
                                                      Status = TopicStatus.WasAffected,
                                                  },
                                          },
                              };

            // act
            Action action = () => handler.HandleAsync(message).Wait();
            var ex = Record.Exception(action);
            Thread.Sleep(50); // make sure all log messages get flushed

            // assert
            Assert.NotNull(Logger.LoggedItems.SingleOrDefault(_ => _.Message.Contains($"Found matching reports for topic: {message.TopicToCheckAffectedItemsFor.Name} with affects completed on: ")));
            Assert.NotNull(Logger.LoggedItems.SingleOrDefault(_ => _.Message.Contains("Found affected item: ")));
            Assert.Equal(affectedItem.FileLocation, message.FileLocation);

            Assert.NotNull(ex);
            Assert.NotNull(ex.InnerException);
            Assert.IsType<AbortParcelDeliveryException>(ex.InnerException);
            Assert.Equal($"Found that the affected items for affects complete {affectsCompletedDateTimeUtc} matched specified file location.", ex.InnerException.Message);
        }

        [Fact]
        public void Handle_MatchingStatusReportsWithAffectedItemsDoesNotEqualKey_DoesNotAbort()
        {
            // arrange
            Logger.PurgeAllLoggedItems();

            var handler = new AbortIfNoNewFileLocationForTopicMessageHandler();
            var topic = new NamedTopic("topic");
            var affectedItem =
                new FileLocationAffectedItem
                    {
                        FileLocationAffectedItemMessage = "did stuff",
                        FileLocation = new FileLocation { ContainerLocation = "containerLocation", Container = "container", Key = "key1" },
                        FilePath = "path",
                    };

            var json = Serializer.SerializeToString(affectedItem);

            var message = new AbortIfNoNewFileLocationForTopicMessage
                              {
                                  FileLocation = new FileLocation { ContainerLocation = "containerLocation", Container = "container", Key = "key2" },
                                  TopicToCheckAffectedItemsFor = topic,
                                  TopicStatusReports =
                                      new[]
                                          {
                                              new TopicStatusReport
                                                  {
                                                      Topic = new AffectedTopic(topic.Name),
                                                      AffectedItems = new[] { new AffectedItem { Id = json } },
                                                      Status = TopicStatus.WasAffected,
                                                  },
                                          },
                              };

            // act
            handler.HandleAsync(message).Wait();
            Thread.Sleep(50); // make sure all log messages get flushed

            // assert
            Assert.NotNull(Logger.LoggedItems.SingleOrDefault(_ => _.Message.Contains($"Found matching reports for topic: {message.TopicToCheckAffectedItemsFor.Name} with affects completed on: ")));
            Assert.NotNull(Logger.LoggedItems.SingleOrDefault(_ => _.Message.Contains("Found affected item: ")));
            Assert.NotNull(Logger.LoggedItems.SingleOrDefault(_ => _.Message.Contains("Affected items did not match, NOT aborting.")));
        }

        [Fact]
        public void Handle_MatchingStatusReportsWithAffectedItemsDoesNotEqualContainer_DoesNotAbort()
        {
            // arrange
            Logger.PurgeAllLoggedItems();

            var handler = new AbortIfNoNewFileLocationForTopicMessageHandler();
            var topic = new NamedTopic("topic");
            var affectedItem =
                new FileLocationAffectedItem
                    {
                        FileLocationAffectedItemMessage = "did stuff",
                        FileLocation = new FileLocation { ContainerLocation = "containerLocation", Container = "container1", Key = "key" },
                        FilePath = "path",
                    };

            var json = Serializer.SerializeToString(affectedItem);

            var message = new AbortIfNoNewFileLocationForTopicMessage
                              {
                                  FileLocation = new FileLocation { ContainerLocation = "containerLocation", Container = "container2", Key = "key" },
                                  TopicToCheckAffectedItemsFor = topic,
                                  TopicStatusReports =
                                      new[]
                                          {
                                              new TopicStatusReport
                                                  {
                                                      Topic = new AffectedTopic(topic.Name),
                                                      AffectedItems = new[] { new AffectedItem { Id = json } },
                                                      Status = TopicStatus.WasAffected,
                                                  },
                                          },
                              };

            // act
            handler.HandleAsync(message).Wait();
            Thread.Sleep(50); // make sure all log messages get flushed

            // assert
            Assert.NotNull(Logger.LoggedItems.SingleOrDefault(_ => _.Message.Contains($"Found matching reports for topic: {message.TopicToCheckAffectedItemsFor.Name} with affects completed on: ")));
            Assert.NotNull(Logger.LoggedItems.SingleOrDefault(_ => _.Message.Contains("Found affected item: ")));
            Assert.NotNull(Logger.LoggedItems.SingleOrDefault(_ => _.Message.Contains("Affected items did not match, NOT aborting.")));
        }

        [Fact]
        public void Handle_MatchingStatusReportsWithAffectedItemsDoesNotEqualContainerLocation_DoesNotAbort()
        {
            // arrange
            Logger.PurgeAllLoggedItems();

            var handler = new AbortIfNoNewFileLocationForTopicMessageHandler();
            var topic = new NamedTopic("topic");
            var affectedItem =
                new FileLocationAffectedItem
                    {
                        FileLocationAffectedItemMessage = "did stuff",
                        FileLocation = new FileLocation { ContainerLocation = "containerLocation1", Container = "container", Key = "key" },
                        FilePath = "path",
                    };

            var json = Serializer.SerializeToString(affectedItem);

            var message = new AbortIfNoNewFileLocationForTopicMessage
                              {
                                  FileLocation = new FileLocation { ContainerLocation = "containerLocation2", Container = "container", Key = "key" },
                                  TopicToCheckAffectedItemsFor = topic,
                                  TopicStatusReports =
                                      new[]
                                          {
                                              new TopicStatusReport
                                                  {
                                                      Topic = new AffectedTopic(topic.Name),
                                                      AffectedItems = new[] { new AffectedItem { Id = json } },
                                                      Status = TopicStatus.WasAffected,
                                                  },
                                          },
                              };

            // act
            handler.HandleAsync(message).Wait();
            Thread.Sleep(50); // make sure all log messages get flushed

            // assert
            Assert.NotNull(Logger.LoggedItems.SingleOrDefault(_ => _.Message.Contains($"Found matching reports for topic: {message.TopicToCheckAffectedItemsFor.Name} with affects completed on: ")));
            Assert.NotNull(Logger.LoggedItems.SingleOrDefault(_ => _.Message.Contains("Found affected item: ")));
            Assert.NotNull(Logger.LoggedItems.SingleOrDefault(_ => _.Message.Contains("Affected items did not match, NOT aborting.")));
        }

        [Fact]
        public void Handle_MatchingStatusReportsButNoAffectedItems_DoesNotAbort()
        {
            // arrange
            Logger.PurgeAllLoggedItems();
            var handler = new AbortIfNoNewFileLocationForTopicMessageHandler();
            var topic = new NamedTopic("topic");
            var message = new AbortIfNoNewFileLocationForTopicMessage
                              {
                                  FileLocation = new FileLocation { ContainerLocation = "containerLocation", Container = "container", Key = "key" },
                                  TopicToCheckAffectedItemsFor = topic,
                                  TopicStatusReports = new[] { new TopicStatusReport { Topic = new AffectedTopic(topic.Name), Status = TopicStatus.WasAffected } },
                              };

            // act
            handler.HandleAsync(message).Wait();
            Thread.Sleep(50); // make sure all log messages get flushed

            // assert
            Assert.NotNull(Logger.LoggedItems.SingleOrDefault(_ => _.Message.Contains($"Found matching reports for topic: {message.TopicToCheckAffectedItemsFor.Name} with affects completed on: ")));
            Assert.NotNull(Logger.LoggedItems.SingleOrDefault(_ => _.Message.Contains("Did not find any affected items with expected token: ")));
        }

        [Fact]
        public void Handle_NoMatchingStatusReports_DoesNotAbort()
        {
            // arrange
            Logger.PurgeAllLoggedItems();

            var handler = new AbortIfNoNewFileLocationForTopicMessageHandler();
            var message = new AbortIfNoNewFileLocationForTopicMessage
                              {
                                  FileLocation = new FileLocation { ContainerLocation = "containerLocation", Container = "container", Key = "key" },
                                  TopicToCheckAffectedItemsFor = new NamedTopic("topic"),
                                  TopicStatusReports = new[] { new TopicStatusReport { Topic = new AffectedTopic("other topic") } },
                              };

            // act
            handler.HandleAsync(message).Wait();
            Thread.Sleep(50); // make sure all log messages get flushed

            // assert
            Assert.NotNull(Logger.LoggedItems.SingleOrDefault(_ => _.Message.Contains($"Did not find matching reports for topic: {message.TopicToCheckAffectedItemsFor.Name}")));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Emtpy", Justification = "Spelling/name is correct.")]
        [Fact]
        public void Handle_EmtpyStatusReports_DoesNotAbort()
        {
            // arrange
            var handler = new AbortIfNoNewFileLocationForTopicMessageHandler();
            var message = new AbortIfNoNewFileLocationForTopicMessage
                              {
                                  FileLocation = new FileLocation { ContainerLocation = "containerLocation", Container = "container", Key = "key" },
                                  TopicToCheckAffectedItemsFor = new NamedTopic("topic"),
                                  TopicStatusReports = new TopicStatusReport[0],
                              };

            // act
            handler.HandleAsync(message).Wait();
            Thread.Sleep(50); // make sure all log messages get flushed

            // assert
            Assert.NotNull(Logger.LoggedItems.FirstOrDefault(_ => _.Message.Contains($"Did not find matching reports for topic: {message.TopicToCheckAffectedItemsFor.Name}")));
        }

        [Fact]
        public void Handle_FileLocationIsNull_ThrowsArgumentException()
        {
            // arrange
            var handler = new AbortIfNoNewFileLocationForTopicMessageHandler();
            var message = new AbortIfNoNewFileLocationForTopicMessage
                              {
                                  FileLocation = null,
                                  TopicToCheckAffectedItemsFor = new NamedTopic("topic"),
                                  TopicStatusReports = new TopicStatusReport[0],
                              };

            // act
            Action action = () => handler.HandleAsync(message).Wait();
            var ex = Record.Exception(action);

            // assert
            Assert.NotNull(ex);
            Assert.NotNull(ex.InnerException);
            Assert.IsType<ArgumentException>(ex.InnerException);
            Assert.Equal("Must provide a file location.", ex.InnerException.Message);
        }

        [Fact]
        public void Handle_TopicStatusReportsIsNull_ThrowsArgumentException()
        {
            // arrange
            var handler = new AbortIfNoNewFileLocationForTopicMessageHandler();
            var message = new AbortIfNoNewFileLocationForTopicMessage
                              {
                                  FileLocation = new FileLocation { ContainerLocation = "containerLocation", Container = "container", Key = "key" },
                                  TopicToCheckAffectedItemsFor = new NamedTopic("topic"),
                                  TopicStatusReports = null,
                              };

            // act
            Action action = () => handler.HandleAsync(message).Wait();
            var ex = Record.Exception(action);

            // assert
            Assert.NotNull(ex);
            Assert.NotNull(ex.InnerException);
            Assert.IsType<ArgumentException>(ex.InnerException);
            Assert.Equal("Must supply topic status reports or empty collection.", ex.InnerException.Message);
        }

        [Fact]
        public void Handle_TopicToCheckAffectedItemsForIsNull_ThrowsArgumentException()
        {
            // arrange
            var handler = new AbortIfNoNewFileLocationForTopicMessageHandler();
            var message = new AbortIfNoNewFileLocationForTopicMessage
                              {
                                  FileLocation = new FileLocation { ContainerLocation = "containerLocation", Container = "container", Key = "key" },
                                  TopicToCheckAffectedItemsFor = null,
                                  TopicStatusReports = new TopicStatusReport[0],
                              };

            // act
            Action action = () => handler.HandleAsync(message).Wait();
            var ex = Record.Exception(action);

            // assert
            Assert.NotNull(ex);
            Assert.NotNull(ex.InnerException);
            Assert.IsType<ArgumentException>(ex.InnerException);
            Assert.Equal("Must supply topic to check.", ex.InnerException.Message);
        }
    }
}
