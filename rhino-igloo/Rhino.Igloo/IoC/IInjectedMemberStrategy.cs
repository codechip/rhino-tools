using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Rhino.Igloo
{
    internal interface IInjectedMemberStrategy
    {
        bool IsSatisfiedBy(ScopeType scope);
        void SetValueFor(string key, PropertyInfo property, Object instance);
    }

    internal class InputsInjectedMemberStrategy : IInjectedMemberStrategy
    {
        public bool IsSatisfiedBy(ScopeType scope)
        {
            return scope == ScopeType.Inputs;
        }

        public void SetValueFor(string key, PropertyInfo property, Object instance)
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

        public void SetValueFor(string key, PropertyInfo property, Object instance)
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

        public void SetValueFor(string key, PropertyInfo property, Object instance)
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

        public void SetValueFor(string key, PropertyInfo property, Object instance)
        {
            property.SetValue(instance, Scope.Flash[key], null);
        }
    }
}
