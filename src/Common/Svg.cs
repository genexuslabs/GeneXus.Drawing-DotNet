using System;
using System.IO;
using System.Text;
using System.Xml;
using GeneXus.Drawing.Imaging;
using SkiaSharp;

namespace GeneXus.Drawing;

public class Svg : Image
{
	internal SkiaSharp.Extended.Svg.SKSvg m_svg;
	internal XmlDocument m_doc = new();

	internal Svg(SkiaSharp.Extended.Svg.SKSvg svg, XmlDocument doc) : base(ImageFormat.Svg, 1, svg.ViewBox.Size)
	{
		m_svg = svg ?? throw new ArgumentNullException(nameof(svg));
		m_doc = doc ?? throw new ArgumentNullException(nameof(doc));

		Width = (int)m_svg.Picture.CullRect.Width;
		Height = (int)m_svg.Picture.CullRect.Height;

		Flags = (int)(ImageFlags.ReadOnly | ImageFlags.Scalable);
		PixelFormat = PixelFormat.Undefined;
	}

	private Svg(Svg other)
		: this(other.m_svg, other.m_doc) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Svg'/> class from a filename
	/// </summary>
	public Svg(string filename)
		: this(FromFile(filename)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Svg'/> class from a file stream
	/// </summary>
	public Svg(Stream stream)
		: this(FromStream(stream)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Svg'/> class with the specified <see cref='Image'/>
	/// </summary>
	public Svg(Image original)
		: this(original, original.Width, original.Height) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Svg'/> class with the specified <see cref='Image'/>, width and height
	/// </summary>
	public Svg(Image original, float width, float height)
		: this(Resize(original, width, height)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Svg'/> class with the specified <see cref='Image'/> and <see cref='Size'/>
	/// </summary>
	public Svg(Image original, Size size)
		: this(original, size.Width, size.Height) { }

	/// <summary>
	/// Creates a human-readable string that represents this <see cref='Svg'/>.
	/// </summary>
	public override string ToString() => $"{GetType().Name}: {Width}x{Height}";


	#region IDisposable

	/// <summary>
	///  Cleans up resources for this <see cref='Svg'/>.
	/// </summary>
	protected override void Dispose(bool disposing) { }

	#endregion


	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='Svg'/>.
	/// </summary>
	public override object Clone() => new Svg(m_svg, m_doc);

	#endregion


	#region Operators

	/// <summary>
	/// Creates a <see cref='SkiaSharp.Extended.Svg.SKSvg'/> with the coordinates of the specified <see cref='Svg'/>.
	/// </summary>
	public static explicit operator SkiaSharp.Extended.Svg.SKSvg(Svg svg) => svg.m_svg;

	#endregion


	#region Properties

	/// <summary>
	///  Gets the XML string of this <see cref='Svg'/>.
	/// </summary>
	public string Xml => m_doc.OuterXml;

	#endregion


	#region Factory

	/// <summary>
	///  Creates an <see cref='Svg'/> from the specified data xml string.
	/// </summary>
	public static Svg FromXml(string xml)
	{
		using var reader = new XmlTextReader(new StringReader(xml));

		var svg = new SkiaSharp.Extended.Svg.SKSvg();
		svg.Load(reader);
		
		var doc = new XmlDocument();
		doc.LoadXml(xml);
		
		return new Svg(svg, doc);
	}

	#endregion


	#region Methods

	/// <inheritdoc cref="Image.RotateFlip(int, float, float)" />
	protected override void RotateFlip(int degrees, float scaleX, float scaleY)
	{
		var width = (int)Math.Abs(scaleX * Width);
		var height = (int)Math.Abs(scaleX * Height);

		m_doc.DocumentElement.SetAttribute("width", $"{width}");
		m_doc.DocumentElement.SetAttribute("height", $"{height}");
		m_doc.DocumentElement.SetAttribute("viewBox", $"0 0 {width} {height}");

		var group = m_doc.SelectSingleNode("//g");
		if (group == null)
		{
			group = m_doc.CreateElement("g", m_doc.DocumentElement.NamespaceURI);
			foreach (var tag in SHAPE_TAGS)
			{
				var shapes = m_doc.GetElementsByTagName(tag);
				for (int i = 0; i < shapes.Count; i++)
					group.AppendChild(shapes[i]);
			}
			m_doc.DocumentElement.PrependChild(group);
		}

		var centerX = Width / 2f;
		var centerY = Height / 2f;

		var offsetX = scaleX < 0 ? Width : 0;
		var offsetY = scaleY < 0 ? Height : 0;

		var transform = m_doc.CreateAttribute("transform");
		transform.Value = string.Join(" ", group.Attributes["transform"]?.Value ?? string.Empty,
			$"translate({offsetX} {offsetY})",
			$"scale({scaleX}, {scaleY})",
			$"rotate({degrees} {centerX} {centerY})").Trim();
		group.Attributes.SetNamedItem(transform);

		using var reader = new XmlNodeReader(m_doc);
		m_svg.Load(reader);
	}

	/// <summary>
	///  Saves this <see cref='Svg'/> to the specified file.
	/// </summary>
	public override void Save(string filename) => m_doc.Save(filename);

	#endregion


	#region Utilities

	private static readonly string[] SHAPE_TAGS = { "path", "rect", "circle", "ellipse", "line", "polyline", "polygon" };

	internal override SKImage m_image 
		=> SKImage.FromPicture(m_svg.Picture, SKRectI.Truncate(m_svg.Picture.CullRect).Size);

	private static Svg Resize(Image original, float width, float height)
	{
		var result = original.Clone() is Svg svg ? svg : throw new NotImplementedException("image to svg.");
		result.RotateFlip(0, width / original.Width, height / original.Height);
		return result;
	}

	#endregion
}
