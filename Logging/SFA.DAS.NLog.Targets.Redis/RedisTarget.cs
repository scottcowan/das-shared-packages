﻿namespace NLog.Targets
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Microsoft.Azure;
    using Newtonsoft.Json;
    using NLog.Config;

    [Target("Redis")]
    public sealed class RedisTarget : TargetWithLayout
    {
        private const string AppNameKey = "app_Name";

        private RedisConnectionManager _redisConnectionManager;
        private string _key;

        [RequiredParameter]
        public string AppName { get; set; }

        [RequiredParameter]
        public string ConnectionStringKey { get; set; }

        [RequiredParameter]
        public string KeySettingsKey { get; set; }

        public string EnvironmentKey { get; set; }

        public bool IncludeAllProperties { get; set; }

        public RedisTarget()
        {
        }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            _key = CloudConfigurationManager.GetSetting(KeySettingsKey);

            var connectionString = CloudConfigurationManager.GetSetting(ConnectionStringKey);

            _redisConnectionManager = new RedisConnectionManager(connectionString);
        }

        protected override void CloseTarget()
        {

            _redisConnectionManager?.Dispose();

            base.CloseTarget();
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var dict = GetDictionary(logEvent, IncludeAllProperties);
            var redisValue = CreateRedisJsonValue(dict);

            var redisDatabase = _redisConnectionManager.GetDatabase();

            redisDatabase.ListRightPush(_key, redisValue);
        }

        protected override void Write(Common.AsyncLogEventInfo logEvent)
        {
            var dict = GetDictionary(logEvent.LogEvent, IncludeAllProperties);
            var redisValue = CreateRedisJsonValue(dict);

            var redisDatabase = _redisConnectionManager.GetDatabase();

            redisDatabase.ListRightPushAsync(_key, redisValue);
        }

        private IDictionary<object, object> GetDictionary(LogEventInfo logEvent, bool includeProperties)
        {
            var message = Layout.Render(logEvent);
            var properties = !includeProperties 
                ? new Dictionary<object, object>() 
                : logEvent.Properties;
            
            properties.Add("message", message);
            properties.Add("level", logEvent.Level.Name);
            properties.Add("@timestamp", logEvent.TimeStamp);
            if (!properties.ContainsKey(AppNameKey))
            {
                properties.Add(AppNameKey, AppName);
            }

            if (!properties.ContainsKey("Environment") && !string.IsNullOrEmpty(EnvironmentKey))
            {
                var environment = CloudConfigurationManager.GetSetting(EnvironmentKey);
                if (environment != null)
                {
                    properties.Add("Environment", environment);
                }
            }

            if (logEvent.Exception != null)
            {
                if (!properties.ContainsKey("Exception"))
                {
                    dynamic innerException = null;

                    if (logEvent.Exception.InnerException != null)
                    {
                        innerException = new { message = logEvent.Exception.InnerException.Message, source = logEvent.Exception.InnerException.Source, stackTrace = logEvent.Exception.InnerException.StackTrace, type = logEvent.Exception.InnerException.GetType().Name };
                    }

                    properties.Add("Exception", new { message = logEvent.Exception.Message, source = logEvent.Exception.Source, innerException = innerException, stackTrace = logEvent.Exception.StackTrace, type = logEvent.Exception.GetType().Name });
                }
            }

            return properties;
        }

        private string CreateRedisJsonValue(IDictionary<object, object> properties)
        {
            return JsonConvert.SerializeObject(properties);
        }
    }
}
