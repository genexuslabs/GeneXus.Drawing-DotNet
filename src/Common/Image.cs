using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SkiaSharp;

namespace GeneXus.Drawing.Common;

[Serializable]
public class Image : IDisposable, ICloneable
{
	internal readonly SKImage m_image;
	internal readonly SKCodec m_codec;
	internal readonly ImageFormat m_format;

	internal Image(SKImage skImage, SKData skData, ImageFormat format)
	{
		m_image = skImage;
		m_codec = SKCodec.Create(skData);
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
	public void Dispose()
	{
		m_image.Dispose();
		m_codec?.Dispose();
	}

	#endregion


	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='Image'/>.
	/// </summary>
	public virtual object Clone()
	{
		using var stream = new MemoryStream();
		Save(stream, RawFormat);
		stream.Seek(0, SeekOrigin.Begin);
		return FromStream(stream);
	}

	#endregion


	#region Operators

	/// <summary>
	/// Creates a <see cref='SKImage'/> with the coordinates of the specified <see cref='Image'/> .
	/// </summary>
	public static explicit operator SKImage(Image image) => image.m_image;

	#endregion


	#region Properties

	/// <summary>
	///  Gets the width of this <see cref='Image'/>.
	/// </summary>
	public int Width => m_image.Width;

	/// <summary>
	///  Gets the height of this <see cref='Image'/>.
	/// </summary>
	public int Height => m_image.Height;

	/// <summary>
	///  Gets the format of this <see cref='Image'/>.
	/// </summary>
	public ImageFormat RawFormat => m_format;

	/// <summary>
	///  Gets the width and height of this <see cref='Image'/>.
	/// </summary>
	public Size Size => new(Width, Height);

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
			return new Image(image, data, ImageFormat.Svg);
		}
		else
		{
			var image = SKImage.FromEncodedData(data);

			var codec = SKCodec.Create(image.EncodedData);
			if (!MAP_FORMAT.TryGetValue(codec.EncodedFormat, out ImageFormat format))
				throw new ArgumentException($"unsupported format {codec.EncodedFormat}");

			return new Image(image, data, format);
		}
	}

	#endregion


	#region Methods

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

	/// <summary>
	///  Saves this <see cref='Image'/> to the specified stream in the specified format.
	/// </summary>
	public void Save(Stream stream, ImageFormat format, int quality = 100)
	{
		if (format == ImageFormat.Svg)
			throw new NotSupportedException($"unsupported save {format} format");

		var skFormat = MAP_FORMAT.FirstOrDefault(kv => kv.Value == format).Key;
		var bitmap = new SKBitmap(new SKImageInfo(Width, Height));

		if (m_codec?.GetPixels(bitmap.Info, bitmap.GetPixels(), new SKCodecOptions(m_index)) != SKCodecResult.Success)
			throw new Exception($"failed to decode frame {m_index}");

		var image = SKImage.FromBitmap(bitmap);

		// TODO: Only Png, Jpeg and Webp allowed to encode, otherwise returns null
		// ref: https://learn.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/saving#exploring-the-image-formats
		var data = image.Encode(skFormat, quality) ?? throw new Exception($"unsupported format {format}");
		data.SaveTo(stream);
		data.Dispose();
	}

	/// <summary>
	///  Returns the number of frames of the given dimension.
	/// </summary>
	public int GetFrameCount() => m_frames;

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

	#endregion


	#region Utilities

	// Indexing for frame-based images (e.g. gif)
	internal int m_index = 0;
	internal int m_frames => Math.Max(m_codec?.FrameCount ?? 0, 1);

	// Redefinition for image type
	private static readonly Dictionary<SKEncodedImageFormat, ImageFormat> MAP_FORMAT = new()
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

	private static bool IsSvg(SKData data)
	{
		byte[] bytes = data.ToArray(), header = new byte[5];
		Array.Copy(bytes, header, 5);
		return Encoding.UTF8.GetString(header) == "<svg ";
	}

	#endregion
}
