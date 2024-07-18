namespace GeneXus.Drawing.Drawing2D;

/// <summary>
///  Specifies how pixels are offset during rendering.
/// </summary>
public enum PixelOffsetMode
{
	/// <summary>
	///  Specifies an invalid mode.
	/// </summary>
	Invalid = -1,

	/// <summary>
	///  Specifies the default mode.
	/// </summary>
	Default = 0,

	/// <summary>
	///  Specifies high speed, low quality rendering.
	/// </summary>
	HighSpeed = 1,

	/// <summary>
	///  Specifies high quality, low speed rendering.
	/// </summary>
	HighQuality = 2,

	/// <summary>
	///  Specifies no pixel offset.
	/// </summary>
	None = 3,

	/// <summary>
	///  Specifies that pixels are offset by -0.5 units, both horizontally 
	///  and vertically, for high speed antialiasing.
	/// </summary>
	Half = 4
}
