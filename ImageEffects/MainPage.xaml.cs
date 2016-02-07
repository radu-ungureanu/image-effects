using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;


namespace ImageEffects
{
    public partial class MainPage : PhoneApplicationPage
    {
        private bool isFishEyeEffect,
                     isGrayScaleEffect,
                     isInvertEffect,
                     isBinaryEffect;
        private PhotoChooserTask photoChooser;
        private WriteableBitmap bitmapImage;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            photoChooser = new PhotoChooserTask();
            photoChooser.Completed += new EventHandler<PhotoResult>(photo_Completed);
        }

        void photo_Completed(object sender, PhotoResult e)
        {
            if (e.ChosenPhoto != null)
            {
                bitmapImage = PictureDecoder.DecodeJpeg(e.ChosenPhoto);
                image1.Source = bitmapImage;

                // No effect is applied
                isFishEyeEffect = false;
                isGrayScaleEffect = false;
                isInvertEffect = false;
                isBinaryEffect = false;
                textBox1.Text = "";
                // After the image is loaded, the buttons are activated
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).IsEnabled = true;
                (ApplicationBar.MenuItems[1] as ApplicationBarMenuItem).IsEnabled = true;
                (ApplicationBar.MenuItems[2] as ApplicationBarMenuItem).IsEnabled = true;
                (ApplicationBar.MenuItems[3] as ApplicationBarMenuItem).IsEnabled = true;
                textBlock1.Visibility = System.Windows.Visibility.Visible;
                textBox1.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            // Open the photo browser
            photoChooser.Show();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Save the picture after any effect is applied
            if (textBox1.Text != "" && (isFishEyeEffect || isGrayScaleEffect || isInvertEffect || isBinaryEffect))
            {
                byte[] byteArray;

                using (MemoryStream stream = new MemoryStream())
                {
                    bitmapImage.SaveJpeg(stream, bitmapImage.PixelWidth, bitmapImage.PixelHeight, 0, 100);
                    byteArray = stream.ToArray();
                }

                // Save the picture to Media Library
                var library = new MediaLibrary();
                library.SavePicture(textBox1.Text, byteArray);

                MessageBox.Show("Image saved to pictures album");

            }
            else
            {
                if (textBox1.Text == "")
                {
                    MessageBox.Show("Please insert a name");
                }
                else
                {
                    MessageBox.Show("Please apply an effect");
                }
            }
        }

        // Apply the Grey Effect
        private void greyEffectButton_Click(object sender, EventArgs e)
        {
            isGrayScaleEffect = true;

            Grey temp = new Grey();
            bitmapImage = temp.GreyEffect(bitmapImage);
            image1.Source = bitmapImage;
        }

        // Apply the Invert Effect
        private void invertEffectButton_Click(object sender, EventArgs e)
        {
            isInvertEffect = true;

            Invert temp = new Invert();
            bitmapImage = temp.InvertEffect(bitmapImage);
            image1.Source = bitmapImage;
        }

        // Apply the Black and White Effect
        private void blackWhiteEffectButton_Click(object sender, EventArgs e)
        {
            isBinaryEffect = true;

            BlackWhite temp = new BlackWhite();
            bitmapImage = temp.BlackWhiteEffect(bitmapImage);
            image1.Source = bitmapImage;
        }

        // Apply the Fish Eye Effect
        private void fishEyeEffectButton_Click(object sender, EventArgs e)
        {
            isFishEyeEffect = true;

            int[] ARGB = bitmapImage.Pixels;
            int[,] mat = new int[bitmapImage.PixelHeight, bitmapImage.PixelWidth];
            int i = 0, j = 0;
            for (int k = 0; k < ARGB.Length; k++)
            {
                mat[i, j] = ARGB[k];
                j++;
                if (j == bitmapImage.PixelWidth)
                {
                    i++;
                    j = 0;
                }
            }
            FishEye temp = new FishEye();
            bitmapImage = temp.FishEyeTransform(mat, bitmapImage.PixelWidth, bitmapImage.PixelHeight, 10);
            image1.Source = bitmapImage;
        }
    }
}