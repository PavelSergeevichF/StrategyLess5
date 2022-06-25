using System;
using UnityEngine;
using Utils;

namespace UserControlSystem
{
    public abstract class ScriptableObjectValueBase<T> : ScriptableObject, IAwaitable<T>
    {
        public T CurrentValue { get; private set; }
        public Action<T> OnNewValue;
        private T _field;
        public T SetValue
        {
            get => _field;
            set 
            {
                _field = value;
                OnNewValue?.Invoke(_field);
            }
        }
        
        public IAwaiter<T> GetAwaiter()
        {
            return new NewValueNotifier<T>(this);
        }
    }
}