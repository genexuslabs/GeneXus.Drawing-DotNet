namespace GeneXus.Drawing.Drawing2D;

/// <summary>
///  Specifies whether commands in the graphics stack are terminated (flushed) immediately 
///  or executed as soon as possible.
/// </summary>
public enum FlushIntention
{
	/// <summary>
	///  Flush all batched rendering operations.
	/// </summary>
	Flush = 0,

	/// <summary>
	///  Flush all batched rendering operations and wait for them to complete.
	/// </summary>
	Sync = 1
}
