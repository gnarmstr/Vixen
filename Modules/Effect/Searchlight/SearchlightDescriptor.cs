using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Searchlight
{
	public class SearchlightDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("dbfe43f6-dc3b-4c08-8b7f-b2f24827a5c8");

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
			get { return typeof(Searchlight); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(SearchlightData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string Description
		{
			get { return "Applies Searchlight like effects to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Searchlight"; }
		}
	}
}
