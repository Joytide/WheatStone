using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWheatstone
{
    class Autostereogram : Image
    {
        //==================================================================================================================================================================================================================================================
        // CONSTRUCTOR
        //==================================================================================================================================================================================================================================================

        #region Constructor

        private Image filter;
        private Image hidden; //B&W image

        public Autostereogram(Image hidden)
        {
            this.filter = new Image("./images/autostereogramfilter.bmp");
            this.hidden = hidden; //This is for the moment a colorful image, we need to make it into a grey scale disparity map
            double zoom = Math.Max((double)hidden.Width / filter.Width, (double)hidden.Height / filter.Height); //We get the biggest difference in proportionnality to resize the filter
            if (zoom > 1)
            {
                Console.WriteLine("Zoom is >1");
                zoom = Math.Round(zoom, 2);
                this.filter.Resize(zoom);
                this.filter = new Image("autostereogramfilter_Resize_" + zoom + ".bmp");
            }

            //We initialize it with its filter because we are going to move pixels from the filter
            this.Height = filter.Height;
            this.Width = filter.Width;
            this.HeaderOffset = 54;
            this.Taille = this.Height * this.Width * 3 + this.HeaderOffset;
            this.Type = "BM";
            this.Bpp = 24;
            this.Name = "Stereo_" + hidden.Name;
            this.Matrice = new Pixel[this.Height, this.Width];
            this.ConstructMat();
            this.toString();
            this.SaveFile(this.Name, true);
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region Constructor helper

        public void ConstructMat()
        {
            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    //Console.WriteLine("i,j=" + i + "," + j);
                    if (i < hidden.Height && j < hidden.Width)
                    {
                        //Console.WriteLine("This is inside the hidden im");
                        int deltaj = 5 - (this.hidden.Matrice[i, j].R / 50); //R should be the same as B and G, the bigger the disparity the bigger the left shift
                        //Console.WriteLine("deltaj is " + deltaj);
                        if (j - deltaj > 0)
                        {
                            for (int k = 0; k <= deltaj; k++)
                            {
                                //Console.WriteLine("Writing to i,j-k:" + i + "," + (j - k));
                                this.Matrice[i, j - k] = filter.Matrice[i, j];
                            }

                        }
                        else
                        {
                            //Console.WriteLine("j-deltaj was <0");
                            this.Matrice[i, j] = filter.Matrice[i, j];
                        }
                    }
                    else
                    {
                        this.Matrice[i, j] = filter.Matrice[i, j];
                    }
                    //if(this.Matrice[i, j] == null) { Console.WriteLine("error at i,j=" + i + "," + j); Console.ReadLine(); }

                }
            }
        }

        #endregion
    }
}
