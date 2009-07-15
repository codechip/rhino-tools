namespace Rhino.Testing.Tests.AutoMocking
{
  public class PartialMockComponent
  {
    private NotImplementService _service;

    public PartialMockComponent(NotImplementService service)
    {
      _service = service;
    }

    public void DoEverything()
    {
      _service.DoNothing();
      _service.DoSomething();
    }
  }

  public abstract class NotImplementService
  {
    public abstract void DoNothing();

    public virtual void DoSomething()
    {
      
    }
  }
}