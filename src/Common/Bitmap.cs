using System;
using System.IO;
using System.Linq;
using GeneXus.Drawing.Imaging;
using SkiaSharp;

namespace GeneXus.Drawing;

[Serializable]
public sealed class Bitmap : Image, IDisposable, ICloneable
{
	internal readonly SKBitmap m_bitmap;

	internal Bitmap(SKBitmap bitmap, ImageFormat format, int frames = 1) : base(format, frames, bitmap.Info.Size)
	{
		m_bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));

		Width = m_bitmap.Width;
		Height = m_bitmap.Height;

		Flags = GetFlags(m_bitmap);
		PixelFormat = GetPixelFormat(m_bitmap.ColorType, m_bitmap.AlphaType, m_bitmap.BytesPerPixel, m_bitmap.Pixels);
	}

	private Bitmap(Bitmap other)
		: this(other.m_bitmap, other.m_format, other.m_frames) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class from a filename
	/// </summary>
	public Bitmap(string filename, bool useIcm = true)
		: this(FromFile(filename)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class from a file stream
	/// </summary>
	public Bitmap(Stream stream, bool useIcm = true)
		: this(FromStream(stream)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> from a specified resource
	/// </summary>
	public Bitmap(Type type, string resource)
		: this(GetResourceStream(type, resource)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class with the specified width and height
	/// </summary>
	public Bitmap(float width, float height)
		: this(new SKBitmap((int)width, (int)height), ImageFormat.Png, 1) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class with the specified size 
	/// and with the resolution of the specified <see cref='Graphics'/> object.
	/// </summary>
	public Bitmap(int width, int height, Graphics g)
		: this(width, height)
	{
		HorizontalResolution = g.DpiX;
		VerticalResolution = g.DpiY;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class with the specified size and format.
	/// </summary>
	public Bitmap(int width, int height, PixelFormat format)
		: this(width, height, 4, format, IntPtr.Zero) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class with the specified size, pixel format, and pixel data
	/// </summary>
	public Bitmap(int width, int height, int stride, PixelFormat format, IntPtr scan0)
		: this(width, height) => throw new NotImplementedException();

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class with the specified <see cref='Size'/>
	/// </summary>
	public Bitmap(Size size)
		: this(size.Width, size.Height) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class with the specified <see cref='Image'/>
	/// </summary>
	public Bitmap(Image original)
		: this(original, original.Width, original.Height) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class with the specified <see cref='Image'/>, width and height
	/// </summary>
	public Bitmap(Image original, float width, float height)
		: this(Resize(original, width, height)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class with the specified <see cref='Image'/> and <see cref='Size'/>
	/// </summary>
	public Bitmap(Image original, Size size)
		: this(original, size.Width, size.Height) { }

	/// <summary>
	/// Creates a human-readable string that represents this <see cref='Bitmap'/>.
	/// </summary>
	public override string ToString() => $"{GetType().Name}: {Width}x{Height}";


	#region IDisposable

	/// <summary>
	///  Cleans up resources for this <see cref='Bitmap'/>.
	/// </summary>
	protected override void Dispose(bool disposing) => m_bitmap.Dispose();

	#endregion


	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='Bitmap'/>.
	/// </summary>
	public override object Clone() => new Bitmap(m_bitmap.Copy(), m_format, m_frames);

	/// <summary>
	/// Creates a copy of the section of this <see cref='Bitmap'/> defined 
	/// by <see cref='Rectangle'/> structure and with a specified PixelFormat enumeration.
	/// </summary>
	public object Clone(RectangleF rect, PixelFormat format)
	{
		var bitmap = new Bitmap(rect.Width, rect.Height);
		var portion = SKRectI.Truncate(rect.m_rect);
		return m_bitmap.ExtractSubset(bitmap.m_bitmap, portion) ? bitmap : Clone();
	}

	/// <summary>
	/// Creates a copy of the section of this <see cref='Bitmap'/> defined 
	/// by <see cref='Rectangle'/> structure and with a specified PixelFormat enumeration.
	/// </summary>
	public object Clone(Rectangle rect, PixelFormat format)
		=> Clone(new RectangleF(rect.m_rect), format);

	#endregion


	#region Operators

	/// <summary>
	/// Creates a <see cref='SKBitmap'/> with the coordinates of the specified <see cref='Bitmap'/>.
	/// </summary>
	public static explicit operator SKBitmap(Bitmap bitmap) => bitmap.m_bitmap;

	#endregion


	#region Properties

	/// <summary>
	///  Gets the bytes-per-pixel value of this <see cref='Bitmap'/>.
	/// </summary>
	public int BytesPerPixel => m_bitmap.BytesPerPixel;

	/// <summary>
	///  Gets the handle of this <see cref='Bitmap'/>.
	/// </summary>
	public IntPtr Handle => m_bitmap.Handle;

	#endregion


	#region Factory

	/// <summary>
	///  Creates a <see cref='Bitmap'/> from a Windows handle to an icon.
	/// </summary>
	public static Bitmap FromHicon(IntPtr hicon)
		=> throw new NotSupportedException("windows specific");

	/// <summary>
	///  Creates a <see cref='Bitmap'/> from the specified Windows resource.
	/// </summary>
	public static Bitmap FromResource(IntPtr hinstance, string bitmapName)
		=> throw new NotSupportedException("windows specific");

	#endregion


	#region Methods

	/// <summary>
	///  Creates a GDI bitmap object from this <see cref='Bitmap'/>.
	/// </summary>
	public IntPtr GetHbitmap()
		=> GetHbitmap(Color.LightGray);

	/// <summary>
	///  Creates a GDI bitmap object from this <see cref='Bitmap'/> with the 
	///  specificed <see cref='Bitmap'/> structure as background.
	/// </summary>
	public IntPtr GetHbitmap(Color background)
		=> throw new NotSupportedException("windows specific");

	/// <summary>
	///  Returns the handle to an icon.
	/// </summary>
	public IntPtr GetHicon()
		=> throw new NotSupportedException("windows specific");

	/// <summary>
	///  Gets the color of the specified pixel in this <see cref='Bitmap'/>.
	/// </summary>
	public Color GetPixel(int x, int y)
	{
		var color = m_bitmap.GetPixel(x, y);
		return new Color(color.Alpha, color.Red, color.Green, color.Blue);
	}

	/// <summary>
	///  Locks a <see cref='Bitmap'/> into system memory.
	/// </summary>
	public object LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format)
		=> LockBits(rect, flags, format, new()); // TODO: implement BitmapData

	/// <summary>
	///  Locks a <see cref='Bitmap'/> into system memory using a BitmapData information.
	/// </summary>
	public object LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format, object bitmapData)
		=> throw new NotImplementedException(); // TODO: implement BitmapData

	/// <summary>
	///  Makes the default transparent color transparent for this <see cref='Bitmap'/>.
	/// </summary>
	public void MakeTransparent()
		=> MakeTransparent(Width > 0 && Height > 0 ? GetPixel(0, Height - 1) : Color.LightGray);

	/// <summary>
	///  Makes the specified color transparent for this <see cref='Bitmap'/>.
	/// </summary>
	public void MakeTransparent(Color transparentColor)
	{
		if (transparentColor.A < 255)
			return; // already transparent, return the bitmap as is

		for (int x = 0; x < Width; x++)
			for (int y = 0; y < Height; y++)
				if (GetPixel(x, y) == transparentColor)
					SetPixel(x, y, Color.Transparent);
	}

	/// <inheritdoc cref="Image.RotateFlip(int, float, float)" />
	protected override void RotateFlip(int degrees, float scaleX, float scaleY)
	{
		float centerX = Width / 2f;
		float centerY = Height / 2f;

		using var surface = SKSurface.Create(m_bitmap.Info);
		surface.Canvas.RotateDegrees(degrees, centerX, centerY);
		surface.Canvas.Scale(scaleX, scaleY, centerX, centerY);
		surface.Canvas.DrawBitmap(m_bitmap, 0, 0);

		var bitmap = SKBitmap.FromImage(surface.Snapshot());
		m_bitmap.SetPixels(bitmap.GetPixels());
	}

	/// <summary>
	///  Sets the color of the specified pixel in this <see cref='Bitmap'/>.
	/// </summary>
	public void SetPixel(int x, int y, Color color)
	{
		var c = new SKColor((byte)color.R, (byte)color.G, (byte)color.B, (byte)color.A);
		m_bitmap.SetPixel(x, y, c);
	}

	/// <summary>
	///  Sets the resolution for this <see cref='Bitmap'/>.
	/// </summary>
	public void SetResolution(float xDpi, float yDpi)
	{
		if (xDpi <= 0 || yDpi <= 0) 
			throw new ArgumentOutOfRangeException("DPI values must be greater than zero.");
		throw new NotSupportedException("not supported by skia"); // SOURCE: https://issues.skia.org/issues/40043604
	}

	/// <summary>
	///  Unlocks this <see cref='Bitmap'/> from system memory.
	/// </summary>
	public void UnlockBits(object bitmapdata)
		=> throw new NotImplementedException(); // TODO: implement BitmapData

	#endregion


	#region Utilities

	internal override SKImage InnerImage => SKImage.FromBitmap(m_bitmap);

	private static Bitmap Resize(Image original, float width, float height)
	{
		if (original.InnerImage is not SKImage image)
			throw new ArgumentException($"not image.", nameof(original));
		var info = new SKImageInfo((int)width, (int)height, image.ColorType, image.AlphaType, image.ColorSpace);
		var bitmap = SKBitmap.FromImage(image).Resize(info, SKFilterQuality.High);
		return new Bitmap(bitmap, original.m_format, original.m_frames);
	}

	private int GetFlags(SKBitmap bitmap)
	{
		var flags = ImageFlags.None;
		
		if (bitmap.IsImmutable)
			flags |= ImageFlags.ReadOnly;

		if (bitmap.Pixels.Any(pixel => pixel.Alpha < 255))
			flags |= ImageFlags.HasAlpha;

		if (bitmap.Pixels.Any(pixel => pixel.Alpha > 0 && pixel.Alpha < 255))
			flags |= ImageFlags.HasTranslucent;

		if (bitmap.Width > 0 && m_bitmap.Height > 0)
			flags |= ImageFlags.HasRealPixelSize;

		switch (bitmap.ColorType)
		{
			case SKColorType.Rgb565:
			case SKColorType.Rgb888x:
			case SKColorType.Rgba8888:
			case SKColorType.Bgra8888:
				flags |= ImageFlags.ColorSpaceRgb;
				break;

			case SKColorType.Gray8:
				flags |= ImageFlags.ColorSpaceGray;
				break;
		}

		/* 
		 * TODO: Missing flags
		 * - ImageFlags.Caching
		 * - ImageFlags.ColorSpaceCmyk
		 * - ImageFlags.ColorSpaceYcbcr
		 * - ImageFlags.ColorSpaceYcck
		 * - ImageFlags.HasRealDpi
		 * - ImageFlags.PartiallyScalable
		 * - ImageFlags.Scalable
		 */

		return (int)flags;
	}

	private static PixelFormat GetPixelFormat(SKColorType colorType, SKAlphaType alphaType, int bytesPerPixel, SKColor[] pixels)
	{
		if (alphaType == SKAlphaType.Opaque && pixels.All(pixel => pixel.Red == pixel.Green && pixel.Green == pixel.Blue))
			return PixelFormat.Format8bppIndexed; // NOTE: to behave as System.Drawing but SkiaSharp seems to treat it differently

		switch (colorType)
		{
			case SKColorType.Rgb565 when alphaType == SKAlphaType.Opaque:
				return bytesPerPixel == 2 ? PixelFormat.Format16bppRgb565
					 : PixelFormat.Undefined;

			case SKColorType.Rgb888x when alphaType == SKAlphaType.Unpremul:
				return bytesPerPixel == 3 ? PixelFormat.Format24bppRgb
					 : bytesPerPixel == 4 ? PixelFormat.Format32bppRgb
					 : bytesPerPixel == 6 ? PixelFormat.Format48bppRgb
					 : PixelFormat.Undefined;

			case SKColorType.Rgba8888 when alphaType == SKAlphaType.Opaque:
			case SKColorType.Bgra8888 when alphaType == SKAlphaType.Opaque:
				return bytesPerPixel == 4 ? PixelFormat.Format24bppRgb
					 : bytesPerPixel == 6 ? PixelFormat.Format32bppRgb
					 : bytesPerPixel == 8 ? PixelFormat.Format48bppRgb
					 : PixelFormat.Undefined;

			case SKColorType.Rgba8888 when alphaType == SKAlphaType.Unpremul:
			case SKColorType.Bgra8888 when alphaType == SKAlphaType.Unpremul:
				return bytesPerPixel == 4 ? PixelFormat.Format32bppArgb
					 : bytesPerPixel == 8 ? PixelFormat.Format64bppArgb
					 : PixelFormat.Undefined;

			case SKColorType.Rgba8888 when alphaType == SKAlphaType.Premul:
			case SKColorType.Bgra8888 when alphaType == SKAlphaType.Premul:
				return bytesPerPixel == 4 ? PixelFormat.Format32bppPArgb
					 : bytesPerPixel == 8 ? PixelFormat.Format64bppPArgb
					 : PixelFormat.Undefined;

			case SKColorType.Gray8:
				return bytesPerPixel == 2 ? PixelFormat.Format16bppGrayScale
					 : PixelFormat.Undefined;

			default:
				/*
				 * TODO: missing value mappings
				 * - Format1bppIndexed
				 * - Format4bppIndexed
				 * - Format8bppIndexed
				 * - Format16bppRgb555
				 * - Format16bppArgb1555
				 */
				return PixelFormat.Undefined;
		}
	}

	#endregion
}
