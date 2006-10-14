using System;
using System.Drawing;
using Netron.Lithium.Interfaces;

namespace Netron.Lithium.Impl.ConnectionLayout
{
	/// <summary>
	/// the traditional connection is a rectangular connections which mimics the traditional
	/// layout of hierarchies and flowcharts
	/// </summary>
	public class TraditionalVerticalConnectionLayoutStrategy : IConnectionLayoutStrategy
	{
		public void DrawConnection(Graphics g, Pen pen, Connection connection)
		{
			PointF p1 = new PointF(connection.From.Left + connection.From.Width/2, connection.From.Top - (connection.From.Top - connection.To.Bottom)/2);
			PointF p2 = new PointF(connection.To.Left + connection.To.Width/2, connection.From.Top - (connection.From.Top - connection.To.Bottom)/2);
			g.DrawLine(pen, connection.Start, p1);
			g.DrawLine(pen, p1, p2);
			g.DrawLine(pen, connection.End, p2);
		}

		public bool Hit(Point p, Connection connection)
		{
			PointF p1,p2,p3;
			RectangleF r1,r2,r3;
			p1 = new PointF(connection.From.Left + connection.From.Width/2 - 5, connection.From.Top - (connection.From.Top - connection.To.Bottom)/2 - 5); //shift 5 to contain the connection
			p2 = new PointF(connection.To.Left + connection.To.Width/2 - 5, connection.From.Top - (connection.From.Top - connection.To.Bottom)/2 - 5);
			p3 = new Point(connection.To.Left + connection.To.Width/2 - 5, connection.To.Bottom - 5);

			r1 = new RectangleF(p1, new SizeF(10, (connection.From.Top - connection.To.Bottom)/2 + 5));
			if (p1.X < p2.X)
				r2 = new RectangleF(p1, new SizeF(p2.X - p1.X, 10));
			else
				r2 = new RectangleF(p2, new SizeF(p1.X - p2.X, 10));
			r3 = new RectangleF(p3, new SizeF(10, (connection.From.Top - connection.To.Bottom)/2 + 5));
			return r1.Contains(p.X, p.Y) || r2.Contains(p.X, p.Y) || r3.Contains(p.X, p.Y);

		}

	}

	/// <summary>
	/// the traditional connection is a rectangular connections which mimics the traditional
	/// layout of hierarchies and flowcharts
	/// </summary>
	public class TraditionalHorizontalConnectionLayoutStrategy : IConnectionLayoutStrategy
	{
		public void DrawConnection(Graphics g, Pen pen, Connection connection)
		{
			PointF p1 = new PointF(connection.To.Right + (connection.From.Left - connection.To.Right)/2, connection.From.Top + connection.From.Height/2);
			PointF p2 = new PointF(connection.To.Right + (connection.From.Left - connection.To.Right)/2, connection.To.Top + connection.To.Height/2);
			g.DrawLine(pen, connection.Start, p1);
			g.DrawLine(pen, p1, p2);
			g.DrawLine(pen, connection.End, p2);
		}

		public bool Hit(Point p, Connection connection)
		{
			PointF p1,p2;
			p1 = new PointF(connection.To.Right + (connection.From.Left - connection.To.Right)/2, connection.From.Top + connection.From.Height/2);
			p2 = new PointF(connection.To.Right + (connection.From.Left - connection.To.Right)/2, connection.To.Top + connection.To.Height/2);

			return false;
		}
	}

	public class TraditionalTreeConnectionLayoutStrategy : IConnectionLayoutStrategy
	{
		public void DrawConnection(Graphics g, Pen pen, Connection connection)
		{
			Point p1 =  new Point(connection.To.Left + (connection.To.Width/2),
				connection.From.Top + connection.From.Height/2);
			g.DrawLine(pen, connection.Start, p1);
			g.DrawLine(pen, p1, connection.End);

		}

		public bool Hit(Point p, Connection connection)
		{
			return false;
		}

	}
}
