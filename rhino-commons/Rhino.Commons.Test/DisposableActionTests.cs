using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Rhino.Commons.Test
{
    [TestFixture]
    public class DisposableActionTests
    {
        [Test]
        public void DisposableActionCalledOnDispose()
        {
            bool disposableCalled = false;
            using (DisposableAction action = new DisposableAction(delegate { disposableCalled = true; })) 
            {}
            Assert.IsTrue(disposableCalled);
        }
        
        [Test]
    public void DisposableActionGetsCorrectParameterFromCtor()
        {
            int expected = 4543;
            int actual = 0;
            DisposableAction<int> action = new DisposableAction<int>(delegate(int i)
            {
                actual = i;
            }, expected);
            action.Dispose();
            
            Assert.AreEqual(expected, actual);
        }
    }
}
