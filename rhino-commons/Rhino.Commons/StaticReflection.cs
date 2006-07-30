using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Rhino.Commons
{
    public static class StaticReflection
    {
        #region Procedures

        public static MethodInfo MethodInfo(Proc func0)
        {
            return func0.Method;
        }
        public static MethodInfo MethodInfo<A0>(Proc<A0> func1)
        {
            return func1.Method;
        }
        public static MethodInfo MethodInfo<A0, A1>(Proc<A0, A1> func2)
        {
            return func2.Method;
        }
        public static MethodInfo MethodInfo<A0, A1, A2>(Proc<A0, A1, A2> func3)
        {
            return func3.Method;
        }
        public static MethodInfo MethodInfo<A0, A1, A2, A3>(Proc<A0, A1, A2, A3> func4)
        {
            return func4.Method;
        }
        public static MethodInfo MethodInfo<A0, A1, A2, A3, A4>(Proc<A0, A1, A2, A3, A4> func5)
        {
            return func5.Method;
        }
        public static MethodInfo MethodInfo<A0, A1, A2, A3, A4, A5>(Proc<A0, A1, A2, A3, A4, A5> func6)
        {
            return func6.Method;
        }
        public static MethodInfo MethodInfo<A0, A1, A2, A3, A4, A5, A6>(Proc<A0, A1, A2, A3, A4, A5, A6> func7)
        {
            return func7.Method;
        }
        public static MethodInfo MethodInfo<A0, A1, A2, A3, A4, A5, A6, A7>(Proc<A0, A1, A2, A3, A4, A5, A6, A7> func8)
        {
            return func8.Method;
        }
        public static MethodInfo MethodInfo<A0, A1, A2, A3, A4, A5, A6, A7, A8>(Proc<A0, A1, A2, A3, A4, A5, A6, A7, A8> func9)
        {
            return func9.Method;
        }

        #endregion

        #region Functions

        public static MethodInfo MethodInfo<TRet>(Func<TRet> func0)
        {
            return func0.Method;
        }

        public static MethodInfo MethodInfo<TRet, A0>(Func<TRet, A0> func1)
        {
            return func1.Method;
        }

        public static MethodInfo MethodInfo<TRet, A0, A1>(Func<TRet, A0, A1> func2)
        {
            return func2.Method;
        }

        public static MethodInfo MethodInfo<TRet, A0, A1, A2>(Func<TRet, A0, A1, A2> func3)
        {
            return func3.Method;
        }

        public static MethodInfo MethodInfo<TRet, A0, A1, A2, A3>(Func<TRet, A0, A1, A2, A3> func4)
        {
            return func4.Method;
        }

        public static MethodInfo MethodInfo<TRet, A0, A1, A2, A3, A4>(Func<TRet, A0, A1, A2, A3, A4> func5)
        {
            return func5.Method;
        }

        public static MethodInfo MethodInfo<TRet, A0, A1, A2, A3, A4, A5>(Func<TRet, A0, A1, A2, A3, A4, A5> func6)
        {
            return func6.Method;
        }

        public static MethodInfo MethodInfo<TRet, A0, A1, A2, A3, A4, A5, A6>(Func<TRet, A0, A1, A2, A3, A4, A5, A6> func7)
        {
            return func7.Method;
        }

        public static MethodInfo MethodInfo<TRet, A0, A1, A2, A3, A4, A5, A6, A7>(Func<TRet, A0, A1, A2, A3, A4, A5, A6, A7> func8)
        {
            return func8.Method;
        }

        public static MethodInfo MethodInfo<TRet, A0, A1, A2, A3, A4, A5, A6, A7, A8>(Func<TRet, A0, A1, A2, A3, A4, A5, A6, A7, A8> func9)
        {
            return func9.Method;
        }


        #endregion
    }
}
