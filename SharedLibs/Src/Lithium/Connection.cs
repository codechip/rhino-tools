using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Netron.Lithium
{
	/// <summary>
	/// Represents the connection between two connectors
	/// </summary>
	public class Connection : Entity
	{
		#region Fields

		/// <summary>
		/// the shape where the connection starts
		/// </summary>
		protected ShapeBase from;

		/// <summary>
		/// the shape where the connection ends
		/// </summary>
		protected ShapeBase to;

		/// <summary>
		/// the start and end points
		/// </summary>
		protected Point start, end;

		/// <summary>
		/// the pen used to draw the connection,
		/// can switch depending on the hovering state e.g.
		/// </summary>
		protected Pen currentPen;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the shape where the connection starts
		/// </summary>
		public ShapeBase From
		{
			get { return from; }
			set { from = value; }
		}

		/// <summary>
		/// Gets or sets where the connection ends
		/// </summary>
		public ShapeBase To
		{
			get { return to; }
			set { to = value; }
		}

		/// <summary>
		/// Get the point where the connection starts
		/// </summary>
		public Point Start
		{
			get { return new Point(from.Left + from.Width/2, from.Top + from.Height/2); }
		}

		/// <summary>
		/// Gets the point where connection ends
		/// </summary>
		public Point End
		{
			get
			{
				end = new Point(to.Left + to.Width/2, to.Top + to.Height/2);

				return end;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Default ctor
		/// </summary>
		public Connection()
		{}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="from">the shape where the connection starts</param>
		/// <param name="to">the shape where the connection ends</param>
		public Connection(ShapeBase from, ShapeBase to)
		{
			this.from = from;
			this.to = to;
			currentPen = blackPen;

		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="from">the shape where the connection starts</param>
		/// <param name="to">the shape where the connection ends</param>
		/// <param name="color">the color of the connection</param>
		public Connection(ShapeBase from, ShapeBase to, Color color) : this(from, to)
		{
			currentPen = new Pen(color, 1f);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="from">the shape where the connection starts</param>
		/// <param name="to">the shape where the connection ends</param>
		/// <param name="color">the color of the connection</param>
		/// <param name="width">the (float) width of the connection (in pixels)</param>
		public Connection(ShapeBase from, ShapeBase to, Color color, float width) : this(from, to, color)
		{
			currentPen = new Pen(color, width);
		}

		#endregion

		#region Methods

		public override void Invalidate()
		{
			if(Lithium!=null)
				Lithium.Invalidate(Rectangle.Union(to.Rectangle,from.Rectangle));
		}

		/// <summary>
		/// Paints the connection on the canvas
		/// The From part is always the child node while the To part is 
		/// always the parent node.
		/// Hence; 
		/// - vertical: Parent->Child <=> Top->Bottom
		/// - horizontal: Parent->Child <=> Left->Right
		/// </summary>
		public override void Paint(Graphics g)
		{
			if (Visible)
			{
				if (IsSelected)
					pen = redPen;
				else
					pen = currentPen;

				Lithium.ConnectionLayoutStrategy.DrawConnection(g,pen,this);
			}

		}

		/// <summary>
		/// Tests if the mouse hits this connection
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public override bool Hit(Point p)
		{
			return Lithium.ConnectionLayoutStrategy.Hit(p,this);
		}

		/// <summary>
		/// Moves the connection with the given shift
		/// </summary>
		/// <param name="p"></param>
		public override void Move(Point p)
		{}

		#endregion
	}
}