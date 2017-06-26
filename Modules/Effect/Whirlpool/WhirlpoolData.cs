using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Whirlpool
{
	[DataContract]
	public class WhirlpoolData : EffectTypeModuleData
	{

		public WhirlpoolData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue)};
			BackgroundColor = new ColorGradient(Color.Red);
			Direction = WhirlpoolDirection.In;
			Speed = 1;
			ToggleBlend = true;
			XOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			YOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			SpacingCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			WidthCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			HeightCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			ThicknessCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			BlendCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 70.0, 70.0 }));
			BackgroundLevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 70.0, 70.0 }));
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public ColorGradient BackgroundColor { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public bool ToggleBlend { get; set; }

		[DataMember]
		public Curve SpacingCurve { get; set; }

		[DataMember]
		public Curve WidthCurve { get; set; }

		[DataMember]
		public Curve HeightCurve { get; set; }

		[DataMember]
		public Curve ThicknessCurve { get; set; }

		[DataMember]
		public Curve YOffsetCurve { get; set; }

		[DataMember]
		public Curve XOffsetCurve { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public Curve BlendCurve { get; set; }

		[DataMember]
		public Curve BackgroundLevelCurve { get; set; }

		[DataMember]
		public WhirlpoolDirection Direction { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			WhirlpoolData result = new WhirlpoolData
			{
				Colors = Colors.ToList(),
				BackgroundColor = BackgroundColor,
				Speed = Speed,
				Direction = Direction,
				SpacingCurve = new Curve(BlendCurve),
				WidthCurve = new Curve(WidthCurve),
				HeightCurve = new Curve(HeightCurve),
				ThicknessCurve = new Curve(ThicknessCurve),
				XOffsetCurve = new Curve(XOffsetCurve),
				YOffsetCurve = new Curve(YOffsetCurve),
				ToggleBlend = ToggleBlend,
				Orientation = Orientation,
				BlendCurve = new Curve(BlendCurve),
				LevelCurve = new Curve(LevelCurve),
				BackgroundLevelCurve = new Curve(BackgroundLevelCurve)
			};
			return result;
		}
	}
}
