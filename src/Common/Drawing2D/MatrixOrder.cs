namespace GeneXus.Drawing.Drawing2D;

/// <summary>
///  Specifies the order for matrix transform operations.
/// </summary>
public enum MatrixOrder
{
	/// <summary>
	///  The new operation is applied after the old operation.
	/// </summary>
	Prepend = 0,

	/// <summary>
	///  The new operation is applied before the old operation.
	/// </summary>
	Append = 1
}
