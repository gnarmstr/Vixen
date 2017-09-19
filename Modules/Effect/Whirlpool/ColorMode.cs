using System.ComponentModel;

namespace VixenModules.Effect.Whirlpool
{
	public enum ColorMode
	{
		[Description("Normal - Graient Over Time")]
		OverTime,
		[Description("Normal - Gradient Over Element")]
		OverElement,
		[Description("Alternating - Graient Over Time")]
		Alternating
	}
}