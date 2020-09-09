using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 
/// 
/// Change to be a simple method and not class
/// 
/// </summary>

namespace ProjectWheatstone
{
    class Rotate : Image
    {

        //==================================================================================================================================================================================================================================================
        // CONSTRUCTOR
        //==================================================================================================================================================================================================================================================

        #region Constructor

        public Rotate() { }
        public Rotate(int angle, Image original)
        {

            this.HeaderOffset = original.HeaderOffset;
            this.Type = original.Type;
            this.Bpp = original.Bpp;
            this.Name = original.Name + "_Rotate";
            this.GetDimensions(angle, original);
            this.Matrice = new Pixel[this.Height, this.Width];
            this.GetMatrice(angle, original);

            Utilities.GreenWriteLine("[+] Done rotating!");
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region Constructor helper

        void GetMatrice(int angle, Image original)
        {
            float rad = (2 * (float)Math.PI * angle) / 360;   //Convert in rad because cos and sin dont work with degrees

            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);
            //Console.WriteLine("cos and sin=" + cos + " " + sin);


            for (int x = 0; x < this.Width; x++)   //getlength1
            {
                for (int y = 0; y < this.Height; y++) //
                {
                    //Translations
                    int deltax = x - this.Width / 2;
                    int deltay = y - this.Height / 2;
                    //Compute Sigma^-1*(x',y') to get (x,y)
                    int xoriginal = (int)(cos * deltax - sin * deltay) + original.Width / 2;
                    int yoriginal = (int)(sin * deltax + cos * deltay) + original.Height / 2;


                    //Console.WriteLine("x'=" + x + " y'=" + y + " [x,y]=["+xoriginal+","+yoriginal+"]");
                    //Console.WriteLine("new bounds: " + this.Matrice.GetLength(0)+this.Matrice.GetLength(1) + " old bounds: " + original.Matrice.GetLength(0)+ original.Matrice.GetLength(1));
                    if (xoriginal >= 0 && xoriginal < original.Matrice.GetLength(1) && yoriginal >= 0 && yoriginal < original.Matrice.GetLength(0))
                    {
                        //Console.WriteLine(this.toString());
                        //Console.WriteLine("Trying to append "+ original.Matrice[yoriginal, xoriginal].toString()+"at y,x");
                        //Console.WriteLine("New Matrice is " + this.MatricetoString() + "\nOld is " + original.MatricetoString());

                        this.Matrice[y, x] = original.Matrice[yoriginal, xoriginal];
                    }
                    else
                    {
                        this.Matrice[y, x] = new Pixel(0, 0, 0);
                    }
                }
            }
        }

        void GetDimensions(int angle, Image original)
        {

            double rad = (2 * Math.PI * angle) / 360;   //Convert in rad because cos and sin dont work with degrees
            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);

            //if (angle > 180) { angle -= 360; }
            //Each four quarter have different magic points, each have 2, one which x' coordinate is half of the new Width, one which y' coordinate is half of the new Heigth
            //We just have to find these 2 magic points, each are 2 corners, in the 4 cases, compute their x' and y', and replace the Width and Heigth with those
            if (angle <= 180)
            {
                if (angle <= 90) //Angle is [0,90]
                {
                    //For example:
                    this.Height = 1 + (int)(2 * (cos * original.Height / 2 - sin * -original.Width / 2));  //Bottom left corner's new y
                    this.Width = 1 + (int)(2 * (cos * original.Width / 2 + sin * original.Height / 2));    //Bottom right corner's new x
                    //With new x and y computed with the sigma*x=x' with sigma the rotation matrix
                }
                else // ]90,180]
                {
                    this.Height = 1 + (int)(2 * (cos * -original.Height / 2 + (-sin * -original.Width / 2)));   //Top left corner's new y
                    this.Width = 1 + (int)(2 * (cos * -original.Width / 2 + (sin * original.Height / 2)));    //Bottom left corner's new x
                }
            }
            else
            {
                if (angle <= 270) //]180;270]
                {
                    this.Height = 1 + (int)(2 * (cos * (-original.Height / 2 - 1) + (-sin * (original.Width / 2 - 1))));  //Top right corner's new y
                    this.Width = 1 + (int)(2 * (cos * (-original.Width / 2 - 1) + (sin * (-original.Height / 2 - 1))));  //Top left corner's new x
                }
                else //]270;360]
                {
                    this.Height = 1 + (int)(2 * (cos * (original.Height / 2 - 1) + (-sin * (original.Width / 2 - 1))));  //Bottom right corner's new y
                    this.Width = 1 + (int)(2 * (cos * (original.Width / 2 - 1) + (sin * (-original.Height / 2 - 1)))); //Top tight corner's new x
                }
            }

            int padding = 4 - (this.Width % 4);
            this.Taille = 3 * (this.Width + padding) * this.Height;

            //Console.WriteLine("new Width and Height= " + this.Width + "x" + this.Height);
        }

        #endregion

    }
}
