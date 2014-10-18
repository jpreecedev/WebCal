namespace Webcal.Shared
{
    using System;

    public interface ISettingsRepository<T> where T : BaseSettings
    {
        T Get(Func<T, bool> filter = null, params string[] includes);
        void Save(T settings);
    }
}