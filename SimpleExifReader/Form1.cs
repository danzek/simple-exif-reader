using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MetadataExtractor;

namespace SimpleExifReader
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult r = openFileDialog1.ShowDialog();
            if (r == DialogResult.OK)
            {
                try
                {
                    // load exif data
                    IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(openFileDialog1.FileName);

                    string GpsLat = null;
                    string GpsLatRef = null;
                    string GpsLong = null;
                    string GpsLongRef = null;

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
                                        GpsLat = tag.Name.Trim().Replace(" ", "");
                                        break;
                                    case "GPS Latitude Ref":
                                        GpsLatRef = tag.Name.Trim().Replace(" ", "");
                                        break;
                                    case "GPS Longitude":
                                        GpsLong = tag.Name.Trim().Replace(" ", "");
                                        break;
                                    case "GPS Longitude Ref":
                                        GpsLongRef = tag.Name.Trim().Replace(" ", "");
                                        break;
                                }
                            }
                        }

                        if (directory.HasError)
                        {
                            foreach (var error in directory.Errors)
                                dataGridView1.Rows.Add("ERROR", "Error Message", error);
                        }
                    }
                }
                catch (MetadataExtractor.ImageProcessingException)
                {
                    MessageBox.Show(String.Format("Simple EXIF Reader was unable to extract EXIF metadata from the file located at {0}", openFileDialog1.FileName));
                }
            }
        }
    }
}
