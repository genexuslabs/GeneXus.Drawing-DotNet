using System;
using System.Numerics;
using SkiaSharp;
using GeneXus.Drawing.Drawing2D;
using GeneXus.Drawing.Text;
using System.Linq;
using System.Collections.Generic;

namespace GeneXus.Drawing;

public sealed class Graphics : IDisposable
{
	internal readonly SKCanvas m_canvas;
	internal readonly SKBitmap m_bitmap;
	internal readonly SKPath m_path; // NOTE: tracks every shape added in order to implement IsVisible method
	private int m_context;

	internal static readonly (int X, int Y) DPI;

	static Graphics()
	{
		using var surface = SKSurface.Create(new SKImageInfo(50, 50));
		DPI.X = (int)(100f * surface.Canvas.DeviceClipBounds.Width / surface.Canvas.LocalClipBounds.Width);
		DPI.Y = (int)(100f * surface.Canvas.DeviceClipBounds.Height / surface.Canvas.LocalClipBounds.Height);
	}

	internal Graphics(SKBitmap bitmap)
	{
		m_bitmap = bitmap;
		m_canvas = new SKCanvas(m_bitmap);
		m_path = new SKPath();
		m_context = -1;

		Clip = new Region(ClipBounds);
	}

	/// <summary>
	///  Cleans up resources for this <see cref='Graphics'/>.
	/// </summary>
	~Graphics() => Dispose(false);


	#region IDisposable

	/// <summary>
	///  Cleans up resources for this <see cref='Graphics'/>.
	/// </summary>
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		Dispose(true);
	}

	private void Dispose(bool disposing)
	{
		if (disposing)
		{
			using var surface = SKSurface.Create(m_bitmap.Info);
			surface.Draw(m_canvas, 0, 0, new SKPaint() { Color = SKColors.Transparent });
			surface.Canvas.ClipRegion(Clip.m_region);

			using var image = surface.Snapshot();
			using var bitmap = SKBitmap.FromImage(image);
			m_bitmap.SetPixels(bitmap.GetPixels());
		}
		m_canvas.Dispose();
	}

	#endregion


	#region Operations

	/// <summary>
	/// Creates a <see cref='SKCanvas'/> with the coordinates of the specified <see cref='Graphics'/> .
	/// </summary>
	public static explicit operator SKCanvas(Graphics graphic) => graphic.m_canvas;

	#endregion


	#region Properties

	/// <summary>
	///  Gets or sets a <see cref='Region'/> that limits the drawing region of this <see cref='Graphics'/>.
	/// </summary>
	public Region Clip
	{
		get => new Region(ClipRegion.m_region);
		set => SetClip(value);
	}

	/// <summary>
	///  Gets a <see cref='Rectangle'/> structure that bounds the clipping region of this <see cref='Graphics'/>.
	/// </summary>
	public RectangleF ClipBounds => new(m_canvas.LocalClipBounds);

	/// <summary>
	///  Gets a value that specifies how composited images are drawn to this <see cref='Graphics'/>.
	/// </summary>
	public CompositeMode CompositingMode { get; set; } = CompositeMode.SourceOver;

	/// <summary>
	///  Gets or sets the rendering quality of composited images drawn to this <see cref='Graphics'/>.
	/// </summary>
	public CompositingQuality CompositingQuality { get; set; } = CompositingQuality.Default;

	/// <summary>
	///  Gets the horizontal resolution of this <see cref='Graphics'/>.
	/// </summary>
	public float DpiX => DPI.X;

	/// <summary>
	///  Gets the vertical resolution of this <see cref='Graphics'/>.
	/// </summary>
	public float DpiY => DPI.Y;

	/// <summary>
	///  Gets or sets the interpolation mode associated with this <see cref='Graphics'/>.
	/// </summary>
	public InterpolationMode InterpolationMode { get; set; } = InterpolationMode.Bilinear;

	/// <summary>
	///  Gets a value indicating whether the clipping region of this <see cref='Graphics'/> is empty.
	/// </summary>
	public bool IsClipEmpty => m_canvas.IsClipEmpty;

	/// <summary>
	///  Gets a value indicating whether the visible clipping region of this <see cref='Graphics'/> is empty.
	/// </summary>
	public bool IsVisibleClipEmpty => m_canvas.LocalClipBounds is { Width: <= 0, Height: <= 0 };

	/// <summary>
	///  Gets or sets the scaling between world units and page units for this <see cref='Graphics'/>.
	/// </summary>
	public float PageScale { get; set; } = 1;

	/// <summary>
	///  Gets or sets the unit of measure used for page coordinates in this <see cref='Graphics'/>.
	/// </summary>
	public GraphicsUnit PageUnit { get; set; } = GraphicsUnit.Display;

	/// <summary>
	///  Gets or sets a value specifying how pixels are offset during rendering of this <see cref='Graphics'/>.
	/// </summary>
	public PixelOffsetMode PixelOffsetMode { get; set; } = PixelOffsetMode.Default;

	/// <summary>
	///  Gets or sets the rendering origin of this <see cref='Graphics'/> for dithering and for hatch brushes.
	/// </summary>
	public Point RenderingOrigin { get; set; } = new Point(0, 0);

	/// <summary>
	///  Gets or sets the rendering quality for this <see cref='Graphics'/>.
	/// </summary>
	public SmoothingMode SmoothingMode { get; set; } = SmoothingMode.None;

	/// <summary>
	///  Gets or sets the gamma correction value for rendering text.
	/// </summary>
	public int TextContrast { get; set; } = 4;

	/// <summary>
	///  Gets or sets the rendering mode for text associated with this <see cref='Graphics'/>.
	/// </summary>
	public TextRenderingHint TextRenderingHint { get; set; } = TextRenderingHint.SystemDefault;

	/// <summary>
	///  Gets or sets a copy of the geometric world transformation for this <see cref='Graphics'/>.
	/// </summary>
	public Matrix Transform
	{
		get => new(m_canvas.TotalMatrix);
		set => m_canvas.SetMatrix(value.m_matrix);
	}

	/// <summary>
	///  Gets or sets the world transform elements for this <see cref='Graphics'/>.
	/// </summary>
	public Matrix3x2 TransformElements
	{
		get => Transform.MatrixElements;
		set => Transform.MatrixElements = value;
	}

	/// <summary>
	///  Gets the bounding rectangle of the visible clipping region of this <see cref='Graphics'/>.
	/// </summary>
	public RectangleF VisibleClipBounds => Clip.GetBounds(this);

	#endregion


	#region Factory

	/// <summary>
	///  Creates a new <see cref='Graphics'/> from the specified handle to a device 
	///  context.
	/// </summary>
	public static Graphics FromHdc(IntPtr hdc)
		=> FromHdc(hdc, IntPtr.Zero);

	/// <summary>
	///  Creates a new <see cref='Graphics'/> from the specified handle to a device 
	///  context and handle to a device.
	/// </summary>
	public static Graphics FromHdc(IntPtr hdc, IntPtr hdevice)
		=> throw new NotImplementedException();

	/// <summary>
	///  Returns a <see cref='Graphics'/> for the specified device context.
	/// </summary>
	public static Graphics FromHdcInternal(IntPtr hdc)
		=> throw new NotImplementedException();

	/// <summary>
	///  Creates a new <see cref='Graphics'/> from the specified handle to a window.
	/// </summary>
	public static Graphics FromHwnd(IntPtr hwnd)
		=> throw new NotImplementedException();

	/// <summary>
	///  Creates a new <see cref='Graphics'/> for the specified windows handle.
	/// </summary>
	public static Graphics FromHwndInternal(IntPtr hwnd)
		=> throw new NotImplementedException();

	/// <summary>
	///  Creates a new <see cref='Graphics'/> from the specified <see cref='Image'/>.
	/// </summary>
	public static Graphics FromImage(Image image)
	{
		var bitmap = image is Bitmap bm ? bm.m_bitmap : SKBitmap.FromImage(image.InnerImage);
		var graphics = new Graphics(bitmap);
		graphics.DrawImage(image, 0, 0);
		return graphics;
	}

	#endregion


	#region Methods

	/// <summary>
	///  Adds a comment to the current <see cref='Metafile'/>.
	/// </summary>
	public void AddMetafileComment(byte[] data)
		=> throw new NotImplementedException();

	/// <summary>
	///  Saves a graphics container with the current state of this <see cref='Graphics'/> and
	///  and opens and uses a new graphics container.
	/// </summary>
	public GraphicsContainer BeginContainer()
	{
		var rect = new RectangleF(m_canvas.LocalClipBounds);
		return BeginContainer(rect, rect, GraphicsUnit.Pixel);
	}

	/// <summary>
	///  Saves a graphics container with the current state of this <see cref='Graphics'/> and
	///  opens and uses a new graphics container with the specified scale transformation.
	/// </summary>
	public GraphicsContainer BeginContainer(RectangleF dstRect, RectangleF srcRect, GraphicsUnit unit)
	{
		int state = m_canvas.SaveLayer();

		float factorX = GetFactor(DpiX, unit, GraphicsUnit.Pixel);
		float factorY = GetFactor(DpiY, unit, GraphicsUnit.Pixel);

		var src = new RectangleF(srcRect.X, srcRect.Y, srcRect.Width * factorX, srcRect.Height * factorY);
		var dst = new RectangleF(dstRect.X, dstRect.Y, dstRect.Width * factorX, dstRect.Height * factorY);

		float scaleX = dst.Width / src.Width;
		float scaleY = dst.Height / src.Height;

		float translateX = dst.Left - src.Left * scaleX;
		float translateY = dst.Top - src.Top * scaleY;

		m_canvas.Translate(translateX, translateY);
		m_canvas.Scale(scaleX, scaleY);

		return new GraphicsContainer(state);
	}

	/// <summary>
	///  Saves a graphics container with the current state of this <see cref='Graphics'/> and
	///  opens and uses a new graphics container with the specified scale transformation.
	/// </summary>
	public GraphicsContainer BeginContainer(Rectangle dstRect, Rectangle srcRect, GraphicsUnit unit)
		=> BeginContainer(new RectangleF(dstRect.m_rect), new RectangleF(srcRect.m_rect), unit);

	/// <summary>
	///  Clears the entire drawing surface and fills it with a transparent background color.
	/// </summary>
	public void Clear()
		=> Clear(Color.Transparent);

	/// <summary>
	///  Clears the entire drawing surface and fills it with the specified background color.
	/// </summary>
	public void Clear(Color color)
	{
		ClipColor = color;
		m_canvas.Clear(ClipColor.m_color);
	}

	/// <summary>
	///  Performs a bit-block transfer of the color data, corresponding to a rectangle of pixels,
	///  from the screen to the drawing surface of the <see cref='Graphics'/>.
	/// </summary>
	public void CopyFromScreen(Point srcUpperLeft, Point dstUpperLeft, Size blockRegionSize, CopyPixelOperation copyPixelOperation = CopyPixelOperation.SourceCopy)
		=> CopyFromScreen(srcUpperLeft.X, srcUpperLeft.Y, dstUpperLeft.X, dstUpperLeft.Y, blockRegionSize, copyPixelOperation);

	/// <summary>
	///  Performs a bit-block transfer of the color data, corresponding to a rectangle of pixels, 
	///  from the screen to the drawing surface of the <see cref='Graphics'/>.
	/// </summary>
	public void CopyFromScreen(int srcX, int srcY, int dstX, int dstY, Size blockRegionSize, CopyPixelOperation copyPixelOperation = CopyPixelOperation.SourceCopy)
		=> throw new NotSupportedException("skia unsupported feature");

	/// <summary>
	///  Draws an arc representing a portion of an ellipse specified by a <see cref='Rectangle'/> structure.
	/// </summary>
	public void DrawArc(Pen pen, RectangleF oval, float startAngle, float sweepAngle)
	{
		using var path = new GraphicsPath();
		path.AddArc(oval, startAngle, sweepAngle);
		DrawPath(pen, path);
	}

	/// <summary>
	///  Draws an arc representing a portion of an ellipse specified by a pair of coordinates, a width, and a height.
	/// </summary>
	public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngule, float sweepAngle)
		=> DrawArc(pen, new RectangleF(x, y, width, height), startAngule, sweepAngle);

	/// <summary>
	///  Draws an arc representing a portion of an ellipse specified by a <see cref='RectangleF'/> structure.
	/// </summary>
	public void DrawArc(Pen pen, Rectangle oval, float startAngle, float sweepAngle)
		=> DrawArc(pen, new RectangleF(oval.m_rect), startAngle, sweepAngle);

	/// <summary>
	///  Draws an arc representing a portion of an ellipse specified by a pair of coordinates, a width, and a height.
	/// </summary>
	public void DrawArc(Pen pen, int x, int y, int width, int height, float startAngule, float sweepAngle)
		=> DrawArc(pen, new Rectangle(x, y, width, height), startAngule, sweepAngle);

	/// <summary>
	///  Draws a cubic Bezier curve defined by four points.
	/// </summary>
	public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
		=> DrawBeziers(pen, new[] { pt1, pt2, pt3, pt4 });

	/// <summary>
	///  Draws a cubic Bezier curve defined by four ordered pairs that represent points.
	/// </summary>
	public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		=> DrawBezier(pen, new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y3), new PointF(x4, y4));

	/// <summary>
	///  Draws a cubic Bezier curve defined by four points.
	/// </summary>
	public void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4)
		=> DrawBeziers(pen, new[] { pt1, pt2, pt3, pt4 });

	/// <summary>
	///  Draws a cubic Bezier curve defined by four ordered pairs that represent points.
	/// </summary>
	public void DrawBezier(Pen pen, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
		=> DrawBezier(pen, new Point(x1, y1), new Point(x2, y2), new Point(x3, y3), new Point(x4, y4));

	/// <summary>
	///  Draws a series of Bézier splines from an array of <see cref='PointF'/> structures.
	/// </summary>
	public void DrawBeziers(Pen pen, params PointF[] points)
	{
		if (points.Length < 4 || points.Length % 3 != 1)
			throw new ArgumentException("invalid number of points for drawing Bezier curves", nameof(points));

		using var path = new GraphicsPath();
		path.AddBeziers(points);
		DrawPath(pen, path);
	}

	/// <summary>
	///  Draws a series of Bézier splines from an array of <see cref='Point'/> structures.
	/// </summary>
	public void DrawBeziers(Pen pen, params Point[] points)
		=> DrawBeziers(pen, Array.ConvertAll(points, point => new PointF(point.m_point)));

	/// <summary>
	///  Draws the given <paramref name="cachedBitmap"/>.
	/// </summary>
	public void DrawCachedBitmap(object cachedBitmap, int x, int y)
		=> throw new NotImplementedException();

	/// <summary>
	///  Draws a closed cardinal spline defined by an array of <see cref='PointF'/> structures using a specified tension
	/// </summary>
	public void DrawClosedCurve(Pen pen, PointF[] points, float tension = 0.5f, FillMode fillMode = FillMode.Winding)
	{
		using var path = GetCurvePath(points, fillMode, tension, true);
		DrawPath(pen, path);
	}

	/// <summary>
	///  Draws a closed cardinal spline defined by an array of <see cref='Point'/> structures using a specified tension
	/// </summary>
	public void DrawClosedCurve(Pen pen, Point[] points, float tension = 0.5f, FillMode fillMode = FillMode.Winding)
		=> DrawClosedCurve(pen, Array.ConvertAll(points, point => new PointF(point.m_point)), tension, fillMode);

	/// <summary>
	///  Draws a cardinal spline through a specified array of <see cref='PointF'/> structures using a specified tension. The drawing 
	///  begins offset from the beginning of the array.
	/// </summary>
	public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension = 0.5f)
	{
		if (offset < 0 || offset >= points.Length)
			throw new ArgumentOutOfRangeException(nameof(offset));

		if (numberOfSegments < 1 || offset + numberOfSegments > points.Length)
			throw new ArgumentOutOfRangeException(nameof(numberOfSegments));

		var fillMode = FillMode.Alternate;
		points = points.Skip(offset).Take(numberOfSegments).ToArray();

		using var path = GetCurvePath(points, fillMode, tension, false);
		DrawPath(pen, path);
	}

	/// <summary>
	///  Draws a cardinal spline through a specified array of <see cref='Point'/> structures using a specified tension. The drawing 
	///  begins offset from the beginning of the array.
	/// </summary>
	public void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension = 0.5f)
		=> DrawCurve(pen, Array.ConvertAll(points, point => new PointF(point.m_point)), offset, numberOfSegments, tension);

	/// <summary>
	///  Draws a cardinal spline through a specified array of <see cref='PointF'/> structures using a specified tension.
	/// </summary>
	public void DrawCurve(Pen pen, PointF[] points, float tension = 0.5f)
		=> DrawCurve(pen, points, 0, points.Length, tension);

	/// <summary>
	///  Draws a cardinal spline through a specified array of <see cref='Point'/> structures using a specified tension.
	/// </summary>
	public void DrawCurve(Pen pen, Point[] points, float tension = 0.5f)
		=> DrawCurve(pen, Array.ConvertAll(points, point => new PointF(point.m_point)), 0, points.Length, tension);

	/// <summary>
	///  Draws an ellipse defined by a bounding rectangle specified by coordinates for the upper-left corner of the 
	///  rectangle, a height, and a width.
	/// </summary>
	public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		=> DrawEllipse(pen, new RectangleF(x, y, width, height));

	/// <summary>
	///  Draws an ellipse specified by a bounding <see cref='RectangleF'/> structure.
	/// </summary>
	public void DrawEllipse(Pen pen, RectangleF rect)
	{
		using var path = GetEllipsePath(rect);
		DrawPath(pen, path);
	}

	/// <summary>
	///  Draws an ellipse defined by a bounding rectangle specified by coordinates for the upper-left corner of the 
	///  rectangle, a height, and a width.
	/// </summary>
	public void DrawEllipse(Pen pen, int x, int y, int width, int height)
		=> DrawEllipse(pen, new Rectangle(x, y, width, height));

	/// <summary>
	///  Draws an ellipse specified by a bounding <see cref='Rectangle'/> structure.
	/// </summary>
	public void DrawEllipse(Pen pen, Rectangle rect)
		=> DrawEllipse(pen, new RectangleF(rect.m_rect));

	/// <summary>
	///  Draws the image represented by the specified <see cref='Icon'/> within the area specified 
	///  by a <see cref='Rectangle'/> structure.
	/// </summary>
	public void DrawIcon(Icon icon, Rectangle rect)
		=> DrawImage(icon.ToBitmap(), rect);

	/// <summary>
	///  Draws the image represented by the specified <see cref='Icon'/> at the specified coordinates.
	/// </summary>
	public void DrawIcon(Icon icon, int x, int y)
		=> DrawImage(icon.ToBitmap(), x, y);

	/// <summary>
	///  Draws the image represented by the specified <see cref='Icon'/> without scaling the image.
	/// </summary>
	public void DrawIconUnstretched(Icon icon, Rectangle rect)
		=> DrawImageUnscaled(icon.ToBitmap(), rect);

	/// <summary>
	///  Draws the specified <see cref='Image'/>, using its original physical size, at the specified
	///  location given by a <see cref='PointF'/> structure.
	/// </summary>
	public void DrawImage(Image image, PointF point)
		=> DrawImage(image, point.X, point.Y);

	/// <summary>
	///  Draws the specified <see cref='Image'/>, using its original physical size, at the specified location.
	/// </summary>
	public void DrawImage(Image image, float x, float y)
		=> DrawImage(image, x, y, image.Width, image.Height);

	/// <summary>
	///  Draws the specified <see cref='Image'/>, using its original physical size, at the specified
	///  location given by a <see cref='Point'/> structure.
	/// </summary>
	public void DrawImage(Image image, Point point)
		=> DrawImage(image, new PointF(point.m_point));

	/// <summary>
	///  Draws the specified <see cref='Image'/>, using its original physical size, at the specified location.
	/// </summary>
	public void DrawImage(Image image, int x, int y)
		=> DrawImage(image, x, y, image.Width, image.Height);

	/// <summary>
	///  Draws the specified <see cref='Image'/> at the specified location and with the specified 
	///  size given by a <see cref='RectangleF'/> structure.
	/// </summary>
	public void DrawImage(Image image, RectangleF rect)
		=> m_canvas.DrawImage(image.InnerImage, rect.m_rect);

	/// <summary>
	///  Draws the specified <see cref='Image'/> at the specified location and with the specified size.
	/// </summary>
	public void DrawImage(Image image, float x, float y, float width, float height)
		=> DrawImage(image, new RectangleF(x, y, width, height));

	/// <summary>
	///  Draws the specified <see cref='Image'/> at the specified location and with the specified 
	///  size given by a <see cref='Rectangle'/> structure.
	/// </summary>
	public void DrawImage(Image image, Rectangle rect)
		=> DrawImage(image, new RectangleF(rect.m_rect));

	/// <summary>
	///  Draws the specified <see cref='Image'/> at the specified location and with the specified size.
	/// </summary>
	public void DrawImage(Image image, int x, int y, int width, int height)
		=> DrawImage(image, new Rectangle(x, y, width, height));

	/// <summary>
	///  Draws the specified <see cref='Image'/> at the specified location and with the specified 
	///  shape and size.
	/// </summary>
	public void DrawImage(Image image, PointF[] points)
	{
		var unit = GraphicsUnit.Pixel;
		var bounds = image.GetBounds(ref unit);
		DrawImage(image, points, new RectangleF(bounds.m_rect), unit);
	}

	/// <summary>
	///  Draws the specified <see cref='Image'/> at the specified location and with the specified 
	///  shape and size.
	/// </summary>
	public void DrawImage(Image image, Point[] points)
		=> DrawImage(image, Array.ConvertAll(points, point => new PointF(point.m_point)));

	/// <summary>
	///  Draws a portion of an <see cref='Image'/> at a specified location.
	/// </summary>
	public void DrawImage(Image image, float x, float y, RectangleF rect, GraphicsUnit unit)
		=> DrawImage(image, new RectangleF(x, y, rect.Width, rect.Height), rect, unit);

	/// <summary>
	///  Draws a portion of an <see cref='Image'/> at a specified location.
	/// </summary>
	public void DrawImage(Image image, int x, int y, Rectangle rect, GraphicsUnit unit)
		=> DrawImage(image, new RectangleF(x, y, rect.Width, rect.Height), rect, unit);

	/// <summary>
	///  Draws the specified portion of the specified <see cref='Image'/> at the specified location 
	///  and with the specified size.
	/// </summary>
	public void DrawImage(Image image, RectangleF destination, RectangleF source, GraphicsUnit unit)
	{
		var dst = new PointF[] 
		{  
			new(destination.Left, destination.Top),
			new(destination.Right, destination.Top),
			new(destination.Right, destination.Bottom),
			new(destination.Left, destination.Bottom)
		};
		DrawImage(image, dst, source, unit);
	}

	/// <summary>
	///  Draws the specified portion of the specified <see cref='Image'/> at the specified location 
	///  and with the specified size.
	/// </summary>
	public void DrawImage(Image image, Rectangle destination, Rectangle source, GraphicsUnit unit)
		=> DrawImage(image, new RectangleF(destination.m_rect), new RectangleF(source.m_rect), unit);

	/// <summary>
	///  Draws a portion of an <see cref='Image'/> at a specified location and with the specified size.
	/// </summary>
	public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, object imageAtt = null, object callback = null, int callbackData = 0)
	{
		// TODO: Implement ImageAttributes (attributes), DrawImageAbort (callback) and IntPtr (callbackData)
		using var path = new GraphicsPath();
		path.AddLines(destPoints);
		path.CloseFigure();

		m_path.AddPath(path.m_path);
		m_canvas.ClipPath(path.m_path);

		var dstRect = path.GetBounds();

		float factorX = GetFactor(DpiX, srcUnit, GraphicsUnit.Pixel);
		float factorY = GetFactor(DpiY, srcUnit, GraphicsUnit.Pixel);

		var src = new RectangleF(srcRect.X, srcRect.Y, srcRect.Width * factorX, srcRect.Height * factorY);
		var dst = new RectangleF(dstRect.X, dstRect.Y, dstRect.Width * factorX, dstRect.Height * factorY);

		m_canvas.DrawImage(image.InnerImage, src.m_rect, dst.m_rect);
	}

	/// <summary>
	///  Draws a portion of an <see cref='Image'/> at a specified location and with the specified size.
	/// </summary>
	public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, object imageAtt = null, object callback = null, int callbackData = 0)
		=> DrawImage(image, Array.ConvertAll(destPoints, point => new PointF(point.m_point)), new RectangleF(srcRect.m_rect), srcUnit, imageAtt, callback, callbackData);

	/// <summary>
	///  Draws the specified portion of the specified <see cref='Image'/> at the specified location and with the specified size.
	/// </summary>
	public void DrawImage(Image image, RectangleF destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, object imageAttrs = null, object callback = null)
		=> DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, callback, IntPtr.Zero);

	/// <summary>
	///  Draws the specified portion of the specified <see cref='Image'/> at the specified location and with the specified size.
	/// </summary>
	public void DrawImage(Image image, RectangleF destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, object imageAttrs, object callback, IntPtr callbackData)
	{
		var destPoints = new PointF[]
		{
			new(destRect.Left,  destRect.Top),
			new(destRect.Right, destRect.Top),
			new(destRect.Right, destRect.Bottom),
			new(destRect.Left,  destRect.Bottom),
		};
		DrawImage(image, destPoints, new RectangleF(srcX, srcY, srcWidth, srcHeight), srcUnit, imageAttrs, callback, unchecked((int)callbackData));
	}

	/// <summary>
	///  Draws the specified portion of the specified <see cref='Image'/> at the specified location and with the specified size.
	/// </summary>
	public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, object imageAttrs = null, object callback = null)
		=> DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, callback, IntPtr.Zero);

	/// <summary>
	///  Draws the specified portion of the specified <see cref='Image'/> at the specified location and with the specified size.
	/// </summary>
	public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, object imageAttrs, object callback, IntPtr callbackData)
		=> DrawImage(image, new RectangleF(destRect.m_rect), srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, callback, callbackData);

	/// <summary>
	///  Draws the specified <see cref='Image'/> using its original physical size at a specified location.
	/// </summary>
	public void DrawImageUnscaled(Image image, Point point)
		=> DrawImageUnscaled(image, point.X, point.Y);

	/// <summary>
	///  Draws the specified <see cref='Image'/> using its original physical size at the location specified by a coordinate pair.
	/// </summary>
	public void DrawImageUnscaled(Image image, int x, int y)
		=> DrawImageUnscaled(image, x, y, image.Width, image.Height);

	/// <summary>
	///  Draws the specified <see cref='Image'/> using its original physical size at a specified location.
	/// </summary>
	public void DrawImageUnscaled(Image image, Rectangle rect)
		=> DrawImage(image, rect);

	/// <summary>
	///  Draws the specified <see cref='Image'/> using its original physical size at a specified location.
	/// </summary>
	public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
		=> DrawImageUnscaled(image, new Rectangle(x, y, width, height));

	/// <summary>
	///  Draws the specified <see cref='Image'/> without scaling and clips it, if necessary, to fit in the specified rectangle.
	/// </summary>
	public void DrawImageUnscaledAndClipped(Image image, Rectangle rect)
		=> DrawImage(image, rect, rect, GraphicsUnit.Pixel);

	/// <summary>
	///  Draws a line connecting the two points specified by the coordinate pairs.
	/// </summary>
	public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
		=> DrawLine(pen, new PointF(x1, y1), new PointF(x2, y2));

	/// <summary>
	///  Draws a line connecting two <see cref='PointF'/> structures.
	/// </summary>
	public void DrawLine(Pen pen, PointF point1, PointF point2)
	{
		using var path = new GraphicsPath();
		path.AddLine(point1, point2);
		DrawPath(pen, path);
	}

	/// <summary>
	///  Draws a line connecting the two points specified by the coordinate pairs.
	/// </summary>
	public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
		=> DrawLine(pen, new Point(x1, y1), new Point(x2, y2));

	/// <summary>
	///  Draws a line connecting two <see cref='Point'/> structures.
	/// </summary>
	public void DrawLine(Pen pen, Point point1, Point point2)
		=> DrawLine(pen, new PointF(point1.m_point), new PointF(point2.m_point));

	/// <summary>
	///  Draws a series of line segments that connect an array of <see cref='PointF'/> structures.
	/// </summary>
	public void DrawLines(Pen pen, params PointF[] points)
	{
		if (points.Length < 2)
			throw new ArgumentException("must define at least 2 points", nameof(points));
		for (int i = 1; i < points.Length; i++)
			DrawLine(pen, points[i - 1], points[i]);
	}

	/// <summary>
	///  Draws a series of line segments that connect an array of <see cref='Point'/> structures.
	/// </summary>
	public void DrawLines(Pen pen, params Point[] points)
		=> DrawLines(pen, Array.ConvertAll(points, point => new PointF(point.m_point)));

	/// <summary>
	///  Draws a <see cref='GraphicsPath'/>.
	/// </summary>
	public void DrawPath(Pen pen, GraphicsPath path)
		=> PaintPath(path.m_path, pen.m_paint);

	/// <summary>
	///  Draws a pie shape defined by an ellipse specified by a <see cref='RectangleF'/> structure and two radial lines.
	/// </summary>
	public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
	{
		using var path = GetPiePath(rect, startAngle, sweepAngle);
		DrawPath(pen, path);
	}

	/// <summary>
	///  Draws a pie shape defined by an ellipse specified by a <see cref='Rectangle'/> structure and two radial lines.
	/// </summary>
	public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
		=> DrawPie(pen, new RectangleF(rect.m_rect), startAngle, sweepAngle);

	/// <summary>
	///  Draws a pie shape defined by an ellipse specified by a coordinate pair, a width, a height, and two radial lines.
	/// </summary>
	public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		=> DrawPie(pen, new RectangleF(x, y, width, height), startAngle, sweepAngle);

	/// <summary>
	///  Draws a pie shape defined by an ellipse specified by a coordinate pair, a width, a height, and two radial lines.
	/// </summary>
	public void DrawPie(Pen pen, int x, int y, int width, int height, float startAngle, float sweepAngle)
		=> DrawPie(pen, new Rectangle(x, y, width, height), startAngle, sweepAngle);

	/// <summary>
	///  Draws a polygon defined by an array of <see cref='PointF'/> structures.
	/// </summary>
	public void DrawPolygon(Pen pen, params PointF[] points)
	{
		using var path = GetPolygonPath(points, FillMode.Winding);
		DrawPath(pen, path);
	}

	/// <summary>
	///  Draws a polygon defined by an array of <see cref='Point'/> structures.
	/// </summary>
	public void DrawPolygon(Pen pen, params Point[] points)
		=> DrawPolygon(pen, Array.ConvertAll(points, point => new PointF(point.m_point)));

	/// <summary>
	///  Draws a rectangle specified by a <see cref='RectangleF'/> structure.
	/// </summary>
	public void DrawRectangle(Pen pen, RectangleF rect)
	{
		using var path = GetRectanglePath(rect);
		DrawPath(pen, path);
	}

	/// <summary>
	///  Draws a rectangle specified by a <see cref='Rectangle'/> structure.
	/// </summary>
	public void DrawRectangle(Pen pen, Rectangle rect)
		=> DrawRectangle(pen, new RectangleF(rect.m_rect));

	/// <summary>
	///  Draws a rectangle specified by a coordinate pair, a width, and a height.
	/// </summary>
	public void DrawRectangle(Pen pen, float x, float y, float witdth, float height)
		=> DrawRectangle(pen, new RectangleF(x, y, witdth, height));

	/// <summary>
	///  Draws a rectangle specified by a coordinate pair, a width, and a height.
	/// </summary>
	public void DrawRectangle(Pen pen, int x, int y, int witdth, int height)
		=> DrawRectangle(pen, new Rectangle(x, y, witdth, height));

	/// <summary>
	///  Draws a series of rectangles specified by <see cref='RectangleF'/> structures.
	/// </summary>
	public void DrawRectangles(Pen pen, RectangleF[] rects)
		=> Array.ForEach(rects, rect => DrawRectangle(pen, rect));

	/// <summary>
	///  Draws a series of rectangles specified by <see cref='Rectangle'/> structures.
	/// </summary>
	public void DrawRectangles(Pen pen, Rectangle[] rects)
		=> DrawRectangles(pen, Array.ConvertAll(rects, rect => new RectangleF(rect.m_rect)));

	/// <summary>
	///  Draws the specified text at the specified location with the specified <see cref="Brush"/> and
	///  <see cref="Font"/> objects using the formatting attributes of the specified StringFormat.
	/// </summary>
	public void DrawString(ReadOnlySpan<char> text, Font font, Brush brush, float x, float y, StringFormat format = null)
		=> DrawString(new string(text.ToArray()), font, brush, x, y, format);

	/// <summary>
	///  Draws the specified text at the specified location with the specified <see cref="Brush"/> and
	///  <see cref="Font"/> objects using the formatting attributes of the specified StringFormat.
	/// </summary>
	public void DrawString(string text, Font font, Brush brush, float x, float y, StringFormat format = null)
		=> DrawString(text, font, brush, new PointF(x, y), format);

	/// <summary>
	///  Draws the specified text string in the specified location with the specified <see cref="Brush"/> and 
	///  <see cref="Font"/> objects using the formatting attributes of the specified StringFormat.
	/// </summary
	public void DrawString(ReadOnlySpan<char> text, Font font, Brush brush, PointF point, StringFormat format = null)
		=> DrawString(new string(text.ToArray()), font, brush, new RectangleF(point.X, point.Y, 0, 0), format);

	/// <summary>
	///  Draws the specified text string in the specified location with the specified <see cref="Brush"/> and 
	///  <see cref="Font"/> objects using the formatting attributes of the specified StringFormat.
	/// </summary
	public void DrawString(string text, Font font, Brush brush, PointF point, StringFormat format = null)
		=> DrawString(text, font, brush, new RectangleF(point.X, point.Y, 0, 0), format);

	/// <summary>
	///  Draws the specified text string in the specified rectangle with the specified <see cref="Brush"/> and 
	///  <see cref="Font"/> objects using the formatting attributes of the specified StringFormat.
	/// </summary>
	public void DrawString(ReadOnlySpan<char> text, Font font, Brush brush, RectangleF layout, StringFormat format = null)
		=> DrawString(new string(text.ToArray()), font, brush, layout, format);

	/// <summary>
	///  Draws the specified text string in the specified rectangle with the specified <see cref="Brush"/> and 
	///  <see cref="Font"/> objects using the formatting attributes of the specified StringFormat.
	/// </summary>
	public void DrawString(string text, Font font, Brush brush, RectangleF layout, StringFormat format = null)
	{
		using var path = new GraphicsPath();
		path.AddString(text, font.FontFamily, (int)font.Style, font.Size, layout, format);
		FillPath(brush, path);
	}

	/// <summary>
	///  Closes the current graphics container and restores the state of this <see cref="Graphics"/> to 
	///  the state saved by a call to the <see cref="BeginContainer"/> method.
	/// </summary>
	public void EndContainer(GraphicsContainer container)
		=> m_canvas.RestoreToCount(container.m_state);

	/// <summary>
	/// Sends the records in the specified Metafile, one at a time, to a callback method 
	/// for display at a specified point.
	/// </summary>
	public void EnumerateMetafile(object metafile, PointF destPoint, object callback)
		=> EnumerateMetafile(metafile, destPoint, callback, IntPtr.Zero);

	/// <summary>
	/// Sends the records in the specified Metafile, one at a time, to a callback method 
	/// for display at a specified point.
	/// </summary>
	public void EnumerateMetafile(object metafile, PointF destPoint, object callback, IntPtr callbackData)
		=> EnumerateMetafile(metafile, destPoint, callback, callbackData, null);

	/// <summary>
	/// Sends the records in the specified Metafile, one at a time, to a callback method 
	/// for display at a specified point.
	/// </summary>
	public void EnumerateMetafile(object metafile, Point destPoint, object callback)
		=> EnumerateMetafile(metafile, destPoint, callback, IntPtr.Zero);

	/// <summary>
	/// Sends the records in the specified Metafile, one at a time, to a callback method 
	/// for display at a specified point.
	/// </summary>
	public void EnumerateMetafile(object metafile, Point destPoint, object callback, IntPtr callbackData)
		=> EnumerateMetafile(metafile, destPoint, callback, callbackData, null);

	/// <summary>
	/// Sends the records of the specified Metafile, one at a time, to a callback method 
	/// for display in a specified rectangle.
	/// </summary>
	public void EnumerateMetafile(object metafile, RectangleF destRect, object callback)
		=> EnumerateMetafile(metafile, destRect, callback, IntPtr.Zero);

	/// <summary>
	/// Sends the records of the specified Metafile, one at a time, to a callback method 
	/// for display in a specified rectangle.
	/// </summary>
	public void EnumerateMetafile(object metafile, RectangleF destRect, object callback, IntPtr callbackData)
		=> EnumerateMetafile(metafile, destRect, callback, callbackData, null);

	/// <summary>
	/// Sends the records of the specified Metafile, one at a time, to a callback method 
	/// for display in a specified rectangle.
	/// </summary>
	public void EnumerateMetafile(object metafile, Rectangle destRect, object callback)
		=> EnumerateMetafile(metafile, destRect, callback, IntPtr.Zero);

	/// <summary>
	/// Sends the records of the specified Metafile, one at a time, to a callback method 
	/// for display in a specified rectangle.
	/// </summary>
	public void EnumerateMetafile(object metafile, Rectangle destRect, object callback, IntPtr callbackData)
		=> EnumerateMetafile(metafile, destRect, callback, callbackData, null);

	/// <summary>
	/// Sends the records in the specified Metafile, one at a time, to a callback method
	/// for display in a specified parallelogram.
	/// </summary>
	public void EnumerateMetafile(object metafile, PointF[] destPoints, object callback)
		=> EnumerateMetafile(metafile, destPoints, callback, IntPtr.Zero);

	/// <summary>
	/// Sends the records in the specified Metafile, one at a time, to a callback method
	/// for display in a specified parallelogram.
	/// </summary>
	public void EnumerateMetafile(object metafile, PointF[] destPoints, object callback, IntPtr callbackData)
		=> EnumerateMetafile(metafile, destPoints, callback, callbackData, null);

	/// <summary>
	/// Sends the records in the specified Metafile, one at a time, to a callback method
	/// for display in a specified parallelogram.
	/// </summary>
	public void EnumerateMetafile(object metafile, Point[] destPoints, object callback)
		=> EnumerateMetafile(metafile, destPoints, callback, IntPtr.Zero);

	/// <summary>
	/// Sends the records in the specified Metafile, one at a time, to a callback method
	/// for display in a specified parallelogram.
	/// </summary>
	public void EnumerateMetafile(object metafile, Point[] destPoints, object callback, IntPtr callbackData)
		=> EnumerateMetafile(metafile, destPoints, callback, callbackData, null);

	/// <summary>
	/// Sends the records in a selected rectangle from a Metafile, one at a time, to a callback 
	/// method for display at a specified point.
	/// </summary>
	public void EnumerateMetafile(object metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, object callback)
		=> EnumerateMetafile(metafile, destPoint, srcRect, srcUnit, callback, IntPtr.Zero);

	/// <summary>
	/// Sends the records in a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display at a specified point.
	/// </summary>
	public void EnumerateMetafile(object metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, object callback, IntPtr callbackData)
		=> EnumerateMetafile(metafile, destPoint, srcRect, srcUnit, callback, callbackData, null);

	/// <summary>
	/// Sends the records in a selected rectangle from a Metafile, one at a time, to a callback 
	/// method for display at a specified point.
	/// </summary>
	public void EnumerateMetafile(object metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, object callback)
		=> EnumerateMetafile(metafile, destPoint, srcRect, srcUnit, callback, IntPtr.Zero);

	/// <summary>
	/// Sends the records in a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display at a specified point.
	/// </summary>
	public void EnumerateMetafile(object metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, object callback, IntPtr callbackData)
		=> EnumerateMetafile(metafile, destPoint, srcRect, srcUnit, callback, callbackData, null);

	/// <summary>
	/// Sends the records of a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display in a specified rectangle.
	/// </summary>
	public void EnumerateMetafile(object metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, object callback)
		=> EnumerateMetafile(metafile, destRect, srcRect, srcUnit, callback, IntPtr.Zero);

	/// <summary>
	/// Sends the records of a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display in a specified rectangle.
	/// </summary>
	public void EnumerateMetafile(object metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, object callback, IntPtr callbackData)
		=> EnumerateMetafile(metafile, destRect, srcRect, srcUnit, callback, callbackData, null);

	/// <summary>
	/// Sends the records of a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display in a specified rectangle.
	/// </summary>
	public void EnumerateMetafile(object metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, object callback)
		=> EnumerateMetafile(metafile, destRect, srcRect, srcUnit, callback, IntPtr.Zero);

	/// <summary>
	/// Sends the records of a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display in a specified rectangle.
	/// </summary>
	public void EnumerateMetafile(object metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, object callback, IntPtr callbackData)
		=> EnumerateMetafile(metafile, destRect, srcRect, srcUnit, callback, callbackData, null);

	/// <summary>
	/// Sends the records in a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display in a specified parallelogram.
	/// </summary>
	public void EnumerateMetafile(object metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, object callback)
		=> EnumerateMetafile(metafile, destPoints, srcRect, srcUnit, callback, IntPtr.Zero);

	/// <summary>
	/// Sends the records in a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display in a specified parallelogram.
	/// </summary>
	public void EnumerateMetafile(object metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, object callback, IntPtr callbackData)
		=> EnumerateMetafile(metafile, destPoints, srcRect, srcUnit, callback, callbackData, null);

	/// <summary>
	/// Sends the records in a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display in a specified parallelogram.
	/// </summary>
	public void EnumerateMetafile(object metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, object callback)
		=> EnumerateMetafile(metafile, destPoints, srcRect, srcUnit, callback, IntPtr.Zero);

	/// <summary>
	/// Sends the records in a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display in a specified parallelogram.
	/// </summary>
	public void EnumerateMetafile(object metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, object callback, IntPtr callbackData)
		=> EnumerateMetafile(metafile, destPoints, srcRect, srcUnit, callback, callbackData, null);

	/// <summary>
	/// Sends the records in a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display at a specified point using specified image attributes.
	/// </summary>
	public void EnumerateMetafile(object metafile, PointF destPoint, object callback, IntPtr callbackData, object imageAttr)
		=> EnumerateMetafile(metafile, new[] { destPoint }, callback, callbackData, imageAttr);

	/// <summary>
	/// Sends the records in a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display at a specified point using specified image attributes.
	/// </summary>
	public void EnumerateMetafile(object metafile, Point destPoint, object callback, IntPtr callbackData, object imageAttr)
		=> EnumerateMetafile(metafile, new[] { destPoint }, callback, callbackData, imageAttr);

	/// <summary>
	/// Sends the records of a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display in a specified rectangle using specified image attributes.
	/// </summary>
	public void EnumerateMetafile(object metafile, RectangleF destRect, object callback, IntPtr callbackData, object imageAttr)
		=> EnumerateMetafile(metafile, destRect.Points, callback, callbackData, imageAttr);

	/// <summary>
	/// Sends the records of a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display in a specified rectangle using specified image attributes.
	/// </summary>
	public void EnumerateMetafile(object metafile, Rectangle destRect, object callback, IntPtr callbackData, object imageAttr)
		=> EnumerateMetafile(metafile, destRect.Points, callback, callbackData, imageAttr);

	/// <summary>
	/// Sends the records in a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display in a specified parallelogram using specified image attributes.
	/// </summary>
	public void EnumerateMetafile(object metafile, PointF[] destPoints, object callback, IntPtr callbackData, object imageAttr)
		=> EnumerateMetafile(metafile, destPoints, RectangleF.Empty, GraphicsUnit.Pixel, callback, callbackData, imageAttr);

	/// <summary>
	/// Sends the records in a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display in a specified parallelogram using specified image attributes.
	/// </summary>
	public void EnumerateMetafile(object metafile, Point[] destPoints, object callback, IntPtr callbackData, object imageAttr)
		=> EnumerateMetafile(metafile, destPoints, Rectangle.Empty, GraphicsUnit.Pixel, callback, callbackData, imageAttr);

	/// <summary>
	/// Sends the records in a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display at a specified point using specified image attributes.
	/// </summary>
	public void EnumerateMetafile(object metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, object callback, IntPtr callbackData, object imageAttr)
		=> EnumerateMetafile(metafile, new[] { destPoint }, srcRect, srcUnit, callback, callbackData, imageAttr);

	/// <summary>
	/// Sends the records in a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display at a specified point using specified image attributes.
	/// </summary>
	public void EnumerateMetafile(object metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, object callback, IntPtr callbackData, object imageAttr)
		=> EnumerateMetafile(metafile, new[] { destPoint }, srcRect, srcUnit, callback, callbackData, imageAttr);

	/// <summary>
	/// Sends the records of a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display in a specified rectangle using specified image attributes.
	/// </summary>
	public void EnumerateMetafile(object metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, object callback, IntPtr callbackData, object imageAttr)
		=> EnumerateMetafile(metafile, destRect.Points, srcRect, srcUnit, callback, callbackData, imageAttr);

	/// <summary>
	/// Sends the records of a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display in a specified rectangle using specified image attributes.
	/// </summary>
	public void EnumerateMetafile(object metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, object callback, IntPtr callbackData, object imageAttr)
		=> EnumerateMetafile(metafile, destRect.Points, srcRect, srcUnit, callback, callbackData, imageAttr);

	/// <summary>
	/// Sends the records in a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display in a specified parallelogram using specified image attributes.
	/// </summary>
	public void EnumerateMetafile(object metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, object callback, IntPtr callbackData, object imageAttr)
		=> throw new NotImplementedException(); // TODO: Implement Metafile, EnumerateMetafileProc, ImageAttributes classes

	/// <summary>
	/// Sends the records in a selected rectangle from a Metafile, one at a time, to a callback
	/// method for display in a specified parallelogram using specified image attributes.
	/// </summary>
	public void EnumerateMetafile(object metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, object callback, IntPtr callbackData, object imageAttr)
		=> EnumerateMetafile(metafile, Array.ConvertAll(destPoints, point => new PointF(point.m_point)), new RectangleF(srcRect.m_rect), srcUnit, callback, callbackData, imageAttr);

	/// <summary>
	///  Updates the clip region of this <see cref="Graphics"/> to exclude the area specified 
	///  by a <see cref="Region"/>.
	/// </summary>
	public void ExcludeClip(Region region)
		=> SetClip(region, CombineMode.Exclude);

	/// <summary>
	///  Updates the clip region of this <see cref="Graphics"/> to exclude the area specified 
	///  by a <see cref="Rectangle"/> structure.
	/// </summary>
	public void ExcludeClip(Rectangle rect)
		=> ExcludeClip(new Region(rect));

	/// <summary>
	///  Fills the interior of a closed cardinal spline curve defined by an array 
	///  of <see cref="PointF"/> structures using the specified fill mode and tension
	/// </summary>
	public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillMode = FillMode.Alternate, float tension = 0.5f)
	{
		using var path = GetCurvePath(points, fillMode, tension, true);
		FillPath(brush, path);
	}

	/// <summary>
	///  Fills the interior of a closed cardinal spline curve defined by an array 
	///  of <see cref="Point"/> structures using the specified fill mode and tension
	/// </summary>
	public void FillClosedCurve(Brush brush, Point[] points, FillMode fillMode = FillMode.Alternate, float tension = 0.5f)
		=> FillClosedCurve(brush, Array.ConvertAll(points, point => new PointF(point.m_point)));

	/// <summary>
	///  Fills the interior of an ellipse defined by a bounding rectangle specified 
	///  by a pair of coordinates, a width, and a height.
	/// </summary>
	public void FillEllipse(Brush brush, float x, float y, float width, float height)
		=> FillEllipse(brush, new RectangleF(x, y, width, height));

	/// <summary>
	///  Fills the interior of an ellipse defined by a bounding rectangle specified 
	///  by a pair of coordinates, a width, and a height.
	/// </summary>
	public void FillEllipse(Brush brush, int x, int y, int width, int height)
		=> FillEllipse(brush, new Rectangle(x, y, width, height));

	/// <summary>
	///  Fills the interior of an ellipse defined by a bounding <see cref="RectangleF"/> 
	///  specified by a Rectangle structure.
	/// </summary>
	public void FillEllipse(Brush brush, RectangleF rect)
	{
		using var path = GetEllipsePath(rect);
		FillPath(brush, path);
	}

	/// <summary>
	///  Fills the interior of an ellipse defined by a bounding <see cref="Rectangle"/> 
	///  specified by a Rectangle structure.
	/// </summary>
	public void FillEllipse(Brush brush, Rectangle rect)
		=> FillEllipse(brush, new RectangleF(rect.m_rect));

	/// <summary>
	///  Fills the interior of a <see cref='GraphicsPath'/>.
	/// </summary>
	public void FillPath(Brush brush, GraphicsPath path)
		=> PaintPath(path.m_path, brush.m_paint);

	/// <summary>
	///  Fills the interior of a pie section defined by an ellipse specified by 
	///  a <see cref="RectangleF"/> structure and two radial lines.
	/// </summary>
	public void FillPie(Brush brush, RectangleF oval, float startAngle, float sweepAngle)
	{
		using var path = GetPiePath(oval, startAngle, sweepAngle);
		FillPath(brush, path);
	}

	/// <summary>
	///  Fills the interior of a pie section defined by an ellipse specified by 
	///  a <see cref="Rectangle"/> structure and two radial lines.
	/// </summary>
	public void FillPie(Brush brush, Rectangle oval, float startAngle, float sweepAngle)
		=> FillPie(brush, new RectangleF(oval.m_rect), startAngle, sweepAngle);

	/// <summary>
	///  Fills the interior of a pie section defined by an ellipse specified by a 
	///  pair of coordinates, a width, a height, and two radial lines.
	/// </summary>
	public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		=> FillPie(brush, new RectangleF(x, y, width, height), startAngle, sweepAngle);

	/// <summary>
	///  Fills the interior of a pie section defined by an ellipse specified by a 
	///  pair of coordinates, a width, a height, and two radial lines.
	/// </summary>
	public void FillPie(Brush brush, int x, int y, int width, int height, float startAngle, float sweepAngle)
		=> FillPie(brush, new Rectangle(x, y, width, height), startAngle, sweepAngle);

	/// <summary>
	///  Fills the interior of a polygon defined by an array of <see cref="PointF"/> structures 
	///  and optionally using the specified fill mode.
	/// </summary>
	public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode = FillMode.Alternate)
	{
		using var path = GetPolygonPath(points, fillMode);
		FillPath(brush, path);
	}

	/// <summary>
	///  Fills the interior of a polygon defined by an array of <see cref="Point"/> structures 
	///  and optionally using the specified fill mode.
	/// </summary>
	public void FillPolygon(Brush brush, Point[] points, FillMode fillMode = FillMode.Alternate)
		=> FillPolygon(brush, Array.ConvertAll(points, point => new PointF(point.m_point)), fillMode);

	/// <summary>
	///  Fills the interior of a rectangle specified by a <see cref="RectangleF"/> structure.
	/// </summary>
	public void FillRectangle(Brush brush, RectangleF rect)
	{
		using var path = GetRectanglePath(rect);
		FillPath(brush, path);
	}

	/// <summary>
	///  Fills the interior of a rectangle specified by a <see cref="Rectangle"/> structure.
	/// </summary>
	public void FillRectangle(Brush brush, Rectangle rect)
		=> FillRectangle(brush, new RectangleF(rect.m_rect));

	/// <summary>
	///  Fills the interior of a rectangle specified by a pair of coordinates, a width, and a height.
	/// </summary>
	public void FillRectangle(Brush brush, float x, float y, float width, float height)
		=> FillRectangle(brush, new RectangleF(x, y, width, height));

	/// <summary>
	///  Fills the interior of a rectangle specified by a pair of coordinates, a width, and a height.
	/// </summary>
	public void FillRectangle(Brush brush, int x, int y, int width, int height)
		=> FillRectangle(brush, new Rectangle(x, y, width, height));

	/// <summary>
	///  Fills the interiors of a series of rectangles specified by <see cref="RectangleF"/> structures.
	/// </summary>
	public void FillRectangles(Brush brush, params RectangleF[] rects)
		=> Array.ForEach(rects, rect => FillRectangle(brush, rect));

	/// <summary>
	///  Fills the interiors of a series of rectangles specified by <see cref="Rectangle"/> structures.
	/// </summary>
	public void FillRectangles(Brush brush, params Rectangle[] rects)
		=> FillRectangles(brush, Array.ConvertAll(rects, rect => new RectangleF(rect.m_rect)));

	/// <summary>
	///  Fills the interior of a <see cref="Region"/>.
	/// </summary>
	public void FillRegion(Brush brush, Region region)
	{
		using var boundaries = region.m_region.GetBoundaryPath();
		using var path = new GraphicsPath(boundaries);
		FillPath(brush, path);
	}

	/// <summary>
	///  Forces execution of all pending graphics operations with the method waiting 
	///  or not waiting, as specified, to return before the operations finish.
	/// </summary>
	public void Flush(FlushIntention intention = FlushIntention.Flush)
	{
		if (intention == FlushIntention.Sync)
			throw new NotSupportedException($"skia unsupported feature (param: {intention})");
		m_canvas.Flush();
	} 

	/// <summary>
	///  Combines current Graphics context with all previous contexts.
	///  When BeginContainer() is called, a copy of the current context is pushed into the GDI+ context stack, it keeps track of the
	///  absolute clipping and transform but reset the public properties so it looks like a brand new context.
	///  When Save() is called, a copy of the current context is also pushed in the GDI+ stack but the public clipping and transform
	///  properties are not reset (cumulative). Consecutive Save context are ignored with the exception of the top one which contains
	///  all previous information.
	///  The return value is an object array where the first element contains the cumulative clip region and the second the cumulative
	///  translate transform matrix.
	/// </summary>
	public object GetContextInfo()
	{
		using var path = new GraphicsPath(m_path);

		using var cumulativeClip = m_canvas.IsClipEmpty ? null : new Region(path);
		var cumulativeTransform = TransformElements;

		// TODO: keep the context tracking when calling to BeginContainer/Save and EndContainer/Restore
		return new object[] { cumulativeClip ?? new Region(), new Matrix(cumulativeTransform) };
	}

	/// <summary>
	///  Gets a handle to the current Windows halftone palette.
	/// </summary>
	public static IntPtr GetHalftonePalette()
		=> throw new NotSupportedException("skia unsupported feature");

	/// <summary>
	///  Gets the handle to the device context associated with this <see cref="Graphics"/>.
	/// </summary>
	public IntPtr GetHdc()
		=> throw new NotSupportedException("skia unsupported feature");

	/// <summary>
	///  Gets the nearest color to the specified <see cref="Drawing.Color"/> structure.
	/// </summary>
	public Color GetNearestColor(Color color)
		=> color; // NOTE: skia does not provides the device color palette to find the nearest color

	/// <summary>
	///  Updates the clip region of this <see cref="Graphics"/> to the intersection
	///  of the current clip region and the specified <see cref="Region"/>.
	/// </summary>
	public void IntersectClip(Region region)
		=> SetClip(region, CombineMode.Intersect); // TODO: implement Region

	/// <summary>
	///  Updates the clip region of this <see cref="Graphics"/> to the intersection
	///  of the current clip region and the specified <see cref="RectangleF"/> structure.
	/// </summary>
	public void IntersectClip(RectangleF rect)
		=> IntersectClip(new Region(rect));

	/// <summary>
	///  Updates the clip region of this <see cref="Graphics"/> to the intersection
	///  of the current clip region and the specified <see cref="Rectangle"/> structure.
	/// </summary>
	public void IntersectClip(Rectangle rect)
		=> IntersectClip(new RectangleF(rect.m_rect));

	/// <summary>
	///  Indicates whether the specified <see cref="PointF"/> structure is contained 
	///  within the visible clip region of this <see cref="Graphics"/>.
	/// </summary>
	public bool IsVisible(PointF point)
		=> m_canvas.LocalClipBounds.Contains(point.m_point);

	/// <summary>
	///  Indicates whether the specified <see cref="Point"/> structure is contained 
	///  within the visible clip region of this <see cref="Graphics"/>.
	/// </summary>
	public bool IsVisible(Point point)
		=> IsVisible(new PointF(point.X, point.Y));

	/// <summary>
	///  Indicates whether the specified <see cref="RectangleF"/> structure is contained 
	///  within the visible clip region of this <see cref="Graphics"/>.
	/// </summary>
	public bool IsVisible(RectangleF rect)
		=> m_canvas.LocalClipBounds.Contains(rect.m_rect);

	/// <summary>
	///  Indicates whether the specified <see cref="Rectangle"/> structure is contained 
	///  within the visible clip region of this <see cref="Graphics"/>.
	/// </summary>
	public bool IsVisible(Rectangle rect)
		=> IsVisible(new RectangleF(rect.X, rect.Y, rect.Width, rect.Height));

	/// <summary>
	///  Indicates whether the point specified by a pair of coordinates structure is contained 
	///  within the visible clip region of this <see cref="Graphics"/>.
	/// </summary>
	public bool IsVisible(int x, int y)
		=> IsVisible(new Point(x, y));

	/// <summary>
	///  Indicates whether the point specified by a pair of coordinates structure is contained 
	///  within the visible clip region of this <see cref="Graphics"/>.
	/// </summary>
	public bool IsVisible(float x, float y)
		=> IsVisible(new PointF(x, y));

	/// <summary>
	///  Indicates whether the specified by a pair of coordinates, a width, and a height is contained 
	///  within the visible clip region of this <see cref="Graphics"/>.
	/// </summary>
	public bool IsVisible(float x, float y, float width, float height)
		=> IsVisible(new RectangleF(x, y, width, height));

	/// <summary>
	///  Indicates whether the specified by a pair of coordinates, a width, and a height is contained 
	///  within the visible clip region of this <see cref="Graphics"/>.
	/// </summary>
	public bool IsVisible(int x, int y, int width, int height)
		=> IsVisible(new Rectangle(x, y, width, height));

	/// <summary>
	///  Gets an array of <see cref="Region"/> objects, each of which bounds a range of character 
	///  positions within the specified string.
	/// </summary>
	public Region[] MeasureCharacterRanges(ReadOnlySpan<char> text, Font font, RectangleF layout, StringFormat format)
		=> MeasureCharacterRanges(new string(text.ToArray()), font, layout, format);

	/// <summary>
	///  Gets an array of <see cref="Region"/> objects, each of which bounds a range of character 
	///  positions within the specified string.
	/// </summary>
	public Region[] MeasureCharacterRanges(string text, Font font, RectangleF layout, StringFormat format)
	{
		// TODO: implement StringFormat
		if (text == null) throw new ArgumentNullException(nameof(text));
		if (font == null) throw new ArgumentNullException(nameof(font));

		var localBounds = new RectangleF(m_canvas.LocalClipBounds);
		var totalBounds = RectangleF.Intersect(localBounds, layout);

		var regions = new List<Region>();
		var lines = text.Split(StringFormat.BREAKLINES, StringSplitOptions.None);

		foreach (var line in lines)
		{
			foreach (var character in line)
			{
				var path = new GraphicsPath();
				path.AddString(character.ToString(), font.FontFamily, (int)font.Style, font.Size, totalBounds, format);

				var charBounds = path.GetBounds();
				var charRegion = new Region(charBounds);
				regions.Add(charRegion);
			}
		}

		return regions.ToArray();
	}

	/// <summary>
	///  Measures the specified string when drawn with the specified <see cref="Font"/> in a <see cref="RectangleF"/> area, 
	///  formatted with the specified <see cref="StringFormat"/>, and the maximum bounding box 
	///  specified by a <see cref="RectangleF"/> structure.
	/// </summary>
	public SizeF MeasureString(string text, Font font, RectangleF layoutArea = default, StringFormat format = null)
		=> MeasureString(text, font, layoutArea.Size, format, out int _, out int _);

	/// <summary>
	///  Measures the specified string when drawn with the specified <see cref="Font"/> in a <see cref="SizeF"/> area, formatted 
	///  with the specified <see cref="StringFormat"/>, and a maximum layout area specified by
	///  a <see cref="SizeF"/> structure.
	/// </summary>
	public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat format = null)
		=> MeasureString(text, font, new RectangleF(default, layoutArea), format);

	/// <summary>
	///  Measures the specified string when drawn with the specified <see cref="Font"/> from an origin <see cref="PointF"/>, formatted 
	///  with the specified <see cref="StringFormat"/>, and the upper-left corner speficied by
	///  a <see cref="PointF"/> structure.
	/// </summary>
	public SizeF MeasureString(string text, Font font, PointF origin, StringFormat format = null)
		=> MeasureString(text, font, new RectangleF(origin, default), format);

	/// <summary>
	///  Measures the specified string when drawn with the specified <see cref="Font"/> and certain width area, formatted
	///  with the specified <see cref="StringFormat"/>, and indicating the maximum width of the string.
	/// </summary>
	public SizeF MeasureString(string text, Font font, int width, StringFormat format = null)
		=> MeasureString(text, font, new SizeF(width, int.MaxValue), format);

	/// <summary>
	///  Measures the specified string when drawn with the specified <see cref="Font"/> in a <see cref="SizeF"/> area, formatted
	///  with the specified <see cref="StringFormat"/>, and returning the <see cref="Size"/> structure,
	///  the numbers of characters in the string, and the number of text lines in the string.
	/// </summary>
	public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat format, out int charsFitted, out int linesFilled)
		=> MeasureStringInternal(new ReadOnlySpan<char>(text.ToArray()), font, new RectangleF(0, 0, layoutArea), format, out charsFitted, out linesFilled);

	/// <summary>
	///  Measures the specified string when drawn with the specified <see cref="Font"/> in a <see cref="RectangleF"/> area, formatted 
	///  with the specified <see cref="StringFormat"/>, and the maximum bounding box 
	///  specified by a <see cref="RectangleF"/> structure.
	/// </summary>
	public SizeF MeasureString(ReadOnlySpan<char> text, Font font, RectangleF layoutArea = default, StringFormat format = null)
		=> MeasureString(new string(text.ToArray()), font, layoutArea, format);

	/// <summary>
	///  Measures the specified string when drawn with the specified <see cref="Font"/> in a <see cref="SizeF"/> area, formatted 
	///  with the specified <see cref="StringFormat"/>, and a maximum layout area specified by
	///  a <see cref="SizeF"/> structure.
	/// </summary>
	public SizeF MeasureString(ReadOnlySpan<char> text, Font font, SizeF layoutArea, StringFormat format = null)
		=> MeasureString(new string (text.ToArray()), font, layoutArea, format);

	/// <summary>
	///  Measures the specified string when drawn with the specified <see cref="Font"/> from an origin <see cref="PointF">, formatted 
	///  with the specified <see cref="StringFormat"/>, and the upper-left corner speficied by
	///  a <see cref="PointF"/> structure.
	/// </summary>
	public SizeF MeasureString(ReadOnlySpan<char> text, Font font, PointF origin, StringFormat format = null)
		=> MeasureString(new string(text.ToArray()), font, origin, format);

	/// <summary>
	///  Measures the specified string when drawn with the specified <see cref="Font"/> and certail width area, formatted
	///  with the specified <see cref="StringFormat"/>, and indicating the maximum width of the string.
	/// </summary>
	public SizeF MeasureString(ReadOnlySpan<char> text, Font font, int width, StringFormat format = null)
		=> MeasureString(new string(text.ToArray()), font, width, format);

	/// <summary>
	///  Measures the specified string when drawn with the specified <see cref="Font"/> in a <see cref="SizeF"/> area, formatted
	///  with the specified <see cref="StringFormat"/>, and returning the <see cref="SizeF"/> structure,
	///  the numbers of characters in the string, and the number of text lines in the string.
	/// </summary>
	public SizeF MeasureString(ReadOnlySpan<char> text, Font font, SizeF layoutArea, StringFormat format, out int charsFitted, out int linesFilled)
			=> MeasureString(new string(text.ToArray()), font, layoutArea, format, out charsFitted, out linesFilled);

	/// <summary>
	///  Measures the size of a string when drawn with the specified <see cref="Font"/> in a <see cref="RectangleF"/> area, 
	///  formatted with the specified <see cref="StringFormat"/>, and returning the <see cref="SizeF"/> structure,
	///  the numbers of characters in the string, and the number of text lines in the string.
	/// </summary>
	public SizeF MeasureStringInternal(ReadOnlySpan<char> text, Font font, RectangleF layoutArea, StringFormat format, out int charsFitted, out int linesFilled)
		=> MeasureStringInternal(new string(text.ToArray()), font, layoutArea, format, out charsFitted, out linesFilled);

	/// <summary>
	///  Measures the size of a string when drawn with the specified <see cref="Font"/> in a <see cref="RectangleF"/> area, 
	///  formatted with the specified <see cref="StringFormat"/>, and returning the <see cref="SizeF"/> structure,
	///  the numbers of characters in the string, and the number of text lines in the string.
	/// </summary>
	public SizeF MeasureStringInternal(string text, Font font, RectangleF layoutArea, StringFormat format, out int charsFitted, out int linesFilled)
	{
		using var path = new GraphicsPath();
		path.AddString(text, font.FontFamily, (int)font.Style, font.Size, layoutArea, format);

		var bounds = path.GetBounds();

		var charWidth = bounds.Width / text.Split(StringFormat.BREAKLINES, StringSplitOptions.None).Max(line => line.Length);
		var availableWidth = Math.Min(layoutArea.Width, m_canvas.LocalClipBounds.Width);
		charsFitted = (int)(availableWidth / charWidth);

		var lineHeight = bounds.Height;
		var availableHeight = Math.Min(layoutArea.Height, m_canvas.LocalClipBounds.Height);
		linesFilled = (int)(availableHeight / lineHeight);

		var width = Math.Min(bounds.Width, availableWidth);
		var height = Math.Min(bounds.Height, availableHeight);

		return new SizeF(width, height);
	}

	/// <summary>
	///  Multiplies the world transformation of this <see cref="Graphics"/> and
	///  speciried the <see cref="Matrix"/> in the specified order.
	/// </summary>
	public void MultiplyTransform(Matrix matrix, MatrixOrder order = MatrixOrder.Prepend)
	{
		var result = new Matrix(Transform.m_matrix);
		result.Multiply(matrix, order);
		Transform = result;
	}

	/// <summary>
	///  Releases a device context handle obtained by a previous call to 
	///  the <see cref="GetHdc"/> method of this <see cref="Graphics"/>.
	/// </summary>
	public void ReleaseHdc()
		=> ReleaseHdc(IntPtr.Zero);

	/// <summary>
	///  Releases a device context handle obtained by a previous call to 
	///  the <see cref="GetHdc"/> method of this <see cref="Graphics"/>.
	/// </summary>
	public void ReleaseHdc(IntPtr hdc)
		=> throw new NotSupportedException("skia unsupported feature");

	/// <summary>
	///  Releases a handle to a device context.
	/// </summary>
	public void ReleaseHdcInternal(IntPtr hdc)
		=> throw new NotSupportedException("skia unsupported feature");

	/// <summary>
	///  Resets the clip region of this <see cref="Graphics"/> to an infinite region.
	/// </summary>
	public void ResetClip()
	{
		m_context = -1;

		// NOTE: apply a full reset without losing the transformation matrix
		var matrix = m_canvas.TotalMatrix;
		m_canvas.RestoreToCount(m_context);
		m_canvas.SetMatrix(matrix);

		ClipRegion.MakeInfinite();
		SetClip(ClipRegion);
	}

	/// <summary>
	///  Resets the world transformation matrix of this <see cref="Graphics"/> to 
	///  an the identity matrix.
	/// </summary>
	public void ResetTransform()
		=> m_canvas.ResetMatrix();

	/// <summary>
	///  Restores the state of this <see cref="Graphics"/> to the state 
	///  represented by a <see cref="GraphicsState"/>.
	/// </summary>
	public void Restore(GraphicsState state)
		=> m_canvas.RestoreToCount(state.m_index);

	/// <summary>
	///  Applies the specified rotation to the transformation matrix of this <see cref="Graphics"/>.
	/// </summary>
	public void RotateTransform(float angle, MatrixOrder order = MatrixOrder.Prepend)
	{
		var scaleMatrix = SKMatrix.CreateRotationDegrees(-angle);
		if (order == MatrixOrder.Append)
			scaleMatrix = scaleMatrix.PreConcat(m_canvas.TotalMatrix);
		m_canvas.Concat(ref scaleMatrix);
	}

	/// <summary>
	///  Saves the current state of this <see cref="Graphics"/> and 
	///  identifies the saved state with a <see cref="GraphicsState"/>.
	/// </summary>
	public GraphicsState Save()
		=> new(m_canvas.Save());

	/// <summary>
	///  Applies the specified scaling operation to the transformation matrix of 
	///  this <see cref="Graphics"/> by prepending it to the object's transformation matrix.
	/// </summary>
	public void ScaleTransform(float sx, float sy, MatrixOrder order = MatrixOrder.Prepend)
	{
		var scaleMatrix = SKMatrix.CreateScale(sx, sy);
		if (order == MatrixOrder.Append)
			scaleMatrix = scaleMatrix.PreConcat(m_canvas.TotalMatrix);
		m_canvas.Concat(ref scaleMatrix);
	}

	/// <summary>
	///  Sets the clipping region of this <see cref="Graphics"/> to the result
	///  of the specified operation combining the current clip region and the 
	///  specified <see cref="Region"/>.
	/// </summary>
	public void SetClip(Region region, CombineMode combineMode = CombineMode.Replace)
	{
		switch (combineMode)
		{
			case CombineMode.Replace:
				ClipRegion = region;
				break;

			case CombineMode.Union:
				ClipRegion.Union(region);
				break;

			case CombineMode.Intersect: 
				ClipRegion.Intersect(region);
				break;

			case CombineMode.Exclude:
				ClipRegion.Exclude(region);
				break;

			case CombineMode.Complement: 
				ClipRegion.Complement(region);
				break;

			case CombineMode.Xor: 
				ClipRegion.Xor(region);
				break;

			default:
				throw new ArgumentException($"{combineMode} value is not supported", nameof(combineMode));
		}
		m_context = m_canvas.Save();
		m_canvas.ClipRegion(ClipRegion.m_region);
		m_canvas.Clear(ClipColor.m_color);
	}

	/// <summary>
	///  Sets the clipping region of this <see cref="Graphics"/> to the result
	///  of the specified operation combining the current clip region and the 
	///  specified <see cref="Rectangle"/> structure.
	/// </summary>
	public void SetClip(Rectangle rect, CombineMode combineMode = CombineMode.Replace)
		=> SetClip(new Region(rect), combineMode);

	/// <summary>
	///  Sets the clipping region of this <see cref="Graphics"/> to the result 
	///  of the specified combining operation of the current <see cref="Clip"/> region 
	///  and the Clip property of the specified <see cref="Graphics"/>.
	/// </summary>
	public void SetClip(Graphics g, CombineMode combineMode = CombineMode.Replace)
		=> SetClip(g.Clip, combineMode);

	/// <summary>
	///  Sets the clipping region of this <see cref="Graphics"/> to the result 
	///  of the specified operation combining the current clip region and the 
	///  specified <see cref="GraphicsPath"/>.
	/// </summary>
	public void SetClip(GraphicsPath path, CombineMode combineMode = CombineMode.Replace)
		=> SetClip(new Region(path), combineMode);

	/// <summary>
	///  Transforms an array of points from one coordinate space to another using 
	///  the current world and page transformations of this <see cref="Graphics"/>.
	/// </summary>
	public void TransformPoints(CoordinateSpace destination, CoordinateSpace source, PointF[] points)
		=> TransformPoints(destination, source, points, p => p.m_point, p => new(p.X, p.Y));

	/// <summary>
	///  Transforms an array of points from one coordinate space to another using 
	///  the current world and page transformations of this <see cref="Graphics"/>.
	/// </summary>
	public void TransformPoints(CoordinateSpace destination, CoordinateSpace source, Point[] points)
		=> TransformPoints(destination, source, points, p => p.m_point, p => new((int)p.X, (int)p.Y));

	/// <summary>
	///  Translates the clipping region of this <see cref="Graphics"/> by specified 
	///  amounts in the horizontal and vertical directions.
	/// </summary>
	public void TranslateClip(float dx, float dy)
	{
		// NOTE: restore without losing the transformation matrix
		var matrix = m_canvas.TotalMatrix;
		m_canvas.RestoreToCount(m_context);
		m_canvas.SetMatrix(matrix);

		Clip.Translate(dx, dy);
		m_canvas.ClipRegion(Clip.m_region);
		m_canvas.Clear(ClipColor.m_color);
	}

	/// <summary>
	///  Changes the origin of the coordinate system by applying the specified translation to the
	///  transformation matrix of this <see cref="Graphics"/> in the specified order.
	/// </summary>
	public void TranslateTransform(float dx, float dy, MatrixOrder order = MatrixOrder.Prepend)
	{
		var scaleMatrix = SKMatrix.CreateTranslation(dx, dy);
		if (order == MatrixOrder.Append)
			scaleMatrix = scaleMatrix.PreConcat(m_canvas.TotalMatrix);
		m_canvas.Concat(ref scaleMatrix);
	}

	#endregion


	#region Utitlies

	internal static float GetFactor(float dpi, GraphicsUnit sourceUnit, GraphicsUnit targetUnit)
	{
		float sourceFactor = GetPointFactor(sourceUnit, dpi);
		float targetFactor = GetPointFactor(targetUnit, dpi);
		return sourceFactor / targetFactor;

		static float GetPointFactor(GraphicsUnit unit, float dpi) => unit switch
		{
			GraphicsUnit.World => throw new NotSupportedException("World unit conversion is not supported."),
			GraphicsUnit.Display => 72 / dpi, 		// Assuming display unit is pixels
			GraphicsUnit.Pixel => 72 / dpi, 		// 1 pixel = 72 points per inch / Dots Per Inch
			GraphicsUnit.Point => 1, 				// Already in points
			GraphicsUnit.Inch => 72,				// 1 inch = 72 points
			GraphicsUnit.Document => 72 / 300f, 	// 1 document unit = 1/300 inch
			GraphicsUnit.Millimeter => 72 / 25.4f, 	// 1 millimeter = 1/25.4 inch
			_ => throw new ArgumentException("Invalid GraphicsUnit")
		};
	}


	#endregion


	#region Helpers

	private Color ClipColor { get; set; }

	private Region ClipRegion { get; set; }

	private void PaintPath(SKPath path, SKPaint paint)
	{
		m_path.AddPath(path); // used by IsVisible method
		m_canvas.DrawPath(path, paint);
	}

	private static GraphicsPath GetCurvePath(PointF[] points, FillMode fillMode, float tension, bool closed)
	{
		if (points.Length < 3)
			throw new ArgumentException("invalid number of points for drawing a closed curve (at least 3)");

		var path = new GraphicsPath(fillMode);
		if (closed) 
			path.AddClosedCurve(points, tension);
		else 
			path.AddCurve(points, tension);

		return path;
	}

	private static GraphicsPath GetEllipsePath(RectangleF rect)
	{
		var path = new GraphicsPath();
		path.AddEllipse(rect);
		return path;
	}

	private static GraphicsPath GetPiePath(RectangleF rect, float startAngle, float sweepAngle)
	{
		var path = new GraphicsPath();
		path.AddPie(rect, startAngle, sweepAngle);
		return path;
	}

	private static GraphicsPath GetPolygonPath(PointF[] points, FillMode fillMode)
	{
		var path = new GraphicsPath(fillMode);
		path.AddPolygon(points);
		return path;
	}

	private static GraphicsPath GetRectanglePath(RectangleF rect)
	{
		var path = new GraphicsPath();
		path.AddRectangle(rect);
		return path;
	}

	private void TransformPoints<T>(CoordinateSpace destination, CoordinateSpace source, T[] points, Func<T, SKPoint> getPoint, Func<SKPoint, T> newPoint)
	{
		if (source == destination)
			return;

		void ApplyTransform(SKMatrix matrix)
		{
			for (int i = 0; i < points.Length; i++)
			{
				var srcPoint = getPoint(points[i]);
				var dstPoint = matrix.MapPoint(srcPoint);
				points[i] = newPoint(dstPoint);
			}
		}

		// get factors according to page unot and scale
		var factorX = GetFactor(DpiX, PageUnit, GraphicsUnit.Pixel) * PageScale;
		var factorY = GetFactor(DpiY, PageUnit, GraphicsUnit.Pixel) * PageScale;

		var factorMatrix = SKMatrix.CreateScale(factorX, factorY);

		// get the destination and source matrices
		var dstTransMatrix = new SKMatrix(m_canvas.TotalMatrix.Values);
		var dstScaleMatrix = SKMatrix.Concat(dstTransMatrix, factorMatrix);

		var srcTransMatrix = dstTransMatrix.Invert();
		var srcScaleMatrix = dstScaleMatrix.Invert();

		// apply transform based on source
		switch (source)
		{
			case CoordinateSpace.World:
				break;

			case CoordinateSpace.Page:
				ApplyTransform(srcTransMatrix);
				break;

			case CoordinateSpace.Device:
				ApplyTransform(srcScaleMatrix);
				break;

			default:
				throw new ArgumentException($"{source} coordinate space is not supported.", nameof(source));
		}

		// apply transform based on destination
		switch (destination)
		{
			case CoordinateSpace.World:
				break;

			case CoordinateSpace.Page:
				ApplyTransform(dstTransMatrix);
				break;

			case CoordinateSpace.Device:
				ApplyTransform(dstScaleMatrix);
				break;

			default:
				throw new ArgumentException($"{destination} coordinate space is not supported.", nameof(destination));
		}
	}

	#endregion
}
