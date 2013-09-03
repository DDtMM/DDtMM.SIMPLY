using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DDtMM.SIMPLY.Visualizer.Model
{
    public class PropertyManager
    {
        public delegate void propertySetterDelegate(string name, object value);

        public propertySetterDelegate PropertySetter;

        private Dictionary<string, object> propertyTable;
        private INotifyPropertyChanged managed;
        private FieldInfo changedField;
        private FieldInfo changingField;


        public PropertyManager(INotifyPropertyChanged managedModel)
        {
            managed = managedModel;
            Type managedType = managed.GetType();

            changedField = managedType.GetField("PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            PropertySetter = SetValueDefault;

            if (managed is INotifyPropertyChanging)
            {
                changingField = managedType.GetField("PropertyChanging", BindingFlags.Instance | BindingFlags.NonPublic);
                PropertySetter = SetValueWithChanging;
            }
            
            propertyTable = new Dictionary<string, object>();
        }

        public bool Set(string name, object value)
        {
            object oldValue;
            if (!propertyTable.ContainsKey(name) || !(oldValue = propertyTable[name]).Equals(value)) 
            {
                PropertySetter(name, value);
                return true;
            }
            return false;
        }

        public object Get(string name)
        {
            if (propertyTable.ContainsKey(name)) return propertyTable[name];
            return null;
        }
        /// <summary>
        /// Type safe method to get a value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Get<T>(string name)
        {
            object value;
            if (propertyTable.ContainsKey(name) && (value = propertyTable[name]) is T) return (T)value ;
            return default(T);
        }

        /// <summary>
        /// Triggers propertychanging then sets the property value, then triggers changed
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void SetValueWithChanging(string name, object value)
        {
            TriggerDelegateInstance(changingField, managed, new object[] { managed, new PropertyChangingEventArgs( name ) });
            propertyTable[name] = value;
            TriggerDelegateInstance(changedField, managed, new object[] { managed, new PropertyChangedEventArgs( name ) });
        }

        /// <summary>
        /// Sets the property value and then triggers Changed Event
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void SetValueDefault(string name, object value)
        {
            propertyTable[name] = value;
            TriggerDelegateInstance(changedField, managed, new object[] { managed, name });
        }

        private void TriggerDelegateInstance(FieldInfo delegateField, object instance, object[] handlerProperties)
        {
            MulticastDelegate delegateInstance = ((MulticastDelegate)delegateField.GetValue(instance));
            if (delegateInstance != null)
            {
                foreach (Delegate handler in delegateInstance.GetInvocationList())
                {
                    handler.Method.Invoke(handler.Target, handlerProperties);
                }
            }
        }
    }

}
