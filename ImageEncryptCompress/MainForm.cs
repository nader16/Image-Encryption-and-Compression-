using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;


        private void btnOpen_Click(object sender, EventArgs e)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
                txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
                txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
            }
            else if (ImageMatrix != null)
            {
                txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
                txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
            }
            else
                MessageBox.Show(" Choose Image ");
            s.Stop();
            txtTime.Text = Convert.ToString(s.Elapsed);
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            if (ImageMatrix != null && txtIntialSeed.Text.Length > 0 && txtTape.Text.Length > 0)
            {
                ImageOperations.Encrypt_Decrypt(ref ImageMatrix, txtIntialSeed.Text, Convert.ToInt32(txtTape.Text));
                label4.Text = "Encrypt Image";
                ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
            }
            else if ((txtIntialSeed.Text.Length == 0 || txtTape.Text.Length == 0) && ImageMatrix != null)
            {
                MessageBox.Show("Enter Intial Seed & Tape Position");
            }
            else
                MessageBox.Show(" Open Image ");
            s.Stop();
            txtTime.Text = Convert.ToString(s.Elapsed);
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            if (ImageMatrix != null && txtIntialSeed.Text.Length > 0 && txtTape.Text.Length > 0)
            {
                ImageOperations.Encrypt_Decrypt(ref ImageMatrix, txtIntialSeed.Text, Convert.ToInt32(txtTape.Text));
                label4.Text = "Decrypt Image";
                ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
            }
            else if ((txtIntialSeed.Text.Length == 0 || txtTape.Text.Length == 0) && ImageMatrix != null)
                MessageBox.Show("Enter Intial Seed & Tape Position");
            else
                MessageBox.Show(" Open Image ");
            s.Stop();
            txtTime.Text = Convert.ToString(s.Elapsed);
        }

        private void btnHuffman_Click(object sender, EventArgs e)
        {
            ImageOperations.Total_Bits = 0;
            Stopwatch s = new Stopwatch();
            s.Start();
            if (ImageMatrix != null)
                ImageOperations.Huffman_Code(ref ImageMatrix);
            else
                MessageBox.Show(" Open Image ");
            s.Stop();
            txtTime.Text = Convert.ToString(s.Elapsed);
            MessageBox.Show("File Has been saved  ");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ImageOperations.Huffman_Code(ref ImageMatrix);
            Stopwatch s = new Stopwatch();
            s.Start();
            ImageOperations.str_1(ImageMatrix);
            ImageOperations.compress_image(ImageOperations.R, ImageOperations.G, ImageOperations.B, ImageOperations.arr, ImageOperations.arr1, ImageOperations.arr2, Convert.ToInt32(txtTape.Text), txtIntialSeed.Text, Convert.ToInt32(txtWidth.Text), Convert.ToInt32(txtHeight.Text));
            s.Stop();
            txtTime.Text = Convert.ToString(s.Elapsed);
            MessageBox.Show("File Has Been Saved ");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            ImageMatrix = ImageOperations.decompress_image();
            s.Stop();
            txtTime.Text = Convert.ToString(s.Elapsed);
            txtIntialSeed.Text = ImageOperations.Initial_Seed;
            txtTape.Text = ImageOperations.Tape_Position.ToString();
            ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            MessageBox.Show("Decompressed Done ");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Save";
            saveFileDialog1.Filter = "Bitmap (.bmp)|*.bmp|Gif (.gif)|*.gif|JPG (.jpg)|*.jpg |JPEG (.jpeg)|*.jpeg |Png (.png)|*.png |Tiff (.tiff)|*.tiff |Wmf (.wmf)|*.wmf";
            saveFileDialog1.DefaultExt = "bmp";
            saveFileDialog1.FileName = "Image.bmp";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog1.FileName);
            }
        }
    }
}