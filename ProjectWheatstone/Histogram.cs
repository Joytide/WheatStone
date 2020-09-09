using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWheatstone
{
    /// <summary>
    /// Daughter class of image reserved to the creation of histograms
    /// </summary>
    class Histogram : Image
    {

        //==================================================================================================================================================================================================================================================
        // CONSTRUCTOR
        //==================================================================================================================================================================================================================================================

        #region Constructor

        /// <summary>
        /// 256 lengths lists that will hold the value corresponding to the number of pixel that share the same color value as their index
        /// </summary>
        private int[] RedList;
        private int[] GreenList;
        private int[] BlueList;
        private int ValueWidth; //Giving a single value multiple pixels to avoid histogram from bigger pictures being 2000x255, which isnt very pretty



        /// <summary>
        /// Main constructor of the images, collects all the parameters
        /// </summary>
        /// <param name="im"></param>
        public Histogram(Image im)
        {
            this.RedList = new int[256];
            this.GreenList = new int[256];
            this.BlueList = new int[256];
            this.ConstructTabs(im);
            /*
            Program.ShowTab(RedList);
            Program.ShowTab(BlueList);
            Program.ShowTab(GreenList);
            */
            this.Height = (new int[3] { RedList.Max(), GreenList.Max(), BlueList.Max() }).Max();  //Could be changeable for more distinction in very colorful picture?
            //this.Width = 256; //A pixel for each color => a 3 wave histogram
            this.ValueWidth = this.Height / 256;  //For the histogram to approach a square size, now there can't be a difference of more than 255 pixel in Height and Width
            this.Width = this.ValueWidth * 256;
            this.HeaderOffset = 54;
            this.Taille = this.Height * this.Width * 3 + this.HeaderOffset;
            this.Type = "BM";
            this.Bpp = 24;

            this.Matrice = new Pixel[this.Height, this.Width];
            this.ConstructMat();

            //Console.WriteLine("Matrice length= " + this.Matrice.GetLength(0) + "," + this.Matrice.GetLength(1));
            Console.WriteLine("Done constructing histogram of " + im.Name);
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region Constructor helper

        /// <summary>
        /// Review the whole image pixel matrix and construct each tabs by incrementing the tab's value when its index correspond to the value of a pixel 
        /// </summary>
        /// <param name="im">the image we're using to create the histogram</param>
        public void ConstructTabs(Image im)
        {
            for (int i = 0; i < im.Height; i++)
            {
                for (int j = 0; j < im.Width; j++) //For each pixel, we fetch their rgb values and increment the corresponding index of their value (0 to 255 corresponding to 0/255 indexes)
                {
                    this.RedList[im.Matrice[i, j].R]++;
                    this.GreenList[im.Matrice[i, j].G]++;
                    this.BlueList[im.Matrice[i, j].B]++;

                    //To modify Height: need to add an if and keeping the max value
                }
            }
        }
        /// <summary>
        /// construct the pixel matrix of the histogram with the tabs
        /// </summary>
        public void ConstructMat()
        {
            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    Pixel temp = new Pixel((this.Height - i) <= this.BlueList[j] ? 255 : 0, (this.Height - i) <= this.GreenList[j] ? 255 : 0, (this.Height - i) <= this.RedList[j] ? 255 : 0);
                    //Set the R/G/B value to 255 if the value in the list ( the number of pixel with that specific value for r g or b, which can go from 0 to Height which is max of the 3 lists)
                    //is greater or equal than then considered (max-i) value
                    //Eg: if the 0 value for R bytes is 2 (so RedTab[0]=2 ) and Height is 3, it will set only the [1;0] and [2;0] r bytes to 255 since (3-0)=3>2, but 3-1 and 3-2 <=2
                    for (int k = 0; k < this.ValueWidth; k++)
                    {
                        this.Matrice[i, j * ValueWidth + k] = temp;
                    }

                }
            }
        }

        #endregion
    }
}
