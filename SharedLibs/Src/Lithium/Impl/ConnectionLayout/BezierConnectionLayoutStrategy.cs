using System;
using System.Drawing;
using Netron.Lithium.Interfaces;

namespace Netron.Lithium.Impl.ConnectionLayout
{
	/// <summary>
	/// a smoothly curved form connecting the shapes
	/// </summary>
	public class BezierVerticalConnectionLayoutStrategy : IConnectionLayoutStrategy
	{
		public void DrawConnection(Graphics g, Pen pen, Connection connection)
		{
			PointF p1 = new PointF(connection.From.Left + connection.From.Width/2, connection.From.Top);
			PointF p2 = new PointF(connection.From.Left + connection.From.Width/2, connection.From.Top - (connection.From.Top - connection.To.Bottom)/2);
			PointF p3 = new PointF(connection.To.Left + connection.To.Width/2, connection.From.Top - (connection.From.Top - connection.To.Bottom)/2);
			PointF p4 = new PointF(connection.To.Left + connection.To.Width/2, connection.To.Bottom);
			g.DrawBezier(pen, p1, p2, p3, p4);
		}

		public bool Hit(Point p, Connection connection)
		{
			return false;
		}

	}

	public class BezierHorizontalConnectionLayoutStrategy : IConnectionLayoutStrategy
	{
		public void DrawConnection(Graphics g, Pen pen, Connection connection)
		{
			PointF p1 = new PointF(connection.From.Left + connection.From.Width/2, connection.From.Top);
			PointF p2 = new PointF(connection.From.Left + connection.From.Width/2, connection.From.Top - (connection.From.Top - connection.To.Bottom)/2);
			PointF p3 = new PointF(connection.To.Left + connection.To.Width/2, connection.From.Top - (connection.From.Top - connection.To.Bottom)/2);
			PointF p4 = new PointF(connection.To.Left + connection.To.Width/2, connection.To.Bottom);
			g.DrawBezier(pen, p1, p2, p3, p4);
		}

		public bool Hit(Point p, Connection connection)
		{
			return false;
		}

	}
}
