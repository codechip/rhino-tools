using System.Windows;
namespace Binsor.Presentation.Framework.Interfaces
{
	public interface IApplicationContext
	{
        void Start();

        ILayoutRegistry Layouts { get; } 
    }
}