using System;
using UnityEngine;

namespace BehaviorGraph
{
    [RequireComponent(typeof(BehaviorTree))]
    public abstract class BehaviorData : MonoBehaviour
    {
        // No-op
    }

    [Serializable]
    public class DataField
    {
        // No-op
    }

    [Serializable]
    public class DataField<T> : DataField// where T : struct
    {
        public DataField(T value)
        {
            Value = value;
        }

        public T Value;
    }
}
