using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GeneXus.Drawing.Imaging;
using SkiaSharp;

namespace GeneXus.Drawing;

[Serializable]
public class Image : IDisposable, ICloneable
{
	internal readonly SKBitmap m_bitmap;
	internal readonly ImageFormat m_format;
	internal readonly int m_frames;

	internal Image(SKBitmap bitmap, ImageFormat format, int frames)
	{
		m_bitmap = bitmap;
		m_frames = frames;
		m_format = format;
	}

	/// <summary>
	///  Cleans up resources for this <see cref='Image'/>.
	/// </summary>
	~Image() => Dispose();

	/// <summary>
	/// Creates a human-readable string that represents this <see cref='Image'/>.
	/// </summary>
	public override string ToString() => $"{GetType().Name}: {RawFormat} {Width}x{Height}";


	#region IDisposable

	/// <summary>
	///  Cleans up resources for this <see cref='Image'/>.
	/// </summary>
	public virtual void Dispose() => m_bitmap.Dispose();

	#endregion


	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='Image'/>.
	/// </summary>
	public virtual object Clone() => new Bitmap(m_bitmap.Copy());

	#endregion


	#region Operators

	/// <summary>
	/// Creates a <see cref='SKImage'/> with the coordinates of the specified <see cref='Image'/> .
	/// </summary>
	public static explicit operator SKImage(Image image) => SKImage.FromBitmap(image.m_bitmap);

	#endregion


	#region Properties

	/// <summary>
	///  Gets attribute flags for the pixel data of this <see cref='Image'/>.
	/// </summary>
	public int Flags => throw new NotImplementedException();

	/// <summary>
	///  Gets an array of GUIDs that represent the dimensions of frames within this <see cref='Image'/>.
	/// </summary>
	public Guid[] FrameDimensionsList => throw new NotImplementedException();

	/// <summary>
	///  Gets the height of this <see cref='Image'/>.
	/// </summary>
	public int Height => m_bitmap.Height;

	/// <summary>
	///  Gets the horizontal resolution, in pixels-per-inch, of this <see cref='Image'/>.
	/// </summary>
	public float HorizontalResolution { get; protected set; }

	/// <summary>
	///  Gets or sets the color palette used for this <see cref='Image'/>.
	/// </summary>
	public object Palette // TODO: implement ColorPalette
	{
		get => throw new NotImplementedException();
		set => throw new NotImplementedException();
	}

	/// <summary>
	///  Gets the width and height of this <see cref='Image'/>.
	/// </summary>
	public Size PhysicalDimension => throw new NotImplementedException();

	/// <summary>
	///  Gets the <see cref='Drawing.PixelFormat'/> for this <see cref='Image'/>.
	/// </summary>
	public object PixelFormat => throw new NotImplementedException();

	/// <summary>
	///  Gets IDs of the property items stored in this <see cref='Image'/>.
	/// </summary>
	public int[] PropertyIdList => throw new NotImplementedException();

	/// <summary>
	///  Gets all the property items (pieces of metadata) stored in this <see cref='Image'/>.
	/// </summary>
	public object[] PropertyItems => throw new NotImplementedException();

	/// <summary>
	///  Gets the format of this <see cref='Image'/>.
	/// </summary>
	public ImageFormat RawFormat => m_format;

	/// <summary>
	///  Gets the width and height of this <see cref='Image'/>.
	/// </summary>
	public Size Size => new(Width, Height);

	/// <summary>
	///  Gets or sets an object that provides additional data about the image.
	/// </summary>
	public object Tag { get; set; }

	/// <summary>
	///  Gets the vertical resolution, in pixels-per-inch, of this <see cref='Image'/>.
	/// </summary>
	public float VerticalResolution { get; protected set; }

	/// <summary>
	///  Gets the width of this <see cref='Image'/>.
	/// </summary>
	public int Width => m_bitmap.Width;

	#endregion


	#region Factory

	/// <summary>
	///  Creates an <see cref='Image'/> from the specified file.
	/// </summary>
	public static Image FromFile(string filename)
	{
		using var stream = File.OpenRead(filename);
		return FromStream(stream);
	}

	/// <summary>
	///  Creates a <see cref='Image'/> from a handle to a GDI bitmap.
	/// </summary>
	public static Bitmap FromHbitmap(IntPtr hbitmap)
		=> throw new NotSupportedException("windows specific");

	/// <summary>
	///  Creates an <see cref='Image'/> from the specified data stream.
	/// </summary>
	public static Image FromStream(Stream stream)
	{
		var data = SKData.Create(stream);
		if (IsSvg(data))
		{
			var svg = new SkiaSharp.Extended.Svg.SKSvg();
			svg.Load(data.AsStream());

			var bounds = svg.Picture.CullRect;
			var size = new SKSizeI((int)bounds.Width, (int)bounds.Height);
			var image = SKImage.FromPicture(svg.Picture, size);

			var bitmap = SKBitmap.FromImage(image);
			return new Image(bitmap, ImageFormat.Svg, 1);
		}
		else
		{
			var image = SKImage.FromEncodedData(data);

			var codec = SKCodec.Create(image.EncodedData);
			if (!SK2GX.TryGetValue(codec.EncodedFormat, out var format))
				throw new ArgumentException($"unsupported format {codec.EncodedFormat}");

			var bitmap = SKBitmap.FromImage(image);
			return new Image(bitmap, format, Math.Max(codec.FrameCount, 1));
		}
	}

	#endregion


	#region Methods

	/// <summary>
	///  Gets a bounding rectangle in the specified units for this <see cref='Image'/>.
	/// </summary>
	public Rectangle GetBounds(ref object pageUnit)
		=> throw new NotImplementedException(); // TODO: implement GraphicsUnit

	/// <summary>
	///  Returns information about the codecs used for this <see cref='Image'/>.
	/// </summary>
	public object GetEncoderParameterList(Guid encoder)
		=> throw new NotImplementedException(); // TODO: implement EncoderParameters

	/// <summary>
	///  Returns the number of frames of the given dimension.
	/// </summary>
	public int GetFrameCount()
		=> m_frames;

	/// <summary>
	///  Returns the size of the specified pixel format.
	/// </summary>
	public static int GetPixelFormatSize(object pixfmt)
		=> throw new NotImplementedException(); // TODO: implement PixelFormat

	/// <summary>
	///  Gets the specified property item from this <see cref='Image'/>.
	/// </summary>
	public object GetPropertyItem(int propid)
		=> throw new NotImplementedException(); // TODO: implement PropertyItem

	/// <summary>
	///  Returns the thumbnail for this <see cref='Image'/>.
	/// </summary>
	public Image GetThumbnailImage(int thumbWidth, int thumbHeight, object callback, IntPtr callbackData)
		=> throw new NotImplementedException();

	/// <summary>
	///  Returns a value indicating whether the pixel format contains alpha information.
	/// </summary>
	public static bool IsAlphaPixelFormat(object pixfmt)
		=> throw new NotImplementedException(); // TODO: implement PixelFormat

	/// <summary>
	///  Returns a value indicating whether the pixel format is canonical.
	/// </summary>
	public static bool IsCanonicalPixelFormat(object pixfmt)
		=> throw new NotImplementedException(); // TODO: implement PixelFormat

	/// <summary>
	///  Returns a value indicating whether the pixel format is extended.
	/// </summary>
	public static bool IsExtendedPixelFormat(object pixfmt)
		=> throw new NotImplementedException(); // TODO: implement PixelFormat

	/// <summary>
	///  Removes the specified property item from this <see cref='Image'/>.
	/// </summary>
	public void RemovePropertyItem(int propid)
		=> throw new NotImplementedException();

	/// <summary>
	///  Rotates, flips, or rotates and flips the <see cref='Image'/>.
	/// </summary>
	public void RotateFlip(object rotateFlipType)
		=> throw new NotImplementedException(); // TODO: implement RotateFlipType

	/// <summary>
	///  Saves this <see cref='Image'/> to the specified file.
	/// </summary>
	public void Save(string filename) => Save(filename, RawFormat);

	/// <summary>
	///  Saves this <see cref='Image'/> to the specified file in the specified format.
	/// </summary>
	public void Save(string filename, ImageFormat format, int quality = 100)
	{
		var file = new FileInfo(filename);

		string extension = file.Extension.TrimStart('.').Replace("jpg", "jpeg");
		if (!extension.Equals($"{format}", StringComparison.OrdinalIgnoreCase))
			throw new ArgumentException($"filename extension must be .{format.ToString().ToLower()}", nameof(filename));

		var stream = file.OpenWrite();
		Save(stream, format, quality);
		stream.Close();
	}

	// <summary>
	///  Saves this <see cref='Image'/> to the specified file with the specified encoder and parameters.
	/// </summary>
	public void Save(string filename, object encoder, object encoderParams) // TODO: implement EncoderParameters
	{
		var file = new FileInfo(filename);
		var stream = file.OpenWrite();
		Save(stream, encoder, encoderParams);
		stream.Close();
	}

	/// <summary>
	///  Saves this <see cref='Image'/> to the specified stream in the specified format.
	/// </summary>
	public void Save(Stream stream, ImageFormat format, int quality = 100)
	{
		if (!GX2SK.TryGetValue(format, out var skFormat))
			throw new NotSupportedException($"unsupported save {format} format");

		var image = SKImage.FromBitmap(m_bitmap);

		// TODO: Only Png, Jpeg and Webp allowed to encode, otherwise returns null
		// ref: https://learn.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/saving#exploring-the-image-formats
		var data = image.Encode(skFormat, quality) ?? throw new NotSupportedException($"unsupported encoding format {format}");
		data.SaveTo(stream);
		data.Dispose();
	}

	/// <summary>
	///  Saves this <see cref='Image'/> to the specified stream with the specified encoder and parameters.
	/// </summary>
	public void Save(Stream stream, object encoder, object encoderParams)
		=> throw new NotImplementedException(); // TODO: implement ImageCodecInfo & EncoderParameters

	/// <summary>
	///  Adds an EncoderParameters to this <see cref='Image'/>.
	/// </summary>
	public void SaveAdd(object encoderParams) // TODO: implement EncoderParameters
		=> SaveAdd(this, encoderParams);

	/// <summary>
	///  Adds an EncoderParameters to the specified <see cref='Image'/>.
	/// </summary>
	public void SaveAdd(Image image, object encoderParams) // TODO: implement EncoderParameters
		=> throw new NotImplementedException();

	/// <summary>
	///  Selects the frame specified by the given dimension and index.
	/// </summary>
	public void SelectActiveFrame(int index)
	{
		if (m_frames == 0)
			throw new Exception("image does not contain frames");

		if (index < 0 || index > m_frames - 1)
			throw new ArgumentException("frame index is out of range", nameof(index));

		m_index = index;
	}

	/// <summary>
	///  Sets the specified property item to the specified value.
	/// </summary>
	public void SetPropertyItem(object propitem)
		=> throw new NotImplementedException(); // TODO: implement PropertyItem 

	#endregion


	#region Utilities

	// Indexing for frame-based images (e.g. gif)
	internal int m_index = 0;

	// Redefinition for image type
	internal static readonly Dictionary<SKEncodedImageFormat, ImageFormat> SK2GX = new()
		{
			{ SKEncodedImageFormat.Bmp , ImageFormat.Bmp  },
			{ SKEncodedImageFormat.Gif , ImageFormat.Gif  },
			{ SKEncodedImageFormat.Ico , ImageFormat.Ico  },
			{ SKEncodedImageFormat.Png , ImageFormat.Png  },
			{ SKEncodedImageFormat.Wbmp, ImageFormat.Wbmp },
			{ SKEncodedImageFormat.Webp, ImageFormat.Webp },
			{ SKEncodedImageFormat.Pkm , ImageFormat.Pkm  },
			{ SKEncodedImageFormat.Ktx , ImageFormat.Ktx  },
			{ SKEncodedImageFormat.Astc, ImageFormat.Astc },
			{ SKEncodedImageFormat.Dng , ImageFormat.Dng  },
			{ SKEncodedImageFormat.Heif, ImageFormat.Heif },
			{ SKEncodedImageFormat.Jpeg, ImageFormat.Jpeg },
		};

	internal static readonly Dictionary<ImageFormat, SKEncodedImageFormat> GX2SK = SK2GX.ToDictionary(kv => kv.Value, kv => kv.Key);

	private static bool IsSvg(SKData data)
	{
		byte[] bytes = data.ToArray(), header = new byte[5];
		Array.Copy(bytes, header, 5);
		return Encoding.UTF8.GetString(header) == "<svg ";
	}

	#endregion
}
