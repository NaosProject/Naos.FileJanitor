// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Retry.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.FileJanitor.MessageBus.Handler
{
    using System;
    using System.Threading.Tasks;

    using Polly;

    /// <summary>
    /// Retry utility.
    /// </summary>
    internal static class Retry
    {
        /// <summary>
        /// Runs a function with specified number or retries (3 is default).
        /// </summary>
        /// <param name="func">Function to run.</param>
        /// <param name="retryCount">Retry count (Default: 3).</param>
        /// <returns>Task for async.</returns>
        public static async Task RunAsync(Func<Task> func, int retryCount = 3)
        {
            await Policy.Handle<Exception>().WaitAndRetry(retryCount, attempt => TimeSpan.FromSeconds(attempt * 5)).Execute(func);
        }

        /// <summary>
        /// Runs a function with specified number or retries (3 is default).
        /// </summary>
        /// <param name="func">Function to run.</param>
        /// <param name="retryCount">Retry count (Default: 3).</param>
        /// <returns>Specified return type.</returns>
        /// <typeparam name="T">Type of return of the provided function.</typeparam>
        public static async Task<T> RunAsync<T>(Func<Task<T>> func, int retryCount = 3)
        {
            return await Policy.Handle<Exception>().WaitAndRetry(retryCount, attempt => TimeSpan.FromSeconds(attempt * 5)).Execute(func);
        }
    }
}
