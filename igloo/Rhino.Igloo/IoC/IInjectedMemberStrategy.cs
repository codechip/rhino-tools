using System;
using System.Reflection;

namespace Rhino.Igloo
{
    internal interface IInjectedMemberStrategy
    {
        bool IsSatisfiedBy(ScopeType scope);
        void SetValueFor(Object instance, string key, PropertyInfo property);
    }

    internal class InputsInjectedMemberStrategy : IInjectedMemberStrategy
    {
        public bool IsSatisfiedBy(ScopeType scope)
        {
            return scope == ScopeType.Inputs;
        }

        public void SetValueFor(Object instance, string key, PropertyInfo property)
        {
            if (Scope.Inputs[key] == null) return;

            object results = ConversionUtil.ConvertTo(property.PropertyType, Scope.Inputs[key]);
            if (results != null) property.SetValue(instance, results, null);
        }
    }

    internal class InputInjectedMemberStrategy : IInjectedMemberStrategy
    {
        public bool IsSatisfiedBy(ScopeType scope)
        {
            return scope == ScopeType.Input;
        }

        public void SetValueFor(Object instance, string key, PropertyInfo property)
        {
            if (Scope.Input[key] == null) return;
            object result = ConversionUtil.ConvertTo(property.PropertyType, Scope.Input[key]);
            if (result != null) property.SetValue(instance, result, null);
        }
    }

    internal class SessionInjectedMemberStrategy : IInjectedMemberStrategy
    {
        public bool IsSatisfiedBy(ScopeType scope)
        {
            return scope == ScopeType.Session;
        }

        public void SetValueFor(Object instance, string key, PropertyInfo property)
        {
            property.SetValue(instance, Scope.Session[key], null);
        }
    }

    internal class FlashInjectedMemberStrategy : IInjectedMemberStrategy
    {
        public bool IsSatisfiedBy(ScopeType scope)
        {
            return scope == ScopeType.Flash;
        }

        public void SetValueFor(Object instance, string key, PropertyInfo property)
        {
            property.SetValue(instance, Scope.Flash[key], null);
        }
    }

    internal class ThrowingInjectedMemberStrategy : IInjectedMemberStrategy
    {
        private ScopeType _scope;

        public bool IsSatisfiedBy(ScopeType scope)
        {
            _scope = scope;
            return true;
        }

        public void SetValueFor(Object instance, string key, PropertyInfo property)
        {
            throw new ArgumentOutOfRangeException("ScopeType.Scope");
        }
    }
}
