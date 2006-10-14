using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
namespace Netron.Lithium
{
	/// <summary>
	/// Base class for shapes
	/// </summary>
	public abstract class ShapeBase : Entity
	{
		#region Fields
		/// <summary>
		/// whether this shape if the root
		/// </summary>
		protected bool isRoot = false;
		/// <summary>
		/// the child nodes collection
		/// </summary>
		protected ShapeCollection childNodes;
		/// <summary>
		/// used to drag child nodes
		/// </summary>
		protected bool pickup = false;
		/// <summary>
		/// points to the unique parent of this shape, unless it's the root and then Null
		/// </summary>
		protected ShapeBase parentNode = null;
		/// <summary>
		/// whether the shape is expanded
		/// If expanded, all the child nodes will have visible=true and vice versa
		/// </summary>
		protected bool expanded = false;
		/// <summary>
		/// this is the unique link to the parent unless this shape is the root
		/// </summary>
		protected Connection connection = null;
		/// <summary>
		/// used by the visiting pattern and tags whether this shape has been visited already
		/// </summary>
		protected bool visited = false;

		private Rectangle rectangle;

		private string text;

		#endregion

		#region Properties

		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		public Rectangle Rectangle
		{
			get { return rectangle; }
			set
			{
				rectangle = value;
				OnResize();
			}
		}

		[Browsable(false)]
		public Connection Connection
		{
			get { return connection; }
			set { connection = value; }
		}

		[Browsable(false)]
		public bool Pickup
		{
			get { return pickup; }
			set { pickup = value; }
		}

		[Browsable(false)]
		public bool Visited
		{
			get { return visited; }
			set { visited = value; }
		}

		/// <summary>
		/// Gets or sets the child node collection of this shape
		/// </summary>
		[Browsable(false)]
		public ShapeCollection ChildNodes
		{
			get{return childNodes;}
			set{childNodes = value;}
		}


		/// <summary>
		/// Gets or sets whether this is the root of the diagram
		/// </summary>
		[Browsable(false)]
		public bool IsRoot
		{
			get{return isRoot;}
			set{isRoot = value;}
		}

		/// <summary>
		/// Gets or sets whether the shape is expanded/collapsed
		/// </summary>
		[Browsable(false)]
		public bool Expanded
		{
			get{return expanded;}
			set{expanded = value;}
		}
		/// <summary>
		/// Gets the (unique) parent node of this shape
		/// Null if this is the root
		/// </summary>
		[Browsable(false)]
		public ShapeBase ParentNode
		{
			get{return parentNode;}			
			set{parentNode = value;}
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Default ctor
		/// </summary>
		public ShapeBase()
		{
			childNodes = new ShapeCollection();
		}

		protected override void OnLithiumChanged()
		{
			Fit();
		}


		#endregion

		#region Methods

		/// <summary>
		/// Resizes the shape's rectangle in function of the containing text
		/// </summary>
		public virtual void Fit()
		{
			Graphics g = Graphics.FromHwnd(Lithium.Handle);
			Size s =  Size.Round(g.MeasureString(string.Concat(text,"[",childNodes.Count.ToString(),"]"),this.Font));

			//Put some space around it.
			Rectangle = new Rectangle(Location,
				new Size(s.Width+10,s.Height+8));
		}

		public Point Location
		{
			get { return new Point(Left,Top); }
			set
			{
				Rectangle = new Rectangle(value,new Size(Width, Height));
			}
		}

		/// <summary>
		/// Expand the children, if any
		/// </summary>
		public void Expand()
		{
			OnBeforeExpand();
			expanded = true;			
			Visible = true;
			for(int k =0; k<childNodes.Count;k++)
			{
				childNodes[k].Visible = true;
				childNodes[k].connection.Visible = true;
				if(childNodes[k].expanded) childNodes[k].Expand();
			}
			OnAfterExpand();
			Lithium.DrawTree();	
		}
		/// <summary>
		/// Collapses the children underneath this shape
		/// </summary>
		public void Collapse()
		{
			expanded = false;
			for(int k =0; k<childNodes.Count;k++)
			{
				childNodes[k].Visible = false;
				childNodes[k].connection.Visible = false;
				if(childNodes[k].Expanded) childNodes[k].Visible= false;
			}
		}
								
		protected override void OnVisibleChanged()
		{
			if(Connection!=null)
				Connection.Visible = Visible;
			if(Expanded)
			{
				for (int i = 0; i < ChildNodes.Count; i++)
				{
					ChildNodes[i].Visible = Visible;
					ChildNodes[i].Connection.Visible = Visible;
				}
			}
		}

		/// <summary>
		/// Adds a child to this shape
		/// </summary>
		/// <param name="shape">the shape to add</param>
		public void AddChild(ShapeBase shape)
		{
//			if(Lithium==null)
//				throw new NotSupportedException("Can't add a child to a shape that is not bound to a lithium");

			shape.Lithium = Lithium;
			//add to the collections
			Lithium.graphAbstract.Shapes.Add(shape);
			this.childNodes.Add(shape);		
			//add a connection; From=child, To=parent
			Connection con = new Connection(shape, this);
			Lithium.Connections.Add(con);
			con.Lithium = this.Lithium;
			con.Visible = Visible;
			shape.connection = con;
			shape.Visible = Visible;
			Fit(); //the child count added at the end will enlarge the rectangle, so we have to fit 
			Lithium.DrawTree();
		}

		

		/// <summary>
		/// Override the abstract Hit method
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public override bool Hit(System.Drawing.Point p)
		{
			return false;
		}

		/// <summary>
		/// Moves the shape with the given shift
		/// </summary>
		/// <param name="p">represent a shift-vector, not the absolute position!</param>
		public override void Move(Point p)
		{	
			Rectangle = new Rectangle(Left+p.X,Top+p.Y, Width,Height);
			this.Invalidate();
		}

		protected virtual void OnBeforeExpand()
		{	
		}

		protected virtual void OnAfterExpand()
		{}

		public override void Invalidate()
		{
			if(Lithium!=null)
				Lithium.Invalidate(Rectangle);
		}

		#endregion

		public virtual void Delete()
		{
			foreach (ShapeBase childNode in childNodes)
			{
				childNode.Delete();
			}
			if(ParentNode!=null)
				ParentNode.ChildNodes.Remove(this);
			Lithium.graphAbstract.Connections.Remove(connection);
			Lithium.graphAbstract.Shapes.Remove(this);
		}

		public int Left
		{
			get { return rectangle.Left; }
			set
			{
				Rectangle = new Rectangle(value,Top,Width,Height);
			}
		}

		public int Width
		{
			get { return rectangle.Width; }
		}

		public int Top
		{
			get { return rectangle.Top; }
			set
			{
				Rectangle = new Rectangle(Left,value,Width,Height);
			}
		}

		public int Height
		{
			get { return rectangle.Height; }
		}

		public Font Font
		{
			get
			{
				if(Lithium==null)
					return SystemInformation.MenuFont;
				return Lithium.Font;
			}
		}

		public virtual void MouseDown(MouseEventArgs e)
		{}

		protected virtual void OnResize()
		{}

		public int Bottom
		{
			get { return Top+Height; }
		}

		public int Right
		{
			get { return Left+Width; }
		}
	}
}
