namespace GeneXus.Drawing;

/// <summary>
///  Specifies the format of the image.
/// </summary>
public enum ImageFormat
{
	/// <summary>
	///  The BMP image format.
	/// </summary>
	Bmp,

	/// <summary>
	///  The GIF image format.
	/// </summary>
	Gif,

	/// <summary>
	///  The ICO image format.
	/// </summary>
	Ico,

	/// <summary>
	///  The JPEG image format.
	/// </summary>
	Jpeg,

	/// <summary>
	///  The PNG image format.
	/// </summary>
	Png,

	/// <summary>
	///  The WBMP image format.
	/// </summary>
	Wbmp,

	/// <summary>
	///  The WEBP image format.
	/// </summary>
	Webp,

	/// <summary>
	///  The PKM image format.
	/// </summary>
	Pkm,

	/// <summary>
	///  The KTX image format.
	/// </summary>
	Ktx,

	/// <summary>
	///  The ASTC image format.
	/// </summary>
	Astc,

	/// <summary>
	///  The DNG image format.
	/// </summary>
	Dng,

	/// <summary>
	///  The HEIG image format.
	/// </summary>
	Heif,

	/// <summary>
	///  The SVG image format.
	/// </summary>
	Svg
}

/* 
 * NOTE:
 * 1) Not supported by System.Drawing but supported by SkiaSharp: Astc, Dng, Ktx, Pkm, Wbpm
 * 2) Not supported by SkiaSharp but supported by System.Drawing: Tiff, Exif, Wmf, Svg (extra lib)
 */
