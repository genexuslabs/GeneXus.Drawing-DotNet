
using System;
using System.Linq;
using SkiaSharp;

namespace GeneXus.Drawing.Drawing2D;

public sealed class GraphicsPath : ICloneable, IDisposable
{
	internal readonly SKPath m_path;

	private GraphicsPath(SKPath path)
	{
		m_path = path;
	}

	/// <summary>
	///  Initializes a new instance of the <see cref='GraphicsPath'/> class with 
	///  a <see cref='Drawing.FillMode'/> value of Alternate.
	/// </summary>
	public GraphicsPath()
		: this(FillMode.Alternate) { }

	/// <summary>
	///  Initializes a new instance of the <see cref='GraphicsPath'/> class with 
	///  the <see cref='Drawing.FillMode'/> enumeration.
	/// </summary>
	public GraphicsPath(FillMode mode)
		: this(Array.Empty<Point>(), Array.Empty<byte>(), mode) { }

	/// <summary>
	///  Initializes a new instance of the <see cref='GraphicsPath'/> class with 
	///  the specified <see cref='PathPointType'/> and <see cref='PointF'/> arrays and with the 
	///  specified <see cref='Drawing.FillMode'/> enumeration element.
	/// </summary>
	public GraphicsPath(PointF[] points, byte[] types, FillMode mode = FillMode.Alternate)
		: this(CreatePath(points, types, mode)) { }

	/// <summary>
	///  Initializes a new instance of the <see cref='GraphicsPath'/> class with 
	///  the specified <see cref='PathPointType'/> and <see cref='Point'/> arrays and with the 
	///  specified <see cref='Drawing.FillMode'/> enumeration element.
	/// </summary>
	public GraphicsPath(Point[] points, byte[] types, FillMode mode = FillMode.Alternate)
		: this(Array.ConvertAll(points, point => new PointF(point.m_point)), types, mode) { }

	/// <summary>
	///  Cleans up resources for this <see cref='GraphicsPath'/>.
	/// </summary>
	~GraphicsPath() => Dispose(false);


	#region IDisposable

	/// <summary>
	///  Cleans up resources for this <see cref='GraphicsPath'/>.
	/// </summary>
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		Dispose(true);
	}

	private void Dispose(bool disposing) => m_path.Dispose();

	#endregion


	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='GraphicsPath'/>.
	/// </summary>
	public object Clone()
		=> new GraphicsPath(m_path);

	#endregion


	#region Operators

	/// <summary>
	///  Creates a <see cref='SKPath'/> with the coordinates of the specified <see cref='GraphicsPath'/>.
	/// </summary>
	public static explicit operator SKPath(GraphicsPath path) => path.m_path;

	#endregion


	#region Properties

	/// <summary>
	///  Gets or sets a <see cref='Drawing.FillMode'/> enumeration that determines how the 
	///  interiors of shapes in this <see cref='GraphicsPath'/> are filled.
	/// </summary>
	public FillMode FillMode
	{
		get => m_path.FillType switch
		{ 
			SKPathFillType.EvenOdd => FillMode.Alternate,
			SKPathFillType.Winding => FillMode.Winding,
			_ => throw new NotImplementedException($"value {m_path.FillType}")
		};
		set => m_path.FillType = value switch
		{
			FillMode.Alternate => SKPathFillType.EvenOdd,
			FillMode.Winding => SKPathFillType.Winding,
			_ => throw new NotImplementedException($"value {value}")
		};
	}

	/// <summary>
	///  Gets a <see cref='Drawing.PathData'/> that encapsulates arrays of points and types 
	///  for this <see cref='GraphicsPath'/>.
	/// </summary>
	public PathData PathData => new()
	{
		Points = PathPoints,
		Types = PathTypes
	};

	/// <summary>
	///  Gets the points in the path.
	/// </summary>
	public PointF[] PathPoints => Array.ConvertAll(m_path.Points, point => new PointF(point));

	/// <summary>
	///  Gets the types of the corresponding points in the <see cref='PathPoints'/> array.
	/// </summary>
	public byte[] PathTypes
	{
		get
		{
			using var iterator = m_path.CreateIterator(false);
			byte[] types = new byte[m_path.PointCount];

			int index = 0;
			var points = new SKPoint[4];

			SKPathVerb verb;
			while ((verb = iterator.Next(points)) != SKPathVerb.Done)
			{
				if (verb == SKPathVerb.Close)
				{
					types[index - 1] |= (byte)PathPointType.CloseSubpath;
					continue;
				}

				(int size, byte type) = verb switch
				{
					SKPathVerb.Move  => (1, (byte)PathPointType.Start),
					SKPathVerb.Line  => (2, (byte)PathPointType.Line),
					SKPathVerb.Conic => (3, (byte)PathPointType.Bezier),
					SKPathVerb.Cubic => (3, (byte)PathPointType.Bezier),
					SKPathVerb.Quad  => (4, (byte)PathPointType.Bezier),
					_ => throw new NotImplementedException($"verb {verb}")
				};

				for (int offset = 0; offset < size && index < types.Length; offset++)
					types[index++] = type;
			}

			return types;
		}
	}

	/// <summary>
	///  Gets the number of elements in the <see cref='PathPoints'/> or the <see cref='PathTypes'/> array.
	/// </summary>
	public int PointCount => m_path.PointCount;

	#endregion


	#region Methods

	/// <summary>
	///  Appends an elliptical arc to the current figure bounded
	///  by a rectangle defined by position and size.
	/// </summary>
	public void AddArc(int x, int y, int width, int height, float startAngle, float sweepAngle)
		=> AddArc(new Rectangle(x, y, width, height), startAngle, sweepAngle);

	/// <summary>
	///  Appends an elliptical arc to the current figure bounded 
	///  by a <see cref='Rectangle'/> structure.
	/// </summary>
	public void AddArc(Rectangle rect, float startAngle, float sweepAngle)
		=> AddArc(rect.m_rect, startAngle, sweepAngle);

	/// <summary>
	///  Appends an elliptical arc to the current figure bounded
	///  by a rectangle defined by position and size.
	/// </summary>
	public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
		=> AddArc(new RectangleF(x, y, width, height), startAngle, sweepAngle);

	/// <summary>
	///  Appends an elliptical arc to the current figure bounded 
	///  by a <see cref='Rectangle'/> structure.
	/// </summary>
	public void AddArc(RectangleF rect, float startAngle, float sweepAngle)
		=> AddArc(rect.m_rect, startAngle, sweepAngle);

	/// <summary>
	///  Adds a cubic Bézier curve to the current figure defined 
	///  by 4 points' coordinates.
	/// </summary>
	public void AddBezier(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
		=> AddBezier(new Point(x1, y1), new Point(x2, y2), new Point(x3, y3), new Point(x4, y4));

	/// <summary>
	///  Adds a cubic Bézier curve to the current figure defined 
	///  by 4 <see cref='Point'/> structures.
	/// </summary>
	public void AddBezier(Point pt1, Point pt2, Point pt3, Point pt4)
		=> AddBezier(pt1.m_point, pt2.m_point, pt3.m_point, pt4.m_point);

	/// <summary>
	///  Adds a cubic Bézier curve to the current figure defined 
	///  by 4 points' coordinates.
	/// </summary>
	public void AddBezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		=> AddBezier(new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y3), new PointF(x4, y4));

	/// <summary>
	///  Adds a cubic Bézier curve to the current figure defined 
	///  by 4 <see cref='PointF'/> structures.
	/// </summary>
	public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
		=> AddBezier(pt1.m_point, pt2.m_point, pt3.m_point, pt4.m_point);

	/// <summary>
	///  Adds a sequence of connected cubic Bézier curves defined by an 
	///  array of <see cref='PointF'/> structure to the current figure.
	/// </summary>
	public void AddBeziers(params PointF[] points)
		=> AddBeziers(Array.ConvertAll(points, point => point.m_point));

	/// <summary>
	///  Adds a sequence of connected cubic Bézier curves defined by an 
	///  array of <see cref='Point'/> structure to the current figure.
	/// </summary>
	public void AddBeziers(params Point[] points)
		=> AddBeziers(Array.ConvertAll(points, point => point.m_point));

	/// <summary>
	///  Adds a closed curve to this path defined by a sequence 
	///  of <see cref='PointF'/> structure. A cardinal spline 
	///  curve is used because the curve travels through each 
	///  of the points in the array.
	/// </summary>
	public void AddClosedCurve(params PointF[] points)
		=> AddClosedCurve(points, 0.5f);

	/// <summary>
	///  Adds a closed curve to this path defined by an array 
	///  of <see cref='PointF'/> structure.
	/// </summary>
	public void AddClosedCurve(PointF[] points, float tension = 0.5f)
		=> AddCurve(Array.ConvertAll(points, point => point.m_point), tension, true);

	/// <summary>
	///  Adds a closed curve to this path defined by a sequence 
	///  of <see cref='Point'/> structure. A cardinal spline 
	///  curve is used because the curve travels through each 
	///  of the points in the array.
	/// </summary>
	public void AddClosedCurve(params Point[] points)
		=> AddClosedCurve(points);

	/// <summary>
	///  Adds a closed curve to this path defined by an array 
	///  of <see cref='Point'/> structure.
	/// </summary>
	public void AddClosedCurve(Point[] points, float tension = 0.5f)
		=> AddCurve(Array.ConvertAll(points, point => point.m_point), tension, true);

	/// <summary>
	///  Adds a spline curve to the current figure defined 
	///  by a sequence of <see cref='PointF'/> structures. A 
	///  cardinal spline curve is used because the curve 
	///  travels through each of the points in the array.
	/// </summary>
	public void AddCurve(params PointF[] points)
		=> AddCurve(points, 0.5f);

	/// <summary>
	///  Adds a spline curve to the current figure defined
	///  by an array of <see cref='PointF'/> structure.
	/// </summary>
	public void AddCurve(PointF[] points, float tension = 0.5f)
		=> AddCurve(points, 0, points.Length - 1, tension);

	/// <summary>
	///  Adds a spline curve to the current figure defined
	///  by an array of <see cref='PointF'/> structure but taking
	///  a certain number of segments starting from an offset.
	/// </summary>
	public void AddCurve(PointF[] points, int offset, int numberOfSegments, float tension = 0.5f)
		=> AddCurve(points.Skip(offset).Take(numberOfSegments + 1).Select(p => p.m_point).ToArray(), tension, false);

	/// <summary>
	///  Adds a spline curve to the current figure defined 
	///  by a sequence of <see cref='Point'/> structures. A 
	///  cardinal spline curve is used because the curve 
	///  travels through each of the points in the array.
	/// </summary>
	public void AddCurve(params Point[] points)
		=> AddCurve(points, 0.5f);

	/// <summary>
	///  Adds a spline curve to the current figure defined
	///  by an array of <see cref='Point'/> structure.
	/// </summary>
	public void AddCurve(Point[] points, float tension = 0.5f)
		=> AddCurve(points, 0, points.Length - 1, tension);

	/// <summary>
	///  Adds a spline curve to the current figure defined
	///  by an array of <see cref='Point'/> structure but taking
	///  a certain number of segments starting from an offset.
	/// </summary>
	public void AddCurve(Point[] points, int offset, int numberOfSegments, float tension = 0.5f)
		=> AddCurve(points.Skip(offset).Take(numberOfSegments + 1).Select(p => p.m_point).ToArray(), tension, false);

	/// <summary>
	///  Adds an ellipse to the current path bounded
	///  by a rectangle defined by position and size.
	/// </summary>
	public void AddEllipse(float x, float y, float width, float height)
		=> AddEllipse(new RectangleF(x, y, width, height));

	/// <summary>
	///  Adds an ellipse to the current path bounded by
	///  a <see cref='Rectangle'/> structure.
	/// </summary>
	public void AddEllipse(RectangleF rect)
		=> AddEllipse(rect.m_rect);

	/// <summary>
	///  Adds an ellipse to the current path bounded
	///  by a rectangle defined by position and size.
	/// </summary>
	public void AddEllipse(int x, int y, int width, int height)
		=> AddEllipse(new Rectangle(x, y, width, height));

	/// <summary>
	///   Adds an ellipse to the current path bounded by
	///   a <see cref='Rectangle'/> structure.
	/// </summary>
	public void AddEllipse(Rectangle rect)
		=> AddEllipse(rect.m_rect);

	/// <summary>
	///  Appends a line segment to the current figure defined by
	///  the coordinates of the starting and ending points.
	/// </summary>
	public void AddLine(float x1, float y1, float x2, float y2)
		=> AddLine(new PointF(x1, y1), new PointF(x2, y2));

	/// <summary>
	///  Appends a line segment to the current figure defined by
	///  the starting and ending <see cref='PointF'/> structures. 
	/// </summary>
	public void AddLine(PointF pt1, PointF pt2)
		=> AddLine(pt1.m_point, pt2.m_point);

	/// <summary>
	///  Appends a line segment to the current figure defined by
	///  the coordinates of the starting and ending points.
	/// </summary>
	public void AddLine(int x1, int y1, int x2, int y2)
		=> AddLine(new Point(x1, y1), new Point(x2, y2));

	/// <summary>
	///  Appends a line segment to the current figure defined by
	///  the starting and ending <see cref='Point'/> structures. 
	/// </summary>
	public void AddLine(Point pt1, Point pt2)
		=> AddLine(pt1.m_point, pt2.m_point);

	/// <summary>
	///  Appends a series of connected line segments to the end 
	///  of the current figure defined by a sequence of <see cref='PointF'/> structures.
	/// </summary>
	public void AddLines(params PointF[] points)
		=> Array.ForEach(Enumerable.Range(0, points.Length - 2).ToArray(), i => AddLine(points[i], points[i + 1]));

	/// <summary>
	///  Appends a series of connected line segments to the end 
	///  of the current figure defined by a sequence of <see cref='Point'/> structures.
	/// </summary>
	public void AddLines(params Point[] points)
		=> Array.ForEach(Enumerable.Range(0, points.Length - 2).ToArray(), i => AddLine(points[i], points[i + 1]));

	/// <summary>
	///  Appends the specified <see cref='GraphicsPath'/> to this path.
	/// </summary>
	public void AddPath(GraphicsPath path, bool connect)
		=> m_path.AddPath(path.m_path, connect ? SKPathAddMode.Append : SKPathAddMode.Extend);

	/// <summary>
	///  Adds the outline of a pie shape to this path bounded
	///  by a rectangle defined by position and size.
	/// </summary>
	public void AddPie(float x, float y, float width, float height, float startAngle, float sweepAngle)
		=> AddPie(new RectangleF(x, y, width, height), startAngle, sweepAngle);

	/// <summary>
	///  Adds the outline of a pie shape to this path bounded
	///  by a <see cref='RectangleF'/> structure.
	/// </summary>
	public void AddPie(RectangleF rect, float startAngle, float sweepAngle)
		=> AddPie(rect.m_rect, startAngle, sweepAngle);

	/// <summary>
	///  Adds the outline of a pie shape to this path bounded
	///  by a rectangle defined by position and size.
	/// </summary>
	public void AddPie(int x, int y, int width, int height, float startAngle, float sweepAngle)
		=> AddPie(new Rectangle(x, y, width, height), startAngle, sweepAngle);

	/// <summary>
	///  Adds the outline of a pie shape to this path bounded
	///  by a <see cref='Rectangle'/> structure.
	/// </summary>
	public void AddPie(Rectangle rect, float startAngle, float sweepAngle)
		=> AddPie(rect.m_rect, startAngle, sweepAngle);

	/// <summary>
	///  Adds a polygon to this path defined by an array of <see cref='PointF'/> structures.
	/// </summary>
	public void AddPolygon(PointF[] points)
		=> AddPolygon(Array.ConvertAll(points, point => point.m_point));

	/// <summary>
	///  Adds a polygon to this path defined by an array of <see cref='Point'/> structures.
	/// </summary>
	public void AddPolygon(Point[] points)
		=> AddPolygon(Array.ConvertAll(points, point => point.m_point));

	/// <summary>
	///  Adds a <see cref='RectnalgeF'/> structure to this path.
	/// </summary>
	public void AddRectangle(RectangleF rect)
		=> AddRectangle(rect.m_rect);

	/// <summary>
	///  Adds a <see cref='Rectnalge'/> structure to this path.
	/// </summary>
	public void AddRectangle(Rectangle rect)
		=> AddRectangle(rect.m_rect);

	/// <summary>
	///  Adds a series of <see cref='RectangleF'/> structures to this path.
	/// </summary>
	public void AddRectangles(params RectangleF[] rects)
		=> Array.ForEach(rects, AddRectangle);

	/// <summary>
	///  Adds a series of <see cref='Rectangle'/> structures to this path.
	/// </summary>
	public void AddRectangles(params Rectangle[] rects)
		=> Array.ForEach(rects, AddRectangle);

	/// <summary>
	///  Adds a text string to this path bounded by <see cref='RectangleF'/> structure.
	/// </summary>
	public void AddString(string text, FontFamily family, int style, float emSize, RectangleF layout, StringFormat format)
		=> AddString(text, family, style, emSize, layout.m_rect, format);

	/// <summary>
	///  Adds a text string to this path bounded by <see cref='Rectangle'/> structure.
	/// </summary>
	public void AddString(string text, FontFamily family, int style, float emSize, Rectangle layout, StringFormat format)
		=> AddString(text, family, style, emSize, layout.m_rect, format);

	/// <summary>
	///  Adds a text string to this path starting from a <see cref='PointF'/> structure.
	/// </summary>
	public void AddString(string text, FontFamily family, int style, float emSize, PointF origin, StringFormat format)
		=> AddString(text, family, style, emSize, new RectangleF(origin, float.MaxValue, float.MaxValue), format);

	/// <summary>
	///  Adds a text string to this path starting from a <see cref='Point'/> structure.
	/// </summary>
	public void AddString(string text, FontFamily family, int style, float emSize, Point origin, StringFormat format)
		=> AddString(text, family, style, emSize, new Rectangle(origin, int.MaxValue, int.MaxValue), format);

	/// <summary>
	///  Clears all markers from this path.
	/// </summary>
	public void ClearMarkers()
		=> throw new NotSupportedException("skia unsupported feature");

	/// <summary>
	///  Closes all open figures in this path and starts a new figure. It closes 
	///  each open figure by connecting a line from its endpoint to its starting point.
	/// </summary>
	public void CloseAllFigures()
		=> throw new NotSupportedException("skia unsupported feature");

	/// <summary>
	///  Closes the current figure and starts a new figure. If the current figure 
	///  contains a sequence of connected lines and curves, the method closes the 
	///  loop by connecting a line from the endpoint to the starting point.
	/// </summary>
	public void CloseFigure()
		=> m_path.Close();

	/// <summary>
	///  Converts each curve in this path into a sequence of connected line segments.
	/// </summary>
	public void Flatten()
		=> Flatten(new Matrix());

	/// <summary>
	///  Applies the specified transform and then converts each curve 
	///  in this <see cref='GraphicsPath'/> into a sequence of connected 
	///  line segments.
	/// </summary>
	public void Flatten(Matrix matrix, float flatness = 0.25f)
		=> throw new NotImplementedException("skia unsupported feature");

	/// <summary>
	///  Returns a rectangle that bounds this <see cref='GraphicsPath'/>.
	/// </summary>
	public RectangleF GetBounds()
		=> GetBounds(new Matrix());

	/// <summary>
	///  Returns a rectangle that bounds this <see cref='GraphicsPath'/>
	///  when this path is transformed by the specified <see cref='Matrix'/>.
	/// </summary>
	public RectangleF GetBounds(Matrix matrix)
		=> GetBounds(matrix, new Pen(Color.Transparent, 0));

	/// <summary>
	///  Returns a rectangle that bounds this <see cref='GraphicsPath'/>
	///  when the current path is transformed by the specified <see cref='Matrix'/> and 
	///  drawn with the specified <see cref='Pen'/>.
	/// </summary>
	public RectangleF GetBounds(Matrix matrix, Pen pen)
	{
		using var transformed = new GraphicsPath(m_path);
		transformed.Transform(matrix);
		using var fill = pen.m_paint.GetFillPath(transformed.m_path) ?? transformed.m_path;
		return new RectangleF(fill.Bounds);
	}

	/// <summary>
	///  Gets the last point in the <see cref='PathPoints'/> array of this <see cref='GraphicsPath'/>.
	/// </summary>
	public PointF GetLastPoint()
		=> new(m_path.LastPoint);

	/// <summary>
	///  Indicates whether the specified point is contained within (under) the outline of 
	///  this <see cref='GraphicsPath'/> when drawn with the specified <see cref='Pen'/> and 
	///  using the specified <see cref='Graphics'/>.
	/// </summary>
	public bool IsOutlineVisible(float x, float y, Pen pen, Graphics g = null)
		=> IsOutlineVisible(new PointF(x, y), pen, g);

	/// <summary>
	///  Indicates whether the specified <see cref='PointF'/> structure is contained 
	///  within (under) the outline of this <see cref='GraphicsPath'/> when drawn 
	///  with the specified <see cref='Pen'/> and using the specified <see cref='Graphics'/>.
	/// </summary>
	public bool IsOutlineVisible(PointF point, Pen pen, Graphics g = null)
		=> IsOutlineVisible(point.m_point, pen.m_paint, g?.m_canvas.LocalClipBounds);

	/// <summary>
	///  Indicates whether the specified point is contained within (under) the outline of 
	///  this <see cref='GraphicsPath'/> when drawn with the specified <see cref='Pen'/> and 
	///  using the specified <see cref='Graphics'/>.
	/// </summary>
	public bool IsOutlineVisible(int x, int y, Pen pen, Graphics g = null)
		=> IsOutlineVisible(new Point(x, y), pen, g);

	/// <summary>
	///  Indicates whether the specified <see cref='Point'/> structure is contained 
	///  within (under) the outline of this <see cref='GraphicsPath'/> when drawn 
	///  with the specified <see cref='Pen'/> and using the specified <see cref='Graphics'/>.
	/// </summary>
	public bool IsOutlineVisible(Point point, Pen pen, Graphics g = null)
		=> IsOutlineVisible(point.m_point, pen.m_paint, g?.m_canvas.LocalClipBounds);

	/// <summary>
	///  Indicates whether the specified point's coordinate is contained 
	///  within this <see cref='GraphicsPath'/>.
	/// </summary>
	public bool IsVisible(float x, float y, Graphics g = null)
		=> IsVisible(new PointF(x, y), g);

	/// <summary>
	///  Indicates whether the specified <see cref='PointF'/> structure is 
	///  contained within this <see cref='GraphicsPath'/>.
	/// </summary>
	public bool IsVisible(PointF point, Graphics g = null)
		=> IsVisible(point.m_point, g?.m_canvas.LocalClipBounds);

	/// <summary>
	///  Indicates whether the specified point's coordinate is contained 
	///  within this <see cref='GraphicsPath'/>.
	/// </summary>
	public bool IsVisible(int x, int y, Graphics g = null)
		=> IsVisible(new Point(x, y), g);

	/// <summary>
	///  Indicates whether the specified <see cref='Point'/> structure is 
	///  contained within this <see cref='GraphicsPath'/>.
	/// </summary>
	public bool IsVisible(Point point, Graphics g = null)
		=> IsVisible(point.m_point, g?.m_canvas.LocalClipBounds);

	/// <summary>
	///  Empties the <see cref="PathPoints"/> and <see cref="PathTypes"/> arrays 
	///  and sets the <see cref="Drawing.FillMode"/> to <see cref="FillMode.Alternate"/>.
	/// </summary>
	public void Reset()
		=> m_path.Reset();

	/// <summary>
	///  Reverses the order of points in the <see cref="PathPoints"/> array of this <see cref="GraphicsPath"/>.
	/// </summary>
	public void Reverse()
	{
		var path = new SKPath(m_path);
		m_path.Reset();
		m_path.AddPathReverse(path);
	}

	/// <summary>
	///  Sets a marker on this <see cref="GraphicsPath"/>.
	/// </summary>
	public void SetMarkers()
		=> throw new NotImplementedException();

	/// <summary>
	///  Starts a new figure without closing the current figure. All subsequent 
	///  points added to the path are added to this new figure.
	/// </summary>
	public void StartFigure()
		=> throw new NotImplementedException();

	/// <summary>
	///  Applies a transform matrix to this <see cref="GraphicsPath"/>.
	/// </summary>
	public void Transform(Matrix matrix)
		=> m_path.Transform(matrix.m_matrix);

	/// <summary>
	///  Applies a warp transform to this <see cref="GraphicsPath"/>, defined by a <see cref="Rectangle"/>, 
	///  a parallelogram (serie of <see cref="Point"/> structure), a <see cref="WrapMode"/>, and a flatness 
	///  value for curves.
	/// </summary>
	public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix = null, WarpMode warpMode = WarpMode.Perspective, float flatness = 0.25f)
		=> throw new NotImplementedException();

	/// <summary>
	///  Replaces this <see cref="GraphicsPath"/> with curves that enclose the area that is 
	///  filled when this path is drawn by the specified <see cref="Pen"/> and flatness
	///  value for curves.
	/// </summary>
	public void Widen(Pen pen, Matrix matrix = null, float flatness = 0.25f)
		=> throw new NotImplementedException();

	#endregion


	#region Helpers

	private void AddArc(SKRect rect, float startAngle, float sweepAngle)
		=> m_path.AddArc(rect, startAngle, sweepAngle);


	private void AddBezier(SKPoint pt1, SKPoint pt2, SKPoint pt3, SKPoint pt4)
	{
		if (m_path.LastPoint != pt1)
			m_path.MoveTo(pt1);
		m_path.CubicTo(pt2, pt3, pt4);
	}


	private void AddBeziers(params SKPoint[] points)
	{
		if (points.Length % 4 != 0)
			throw new ArgumentException($"beziers requires points lenght with multiple of 4", nameof(points));
		for (int i = 0; i < points.Length; i += 4)
			AddBezier(points[i], points[i + 1], points[i + 2], points[i + 4]);
	}


	private void AddCurve(SKPoint[] points, float tension, bool closed) // TODO: implement tension
	{
		if (points.Length < 2)
			throw new ArgumentException("At least two points are required", nameof(points));

		tension = Math.Max(0, tension);

		if (m_path.LastPoint != points[0])
			m_path.MoveTo(points[0]);

		if (points.Length == 2)
		{
			m_path.LineTo(points[1]);
			return;
		}

		// calculate and add cubic bézier curves
		for (int i = 0; i < points.Length - 1; i++)
		{
			SKPoint p0 = points[i];
			SKPoint p3 = points[i + 1];

			SKPoint p1, p2;

			if (i == 0)
			{
				// first segment
				p1 = new(p0.X + (closed ? p0.X - p3.X : p3.X - p0.X) * tension / 3, p0.Y + (p3.Y - p0.Y) * tension / 3);
			}
			else
			{
				SKPoint pPrev = points[i - 1];
				p1 = new(p0.X + (p3.X - pPrev.X) * tension / 3, p0.Y + (p3.Y - pPrev.Y) * tension / 3);
			}

			if (i == points.Length - 2)
			{
				// last segment
				p2 = new(p3.X - (closed ? p0.X - p3.X : p3.X - p0.X) * tension / 3, p3.Y - (p3.Y - p0.Y) * tension / 3);
			}
			else
			{
				SKPoint pNext = points[i + 2];
				p2 = new(p3.X - (pNext.X - p0.X) * tension / 3, p3.Y - (pNext.Y - p0.Y) * tension / 3);
			}

			// add cubic bézier curve
			m_path.CubicTo(p1, p2, p3);
		}

		if (closed)
		{
			// Close the path by adding a segment from the last point to the first point
			SKPoint p0 = points[points.Length - 1];
			SKPoint p3 = points[0];

			// Calculate control points for the closing segment
			SKPoint pPrev = points[points.Length - 2];
			SKPoint p1 = new(p0.X - (p0.X - pPrev.X) * tension / 3, p0.Y + (p0.Y - pPrev.Y) * tension / 3);
			SKPoint p2 = new(p3.X - (pPrev.X - p0.X) * tension / 3, p3.Y - (pPrev.Y - p0.Y) * tension / 3);

			// add the closing cubic bézier curve and close path
			m_path.CubicTo(p1, p2, p3);
			m_path.Close();
		}
	}


	private void AddEllipse(SKRect rect)
		=> m_path.AddOval(rect);


	private void AddLine(SKPoint pt1, SKPoint pt2)
	{
		if (m_path.LastPoint != pt1)
			m_path.MoveTo(pt1);
		m_path.LineTo(pt2);
	}


	private void AddPie(SKRect rect, float startAngle, float sweepAngle)
	{
		m_path.ArcTo(rect, startAngle, sweepAngle, false);
		m_path.LineTo(rect.MidX, rect.MidY);
		m_path.Close();
	}


	private void AddPolygon(SKPoint[] points)
	{
		if (points.Length < 3)
			throw new ArgumentException("At least three points are required.");
		m_path.AddPoly(points, true);
	}


	private void AddRectangle(SKRect rect)
		=> m_path.AddRect(rect);


	private void AddString(string text, FontFamily family, int style, float emSize, SKRect layout, StringFormat format)
	{
		format ??= new StringFormat();

		bool isRightToLeft = format.FormatFlags.HasFlag(StringFormatFlags.DirectionRightToLeft);

		using var paint = new SKPaint
		{
			Typeface = family.GetTypeface((FontStyle)style),
			TextSize = emSize,
			TextAlign = format.Alignment switch
			{
				StringAlignment.Near => isRightToLeft ? SKTextAlign.Right : SKTextAlign.Left,
				StringAlignment.Far => isRightToLeft ? SKTextAlign.Left : SKTextAlign.Right,
				StringAlignment.Center => SKTextAlign.Center,
				_ => throw new ArgumentException($"invalid {format.Alignment} text alignment.", nameof(format))
			},
			Style = SKPaintStyle.Stroke
		};

		float baselineHeight = -paint.FontMetrics.Ascent;
		float underlineOffset = paint.FontMetrics.UnderlinePosition ?? 1.8f;
		float underlineHeight = paint.FontMetrics.UnderlineThickness ?? paint.GetTextPath("_", 0, 0).Bounds.Height;
		float paragraphOffset = paint.FontSpacing - baselineHeight - underlineHeight - underlineOffset;

		// define offset based on System.Drawing (based on trial/error comparison)
		float offsetX = isRightToLeft ? 0 : 5, offsetY = baselineHeight - 6;

		// apply format to the string
		text = format.ApplyDirection(text);
		text = format.ApplyTabStops(text);
		text = format.ApplyControlEscape(text);
		text = format.ApplyWrapping(text, layout, paint.MeasureText);
		text = format.ApplyTrimming(text, layout, paint.FontSpacing, paint.MeasureText);
		text = format.ApplyHotkey(text, out var underlines);

		// create returning path
		var path = new SKPath();

		// get path for current text, including breaklines and underlines
		float lineHeightOffset = 0f;
		int lineIndexOffset = 0;
		int breaklineOffset = format.FormatFlags.HasFlag(StringFormatFlags.NoWrap) ? 1 : 0;
		foreach (string line in text.Split(StringFormat.BREAKLINES, StringSplitOptions.None))
		{
			// check if the line fits within the layout height
			if (format.FormatFlags.HasFlag(StringFormatFlags.LineLimit) && lineHeightOffset + baselineHeight > layout.Height)
				break;

			// get text path for current line
			var linePath = paint.GetTextPath(line, 0, lineHeightOffset);

			// Adjust horizontal alignments for right-to-left text
			float rtlOffset = isRightToLeft ? layout.Width - paint.MeasureText(line) - 5 : 0;
			linePath.Offset(rtlOffset, 0);

			// get rect path for each underline defined by hotkey prefix
			foreach (var index in underlines.Where(idx => idx >= lineIndexOffset && idx < lineIndexOffset + line.Length))
			{
				int relIndex = index - lineIndexOffset;
				if (isRightToLeft && relIndex == 0 && line[relIndex] == '…')
					relIndex += 1; // TODO: look for a better fix for this (in rtl)

				float origin = paint.MeasureText(line.Substring(0, relIndex));
				float length = paint.MeasureText(line.Substring(relIndex, 1));
				
				var underline = new SKRect(
					origin + rtlOffset,
					lineHeightOffset + underlineOffset,
					origin + length + rtlOffset,
					lineHeightOffset + underlineOffset + underlineHeight);
				linePath.AddRect(underline);
			}

			// add line path
			path.AddPath(linePath);

			lineIndexOffset += line.Length + breaklineOffset;
			lineHeightOffset += baselineHeight + paragraphOffset;
		}

		// align path vertically
		if (format.LineAlignment == StringAlignment.Center)
			path.Offset(0, (layout.Height - lineHeightOffset) / 2);
		else if (format.LineAlignment == StringAlignment.Far)
			path.Offset(0, layout.Height - lineHeightOffset);

		// apply fit if required
		if (format.FormatFlags.HasFlag(StringFormatFlags.FitBlackBox))
		{
			float fitOffsetX = isRightToLeft
				? Math.Min(0, layout.Right - path.Bounds.Right)
				: Math.Max(0, layout.Left - path.Bounds.Left);
			path.Offset(fitOffsetX, 0);
		}

		// apply offset reltive to layout
		path.Offset(layout.Left + offsetX, layout.Top + offsetY);

		// apply rotation and offset if required
		if (format.FormatFlags.HasFlag(StringFormatFlags.DirectionVertical))
		{
			path.Transform(SKMatrix.CreateRotationDegrees(90));
			path.Offset(path.Bounds.Standardized.Height + paragraphOffset + underlineOffset + underlineHeight, 0);
		}

		// apply clip if required
		if (!format.FormatFlags.HasFlag(StringFormatFlags.NoClip))
		{
			var bounds = new SKPath();
			bounds.AddRect(layout);
			path = path.Op(bounds, SKPathOp.Intersect);
		}

		m_path.AddPath(path);
	}


	private bool IsOutlineVisible(SKPoint point, SKPaint pen, SKRect? bounds)
	{
		bool isBoundContained = bounds?.Contains(point) ?? true;
		var path = pen.GetFillPath(m_path) ?? m_path;
		return isBoundContained && path.Contains(point.X, point.Y);
	}


	private bool IsVisible(SKPoint point, SKRect? bounds)
	{
		bool isBoundContained = bounds?.Contains(point) ?? true;
		return isBoundContained && m_path.Bounds.Contains(point.X, point.Y);
	}

	#endregion


	#region Utilities

	private static SKPath CreatePath(PointF[] points, byte[] types, FillMode mode)
	{
		if (points.Length != types.Length)
			throw new ArgumentException("points and types arrays must have the same length.");

		var path = new SKPath()
		{
			FillType = mode switch
			{
				FillMode.Alternate => SKPathFillType.EvenOdd,
				FillMode.Winding => SKPathFillType.Winding,
				_ => throw new ArgumentException($"invlid value {mode}.", nameof(mode))
			}
		};

		for (int i = 0; i < points.Length; i++)
		{
			var type = types[i] & (byte)PathPointType.PathTypeMask;
			switch ((PathPointType)type)
			{
				case PathPointType.Start:
					path.MoveTo(points[i].m_point);
					break;

				case PathPointType.Line:
					path.LineTo(points[i].m_point);
					break;

				case PathPointType.Bezier:
					if (i + 2 >= points.Length || (PathPointType)types[i + 1] != PathPointType.Bezier || (PathPointType)types[i + 2] != PathPointType.Bezier)
						throw new ArgumentException("Invalid Bezier curve definition.");
					path.CubicTo(points[i].m_point, points[i + 1].m_point, points[i + 2].m_point);
					i += 2;
					break;

				default:
					throw new ArgumentException($"unknown type 0x{type:X2} at index {i}", nameof(types));
			}

			if ((types[i] & (byte)PathPointType.CloseSubpath) == (byte)PathPointType.CloseSubpath)
				path.Close();

			// TODO: consider PathPointType.PathMarker, PathPointType.DashMode
		}

		return path;
	}

	#endregion
}
