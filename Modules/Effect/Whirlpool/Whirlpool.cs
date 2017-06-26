using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Whirlpool
{
	public class Whirlpool : PixelEffectBase
	{
		private WhirlpoolData _data;
		private int _frame;
		private int _frameCount;
		private double _minBufferSize;

		public Whirlpool()
		{
			_data = new WhirlpoolData();
			EnableTargetPositioning(true, true);
			UpdateAllAttributes();
		}

		#region String Setup properties

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

		#region Movement properties

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		[PropertyOrder(0)]
		public WhirlpoolDirection Direction
		{
			get { return _data.Direction; }
			set
			{
				_data.Direction = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Iterations")]
		[ProviderDescription(@"Iterations")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 10, 1)]
		[PropertyOrder(1)]
		public int Speed
		{
			get { return _data.Speed; }
			set
			{
				_data.Speed = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Spacing")]
		[ProviderDescription(@"Spacing")]
		[PropertyOrder(4)]
		public Curve SpacingCurve
		{
			get { return _data.SpacingCurve; }
			set
			{
				_data.SpacingCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Width")]
		[ProviderDescription(@"Width")]
		[PropertyOrder(2)]
		public Curve WidthCurve
		{
			get { return _data.WidthCurve; }
			set
			{
				_data.WidthCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Height")]
		[ProviderDescription(@"Height")]
		[PropertyOrder(3)]
		public Curve HeightCurve
		{
			get { return _data.HeightCurve; }
			set
			{
				_data.HeightCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Thickness")]
		[ProviderDescription(@"Thickness")]
		[PropertyOrder(5)]
		public Curve ThicknessCurve
		{
			get { return _data.ThicknessCurve; }
			set
			{
				_data.ThicknessCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Vertical Offset")]
		[ProviderDescription(@"Vertical Offset")]
		[PropertyOrder(6)]
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

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Horizontal Offset")]
		[ProviderDescription(@"Horizontal Offset")]
		[PropertyOrder(7)]
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

		#endregion

		#region Color properties

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"Colors")]
		[ProviderDescription(@"Colors")]
		[PropertyOrder(0)]
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

		[Value]
		[ProviderCategory(@"Brightness", 3)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"TextBrightness")]
		[PropertyOrder(0)]
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

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as WhirlpoolData;
				UpdateAllAttributes();
				IsDirty = true;
			}
		}

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/whirlpool/"; }
		}

		#endregion

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		private void UpdateAllAttributes()
		{
			UpdateStringOrientationAttributes();
			TypeDescriptor.Refresh(this);
		}

		private int _bufferHt;
		private int _bufferWi;

		protected override void SetupRender()
		{
			_bufferHt = BufferHt;
			_bufferWi = BufferWi;
			_minBufferSize = Math.Min(_bufferHt, _bufferWi);
		}

		protected override void CleanUpRender()
		{
			//Nothing to do.
		}

		private object a;

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
			var intervalPos = GetEffectTimeIntervalPosition(frame);
			var intervalPosFactor = intervalPos * 100;

			InitialRender(frame, frameBuffer, level);
			
				
		//	HSV hsv = HSV.FromRGB(Colors[1].GetColorAt((intervalPosFactor)/100));
			
			// copy to frameBuffer
			//for (int x = 0; x < BufferWi; x++) //(BufferWi / 2) - (Width / 2) - 1; x < (BufferWi / 2) + (Width / 2) + 1; x++)
			//{
			//	for (int y = 0; y < BufferHt; y++) //(BufferHt / 2) - (Height / 2) - 1; y < (BufferHt / 2) + (Height / 2) + 1; y++)
			//	{
			//if (a[x, y] == Color.Red)
			//	CalculatePixel(x, y, level, frameBuffer, intervalPosFactor);
			//	}
			//}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
			for (int frame = 0; frame < numFrames; frame++)
			{
				var intervalPos = GetEffectTimeIntervalPosition(frame);
				var intervalPosFactor = intervalPos * 100;
				frameBuffer.CurrentFrame = frame;
				double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame)*100)/100;

		//		HSV hsv = HSV.FromRGB(Colors[1].GetColorAt((intervalPosFactor)/100));
				InitialRender(frame, frameBuffer, level);
				//foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
				//{
				//	foreach (var elementLocation in elementLocations)
				//	{
						
				//			CalculatePixel(elementLocation.X, elementLocation.Y, level, frameBuffer, intervalPosFactor);
				//	}
				//}

				//foreach (var elementLocation in frameBuffer.ElementLocations)
				//{
				//	CalculatePixel(elementLocation.X, elementLocation.Y, level, frameBuffer, intervalPosFactor);
				//}
			}

		}

		private void CalculatePixel(int x, int y, double level, IPixelFrameBuffer frameBuffer, double intervalPosFactor)
		{
			int yCoord = y;
			int xCoord = x;
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				//Flip me over so and offset my coordinates I can act like the string version
				y = Math.Abs((BufferHtOffset - y) + (_bufferHt - 1 + BufferHtOffset));
				y = y - BufferHtOffset;
				x = x - BufferWiOffset;
			}

		}

		private void InitialRender(int frame, IPixelFrameBuffer frameBuffer, double level)
		{

			double intervalPos = GetEffectTimeIntervalPosition(frame);
			double intervalPosFactor = intervalPos*100;
			int width = CalculateWidth(intervalPosFactor);
			int height = CalculateHeight(intervalPosFactor);
			int spacing = CalculateSpacing(intervalPosFactor);
			int thickness = CalculateThickness(intervalPosFactor);
			int numberFrames = GetNumberFrames();
			int positionX;
			int positionY;
			int direction;
			int stepsCount;
			int horizontalStepsCount = width;
			int verticalStepsCount = height;
			int stepPosition;
			int thicknessDirection;

			HSV hsv = HSV.FromRGB(Colors[1].GetColorAt((intervalPosFactor)/100));

			if (frame == 0)
			{
				_frame = 1;
				_frameCount = numberFrames/Speed;
			}

			//Setup Initial Values
			if (Direction == WhirlpoolDirection.In)
			{
				//Assuming end location is the middle of the Matrix we need to start at a location half the height and half the width away from middle.
				positionX = (int)(_bufferWi / 2) - (width / 2);
				positionY = (int)(_bufferHt / 2) - (height / 2);

				direction = 1; //Initial Direction is Right as we are starting in he lower left endge of the whirlpool.
				thicknessDirection = 3;
				stepsCount = width; //Performs number of width steps, this will be the opposite side of the whirlpool.
				stepPosition = 2; //Steps already performed.
			}
			else
			{
				//First location is in the middle of the matrix - half the difference between the Width and Height of the whirlpool.
				int maxSize = Math.Max(height, width);
				int minSize = Math.Min(height, width); 
				int steps = (width - height);

				stepPosition = 2;  //Steps already performed.

				if (steps > 0)
				{
					stepsCount = steps; //Initial number of moves.
					positionX = (_bufferWi/2) - (steps/2);
					positionY = (_bufferHt/2);
					direction = 3; //initial Direction is Right.
					thicknessDirection = 3;
					verticalStepsCount = 0; //Ensures subsequent Vertical steps start in the correct position.
					horizontalStepsCount = steps - 1; //Ensures subsequent Horizontal steps start in the correct position.
				}
				else
				{
					stepsCount = -steps; //Initial number of moves.
					positionX = (int) (BufferWi/2);
					positionY = (BufferHt/2) - (-steps)/2;
					direction = 0; //initial Direction is Up.
					thicknessDirection = 0;
					verticalStepsCount = -steps - 1; //Ensures subsequent Vertical steps start in the correct position.
					horizontalStepsCount = 0; //Ensures subsequent Horizontal steps start in the correct position.
				}

			}

			//Loops through the number of required locations and adjust for Speed and Spacing.
			for (int i = 0; i <= (((width * height + (((167)))) / _frameCount) * _frame) / spacing; i++)
			{
			//		hsv.V *= (((double)(((width * height) / _frameCount) * (_frame / spacing)) - i) / 10);

				//Loops through to obtain correct Thickness.
				for (int k = 0; k < thickness; k++)
				{
					switch (thicknessDirection)
					{
						case 0:
							// add if statement and swap X and Y around for clockwise and anti-clockwise
							frameBuffer.SetPixel(positionX - k + CalculateXOffset(intervalPosFactor),
								positionY + CalculateYOffset(intervalPosFactor), hsv);
							break;
						case 1:
							// add if statement and swap X and Y around for clockwise and anti-clockwise
							frameBuffer.SetPixel(positionX + CalculateXOffset(intervalPosFactor),
								positionY - k + CalculateYOffset(intervalPosFactor), hsv);
							break;
						case 2:
							// add if statement and swap X and Y around for clockwise and anti-clockwise
							frameBuffer.SetPixel(positionX + k + CalculateXOffset(intervalPosFactor),
								positionY + CalculateYOffset(intervalPosFactor), hsv);
							break;
						case 3:
							// add if statement and swap X and Y around for clockwise and anti-clockwise
							frameBuffer.SetPixel(positionX + CalculateXOffset(intervalPosFactor),
								positionY + k + CalculateYOffset(intervalPosFactor), hsv);
							break;
					}
				}

				//Checks to see if we have reached the end of the currect Direction.
				if (stepPosition <= stepsCount)
				{
					stepPosition++;
				}
				else
				{
					//Change Direction.
					stepPosition = 2; //Direction == WhirlpoolDirection.In ? 2 : 2;
					direction = (direction + 1)%4;
					if (Direction == WhirlpoolDirection.In)
					{
						if (direction == 0 || direction == 2)
						{
							stepsCount = verticalStepsCount - spacing;
							verticalStepsCount = verticalStepsCount - spacing;
						}
						else
						{
							stepsCount = horizontalStepsCount - spacing;
							horizontalStepsCount = horizontalStepsCount - spacing;
						}
					}
					else
					{
						if (direction == 0 || direction == 2)
						{
							stepsCount = verticalStepsCount + spacing;
							verticalStepsCount = verticalStepsCount + spacing;
						}
						else
						{
							stepsCount = horizontalStepsCount + spacing;
							horizontalStepsCount = horizontalStepsCount + spacing;
						}
					}
				}

				//Change Postion X/Y based on Direction
				if (Direction == WhirlpoolDirection.In)
				{
					switch (direction)
					{
						case 0: //Down
							positionY--;
							thicknessDirection = 2;
							break;
						case 1: //Right
							positionX++;
							thicknessDirection = 3;
							break;
						case 2: //Up
							positionY++;
							thicknessDirection = 0;
							break;
						case 3: //Left
							positionX--;
							thicknessDirection = 1;
							break;
					}
				}
				else
				{
					switch (direction)
					{
						case 0: //Up
							positionY++;
							thicknessDirection = 0;
							break;
						case 1: //Left
							positionX--;
							thicknessDirection = 1;
							break;
						case 2: //Down
							positionY--;
							thicknessDirection = 2;
							break;
						case 3: //Right
							positionX++;
							thicknessDirection = 3;
							break;
					}
				}
			}
			//Checks to see if we are at the end of the current Iteration and reset _frame count if we are, this will then start a new Whirlpool.
			if (_frameCount == _frame)
			{
				if (numberFrames/Speed > numberFrames - frame) return;
				_frame = 1;
			}
			_frame++;
		}

		private int CalculateXOffset(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(XOffsetCurve.GetValue(intervalPos), (int)(_bufferWi / 2), (int)(-_bufferWi / 2)));
		}

		private int CalculateYOffset(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(YOffsetCurve.GetValue(intervalPos), (int)(_bufferHt / 2), (int)(-_bufferHt / 2)));
		}

		private int CalculateSpacing(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(SpacingCurve.GetValue(intervalPos), _minBufferSize / 2, 1));
		}

		private int CalculateWidth(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(WidthCurve.GetValue(intervalPos), _bufferWi, 0));
		}

		private int CalculateHeight(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(HeightCurve.GetValue(intervalPos), _bufferHt, 0));
		}

		private int CalculateThickness(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(ThicknessCurve.GetValue(intervalPos), CalculateSpacing(intervalPos), 1));
		}

	}
}
