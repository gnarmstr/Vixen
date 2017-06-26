using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Whirlpool
{
	public class WhirlpoolDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("c2c2ced8-a2a8-46f5-a4d4-43441e68c2bc");

		public override ParameterSignature Parameters
		{
			get { return new ParameterSignature(); }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Pixel; }
		}

		public override string TypeName
		{
			get { return EffectName; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof(Whirlpool); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(WhirlpoolData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string Description
		{
			get { return "Applies a Whirlpool like effect to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Whirlpool"; }
		}
	}
}
