using System;

namespace Netron.Lithium
{
		/// <summary>
	/// to pass shape data to the outside world
	/// </summary>
	public delegate void ShapeData(ShapeBase shape);

	/// <summary>
	/// General purpose delegate to pass info to the outside world
	/// </summary>
	public delegate void Messager(string message);

	public delegate void ItemHover(ShapeBase hoveredShape);
}
