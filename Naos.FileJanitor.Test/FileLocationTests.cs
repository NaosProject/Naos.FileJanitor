// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLocationTests.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.Test
{
    using System;

    using Naos.FileJanitor.MessageBus.Contract;
    using Naos.FileJanitor.MessageBus.Handler;

    using Xunit;

    public class FileLocationTests
    {
        [Fact]
        public void Equal_AreEqual()
        {
            var containerLocation = Guid.NewGuid().ToString().ToUpperInvariant();
            var container = Guid.NewGuid().ToString().ToUpperInvariant();
            var key = Guid.NewGuid().ToString();

            var first = new FileLocation { ContainerLocation = containerLocation, Container = container, Key = key };

            var secondContainerLocation = containerLocation;
            var secondContainer = container;
            var secondKey = key;
            var second = new FileLocation { ContainerLocation = secondContainerLocation, Container = secondContainer, Key = secondKey };

            Assert.True(first == second);
            Assert.False(first != second);
            Assert.True(first.Equals(second));
            Assert.True(first.Equals((object)second));
            Assert.Equal(first, second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void NotEqualAreNotEqual_ContainerLocation()
        {
            var containerLocation = Guid.NewGuid().ToString().ToUpperInvariant();
            var container = Guid.NewGuid().ToString().ToUpperInvariant();
            var key = Guid.NewGuid().ToString();
                        var first = new FileLocation { ContainerLocation = containerLocation, Container = container, Key = key };

            var secondContainerLocation = Guid.NewGuid().ToString().ToUpperInvariant();
            var secondContainer = container;
            var secondKey = key;
            var second = new FileLocation { ContainerLocation = secondContainerLocation, Container = secondContainer, Key = secondKey };

            Assert.False(first == second);
            Assert.True(first != second);
            Assert.False(first.Equals(second));
            Assert.False(first.Equals((object)second));
            Assert.NotEqual(first, second);
            Assert.NotEqual(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void NotEqualAreNotEqual_Container()
        {
            var containerLocation = Guid.NewGuid().ToString().ToUpperInvariant();
            var container = Guid.NewGuid().ToString().ToUpperInvariant();
            var key = Guid.NewGuid().ToString();
                        var first = new FileLocation { ContainerLocation = containerLocation, Container = container, Key = key };

            var secondContainerLocation = containerLocation;
            var secondContainer = Guid.NewGuid().ToString().ToUpperInvariant();
            var secondKey = key;
            var second = new FileLocation { ContainerLocation = secondContainerLocation, Container = secondContainer, Key = secondKey };

            Assert.False(first == second);
            Assert.True(first != second);
            Assert.False(first.Equals(second));
            Assert.False(first.Equals((object)second));
            Assert.NotEqual(first, second);
            Assert.NotEqual(first.GetHashCode(), second.GetHashCode());
        }

        [Fact]
        public void NotEqualAreNotEqual_Key()
        {
            var containerLocation = Guid.NewGuid().ToString().ToUpperInvariant();
            var container = Guid.NewGuid().ToString().ToUpperInvariant();
            var key = Guid.NewGuid().ToString();
                        var first = new FileLocation { ContainerLocation = containerLocation, Container = container, Key = key };

            var secondContainerLocation = containerLocation;
            var secondContainer = container;
            var secondKey = Guid.NewGuid().ToString();
            var second = new FileLocation { ContainerLocation = secondContainerLocation, Container = secondContainer, Key = secondKey };

            Assert.False(first == second);
            Assert.True(first != second);
            Assert.False(first.Equals(second));
            Assert.False(first.Equals((object)second));
            Assert.NotEqual(first, second);
            Assert.NotEqual(first.GetHashCode(), second.GetHashCode());
        }
    }
}
