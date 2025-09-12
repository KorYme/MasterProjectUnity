using System;

namespace SimpleGraph
{
    [Serializable]
    public class SimplePortData<T>
    {
        public T PortData { get; set; }
        
        public static implicit operator T(SimplePortData<T> d) => d.PortData;
    }
}
