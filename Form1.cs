using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public const int DefaultRotateInterval = 5000;
        public const int NewPhotoRotateInterval = 60000;

        public HashSet<string> ImagesHashSet = new HashSet<string>();
        public List<Image> PhotoList = new List<Image>();
        public Timer RefillTimer = new Timer();
        public Timer Rotate = new Timer();
        public int CurrentPhotoIndex = 0;
        public int PreviousEnd = 0;

        public Form1()
        {
            InitializeComponent();

            pictureBox1.Size = new Size(this.Width, this.Height);

            // Load all the pictures
            FillHashset();
            PreviousEnd = PhotoList.Count();

            pictureBox1.Image = PhotoList[CurrentPhotoIndex];
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            // Setup timer to rotate photos
            Rotate.Interval = DefaultRotateInterval;
            Rotate.Tick += Rotate_Tick;
            Rotate.Start();

            // Setup timer to look for new photos.  If this doesn't work we could use a file system monitor
            RefillTimer.Interval = 1000;
            RefillTimer.Tick += RefillTimer_Tick;
            RefillTimer.Start();
        }

        void Rotate_Tick(object sender, EventArgs e)
        {
            //Reset the timer inteval incase it was changed
            Rotate.Interval = DefaultRotateInterval;

            //Go to the next photo
            CurrentPhotoIndex++;

            //If this is the end of the list start from the beginning
            if (CurrentPhotoIndex == PhotoList.Count())
            {
                CurrentPhotoIndex = 0;
            }
                        
            RotateImage(CurrentPhotoIndex);
        }

        private void RotateImage(int target)
        {
            pictureBox1.Image = PhotoList[target];
        }

        void RefillTimer_Tick(object sender, EventArgs e)
        {
            FillHashset();
            CheckForNewPhotos();
        }

        public void FillHashset()
        {
            string dir = string.Format(@"C:\Users\brian\Documents\PhotoboothImages\{0}\prints", DateTime.Now.ToString("yyyy-MM-dd"));

            var files = Directory.GetFiles(dir);

            foreach (var file in files.Where(f => f.EndsWith(".jpg")))
            {
                if (ImagesHashSet.Add(file))
                {
                    try
                    {
                        PhotoList.Add(Image.FromFile(file));
                    }
                    catch { }
                }

            }            
        }

        private void CheckForNewPhotos()
        {
            //If we have a new photo.  Go to that image and increase rotate interval
            if (PhotoList.Count() != PreviousEnd)
            {
                Rotate.Stop();

                PreviousEnd = PhotoList.Count();
                CurrentPhotoIndex = 0;
                RotateImage(PreviousEnd - 1);

                Rotate.Interval = NewPhotoRotateInterval;
                Rotate.Start();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Hide();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }
    }
}
