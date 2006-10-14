using System;

namespace Netron.Lithium
{
	

	/// <summary>
	/// The types of connections in this assembly
	/// </summary>
	public enum ConnectionLayout
	{
		/// <summary>
		/// The default connection simply connects the centers of the shapes
		/// </summary>
		Default,
		/// <summary>
		/// the traditional connection is a rectangular connections which mimics the traditional
		/// layout of hierarchies and flowcharts
		/// </summary>
		Traditional,
		/// <summary>
		/// a smoothly curved form connecting the shapes
		/// </summary>
		Bezier
	}

}
