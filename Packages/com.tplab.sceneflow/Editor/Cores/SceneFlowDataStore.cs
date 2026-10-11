using System;
using System.Collections.Generic;

namespace TpLab.SceneFlow.Editor.Cores
{
    public sealed class SceneFlowDataStore
    {
        readonly Dictionary<Type, object> _values = new Dictionary<Type, object>();

        public void Set<T>(T value) where T : class
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var type = typeof(T);

            if (_values.ContainsKey(type))
            {
                throw new InvalidOperationException($"SceneFlow data is already registered: {type.FullName}");
            }

            _values.Add(type, value);
        }

        public bool TryGet<T>(out T value) where T : class
        {
            if (_values.TryGetValue(typeof(T), out var stored))
            {
                value = (T)stored;
                return true;
            }

            value = null;
            return false;
        }

        public T GetRequired<T>() where T : class
        {
            if (TryGet<T>(out var value)) return value;

            throw new InvalidOperationException($"SceneFlow data is not registered: {typeof(T).FullName}");
        }

        public bool Remove<T>() where T : class
        {
            return _values.Remove(typeof(T));
        }

        public void Clear()
        {
            _values.Clear();
        }
    }
}