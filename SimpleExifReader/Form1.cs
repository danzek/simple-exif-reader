using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MetadataExtractor;

namespace SimpleExifReader
{
    public partial class FormMain : Form
    {
        private string gmUrl = null;

        public FormMain()
        {
            InitializeComponent();
            pictureBox1.AllowDrop = true;
            pictureBox1.DragEnter += new DragEventHandler(pictureBox1_DragEnter);
            pictureBox1.DragDrop += new DragEventHandler(pictureBox1_DragDrop);
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult r = openFileDialog1.ShowDialog();
            if (r == DialogResult.OK)
            {
                handleImage(openFileDialog1.FileName);
            }
        }

        private void handleImage(string filename)
        {
            try
            {
                // load exif data
                IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(filename);

                string GpsLat = null;
                string GpsLatRef = null;
                string GpsLong = null;
                string GpsLongRef = null;
                gmUrl = null;

                // set image in picturebox
                pictureBox1.Image = Image.FromFile(filename);

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.Columns.Add("ParentCategory", "Parent Category");
                dataGridView1.Columns.Add("TagName", "Tag Name");
                dataGridView1.Columns.Add("TagValue", "Tag Value");

                foreach (var directory in directories)
                {
                    foreach (var tag in directory.Tags)
                    {
                        dataGridView1.Rows.Add(directory.Name, tag.Name, tag.Description);

                        if (directory.Name.Trim() == "GPS")
                        {
                            switch (tag.Name.Trim())
                            {
                                case "GPS Latitude":
                                    GpsLat = tag.Description.Trim().Replace(" ", "").Replace("-", "");
                                    break;
                                case "GPS Latitude Ref":
                                    GpsLatRef = tag.Description.Trim().Replace(" ", "").Replace("-", "");
                                    break;
                                case "GPS Longitude":
                                    GpsLong = tag.Description.Trim().Replace(" ", "").Replace("-", "");
                                    break;
                                case "GPS Longitude Ref":
                                    GpsLongRef = tag.Description.Trim().Replace(" ", "").Replace("-", "");
                                    break;
                            }
                        }
                    }

                    if (directory.HasError)
                    {
                        foreach (var error in directory.Errors)
                            dataGridView1.Rows.Add("ERROR", "Error Message", error);
                    }

                    dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridView1.AutoResizeColumns();
                }

                if (GpsLat != null && GpsLatRef != null && GpsLong != null && GpsLongRef != null)
                {
                    gmUrl = String.Format("https://www.google.com/maps/place/{0}{1}+{2}{3}", GpsLat, GpsLatRef, GpsLong, GpsLongRef);
                }
            }
            catch (MetadataExtractor.ImageProcessingException)
            {
                MessageBox.Show(String.Format("Simple EXIF Reader was unable to extract EXIF metadata from the file located at {0}", filename));
            }
        }

        void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files[0] != null)
                handleImage(files[0]);  // handle first file only for now (no planned support for multiple drop)
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
        }

        private void gmapsPictureBox_Click(object sender, EventArgs e)
        {
            if (gmUrl != null)
                Process.Start(gmUrl);
            else
                MessageBox.Show("No recognized GPS data available.");
        }
    }
}
