using System;

namespace GeneXus.Drawing.Imaging;

[Flags]
/// <summary>
///  Specifies the attributes of the pixel data contained in an <see cref='Image'/> object.
/// </summary>
public enum ImageFlags
{
	/// <summary>
	/// There is no format information.
	/// </summary>
	None = 0,

	/// <summary>
	/// Pixel data is scalable.
	/// </summary>
	Scalable = 0x0001,

	/// <summary>
	/// Pixel data contains alpha information.
	/// </summary>
	HasAlpha = 0x0002,

	/// <summary>
	/// Pixel data has alpha values other than 0 (transparent) and 255 (opaque).
	/// </summary>
	HasTranslucent = 0x0004,

	/// <summary>
	/// Pixel data is partially scalable, but there are some limitations.
	/// </summary>
	PartiallyScalable = 0x0008,

	/// <summary>
	/// Pixel data uses an RGB color space.
	/// </summary>
	ColorSpaceRgb = 0x0010,

	/// <summary>
	/// Pixel data uses a CMYK color space.
	/// </summary>
	ColorSpaceCmyk = 0x0020,

	/// <summary>
	/// Pixel data is grayscale.
	/// </summary>
	ColorSpaceGray = 0x0040,

	/// <summary>
	/// Pixel data is stored using a YCBCR color space.
	/// </summary>
	ColorSpaceYcbcr = 0x0080,

	/// <summary>
	/// Pixel data is stored using a YCCK color space.
	/// </summary>
	ColorSpaceYcck = 0x0100,

	/// <summary>
	/// Specifies that dots per inch information is stored in the image.
	/// </summary>
	HasRealDpi = 0x1000,

	/// <summary>
	/// Specifies that the pixel size is stored in the image.
	/// </summary>
	HasRealPixelSize = 0x2000,

	/// <summary>
	/// The pixel data is read-only.
	/// </summary>
	ReadOnly = 0x00010000,

	/// <summary>
	/// The pixel data can be cached for faster access.
	/// </summary>
	Caching = 0x00020000
}
