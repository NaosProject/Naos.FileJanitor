// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbortIfNoNewFileLocationForTopicMessageHandlerTest.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Its.Log.Instrumentation;

    using Naos.FileJanitor.MessageBus.Contract;
    using Naos.FileJanitor.MessageBus.Handler;
    using Naos.MessageBus.Domain;
    using Naos.MessageBus.Domain.Exceptions;

    using Xunit;

    public class AbortIfNoNewFileLocationForTopicMessageHandlerTest
    {
        static AbortIfNoNewFileLocationForTopicMessageHandlerTest()
        {
            Log.EntryPosted += (sender, args) => LogMessages.Add(args.LogEntry);
        }

        private static readonly List<LogEntry> LogMessages = new List<LogEntry>();

        public AbortIfNoNewFileLocationForTopicMessageHandlerTest()
        {
            LogMessages.Clear();
        }

        [Fact]
        public void Handle_MatchingStatusReportsWithAffectedItemsEqualFileLocation_DoesAbort()
        {
            // arrange
            var handler = new AbortIfNoNewFileLocationForTopicMessageHandler();
            var topic = new NamedTopic("topic");
            var affectedItem = new FileLocationAffectedItem
                                           {
                                               FileLocationAffectedItemMessage = "did stuff",
                                               FileLocation = new FileLocation { ContainerLocation = "containerLocation", Container = "container", Key = "key" },
                                               FilePath = "path"
                                           };
            var affectedItemId = affectedItem.ToJson();
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
                                                      AffectsCompletedDateTimeUtc = affectsCompletedDateTimeUtc
                                                  }
                                          }
                              };

            // act
            Action action = () => handler.HandleAsync(message).Wait();
            var ex = Record.Exception(action);
            Thread.Sleep(50); // make sure all log messages get flushed

            // assert
            Assert.NotNull(LogMessages.SingleOrDefault(_ => _.ToJson().Contains($"Found matching reports for topic: {message.TopicToCheckAffectedItemsFor.Name} with affects completed on: ")));
            Assert.NotNull(LogMessages.SingleOrDefault(_ => _.ToJson().Contains("Found affected item: ")));
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
            var handler = new AbortIfNoNewFileLocationForTopicMessageHandler();
            var topic = new NamedTopic("topic");
            var affectedItem =
                new FileLocationAffectedItem
                    {
                        FileLocationAffectedItemMessage = "did stuff",
                        FileLocation = new FileLocation { ContainerLocation = "containerLocation", Container = "container", Key = "key1" },
                        FilePath = "path"
                    }.ToJson();

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
                                                      AffectedItems = new[] { new AffectedItem { Id = affectedItem } }
                                                  }
                                          }
                              };

            // act
            handler.HandleAsync(message).Wait();
            Thread.Sleep(50); // make sure all log messages get flushed

            // assert
            Assert.NotNull(LogMessages.SingleOrDefault(_ => _.ToJson().Contains($"Found matching reports for topic: {message.TopicToCheckAffectedItemsFor.Name} with affects completed on: ")));
            Assert.NotNull(LogMessages.SingleOrDefault(_ => _.ToJson().Contains("Found affected item: ")));
            Assert.NotNull(LogMessages.SingleOrDefault(_ => _.ToJson().Contains("Affected items did not match, NOT aborting.")));
        }

        [Fact]
        public void Handle_MatchingStatusReportsWithAffectedItemsDoesNotEqualContainer_DoesNotAbort()
        {
            // arrange
            var handler = new AbortIfNoNewFileLocationForTopicMessageHandler();
            var topic = new NamedTopic("topic");
            var affectedItem =
                new FileLocationAffectedItem
                    {
                        FileLocationAffectedItemMessage = "did stuff",
                        FileLocation = new FileLocation { ContainerLocation = "containerLocation", Container = "container1", Key = "key" },
                        FilePath = "path"
                    }.ToJson();

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
                                                      AffectedItems = new[] { new AffectedItem { Id = affectedItem } }
                                                  }
                                          }
                              };

            // act
            handler.HandleAsync(message).Wait();
            Thread.Sleep(50); // make sure all log messages get flushed

            // assert
            Assert.NotNull(LogMessages.SingleOrDefault(_ => _.ToJson().Contains($"Found matching reports for topic: {message.TopicToCheckAffectedItemsFor.Name} with affects completed on: ")));
            Assert.NotNull(LogMessages.SingleOrDefault(_ => _.ToJson().Contains("Found affected item: ")));
            Assert.NotNull(LogMessages.SingleOrDefault(_ => _.ToJson().Contains("Affected items did not match, NOT aborting.")));
        }

        [Fact]
        public void Handle_MatchingStatusReportsWithAffectedItemsDoesNotEqualContainerLocation_DoesNotAbort()
        {
            // arrange
            var handler = new AbortIfNoNewFileLocationForTopicMessageHandler();
            var topic = new NamedTopic("topic");
            var affectedItem =
                new FileLocationAffectedItem
                    {
                        FileLocationAffectedItemMessage = "did stuff",
                        FileLocation = new FileLocation { ContainerLocation = "containerLocation1", Container = "container", Key = "key" },
                        FilePath = "path"
                    }.ToJson();

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
                                                      AffectedItems = new[] { new AffectedItem { Id = affectedItem } }
                                                  }
                                          }
                              };

            // act
            handler.HandleAsync(message).Wait();
            Thread.Sleep(50); // make sure all log messages get flushed

            // assert
            Assert.NotNull(LogMessages.SingleOrDefault(_ => _.ToJson().Contains($"Found matching reports for topic: {message.TopicToCheckAffectedItemsFor.Name} with affects completed on: ")));
            Assert.NotNull(LogMessages.SingleOrDefault(_ => _.ToJson().Contains("Found affected item: ")));
            Assert.NotNull(LogMessages.SingleOrDefault(_ => _.ToJson().Contains("Affected items did not match, NOT aborting.")));
        }

        [Fact]
        public void Handle_MatchingStatusReportsButNoAffectedItems_DoesNotAbort()
        {
            // arrange
            var handler = new AbortIfNoNewFileLocationForTopicMessageHandler();
            var topic = new NamedTopic("topic");
            var message = new AbortIfNoNewFileLocationForTopicMessage
                              {
                                  FileLocation = new FileLocation { ContainerLocation = "containerLocation", Container = "container", Key = "key" },
                                  TopicToCheckAffectedItemsFor = topic,
                                  TopicStatusReports = new[] { new TopicStatusReport { Topic = new AffectedTopic(topic.Name) } }
                              };

            // act
            handler.HandleAsync(message).Wait();
            Thread.Sleep(50); // make sure all log messages get flushed

            // assert
            Assert.NotNull(LogMessages.SingleOrDefault(_ => _.ToJson().Contains($"Found matching reports for topic: {message.TopicToCheckAffectedItemsFor.Name} with affects completed on: ")));
            Assert.NotNull(LogMessages.SingleOrDefault(_ => _.ToJson().Contains("Did not fine any affected items with expected token: ")));
        }

        [Fact]
        public void Handle_NoMatchingStatusReports_DoesNotAbort()
        {
            // arrange
            var handler = new AbortIfNoNewFileLocationForTopicMessageHandler();
            var message = new AbortIfNoNewFileLocationForTopicMessage
                              {
                                  FileLocation = new FileLocation { ContainerLocation = "containerLocation", Container = "container", Key = "key" },
                                  TopicToCheckAffectedItemsFor = new NamedTopic("topic"),
                                  TopicStatusReports = new[] { new TopicStatusReport { Topic = new AffectedTopic("other topic") } }
                              };

            // act
            handler.HandleAsync(message).Wait();
            Thread.Sleep(50); // make sure all log messages get flushed

            // assert
            Assert.NotNull(
                LogMessages.SingleOrDefault(
                    _ => _.ToJson().Contains($"Did not find matching reports for topic: {message.TopicToCheckAffectedItemsFor.Name}")));
        }

        [Fact]
        public void Handle_EmtpyStatusReports_DoesNotAbort()
        {
            // arrange
            var handler = new AbortIfNoNewFileLocationForTopicMessageHandler();
            var message = new AbortIfNoNewFileLocationForTopicMessage
                              {
                                  FileLocation = new FileLocation { ContainerLocation = "containerLocation", Container = "container", Key = "key" },
                                  TopicToCheckAffectedItemsFor = new NamedTopic("topic"),
                                  TopicStatusReports = new TopicStatusReport[0]
                              };

            // act
            handler.HandleAsync(message).Wait();
            Thread.Sleep(50); // make sure all log messages get flushed

            // assert
            Assert.NotNull(
                LogMessages.SingleOrDefault(
                    _ => _.ToJson().Contains($"Did not find matching reports for topic: {message.TopicToCheckAffectedItemsFor.Name}")));
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
                                  TopicStatusReports = new TopicStatusReport[0]
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
                                  TopicStatusReports = null
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
                                  TopicStatusReports = new TopicStatusReport[0]
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
