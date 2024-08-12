using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Common
{
    /// <summary>
    /// make timeout task
    /// </summary>
    internal static class TaskEx
    {
        /// <summary>
        /// Task set timeout
        /// </summary>
        /// <param name="task"></param>
        /// <param name="timeout">max milliseconds</param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public static async Task Timeout(this Task task, int timeout)
        {
            var complete = await Task.WhenAny(task, Task.Delay(timeout));

            if (task != complete)
                throw new TimeoutException();
        }

        /// <summary>
        /// ValueTask set timeout
        /// </summary>
        /// <param name="vt"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public static async ValueTask Timeout(this ValueTask vt, int timeout)
        {
            var task = vt.AsTask();
            var complete = await Task.WhenAny(task, Task.Delay(timeout));

            if (task != complete)
                throw new TimeoutException();
        }

        /// <summary>
        /// Task set timeout
        /// </summary>
        /// <typeparam name="T">result type</typeparam>
        /// <param name="task"></param>
        /// <param name="timeout">max milliseconds</param>
        /// <returns>value after wait</returns>
        /// <exception cref="TimeoutException"></exception>
        public static async Task<T> Timeout<T>(this Task<T> task, int timeout)
        {
            var complete = await Task.WhenAny(task, Task.Delay(timeout));

            if (task != complete)
                throw new TimeoutException();

            return task.Result;
        }

        /// <summary>
        /// ValueTask Set Timeout
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vt"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public static async ValueTask<T> Timeout<T>(this ValueTask<T> vt, int timeout)
        {
            var task = vt.AsTask();
            var complete = await Task.WhenAny(task, Task.Delay(timeout));

            if (task != complete)
                throw new TimeoutException();

            return task.Result;
        }
    }
}
