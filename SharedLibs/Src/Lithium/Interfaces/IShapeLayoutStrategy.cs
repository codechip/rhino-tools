using System;
using System.Drawing;

namespace Netron.Lithium.Interfaces
{
	/// <summary>
	/// Represent a strategy to layout the tree
	/// </summary>
	public interface IShapeLayoutStrategy
	{
		/// <summary>
		/// the space between the nodes
		/// </summary>
		int WordSpacing { get; set; }

		/// <summary>
		/// the height between branches
		/// </summary>
		int BranchHeight { get; set; }

		/// <summary>
		/// Draws the tree starting from the root shape.
		/// </summary>
		/// <param name="root">the root to draw from</param>
		/// <param name="shiftLeft">the current shift</param>
		/// <param name="shiftTop">the current shift</param>
		/// <returns></returns>
		int DrawTree(ShapeBase root, int shiftLeft, int shiftTop, ref Size size);

		/// <summary>
		/// Get rectangle for this shape's [+/-] sign
		/// </summary>
		Rectangle GetToggleRectangle(ShapeBase shape);

		/// <summary>
		/// The default root position
		/// </summary>
		Point GetDefaultRootPosition(LithiumControl site, ShapeBase root);

		/// <summary>
		/// Create an appropriate connection layout of this type for this shape layout.
		/// </summary>
		IConnectionLayoutStrategy CreateConnectionLayoutStrategy(ConnectionLayout connectionLayout);
	}
}
