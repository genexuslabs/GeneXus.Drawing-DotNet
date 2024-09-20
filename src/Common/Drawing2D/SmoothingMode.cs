namespace GeneXus.Drawing.Drawing2D;

/// <summary>
///  Specifies whether smoothing (antialiasing) is applied to lines and curves and the edges of filled areas.
/// </summary>
public enum SmoothingMode
{
	/// <summary>
	///  Specifies an invalid mode.
	/// </summary>
	Invalid = -1,

	/// <summary>
	///  Specifies no antialiasing.
	/// </summary>
	Default = 0,

	/// <summary>
	///  Specifies no antialiasing.
	/// </summary>
	HighSpeed = 1,

	/// <summary>
	///  Specifies antialiased rendering.
	/// </summary>
	HighQuality = 2,

	/// <summary>
	///  Specifies no antialiasing.
	/// </summary>
	None = 3,

	/// <summary>
	///  Specifies antialiased rendering.
	/// </summary>
	AntiAlias = 4
}
