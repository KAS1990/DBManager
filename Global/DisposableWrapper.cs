using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.Global
{
    public class DisposableWrapper<T> : IDisposable
    {
        bool m_Disposed = false;
        bool m_Attached = false;

        public T Object { get; private set; } = default(T);
        public Action<T> OnDispose { get; private set; } = null;

        public DisposableWrapper(T obj, Action<T> onDispose)
        {
            Object = obj;
            OnDispose = onDispose;
            m_Attached = true;
        }

        public void Dispose()
        {
            if (!m_Disposed && m_Attached)
            {
                OnDispose(Object);
                Detach();
                m_Disposed = true;
            }
        }

        public void Detach()
        {
            Object = default(T);
            OnDispose = null;
            m_Attached = false;
        }

        public static implicit operator T(DisposableWrapper<T> wrapper)
        {
            return wrapper.Object;
        }
    }
}
