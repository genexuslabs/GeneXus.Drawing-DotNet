namespace GeneXus.Drawing.Drawing2D;

/// <summary>
///  Specifies the type of point in a <see cref='Drawing.GraphicsPath'/> object.
/// </summary>
public enum PathPointType : byte
{
	/// <summary>
	///  Indicates that the point is the start of a figure.
	/// </summary>
	Start = 0x00,

	/// <summary>
	///  Indicates that the point is an endpoint of a line.
	/// </summary>
	Line = 0x01,

	/// <summary>
	///  Indicates that the point is an endpoint or a control point of a cubic Bézier spline.
	/// </summary>
	Bezier = 0x03,

	/// <summary>
	///  Masks all bits except for the three low-order bits, which indicate the point type.
	/// </summary>
	PathTypeMask = 0x07,

	/// <summary>
	///  Not used.
	/// </summary>
	DashMode = 0x10,

	/// <summary>
	///  Specifies that the point is a marker.
	/// </summary>
	PathMarker = 0x20,

	/// <summary>
	///  Specifies that the point is the last point in a closed subpath (figure).
	/// </summary>
	CloseSubpath = 0x80,

	/// <summary>
	///  A cubic Bézier curve.
	/// </summary>
	Bezier3 = Bezier
}
