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
		private int _numberFrames;
		private IPixelFrameBuffer _tempBuffer;
		private bool _whirlpoolDirection;
		private int _adjustedIterations;

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
		public int Iterations
		{
			get { return _data.Iterations; }
			set
			{
				_data.Iterations = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Spacing")]
		[ProviderDescription(@"Spacing")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(2)]
		public int Spacing
		{
			get { return _data.Spacing; }
			set
			{
				_data.Spacing = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"WhirlpoolThickness")]
		[ProviderDescription(@"WhirlpoolThickness")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(3)]
		public int Thickness
		{
			get { return _data.Thickness; }
			set
			{
				_data.Thickness = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Width")]
		[ProviderDescription(@"Width")]
		[PropertyOrder(4)]
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
		[PropertyOrder(5)]
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
		[ProviderDisplayName(@"GradientMode")]
		[ProviderDescription(@"GradientMode")]
		[PropertyOrder(0)]
		public GradientMode GradientMode
		{
			get { return _data.GradientMode; }
			set
			{
				_data.GradientMode = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"Colors")]
		[ProviderDescription(@"Colors")]
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

		protected override void SetupRender()
		{
			_tempBuffer = new PixelFrameBuffer(BufferWi, BufferHt);
			_numberFrames = GetNumberFrames();
			switch (Direction)
			{
				case WhirlpoolDirection.InOut:
					_adjustedIterations = Iterations*2;
					_whirlpoolDirection = true;
					break;
				case WhirlpoolDirection.In:
					_whirlpoolDirection = true;
					_adjustedIterations = Iterations;
					break;
				case WhirlpoolDirection.OutIn:
					_adjustedIterations = Iterations * 2;
					_whirlpoolDirection = false;
					break;
				case WhirlpoolDirection.Out:
					_whirlpoolDirection = false;
					_adjustedIterations = Iterations;
					break;
			}
		}

		protected override void CleanUpRender()
		{
			//Nothing to do.
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;

			InitialRender(frame, level);
			
			for (int x = 0; x < BufferWi; x++)
			{
				for (int y = 0; y < BufferHt; y++)
				{
						CalculatePixel(x, y, frameBuffer);
				}
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			for (int frame = 0; frame < numFrames; frame++)
			{
				frameBuffer.CurrentFrame = frame;
				double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;

				InitialRender(frame, level);

				foreach (var elementLocation in frameBuffer.ElementLocations)
				{
					CalculatePixel(elementLocation.X, elementLocation.Y, frameBuffer);
				}
			}

		}

		private void CalculatePixel(int x, int y, IPixelFrameBuffer frameBuffer)
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

			if (_tempBuffer.GetColorAt(x, y) != Color.Transparent)
			{
				frameBuffer.SetPixel(xCoord, yCoord, _tempBuffer.GetColorAt(x, y));
			}
		}

		private void InitialRender(int frame, double level)
		{
			double intervalPos = GetEffectTimeIntervalPosition(frame);
			double intervalPosFactor = intervalPos*100;
			int width = CalculateWidth(intervalPosFactor);
			int height = CalculateHeight(intervalPosFactor);
			int spacing = CalculateSpacing();
			int thickness = CalculateThickness();
			int xOffSet = CalculateXOffset(intervalPosFactor);
			int yOffSet = CalculateYOffset(intervalPosFactor);
			int positionX;
			int positionY;
			int direction;
			int stepsCount;
			int horizontalStepsCount = width;
			int verticalStepsCount = height;
			int stepPosition;
			int thicknessDirection;
			int colorIndex = 0;

			HSV hsv = new HSV();
			for (int i = 0; i < BufferWi; i++)
			{
				for (int z = 0; z < BufferHt; z++)
				{
					_tempBuffer.SetPixel(i, z, Color.Transparent);
				}
			}

			if (frame == 0)
			{
				_frame = 1;
				_frameCount = _numberFrames / _adjustedIterations;
			}

			int maxPixels = (((width * height + 167) / _frameCount) * _frame) / spacing;

			//Setup Initial Values
			if (_whirlpoolDirection)
			{
				//Assuming end location is the middle of the Matrix we need to start at a location half the height and half the width away from middle.
				positionX = (BufferWi / 2) - (width / 2);
				positionY = (BufferHt / 2) - (height / 2);

				direction = 1; //Initial Direction is Right as we are starting in he lower left endge of the whirlpool.
				thicknessDirection = 3;
				stepsCount = width; //Performs number of width steps, this will be the opposite side of the whirlpool.
				stepPosition = 2; //Steps already performed.
			}
			else
			{
				//First location is in the middle of the matrix - half the difference between the Width and Height of the whirlpool.
				int steps = (width - height);

				stepPosition = 2;  //Steps already performed.

				if (steps > 0)
				{
					stepsCount = steps; //Initial number of moves.
					positionX = (BufferWi / 2) - (steps / 2);
					positionY = (BufferHt / 2);
					direction = 3; //initial Direction is Right.
					thicknessDirection = 3;
					verticalStepsCount = 0; //Ensures subsequent Vertical steps start in the correct position.
					horizontalStepsCount = steps - 1; //Ensures subsequent Horizontal steps start in the correct position.
				}
				else
				{
					stepsCount = -steps; //Initial number of moves.
					positionX = (int)(BufferWi / 2);
					positionY = (BufferHt / 2) - (-steps) / 2;
					direction = 0; //initial Direction is Up.
					thicknessDirection = 0;
					verticalStepsCount = -steps - 1; //Ensures subsequent Vertical steps start in the correct position.
					horizontalStepsCount = 0; //Ensures subsequent Horizontal steps start in the correct position.
				}

			}

			//Loops through the number of required locations.
			for (int i = 0; i <= maxPixels; i++)
			{
				
				switch (GradientMode)
				{
					case GradientMode.OverTime:
						hsv = HSV.FromRGB(Colors[colorIndex].GetColorAt((intervalPosFactor)/100));
						break;
					case GradientMode.OverElement:
						hsv = HSV.FromRGB(Colors[colorIndex].GetColorAt((100 / (double)(maxPixels) * i) / 100));
						break;
				}

				hsv.V = hsv.V * level;

				if (i >= 0)
				{
					//Loops through to set correct Thickness.
					for (int k = 0; k < thickness; k++)
					{
						switch (thicknessDirection)
						{
							case 0:
								_tempBuffer.SetPixel(positionX - k + xOffSet, positionY + yOffSet, hsv);
								break;
							case 1:
								_tempBuffer.SetPixel(positionX + xOffSet, positionY - k + yOffSet, hsv);
								break;
							case 2:
								_tempBuffer.SetPixel(positionX + k + xOffSet, positionY + yOffSet, hsv);
								break;
							case 3:
								_tempBuffer.SetPixel(positionX + xOffSet, positionY + k + yOffSet, hsv);
								break;
						}
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
					stepPosition = 2;
					direction = (direction + 1)%4;
					colorIndex = (colorIndex + 1)%Colors.Count;
					if (_whirlpoolDirection)
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
				if (_whirlpoolDirection)
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

			//Checks to see if we are at the end of the current Iteration and reset _frame count and tempbuffer if we are, this will then start a new Whirlpool.
			if (_frameCount == _frame)
			{
				if (_numberFrames / _adjustedIterations > _numberFrames - frame) return;
				if (Direction == WhirlpoolDirection.InOut || Direction == WhirlpoolDirection.OutIn)
					_whirlpoolDirection = !_whirlpoolDirection;
				_frame = 1;
			}
			_frame++;
		}

		private int CalculateXOffset(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(XOffsetCurve.GetValue(intervalPos), (int)(BufferWi / 2), (int)(-BufferWi / 2)));
		}

		private int CalculateYOffset(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(YOffsetCurve.GetValue(intervalPos), (int)(BufferHt / 2), (int)(-BufferHt / 2)));
		}

		private int CalculateSpacing()
		{
			return (int)Math.Round(ScaleCurveToValue(Spacing, (int)(Math.Min(BufferHt, BufferWi) / 2), 1));
		}

		private int CalculateWidth(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(WidthCurve.GetValue(intervalPos), BufferWi, 1));
		}

		private int CalculateHeight(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(HeightCurve.GetValue(intervalPos), BufferHt, 1));
		}

		private int CalculateThickness()
		{
			return (int)Math.Round(ScaleCurveToValue(Thickness, CalculateSpacing(), 1));
		}

	}
}
