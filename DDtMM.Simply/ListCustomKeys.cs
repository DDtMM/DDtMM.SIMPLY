using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDtMM.SIMPLY
{
    /// <summary>
    /// Stores custom keys for an IList.  Easy to add this[TKey] to the list and use this to find values.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ListCustomKeys<TKey, TValue>
    {
        public delegate TKey KeyGetter(TValue item);

        private KeyGetter getter;
        private Dictionary<TKey, int> customKeys;
        private IList<TValue> myList;

        public ListCustomKeys(IList<TValue> list, KeyGetter getter)
        {
            myList = list;
            customKeys = new Dictionary<TKey, int>();
            this.getter = getter;
        }

        public int GetIndex(TKey key) 
        {
            if (customKeys.ContainsKey(key)) 
            {
                int index = customKeys[key];
                if (myList.Count > index) 
                {
                    if (key.Equals(getter(myList[index]))) 
                    {
                        return index;
                    }
                }
                // if we didn't return a value there is something wrong with the record so remove it
                customKeys.Remove(key);
            }
            
            var foundIndex = myList.Select((v, i) => new { v, i }).FirstOrDefault(a => getter(a.v).Equals(key));
            
            if (foundIndex != null) 
            {
                customKeys.Add(key, foundIndex.i);
                return foundIndex.i;
            }

            return -1;
        }
        public TValue GetValue(TKey key) 
        {
            return myList[GetIndex(key)];
        }

        public bool ContainsKey(TKey key)
        {
            return (GetIndex(key) >= 0);
        }
    }
}
