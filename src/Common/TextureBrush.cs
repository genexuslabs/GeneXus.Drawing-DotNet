using System;
using SkiaSharp;
using GeneXus.Drawing.Drawing2D;

namespace GeneXus.Drawing;

public sealed class TextureBrush : Brush
{
	internal Image m_image;
	internal RectangleF m_bounds;
	internal WrapMode m_mode;

	private TextureBrush(RectangleF rect, Image image, WrapMode mode)  
		: base(new SKPaint { }) 
	{
		m_bounds = rect;
		m_image = image;
		m_mode = mode;

		UpdateShader(() => { });
	}

	/// <summary>
	///  Initializes a new <see cref='TextureBrush'/> object that uses the 
	///  specified image, wrap mode, and bounding rectangle.
	/// </summary>
	public TextureBrush(Image image, RectangleF rect, WrapMode mode = WrapMode.Tile)
		: this(rect, image, mode) { }

	/// <summary>
	///  Initializes a new <see cref='TextureBrush'/> object that uses the 
	///  specified image, wrap mode, and bounding rectangle.
	/// </summary>
	public TextureBrush(Image image, Rectangle rect, WrapMode mode = WrapMode.Tile)
		: this(new RectangleF(rect.m_rect), image, mode) { }

	/// <summary>
	///  Initializes a new <see cref='TextureBrush'/> object that uses the 
	///  specified image and wrap mode.
	/// </summary>
	public TextureBrush(Image image, WrapMode mode = WrapMode.Tile)
		: this(image, new Rectangle(0, 0, image.Size), mode) { }

	/// <summary>
	///  Initializes a new <see cref='TextureBrush'/> object that uses 
	///  the specified image, bounding rectangle, and image attributes.
	/// </summary>
	public TextureBrush(Image image, RectangleF rect, object imageAttributes)
		: this(image, rect, WrapMode.Tile) { } // TODO: implement ImageAttributes class

	/// <summary>
	///  Initializes a new <see cref='TextureBrush'/> object that uses 
	///  the specified image, bounding rectangle, and image attributes.
	/// </summary>
	public TextureBrush(Image image, Rectangle rect, object imageAttributes)
		: this(image, new RectangleF(rect.m_rect), imageAttributes) { }


	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='TextureBrush'/>.
	/// </summary>
	public override object Clone()
		=> new TextureBrush(m_bounds, m_image, m_mode);

	#endregion


	#region Properties

	/// <summary>
	///  Gets the <see cref='Drawing.Image'/> object associated with 
	///  this <see cref='TextureBrush'/> object.
	/// </summary>
	public Image Image => m_image;

	/// <summary>
	///  Gets or sets a copy of the <see cref='Matrix'/>  object 
	///  that defines a local geometric transformation for the image associated 
	///  with this <see cref='TextureBrush'/>  object.
	/// </summary>
	public Matrix Transform { get; set; } = new Matrix();

	/// <summary>
	///  Gets or sets a <see cref='Drawing.WrapMode'/> enumeration that 
	///  indicates the wrap mode for this <see cref='TextureBrush'/> object.
	/// </summary>
	public WrapMode WrapMode
	{
		get => m_mode;
		set => UpdateShader(() => m_mode = value);
	}

	#endregion


	#region Methods

	/// <summary>
	///  Multiplies the <see cref='Matrix'/> object that represents the local 
	///  geometric transformation of this <see cref='TextureBrush'/> object 
	///  by the specified <see cref='Matrix'/> object in the specified order.
	/// </summary>
	public void MultiplyTransform(Matrix matrix, MatrixOrder order = MatrixOrder.Prepend)
		=> Transform.Multiply(matrix, order);

	/// <summary>
	///  Resets the Transform property of this <see cref='TextureBrush'/> object to identity.
	/// </summary>
	public void ResetTransform()
		=> Transform.Reset();

	/// <summary>
	///  Rotates the local geometric transformation of this <see cref='TextureBrush'/> object 
	///  by the specified amount in the specified order.
	/// </summary>
	public void RotateTransform(float angle, MatrixOrder order = MatrixOrder.Prepend)
		=> Transform.Rotate(angle, order);

	/// <summary>
	///  Scales the local geometric transformation of this <see cref='TextureBrush'/> object 
	///  by the specified amounts in the specified order.
	/// </summary>
	public void ScaleTransform(float sx, float sy, MatrixOrder order = MatrixOrder.Prepend)
		=> Transform.Scale(sx, sy, order);

	/// <summary>
	///  Translates the local geometric transformation of this <see cref='TextureBrush'/> object
	///  by the specified dimensions in the specified order.
	/// </summary>
	public void TranslateTransform(float dx, float dy, MatrixOrder order)
		=> Transform.Translate(dx, dy, order);

	#endregion


	#region Utilities

	private void UpdateShader(Action action)
	{
		action();

		(var tmx, var tmy) = WrapMode switch
		{
			WrapMode.Tile       => (SKShaderTileMode.Repeat, SKShaderTileMode.Repeat),
			WrapMode.Clamp      => (SKShaderTileMode.Decal,  SKShaderTileMode.Decal),
			WrapMode.TileFlipX  => (SKShaderTileMode.Mirror, SKShaderTileMode.Repeat),
			WrapMode.TileFlipY  => (SKShaderTileMode.Repeat, SKShaderTileMode.Mirror),
			WrapMode.TileFlipXY => (SKShaderTileMode.Mirror, SKShaderTileMode.Mirror),
			_ => throw new NotImplementedException()
		};

		var info = new SKImageInfo((int)m_bounds.Width, (int)m_bounds.Height);

		using var surfece = SKSurface.Create(info);
		switch (m_image)
		{
			case Bitmap bitmap:
				surfece.Canvas.DrawBitmap(bitmap.m_bitmap, m_bounds.m_rect);
				break;
			
			case Svg svg:
				surfece.Canvas.DrawImage(svg.InnerImage, m_bounds.m_rect);
				break;

			default:
				throw new NotImplementedException($"image type {m_image.GetType().Name}.");
		}
		
		var src = surfece.Snapshot();

		m_paint.Shader = SKShader.CreateImage(src, tmx, tmy);
	}

	#endregion
}
