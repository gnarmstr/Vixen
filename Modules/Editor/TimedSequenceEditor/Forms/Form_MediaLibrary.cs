using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using WeifenLuo.WinFormsUI.Docking;
using Svg;
using VixenModules.Effect.Effect;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class MediaLibraryForm : DockContent
	{
		private static readonly string TempPath = Path.Combine(Path.GetTempPath(), "Vixen", "MediaLibrary");
		private TreeNode _selectedItem = new TreeNode();
		private int _processed;
		private static readonly string MediaFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen", "MediaLibrary.xml");
		private Dictionary<string, string> _nodes;
		private int _treeViewFolderIndex;
		private readonly int _pictureBoxHeightAdjust = 60;
		private bool _sortRequired;
		private MediaSettings _mediaSettings;
		private int _filesExist;

		// Grabs the supported file extensions for Pictures and Videos.
		private string[] SupportedVideoExtensions => StandardMediaExtensions.VideoExtensions;
		private string[] SupportedImageExtensions => StandardMediaExtensions.ImageExtensions;

		public MediaLibraryForm()
		{
			InitializeComponent();
			contextMenuStripMedia.Renderer = new ThemeToolStripRenderer();
			Icon = Resources.Icon_Vixen3;
			ThemeUpdateControls.UpdateControls(this);
		}
		private void MediaLibraryForm_Load(object sender, EventArgs e)
		{
			_processed = 0;
			pictureBox.Height = pictureBox.Height + _pictureBoxHeightAdjust;
			// Sets the Folder Node index. Used throughout so its dynamic if more root nodes are added to the TreeView design.
			for (int i = 0; i < mediaTreeview.Nodes.Count; i++)
			{
				if (mediaTreeview.Nodes[i].Name == "Folders") _treeViewFolderIndex = i;
			}
			PopulateTreeNode();
		}

		#region GetFilesFolders

		// Get all files within a Windows folder.
		private void toolStripMenuItemAddMediaFolder_Click(object sender, EventArgs e)
		{
			OpenFileDialog folderBrowser = new OpenFileDialog();
			// Set validate names and check file exists to false otherwise windows will
			// not let you select "Folder Selection."
			folderBrowser.ValidateNames = false;
			folderBrowser.CheckFileExists = false;
			folderBrowser.CheckPathExists = true;
			// Always default to Folder Selection.
			folderBrowser.FileName = "Folder Selection.";
			if (folderBrowser.ShowDialog() == DialogResult.OK)
			{
				string folderPath = Path.GetDirectoryName(folderBrowser.FileName);
				string[] files = Directory.GetFiles(folderPath);

				// If Folder alreay exists then no need to continue and let the user know.
				if (CheckIfFileExists(folderPath))
				{
					var messageBox = new MessageBoxForm("The selected Folder already exists in the media library and will not be added.", "Information", MessageBoxButtons.OK, SystemIcons.Information);
					messageBox.ShowDialog();
					return;
				}

				using (TextDialog textDialog = new TextDialog("Enter Folder name", "Folder Name", Path.GetFileNameWithoutExtension(folderPath)))
				{
					if (textDialog.ShowDialog() == DialogResult.OK)
					{
						var newName = textDialog.Response == string.Empty ? folderPath : textDialog.Response;
						AddNewFolderNode(newName, folderPath);
					}
					else
					{
						return;
					}
				}
				AddFilesToList(files, folderPath, true);
			}
		}

		private void AddNewFolderNode(string newName, string folderPath)
		{
			TreeNode folderNode = new TreeNode();
			folderNode.Text = newName;
			folderNode.Tag = "Folder";
			folderNode.Name = folderPath;
			folderNode.ToolTipText = folderPath;
			mediaTreeview.Nodes[_treeViewFolderIndex].Nodes.Add(folderNode);
			int folderIndex = mediaTreeview.Nodes[_treeViewFolderIndex].Nodes.Count - 1;
			foreach (TreeNode mediaNodeName in mediaTreeview.Nodes)
			{
				if (mediaNodeName.Name == "Folders") continue;
				AddNewMediaNode(folderIndex, mediaNodeName.Name);
			}
		}

		private void AddNewMediaNode(int folderIndex, string mediaNodeName)
		{
			TreeNode mediaNode = new TreeNode();
			mediaNode.Text = mediaNodeName;
			mediaNode.Tag = "Root";
			mediaNode.Name = mediaNodeName;
			mediaTreeview.Nodes[_treeViewFolderIndex].Nodes[folderIndex].Nodes.Add(mediaNode); 
		}

		// Get Windows selected files. 
		private void toolStripMenuItemAddMediaFiles_Click(object sender, EventArgs e)
		{
			OpenFileDialog folderBrowser = new OpenFileDialog();
			folderBrowser.Multiselect = true;
			if (folderBrowser.ShowDialog() == DialogResult.OK)
			{
				string folderPath = Path.GetDirectoryName(folderBrowser.FileName);
				string[] files = folderBrowser.FileNames;
				AddFilesToList(files, folderPath, false);

				// If Folder already exists then no need to continue, so let the user know.
				if (_filesExist > 0)
				{
					var messageBox = new MessageBoxForm($"{_filesExist} files already exists in the media library and will not be added.", "Information", MessageBoxButtons.OK, SystemIcons.Information);
					messageBox.ShowDialog();
				}
			}
		}
		
		private void AddFilesToList(string[] files, string folderPath, bool folder)
		{
			int folderIndex = mediaTreeview.Nodes[_treeViewFolderIndex].Nodes.Count;
			if (folder) // Do only if user grabbed entire folder.
			{
				for (int i = 0; i < mediaTreeview.Nodes[_treeViewFolderIndex].Nodes.Count; i++)
				{
					if (mediaTreeview.Nodes[_treeViewFolderIndex].Nodes[i].Name == folderPath)
					{
						folderIndex = i;
						break;
					}
				}
			}

			_filesExist = 0;
			foreach (var file in files)
			{
				if (CheckIfFileExists(file))
				{
					_filesExist++;
					continue;   // Don't add if media file already exists and report back to
								// the user later how many have not been added so they are aware.
				}

				TreeNode node = new TreeNode();
				int nodeIndex;
				var extention = Path.GetExtension(file)?.ToLower();
				if (((IList)SupportedImageExtensions).Contains(extention) && extention != ".gif")
				{
					node.Tag = "Picture";
					nodeIndex = 0;
				}
				else if (((IList)SupportedVideoExtensions).Contains(extention))
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
					node.Tag = "Shapes (SVG)";
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
				node.ToolTipText = folderPath;
				if (folder)
				{
					mediaTreeview.Nodes[_treeViewFolderIndex].Nodes[folderIndex].Nodes[nodeIndex].Nodes.Add(node);
				}
				else
				{
					mediaTreeview.Nodes[nodeIndex].Nodes.Add(node);
				}
				_sortRequired = true;
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
		private void SetThumbNailImage()
		{
			if (_processed == 1)
			{
				_processed = 0;
				return;
			}

			_processed++;
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
						if (frameValue.Visible) pictureBox.Height = pictureBox.Height + _pictureBoxHeightAdjust;
						frameValue.Visible = labelFrame.Visible = false;
						break;
					case "GIF":
						pictureBox.Image = ProcessImage();
						if (frameValue.Visible) pictureBox.Height = pictureBox.Height + _pictureBoxHeightAdjust;
						frameValue.Visible = labelFrame.Visible = false;
						break;
					case "Glediator":
						pictureBox.Image = null;
						break;
					case "Shapes (SVG)":
						pictureBox.Image = ProcessSVGShapes();
						if(frameValue.Visible) pictureBox.Height = pictureBox.Height + _pictureBoxHeightAdjust;
						frameValue.Visible = labelFrame.Visible = false;
						break;
					case "Video":
						image = ProcessVideo();
						pictureBox.Image = new Bitmap(image);
						if (!frameValue.Visible) pictureBox.Height = pictureBox.Height - _pictureBoxHeightAdjust;
						frameValue.Visible = labelFrame.Visible = true;
						break;
				}

				if (image != null) image.Dispose();
			}
			catch (Exception)
			{
				// We should never get here, but just in case. 
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
			mediaTreeview.SuspendLayout();
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
				if (mediaTreeview.Nodes[_treeViewFolderIndex].Nodes[i].Nodes.Count == 0)
				{
					mediaTreeview.Nodes[_treeViewFolderIndex].Nodes.Remove(mediaTreeview.Nodes[_treeViewFolderIndex].Nodes[i]);
					_sortRequired = true;
				}
			}
			
			pictureBox.Image = null;
			frameValue.Visible = labelFrame.Visible = false;
			mediaTreeview.ResumeLayout();
		}

		private void TraverseTree(TreeNodeCollection nodes)
		{
			for (int i = nodes.Count - 1; i >= 0; i--)
			{
				if (nodes[i].Name == "Folder") continue;
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
		// or removed files to a Windows folder that is listed in the tree view.
		private void mediaTreeview_MouseEnter(object sender, EventArgs e)
		{
			mediaTreeview.SuspendLayout();

			_sortRequired = false;
			// Will check if any files have been added or removed in the linked folders.
			UpdateMediaLibrary();

			// Will only sort if a media library node was modified.
			if (_sortRequired)
			{
				mediaTreeview.TreeViewNodeSorter = new NodeSorter();
				mediaTreeview.Sort();
				_sortRequired = false;
			}
			
			mediaTreeview.ResumeLayout();
		}

		private void UpdateMediaLibrary()
		{
			List<string> folders = new List<string>();
			for (int i = mediaTreeview.Nodes[_treeViewFolderIndex].Nodes.Count - 1; i >= 0; i--)
			{
				if (!Directory.Exists(mediaTreeview.Nodes[_treeViewFolderIndex].Nodes[i].Name))
				{
					mediaTreeview.Nodes[_treeViewFolderIndex].Nodes[i].Remove();
					_sortRequired = true;
					continue;
				}

				folders.Add(mediaTreeview.Nodes[_treeViewFolderIndex].Nodes[i].Name);
				foreach (TreeNode node in mediaTreeview.Nodes[_treeViewFolderIndex].Nodes[i].Nodes)
				{
					for (int j = node.Nodes.Count - 1; j >= 0; j--)
					{
						if (!File.Exists(node.Nodes[j].Name))
						{
							node.Nodes[j].Remove();
							_sortRequired = true;
						}
					}
				}
			}

			foreach (string folderNode in folders)
			{
				// Updates all the Folder paths.
				string[] files = Directory.GetFiles(folderNode);
				AddFilesToList(files, folderNode, true);
			}

		}
		
		class NodeSorter : IComparer
		{
			public int Compare(object x, object y)
			{
				TreeNode tx = (TreeNode) x;
				TreeNode ty = (TreeNode) y;
				if (tx.Tag.ToString() != "Root")
				{
					return String.Compare(tx.Text, ty.Text, StringComparison.OrdinalIgnoreCase) <= 0 ? -1 : 1;
				}
				return 0;
			}
		}

		#endregion
		
		#region LoadData
		
		// Load Data
		private void PopulateTreeNode()
		{
			_nodes = new Dictionary<string, string>();

			if (File.Exists(MediaFilePath))
			{
				Load_MediaLibraryFile();

				foreach (var node in _nodes)
				{
					if (Path.GetExtension(node.Key) == "")
					{
						AddNewFolderNode(node.Value, node.Key);
						string[] files = Directory.GetFiles(node.Key);
						AddFilesToList(files, node.Key, true);
					}
					else
					{
						if (!File.Exists(node.Key)) continue;
						string[] files = { node.Key };
						string folderPath = Path.GetDirectoryName(node.Key);
						AddFilesToList(files, folderPath, false);
					}
				}
				// Restore expanded state of Treeview.
				RestoreTreeViewState(mediaTreeview, _mediaSettings.ExpState);
			}
		}

		public void Load_MediaLibraryFile()
		{
			_mediaSettings = new MediaSettings();
			if (File.Exists(MediaFilePath))
			{
				using (FileStream reader = new FileStream(MediaFilePath, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer ser = new DataContractSerializer(typeof(MediaSettings));
					_mediaSettings = (MediaSettings)ser.ReadObject(reader);
				}
			}
			_nodes = _mediaSettings.MediaPaths;
		}

		#endregion

		#region SaveData
		// Save Data
		private void MediaLibraryForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_nodes != null) _nodes.Clear();
			GetNodeData(mediaTreeview.Nodes, true); // Get Folder data first.
			GetNodeData(mediaTreeview.Nodes, false); // Now get single media data (Not in folders).
			Save_MediaLibraryFile();
		}

		public void Save_MediaLibraryFile()
		{
			_mediaSettings = new MediaSettings();
			_mediaSettings.ExpState = GetAllExpandedNodesList(mediaTreeview);
			_mediaSettings.MediaPaths = new Dictionary<string, string>(_nodes);

			var xmlsettings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
			};

			DataContractSerializer dataSer = new DataContractSerializer(typeof(MediaSettings));
			var dataWriter = XmlWriter.Create(MediaFilePath, xmlsettings);
			dataSer.WriteObject(dataWriter, _mediaSettings);
			dataWriter.Close();
		}

		private void GetNodeData(TreeNodeCollection Nodes, bool folder)
		{
			foreach (TreeNode node in Nodes)
			{
				if (node.Tag.ToString() != "Root")
				{
					if (folder)
					{
						if (node.Tag.ToString() == "Folder")
						{
							_nodes.Add(node.Name, node.Text);
							continue;
						}
					}
					else
					{
						if (node.Tag.ToString() == "Folder") continue;
						_nodes.Add(node.Name, node.Text);
					}
				}
				GetNodeData(node.Nodes, folder);
			}
		}
		#endregion

		#region Store Expanded State
		private static void UpdateExpandedList(ref List<string> expNodeList, TreeNode node)
		{
			if (node.IsExpanded) expNodeList.Add(node.FullPath);
			foreach (TreeNode n in node.Nodes)
			{
				if (n.IsExpanded) UpdateExpandedList(ref expNodeList, n);
			}
		}

		private static List<string> GetAllExpandedNodesList(TreeView tree)
		{
			var expandedNodesList = new List<string>();

			foreach (TreeNode node in tree.Nodes)
			{
				UpdateExpandedList(ref expandedNodesList, node);
			}
			return expandedNodesList;
		}


		private static void ExpandNodes(TreeNode node, string nodeFullPath)
		{
			if (node.FullPath == nodeFullPath) node.Expand();
			foreach (TreeNode n in node.Nodes)
			{
				if (n.Nodes.Count > 0) ExpandNodes(n, nodeFullPath);
			}
		}

		private static void RestoreTreeViewState(TreeView tree, List<string> expandedState)
		{
			foreach (TreeNode node in tree.Nodes)
			{
				foreach (var state in expandedState)
				{
					ExpandNodes(node, state);
				}
			}
		}
		#endregion

		#region Helpers

		// Collapse All Nodes.
		private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			mediaTreeview.CollapseAll();
		}

		// Enable/Disable Context Menu items.
		private void contextMenuStripMedia_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (mediaTreeview.SelectedNode != null)
				removeToolStripMenuItem.Enabled = !mediaTreeview.SelectedNode.FullPath.Contains("Folder") || mediaTreeview.SelectedNode.Tag.ToString() == "Folder";
			openFileLocationToolStripMenuItem.Enabled = mediaTreeview.SelectedNodes.Count > 0 && mediaTreeview.SelectedNodes[0].Tag.ToString() != "Root";
		}

		// Open selected node file/folder location.
		private void openFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string previousToolTip = string.Empty;
			foreach (TreeNode node in mediaTreeview.SelectedNodes)
			{
				if (node.ToolTipText == previousToolTip) continue;
				previousToolTip = node.ToolTipText;
				string cmd = "explorer.exe";
				string arg = "/select, " + node.Name;
				Process.Start(cmd, arg);
			}
		}
		
		// Allows multiple variables to be serialized and saved to single file.
		[Serializable]
		internal class MediaSettings
		{
			public List<string> ExpState = new List<string>();
			public Dictionary<string, string> MediaPaths = new Dictionary<string, string>();
		}

		#endregion
	}

}
