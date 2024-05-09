using SkiaSharp;
using System;
using System.IO;

namespace GeneXus.Drawing.Common;

[Serializable]
public class Bitmap : IDisposable, ICloneable
{
	internal readonly SKBitmap m_bitmap;

	internal Bitmap(SKBitmap skBitmap)
	{
		m_bitmap = skBitmap;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class from a filename
	/// </summary>
	public Bitmap(string filename)
		: this(Image.FromFile(filename)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class from a file stream
	/// </summary>
	public Bitmap(Stream stream)
		: this(Image.FromStream(stream)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class with the specified width and height
	/// </summary>
	public Bitmap(float width, float height)
		: this(new SKBitmap((int)width, (int)height)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class with the specified <see cref='Size'/>
	/// </summary>
	public Bitmap(Size size)
		: this(size.Width, size.Height) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class with the specified <see cref='Image'/>
	/// </summary>
	public Bitmap(Image original)
		: this(SKBitmap.FromImage(original.m_image))
	{
		if (original.m_index == 0 && original.m_frames == 1) return;

		using var stream = new MemoryStream();
		original.Save(stream, ImageFormat.Png);
		stream.Seek(0, SeekOrigin.Begin);

		var bitmap = m_bitmap;

		var image = Image.FromStream(stream);
		m_bitmap = SKBitmap.FromImage(image.m_image);

		bitmap.Dispose();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref='Bitmap'/> class with the specified <see cref='Image'/>, width and height
	/// </summary>
	public Bitmap(Image original, float width, float height)
		: this(original)
	{
		var resizeInfo = new SKImageInfo(Convert.ToInt32(width), Convert.ToInt32(height));
		m_bitmap = m_bitmap.Resize(resizeInfo, SKFilterQuality.High);
	}

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


	#region IDisposable

	/// <summary>
	///  Cleans up resources for this <see cref='Bitmap'/>.
	/// </summary>
	public void Dispose() => m_bitmap.Dispose();

	#endregion


	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='Bitmap'/>.
	/// </summary>
	public object Clone() => new Bitmap(m_bitmap.Copy());

	#endregion


	#region Operators

	/// <summary>
	/// Creates a <see cref='SKBitmap'/> with the coordinates of the specified <see cref='Bitmap'/> .
	/// </summary>
	public static explicit operator SKBitmap(Bitmap bitmap) => bitmap.m_bitmap;

	#endregion


	#region Properties

	/// <summary>
	///  Gets the width of this <see cref='Bitmap'/>.
	/// </summary>
	public int Width => m_bitmap.Width;

	/// <summary>
	///  Gets the height of this <see cref='Bitmap'/>.
	/// </summary>
	public int Height => m_bitmap.Height;

	/// <summary>
	///  Gets the bytes-per-pixel value of this <see cref='Bitmap'/>.
	/// </summary>
	public int BytesPerPixel => m_bitmap.BytesPerPixel;

	#endregion


	#region Methods

	/// <summary>
	///  Gets the color of the specified pixel in this <see cref='Bitmap'/>.
	/// </summary>
	public Color GetPixel(int x, int y)
	{
		var color = m_bitmap.GetPixel(x, y);
		return new Color(color.Alpha, color.Red, color.Green, color.Blue);
	}

	/// <summary>
	///  Sets the color of the specified pixel in this <see cref='Bitmap'/>.
	/// </summary>
	public void SetPixel(int x, int y, Color color)
	{
		var c = new SKColor((byte)color.R, (byte)color.G, (byte)color.B, (byte)color.A);
		m_bitmap.SetPixel(x, y, c);
	}

	#endregion


	#region Utilities

	/// <summary>
	///  Saves this <see cref='Bitmap'/> into a stream with a specified format.
	/// </summary>
	internal void Save(Stream stream, SKEncodedImageFormat format, int quality = 100)
	{
		using var skStream = new SKManagedWStream(stream);
		var isOk = m_bitmap.Encode(skStream, format, quality);
		if (isOk) return;
		throw new Exception("failed to save bitmap");
	}

	#endregion
}
