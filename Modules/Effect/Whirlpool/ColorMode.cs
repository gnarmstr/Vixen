using System.ComponentModel;

namespace VixenModules.Effect.Whirlpool
{
	public enum ColorMode
	{
		[Description("Normal - Gradient Over Time")]
		OverTime,
		[Description("Normal - Gradient Over Element")]
		OverElement,
		[Description("Alternating - Gradient Over Time")]
		Alternating
	}
}