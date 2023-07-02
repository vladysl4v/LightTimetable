using Newtonsoft.Json;

using System;
using System.IO;
using System.Collections.Generic;


namespace LightTimetable.Tools
{
    /// <summary>
    /// Manages data saving and retrieving by storing it in JSON file for certain time.
    /// </summary>
    /// <typeparam name="T">Any normally serializable/deserializable type.</typeparam>
    public class CacheManager<T>
    {
        /// <summary>
        /// The time after which the cache becomes absolete
        /// and becomes possible to set a new data. 
        /// </summary>
        private readonly TimeSpan _cacheLifetime;
        private readonly string _fullPath;
        private readonly object _fileLock;

        /// <param name="nameTag">
        /// Cache file name tag.
        /// </param>
        /// <param name="cacheLifetime">
        /// The time after which the cache becomes absolete
        /// and becomes possible to set a new data.
        /// </param>
        public CacheManager(string nameTag, TimeSpan cacheLifetime)
        {
            _fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $".{nameTag}_cache.json");
            _cacheLifetime = cacheLifetime;
            _fileLock = new object();
        }

        /// <summary>
        /// The condition that will check the extra data for correctness. <br/>
        /// If there are differences in data, the cache will be overwritten
        /// or be not returned.
        /// </summary>
        /// <value>Condition that checks extra data correctness.</value>
        public Func<Dictionary<string, object>, bool>? ExtraDataCondition { get; init; }
        
        /// <summary>
        /// A dictionary of additional data for the cache.
        /// </summary>
        public Dictionary<string, object>? ExtraData { get; init; }
        
        /// <summary>
        /// Sets the new cache value if previous value is stale
        /// or the ExtraDataCondition passes.
        /// </summary>
        public void SetCache(T objectToCache)
        {
            if (objectToCache == null)
                return;

            var cached = GetCachedExtraData();

            // Write to file if ExtraDataCondition not null and passes
            // or previous cache is stale.
            if ((cached != null && (ExtraDataCondition?.Invoke(cached) ?? false)) ||
                GetLastCachedTime() > _cacheLifetime)
            {
                WriteToJsonFile(objectToCache);
            }
        }

        /// <summary>
        /// Get current cache value if the data is fresh. <br/>
        /// Returns null if the data is obsolete or the ExtraDataCondition passes.
        /// </summary>
        public T? GetCache()
        {
            var cached = GetCachedExtraData();

            // Returns null if ExtraDataCondition not null and passes
            // or previous cache is stale.
            if ((cached != null && (ExtraDataCondition?.Invoke(cached) ?? false)) ||
                GetLastCachedTime() > _cacheLifetime)
            {
                return default(T);
            }
            
            return ReadFromJsonFile();
        }
        
        /// <summary>
        /// Get current cache extra data.<br/>
        /// Returns null if the file does not exist or error occured.
        /// </summary>
        public Dictionary<string, object>? GetCachedExtraData()
        {
            if (!File.Exists(_fullPath))
                return null;

            var dataReader = new StreamReader(_fullPath);
            string fileAdditionalData;

            lock (_fileLock)
            {
                try
                {
                    fileAdditionalData = dataReader.ReadLine();
                }
                catch
                {
                    return null;
                }
                finally
                {
                    dataReader?.Close();
                }
            }
 
            return JsonConvert.DeserializeObject<Dictionary<string, object>?>(fileAdditionalData);
        }


        private TimeSpan? GetLastCachedTime()
        {
            return File.Exists(_fullPath)
                    ? DateTime.Now - File.GetLastWriteTime(_fullPath)
                    : TimeSpan.MaxValue;
        }

        private void WriteToJsonFile(T data)
        {
            var dataWriter = new StreamWriter(_fullPath, false);

            lock (_fileLock)
            {
                try
                {
                    dataWriter.WriteLine(JsonConvert.SerializeObject(ExtraData));
                    dataWriter.Write(JsonConvert.SerializeObject(data));
                }
                finally
                {
                    dataWriter?.Close();
                }
            }
        }

        private T? ReadFromJsonFile()
        {
            if (!File.Exists(_fullPath))
            {
                return default(T);
            }

            var dataReader = new StreamReader(_fullPath);
            string fileContent;

            lock (_fileLock)
            {
                try
                {
                    // skip extra data line
                    dataReader.ReadLine();
                    fileContent = dataReader.ReadToEnd();
                }
                catch
                {
                    return default(T);
                }
                finally
                {
                    dataReader?.Close();
                }
            }
 
            return JsonConvert.DeserializeObject<T>(fileContent);
        }
    }
}