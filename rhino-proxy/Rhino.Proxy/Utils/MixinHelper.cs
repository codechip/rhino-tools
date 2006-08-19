using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Rhino.Proxy.Utils
{
	class MixinHelper
	{
		private Hashtable interface2Mixin = new Hashtable();

		GeneratorContext context;
		
		public MixinHelper(GeneratorContext context)
		{
			this.context = context;
		}

		public bool IsMixinMethod(MethodInfo method)
		{
			return interface2Mixin.ContainsKey(method.DeclaringType);
		}
		
		public int GetMixinIndex(MethodInfo method)
		{
			return (int) interface2Mixin[method.DeclaringType];
		}
		
		public Type[] JoinInterfacesAndMixins(Type[] initialInterfaces)
		{
			Type[] mixinInterfaces = InspectAndRegisterInterfaces(context.MixinsAsArray());
			if (mixinInterfaces.Length == 0)
			{
				return initialInterfaces;
			}
			else
			{
				Set set = new Set();
				set.AddArray(initialInterfaces);
				set.AddArray(mixinInterfaces);
				return (Type[])set.ToArray(typeof(Type));
			}
		}

		protected Type[] InspectAndRegisterInterfaces(object[] mixins)
		{
			if (mixins == null) return new Type[0];

			Set allMixinInterfaces = new Set();

			for (int i = 0; i < mixins.Length; ++i)
			{
				object mixin = mixins[i];

				Type[] mixinInterfaces = mixin.GetType().GetInterfaces();
				mixinInterfaces = Filter(mixinInterfaces);

				allMixinInterfaces.AddArray(mixinInterfaces);

				// Later we gonna need to say which mixin
				// handle the method of a specific interface
				foreach (Type inter in mixinInterfaces)
				{
					interface2Mixin.Add(inter, i);
				}
			}

			return (Type[])allMixinInterfaces.ToArray(typeof(Type));
		}

		protected  Type[] Filter(Type[] mixinInterfaces)
		{
			ArrayList retType = new ArrayList();
			foreach (Type type in mixinInterfaces)
			{
				if (!context.ShouldSkip(type))
					retType.Add(type);
			}

			return (Type[])retType.ToArray(typeof(Type));
		}
	}
}
