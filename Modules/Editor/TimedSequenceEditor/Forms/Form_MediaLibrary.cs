using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Controls.Timeline;
using Common.Resources.Properties;
using WeifenLuo.WinFormsUI.Docking;
using Svg;
using Label = System.Reflection.Emit.Label;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class MediaLibraryForm : DockContent
	{
		public TimelineControl TimelineControl { get; set; }

		private static readonly string TempPath = Path.Combine(Path.GetTempPath(), "Vixen", "MediaLibrary");
		private TreeNode _selectedItem = new TreeNode();
		private TreeNode _mNode;
		private bool _beginDragDrop;

		public MediaLibraryForm(TimelineControl timelineControl)
		{
			InitializeComponent();
			contextMenuStripMedia.Renderer = new ThemeToolStripRenderer();
			Icon = Resources.Icon_Vixen3;
			TimelineControl = timelineControl;
			ThemeUpdateControls.UpdateControls(this);
		}

		#region GetFiles

		// Get files from folder and reference to node
		private void toolStripMenuItemAddMediaFolder_Click(object sender, EventArgs e)
		{
			OpenFileDialog folderBrowser = new OpenFileDialog();
			// Set validate names and check file exists to false otherwise windows will
			// not let you select "Folder Selection."
			folderBrowser.ValidateNames = false;
			folderBrowser.CheckFileExists = false;
			folderBrowser.CheckPathExists = true;
			// Always default to Folder Selection.
			folderBrowser.FileName = "     Folder Selection.";
			if (folderBrowser.ShowDialog() == DialogResult.OK)
			{
				string folderPath = Path.GetDirectoryName(folderBrowser.FileName);
				string[] files = Directory.GetFiles(folderPath);
				AddFilesToList(files, folderPath, true);
			}
		}

		private void toolStripMenuItemAddMediaFiles_Click(object sender, EventArgs e)
		{
			OpenFileDialog folderBrowser = new OpenFileDialog();
			folderBrowser.Multiselect = true;
			if (folderBrowser.ShowDialog() == DialogResult.OK)
			{
				string folderPath = Path.GetDirectoryName(folderBrowser.FileName);
				string[] files = folderBrowser.FileNames;
				AddFilesToList(files, folderPath, false);
			}
		}
		
		private void AddFilesToList(string[] files, string folderPath, bool folder)
		{
			int folderIndex = mediaTreeview.Nodes[5].Nodes.Count;
			if (folder) // Only if user grabbed entire folder.
			{
				bool folderExists = false;
				for (int i = 0; i < mediaTreeview.Nodes[5].Nodes.Count; i++)
				{
					if (mediaTreeview.Nodes[5].Nodes[i].Name == folderPath)
					{
						folderIndex = i;
						folderExists = true;
						break;
					}
				}
				if (!folderExists)
				{
					TreeNode folderNode = new TreeNode();
					folderNode.Tag = "Folder";
					folderNode.Name = folderPath;
					folderNode.Text = folderPath;
					mediaTreeview.Nodes[5].Nodes.Add(folderNode);
				}
			}
			foreach (var file in files)
			{
				if (CheckIfFileExists(file)) continue; // Don't add if media file already exists.

				TreeNode node = new TreeNode();
				int nodeIndex = 0;
				var extention = Path.GetExtension(file);
				if (extention == ".jpg" || extention == ".png" || extention == ".bmp")
				{
					node.Tag = "Picture";
					nodeIndex = 0;
				}
				else if (extention == ".mov" || extention == ".avi" || extention == ".mp4")
				{
					node.Tag = "Video";
					nodeIndex = 1;
				}
				else if (extention == ".gif")
				{
					node.Tag = "GIF";
					nodeIndex = 2;
				}
				else if (extention == ".svg")
				{
					node.Tag = "Shapes";
					nodeIndex = 3;
				}
				else if (extention == ".gled")
				{
					node.Tag = "Glediator";
					nodeIndex = 4;
				}
				else continue;
				node.Name = file;
				node.Text = Path.GetFileName(file);
				mediaTreeview.Nodes[nodeIndex].Nodes.Add(node);
				if (folder) mediaTreeview.Nodes[5].Nodes[folderIndex].Nodes.Add((TreeNode) node.Clone());
			}
		}

		private bool CheckIfFileExists(string file)
		{
			// Traverse and remove all selected nodes.
			return TraverseTree(mediaTreeview.Nodes, file);
		}
		
		private bool TraverseTree(TreeNodeCollection nodes, string file)
		{
			foreach (TreeNode node in nodes)
			{
				if (node.Name == file)
				{
					return true;
				}
				if (TraverseTree(node.Nodes, file)) return true;
			}
			return false;
		}

		#endregion

		#region SetThumbnailImage

		private void mediaTreeview_Click(object sender, TreeViewEventArgs e)
		{
			_selectedItem = e.Node;
			frameValue.Value = 1;
			SetThumbNailImage();
		}

		private void frameValue_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
		{
			SetThumbNailImage();
		}

		// Process required media type and get a thumbnail image to display.
		// Sets Thumbnail image
		private void SetThumbNailImage()
		{
			try
			{
				if (_selectedItem.Tag.ToString() == "Root" || _selectedItem.Tag.ToString() == "Folder")
				{
					//Remove the displayed image if a root or folder node is selected.
					pictureBox.Image = null;
					return;
				}
				if (!File.Exists(_selectedItem.Name) && _selectedItem.Tag.ToString() != "Root" && _selectedItem.Tag.ToString() != "Folder")
				{
					// Remove the nodes if the file no longer exists at the path.
					RemoveNodes(mediaTreeview.Nodes, _selectedItem.Name);
					pictureBox.Image = null;
					return;
				}
				Image image = null;
				switch (_selectedItem.Tag)
				{
					case "Picture":
						pictureBox.Image = ProcessImage();
						frameValue.Visible = labelFrame.Visible = false;
						break;
					case "GIF":
						pictureBox.Image = ProcessImage();
						frameValue.Visible = labelFrame.Visible = false;
						break;
					case "Glediator":
						break;
					case "Shapes":
						pictureBox.Image = ProcessSVGShapes();
						frameValue.Visible = labelFrame.Visible = false;
						break;
					case "Video":
						image = ProcessVideo();
						pictureBox.Image = new Bitmap(image);
						frameValue.Visible = labelFrame.Visible = true;
						break;
				}

				if (image != null) image.Dispose();
			}
			catch (Exception exception)
			{
				// ignored
			}

		}

		// Process Media
		private Image ProcessImage()
		{
			return Image.FromFile(_selectedItem.Name);
		}

		private void RemoveNodes(TreeNodeCollection nodes, string file)
		{
			foreach (TreeNode node in nodes)
			{
				if (node.Name == file) node.Remove();
				RemoveNodes(node.Nodes, file);
			}
		}

		private Image ProcessSVGShapes()
		{
			SvgDocument svgImage = SvgDocument.Open(_selectedItem.Name);
			svgImage.Height = new SvgUnit(pictureBox.Height);
			svgImage.Width = new SvgUnit(pictureBox.Width);
			Image image = new Bitmap(pictureBox.Width, pictureBox.Height);
			using (Graphics g = Graphics.FromImage(image))
			{
				g.DrawImage(svgImage.Draw(), 0, 0);
			}
			return image;
		}

		private Image ProcessVideo()
		{
			if (!Directory.Exists(TempPath))
			{
				Directory.CreateDirectory(TempPath);
			}
			else
			{
				if (File.Exists(TempPath + "\\Temp.bmp")) File.Delete(TempPath + "\\Temp.bmp");
			}
			ffmpeg.ffmpeg getVideoThumbNail = new ffmpeg.ffmpeg(_selectedItem.Name);
			getVideoThumbNail.GetVideoThumbNail(TempPath + "\\Temp.bmp", frameValue.Value);
			return Image.FromFile(TempPath + "\\Temp.bmp");
		}
		#endregion

		#region RemoveNodes
		// Remove selected nodes.
		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < mediaTreeview.SelectedNodes.Count; i++)
			{
				if (mediaTreeview.SelectedNodes[i].Tag.ToString() == "Folder" ||
				    mediaTreeview.SelectedNodes[i].Tag.ToString() == "Root")
				{
					foreach (TreeNode node in mediaTreeview.SelectedNodes[i].Nodes) mediaTreeview.SelectedNodes.Add(node);
				}
			}

			// Traverse and remove all selected nodes.
			TraverseTree(mediaTreeview.Nodes);

			// Check Folder Nodes to see if any are empty and if they are then remove it.
			for (int i = mediaTreeview.Nodes[5].Nodes.Count - 1; i >= 0  ; i--)
			{
				if(mediaTreeview.Nodes[5].Nodes[i].Nodes.Count == 0) mediaTreeview.Nodes[5].Nodes.Remove(mediaTreeview.Nodes[5].Nodes[i]);
			}
			
			pictureBox.Image = null;
			frameValue.Visible = labelFrame.Visible = false;
		}

		private void TraverseTree(TreeNodeCollection nodes)
		{
			for (int i = nodes.Count - 1; i >= 0; i--)
			{
				for (int j = mediaTreeview.SelectedNodes.Count - 1; j >= 0; j--)
				{
					mediaTreeview.SelectedNodes.Sort(MySorter);
					if (mediaTreeview.SelectedNodes[j].Tag == null || mediaTreeview.SelectedNodes[j].Tag.ToString() == "Root") continue; // Ensures root nodes are not removed and only media files/locations are.
					if (nodes[i] != null && nodes[i].Name == mediaTreeview.SelectedNodes[j].Name)
					{
						nodes.Remove(nodes[i]);
						i--;
						if (i < 0 || nodes[i] == null) break;
					}
				}
				if (i < 0 || nodes[i] == null) continue;
				TraverseTree(nodes[i].Nodes);
			}
		}

		private int MySorter(TreeNode x, TreeNode y)
		{
				return x.Text.CompareTo(y.Text);
		}
		#endregion

		#region DragToTimeLine
		private void mediaTreeview_MouseDown(object sender, MouseEventArgs e)
		{

			_mNode = mediaTreeview.GetNodeAt(e.X, e.Y);

			_beginDragDrop =
				(_mNode != null && _mNode.Nodes.Count == 0) &&
				(e.Button == MouseButtons.Left && e.Clicks == 1);
		}

		private void mediaTreeview_MouseMove(object sender, MouseEventArgs e)
		{
			//if (_beginDragDrop)
			//{
			//	_beginDragDrop = false;
			//	DataObject data = new DataObject(_mNode.Tag);
			//	mediaTreeview.DoDragDrop(data, DragDropEffects.Copy);
			//	_DropMediaLibrary();
			//}

		}

		private void mediaTreeview_ItemDrag(object sender, ItemDragEventArgs e)
		{
			DataObject data = new DataObject(_mNode.Tag);
			mediaTreeview.DoDragDrop(data, DragDropEffects.Copy);
		}
		#endregion

		#region Load_Save
		private void MediaLibraryForm_Load(object sender, EventArgs e)
		{
			// Save Node Tree here and check each reference and remove any that are not found.

		}
		private void MediaLibraryForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Save Node Tree here

		}
		#endregion
	}
}
