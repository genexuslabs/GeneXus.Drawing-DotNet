namespace GeneXus.Drawing.Drawing2D;

/// <summary>
///  Specifies the algorithm that is used when images are scaled or rotated.
/// </summary>
public enum InterpolationMode
{
	/// <summary>
	///  Specifies an invalid mode.
	/// </summary>
	Invalid = -1,

	/// <summary>
	///  Specifies default mode.
	/// </summary>
	Default = 0,

	/// <summary>
	///  Specifies low quality interpolation.
	/// </summary>
	Low = 1,

	/// <summary>
	///  Specifies high quality interpolation.
	/// </summary>
	High = 2,

	/// <summary>
	///  Specifies bilinear interpolation. No prefiltering is done. This mode is not 
	///  suitable for shrinking an image below 50 percent of its original size.
	/// </summary>
	Bilinear = 3,

	/// <summary>
	///  Specifies bicubic interpolation. No prefiltering is done. This mode is not 
	///  suitable for shrinking an image below 25 percent of its original size.
	/// </summary>
	Bicubic = 4,

	/// <summary>
	///  Specifies nearest-neighbor interpolation.
	/// </summary>
	NearestNeighbor = 5,

	/// <summary>
	///  Specifies high-quality, bilinear interpolation. Prefiltering is performed to 
	///  ensure high-quality shrinking.
	/// </summary>
	HighQualityBilinear = 6,

	/// <summary>
	///  Specifies high-quality, bicubic interpolation. Prefiltering is performed to 
	///  ensure high-quality shrinking. This mode produces the highest quality transformed images.
	/// </summary>
	HighQualityBicubic = 7
}
