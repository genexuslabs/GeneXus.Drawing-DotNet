namespace GeneXus.Drawing.Drawing2D;

/// <summary>
///  Specifies the alignment of a <see cref='Pen'/> object in relation to the theoretical, zero-width line.
/// </summary>
public enum PenAlignment
{
	/// <summary>
	///  Specifies the <see cref='Pen'/> object is centered over the theoretical line.
	/// </summary>
	Center = 0,

	/// <summary>
	///  Specifies the <see cref='Pen'/> is positioned on the inside of the theoretical line.
	/// </summary>
	Inset = 1,

	/// <summary>
	///  Specifies the <see cref='Pen'/> is positioned on the outside of the theoretical line.
	/// </summary>
	Outset = 2,

	/// <summary>
	///  Specifies the <see cref='Pen'/> is positioned on the left of the theoretical line.
	/// </summary>
	Left = 3,

	/// <summary>
	///  Specifies the <see cref='Pen'/> is positioned on the right of the theoretical line.
	/// </summary>
	Right = 4
}
