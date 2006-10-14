using Nullables;
using Nullables.NHibernate;

namespace Ayende.NHibernateQueryAnalyzer.ProjectLoader
{
    class CompilerTrick
    {
        /// <summary>
        /// If I don't do this, the compiler will remove the reference to the Nullables library, and I
        /// need this reference.
        /// </summary>
        public Nullables.NullableChar needThisToMakeCompilerKeepReferenceToNullables = new NullableChar();
        public Nullables.NHibernate.EmptyAnsiStringType needThisToMakeCompilerKeepReferenceToNullables_Nhibernate = new EmptyAnsiStringType();
    }
}
