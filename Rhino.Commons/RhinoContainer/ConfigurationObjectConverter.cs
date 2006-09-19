namespace Rhino.Commons
{
	using System;
	using System.Text;
	using Castle.Core.Configuration;
	using System.Reflection;

    using Castle.MicroKernel.SubSystems.Conversion;

	/// <summary>
	/// Perform the conversion by mapping the configuration values
	/// to the object properties.
	/// </summary>
	[Serializable]
	public class ConfigurationObjectConverter : AbstractTypeConverter
	{
		public override bool CanHandleType(Type type)
		{
			return type.IsDefined(typeof(ConfigurationObjectAttribute), true);
		}

		public override object PerformConversion(string value, Type targetType)
		{
			throw new NotImplementedException();
		}

		public override object PerformConversion(IConfiguration configuration, Type targetType)
		{
			object instance = Activator.CreateInstance(targetType);

			BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | 
				BindingFlags.Public | BindingFlags.NonPublic;
			foreach (IConfiguration itemConfig in configuration.Children)
			{
				PropertyInfo propInfo = targetType.GetProperty(itemConfig.Name, bindingFlags);
				
				if (propInfo == null)//in configuration and not in the object? this is an error.
					throw new InvalidOperationException(
						string.Format("Could not find property {0} on type {1}", itemConfig.Name, targetType));
				
				if(propInfo.CanWrite==false)
					throw new InvalidOperationException(
						string.Format("Could not set property {0} on type {1}. It has no setter", 
						itemConfig.Name, targetType));

				object value = Context.Composition.PerformConversion(itemConfig.Value, propInfo.PropertyType);

				propInfo.SetValue(instance, value, null);
			}

			return instance;
		}
	}
}
