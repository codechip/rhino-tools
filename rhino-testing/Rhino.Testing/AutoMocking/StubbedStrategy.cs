using System;
using System.Reflection;
using Castle.MicroKernel;
using Rhino.Mocks;

namespace Rhino.Testing.AutoMocking
{
    public class StubbedStrategy : AbstractMockingStrategy
    {
        #region Member Data

        private StandardMockingStrategy _default;

        #endregion

        #region StubbedStrategy()

        public StubbedStrategy(IAutoMockingRepository autoMock)
            : base(autoMock)
        {
            _default = new StandardMockingStrategy(autoMock);
        }

        #endregion

        public override object Create(CreationContext context, Type type)
        {
            object target = Mocks.CreateMock(type);
            AutoMock.AddService(type, target);
            foreach (PropertyInfo property in type.GetProperties())
            {
                IMockingStrategy strategy = AutoMock.GetMockingStrategy(property.PropertyType);
                object value = strategy.Create(context, property.PropertyType);
                Expect.Call(property.GetValue(target, null)).Repeat.Any().Return(value);
            }
            Mocks.Replay(target);
            return target;
        }
    }
}