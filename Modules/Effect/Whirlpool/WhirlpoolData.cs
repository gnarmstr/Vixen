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
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red)};
			Direction = WhirlpoolDirection.In;
			Speed = 1;
			XOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			YOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			Spacing = 2;
			WidthCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			HeightCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			Thickness = 1;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			BlendCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 70.0, 70.0 }));
			BackgroundLevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 70.0, 70.0 }));
			Orientation = StringOrientation.Vertical;
			GradientMode = GradientMode.OverTime;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public int Spacing { get; set; }

		[DataMember]
		public GradientMode GradientMode { get; set; }

		[DataMember]
		public Curve WidthCurve { get; set; }

		[DataMember]
		public Curve HeightCurve { get; set; }

		[DataMember]
		public int Thickness { get; set; }

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
				Speed = Speed,
				Direction = Direction,
				Spacing = Spacing,
				WidthCurve = new Curve(WidthCurve),
				HeightCurve = new Curve(HeightCurve),
				Thickness = Thickness,
				XOffsetCurve = new Curve(XOffsetCurve),
				YOffsetCurve = new Curve(YOffsetCurve),
				Orientation = Orientation,
				GradientMode = GradientMode,
				BlendCurve = new Curve(BlendCurve),
				LevelCurve = new Curve(LevelCurve),
				BackgroundLevelCurve = new Curve(BackgroundLevelCurve)
			};
			return result;
		}
	}
}
