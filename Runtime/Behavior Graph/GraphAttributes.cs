using System;

namespace BehaviorGraph
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BaseBehaviorElement : Attribute
    {
        // No-op
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AllowMultipleTransition : Attribute
    {
        // No-op
    }
}
