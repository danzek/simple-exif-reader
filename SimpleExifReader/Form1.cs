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
                // load exif data
                IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(openFileDialog1.FileName);

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.Columns.Add("ParentCategory", "ParentCategory");
                dataGridView1.Columns.Add("TagName", "TagName");
                dataGridView1.Columns.Add("TagValue", "Tag Value");

                foreach (var directory in directories)
                {
                    foreach (var tag in directory.Tags)
                    {
                        dataGridView1.Rows.Add(directory.Name, tag.Name, tag.Description);
                    }
                }
            }
        }
    }
}
