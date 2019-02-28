using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Extensions;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Searchlight
{
	public class Searchlight : PixelEffectBase
	{
		private SearchlightData _data;
		private double _circleCount;
		private int _maxBuffer;
		private int _minBuffer;
		private int _colorIndex;

		public Searchlight()
		{
			_data = new SearchlightData();
			EnableTargetPositioning(true, true);
			UpdateAttributes();
		}

		#region Setup

		[Value]
		public override StringOrientation StringOrientation
		{
			get { return _data.Orientation; }
			set
			{
				_data.Orientation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Config properties

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"CircleFill")]
		[ProviderDescription(@"CircleFill")]
		[PropertyOrder(1)]
		public SearchlightFill SearchlightFill
		{
			get { return _data.SearchlightFill; }
			set
			{
				_data.SearchlightFill = value;
				UpdateColorAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"RadialDirection")]
		[ProviderDescription(@"RadialDirection")]
		[PropertyOrder(2)]
		public SearchlightRadialDirection SearchlightRadialDirection
		{
			get { return _data.SearchlightRadialDirection; }
			set
			{
				_data.SearchlightRadialDirection = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"RadialSpeed")]
		[ProviderDescription(@"RadialSpeed")]
		[PropertyOrder(3)]
		public Curve CenterSpeedCurve
		{
			get { return _data.CenterSpeedCurve; }
			set
			{
				_data.CenterSpeedCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Size")]
		[ProviderDescription(@"Size")]
		[PropertyOrder(5)]
		public Curve SizeCurve
		{
			get { return _data.SizeCurve; }
			set
			{
				_data.SizeCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"CircleCount")]
		[ProviderDescription(@"CircleCount")]
		[PropertyOrder(6)]
		public Curve CircleCountCurve
		{
			get { return _data.CircleCountCurve; }
			set
			{
				_data.CircleCountCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"CircleEdgeWidth")]
		[ProviderDescription(@"CircleEdgeWidth")]
		[PropertyOrder(6)]
		public Curve CircleEdgeWidthCurve
		{
			get { return _data.CircleEdgeWidthCurve; }
			set
			{
				_data.CircleEdgeWidthCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Movement

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"HorizontalOffset")]
		[ProviderDescription(@"HorizontalOffset")]
		[PropertyOrder(1)]
		public Curve XOffsetCurve
		{
			get { return _data.XOffsetCurve; }
			set
			{
				_data.XOffsetCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"VerticalOffset")]
		[ProviderDescription(@"VerticalOffset")]
		[PropertyOrder(2)]
		public Curve YOffsetCurve
		{
			get { return _data.YOffsetCurve; }
			set
			{
				_data.YOffsetCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

#endregion

		#region Color properties

		[Value]
		[ProviderCategory(@"Color", 3)]
		[ProviderDisplayName(@"ColorGradients")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(1)]
		public List<ColorGradient> Colors
		{
			get { return _data.Colors; }
			set
			{
				_data.Colors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Level properties

		[Value]
		[ProviderCategory(@"Brightness", 4)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
		public Curve LevelCurve
		{
			get { return _data.LevelCurve; }
			set
			{
				_data.LevelCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Update Attributes

		private void UpdateAttributes()
		{
			UpdateColorAttribute(false);
			UpdateStringOrientationAttributes();
			TypeDescriptor.Refresh(this);
		}

		private void UpdateColorAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(4);
			{
				propertyStates.Add("CircleEdgeWidthCurve", SearchlightFill == SearchlightFill.Empty);
			}
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		#endregion

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/circles/"; }
		}

		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as SearchlightData;
				UpdateAttributes();
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		protected override void SetupRender()
		{
			_maxBuffer = Math.Max(BufferHt - 1, BufferWi - 1);
			_minBuffer = Math.Min(BufferHt - 1, BufferWi - 1);
		}

		protected override void CleanUpRender()
		{
			//Nothing to do
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			//double radius = radius1 / 2 / _circleCount;
			//double currentRadius = radius;
			//double barht = _maxBuffer / _circleCount;
			//if (SearchlightFill == SearchlightFill.Empty || SearchlightFill == SearchlightFill.Fade) barht /= _circleCount;
			//if (barht < 1) barht = 1;
			//double blockHt = Colors.Count * barht;
			//double foffset = frame * CalculateCenterSpeed(intervalPosFactor) / 4 % (blockHt + 1);
			//barht = barht > 0 ? barht : 1;

			//_circleCount = CalculateCircleCount(intervalPosFactor);
			//if (SearchlightFill == SearchlightFill.Fade || SearchlightFill == SearchlightFill.Empty) _circleCount /= 5;

			double intervalPos = GetEffectTimeIntervalPosition(frame);
			double intervalPosFactor = intervalPos * 100;
			int size = CalculateRadialSize(intervalPosFactor);
			List<Point> minPoints = new List<Point>();
			List<Point> maxPoints = new List<Point>();
			int distance = CalculateYOffset(intervalPosFactor);
			int xLocation = CalculateXOffset(intervalPosFactor);

			Point lightPosition = new Point(BufferWi / 2, 0);

			var line = new Line(lightPosition, new Point(xLocation, distance));
			minPoints.AddRange(line.getPoints((int) DistanceFromPoint(lightPosition, new Point(xLocation, distance))));

			//line = new Line(lightPosition, new Point(xLocation + size, distance));
			//maxPoints.AddRange(line.getPoints((int)DistanceFromPoint(lightPosition, new Point(xLocation + size, distance))));

			for (int y = 0; y < BufferHt; y++)
			{
				for (int x = 0; x < BufferWi; x++)
			{
					CalculatePixel(x, y, frameBuffer, intervalPosFactor, intervalPos, minPoints, maxPoints, distance, size);
				}
			}
		}
		
		private void CalculatePixel(int x, int y, IPixelFrameBuffer frameBuffer, double intervalPosFactor,
			double intervalPos, List<Point> minPoints, List<Point>  maxPoints, int distance, int size)
		{
			int yCoord = y;
			int xCoord = x;
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				//Flip me over so and offset my coordinates I can act like the string version
				y = Math.Abs((BufferHtOffset - y) + (BufferHt - 1 + BufferHtOffset));
				y = y - BufferHtOffset;
				x = x - BufferWiOffset;
			}

			Color color = Colors[0].GetColorAt(intervalPos);
			//int minCount = Math.Min(minPoints.Count, maxPoints.Count);
			for (var index = 0; index < minPoints.Count; index++)
			{
				var minPoint = minPoints[index];
				//var maxPoint = maxPoints[index];
				//if (minPoint.X != x || minPoint.Y != y) continue;
				int newSize = (int) ((double) size / minPoints.Count * y);
				if (x >= minPoint.X && x <= minPoint.X + newSize && y == minPoint.Y)
				{
					double level = 1;
					if (y <= distance) level = LevelCurve.GetValue((double) 100 / distance * y) / 100;
					if (level < 1 && level > 0.0)
					{
						HSV hsv = HSV.FromRGB(color);
						hsv.V = hsv.V * level;
						color = hsv.ToRGB();
					}

					if (level > 0.0) frameBuffer.SetPixel(xCoord, yCoord, color);
				}
				Point centerPoint = new Point((int) (minPoints.Last().X + (double)size / 2), minPoints.Last().Y);

				double distanceFromCentre = DistanceFromPoint(centerPoint, new Point(x, y));
				if (index == minPoints.Count - 1 && distanceFromCentre < (double)size / 2)
				{
					frameBuffer.SetPixel(xCoord, yCoord, color);
				}
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
			for (int frame = 0; frame < numFrames; frame++)
			{

				frameBuffer.CurrentFrame = frame;


				double intervalPos = GetEffectTimeIntervalPosition(frame);
				double intervalPosFactor = intervalPos * 100;
				int size = CalculateRadialSize(intervalPosFactor);
				List<Point> minPoints = new List<Point>();
				List<Point> maxPoints = new List<Point>();
				int distance = CalculateYOffset(intervalPosFactor);
				int xLocation = CalculateXOffset(intervalPosFactor);

				Point lightPosition = new Point(BufferWi / 2, 0);

				var line = new Line(lightPosition, new Point(xLocation, distance));
				minPoints.AddRange(line.getPoints((int)DistanceFromPoint(lightPosition, new Point(xLocation, distance))));
				foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
				{
					foreach (var elementLocation in elementLocations)
					{
						CalculatePixel(elementLocation.X, elementLocation.Y, frameBuffer, intervalPosFactor, intervalPos, minPoints, maxPoints, distance, size);
					}
				}
			}
		}

		private void CalculatePixel(int x, int y, double level, IPixelFrameBuffer frameBuffer, double intervalPosFactor, double intervalPos, double radius, double radius1, double currentRadius, double foffset, double barht, double blockHt)
		{
			int yCoord = y;
			int xCoord = x;
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				//Flip me over so and offset my coordinates I can act like the string version
				y = Math.Abs((BufferHtOffset - y) + (BufferHt - 1 + BufferHtOffset));
				y = y - BufferHtOffset;
				x = x - BufferWiOffset;
			}

			//This saves going through all X and Y locations significantly reducing render times.
			if ((y >= ((BufferWi - 1) / 2) + radius1 + 1 || y <= ((BufferWi - 1) / 2) - radius1) && (x >= ((BufferWi - 1) / 2) + radius1 + 1 || x <= ((BufferWi - 1) / 2) - radius1)) return;

			double distanceFromBallCenter = DistanceFromPoint(new Point((BufferWi - 1) / 2, (BufferHt - 1) / 2), x + CalculateXOffset(intervalPosFactor), y + CalculateYOffset(intervalPosFactor));

			int distance = distanceFromBallCenter > 1.4 && distanceFromBallCenter < 1.51
				? 2
				: (int) Math.Round(distanceFromBallCenter);

			switch (SearchlightRadialDirection)
			{
				case SearchlightRadialDirection.In:
					for (int i = (int)_circleCount; i >= 0; i--)
					{
						SetFramePixel(i, foffset, blockHt, barht, intervalPos, intervalPosFactor, level, frameBuffer, xCoord, yCoord,
							distance, radius, currentRadius, radius1);
						radius = radius + currentRadius;
					}
					break;
				case SearchlightRadialDirection.Out:
					for (int i = 0; i <= _circleCount; i++)
					{
						SetFramePixel(i, foffset, blockHt, barht, intervalPos, intervalPosFactor, level, frameBuffer, xCoord, yCoord,
							distance, radius, currentRadius, radius1);
						radius = radius + currentRadius;
					}
					break;
			}
		}

		private void SetFramePixel(int i, double foffset, double blockHt, double barht, double intervalPos, double intervalPosFactor, double level, IPixelFrameBuffer frameBuffer, int xCoord, int yCoord, int distance, double radius, double currentRadius, double radius1)
		{
			if (distance <= radius && distance >= radius - currentRadius)
			{
				double n = i - foffset + blockHt;

				_colorIndex = (int) ((n)%blockHt/barht);
				Color color = Colors[_colorIndex].GetColorAt(intervalPos);
				switch (SearchlightFill)
				{
					case SearchlightFill.GradientOverElement: //Gradient over Element
						color = Colors[_colorIndex].GetColorAt((1 / radius1) * radius);
						break;
					case SearchlightFill.Empty:
						if (!(distance >= radius - CalculateEdgeWidth(intervalPosFactor, currentRadius))) return;
						break;
					case SearchlightFill.Fade:
						HSV hsv = HSV.FromRGB(color);
						hsv.V *= 1.0 - distance/radius * level;
						frameBuffer.SetPixel(xCoord, yCoord, hsv);
						return;
				}
				if (level < 1)
				{
					HSV hsv = HSV.FromRGB(color);
					hsv.V = hsv.V * level;
					color = hsv.ToRGB();
				}
				frameBuffer.SetPixel(xCoord, yCoord, color);
			}
		}
		public class Line
		{
			public Point p1, p2;

			public Line(Point p1, Point p2)
			{
				this.p1 = p1;
				this.p2 = p2;
			}

			public List<Point> getPoints(int quantity)
			{
				var points = new List<Point>();
				int ydiff = p2.Y - p1.Y, xdiff = p2.X - p1.X;
				double slope = (double)(p2.Y - p1.Y) / (p2.X - p1.X);
				double x, y;

				--quantity;

				for (double i = 0; i < quantity; i++)
				{
					y = slope == 0 ? 0 : ydiff * (i / quantity);
					x = slope == 0 ? xdiff * (i / quantity) : y / slope;
					points.Add(new Point((int)Math.Round(x) + p1.X, (int)Math.Round(y) + p1.Y));
				}

				points.Add(p2);
				return points;
			}
		}

		private double CalculateCenterSpeed(double intervalPos)
		{
			return ScaleCurveToValue(CenterSpeedCurve.GetValue(intervalPos), 10, 0);
		}

		private int CalculateRadialSize(double intervalPos)
		{
			return (int)ScaleCurveToValue(SizeCurve.GetValue(intervalPos), (double)_minBuffer / 2, 1);
		}

		private double CalculateEdgeWidth(double intervalPosFactor, double currentRadius)
		{
			double value = ScaleCurveToValue(CircleEdgeWidthCurve.GetValue(intervalPosFactor), currentRadius, 1);
			if (value < 1) value = 1;
			return value;
		}

		private double CalculateCircleCount(double intervalPosFactor)
		{
			double value = (int)ScaleCurveToValue(CircleCountCurve.GetValue(intervalPosFactor), _maxBuffer / (double)2, 1);
			if (value < 1) value = 1;
			return value;
		}

		private int CalculateXOffset(double intervalPos)
		{
			return (int)ScaleCurveToValue(XOffsetCurve.GetValue(intervalPos), BufferWi, 0);
		}

		private int CalculateYOffset(double intervalPos)
		{
			return (int)ScaleCurveToValue(YOffsetCurve.GetValue(intervalPos), BufferHt, 0);
		}
	}
}
