using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using System.Drawing;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Wipe {
	[DataContract]
	public class WipeData : EffectTypeModuleData {
		
		public WipeData() {
			Curve = new Curve(new PointPairList(new[] { 0.0, 50.0, 100.0 }, new[] { 0.0, 100.0, 0.0 }));
			Direction = WipeDirection.Horizontal;
			ColorGradient = new ColorGradient(Color.White);
			PulseTime = 1000;
			PassCount = 1;
			PulsePercent = 33;
			MovementCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 100.0 }));
			WipeMovement = WipeMovement.Count;
			ReverseDirection = false;
			ColorHandling = ColorHandling.GradientThroughWholeEffect;
			WipeOn = false;
			WipeOff = false;
			ColorAcrossItemPerCount = true;
			ReverseColorDirection = true;
			XOffset = 0.0;
			YOffset = 0.0;
			Sensitivity = -70;
			LowPass = false;
			LowPassFreq = 1000;
			HighPass = false;
			HighPassFreq = 500;
			Normalize = true;
			DecayTime = 1500;
			AttackTime = 52;
			Gain = 0;
		}

		[DataMember]
		public ColorHandling ColorHandling { get; set; }

		[DataMember]
		public ColorGradient ColorGradient { get; set; }

		[DataMember]
		public WipeDirection Direction{ get; set; }
		
		[DataMember]
		public Curve Curve { get; set; }

		[DataMember]
		public int PulseTime { get; set; }

		[DataMember]
		public int PassCount { get; set; }

		[DataMember]
		public double PulsePercent { get; set; }
		
		[DataMember]
		public bool WipeOn { get; set; }

		[DataMember]
		public bool WipeOff { get; set; }
		
		[DataMember]
		public Curve MovementCurve { get; set; }

		[DataMember]
		public WipeMovement WipeMovement { get; set; }

		[DataMember] 
		public bool ReverseDirection { get; set; }

		[DataMember]
		public bool ColorAcrossItemPerCount { get; set; }

		[DataMember]
		public bool ReverseColorDirection { get; set; }

		[DataMember]
		public double XOffset { get; set; }

		[DataMember]
		public double YOffset { get; set; }

		[DataMember]
		public int DecayTime { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool EnableAudio { get; set; }

		[DataMember]
		public int Gain { get; set; }

		[DataMember]
		public int AttackTime { get; set; }

		[DataMember]
		public int Velocity { get; set; }

		[DataMember]
		public int Sensitivity { get; set; }

		[DataMember]
		public bool LowPass { get; set; }

		[DataMember]
		public int LowPassFreq { get; set; }

		[DataMember]
		public bool HighPass { get; set; }

		[DataMember]
		public int HighPassFreq { get; set; }

		[DataMember]
		public bool Normalize { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			return (WipeData)MemberwiseClone();
		}
	}

	
}
