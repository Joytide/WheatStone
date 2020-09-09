using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ProjectWheatstone
{
    /// <summary>
    /// 
    /// PROJET INFORMATIQUE A2
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// </summary>
    class Program
    {

        //==================================================================================================================================================================================================================================================
        // MENU
        //==================================================================================================================================================================================================================================================

        #region Menu

        static void Main(string[] args)
        {

            Console.SetWindowSize(140, 40);

            Image ImageDeTravail = null;
            ConsoleKeyInfo cki;

            do
            {
                Console.Clear();
                string logo = System.IO.File.ReadAllText("./logo.txt");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("\n\n" + logo + "\n\n-----------------------------------------Made by 2 Students-----------------------------------------\n\n\n");

                Console.ResetColor();
                ShowMenu("Menu");

                if (ImageDeTravail != null)
                {
                    Console.Write("The current work image is ");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(ImageDeTravail.Name);
                    Console.ResetColor();
                }
                else
                {
                    Utilities.RedWriteLine("No work image yet");
                }
                Console.Write("[+] 0 : Change work image\n");

                ShowMenu("Image transformation");
                Console.Write("[+] 1: Resize Image\n"
                + "[+] 2 : Black and White conversion\n"
                + "[+] 3 : Greyscale\n"
                + "[+] 4 : Rotation\n"
                + "[+] 5 : Flip\n"
                + "[+] 6 : Invert Colours\n"
                + "[+] 7 : Cropping\n"
                + "[+] 8 : Channels\n");
                ShowMenu("Matrix Convolution");
                Console.Write("[+] 9 : Convolution: Edge Detect\n"
                + "[+] 10 : Convolution: Edge Enhance\n"
                + "[+] 11 : Convolution: Emboss\n"
                + "[+] 12 : Convolution: BoxBlur\n"
                + "[+] 13 : Convolution: Sharpen\n");
                ShowMenu("New Image");
                Console.Write("[+] 14 : Histogram\n"
                + "[+] 15 : Steganography\n"
                + "[+] 16 : Mandelbrot Fractal\n"
                + "[+] 17 : Julia's sets Fractals\n");
                ShowMenu("QRCode");
                Console.Write("[+] 18 : Create your QR Code\n");
                ShowMenu("Autostereogram");
                Console.Write("[+] 19: Create your autostereogram\n\n");

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("[+] Exercise nb > ");
                Console.ResetColor();

                int Exercice = Utilities.IntQuery(0, 19);


                switch (Exercice)
                {
                    case 0:
                        ImageDeTravail = ImageChoice(true);
                        break;
                    case 1:
                        if (ImageDeTravail == null) { ImageDeTravail = NoImageWarning(); }
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine("[+] What coefficient do you want to apply? (>1 for enlarging, <1 for shrinking)");
                        Console.Write("[+] Coefficient > ");
                        Console.ResetColor();
                        double zoom = Utilities.DoubleQuery(0, 50);
                        ImageDeTravail.Resize(zoom);
                        break;
                    case 2:
                        if (ImageDeTravail == null) { ImageDeTravail = NoImageWarning(); }
                        ImageDeTravail.BW();
                        break;
                    case 3:
                        if (ImageDeTravail == null) { ImageDeTravail = NoImageWarning(); }
                        ImageDeTravail.GreyShades();
                        break;
                    case 4:
                        if (ImageDeTravail == null) { ImageDeTravail = NoImageWarning(); }
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine("[+] What angle do you want to apply ?");
                        Console.WriteLine();
                        Console.Write("[+] Angle > ");
                        Console.ResetColor();
                        int angle = Utilities.IntQuery(0, 360);
                        Rotate ImageRotate = new Rotate(angle, ImageDeTravail);
                        ImageRotate.SaveFile(ImageDeTravail.Name + "_Rotate_" + angle, true);
                        break;
                    case 5:
                        if (ImageDeTravail == null) { ImageDeTravail = NoImageWarning(); }
                        string userinput = "";
                        while (userinput != "H" && userinput != "V")
                        {
                            Utilities.GreenWriteLine("[+] Do you want to flip the image horizontally or vertically? H|V \n>");
                            userinput = Console.ReadLine();
                            if (userinput != "H" && userinput != "V") { Utilities.RedWriteLine("[+] Hmmm, try again..."); }
                        }
                        if (userinput == "H")
                        {
                            ImageDeTravail.HorizontalFlip();
                        }
                        else
                        {
                            ImageDeTravail.VerticalFlip();
                        }
                        break;
                    case 6:
                        if (ImageDeTravail == null) { ImageDeTravail = NoImageWarning(); }
                        ImageDeTravail.InvertedColors();
                        break;
                    case 7:
                        if (ImageDeTravail == null) { ImageDeTravail = NoImageWarning(); }
                        Utilities.GreenWriteLine("[+] The dimension of your image is " + ImageDeTravail.Width + " (Width) x " + ImageDeTravail.Height + " (Height)");
                        Utilities.GreenWriteLine("[+] Enter the dimension of the cropped image");
                        int min = ImageDeTravail.Width < ImageDeTravail.Height ? ImageDeTravail.Width : ImageDeTravail.Height;
                        Console.Write("Dimension > ");
                        int dimension = Utilities.IntQuery(0, min);
                        Utilities.GreenWriteLine("[+] Enter the origin coordinate of the cropped image");
                        Console.Write("x > ");
                        int x = Utilities.IntQuery(0, ImageDeTravail.Width - dimension);
                        Console.Write("y > ");
                        int y = Utilities.IntQuery(0, ImageDeTravail.Height - dimension);
                        ImageDeTravail.Crop(dimension, dimension, x, y);
                        break;
                    case 8:
                        if (ImageDeTravail == null) { ImageDeTravail = NoImageWarning(); }
                        ImageDeTravail.Channels();
                        break;
                    case 9:
                        if (ImageDeTravail == null) { ImageDeTravail = NoImageWarning(); }
                        Convolution ImageDeTravail2 = new Convolution(ImageDeTravail, "Edge_detect");
                        ImageDeTravail2.SaveFile(ImageDeTravail.Name + "_Edge_detect", true);
                        break;
                    case 10:
                        if (ImageDeTravail == null) { ImageDeTravail = NoImageWarning(); }
                        Convolution ImageDeTravail3 = new Convolution(ImageDeTravail, "Edge_enhance");
                        ImageDeTravail3.SaveFile(ImageDeTravail.Name + "_Edge_enhance", true);
                        break;
                    case 11:
                        if (ImageDeTravail == null) { ImageDeTravail = NoImageWarning(); }
                        Convolution ImageDeTravail4 = new Convolution(ImageDeTravail, "Emboss");
                        ImageDeTravail4.SaveFile(ImageDeTravail.Name + "_Emboss", true);
                        break;
                    case 12:
                        if (ImageDeTravail == null) { ImageDeTravail = NoImageWarning(); }
                        Convolution ImageDeTravail5 = new Convolution(ImageDeTravail, "BoxBlur");
                        ImageDeTravail5.SaveFile(ImageDeTravail.Name + "_BoxBlur", true);
                        break;
                    case 13:
                        if (ImageDeTravail == null) { ImageDeTravail = NoImageWarning(); }
                        Convolution ImageDeTravail7 = new Convolution(ImageDeTravail, "Sharpen");
                        ImageDeTravail7.SaveFile(ImageDeTravail.Name + "_Sharpen", true);
                        break;
                    case 14:
                        if (ImageDeTravail == null) { ImageDeTravail = NoImageWarning(); }
                        Histogram H = new Histogram(ImageDeTravail);
                        H.SaveFile(ImageDeTravail.Name + "_Histogram", true);
                        break;
                    case 15:
                        if (ImageDeTravail == null) { ImageDeTravail = NoImageWarning(); }
                        Steganography S = new Steganography(ImageDeTravail);
                        Utilities.GreenWriteLine("[+] Do you want to encode or decode the image? E|D ");
                        string input = "";
                        while (input != "E" && input != "D")
                        {
                            input = Console.ReadLine();
                            if (input != "E" && input != "D")
                            {
                                Utilities.RedWriteLine("[!] Hmm, try again?");
                            }
                        }

                        if (input == "E")
                        {
                            Image hidden = ImageChoice(false);
                            Utilities.GreenWriteLine("[+] How many significant bits do you want to hide? ");
                            int sbits = Utilities.IntQuery(1, 7);
                            S.Encode(hidden, sbits);
                            break;
                        }
                        else
                        {
                            S.Decode();
                            break;
                        }

                    case 16:
                        Image.MandelBrot();
                        break;
                    case 17:
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine("[+] Julia's set c number selection");
                        Console.WriteLine("[+] c = a + ib");
                        Console.WriteLine("A few set you may enjoy : \n >> -0,4 + 0,6i  \n >> -0,54 + 0,54i \n >> 0,285 + 0,01i  \n >> -0,8i \n ");
                        Console.Write("a > "); //We let the user choose the complex he wants to choose
                        double Re = Convert.ToDouble(Console.ReadLine());
                        Console.Write("b > ");
                        double Im = Convert.ToDouble(Console.ReadLine());
                        Console.ResetColor();
                        Image.Julia(Re, Im);
                        break;
                    case 18:
                        bool correctalphanum = false;  //Set it to false
                        while (!correctalphanum)   //Repeat until true
                        {
                            string phrase = "";
                            Utilities.GreenWriteLine("Write the sentence you want to encode >");
                            phrase = Console.ReadLine().ToUpper();
                            char[] alphanum = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '1', '2', '3', '4', '5', '6', '7', '8', '9','0', ' ', '$', '%', '*', '+', '-', '.', '/', ':' };
                            correctalphanum = true;   //Set it to true
                            for (int i = 0; i < phrase.Length; i++)
                            {
                                if (!alphanum.Contains(phrase[i]))  //If any characters are not in the alphanum table, we set it to false and break
                                {
                                    correctalphanum = false;
                                    break;
                                }
                            }   
                            if (correctalphanum)
                            {
                                QRCode qrcode = new QRCode(phrase);
                            }
                            else
                            {
                                Utilities.RedWriteLine("[!] I'm sorry, I only know how to do alphanumeric QRCode, try again with only alphanumeric values please");
                            }
                        }
                        
                        
                        break;
                    case 19:
                        Utilities.GreenWriteLine("A black and white or grey scaled image is required for the autostereogram disparity mapping, please try the image stereotest.bmp for best results");
                        if (ImageDeTravail == null) { ImageDeTravail = NoImageWarning(); }
                        Image autostereo = new Autostereogram(ImageDeTravail);
                        break;
                    default:
                        Utilities.RedWriteLine("[!] Sorry, i don't know this exercise yet... try again");
                        break;
                }
                Utilities.GreenWriteLine("[+] Press esc to exit, press anything else to return to the menu");
                cki = Console.ReadKey();
            } while (cki.Key != ConsoleKey.Escape);

        }

        #endregion

        //==================================================================================================================================================================================================================================================
        // MENU FUNCTIONS
        //==================================================================================================================================================================================================================================================

        #region Menu functions

        static void ShowMenu(string word)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            string menu = "[" + word + "]";
            while (menu.Length < 90)
            {
                menu = "-" + menu;
                if (menu.Length < 90) { menu += "-"; }
            }

            Console.WriteLine(menu);
            Console.ResetColor();
        }



        static Image NoImageWarning()
        {
            Utilities.RedWriteLine("[!] You need a work image for this function, please select one:");
            Image ret = ImageChoice(true);
            return ret;
        }



        /// <summary>
        /// Propose to the user a list of images actually in the debug folder
        /// </summary>
        /// <param name="wipe"> if the user want to wipe the list of images</param>
        /// <returns></returns>
        static Image ImageChoice(bool wipe)
        {
            Utilities.GreenWriteLine("[+] Here are all the available images\n");
            string[] files = Directory.GetFiles("./images/");
            string[] filenames = new string[files.Length];//Should we do a dictionnary and have the keys match the extensions? Would be useful in case 2 imagse with the same name exist in different formats
            for (int i = 0; i < filenames.Length; i++)
            {
                string[] temp = files[i].Split('/');
                string[] fileinfo = temp[2].Split('.');//2nd index since path or given like this: ./images/name.extension
                string name = fileinfo[0];
                filenames[i] = name; //keep the names in a list for sanity checks
                string extension = fileinfo[1];
                Console.WriteLine(name + "(" + extension.ToUpper() + " file format)");
            }
            Console.WriteLine();
            if (wipe) { FolderWipe(); } //If the debug is messy the user can wipe the list and find itself back to the orignal images
            string input = "";
            while (!filenames.Contains(input)) //Let the user choose his image
            {
                Utilities.GreenWriteLine("\nImage Choice >");
                input = Console.ReadLine();
                if (!filenames.Contains(input)) { Utilities.RedWriteLine("[!] We do not have this image, sorry..."); }
            }
            string nomimage = "./images/" + input + ".bmp";
            return new Image(nomimage);
        }



        /// <summary>
        /// Propose to the user to reset the list of images proposed at the start of the program (in case it's messy)
        /// </summary>
        static void FolderWipe()
        {
            Utilities.GreenWriteLine("[+] Do you want to reinitialize the folder of images ?");
            bool answer = Utilities.BoolQuery();
            if (answer)
            {
                string[] files = Directory.GetFiles("./images/");
                //Delete all the files in the debug
                for (int i = 0; i < files.Length; i++)
                {
                    File.Delete(files[i]);
                }
                //Copy the original images in the debug from a folder which contains them
                string[] basefiles = Directory.GetFiles("./images_de_base/");
                string[] filenames = new string[basefiles.Length];
                Utilities.GreenWriteLine("\n[+] Here are all the available images\n");
                for (int i = 0; i < filenames.Length; i++)
                {
                    string[] temp = basefiles[i].Split('/');
                    string[] fileinfo = temp[2].Split('.'); //2nd index since path or given like this: ./images/name.extension
                    string name = fileinfo[0];
                    filenames[i] = name; //keep the names in a list for sanity checks
                    File.Copy(basefiles[i], ".//images//" + name + ".bmp");
                    string extension = fileinfo[1];
                    Console.WriteLine(name + "(" + extension.ToUpper() + " file format)");
                }
            }
            Console.WriteLine();
        }

        #endregion

    }
}
