using System;
using System.Drawing;
using Netron.Lithium.Impl.ConnectionLayout;
using Netron.Lithium.Interfaces;

namespace Netron.Lithium.Impl.ShapeLayout
{
	/// <summary>
	/// Summary description for TreeLayoutStrategy.
	/// </summary>
	public class TreeLayoutStrategy	 : IShapeLayoutStrategy
	{
		protected int direction = 1;
		int branchHeight = 30;
		int wordSpacing = 30;

		public int BranchHeight
		{
			get { return branchHeight; }
			set { branchHeight = value; }
		}

		public int WordSpacing
		{
			get { return wordSpacing; }
			set { wordSpacing = value; }
		}

		public int DrawTree(ShapeBase root, int shiftLeft, int shiftTop, 
			ref Size size)
		{
			int childrenHeight = 15 + root.Height;
			int horizonalspacing = WordSpacing + root.Width/2;
			if(root.Expanded)
			{
				for (int i = 0; i < root.ChildNodes.Count; i++)
				{	   
					if(root.ChildNodes[i].Visible)
					{
						childrenHeight += DrawTree(root.ChildNodes[i],
							shiftLeft+horizonalspacing,
							shiftTop+childrenHeight*direction,ref size);
					}
				}
			}
			root.Left = shiftLeft;
			root.Top = shiftTop;
			size.Width = Math.Max(root.Right,size.Width);
			size.Height = Math.Max(root.Bottom,size.Height);
			return childrenHeight;
		}

		public virtual Rectangle GetToggleRectangle(ShapeBase shape)
		{
			return new Rectangle(shape.Left-11 , shape.Top +shape.Height/2-5 , 10, 10);
		}

		public virtual Point GetDefaultRootPosition(LithiumControl site, ShapeBase root)
		{
			return new Point(15,5);
		}

		public virtual IConnectionLayoutStrategy CreateConnectionLayoutStrategy(Lithium.ConnectionLayout connectionLayout)
		{
			switch(connectionLayout)
			{
				case Lithium.ConnectionLayout.Default:								   
					return new DefaultVerticalConnectionLayoutStrategy();
				case Lithium.ConnectionLayout.Traditional:
					return new TraditionalTreeConnectionLayoutStrategy();
				case Lithium.ConnectionLayout.Bezier:
					return new BezierVerticalConnectionLayoutStrategy();
				default:
					throw new NotImplementedException(connectionLayout.ToString());
			}
		}
	}
}
