using System;
using System.IO;
using GeneXus.Drawing.Imaging;
using SkiaSharp;

namespace GeneXus.Drawing;

[Serializable]
public class Bitmap : Image, IDisposable, ICloneable
{
	internal Bitmap(SKBitmap skBitmap)
		: base(skBitmap, ImageFormat.Png, 1) { }

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
	/// Initializes a new instance of the <see cref='Bitmap'/> class from a file stream
	/// </summary>
	public Bitmap(Type type, string resource)
		: this(GetResourceStream(type, resource)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class with the specified width and height
	/// </summary>
	public Bitmap(float width, float height)
		: this(new SKBitmap((int)width, (int)height)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class with the specified size 
	/// and with the resolution of the specified <see cref='Graphics'/> object.
	/// </summary>
	//public Bitmap(int width, int height, object g) // TODO: Implement Graphics
	//	: this(width, height) => throw new NotImplementedException();

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class with the specified size and format.
	/// </summary>
	public Bitmap(int width, int height, object format)  // TODO: Implement PixelFormat
		: this(width, height, 4, format, IntPtr.Zero) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class with the specified size, pixel format, and pixel data
	/// </summary>
	public Bitmap(int width, int height, int stride, object format, IntPtr scan0) // TODO: Implement PixelFormat
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
	: this(original.m_bitmap.Resize(new SKImageInfo((int)width, (int)height), SKFilterQuality.High)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class with the specified <see cref='Image'/> and <see cref='Size'/>
	/// </summary>
	public Bitmap(Image original, Size size)
		: this(original, size.Width, size.Height) { }

	/// <summary>
	///  Cleans up resources for this <see cref='Bitmap'/>.
	/// </summary>
	~Bitmap() => Dispose();

	/// <summary>
	/// Creates a human-readable string that represents this <see cref='Bitmap'/>.
	/// </summary>
	public override string ToString() => $"{GetType().Name}: {Width}x{Height}";


	#region IClonable

	/// <summary>
	/// Creates a copy of the section of this <see cref='Bitmap'/> defined 
	/// by <see cref='Rectangle'/> structure and with a specified PixelFormat enumeration.
	/// </summary>
	public object Clone(Rectangle rect, object format) // TODO: Implement PixelFormat
	{
		var bitmap = new Bitmap(rect.Width, rect.Height);
		var portion = new SKRectI((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom);
		return m_bitmap.ExtractSubset(bitmap.m_bitmap, portion) ? bitmap : base.Clone();
	}

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
	public object LockBits(Rectangle rect, object flags, object format) // TODO: Implement ImageLockMode and PixelFormat
		=> LockBits(rect, flags, format, new());

	/// <summary>
	///  Locks a <see cref='Bitmap'/> into system memory using a BitmapData information.
	/// </summary>
	public object LockBits(Rectangle rect, object flags, object format, object bitmapData)
		=> throw new NotImplementedException();

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
					SetPixel(x, y, Color.Translarent);
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
		HorizontalResolution = xDpi; // TODO: this is not doing anything
		VerticalResolution = yDpi;

		using var surface = SKSurface.Create(new SKImageInfo(Width, Height));
		float dpiX = 100f * surface.Canvas.DeviceClipBounds.Width / surface.Canvas.LocalClipBounds.Width;
		float dpiY = 100f * surface.Canvas.DeviceClipBounds.Height / surface.Canvas.LocalClipBounds.Height;

		int width = (int)(Width * xDpi / dpiX);
		int height = (int)(Height * yDpi / dpiY);

		using var resize = m_bitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.High);
		if (m_bitmap.ScalePixels(resize, SKFilterQuality.High))
			return;
		throw new Exception("could not set resolution");
	}

	/// <summary>
	///  Unlocks this <see cref='Bitmap'/> from system memory.
	/// </summary>
	public void UnlockBits(object bitmapdata)
		=> throw new NotImplementedException(); // TODO: implement BitmapData

	#endregion


	#region Utilities

	private static Stream GetResourceStream(Type type, string resource)
	{
		if (type == null)
			throw new ArgumentNullException(nameof(type));
		if (resource == null)
			throw new ArgumentNullException(nameof(resource));
		return type.Module.Assembly.GetManifestResourceStream(type, resource)
			?? throw new ArgumentException("resource not found", nameof(resource));
	}

	#endregion
}
