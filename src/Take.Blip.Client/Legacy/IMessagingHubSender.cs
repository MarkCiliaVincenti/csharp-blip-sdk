﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
using Lime.Protocol;
using Take.Blip.Client;

namespace Takenet.MessagingHub.Client.Sender
{
    [Obsolete("Use the ISender interface instead")]
    public interface IMessagingHubSender : ISender
    {
    }

    public static class MessagingHubSenderExtensions
    {
        [Obsolete("Use the ProcessCommandAsync method instead")]
        public static Task<Command> SendCommandAsync(this IMessagingHubSender sender, Command command, CancellationToken cancellationToken = default(CancellationToken))
            => sender.ProcessCommandAsync(command, cancellationToken);

        /// <summary>
        /// Send a command through the available connection and awaits for its response.
        /// </summary>
        /// <param name="command">Command to be sent</param>
        /// <param name="cancellationToken">A cancellation token to allow the task to be canceled</param>
        /// <returns>A task representing the sending operation. When completed, it will contain the command response</returns>
        public static Task<Command> ProcessCommandAsync(this IMessagingHubSender sender, Command command, CancellationToken cancellationToken = default(CancellationToken))
            => sender.ProcessCommandAsync(command, cancellationToken);

        /// <summary>
        /// Send a command response through the available connection.
        /// </summary>
        /// <param name="command">Command to be sent</param>
        /// <param name="cancellationToken">A cancellation token to allow the task to be canceled</param>
        /// <returns>A task representing the sending operation.</returns>
        public static Task SendCommandResponseAsync(this IMessagingHubSender sender, Command command, CancellationToken cancellationToken = default(CancellationToken))
            => sender.SendCommandAsync(command, cancellationToken);

        /// <summary>
        /// Send a message through the available connection.
        /// </summary>
        /// <param name="message">Message to be sent</param>
        /// <param name="cancellationToken">A cancellation token to allow the task to be canceled</param>
        /// <returns>A task representing the sending operation</returns>
        public static Task SendMessageAsync(this IMessagingHubSender sender, Message message, CancellationToken cancellationToken = default(CancellationToken))
            => sender.SendMessageAsync(message, cancellationToken);

        /// <summary>
        /// Send a notification through the available connection.
        /// </summary>
        /// <param name="notification">Notification to be sent</param>
        /// <param name="cancellationToken">A cancellation token to allow the task to be canceled</param>
        /// <returns>A task representing the sending operation</returns>
        public static Task SendNotificationAsync(this IMessagingHubSender sender, Notification notification, CancellationToken cancellationToken = default(CancellationToken))
            => sender.SendNotificationAsync(notification, cancellationToken);

    }
}
