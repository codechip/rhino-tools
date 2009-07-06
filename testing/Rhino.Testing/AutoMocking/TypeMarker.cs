using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Testing.AutoMocking
{
    /// <summary>
    /// Syntax sugar. Used in conjuction with <see cref="IAutoMockingRepository.Mark"/> to indicate
    /// which type of mock (if any) should be created when the auto mocking container needs to resolve
    /// dependencies
    /// </summary>
    public class TypeMarker
    {
        private readonly IAutoMockingRepository _repository;
        private readonly Type _type;

        public TypeMarker(Type type, IAutoMockingRepository repository)
        {
            _repository = repository;
            _type = type;
        }

        public void Dynamic()
        {
            _repository.SetMockingStrategy(_type, new DynamicMockingStrategy(_repository));
        }

        public void Stubbed()
        {
            _repository.SetMockingStrategy(_type, new StubbedStrategy(_repository));
        }

        public void NotMocked()
        {
            _repository.SetMockingStrategy(_type, new NonMockedStrategy(_repository));
        }

        public void Missing()
        {
            _repository.MarkMissing(_type);
        }

        public void NonDynamic()
        {
            _repository.SetMockingStrategy(_type, new StandardMockingStrategy(_repository));
        }

        public void MultiMock(params Type[] extraInterfaces)
        {
          _repository.SetMockingStrategy(_type, new MultiMockingStrategy(_repository, extraInterfaces));
        }
    }
}
