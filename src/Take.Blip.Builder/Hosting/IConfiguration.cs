﻿using System;

namespace Take.Blip.Builder.Hosting
{
    public interface IConfiguration
    {
        TimeSpan ExecutionSemaphoreExpiration { get; }

        TimeSpan SessionExpiration { get; }

        string RedisStorageConfiguration { get; }
    }

    public class ConventionsConfiguration : IConfiguration
    {
        public TimeSpan ExecutionSemaphoreExpiration => TimeSpan.FromMinutes(5);

        public TimeSpan SessionExpiration => TimeSpan.FromMinutes(30);

        public string RedisStorageConfiguration => "localhost";
    }
}
