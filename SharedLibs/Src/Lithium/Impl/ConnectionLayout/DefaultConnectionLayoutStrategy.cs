using System.Drawing;
using Netron.Lithium.Interfaces;

namespace Netron.Lithium.Impl.ConnectionLayout
{
	public abstract class DefaultConnectionLayoutStrategyBase : IConnectionLayoutStrategy
	{
		public abstract void DrawConnection(Graphics g, Pen pen, Connection connection);

		public bool Hit(Point p, Connection connection)
		{
			float o, u;
			PointF p1 = connection.Start,
				p2 = connection.End,s;
			RectangleF r1, r2;

			// p1 must be the leftmost point.
			if (p1.X > p2.X)
			{
				s = p2;
				p2 = p1;
				p1 = s;
			}

			//this is specifically necessary when the layout works horizontally
			//the method beneth will not return true as should be in this case
			if (p1.Y == p2.Y)
			{
				p1.Y += -3;
				return new RectangleF(p1, new SizeF(p2.X - p1.X, 6)).Contains(p);
			}
			r1 = new RectangleF(p1.X, p1.Y, 0, 0);
			r2 = new RectangleF(p2.X, p2.Y, 0, 0);
			r1.Inflate(3, 3);
			r2.Inflate(3, 3);
			//this is like a topological neighborhood
			//the connection is shifted left and right
			//and the point under consideration has to be in between.						
			if (RectangleF.Union(r1, r2).Contains(p))
			{
				if (p1.Y < p2.Y) //SWNE
				{
					o = r1.Left + (((r2.Left - r1.Left)*(p.Y - r1.Bottom))/(r2.Bottom - r1.Bottom));
					u = r1.Right + (((r2.Right - r1.Right)*(p.Y - r1.Top))/(r2.Top - r1.Top));
					return ((p.X > o) && (p.X < u));
				}
				else //NWSE
				{
					o = r1.Left + (((r2.Left - r1.Left)*(p.Y - r1.Top))/(r2.Top - r1.Top));
					u = r1.Right + (((r2.Right - r1.Right)*(p.Y - r1.Bottom))/(r2.Bottom - r1.Bottom));
					return ((p.X > o) && (p.X < u));
				}
			}
			return false;
		}
	}

	/// <summary>
	/// The default connection simply connects the centers of the shapes
	/// </summary>
	public class DefaultVerticalConnectionLayoutStrategy : DefaultConnectionLayoutStrategyBase
	{
		public override void DrawConnection(Graphics g, Pen pen, Connection connection)
		{
			PointF p1 = new PointF(connection.From.Left + connection.From.Width/2, connection.From.Top);
			PointF p2 = new PointF(connection.To.Left + connection.To.Width/2, connection.To.Bottom + 5);
			g.DrawLine(pen, p1, p2);
		}
	}

	/// <summary>
	/// The default connection simply connects the centers of the shapes
	/// </summary>
	public class DefaultHorizontalConnectionLayoutStrategy : DefaultConnectionLayoutStrategyBase
	{
		public override void DrawConnection(Graphics g, Pen pen, Connection connection)
		{
			PointF p1 = new PointF(connection.From.Left, connection.From.Top + connection.From.Height/2);
			PointF p2 = new PointF(connection.To.Right + 4, connection.To.Top + connection.To.Height/2);
			g.DrawLine(pen, p1, p2);
		}
	}
}
