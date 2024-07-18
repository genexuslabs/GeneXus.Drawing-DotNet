namespace GeneXus.Drawing.Drawing2D;

/// <summary>
///  Specifies how the source colors are combined with the background colors.
/// </summary>
public enum CompositingQuality
{
	/// <summary>
	///  Invalid quality.
	/// </summary>
	Invalid = -1,

	/// <summary>
	///  Default quality.
	/// </summary>
	Default = 0,

	/// <summary>
	///  High speed, low quality.
	/// </summary>
	HighSpeed = 1,

	/// <summary>
	///  High quality, low speed compositing.
	/// </summary>
	HighQuality = 2,

	/// <summary>
	///  Gamma correction is used.
	/// </summary>
	GammaCorrected = 3,

	/// <summary>
	///  Assume linear values.
	/// </summary>
	AssumeLinear = 4
}
