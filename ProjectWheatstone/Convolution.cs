using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 
/// https://lodev.org/cgtutor/filtering.html
/// https://en.wikipedia.org/wiki/Kernel_(image_processing)
/// https://xphilipp.developpez.com/articles/filtres/?page=page_14
/// 
/// 
/// 
/// </summary>
namespace ProjectWheatstone
{
    /// <summary>
    /// Convolution class, daughter of the image class, used to add convolution filters to an image
    /// </summary>
    class Convolution : Image
    {

        //==================================================================================================================================================================================================================================================
        // CONSTRUCTOR
        //==================================================================================================================================================================================================================================================

        #region Constructor

        /// <summary>
        /// Characteristics of a convolution filter
        /// </summary>
        int[,] filter; //filter matrix
        int factor;  //Used to multiply the result
        int bias; //Used to add brightness, to be added after the factor multiplication



        /// <summary>
        /// simple constructor
        /// </summary>
        /// <param name="im">Image to apply the convolution filter to</param>
        /// <param name="filter">the filter that need to be applied, as a string</param>
        public Convolution(Image im, string filter)  //Add type of convolution, and search in the according db? db inside this function?
        {
            this.filter = GetFilter(filter);  //get the matrix filter
            this.factor = GetFactor(filter);  //get the multiplicative factor
            this.bias = GetBias(filter);    //Get the bias, not use for the moment but often use in some filters, to modify overall visibility by lighting up or darkening the image
            this.Height = im.Height;
            this.Width = im.Width;
            this.HeaderOffset = im.HeaderOffset;
            this.Taille = im.Taille;
            this.Type = im.Type;
            this.Bpp = im.Bpp;
            this.Matrice = new Pixel[im.Matrice.GetLength(0), im.Matrice.GetLength(1)];
            this.ConstructMat(im);
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region Constructor helper

        /// <summary>
        /// Construct the image pixel matrix with the addition of the convolution filter
        /// </summary>
        /// <param name="im">the image on which we apply the filter</param>
        public void ConstructMat(Image im)
        {

            for (int x = 0; x < im.Matrice.GetLength(0); x++)
            {
                for (int y = 0; y < im.Matrice.GetLength(1); y++)
                {
                    double red = 0;
                    double blue = 0;
                    double green = 0;

                    for (int filterx = 0; filterx < filter.GetLength(0); filterx++)  //If bug, change filterx/y and getlengths
                    {
                        for (int filtery = 0; filtery < filter.GetLength(1); filtery++)
                        {
                            int i = (x - filter.GetLength(0) / 2 + filterx + im.Height) % im.Height;   //Minus the half length of the filter matrix
                            int j = (y - filter.GetLength(1) / 2 + filtery + im.Width) % im.Width;
                            double filtervalue = filter[filterx, filtery];
                            //Console.WriteLine("im.Width=" + im.Width + "/" + im.Matrice.GetLength(1) + " im.Heigh" + im.Height + "/" + im.Matrice.GetLength(0));
                            //Console.WriteLine("i=" + i + " j=" + j);
                            red += im.Matrice[i, j].R * filtervalue / this.factor;
                            green += im.Matrice[i, j].G * filtervalue / this.factor;
                            blue += im.Matrice[i, j].B * filtervalue / this.factor;

                        }
                    }

                    this.Matrice[x, y] = new Pixel();
                    this.Matrice[x, y].R = Math.Min(Math.Max(Convert.ToInt32(red + bias), 0), 255);
                    this.Matrice[x, y].G = Math.Min(Math.Max(Convert.ToInt32(green + bias), 0), 255);
                    this.Matrice[x, y].B = Math.Min(Math.Max(Convert.ToInt32(blue + bias), 0), 255);
                }
            }
        }



        /// <summary>
        /// databases of all filters
        /// </summary>
        /// <param name="filtername">name of the filter</param>
        /// <returns>returns the filter string into its corresponding matrix</returns>
        static int[,] GetFilter(string filtername)
        {
            switch (filtername)
            {
                case "Edge_detect":
                    return new int[,] { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
                case "Edge_enhance":
                    return new int[,] { { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } };
                case "Emboss":
                    return new int[,] { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
                case "BoxBlur":
                    return new int[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
                case "GaussianBlur":
                    return new int[,] { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } };
                case "Sharpen":
                    return new int[,] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
            }
            return new int[,] { { 0, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } };
        }



        /// <summary>
        /// Returns the factor (if any) required by the matrix
        /// </summary>
        /// <param name="filtername">string of the current filer</param>
        /// <returns>the factor as an int</returns>
        static int GetFactor(string filtername)
        {
            switch (filtername)
            {
                case "BoxBlur":
                    return 9;
                case "GaussianBlur":
                    return 16;
                default:
                    return 1;
            }
        }



        /// <summary>
        /// Return the bias based on the filter, not used for the filters we have in our database in the moment
        /// </summary>
        /// <param name="filtername">string of the current filer</param>
        /// <returns>the bias number as an int</returns>
        static int GetBias(string filtername)
        {
            switch (filtername)
            {
                default:
                    return 0;
            }
        }

        #endregion
    }
}
