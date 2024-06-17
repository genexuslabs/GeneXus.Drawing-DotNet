using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeneXus.Drawing.Imaging;
using SkiaSharp;

namespace GeneXus.Drawing;

[Serializable]
public class Icon : IDisposable, ICloneable
{
	internal readonly Bitmap m_bitmap;
	internal readonly List<IconEntry> m_entries;
	internal int m_index { get; private set; } = 0;

	private Icon(Bitmap bitmap, List<IconEntry> entries, float width, float height)
	{
		m_bitmap = bitmap;
		m_entries = entries;

		// find best index that matches width/height
		if (width < 0 && height < 0 || width == 0 && height > 0 || width > 0 && height == 0)
			(width, height) = (bitmap.Width, bitmap.Height);

		float pivot = float.MaxValue;
		for (int i = 0; i < m_entries.Count; i++)
		{
			var entry = m_entries[i];
			var diff = Math.Abs(entry.Width - width) + Math.Abs(entry.Height - height);
			if (diff >= pivot)
				continue;
			pivot = diff;
			m_index = i;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref='Icon'/> class from a  <see cref='Bitmap'/> instance.
	/// </summary>
	public Icon(Bitmap bitmap)
		: this(bitmap, new() { new IconEntry { Width = (byte)bitmap.Width, Height = (byte)bitmap.Height } }, -1, -1) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Icon'/> class from the specified file name.
	/// </summary>
	public Icon(string filename)
		: this(filename, byte.MaxValue, byte.MaxValue) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Icon'/> class of the specified <see cref='Size'/> from the specified file.
	/// </summary>
	public Icon(string filename, Size size)
		: this(filename, size.Width, size.Height) { }

	/// <summary>
	/// Initializes a new instance of the Icon class with the specified width and height from the specified file.
	/// </summary>
	public Icon(string filename, float width, float height)
		: this(File.OpenRead(filename), width, height) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Icon'/> class from the specified data stream.
	/// </summary>
	public Icon(Stream stream)
		: this(stream, byte.MaxValue, byte.MaxValue) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Icon'/> class of the specified size from the specified stream.
	/// </summary>
	public Icon(Stream stream, Size size)
		: this(stream, size.Width, size.Height) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Icon'/> class of the specified width and height from the specified stream.
	/// </summary>
	public Icon(Stream stream, float width, float height)
		: this(new Bitmap(Image.FromStream(stream)), ReadIco(stream), width, height) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Icon'/> class by copy.
	/// </summary>
	public Icon(Icon original)
		: this(original, original.Width, original.Height) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Icon'/> class by copy and attempts to find a version of the
	/// icon that matches the requested size.
	/// </summary>
	public Icon(Icon original, Size size)
		: this(original, size.Width, size.Height) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Icon'/> class by copy and attempts to find a version of the 
	/// icon that matches the requested width and height.
	/// </summary>
	public Icon(Icon original, float width, float height)
		: this(original.m_bitmap.Clone() as Bitmap ?? throw new ArgumentException("uninstantiated", nameof(original)), original.m_entries, width, height) { }

	/// <summary>
	///  Cleans up resources for this <see cref='Icon'/>.
	/// </summary>
	~Icon() => Dispose(false);

	/// <summary>
	/// Creates a human-readable string that represents this <see cref='Icon'/>.
	/// </summary>
	public override string ToString() => $"({GetType().Name})";


	#region IDisposable

	/// <summary>
	///  Cleans up resources for this <see cref='Icon'/>.
	/// </summary>
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		Dispose(true);
	}

	private void Dispose(bool disposing) => m_bitmap.Dispose();

	#endregion


	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='Icon'/>.
	/// </summary>
	public object Clone()
	{
		var bitmap = m_bitmap.Clone() as Bitmap ?? throw new Exception("could not clone");
		return new Icon(bitmap);
	}

	#endregion


	#region Properties

	/// <summary>
	///  Gets the Windows handle for this <see cref='Icon'/>. This is not a copy of the handle; do not free it.
	/// </summary>
	public IntPtr Handle => m_bitmap.Handle;

	/// <summary>
	///  Gets the height of this <see cref='Icon'/>.
	/// </summary>
	public int Height => m_entries[m_index].Height;

	/// <summary>
	///  Gets the size of this <see cref='Icon'/>.
	/// </summary>
	public Size Size => new(Width, Height);

	/// <summary>
	///  Gets the width of this <see cref='Icon'/>.
	/// </summary>
	public int Width => m_entries[m_index].Width;

	#endregion


	#region Factory

	/// <summary>
	///  Creates a GDI+ <see cref='Icon'/> from the specified Windows handle to an icon (HICON).
	/// </summary>
	public static Icon FromHandle(IntPtr handle)
	{
		var info = new SKImageInfo(100, 100);
		var skBitmap = new SKBitmap();
		skBitmap.InstallPixels(info, handle, info.RowBytes);
		var bitmap = new Bitmap(skBitmap);
		return new Icon(bitmap);
	}

	#endregion


	#region Methods

	/// <summary>
	///  Returns an icon representation of an image that is contained in the specified file.
	/// </summary>
	public static Icon ExtractAssociatedIcon(string filePath)
		=> ExtractIcon(filePath, -1); // NOTE: https://stackoverflow.com/a/37419253

	/// <summary>
	///  Extracts a specified icon from the given <paramref name="filePath"/>.
	/// </summary>
	public static Icon ExtractIcon(string filePath, int id, bool smallIcon = false)
		=> ExtractIcon(filePath, id, smallIcon ? 8 : ushort.MaxValue);

	/// <summary>
	///  Extracts a specified icon from the given <paramref name="filePath"/> and specified size.
	/// </summary>
	public static Icon ExtractIcon(string filePath, int id, int size)
	{
		if (size is <= 0 or > ushort.MaxValue)
			throw new ArgumentOutOfRangeException(nameof(size));

		if (new[] { ".dll", ".exe" }.Contains(Path.GetExtension(filePath)))
			throw new ArgumentException("portable executable (PE) file format is not supported.", nameof(filePath));

		var icon = new Icon(filePath, size, size);
		if (icon.Width == size && icon.Height == size)
			id = int.MaxValue; // set to undefined
		
		if (id >= 0 && id < icon.m_entries.Count) 
			icon.m_index = id; // set defined index

		return icon;
	}

	/// <summary>
	///  Saves this <see cref='Icon'/> to the specified output <see cref='Stream'/>.
	/// </summary>
	public void Save(Stream stream)
		=> new Bitmap(m_Resized).Save(stream, ImageFormat.Png, 100);

	/// <summary>
	///  Converts this <see cref='Icon'/> to a <see cref='Bitmap'/>.
	/// </summary>
	public Bitmap ToBitmap()
		=> new(m_Resized);

	#endregion


	#region Utilities

	internal struct IconEntry
	{
		public byte Width { get; set; }
		public byte Height { get; set; }
		public byte Colors { get; set; }
		public byte Reserved { get; set; }
		public ushort Planes { get; set; }
		public ushort Bpp { get; set; }
		public uint Size { get; set; }
		public uint Offset { get; set; }
	}

	private static List<IconEntry> ReadIco(Stream stream)
	{
		var entries = new List<IconEntry>();

		stream.Seek(0, SeekOrigin.Begin);
		using var reader = new BinaryReader(stream);

		// Read ICONDIR structure
		var header = new
		{
			Reserved = reader.ReadUInt16(),
			Type = reader.ReadUInt16(), // Type (1 for icon, 2 for cursor)
			Count = reader.ReadUInt16()  // Number of images
		};

		// Read ICONDIRENTRY structures
		for (int i = 0; i < header.Count; i++)
		{
			var entry = new IconEntry
			{
				Width = reader.ReadByte(),
				Height = reader.ReadByte(),
				Colors = reader.ReadByte(),
				Reserved = reader.ReadByte(),
				Planes = reader.ReadUInt16(),
				Bpp = reader.ReadUInt16(),
				Size = reader.ReadUInt32(),
				Offset = reader.ReadUInt32(),
			};
			entries.Add(entry);
		}
		return entries;
	}

	private SKBitmap m_Resized
	{
		get
		{
			var original = m_bitmap.Clone() as Bitmap ?? throw new Exception("cloning bitmap failed.");
			var resized = new SKBitmap(Width, Height);
			original.m_bitmap.ScalePixels(resized, SKFilterQuality.High);
			return resized;
		}
	}

	#endregion
}
