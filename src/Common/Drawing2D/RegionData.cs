namespace GeneXus.Drawing.Drawing2D;

public sealed class RegionData
{
	internal RegionData(byte[] data) => Data = data;

	/// <summary>
	///  Gets or sets an array of bytes that specify the <see cref='RegionData'/> object.
	/// </summary>
	public byte[] Data { get; set; }
}
