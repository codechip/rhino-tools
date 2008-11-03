using DSL.Demo.Model;

namespace DSL.Demo.Declerative
{
    public interface ICommand
    {
        void Execute(Order order);
    }
}