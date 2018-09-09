using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using Common.Controls;
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
		XMLProfileSettings _xml;
		private int _nodeCount;
		private int processed;
		private string _mediaFilePath;
		private Dictionary<string, string> _nodes;
		private int _treeViewFolderIndex;

		public MediaLibraryForm(TimelineControl timelineControl)
		{
			InitializeComponent();
			contextMenuStripMedia.Renderer = new ThemeToolStripRenderer();
			Icon = Resources.Icon_Vixen3;
			TimelineControl = timelineControl;
			ThemeUpdateControls.UpdateControls(this);
		}
		private void MediaLibraryForm_Load(object sender, EventArgs e)
		{
			processed = 0;
			for (int i = 0; i < mediaTreeview.Nodes.Count; i++)
			{
				if (mediaTreeview.Nodes[i].Name == "Folders")
				{
					_treeViewFolderIndex = i;
				}
			}
			PopulateTreeNode();
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
			int folderIndex = mediaTreeview.Nodes[_treeViewFolderIndex].Nodes.Count;
			if (folder) // Only if user grabbed entire folder.
			{
				bool folderExists = false;
				for (int i = 0; i < mediaTreeview.Nodes[_treeViewFolderIndex].Nodes.Count; i++)
				{
					if (mediaTreeview.Nodes[_treeViewFolderIndex].Nodes[i].Name == folderPath)
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
					mediaTreeview.Nodes[_treeViewFolderIndex].Nodes.Add(folderNode);
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
				if (folder) mediaTreeview.Nodes[_treeViewFolderIndex].Nodes[folderIndex].Nodes.Add((TreeNode) node.Clone());
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
			if (processed == 1)
			{
				processed = 0;
				return;
			}

			processed++;
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
						pictureBox.Image = null;
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
			for (int i = mediaTreeview.Nodes[_treeViewFolderIndex].Nodes.Count - 1; i >= 0  ; i--)
			{
				if(mediaTreeview.Nodes[_treeViewFolderIndex].Nodes[i].Nodes.Count == 0) mediaTreeview.Nodes[_treeViewFolderIndex].Nodes.Remove(mediaTreeview.Nodes[_treeViewFolderIndex].Nodes[i]);
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

		private void mediaTreeview_ItemDrag(object sender, ItemDragEventArgs e)
		{
			List<string> selectedMediaPaths = new List<string>();
			foreach (var node in mediaTreeview.SelectedNodes) selectedMediaPaths.Add(node.Name);

			DataObject data = new DataObject(DataFormats.FileDrop, selectedMediaPaths.ToArray());
			mediaTreeview.DoDragDrop(data, DragDropEffects.Copy);
			selectedMediaPaths.Clear();
		}
		#endregion

		#region UpdateTreeview

		// Updates Treeview when mouse enters treeview control. This is done as the user may of added
		// files to a Windows folder that is listed in the tree view.
		private void mediaTreeview_MouseEnter(object sender, EventArgs e)
		{
			//UpdateMediaLibrary();
		}

		private void UpdateMediaLibrary()
		{
			foreach (TreeNode folderNodes in mediaTreeview.Nodes[_treeViewFolderIndex].Nodes)
			{
				// Updates all the Folder paths.
				string[] files = Directory.GetFiles(folderNodes.Name);
				AddFilesToList(files, folderNodes.Name, true);
			}
		}

		#endregion

		#region Load_Save

		// Load Data
		private void PopulateTreeNode()
		{
			// Load Node data and check each reference and remove any that are not found.
			_nodeCount = 0;

			_mediaFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen", "MediaLibrary.xml");

			if (File.Exists(_mediaFilePath))
			{
				_nodes = new Dictionary<string, string>();
				Load_MediaLibraryFile();
				_nodes = _nodes.Reverse().ToDictionary(x => x.Key, x => x.Value);

				foreach (var node in _nodes)
				{
					if (node.Value == "Folder")
					{
						string[] files = Directory.GetFiles(node.Key);
						AddFilesToList(files, node.Key, true);
					}
					else
					{
						if (node.Value == "Root" || node.Value == "Folder" || !File.Exists(node.Key)) continue;
						string[] files = { node.Key };
						string folderPath = Path.GetDirectoryName(node.Key);
						AddFilesToList(files, folderPath, false);
					}
				}
			}

		}

		public void Load_MediaLibraryFile()
		{
			if (File.Exists(_mediaFilePath))
			{
				using (FileStream reader = new FileStream(_mediaFilePath, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer ser = new DataContractSerializer(typeof(Dictionary<string, string>));
					_nodes = (Dictionary<string, string>)ser.ReadObject(reader);
				}
			}
		}

		// Save Data
		private void MediaLibraryForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			_nodes.Clear();
			SaveNodeData(mediaTreeview.Nodes);
			Save_MediaLibraryFile();
		}

		public void Save_MediaLibraryFile()
		{
			var xmlsettings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
			};

			DataContractSerializer dataSer = new DataContractSerializer(typeof(Dictionary<string, string>));
			var dataWriter = XmlWriter.Create(_mediaFilePath, xmlsettings);
			dataSer.WriteObject(dataWriter, _nodes);
			dataWriter.Close();
		}

		private void SaveNodeData(TreeNodeCollection Nodes)
		{
			foreach (TreeNode node in Nodes)
			{
				if (node.Tag.ToString() != "Root")
				{
					if(node.Parent.Tag.ToString() != "Folder")
						_nodes.Add(node.Name, node.Tag.ToString());
				}
				SaveNodeData(node.Nodes);
			}
		}

		#endregion

	}
}
