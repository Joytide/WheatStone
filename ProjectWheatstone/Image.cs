using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace ProjectWheatstone
{
    /*[TestMethod]
        public void TestMethod1()
        {
            int result = Utilities.IntFromEndian(new byte[] { 100, 100, 0 });
            Assert.AreEqual(25700, result);
        }
        [TestMethod]
        public void TestMethod2()
        {
            byte[] result = Utilities.EndianFromInt(25700,3);
            Assert.AreEqual(new byte[] { 100, 100, 0 }, result);
        }
     * 
     * 
     */
    /// <summary>
    /// Image class, replace the bitmap library, with constructor and basic functions, more elaborate functions are done in daughter classes
    /// </summary>
    class Image
    {
        //==================================================================================================================================================================================================================================================
        // CONSTRUCTOR
        //==================================================================================================================================================================================================================================================

        #region Constructor

        string name; //Name of the image, not mandatory, but useful for renaming afterwards
        string type; //Type, only "BM" is supported here
        int taille; //size in bytes of the image
        int width; //horizontal width/first value in pixel
        int height;  //vertical height/second value in pixel
        int bpp; //Bits per pixel, should be 24 bit for 3*8     //Bits=0 or 1  //Bytes=8bits ex: 10110001    Max byte in binary=11111111 in decimal=255 in hexa=FF
        int headeroffset; //Headeroffset, 54 for us since we are stuck with bitmap
        Pixel[,] matrice; //the pixel matrix of the image
        int hresolution; //window resolution in folder view, useless
        int vresolution;



        /// <summary>
        /// empty constructor
        /// </summary>
        public Image() { }



        /// <summary>
        /// Basic constructor of the image class, construct an image with a path
        /// </summary>
        /// <param name="filename">the filename as a string (with extension)</param>
        public Image(string filename)
        {
            byte[] bytes = File.ReadAllBytes(filename);
            //Creation of all the variables from their data at their specific index in the data byte tab
            this.type = Encoding.ASCII.GetString(bytes, 0, 2);
            this.name = filename.Split('.', '/')[filename.Split('.', '/').Length - 2];
            this.taille = Utilities.IntFromEndian(Utilities.getArray(bytes, 2, 5));
            this.width = Utilities.IntFromEndian(Utilities.getArray(bytes, 18, 21));
            this.height = Utilities.IntFromEndian(Utilities.getArray(bytes, 22, 25));
            this.headeroffset = Utilities.IntFromEndian(Utilities.getArray(bytes, 10, 13));
            this.bpp = Utilities.IntFromEndian(Utilities.getArray(bytes, 28, 29));
            this.hresolution = Utilities.IntFromEndian(Utilities.getArray(bytes, 38, 41));
            this.vresolution = Utilities.IntFromEndian(Utilities.getArray(bytes, 42, 45));

            this.matrice = getMatrice(bytes);
            //Program.ShowArray(bytes);
            Console.WriteLine("\n");

        }



        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="im">the original image</param>
        public Image(Image im)   //used to copy all file properties in steganography
        {
            this.name = im.name + "_copy";
            this.type = im.type;
            this.taille = im.taille;
            this.width = im.width;
            this.height = im.height;
            this.headeroffset = im.headeroffset;
            this.bpp = im.bpp;
            this.hresolution = im.hresolution;
            this.vresolution = im.vresolution;


            this.matrice = new Pixel[im.Matrice.GetLength(0), im.Matrice.GetLength(1)];
            for (int i = 0; i < im.Matrice.GetLength(0); i++)
            {
                for (int j = 0; j < im.Matrice.GetLength(1); j++)
                {
                    this.matrice[i, j] = new Pixel(im.Matrice[i, j]);
                    //Console.WriteLine("here");
                }
            }


        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region Constructor helper

        /// <summary>
        /// Decode a byte array into a Pixel matrix
        /// </summary>
        /// <param name="bytes">the byte array containing the data</param>
        /// <returns>the pixel matrix that is created from the data</returns>
        public Pixel[,] getMatrice(byte[] bytes)   //Still a problem, by cropping a 3x3 into a 2x2, the padding for the first line is not present
        {
            Pixel[,] matrice = new Pixel[this.height, this.width];
            int padding = (4 - ((this.Width * 3) % 4)) % 4;

            int index = this.headeroffset;
            for (int i = matrice.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = 0; j < matrice.GetLength(1); j++)
                {
                    /*
                    Console.WriteLine("index=" + index);
                    Console.WriteLine("Width: " + this.width + " Height: " + this.height + " Offset: " + this.headeroffset + " Bytes length: " + bytes.Length);
                    Console.WriteLine("i=" + i + "Trying to append a new pixel " +
                        (new Pixel(bytes[index], bytes[index + 1], bytes[index + 2])).toString() +
                        " to matrix [" + i + "," + j+ "]");
                    */

                    matrice[i, j] = new Pixel(bytes[index], bytes[index + 1], bytes[index + 2]);
                    index += 3;
                }
                if (padding != 0)
                {
                    for (int k = 0; k < padding; k++)
                    {
                        //Console.WriteLine("index " + index + " is a padding byte");
                        index += 1;

                    }
                }
            }
            return matrice;
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region get/set

        /// <summary>
        /// This is the get/set of the Image's class
        /// </summary>
        public int Taille
        {
            get { return this.taille; }
            set { this.taille = value; }
        }
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        public string Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
        public int Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
        public int HeaderOffset
        {
            get { return this.headeroffset; }
            set { this.headeroffset = value; }
        }
        public int Bpp
        {
            get { return this.bpp; }
            set { this.bpp = value; }
        }
        public int Hresolution
        {
            get { return this.hresolution; }
            set { this.hresolution = value; }
        }
        public int Vresolution
        {
            get { return this.vresolution; }
            set { this.vresolution = value; }
        }
        public Pixel[,] Matrice
        {
            get { return this.matrice; }
            set { this.matrice = value; }
        }

        #endregion

        //==================================================================================================================================================================================================================================================
        // SAVE AND SHOW AN IMAGE
        //==================================================================================================================================================================================================================================================

        #region Save and show an image

        /// <summary>
        /// Saves to /bin/Debug/images the instance of the image
        /// </summary>
        /// <param name="filename">the file name for the image</param>
        /// <param name="showim">a boolean that decide if the process start the image to show it after saving</param>
        public void SaveFile(string filename, bool showim)
        {
            string path = "./images/" + filename;
            path += ".bmp";
            //Math.DivRem(this.Width, 4, out int padding);
            int padding = (4 - ((this.Width * 3) % 4)) % 4;   //Number of bytes of padding needed

            byte[] bytes = new byte[this.taille + padding * this.Height];
            Utilities.ArrayAddBytes(bytes, Encoding.ASCII.GetBytes(this.type), 0);
            Utilities.ArrayAddInt(bytes, this.taille + padding * this.Height, 2, 4);
            Utilities.ArrayAddInt(bytes, this.width, 18, 4);
            Utilities.ArrayAddInt(bytes, this.height, 22, 4);
            Utilities.ArrayAddInt(bytes, this.headeroffset, 10, 4);
            Utilities.ArrayAddInt(bytes, this.bpp, 28, 2);
            //this.hresolution /= 2;
            //this.vresolution /= 2;

            Utilities.ArrayAddInt(bytes, this.hresolution, 38, 4);
            Utilities.ArrayAddInt(bytes, this.vresolution, 42, 4);

            //Arbitrary properties of the headers which are not saved in the properties of an Image instance:
            Utilities.ArrayAddBytes(bytes, new byte[4] { 40, 0, 0, 0 }, 14);  //size of the windows bitmap info header, which is always 40
            Utilities.ArrayAddBytes(bytes, new byte[2] { 1, 0 }, 26); //number of color planes (must be 1)
            Utilities.ArrayAddInt(bytes, (this.taille - this.headeroffset), 34, 4); //Size of the image   //Change for getlength0*getlength1*3???
            Utilities.ArrayAddInt(bytes, this.hresolution, 34, 4);
            //Console.WriteLine("Saving "+bytes.Length + " bytes\n " + this.MatricetoString() +" "+this.matrice.GetLength(0)+"x"+this.matrice.GetLength(1)+ "matrix +" + this.headeroffset);
            //Console.ReadLine();
            Utilities.ArrayAddPixels(bytes, this.matrice, this.headeroffset, padding);


            //Program.ShowArray(bytes);
            File.WriteAllBytes(path, bytes);
            Utilities.GreenWriteLine("[+] Done saving " + filename + " !");
            //Console.WriteLine("trying to access " + path);
            if (showim) { Process.Start(".\\images\\" + filename + ".bmp"); }
        }

        #endregion

        //==================================================================================================================================================================================================================================================
        // TO STRING FUNCTIONS
        //==================================================================================================================================================================================================================================================

        #region To string functions

        /// <summary>
        /// Return a string that describes the image
        /// </summary>
        /// <returns>the image as a string</returns>
        public string toString()
        {
            string a = "\n";
            a += "[+] The image is named " + this.name + "\n";
            a += "[+] Type is " + this.Type + "\n";
            a += "[+] The file size is " + this.Taille + " bytes\n";
            a += "[+] Height= " + this.Height + " pixels\n";
            a += "[+] Width= " + this.Width + " pixels\n";
            a += "[+] Headeroffset= " + this.HeaderOffset + " bytes\n";
            a += "[+] Bits per Pixel= " + this.bpp + " bits\n";
            a += "[+] Resolution is " + this.hresolution + "x" + this.vresolution + " pixel per meter\n";
            return a;
        }



        /// <summary>
        /// Tostring full that show the characteristics of the image and the full pixel matrix
        /// </summary>
        /// <returns>the image and matrix as a string</returns>
        public string toStringFull()
        {
            string a = "";
            a += this.toString();
            a += "[+] The pixel matrix is : " + this.MatricetoString() + "\n";
            return a;
        }



        /// <summary>
        /// ToString of the full pixel matrix
        /// </summary>
        /// <returns>the matrix as a string</returns>
        public string MatricetoString()
        {
            string a = "\n";
            for (int i = 0; i < this.matrice.GetLength(0); i++)
            {
                for (int j = 0; j < this.matrice.GetLength(1); j++)
                {
                    //Console.WriteLine(this.matrice[i,j]==null?"null":"not null");
                    a += this.matrice[i, j].toString();
                }
                a += "\n";
            }
            return a;
        }

        #endregion

        //==================================================================================================================================================================================================================================================
        // FUNCTIONS
        //==================================================================================================================================================================================================================================================

        #region Image effects

        /// <summary>
        /// Apply a grey effect to the image
        /// </summary>
        public void GreyShades()
        {
            Utilities.GreenWriteLine("[+] Trying to grey shade the image...");
            Image greyshade = new Image(this);
            for (int i = 0; i < greyshade.matrice.GetLength(0); i++)
            {
                for (int j = 0; j < greyshade.matrice.GetLength(1); j++)
                {
                    //To have different level of grey we do the average on each pixel
                    int grey = greyshade.matrice[i, j].R / 3 + greyshade.matrice[i, j].G / 3 + greyshade.matrice[i, j].B / 3;
                    greyshade.matrice[i, j].R = grey;
                    greyshade.matrice[i, j].G = grey;
                    greyshade.matrice[i, j].B = grey;
                }
            }
            greyshade.SaveFile(this.name + "_GreyShades", true);
            Utilities.GreenWriteLine("[+] Done grey shading"); //Add 'saved as ..."
        }



        /// <summary>
        /// Apply a black and white effet on the image
        /// </summary>
        public void BW()
        {
            Utilities.GreenWriteLine("[+] Trying to save the image in black and white only...");
            Image bw = new Image(this);
            //Do a double for which takes the average of all pixels to set moyenne max before becoming black (avoid dark or clear picture to be trash)
            for (int i = 0; i < bw.matrice.GetLength(0); i++)
            {
                for (int j = 0; j < bw.matrice.GetLength(1); j++)
                {
                    int moyenne = ((matrice[i, j].R + matrice[i, j].G + matrice[i, j].B) / 3);
                    if (moyenne > 128) //if the average is greater than 255/2 the pixel will be black
                    {
                        moyenne = 255;
                    }
                    else
                    {
                        moyenne = 0; //otherwise it will be white
                    }
                    bw.matrice[i, j] = new Pixel(Convert.ToByte(moyenne), Convert.ToByte(moyenne), Convert.ToByte(moyenne));
                }
            }
            bw.SaveFile(this.name + "_B&W", true);
            Utilities.GreenWriteLine("[+] Done black and white shading"); //Add 'saved as ..."
        }



        /// <summary>
        /// Invert the color of an image
        /// </summary>
        public void InvertedColors()
        {
            Utilities.GreenWriteLine("[+] Trying to invert the colors of the image...");
            Image inverted = new Image(this);
            for (int i = 0; i < this.matrice.GetLength(0); i++)
            {
                for (int j = 0; j < this.matrice.GetLength(1); j++)
                {
                    //We take the rest of 255 minus each RGB code 
                    inverted.matrice[i, j].R = (255 - this.matrice[i, j].R);
                    inverted.matrice[i, j].G = (255 - this.matrice[i, j].G);
                    inverted.matrice[i, j].B = (255 - this.matrice[i, j].B);
                }
            }
            inverted.SaveFile(this.name + "_InvertedColors", true);
            Utilities.GreenWriteLine("[+] Done inverting colors"); //Add 'saved as ..."
        }



        /// <summary>
        /// Apply a certain type of colour effect depending on the colour we remove
        /// Useful for steganography
        /// </summary>
        public void Channels()
        {
            Utilities.GreenWriteLine("[+] Trying to remove R G and B channels one by one...");
            Image channel = new Image(this);
            for (int i = 0; i < this.matrice.GetLength(0); i++)
            {
                for (int j = 0; j < this.matrice.GetLength(1); j++)
                {
                    //If we remove the red colour we will apply a cyan effect on the image
                    channel.matrice[i, j].R = 0;
                }
            }
            channel.SaveFile(this.name + "_ChannelRed", true);
            Utilities.GreenWriteLine("[+] Done saved without red channel");

            channel = new Image(this);
            for (int i = 0; i < this.matrice.GetLength(0); i++)
            {
                for (int j = 0; j < this.matrice.GetLength(1); j++)
                {
                    //If we remove the green colour we will apply a magenta effect on the image
                    channel.matrice[i, j].G = 0;
                }
            }
            channel.SaveFile(this.name + "_ChannelGreen", true);
            Utilities.GreenWriteLine("[+] Done saved without green channel");

            channel = new Image(this);
            for (int i = 0; i < this.matrice.GetLength(0); i++)
            {
                for (int j = 0; j < this.matrice.GetLength(1); j++)
                {
                    //If we remove the blue colour we will apply a yellow effect on the image
                    channel.matrice[i, j].B = 0;
                }
            }
            channel.SaveFile(this.name + "_ChannelBlue", true);
            Utilities.GreenWriteLine("[+] Done saved without blue channel");
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region Flip/Crop

        /// <summary>
        /// Swap Pixels in a matrix
        /// </summary>
        /// <param name="x1"> first pixel line coordinate to swap with the second pixel line coordinate</param>
        /// <param name="y1"> first pixel column coordinate to swap with the second pixel column coordinate</param>
        /// <param name="x2"> second pixel line coordinate to swap with the first pixel line coordinate</param>
        /// <param name="y2"> second pixel column coordinate to swap with the first pixel column coordinate</param>
        public void SwapPixel(int x1, int y1, int x2, int y2)
        {
            Pixel temp = new Pixel(this.matrice[x1, y1]);
            this.matrice[x1, y1] = this.matrice[x2, y2];
            this.matrice[x2, y2] = temp;
        }



        /// <summary>
        /// Miror effet horizontal sens
        /// </summary>
        public void HorizontalFlip()
        {
            Utilities.GreenWriteLine("[+] Trying to flip image horizontally...");
            Image reverse = new Image(this);   //If we don't clone the image, the bus data stays the same and we can't use the image again since it will have been altered

            for (int i = 0; i < reverse.matrice.GetLength(0); i++)
            {
                for (int j = 0; j < (reverse.matrice.GetLength(1) / 2); j++)
                {
                    reverse.SwapPixel(i, j, i, (reverse.matrice.GetLength(1) - j - 1)); //We swap the pixel between the left hand side and the right hand side
                }
            }
            reverse.SaveFile(this.name + "_HFlip", true);
            Utilities.GreenWriteLine("[+] Done Flipping"); //Add 'saved as ..."
        }



        /// <summary>
        /// Miror effect vertical sens
        /// </summary>
        public void VerticalFlip()
        {
            Utilities.GreenWriteLine("[+] Trying to flip image vertically...");
            Image reverse = new Image(this);   //If we don't clone the image, the bus data stays the same and we can't use the image again since it will have been altered

            for (int i = 0; i < reverse.matrice.GetLength(0) / 2; i++)
            {
                for (int j = 0; j < (reverse.matrice.GetLength(1)); j++)
                {
                    reverse.SwapPixel(i, j, reverse.matrice.GetLength(0) - i - 1, j); //We swap the pixel between the top and the bottom
                }
            }
            reverse.SaveFile(this.name + "_VFlip", true);
            Utilities.GreenWriteLine("[+] Done Flipping"); //Add 'saved as ..."
        }



        /// <summary>
        /// Take a part of an image
        /// </summary>
        /// <param name="newheight"> new height of the image </param>
        /// <param name="newwidth"> new width of the image </param>
        public void Crop(int newheight, int newwidth, int newx, int newy)
        {
            Image cropped = new Image(this);
            cropped.taille = newheight * newwidth * 3 + this.headeroffset;
            cropped.height = newheight;
            cropped.width = newwidth;
            cropped.matrice = new Pixel[newheight, newwidth];
            for (int i = 0; i < cropped.matrice.GetLength(0); i++)
            {
                for (int j = 0; j < cropped.matrice.GetLength(1); j++)
                {
                    cropped.matrice[i, j] = this.matrice[newy + i, newx + j];
                }
            }
            //Console.WriteLine("Cropped:" + cropped.toStringFull());
            cropped.SaveFile(this.name + "_Cropped", true);
            Utilities.GreenWriteLine("[+] Done cropping");
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region Resize

        /// <summary>
        /// Resize the image depending on the zoom value 
        /// </summary>
        /// <param name="zoom"> if  zomm >= 1 we extend the size of the image and if zoom<1 we reduce the size of the image </param>
        public void Resize(double zoom)
        {
            Utilities.GreenWriteLine("[+] Trying to resize the image with a " + zoom + " coefficient");
            Image Resize = new Image(this);
            if (zoom >= 1) //Extend image
            {
                int zoomE = Convert.ToInt32(zoom); //Better to work with an int than a double
                //Creation of a new matrix with the right dimension depending on the zoom value
                Resize.taille = 54 + ((int)((double)height * zoomE) * (int)((double)width * zoomE) * 3);
                //Add debug here
                Resize.width = width * zoomE;
                Resize.height = height * zoomE;
                Pixel[,] Extend = new Pixel[matrice.GetLength(0) * zoomE, matrice.GetLength(1) * zoomE];
                // Extending the image
                int line = 0;
                int column = 0;
                for (int i = 0; i < matrice.GetLength(0); i++)
                {
                    column = 0;
                    for (int j = 0; j < matrice.GetLength(1); j++)
                    {
                        for (int index1 = line; index1 < line + zoomE; index1++)
                        {
                            for (int index2 = column; index2 < column + zoomE; index2++)
                            {
                                Extend[index1, index2] = matrice[i, j];
                            }
                        }

                        column += zoomE;
                    }
                    line += zoomE;
                }
                //The image matrix becomes the new matrix extended of pixel
                Resize.matrice = Extend;
                for (int i = 0; i < Extend.GetLength(0); i++)
                {
                    for (int j = 0; j < Extend.GetLength(1); j++)
                    {
                        Resize.matrice[i, j] = Extend[i, j];
                    }
                }
                //Save the image depending on its new factors
                Resize.SaveFile(this.name + "_Resize_" + zoom, true);
            }

            if (zoom > 0 && zoom < 1) //Reduce image
            {
                zoom = 1 - zoom; //Lower is the zoom value, smaller will be the image 
                int zoomR = Convert.ToInt32(zoom * 10); //Better to work with an int than a double
                //Creation of a new matrix with the right dimension depending on the zoom value
                Resize.taille = 54 + (height / zoomR * width / zoomR * 3);
                Resize.width = width / zoomR;
                Resize.height = height / zoomR;
                Pixel[,] Reduce = new Pixel[(matrice.GetLength(0) / zoomR), (matrice.GetLength(1) / zoomR)];
                // Reducing the image
                int line = 0;
                int column = 0;
                for (int i = 0; i < Reduce.GetLength(0); i++)
                {
                    column = 0;
                    for (int j = 0; j < Reduce.GetLength(1); j++)
                    {
                        int Moyenne_Rouge = 0;
                        int Moyenne_Vert = 0;
                        int Moyenne_Bleu = 0;

                        for (int index1 = line; index1 < line + zoomR; index1++)
                        {
                            for (int index2 = column; index2 < column + zoomR; index2++)
                            {
                                //We take the average of the pixels around one pixel
                                Moyenne_Rouge += matrice[index1, index2].R;
                                Moyenne_Vert += matrice[index1, index2].G;
                                Moyenne_Bleu += matrice[index1, index2].B;
                            }
                        }
                        Moyenne_Rouge = (byte)(Moyenne_Rouge / (zoomR * zoomR));
                        Moyenne_Vert = (byte)(Moyenne_Vert / (zoomR * zoomR));
                        Moyenne_Bleu = (byte)(Moyenne_Bleu / (zoomR * zoomR));
                        Reduce[i, j] = new Pixel((byte)Moyenne_Bleu, (byte)Moyenne_Vert, (byte)Moyenne_Rouge); //This pixel is now the average of the pixels around it
                        column += zoomR;
                    }
                    line += zoomR;
                }
                //The image matrix becomes the new matrix reduced of pixel
                Resize.matrice = Reduce;
                for (int i = 0; i < Reduce.GetLength(0); i++)
                {
                    for (int j = 0; j < Reduce.GetLength(1); j++)
                    {
                        Resize.matrice[i, j] = Reduce[i, j];
                    }
                }
                //Save the image depending on its new factors
                Utilities.GreenWriteLine("[+] Done resizing");
                Resize.SaveFile(this.Name + "_Resize_" + (1 - zoom), true);
            }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region Mandelbrot/Julia fractal

        /// <summary>
        /// Create the MandelBrot fractal
        /// </summary>
        static public void MandelBrot()
        {
            Utilities.GreenWriteLine("[+] Creating a 1000*1000 Mandelbrot fractal");
            //Creation of the image
            Image Fractal = new Image();
            Fractal.name = "Mandelbrot";
            Fractal.type = "BM";
            Fractal.Width = 1000;
            Fractal.Height = 1000;
            Fractal.HeaderOffset = 54;
            Fractal.Taille = Fractal.Width * Fractal.Height * 3 + Fractal.headeroffset;
            Fractal.Bpp = 24;
            Fractal.Hresolution = 0;
            Fractal.Vresolution = 0;
            Fractal.Matrice = new Pixel[Fractal.Height, Fractal.Width];
            int[][] Couleurs = new int[3][] { new int[] { 0, 0, 0 }, new int[] { 232, 26, 46 }, new int[] { 255, 255, 255 } };
            //The Mandelbrot set is the set of complex numbers c
            Complex c = new Complex();
            int count = 0;
            for (int i = 0; i < Fractal.Width; i++) // real part of the complex number is represented by a displacement along the x-axis
            {
                c.Re = (double)(i - Fractal.Width / 2) / (0.25 * Fractal.Width);
                for (int j = 0; j < Fractal.Height; j++) //imaginary part of the complex number is represented by a displacement along the y-axis
                {
                    Complex z = new Complex(0, 0);
                    c.Im = (double)(j - Fractal.Height / 2) / (0.25 * Fractal.Height);
                    //z(n+1)=Zn²+c
                    do
                    {
                        z = Complex.ComplexMult(z, z);
                        z = Complex.ComplexAdd(z, c);
                        count++;
                        if (z.Module() > 2) break; //The sequence zn is not bounded if the modulus of one of its terms is greater than 2
                    }
                    while (count < 255);
                    //The number of iterations to reach a modulus greater than 2 is used to determine the color to use

                    int[] colour = new int[] { 0, 0, 0 };
                    if (count != 254) { colour = new int[] { count * 255 / 50, 0, 0 }; }
                    Fractal.Matrice[j, i] = new Pixel(colour);
                    count = 0;
                }
            }
            Fractal.SaveFile("MandelBrot", true);
        }


        /// <summary>
        /// Create the Julia fractal
        /// Depending on the complex you choose
        /// </summary>
        /// <param name="Re"> real part of the complex number </param>
        /// <param name="Im"> imaginary part of the complex number </param>
        static public void Julia(double Re, double Im)
        {
            Utilities.GreenWriteLine("[+] Creating a Julia Set with c=" + Re + "+" + Im + "i");
            //Creation of the image
            Image Fractal = new Image();
            Fractal.name = "Julia";
            Fractal.type = "BM";
            Fractal.Width = 1000;
            Fractal.Height = 1000;
            Fractal.HeaderOffset = 54;
            Fractal.Taille = Fractal.Width * Fractal.Height * 3 + Fractal.headeroffset;
            Fractal.Bpp = 24;
            Fractal.Hresolution = 0;
            Fractal.Vresolution = 0;
            Fractal.Matrice = new Pixel[Fractal.Height, Fractal.Width];
            //The Mandelbrot set is the set of complex numbers c
            Complex c = new Complex();
            c.Re = Re;
            c.Im = Im;
            int count = 0;
            for (int i = 0; i < Fractal.Width; i++) // real part of the complex number is represented by a displacement along the x-axis
            {
                for (int j = 0; j < Fractal.Height; j++) //imaginary part of the complex number is represented by a displacement along the y-axis
                {
                    Complex z = new Complex(0, 0);
                    z.Re = 1.5 * (i - Fractal.width / 2) / (0.5 * Fractal.width);
                    z.Im = (j - Fractal.height / 2) / (0.5 * Fractal.height);
                    do
                    {
                        z = Complex.ComplexMult(z, z);
                        z = Complex.ComplexAdd(z, c);
                        count++;
                        if (z.Module() > 2) break; //The sequence zn is not bounded if the modulus of one of its terms is greater than 2
                    }
                    while (count < 255);
                    //The number of iterations to reach a modulus greater than 2 is used to determine the color to use     
                    int[][] clrList = new int[3][] { new int[] { 30, 67, 249 }, new int[] { 0, 0, 0 }, new int[] { 42, 245, 245 } };
                    double clrCount = (double)clrList.Length;
                    double clr = (count * clrCount / 255) % clrCount;
                    int[] couleur1 = clrList[(int)(clr)];
                    int[] couleur2 = clrList[((int)clr + 1) % clrList.Length];
                    double clrBas = clr - (int)clr;
                    double clrHaut = 1 - clrBas;
                    int[] color = new int[3];
                    color[0] = (int)(couleur1[0] * clrBas + couleur2[0] * (clrHaut));
                    color[1] = (int)(couleur1[1] * clrBas + couleur2[1] * (clrHaut));
                    color[2] = (int)(couleur1[2] * clrBas + couleur2[2] * (clrHaut));
                    Fractal.Matrice[j, i] = new Pixel(color);
                    count = 0;
                }
            }
            Fractal.SaveFile("Julia_" + Re + "+" + Im + "i", true);

        }

        #endregion
    }
}
