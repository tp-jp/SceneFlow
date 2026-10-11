using System;
using System.Collections.Generic;

namespace TpLab.SceneFlow.Editor.Cores
{
    /// <summary>
    /// Pass間で型付きデータを共有するためのストア。
    /// </summary>
    public sealed class SceneFlowDataStore
    {
        readonly Dictionary<Type, object> _values = new Dictionary<Type, object>();

        /// <summary>
        /// 指定した型のデータを登録します。
        /// </summary>
        /// <typeparam name="T">登録するデータの型。</typeparam>
        /// <param name="value">登録するデータ。</param>
        /// <exception cref="ArgumentNullException">データがnullの場合。</exception>
        /// <exception cref="InvalidOperationException">同じ型のデータが既に登録されている場合。</exception>
        public void Set<T>(T value) where T : class
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var type = typeof(T);

            if (!_values.TryAdd(type, value))
            {
                throw new InvalidOperationException($"SceneFlow data is already registered: {type.FullName}");
            }
        }

        /// <summary>
        /// 指定した型のデータの取得を試みます。
        /// </summary>
        /// <typeparam name="T">取得するデータの型。</typeparam>
        /// <param name="value">取得したデータ。登録されていない場合はnull。</param>
        /// <returns>データを取得できた場合はtrue、それ以外はfalse。</returns>
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

        /// <summary>
        /// 指定した型のデータを取得します。
        /// </summary>
        /// <typeparam name="T">取得するデータの型。</typeparam>
        /// <returns>登録されているデータ。</returns>
        /// <exception cref="InvalidOperationException">指定した型のデータが登録されていない場合。</exception>
        public T GetRequired<T>() where T : class
        {
            if (TryGet<T>(out var value)) return value;

            throw new InvalidOperationException($"SceneFlow data is not registered: {typeof(T).FullName}");
        }

        /// <summary>
        /// 指定した型のデータを削除します。
        /// </summary>
        /// <typeparam name="T">削除するデータの型。</typeparam>
        /// <returns>データを削除した場合はtrue、登録されていなかった場合はfalse。</returns>
        public bool Remove<T>() where T : class
        {
            return _values.Remove(typeof(T));
        }

        /// <summary>
        /// 登録されているすべてのデータを削除します。
        /// </summary>
        /// <remarks>
        /// データへの参照を解除するだけで、登録されたオブジェクトのDisposeは実行しません。
        /// </remarks>
        public void Clear()
        {
            _values.Clear();
        }
    }
}