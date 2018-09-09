namespace VixenModules.Editor.TimedSequenceEditor
{
	partial class MediaLibraryForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Picture");
			System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Video");
			System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("GIF");
			System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Shapes (SVG)");
			System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Glediator");
			System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Folders");
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MediaLibraryForm));
			this.toolTipFindEffects = new System.Windows.Forms.ToolTip(this.components);
			this.contextMenuStripMedia = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemAddMediaFolder = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemAddMediaFiles = new System.Windows.Forms.ToolStripMenuItem();
			this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.labelFrame = new System.Windows.Forms.Label();
			this.frameValue = new Common.Controls.ControlsEx.ValueControls.HValueScrollBar();
			this.mediaTreeview = new Common.Controls.MultiSelectTreeview();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.contextMenuStripMedia.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuStripMedia
			// 
			this.contextMenuStripMedia.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.contextMenuStripMedia.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAddMediaFolder,
            this.toolStripMenuItemAddMediaFiles,
            this.removeToolStripMenuItem});
			this.contextMenuStripMedia.Name = "contextMenuStrip1";
			this.contextMenuStripMedia.Size = new System.Drawing.Size(199, 76);
			// 
			// toolStripMenuItemAddMediaFolder
			// 
			this.toolStripMenuItemAddMediaFolder.Name = "toolStripMenuItemAddMediaFolder";
			this.toolStripMenuItemAddMediaFolder.Size = new System.Drawing.Size(198, 24);
			this.toolStripMenuItemAddMediaFolder.Text = "Add Media Folder";
			this.toolStripMenuItemAddMediaFolder.Click += new System.EventHandler(this.toolStripMenuItemAddMediaFolder_Click);
			// 
			// toolStripMenuItemAddMediaFiles
			// 
			this.toolStripMenuItemAddMediaFiles.Name = "toolStripMenuItemAddMediaFiles";
			this.toolStripMenuItemAddMediaFiles.Size = new System.Drawing.Size(198, 24);
			this.toolStripMenuItemAddMediaFiles.Text = "Add Media Files";
			this.toolStripMenuItemAddMediaFiles.Click += new System.EventHandler(this.toolStripMenuItemAddMediaFiles_Click);
			// 
			// removeToolStripMenuItem
			// 
			this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
			this.removeToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
			this.removeToolStripMenuItem.Text = "Remove";
			this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
			// 
			// pictureBox
			// 
			this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox.Location = new System.Drawing.Point(24, 6);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(100, 100);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox.TabIndex = 3;
			this.pictureBox.TabStop = false;
			// 
			// labelFrame
			// 
			this.labelFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelFrame.AutoSize = true;
			this.labelFrame.Location = new System.Drawing.Point(47, 109);
			this.labelFrame.Name = "labelFrame";
			this.labelFrame.Size = new System.Drawing.Size(52, 17);
			this.labelFrame.TabIndex = 5;
			this.labelFrame.Text = "Frame:";
			this.labelFrame.Visible = false;
			// 
			// frameValue
			// 
			this.frameValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.frameValue.Location = new System.Drawing.Point(12, 130);
			this.frameValue.Maximum = 1000;
			this.frameValue.Minimum = 1;
			this.frameValue.Name = "frameValue";
			this.frameValue.Size = new System.Drawing.Size(124, 23);
			this.frameValue.TabIndex = 4;
			this.frameValue.Text = "hValueScrollBar1";
			this.frameValue.Value = 1;
			this.frameValue.Visible = false;
			this.frameValue.ValueChanged += new Common.Controls.ControlsEx.ValueControls.ValueChangedEH(this.frameValue_ValueChanged);
			// 
			// mediaTreeview
			// 
			this.mediaTreeview.AllowDrop = true;
			this.mediaTreeview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mediaTreeview.ContextMenuStrip = this.contextMenuStripMedia;
			this.mediaTreeview.CustomDragCursor = null;
			this.mediaTreeview.DragDefaultMode = ((System.Windows.Forms.DragDropEffects)(((System.Windows.Forms.DragDropEffects.Copy | System.Windows.Forms.DragDropEffects.Move) 
            | System.Windows.Forms.DragDropEffects.Scroll)));
			this.mediaTreeview.DragDestinationNodeBackColor = System.Drawing.SystemColors.Highlight;
			this.mediaTreeview.DragDestinationNodeForeColor = System.Drawing.SystemColors.HighlightText;
			this.mediaTreeview.DragSourceNodeBackColor = System.Drawing.SystemColors.ControlLight;
			this.mediaTreeview.DragSourceNodeForeColor = System.Drawing.SystemColors.ControlText;
			this.mediaTreeview.Location = new System.Drawing.Point(3, 3);
			this.mediaTreeview.Name = "mediaTreeview";
			treeNode1.Name = "Picture";
			treeNode1.Tag = "Root";
			treeNode1.Text = "Picture";
			treeNode2.Name = "Video";
			treeNode2.Tag = "Root";
			treeNode2.Text = "Video";
			treeNode3.Name = "GIF";
			treeNode3.Tag = "Root";
			treeNode3.Text = "GIF";
			treeNode4.Name = "Shapes (SVG)";
			treeNode4.Tag = "Root";
			treeNode4.Text = "Shapes (SVG)";
			treeNode5.Name = "Glediator";
			treeNode5.Tag = "Root";
			treeNode5.Text = "Glediator";
			treeNode6.Name = "Folders";
			treeNode6.Tag = "Root";
			treeNode6.Text = "Folders";
			this.mediaTreeview.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6});
			this.mediaTreeview.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("mediaTreeview.SelectedNodes")));
			this.mediaTreeview.Size = new System.Drawing.Size(142, 109);
			this.mediaTreeview.TabIndex = 1;
			this.mediaTreeview.UsingCustomDragCursor = false;
			this.mediaTreeview.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.mediaTreeview_ItemDrag);
			this.mediaTreeview.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.mediaTreeview_Click);
			this.mediaTreeview.MouseEnter += new System.EventHandler(this.mediaTreeview_MouseEnter);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.pictureBox);
			this.panel1.Controls.Add(this.frameValue);
			this.panel1.Controls.Add(this.labelFrame);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 118);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(148, 165);
			this.panel1.TabIndex = 6;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.mediaTreeview);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(148, 118);
			this.panel2.TabIndex = 7;
			// 
			// MediaLibraryForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.ClientSize = new System.Drawing.Size(148, 283);
			this.ControlBox = false;
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "MediaLibraryForm";
			this.Text = "Media Library";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MediaLibraryForm_FormClosing);
			this.Load += new System.EventHandler(this.MediaLibraryForm_Load);
			this.contextMenuStripMedia.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.ToolTip toolTipFindEffects;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripMedia;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddMediaFolder;
		private Common.Controls.MultiSelectTreeview mediaTreeview;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddMediaFiles;
		private System.Windows.Forms.PictureBox pictureBox;
		private Common.Controls.ControlsEx.ValueControls.HValueScrollBar frameValue;
		private System.Windows.Forms.Label labelFrame;
		private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
	}
}