using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWheatstone
{
    /// <summary>
    /// Pixel class, used to represent
    /// </summary>
    public class Pixel
    {
        //==================================================================================================================================================================================================================================================
        // CONSTRUCTOR
        //==================================================================================================================================================================================================================================================

        #region Constructor

        /// <summary>
        /// Int between 0 and 255, representing 3 8 bits integers for 1 pixel, red green and blue
        /// byte would have obviously been less memory consuming, but we realized that after writing most of the image Class, so we decided since this is clearly a small project the memory can handle int...
        /// </summary>
        private int r;
        private int g;
        private int b;



        /// <summary>
        /// empty constructor
        /// </summary>
        public Pixel() { }



        /// <summary>
        /// basic constructor
        /// </summary>
        /// <param name="b"></param>
        /// <param name="g"></param>
        /// <param name="r"></param>
        public Pixel(int b, int g, int r)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            //this.pixels = new int[3] { r, g, b };  //Should this be a byte? //THIS SHOULDNT EXIST? IT ISNT CORRECT TO HAVE 2 DIFFERENT PROPERTIES TO ACCESS THE SAME DATA
        }



        /// <summary>
        /// Takes a pixel tab [b,g,r] and construct a pixel from its values
        /// </summary>
        /// <param name="pixeltab"></param>
        public Pixel(int[] pixeltab)
        {
            this.r = pixeltab[2];
            this.g = pixeltab[1];
            this.b = pixeltab[0];
        }



        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="pixel"></param>
        public Pixel(Pixel pixel)
        {
            this.r = pixel.r;
            this.g = pixel.g;
            this.b = pixel.b;
            //this.pixels = pixel.pixels;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region get & sets
        public int R
        {
            get { return this.r; }
            set { this.r = value; }
        }
        public int G
        {
            get { return this.g; }
            set { this.g = value; }
        }
        public int B
        {
            get { return this.b; }
            set { this.b = value; }
        }
        #endregion

        #endregion

        //==================================================================================================================================================================================================================================================
        // FUNCTIONS
        //==================================================================================================================================================================================================================================================

        #region Functions

        /// <summary>
        /// To string function, returns pixel as 
        /// </summary>
        /// <returns>returns a string like: (blue value,green value,red value)</returns>
        public string toString()
        {
            //if(this==null) return ("(0,0,0)");
            return ("(" + this.B + "," + this.G + "," + this.R + ")");
        }



        /// <summary>
        /// Converts a pixel to a byte array
        /// </summary>
        /// <returns>returns an array of bytes like:[blue value,green value,red value]</returns>
        public byte[] toByteArray()
        {
            byte[] array = new byte[3] { (byte)this.b, (byte)this.g, (byte)this.r };
            //Console.WriteLine("return an array of bytes: "); Program.ShowArray(array);
            return array;
        }



        /// <summary>
        /// Check if 2 pixels are equal by comparing their r g and b values
        /// </summary>
        /// <param name="a">1rst pixel</param>
        /// <param name="b">2nd pixel</param>
        /// <returns>a bool, true if the pixel are indeed equal, false otherwise</returns>
        public static bool IsEqual(Pixel a, Pixel b)
        {
            if (a.R == b.R && a.G == b.G && a.B == b.B)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        /// <summary>
        /// Copy a pixel
        /// </summary>
        /// <returns> Returns a new pixel with new memory address but same value as the self pixel</returns>
        public Pixel Copy()
        {
            return new Pixel(this.b, this.g, this.r);
        }

        #endregion
    }
}
