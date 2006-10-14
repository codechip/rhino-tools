using System;
using System.Drawing;

namespace Netron.Lithium.Interfaces
{
	/// <summary>
	/// Draw a particular connection
	/// </summary>
	public interface IConnectionLayoutStrategy
	{
		/// <summary>
		/// Draw the connection
		/// </summary>
		void DrawConnection(Graphics g, Pen pen, Connection connection);

		bool Hit(Point p, Connection connection);
	}
}
