namespace GeneXus.Drawing.Drawing2D;

/// <summary>
///  Specifies the type of fill a <see cref='Pen'/> object uses to fill lines.
/// </summary>
public enum PenType
{
	/// <summary>
	///  Specifies a solid fill.
	/// </summary>
	SolidColor = 0,

	/// <summary>
	///  Specifies a hatch fill.
	/// </summary>
	HatchFill = 1,

	/// <summary>
	///  Specifies a bitmap texture fill.
	/// </summary>
	TextureFill = 2,

	/// <summary>
	///  Specifies a path gradient fill.
	/// </summary>
	PathGradient = 3,

	/// <summary>
	///  Specifies a linear gradient fill.
	/// </summary>
	LinearGradient = 4
}
