using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Commons
{
    public class ElementNotfoundException : Exception
    {
        public ElementNotfoundException()
        {
        }
        public ElementNotfoundException(string message) : base(message)
        {
        }
    }
}
