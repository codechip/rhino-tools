using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel;

namespace Rhino.Commons.Binsor
{
    public class Facility : IQuackFu
    {
        private readonly Type facility;
        private readonly string key;
        private readonly IConfiguration configuration = new MutableConfiguration("facility");

        public Facility(string name, Type facility)
        {
            this.key = name;
            this.facility = facility;
        }

        public string Key
        {
            get { return key; }
        }

        public Facility Register()
        {
            IKernel kernel = IoC.Container.Kernel;
            kernel.ConfigurationStore.AddFacilityConfiguration(key, configuration);
            kernel.AddFacility(key, (IFacility)Activator.CreateInstance(facility));
            return this;
        }

        public object QuackGet(string name, object[] property_parameters)
        {
            throw new NotSupportedException("You can't get a property on a facility");
        }

        public object QuackSet(string name, object[] property_parameters, object value)
        {
            if (value is IDictionary)
            {
                configuration.Children.Add(DictionaryToConfiguration(configuration, name, (IDictionary)value));
            }
            else
            {
                configuration.Attributes[name] = (value ?? "").ToString();
            }
            return null;
        }

        public static IConfiguration DictionaryToConfiguration(IConfiguration parentConfig, string name, IDictionary dictionary)
        {
            IConfiguration config = new MutableConfiguration(name);
            parentConfig.Children.Add(config);
            foreach (DictionaryEntry entry in dictionary)
            {
                string configName = entry.Key.ToString();
                if (entry.Value is IDictionary)
                {
                    DictionaryToConfiguration(config, configName, (IDictionary)entry.Value);
                }
                else
                {
                    config.Attributes[configName] = (entry.Value ?? "").ToString();
                }
            }
            return config;
        }

        public object QuackInvoke(string name, params object[] args)
        {
            throw new NotSupportedException("You can't invoke a method on a facility");
        }
    }
}