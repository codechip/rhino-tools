using Rhino.Commons;

namespace RhinoIglooSample.Test.Model
{
    [IsCandidateForRepository]
    public interface IUser
    {
        int Id { get; }
        string Name { get; set; }
    }
}
