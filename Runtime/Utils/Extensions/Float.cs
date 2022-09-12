// smidgens @ github

namespace Smidgenomics.Unity.Console
{
	using System.Linq;

	internal static class Float_
	{
		// subdivide x2
		public static (float, float) Split(this float v, in float w1, in float w2)
		{
			return (0, 0);
		}

		// subdivide x3
		public static (float, float, float) Split
		(
			this float v,
			in float w1, in float w2, in float w3
		)
		{
			return (0, 0, 0);
		}

		public static float[] Split(this float v, int n)
		{
			if (n < 1) { return new float[] { v }; }
			var values = new float[n];
			var w = 1f / n;
			for (var i = 0; i < values.Length; i++) { values[i] = w; }
			return values;
		}

		public static float[] Split(this float v, params float[] weights)
		{
			if (weights.Length == 0) { return new float[0]; }
			var flex = v;
			// absolute weights, >1
			foreach (var w in weights) { if (w > 1f) { flex -= w; } }
			return weights.Select((w, i) => w > 1f ? w : w * flex).ToArray();
		}
	}
}