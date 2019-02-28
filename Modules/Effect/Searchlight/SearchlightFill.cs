using System.ComponentModel;

namespace VixenModules.Effect.Searchlight
{
	public enum SearchlightFill
	{
		[Description("Fade")]
		Fade,
		[Description("Empty")]
		Empty,
		[Description("Gradient over Time")]
		GradientOverTime,
		[Description("Gradient over Element")]
		GradientOverElement
	}
}