using System;
using SkiaSharp;
using GeneXus.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GeneXus.Drawing;

public sealed class Region : IDisposable
{
	internal readonly SKRegion m_region;

	private Region(SKRegion region)
	{
		m_region = region;
	}

	private Region(SKRect rect)
		: this(new SKRegion(SKRectI.Round(rect))) { }

	/// <summary>
	///  Initializes a new <see cref='Region'/>.
	/// </summary>
	public Region()
		: this(new SKRegion()) { }

	/// <summary>
	///  Initializes a new <see cref='Region'/> from the specified <see cref='Rectangle'/> structure.
	/// </summary>
	public Region(RectangleF rect)
		: this(rect.m_rect) { }

	/// <summary>
	///  Initializes a new <see cref='Region'/> from the specified <see cref='Rectangle'/> structure.
	/// </summary>
	public Region(Rectangle rect)
		: this(rect.m_rect) { }

	/// <summary>
	///  Initializes a new <see cref='Region'/> from the specified <see cref='GraphicsPath'/> structure.
	/// </summary>
	public Region(GraphicsPath path)
		: this(new SKRegion(path.m_path)) { }

	/// <summary>
	///  Initializes a new <see cref='Region'/> from the specified data.
	/// </summary>
	public Region(RegionData data)
		: this(ParseRegionData(data)) { }

	/// <summary>
	///  Cleans up resources for this <see cref='Region'/>.
	/// </summary>
	~Region() => Dispose(false);


	#region IDisposable

	/// <summary>
	///  Releases all resources used by this <see cref='Region'/>.
	/// </summary>
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		Dispose(true);
	}

	private void Dispose(bool disposing) => m_region.Dispose();

	#endregion


	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='Region'/>.
	/// </summary>
	public Region Clone() => new(new SKRegion(m_region.GetBoundaryPath()));

	#endregion


	#region IEqualitable

	/// <summary>
	///  Determines whether the specified object is equal to the current object.
	/// </summary>
	public override bool Equals(object obj) => obj is Region region && m_region.Equals(region.m_region);

	/// <summary>
	/// Get the has code of this <see cref='Region'/>.
	/// </summary>
	public override int GetHashCode() => m_region.GetHashCode();

	#endregion


	#region Factory

	/// <summary>
	///  Initializes a new <see cref='Region'/> from a handle to the specified existing GDI region.
	/// </summary>
	public static Region FromHrgn(IntPtr hrgn)
		=> throw new NotSupportedException("windows specific");

	#endregion


	#region Methods

	/// <summary>
	///  Updates this <see cref='Region'/> to contain the portion of the specified <see cref='Region'/> that 
	///  does not intersect with this <see cref='Region'/>.
	/// </summary>
	public void Complement(Region region)
		=> m_region.Op(region.m_region, SKRegionOperation.ReverseDifference);

	/// <summary>
	///  Updates this <see cref='Region'/> to contain the portion of the specified <see cref='RectangleF'/> that 
	///  does not intersect with this <see cref='Region'/>.
	/// </summary>
	public void Complement(RectangleF rect)
		=> Complement(RectangleF.Truncate(rect));

	/// <summary>
	///  Updates this <see cref='Region'/> to contain the portion of the specified <see cref='Rectangle'/> that 
	///  does not intersect with this <see cref='Region'/>.
	/// </summary>
	public void Complement(Rectangle rect)
		=> Complement(new Region(rect));

	/// <summary>
	///  Updates this <see cref='Region'/> to contain the portion of the specified <see cref='GraphicsPath'/> that 
	///  does not intersect with this <see cref='Region'/>.
	/// </summary>
	public void Complement(GraphicsPath path)
		=> Complement(new Region(path));

	/// <summary>
	///  Updates this <see cref='Region'/> to contain only the portion of its interior that does not intersect 
	///  with the specified <see cref='Region'/>.
	/// </summary>
	public void Exclude(Region region)
		=> m_region.Op(region.m_region, SKRegionOperation.Difference);

	/// <summary>
	///  Updates this <see cref='Region'/> to contain only the portion of its interior that does not intersect 
	///  with the specified <see cref='RectangleF'/>.
	/// </summary>
	public void Exclude(RectangleF rect)
		=> Exclude(RectangleF.Truncate(rect));

	/// <summary>
	///  Updates this <see cref='Region'/> to contain only the portion of its interior that does not intersect 
	///  with the specified <see cref='Rectangle'/>.
	/// </summary>
	public void Exclude(Rectangle rect)
		=> Exclude(new Region(rect));

	/// <summary>
	///  Updates this <see cref='Region'/> to contain only the portion of its interior that does not intersect 
	///  with the specified <see cref='GraphicsPath'/>.
	/// </summary>
	public void Exclude(GraphicsPath path)
		=> Exclude(new Region(path));

	/// <summary>
	///  Gets a <see cref='Rectangle'/> structure that represents a rectangle that bounds this <see cref='Region'/> 
	///  on the drawing surface of a <see cref='Graphics'/> object.
	/// </summary>
	public RectangleF GetBounds(Graphics g)
	{
		var bounds = SKRect.Create(m_region.Bounds.Location, m_region.Bounds.Size);
		if (g != null)
			bounds.IntersectsWith(g.m_canvas.LocalClipBounds);
		return new(bounds);
	}

	/// <summary>
	///  Returns a Windows handle to this <see cref='Region'/> in the specified graphics context.
	/// </summary>
	public IntPtr GetHrgn(Graphics g)
		=> throw new NotSupportedException("windows specific");

	/// <summary>
	///  Returns a <see cref='RegionData'/> that represents the information that describes this <see cref='Region'/>.
	/// </summary>
	public RegionData GetRegionData()
	{
		using var iterator = m_region.CreateRectIterator();
		
		var rects = new List<SKRectI>();
		while (iterator.Next(out var rect))
			rects.Add(rect);

		var rdh = new RGNDATAHEADER
		{
			dwSize   = (uint)Marshal.SizeOf<RGNDATAHEADER>(),
			iType	= 1, // RDH_RECTANGLES
			nCount   = (uint)rects.Count,
			nRgnSize = (uint)(rects.Count * Marshal.SizeOf<RECT>())
		};
		var headerSize = Marshal.SizeOf<RGNDATAHEADER>();
		var bufferSize = rects.Count * Marshal.SizeOf<RECT>();
		var data = new byte[headerSize + bufferSize];

		// Copy header to data
		Buffer.BlockCopy(BitConverter.GetBytes(rdh.dwSize), 0, data, 0, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(rdh.iType), 0, data, 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(rdh.nCount), 0, data, 8, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(rdh.nRgnSize), 0, data, 12, 4);

		// Copy rectangles to data
		for (int i = 0; i < rects.Count; i++)
		{
			var rectData = new byte[Marshal.SizeOf<RECT>()];
			Buffer.BlockCopy(BitConverter.GetBytes(rects[i].Left), 0, rectData, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(rects[i].Top), 0, rectData, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(rects[i].Right), 0, rectData, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(rects[i].Bottom), 0, rectData, 12, 4);
			Buffer.BlockCopy(rectData, 0, data, headerSize + i * rectData.Length, rectData.Length);
		}

		return new RegionData(data);
	}

	/// <summary>
	///  Returns an array of <see cref='RectangleF'/> structures that approximate this <see cref='Region'/> after 
	///  the specified matrix transformation is applied.
	/// </summary>
	public RectangleF[] GetRegionScans(Matrix matrix) // TODO: apply Matrix
	{
		Transform(matrix);
		using var iterator = m_region.CreateRectIterator();
		var rects = new List<RectangleF>();
		while (iterator.Next(out var rect))
			rects.Add(new RectangleF(rect));
		return rects.ToArray();
	}

	/// <summary>
	///  Updates this <see cref='Region'/> to the intersection of itself with the specified <see cref='Region'/>.
	/// </summary>
	public void Intersect(Region region)
		=> m_region.Op(region.m_region, SKRegionOperation.Intersect);

	/// <summary>
	///  Updates this <see cref='Region'/> to the intersection of itself with the specified <see cref='RectangleF'/>.
	/// </summary>
	public void Intersect(RectangleF rect)
		=> Intersect(RectangleF.Truncate(rect));

	/// <summary>
	///  Updates this <see cref='Region'/> to the intersection of itself with the specified <see cref='Rectangle'/>.
	/// </summary>
	public void Intersect(Rectangle rect)
		=> Intersect(new Region(rect));

	/// <summary>
	///  Updates this <see cref='Region'/> to the intersection of itself with the specified <see cref='GraphicPath'/>.
	/// </summary>
	public void Intersect(GraphicsPath path)
		=> Intersect(new Region(path));

	/// <summary>
	///  Tests whether this <see cref='Region'/> has an empty interior on the specified drawing surface 
	///  of a <see cref='Graphics'/> object.
	/// </summary>
	public bool IsEmpty(Graphics g)
	{
		var region = new SKRegion(m_region);
		if (g != null)
			region.Intersects(g.Clip.m_region);
		return region.IsEmpty;
	}

	/// <summary>
	///  Tests whether this <see cref='Region'/> has an infinite interior on the specified drawing surface 
	///  of a <see cref='Graphic'/> object.
	/// </summary>
	public bool IsInfinite(Graphics g)
	{
		var region = new SKRegion(m_region);
		if (g != null)
			region.Intersects(g.Clip.m_region);
		return !region.IsRect;
	}

	/// <summary>
	///  Tests whether any portion of the specified rectangle is contained within this <see cref='Region'/> when drawn 
	///  using the specified <see cref='Graphics'/> (if it is defined).
	/// </summary>
	public bool IsVisible(int x, int y, int width, int height, Graphics g = null)
		=> IsVisible(new Rectangle(x, y, width, height), g);

	/// <summary>
	///  Tests whether any portion of the specified rectangle is contained within this <see cref='Region'/> when drawn 
	///  using the specified <see cref='Graphics'/> (if it is defined).
	/// </summary>
	public bool IsVisible(float x, float y, float width, float height, Graphics g = null)
		=> IsVisible(new RectangleF(x, y, width, height), g);

	/// <summary>
	///  Tests whether any portion of the specified <see cref='RectangleF'/> structure is contained within 
	///  this <see cref='Region'/> when drawn using the specified <see cref='Graphics'/> (if it is defined).
	/// </summary>
	public bool IsVisible(RectangleF rect, Graphics g = null)
		=> IsVisible(RectangleF.Truncate(rect), g);

	/// <summary>
	///  Tests whether any portion of the specified <see cref='Rectangle'/> structure is contained within 
	///  this <see cref='Region'/> when drawn using the specified <see cref='Graphics'/> (if it is defined).
	/// </summary>
	public bool IsVisible(Rectangle rect, Graphics g = null)
	{
		var region = new SKRegion(m_region);
		if (g != null)
			region.Intersects(g.Clip.m_region);
		return region.Contains(SKRectI.Round(rect.m_rect));
	}

	/// <summary>
	///  Tests whether the specified point is contained within this <see cref='Region'/> when drawn 
	///  using the specified <see cref='Graphics'/> (if it is defined).
	/// </summary>
	public bool IsVisible(int x, int y, Graphics g = null)
		=> IsVisible(new Point(x, y), g);

	/// <summary>
	///  Tests whether the specified point is contained within this <see cref='Region'/> when drawn 
	///  using the specified <see cref='Graphics'/> (if it is defined).
	/// </summary>
	public bool IsVisible(float x, float y, Graphics g = null)
		=> IsVisible(new PointF(x, y), g);

	/// <summary>
	///  Tests whether the specified <see cref='PointF'/> structure is contained within 
	///  this <see cref='Region'/> when drawn using the specified <see cref='Graphics'/> (if it is defined).
	/// </summary>
	public bool IsVisible(PointF point, Graphics g = null)
		=> IsVisible(new Point(unchecked((int)point.X), unchecked((int)point.Y)), g);

	/// <summary>
	///  Tests whether the specified <see cref='Point'/> structure is contained within 
	///  this <see cref='Region'/> when drawn using the specified <see cref='Graphics'/> (if it is defined).
	/// </summary>
	public bool IsVisible(Point point, Graphics g = null)
	{
		var region = new SKRegion(m_region);
		if (g != null)
		{
			var points = new[] { point };
			g.Transform.TransformPoints(points);
			point = points[0];

			region.Intersects(g.Clip.m_region);
		}
		return region.Contains(point.X, point.Y);
	}

	/// <summary>
	///  Initializes this <see cref='Region'/> to an empty interior.
	/// </summary>
	public void MakeEmpty()
		=> m_region.SetEmpty();

	/// <summary>
	///  Initializes this <see cref='Region'/> to an empty interior.
	/// </summary>
	public void MakeInfinite()
		=> m_region.SetRect(SKRectI.Create(int.MaxValue, int.MaxValue));

	/// <summary>
	///  Releases the handle of the <see cref='Region'/>.
	/// </summary>
	public void ReleaseHrgn(IntPtr regionHandle)
		=> throw new NotImplementedException();

	/// <summary>
	///  Transforms this <see cref='Region'/> by the specified <see cref='Matrix'/>.
	/// </summary>
	public void Transform(Matrix matrix)
	{
		var path = m_region.GetBoundaryPath();
		path.Transform(matrix.m_matrix);
		m_region.SetPath(path);
	}

	/// <summary>
	///  Offsets the coordinates of this <see cref='Region'/> by the specified amount.
	/// </summary>
	public void Translate(float dx, float dy)
		=> m_region.Translate((int)Math.Round(dx), (int)Math.Round(dy));

	/// <summary>
	///  Updates this <see cref='Region'/> to the union of itself with the specified <see cref='Region'/>.
	/// </summary>
	public void Union(Region region)
		=> m_region.Op(region.m_region, SKRegionOperation.Union);

	/// <summary>
	///  Updates this <see cref='Region'/> to the union of itself with the specified <see cref='RectangleF'/>.
	/// </summary>
	public void Union(RectangleF rect)
		=> Union(RectangleF.Truncate(rect));

	/// <summary>
	///  Updates this <see cref='Region'/> to the union of itself with the specified <see cref='Rectangle'/>.
	/// </summary>
	public void Union(Rectangle rect)
		=> Union(new Region(rect));

	/// <summary>
	///  Updates this <see cref='Region'/> to the union of itself with the specified <see cref='GraphicsPath'/>.
	/// </summary>
	public void Union(GraphicsPath path)
		=> Union(new Region(path));

	/// <summary>
	///  Updates this <see cref='Region'/> to the union minus the intersection of itself with the specified <see cref='Region'/>.
	/// </summary>
	public void Xor(Region region)
		=> m_region.Op(region.m_region, SKRegionOperation.XOR);

	/// <summary>
	///  Updates this <see cref='Region'/> to the union minus the intersection of itself with the specified <see cref='RectangleF'/>.
	/// </summary>
	public void Xor(RectangleF rect)
		=> Xor(RectangleF.Truncate(rect));

	/// <summary>
	///  Updates this <see cref='Region'/> to the union minus the intersection of itself with the specified <see cref='Rectangle'/>.
	/// </summary>
	public void Xor(Rectangle rect)
		=> Xor(new Region(rect));

	/// <summary>
	///  Updates this <see cref='Region'/> to the union minus the intersection of itself with the specified <see cref='GraphicsPath'/>.
	/// </summary>
	public void Xor(GraphicsPath path)
		=> Xor(new Region(path));

	#endregion


	#region Utilities

	[StructLayout(LayoutKind.Sequential)]
	private struct RECT
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct RGNDATAHEADER
	{
		public uint dwSize;
		public uint iType;
		public uint nCount;
		public uint nRgnSize;
	}

	private static SKRegion ParseRegionData(RegionData data)
	{
		var rgnData = data.Data;
		var headerSize = Marshal.SizeOf<RGNDATAHEADER>();
		var rectSize = Marshal.SizeOf<RECT>();

		if (rgnData.Length < headerSize)
			throw new ArgumentException("invalid region data.");

		var rdh = new RGNDATAHEADER
		{
			dwSize   = BitConverter.ToUInt32(rgnData, 0),
			iType	 = BitConverter.ToUInt32(rgnData, 4),
			nCount	 = BitConverter.ToUInt32(rgnData, 8),
			nRgnSize = BitConverter.ToUInt32(rgnData, 12)
		};

		if (rgnData.Length < headerSize + rdh.nRgnSize)
			throw new ArgumentException("invalid region data.");

		var region = new SKRegion();
		for (int i = 0; i < rdh.nCount; i++)
		{
			var offset = headerSize + i * rectSize;
			var rect = new SKRectI
			{
				Left   = BitConverter.ToInt32(rgnData, offset + 0),
				Top	= BitConverter.ToInt32(rgnData, offset + 4),
				Right  = BitConverter.ToInt32(rgnData, offset + 8),
				Bottom = BitConverter.ToInt32(rgnData, offset + 12)
			};
			region.Op(rect, SKRegionOperation.Union);
		}
		return region;
	}

	#endregion
}
