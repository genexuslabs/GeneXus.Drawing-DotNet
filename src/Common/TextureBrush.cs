using System;
using SkiaSharp;
using GeneXus.Drawing.Drawing2D;

namespace GeneXus.Drawing;

public sealed class TextureBrush : Brush
{
	internal Bitmap m_bitmap;
	internal Rectangle m_rect;
	internal WrapMode m_mode;

	private TextureBrush(Rectangle rect, Bitmap bitmap, WrapMode mode)  
		: base(new SKPaint { Shader = CreateShader(bitmap, rect, mode) }) 
	{
		m_rect = rect;
		m_bitmap = bitmap;
		m_mode = mode;
	}

	/// <summary>
	///  Initializes a new <see cref='TextureBrush'/> object that uses the 
	///  specified image, wrap mode, and bounding rectangle.
	/// </summary>
	public TextureBrush(Bitmap bitmap, Rectangle rect, WrapMode mode = WrapMode.Tile)
		: this(rect, bitmap, mode) { }

	/// <summary>
	///  Initializes a new <see cref='TextureBrush'/> object that uses the 
	///  specified image and wrap mode.
	/// </summary>
	public TextureBrush(Bitmap bitmap, WrapMode mode = WrapMode.Tile)
		: this(bitmap, new Rectangle(0, 0, bitmap.Size), mode) { }

	/// <summary>
	///  Initializes a new <see cref='TextureBrush'/> object that uses 
	///  the specified image, bounding rectangle, and image attributes.
	/// </summary>
	public TextureBrush(Bitmap bitmap, Rectangle rect, object imageAttributes)
		: this(bitmap, rect, WrapMode.Tile) { } // TODO: implement ImageAttributes class


	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='TextureBrush'/>.
	/// </summary>
	public override object Clone()
		=> new TextureBrush(m_rect, m_bitmap, m_mode);

	#endregion


	#region Properties

	/// <summary>
	///  Gets the <see cref='Drawing.Image'/> object associated with 
	///  this <see cref='TextureBrush'/> object.
	/// </summary>
	public Image Image => m_bitmap;

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
		set => m_mode = value; // TODO: update shader when updating this
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

	private static SKShader CreateShader(Bitmap bitmap, Rectangle rect, WrapMode mode)
	{
		/* 
		 * NOTE: For mapping WrapMode.Clamp
		 * - SKShaderTileMode.Clamp: Replicate the edge color if the shader draws outside of its original bounds
		 * - SKShaderTileMode.Decal: Only draw within the original domain, return transparent-black everywhere else.
		 */
		
		(var tmx, var tmy) = mode switch
		{
			WrapMode.Tile       => (SKShaderTileMode.Repeat, SKShaderTileMode.Repeat),
			WrapMode.Clamp      => (SKShaderTileMode.Decal,  SKShaderTileMode.Decal),
			WrapMode.TileFlipX  => (SKShaderTileMode.Mirror, SKShaderTileMode.Repeat),
			WrapMode.TileFlipY  => (SKShaderTileMode.Repeat, SKShaderTileMode.Mirror),
			WrapMode.TileFlipXY => (SKShaderTileMode.Mirror, SKShaderTileMode.Mirror),
			_ => throw new NotImplementedException()
		};

		var info = new SKImageInfo((int)rect.Width, (int)rect.Height);

		using var surfece = SKSurface.Create(info);
		surfece.Canvas.DrawBitmap(bitmap.m_bitmap, rect.m_rect);
		
		var src = surfece.Snapshot();
		return SKShader.CreateImage(src, tmx, tmy);
	}

	#endregion
}
