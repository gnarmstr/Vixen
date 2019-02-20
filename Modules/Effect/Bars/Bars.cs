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

namespace VixenModules.Effect.Bars
{
	public class Bars:PixelEffectBase
	{
		private BarsData _data;
		private double _position;
		private bool _negPosition;

		public Bars()
		{
			_data = new BarsData();
			EnableTargetPositioning(true, true);
			InitAllAttributes();
		}

		public override bool IsDirty
		{
			get
			{
				if(Colors.Any(x => !x.CheckLibraryReference()))
				{
					base.IsDirty = true;
				}

				return base.IsDirty;
			}
			protected set { base.IsDirty = value; }
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
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		[PropertyOrder(0)]
		public BarDirection Direction
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MovementType")]
		[ProviderDescription(@"MovementType")]
		[PropertyOrder(1)]
		public MovementType MovementType
		{
			get { return _data.MovementType; }
			set
			{
				_data.MovementType = value;
				IsDirty = true;
				UpdateMovementTypeAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Iterations")]
		[ProviderDescription(@"Iterations")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 20, 1)]
		[PropertyOrder(2)]
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		[PropertyOrder(3)]
		public Curve SpeedCurve
		{
			get { return _data.SpeedCurve; }
			set
			{
				_data.SpeedCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Repeat")]
		[ProviderDescription(@"Repeat")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 10, 1)]
		[PropertyOrder(4)]
		public int Repeat
		{
			get { return _data.Repeat; }
			set
			{
				_data.Repeat = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Highlight")]
		[ProviderDescription(@"Highlight")]
		[PropertyOrder(5)]
		public bool Highlight
		{
			get { return _data.Highlight; }
			set
			{
				_data.Highlight = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Show3D")]
		[ProviderDescription(@"Show3D")]
		[PropertyOrder(6)]
		public bool Show3D
		{
			get { return _data.Show3D; }
			set
			{
				_data.Show3D = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		[Value]
		[ProviderCategory(@"Movement", 3)]
		[ProviderDisplayName(@"XOffset")]
		[ProviderDescription(@"XOffset")]
		[PropertyOrder(0)]
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
		[ProviderCategory(@"Movement", 3)]
		[ProviderDisplayName(@"YOffset")]
		[ProviderDescription(@"YOffset")]
		[PropertyOrder(1)]
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

		#region Color properties


		[Value]
		[ProviderCategory(@"Color", 2)]
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
		[ProviderCategory(@"Brightness", 3)]
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

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/bars/"; }
		}

		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as BarsData;
				InitAllAttributes();
				IsDirty = true;
			}
		}

		private void InitAllAttributes()
		{
			UpdateStringOrientationAttributes(true);
			UpdateMovementTypeAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		private void UpdateMovementTypeAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{ "SpeedCurve", MovementType == MovementType.Speed},
				
				{ "Speed", MovementType != MovementType.Speed}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		protected override void SetupRender()
		{
			//Nothing to setup
		}

		protected override void CleanUpRender()
		{
			//Nothing to clean up
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			var buffer = frameBuffer as PixelLocationFrameBuffer;
			if (buffer != null)
			{
				RenderEffectByLocation(frame, buffer);
			}

			int x, y, n, colorIdx;
			int colorcnt = Colors.Count();
			int barCount = Repeat * colorcnt;
			double intervalPosFactor = GetEffectTimeIntervalPosition(frame) * 100;
			var bufferWi = BufferWi;
			var bufferHt = BufferHt;
			int xOffsetAdj = (int)((double)CalculateXOffset(intervalPosFactor));
			int yOffsetAdj = (int)((double)CalculateYOffset(intervalPosFactor));

			_negPosition = false;

			if (MovementType == MovementType.Iterations)
			{
				_position = (GetEffectTimeIntervalPosition(frame) * Speed) % 1;
			}
			else
			{
				if (frame == 0) _position = CalculateSpeed(intervalPosFactor);
				_position += CalculateSpeed(intervalPosFactor) / 100;
				if (_position < 0)
				{
					_negPosition = true;
					_position = -_position;
				}
			}

			if (barCount < 1) barCount = 1;
			double level = LevelCurve.GetValue(intervalPosFactor) / 100;

			if (Direction < BarDirection.Left || Direction == BarDirection.AlternateUp || Direction == BarDirection.AlternateDown)
			{
				int barHt = bufferHt / barCount+1;
				if (barHt < 1) barHt = 1;
				int halfHt = bufferHt / 2;
				int blockHt = colorcnt * barHt;
				if (blockHt < 1) blockHt = 1;
				int fOffset = (int) (_position*blockHt*Repeat);// : Speed * frame / 4 % blockHt);
				if(Direction == BarDirection.AlternateUp || Direction == BarDirection.AlternateDown)
				{
					fOffset = (int)(Math.Floor(_position*barCount)*barHt);
				}
				int indexAdjust = 1;
				
				for (y = 0; y < bufferHt; y++)
				{
					int yOffset;
					if (Direction == BarDirection.Down || Direction == BarDirection.AlternateDown)
						yOffset = y - yOffsetAdj;
					else
						yOffset = yOffsetAdj + y;
					n = yOffset + fOffset;
					colorIdx = ((n + indexAdjust) % blockHt) / barHt;
					if (colorIdx < 0) colorIdx = -colorIdx;
					//we need the integer division here to make things work
					float colorPosition = ((n + indexAdjust) / barHt) - ((n + indexAdjust) / barHt);
					Color c = Colors[colorIdx].GetColorAt(colorPosition);
					if (Highlight || Show3D)
					{
						var hsv = HSV.FromRGB(c);
						if (Highlight && (n + indexAdjust) % barHt == 0) hsv.S = 0.0f;
						if (Show3D) hsv.V *= (float) (barHt - (n + indexAdjust) % barHt - 1) / barHt;
						hsv.V *= level;
						c = hsv.ToRGB();
					}
					else
					{
						if (level < 1)
						{
							HSV hsv = HSV.FromRGB(c);
							hsv.V *= level;
							c = hsv.ToRGB();
						}
					}

					switch (Direction)
					{
						case BarDirection.Down:
						case BarDirection.AlternateDown:
							// dow
							if (_negPosition)
							{
								for (x = 0; x < bufferWi; x++)
								{
									frameBuffer.SetPixel(x - xOffsetAdj, bufferHt + yOffset - 1, c);
								}
							}
							else
							{
								for (x = 0; x < bufferWi; x++)
								{
									frameBuffer.SetPixel(x - xOffsetAdj, yOffset, c);
								}
							}

							break;
						case BarDirection.Expand:
							// expand
							if (_negPosition)
							{
								if (yOffset <= halfHt)
								{
									for (x = 0; x < bufferWi; x++)
									{
										frameBuffer.SetPixel(x - xOffsetAdj, yOffset, c);
										frameBuffer.SetPixel(x - xOffsetAdj, bufferHt - yOffset - 1, c);
									}
								}
							}
							else
							{
								if (yOffset >= halfHt)
								{
									for (x = 0; x < bufferWi; x++)
									{
										frameBuffer.SetPixel(x - xOffsetAdj, yOffset, c);
										frameBuffer.SetPixel(x - xOffsetAdj, bufferHt - yOffset - 1, c);
									}
								}
							}
							break;
						case BarDirection.Compress:
							// compress
							if (!_negPosition)
							{
								if (yOffset <= halfHt)
								{
									for (x = 0; x < bufferWi; x++)
									{
										frameBuffer.SetPixel(x - xOffsetAdj, yOffset, c);
										frameBuffer.SetPixel(x - xOffsetAdj, bufferHt - yOffset - 1, c);
									}
								}
							}
							else
							{
								if (yOffset >= halfHt)
								{
									for (x = 0; x < bufferWi; x++)
									{
										frameBuffer.SetPixel(x - xOffsetAdj, yOffset, c);
										frameBuffer.SetPixel(x - xOffsetAdj, bufferHt - yOffset - 1, c);
									}
								}
							}
							break;
						default:
							// up & AlternateUp
							if (!_negPosition)
							{
								for (x = 0; x < bufferWi; x++)
								{
									frameBuffer.SetPixel(x - xOffsetAdj, bufferHt - yOffset - 1, c);
								}
							}
							else
							{
								for (x = 0; x < bufferWi; x++)
								{
									frameBuffer.SetPixel(x - xOffsetAdj, yOffset, c);
								}
							}
							break;
					}
				}
			}
			else
			{
				int barWi = bufferWi / barCount+1;
				if (barWi < 1) barWi = 1;
				int halfWi = bufferWi / 2;
				int blockWi = colorcnt * barWi;
				if (blockWi < 1) blockWi = 1;
				int fOffset = (int)(_position * blockWi * Repeat);
				if (Direction > BarDirection.AlternateDown)
				{
					fOffset = (int)(Math.Floor(_position * barCount) * barWi);
				} 
				
				for (x = 0; x < bufferWi; x++)
				{
					int xOffset = Direction == BarDirection.Right
						? x + xOffsetAdj
						: x - xOffsetAdj;
					//int xOffset = x - xOffsetAdj;
					n = xOffset + fOffset;
					colorIdx = ((n + 1) % blockWi) / barWi;
					if (colorIdx < 0) colorIdx = -colorIdx;
					//we need the integer division here to make things work
					float colorPosition = ((n + 1) / barWi) - ((n + 1) / barWi);
					Color c = Colors[colorIdx].GetColorAt( colorPosition );
					if (Highlight || Show3D)
					{
						var hsv = HSV.FromRGB(c);
						if (Highlight && n % barWi == 0) hsv.S = 0.0f;
						if (Show3D) hsv.V *= (float)(barWi - n % barWi - 1) / barWi;
						hsv.V *= level;
						c = hsv.ToRGB();
					}
					else
					{
						if (level < 1)
						{
							HSV hsv = HSV.FromRGB(c);
							hsv.V *= level;
							c = hsv.ToRGB();
						}
					}

					switch (Direction)
					{
						case BarDirection.Right:
						case BarDirection.AlternateRight:
							// right
							for (y = 0; y < bufferHt; y++)
							{
								frameBuffer.SetPixel(bufferWi - xOffset - 1, y - yOffsetAdj, c);
							}
							break;
						case BarDirection.HExpand:
							// H-expand
							if (xOffset <= halfWi)
							{
								for (y = 0; y < bufferHt; y++)
								{
									frameBuffer.SetPixel(xOffset, y - yOffsetAdj, c);
									frameBuffer.SetPixel(bufferWi - xOffset - 1, y - yOffsetAdj, c);
								}
							}
							break;
						case BarDirection.HCompress:
							// H-compress
							if (xOffset >= halfWi)
							{
								for (y = 0; y < BufferHt; y++)
								{
									frameBuffer.SetPixel(xOffset, y - yOffsetAdj, c);
									frameBuffer.SetPixel(bufferWi - xOffset - 1, y - yOffsetAdj, c);
								}
							}
							break;
						default:
							// left & AlternateLeft
							for (y = 0; y < bufferHt; y++)
							{
								frameBuffer.SetPixel(xOffset, y - yOffsetAdj, c);
							}
							break;
					}
				}
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var bufferWi = BufferWi;
			var bufferHt = BufferHt;
			int colorcnt = Colors.Count();
			int barCount = Repeat * colorcnt;
			if (barCount < 1) barCount = 1;
			
			int barHt = bufferHt / barCount + 1;
			if (barHt < 1) barHt = 1;
			int blockHt = colorcnt * barHt;
			if (blockHt < 1) blockHt = 1;

			int barWi = bufferWi / barCount + 1;
			if (barWi < 1) barWi = 1;
			int blockWi = colorcnt * barWi;
			if (blockWi < 1) blockWi = 1;

			IEnumerable<IGrouping<int, ElementLocation>> nodes;
			List<IGrouping<int, ElementLocation>> reversedNodes = new List<IGrouping<int, ElementLocation>>();
			
			switch (Direction)
			{
				case BarDirection.AlternateUp:
				case BarDirection.Up:
					nodes = frameBuffer.ElementLocations.OrderBy(x => x.Y).ThenBy(x => x.X).GroupBy(x => x.Y);
					break;
				case BarDirection.Left:
				case BarDirection.AlternateLeft:
					nodes = frameBuffer.ElementLocations.OrderByDescending(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
					break;
				case BarDirection.Right:
				case BarDirection.AlternateRight:
					nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
					break;
				case BarDirection.Compress:
				case BarDirection.Expand:
					nodes = frameBuffer.ElementLocations.OrderByDescending(x => x.Y).ThenBy(x => x.X).GroupBy(x => x.Y);
					reversedNodes = nodes.Reverse().ToList();
					break;
				case BarDirection.HCompress:
				case BarDirection.HExpand:
					nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
					reversedNodes = nodes.Reverse().ToList();
					break;
				default:
					nodes = frameBuffer.ElementLocations.OrderByDescending(x => x.Y).ThenBy(x => x.X).GroupBy(x => x.Y);
					break;

			}
			var nodeCount = nodes.Count();
			var halfNodeCount = (nodeCount - 1) / 2;
			var evenHalfCount = nodeCount%2!=0;
			for (int frame = 0; frame < numFrames; frame++)
			{
				frameBuffer.CurrentFrame = frame;
				double intervalPosFactor = GetEffectTimeIntervalPosition(frame) * 100;
				double level = LevelCurve.GetValue(intervalPosFactor) / 100;
				int xOffsetAdj = CalculateXOffset(intervalPosFactor) * (bufferWi) / 100;
				int yOffsetAdj = CalculateYOffset(intervalPosFactor) * (bufferHt) / 100;

				if (MovementType == MovementType.Iterations)
				{
					_position = (GetEffectTimeIntervalPosition(frame) * Speed) % 1;
				}
				else
				{
					if (frame == 0) _position = CalculateSpeed(intervalPosFactor);
					_position += CalculateSpeed(intervalPosFactor) / 100;
				}

				int n;
				int colorIdx;
				if (Direction < BarDirection.Left || Direction == BarDirection.AlternateUp || Direction == BarDirection.AlternateDown)
				{
					
					int fOffset = (int)(_position * blockHt * Repeat);// : Speed * frame / 4 % blockHt);
					if (Direction == BarDirection.AlternateUp || Direction == BarDirection.AlternateDown)
					{
						fOffset = (int)(Math.Floor(_position * barCount) * barHt);
					}
					if (Direction == BarDirection.Down || Direction == BarDirection.AlternateDown || Direction == BarDirection.Expand)
					{
						fOffset = -fOffset;
					}

					int indexAdjust = 1;

					int i = 0;
					bool exitLoop = false;
					foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
					{
						
						int y = elementLocations.Key - yOffsetAdj;
						n = y + fOffset;
						colorIdx = Math.Abs( ((n + indexAdjust) % blockHt) / barHt );
						
						//we need the integer division here to make things work
						double colorPosition = Math.Abs( (double)(n + indexAdjust) / barHt - (n + indexAdjust) / barHt );
						Color c = Colors[colorIdx].GetColorAt(colorPosition);
						if (Highlight || Show3D)
						{
							var hsv = HSV.FromRGB(c);
							if (Highlight && (n + indexAdjust) % barHt == 0) hsv.S = 0.0f;
							if (Show3D) hsv.V *= (float)(barHt - (n + indexAdjust) % barHt - 1) / barHt;
							hsv.V *= level;
							c = hsv.ToRGB();
						}
						else
						{
							if (level < 1)
							{
								HSV hsv = HSV.FromRGB(c);
								hsv.V *= level;
								c = hsv.ToRGB();
							}
						}

						switch (Direction)
						{
							case BarDirection.Expand:
							case BarDirection.Compress:
								// expand / compress
								if (i <= halfNodeCount)
								{
									foreach (var elementLocation in elementLocations)
									{
										frameBuffer.SetPixel(elementLocation.X - xOffsetAdj, y, c);
									}
									if (i == halfNodeCount & evenHalfCount)
									{
										i++;
										continue;
									}
									foreach (var elementLocation in reversedNodes[i])
									{
										frameBuffer.SetPixel(elementLocation.X - xOffsetAdj, elementLocation.Y, c);
									}

									i++;
								}
								else
								{
									exitLoop = true;
								}
								break;
							default:
								foreach (var elementLocation in elementLocations)
								{
									frameBuffer.SetPixel(elementLocation.X - xOffsetAdj, y, c);
								}
								break;
						}
						if (exitLoop) break;
					}
				}
				else
				{
					
					int fOffset = (int)(_position * blockWi * Repeat);
					if (Direction > BarDirection.AlternateDown)
					{
						fOffset = (int)(Math.Floor(_position * barCount) * barWi);
					}
					if (Direction == BarDirection.Right || Direction == BarDirection.AlternateRight || Direction == BarDirection.HCompress)
					{
						fOffset = -fOffset;
					}

					int i = 0;
					
					foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
					{
						int x = elementLocations.Key - xOffsetAdj;
						n = x + fOffset;
						colorIdx = Math.Abs( ((n + 1) % blockWi) / barWi );
						//we need the integer division here to make things work
						double colorPosition = Math.Abs(  (double)(n + 1) / barWi - (n + 1) / barWi );
						Color c = Colors[colorIdx].GetColorAt(colorPosition);

						if (Highlight || Show3D)
						{
							var hsv = HSV.FromRGB(c);
							if (Highlight && n % barWi == 0) hsv.S = 0.0f;
							if (Show3D) hsv.V *= (float)(barWi - n % barWi - 1) / barWi;
							hsv.V *= level;
							c = hsv.ToRGB();
						}
						else
						{
							if (level < 1)
							{
								HSV hsv = HSV.FromRGB(c);
								hsv.V *= level;
								c = hsv.ToRGB();
							}
						}

						switch (Direction)
						{
							case BarDirection.HExpand:
							case BarDirection.HCompress:
								if (i <= halfNodeCount)
								{
									foreach (var elementLocation in elementLocations)
									{
										frameBuffer.SetPixel(x, elementLocation.Y - yOffsetAdj, c);
									}
									if (i == halfNodeCount & evenHalfCount)
									{
										i++;
										continue;
									}
									foreach (var elementLocation in reversedNodes[i])
									{
										frameBuffer.SetPixel(elementLocation.X, elementLocation.Y - yOffsetAdj, c);
									}

									i++;
								}
								break;
							default:
								foreach (var elementLocation in elementLocations)
								{
									frameBuffer.SetPixel(x, elementLocation.Y - yOffsetAdj, c);
								}
								break;
						}
					}

				}

			}

		}

		private double CalculateSpeed(double intervalPos)
		{
			return ScaleCurveToValue(SpeedCurve.GetValue(intervalPos), 15, -15);
		}

		private int CalculateXOffset(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(XOffsetCurve.GetValue(intervalPos), -BufferWi, BufferWi));
		}

		private int CalculateYOffset(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(YOffsetCurve.GetValue(intervalPos), -BufferHt, BufferHt));
		}

	}
}
