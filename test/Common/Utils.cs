using System;
using System.IO;

namespace GeneXus.Drawing.Test.Drawing2D;

internal abstract class Utils
{
	private static readonly string IMAGE_PATH = Path.Combine(
		Directory.GetParent(Environment.CurrentDirectory).Parent.FullName,
		"res", "images");

	public static double DeltaE(Color color1, Color color2)
    {
        var lab1 = RgbToLab(color1);
        var lab2 = RgbToLab(color2);

		// human eye cannot distinguish colors below 1 DeltaE
		return DistanceCie76(lab1, lab2, color1.A / 255.0, color2.A / 255.0);
    }

    private static double[] RgbToLab(Color color)
    {
        // convert RGB to XYZ
        double r = PivotRgb(color.R / 255.0);
        double g = PivotRgb(color.G / 255.0);
        double b = PivotRgb(color.B / 255.0);

        double x = r * 0.4124564 + g * 0.3575761 + b * 0.1804375;
        double y = r * 0.2126729 + g * 0.7151522 + b * 0.0721750;
        double z = r * 0.0193339 + g * 0.1191920 + b * 0.9503041;

        // convert XYZ to LAB
        double[] xyz = { x / 0.95047, y / 1.00000, z / 1.08883 };
        for (int i = 0; i < 3; i++)
            xyz[i] = PivotXyz(xyz[i]);

        return new[] 
		{ 
			116.0 * xyz[1] - 16, 
			500.0 * (xyz[0] - xyz[1]), 
			200.0 * (xyz[1] - xyz[2])
		};
    }

    private static double PivotRgb(double n)
		=> (n > 0.04045) ? Math.Pow((n + 0.055) / 1.055, 2.4) : n / 12.92;

    private static double PivotXyz(double n)
		=> (n > 0.008856) ? Math.Pow(n, 1.0 / 3.0) : (7.787 * n) + (16.0 / 116.0);

    private static double DistanceCie76(double[] lab1, double[] lab2, double alpha1, double alpha2)
		=> Math.Sqrt(
			Math.Pow(lab2[0] - lab1[0], 2) +
			Math.Pow(lab2[1] - lab1[1], 2) +
			Math.Pow(lab2[2] - lab1[2], 2) +
			Math.Pow(alpha1 - alpha2, 2)
		);

	public static float CompareImage(string filename, Brush brush, bool save = false)
	{
		string filepath = Path.Combine(IMAGE_PATH, filename);
		using var im = Image.FromFile(filepath);
		using var bm = new Bitmap(im);
		
		var gu = GraphicsUnit.Pixel;
		using var bg = new Bitmap(bm.Width, bm.Height);
		using var g = Graphics.FromImage(bg);
		g.FillRectangle(brush, bg.GetBounds(ref gu));

		float hits = 0f; // compare pixel to pixel
		for (int i = 0; i < bg.Width; i++)
			for (int j = 0; j < bg.Height; j++)
				hits += DeltaE(bg.GetPixel(i, j), bm.GetPixel(i, j)) < 10 ? 1 : 0;

		if (save)
		{
			string savepath = Path.Combine(IMAGE_PATH, ".out", filename);

			var dirpath = Path.GetDirectoryName(savepath);
			if (!Directory.Exists(dirpath)) Directory.CreateDirectory(dirpath);

			bg.Save(savepath);
		}

		return hits / (bg.Width * bg.Height);
	}
}