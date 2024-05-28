using System;
using System.Collections.Generic;

namespace GeneXus.Drawing.Text;

public abstract class FontCollection : IDisposable
{
	protected List<FontFamily> m_families = new List<FontFamily>();

	/// <summary>
	///  Cleans up resources for this <see cref='FontCollection'/>.
	/// </summary>
	~FontCollection() => Dispose();


	#region IDisposable

	/// <summary>
	///  Disposes of this <see cref='FontCollection'/>
	/// </summary>
	public void Dispose()
	{
		m_families.ForEach(ff => ff.Dispose());
		m_families.Clear();
	}

	#endregion


	#region Propertie

	/// <summary>
	///  Gets the array of <see cref='FontFamily'/> objects associated with this <see cref='FontCollection'/>.
	/// </summary>
	public FontFamily[] Families => m_families.ToArray();

	#endregion
}
