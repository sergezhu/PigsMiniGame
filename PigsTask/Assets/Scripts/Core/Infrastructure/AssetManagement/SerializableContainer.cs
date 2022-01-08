using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Core.Infrastructure.AssetManagement
{
    [Serializable]
    public class SerializableContainer<T>
    {
        [FormerlySerializedAs("_elements")]
        public List<T> Elements;

        public SerializableContainer()
        {
            Elements = new List<T>();
        }
        
        public SerializableContainer(IEnumerable<T> elements)
        {
            Elements = new List<T>(elements);
        }

        public void Add(T element)
        {
            Elements.Add(element);
        }

        public void Clear()
        {
            Elements.Clear();
        }
    }
}