namespace Uno.Net.Http
{
    public static class HttpMessageCache
    {
        static bool _isInitialized;
        static bool _isCacheDisabled;
        static long _maxCacheSizeInBytes;

        static public bool IsCacheEnabled
        {
            get { return !_isCacheDisabled; }
            set
            {
                if (_isInitialized)
                    debug_log "Uno.Net.Http.HttpMessageCache: Changes to IsCacheEnabled are ignored after initialization.";
                else
                    _isCacheDisabled = !value;
            }
        }

        // Default: 0, for platform-specific default
        static public long MaxCacheSizeInBytes
        {
            get { return _maxCacheSizeInBytes; }
            set
            {
                if (_isInitialized)
                    debug_log "Uno.Net.Http.HttpMessageCache: Changes to MaxCacheSizeInBytes are ignored after initialization.";
                else
                    _maxCacheSizeInBytes = value;
            }
        }

        static HttpMessageCache()
        {
            if defined(MOBILE||MAC)
                Uno.Platform.CoreApp.Started += OnApplicationStarted;
        }

        static void OnApplicationStarted(Uno.Platform.ApplicationState state)
        {
            Init();
        }

        public static void Init()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            if defined(MOBILE||MAC)
                Uno.Platform.CoreApp.Started -= OnApplicationStarted;

            if defined(ANDROID)
            {
                if (!IsCacheEnabled)
                    return;

                long cacheSize = _maxCacheSizeInBytes != 0
                    ? _maxCacheSizeInBytes
                    : 20 * 1024 * 1024;
                Android.com.fuse.ExperimentalHttp.HttpRequest.InstallCache(Android.Base.JNI.GetWrappedActivityObject(), cacheSize);
            }

            if defined(APPLE)
                Uno.Net.Http.Implementation.iOSHttpSharedCache.SetupSharedCache(IsCacheEnabled, MaxCacheSizeInBytes);
        }

        public static void Delete()
        {
            if defined(ANDROID)
                Android.com.fuse.ExperimentalHttp.HttpRequest.CacheDelete();

            if defined(APPLE)
                Uno.Net.Http.Implementation.iOSHttpSharedCache.PurgeSharedCache();
        }
    }
}
