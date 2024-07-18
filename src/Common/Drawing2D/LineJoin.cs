namespace GeneXus.Drawing.Drawing2D;

/// <summary>
///  Specifies how to join consecutive line or curve segments in a figure (subpath) contained in a <see cref='Drawing.GraphicsPath'/> object.
/// </summary>
public enum LineJoin
{
	/// <summary>
	///  Specifies a mitered join. This produces a sharp corner or a clipped corner, depending on whether the 
	///  length of the miter exceeds the miter limit.
	/// </summary>
	Miter = 0,

	/// <summary>
	///  Specifies a beveled join. This produces a diagonal corner.
	/// </summary>
	Bevel = 1,

	/// <summary>
	///  pecifies a circular join. This produces a smooth, circular arc between the lines.
	/// </summary>
	Round = 2,

	/// <summary>
	///  Specifies a mitered join. This produces a sharp corner or a beveled corner, depending on whether the 
	///  length of the miter exceeds the miter limit.
	/// </summary>
	MiterClipped = 3
}
