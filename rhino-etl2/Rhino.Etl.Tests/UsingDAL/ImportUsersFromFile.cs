namespace Rhino.Etl.Tests.UsingDAL
{
    using System.Collections.Generic;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class ImportUsersFromFile : EtlProcess
    {
        protected override void Initialize()
        {
            Register(new ReadUsersFromFile());
            Register(new SaveToDal());
        }
    }
}