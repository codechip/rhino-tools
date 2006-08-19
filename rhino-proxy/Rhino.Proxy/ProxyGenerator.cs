using System;
using System.Collections;

namespace Rhino.Proxy
{
	public class ProxyGenerator
	{
		public CodeDOMProxyBuilder ProxyBuilder
		{
			get { return builder; }
		}

		protected CodeDOMProxyBuilder builder = new CodeDOMProxyBuilder();
		
		public virtual object CreateClassProxy(
			Type baseClass, IInterceptor interceptor, params object[] argumentsForConstructor)
		{
			AssertUtil.IsClass(baseClass, "baseClass");
			AssertUtil.NotNull(interceptor, "interceptor");
			AssertUtil.NotNull(argumentsForConstructor, "argumentsForConstructor");
			Type type = ProxyBuilder.CreateProxy(baseClass, new Type[0]);
			return CreateProxyInstance(type, interceptor, argumentsForConstructor);
		}

		public virtual object CreateClassProxy(
			Type baseClass, Type[] interfaces, IInterceptor interceptor, params object[] argumentsForConstructor)
		{
			AssertUtil.IsInterface(interfaces, "interfaces");
			Type type = ProxyBuilder.CreateProxy(baseClass, interfaces);
			return CreateProxyInstance(type, interceptor, argumentsForConstructor);
		}


		/// <summary>
		/// Generates a proxy implementing all the specified interfaces and
		/// redirecting method invocations to the specifed interceptor.
		/// </summary>
		/// <param name="theInterface">Interface to be implemented</param>
		/// <param name="interceptor">instance of <see cref="IInterceptor"/></param>
		/// <param name="target">The proxy target.</param>
		/// <returns>Proxy instance</returns>
		public virtual object CreateProxy(Type theInterface, IInterceptor interceptor, object target)
		{
			return CreateProxy(new Type[] {theInterface}, interceptor, target);
		}

		/// <summary>
		/// Generates a proxy implementing all the specified interfaces and
		/// redirecting method invocations to the specifed interceptor.
		/// </summary>
		/// <param name="interfaces">Array of interfaces to be implemented</param>
		/// <param name="interceptor">instance of <see cref="IInterceptor"/></param>
		/// <param name="target">The proxy target.</param>
		/// <returns>Proxy instance</returns>
		public virtual object CreateProxy(Type[] interfaces, IInterceptor interceptor, object target)
		{
			AssertUtil.IsInterface(interfaces, "interfaces");
			AssertUtil.NotNull(interceptor, "interceptor");
			AssertUtil.NotNull(target, "target");

			Type newType = ProxyBuilder.CreateInterfaceProxy(target.GetType(), interfaces);
			return CreateProxyInstance(newType, interceptor, target);
		}


		public virtual object CreateProxy(Type baseClass, IInterceptor interceptor, params object[] argumentsForConstructor)
		{
			AssertUtil.NotNull(baseClass, "baseClass");
			AssertUtil.NotNull(interceptor, "interceptor");

			Type newType = ProxyBuilder.CreateProxy(baseClass, new Type[0]);
			return CreateClassProxyInstance(newType, interceptor, argumentsForConstructor);
		}


		public virtual object CreateProxy(
			Type baseClass, Type[] interfaces, IInterceptor interceptor, params object[] argumentsForConstructor)
		{
			AssertUtil.IsClass(baseClass, "baseClass");
			AssertUtil.NotNull(interceptor, "interceptor");
			AssertUtil.IsInterface(interfaces, "interfaces");

			Type newType = ProxyBuilder.CreateProxy(baseClass, interfaces);

			return CreateClassProxyInstance(newType, interceptor, argumentsForConstructor);
		}


		public virtual object CreateCustomClassProxy(
			Type baseClass, IInterceptor interceptor, GeneratorContext context, params object[] argumentsForConstructor)
		{
			AssertUtil.IsClass(baseClass, "baseClass");
			AssertUtil.NotNull(interceptor, "interceptor");
			AssertUtil.NotNull(context, "context");

			Type newType = ProxyBuilder.CreateCustomClassProxy(baseClass, new Type[0], context);
			return CreateCustomClassProxyInstance(newType, interceptor, context, argumentsForConstructor);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="theInterface"></param>
		/// <param name="interceptor"></param>
		/// <param name="context"></param>
		/// <param name="target">The proxy target.</param>
		/// <returns></returns>
		public virtual object CreateCustomProxy(
			Type theInterface, IInterceptor interceptor, object target, GeneratorContext context)
		{
			return CreateCustomProxy(new Type[] {theInterface}, interceptor, target, context);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="interfaces"></param>
		/// <param name="interceptor"></param>
		/// <param name="context"></param>
		/// <param name="target">The proxy target.</param>
		/// <returns></returns>
		public virtual object CreateCustomProxy(
			Type[] interfaces, IInterceptor interceptor, object target, GeneratorContext context)
		{
			AssertUtil.IsInterface(interfaces, "interfaces");
			AssertUtil.NotNull(interceptor, "interceptor");
			AssertUtil.NotNull(target, "target");
			AssertUtil.NotNull(context, "context");

			Type newType = ProxyBuilder.CreateCustomInterfaceProxy(interfaces, target.GetType(), context);
			return CreateCustomProxyInstance(newType, interceptor, target, context);
		}

		protected virtual object CreateProxyInstance(Type type, IInterceptor interceptor, object target)
		{
			return Activator.CreateInstance(type, new object[] {interceptor, target});
		}

		protected virtual object CreateCustomProxyInstance(
			Type type, IInterceptor interceptor, object target, GeneratorContext context)
		{
			if (context.HasMixins)
			{
				return Activator.CreateInstance(type, new object[] {interceptor, target, context.MixinsAsArray()});
			}
			return CreateProxyInstance(type, interceptor, target);
		}

		protected virtual object CreateClassProxyInstance(
			Type type, IInterceptor interceptor, params object[] argumentsForConstructor)
		{
			ArrayList args = new ArrayList();
			args.Add(interceptor);
			args.AddRange(argumentsForConstructor);

			return Activator.CreateInstance(type, args.ToArray());
		}

		private object[] CreateArguments(IInterceptor interceptor, object[] argumentsForConstructor)
		{
			ArrayList list = new ArrayList(argumentsForConstructor.Length + 1);
			list.Add(interceptor);
			list.AddRange(argumentsForConstructor);
			return (object[]) list.ToArray(typeof (object));
		}

		private object CreateProxyInstance(Type type, IInterceptor interceptor, params object[] argumentsForConstructor)
		{
			try
			{
				//this is to allow virtual method calls in the constructor.
				ConstructorOnlyInterceptor.Current = interceptor;
				return Activator.CreateInstance(type, CreateArguments(interceptor, argumentsForConstructor));
			}
			finally
			{
				ConstructorOnlyInterceptor.Current = null;
			}
		}

		protected virtual object CreateCustomClassProxyInstance(
			Type type, IInterceptor interceptor, GeneratorContext context, params object[] argumentsForConstructor)
		{
			if (context.HasMixins)
			{
				ArrayList args = new ArrayList();
				args.Add(interceptor);
				args.Add(context.MixinsAsArray());
				args.AddRange(argumentsForConstructor);

				return Activator.CreateInstance(type, args.ToArray());
			}
			return CreateClassProxyInstance(type, interceptor, argumentsForConstructor);
		}
	}
}