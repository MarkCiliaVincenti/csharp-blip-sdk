﻿using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Lime.Messaging.Resources;
using System;
using Lime.Protocol.Network;
using Take.Blip.Client.Extensions.Contacts;
using Take.Blip.Client.Extensions.Directory;
using Microsoft.Extensions.Caching.Memory;

namespace Take.Blip.Client.Receivers
{
    /// <summary>
    /// Defines a <see cref="IMessageReceiver"/> that includes contact information about the message sender.
    /// </summary>
    public abstract class ContactMessageReceiverBase : IMessageReceiver, IDisposable
    {
        private readonly IContactExtension _contactExtension;
        private readonly IDirectoryExtension _directoryExtension;
        private readonly bool _cacheLocally;
        private readonly MemoryCache _contactCache;
        private readonly TimeSpan _cacheExpiration;

        /// <summary>
        /// Initializes a new instance of <see cref="ContactMessageReceiverBase"/> class.
        /// </summary>
        /// <param name="contactExtension"></param>
        /// <param name="directoryExtension"></param>
        /// <param name="cacheLocally">Indicates if the contact information should be cached locally.</param>
        /// <param name="cacheExpiration">Defines the local cache expiration, if configured.</param>
        protected ContactMessageReceiverBase(
            IContactExtension contactExtension,
            IDirectoryExtension directoryExtension,
            bool cacheLocally = true,
            TimeSpan cacheExpiration = default(TimeSpan))
        {
            _directoryExtension = directoryExtension;
            _contactExtension = contactExtension;
            _cacheLocally = cacheLocally;
            _contactCache = new MemoryCache(new MemoryCacheOptions());
            _cacheExpiration = cacheExpiration == default(TimeSpan) ? TimeSpan.FromMinutes(30) : cacheExpiration;
        }

        public async Task ReceiveAsync(Message envelope, CancellationToken cancellationToken = default(CancellationToken))
        {
            var identity = envelope.From.ToIdentity();

            // First, tries get it from the cache, if configured.
            Contact contact = null;
            if (_cacheLocally) contact = _contactCache.Get(identity.ToString()) as Contact;
            
            if (contact == null)
            {
                try
                {
                    // Second, try from the roster.
                    contact = await _contactExtension.GetAsync(identity, cancellationToken);
                }
                catch (LimeException ex) when (ex.Reason.Code == ReasonCodes.COMMAND_RESOURCE_NOT_FOUND || ex.Reason.Code == ReasonCodes.COMMAND_RESOURCE_NOT_SUPPORTED) { }

                // Third, try from the directory.
                if (contact == null)
                {
                    try
                    {
                        contact = await GetContactFromDirectoryAsync(identity, cancellationToken);
                    }
                    catch (LimeException ex) when (ex.Reason.Code == ReasonCodes.COMMAND_RESOURCE_NOT_FOUND || ex.Reason.Code == ReasonCodes.COMMAND_RESOURCE_NOT_SUPPORTED) { }
                }
                
                // Stores in the cache, if configured.
                if (contact != null && _cacheLocally)
                {
                    _contactCache.Set(identity.ToString(), contact, DateTimeOffset.UtcNow.Add(_cacheExpiration));
                }
            }

            await ReceiveAsync(envelope, contact, cancellationToken);
        }

        /// <summary>
        /// Receives a message with the contact information.
        /// </summary>
        protected abstract Task ReceiveAsync(Message message, Contact contact, CancellationToken cancellationToken = default(CancellationToken));

        private async Task<Contact> GetContactFromDirectoryAsync(Identity identity, CancellationToken cancellationToken)
        {
            var contact = new Contact
            {
                Identity = identity
            };

            var account = await _directoryExtension.GetDirectoryAccountAsync(identity, cancellationToken);
            if (account != null)
            {
                contact.Name = account.FullName;
                contact.Address = account.Address;
                contact.CellPhoneNumber = account.CellPhoneNumber;
                contact.City = account.City;
                contact.Culture = account.Culture;
                contact.Email = account.Email;
                contact.Extras = account.Extras;
                contact.Gender = account.Gender;
                contact.PhoneNumber = account.PhoneNumber;
                contact.PhotoUri = account.PhotoUri;
                contact.Timezone = account.Timezone;
            }

            return contact;
        }        

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _contactCache.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

    }
}
