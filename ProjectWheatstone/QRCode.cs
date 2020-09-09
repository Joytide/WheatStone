using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWheatstone
{
    /// <summary>
    /// Class used to encode and decode QRCode
    /// This class inherits from the Image Class
    /// Data is needed to encode a QRCode
    /// An image is needed to decode a QRCode
    /// </summary>
    class QRCode : Image
    {
        //==================================================================================================================================================================================================================================================
        // CONSTRUCTOR
        //==================================================================================================================================================================================================================================================

        #region Constructor      

        private int version;
        private string data;
        private string[] bindata;
        private int mask;

        /// <summary>
        /// Decoding constructor, not functional
        /// </summary>
        /// <param name="im">QRCode image to be decoded</param>
        public QRCode(Image im)    //Decoding
        {
            this.Name = im.Name;
            this.Type = im.Type;
            this.Taille = im.Taille;
            this.Width = im.Width;
            this.Height = im.Height;
            this.HeaderOffset = im.HeaderOffset;
            this.Bpp = im.Bpp;
            this.Hresolution = im.Hresolution;
            this.Vresolution = im.Vresolution;


            this.Matrice = new Pixel[im.Matrice.GetLength(0), im.Matrice.GetLength(1)];
            for (int i = 0; i < im.Matrice.GetLength(0); i++)
            {
                for (int j = 0; j < im.Matrice.GetLength(1); j++)
                {
                    this.Matrice[i, j] = new Pixel(im.Matrice[i, j]);
                }
            }

            version = GetVersion();


        }
        /// <summary>
        /// Encoding constructor
        /// </summary>
        /// <param name="data">the sentence or word of data to be encoded into a QRCode</param>
        public QRCode(string data)   //Encoding
        {
            Utilities.GreenWriteLine("[+] Encoding your QRcode");

            this.data = data.ToUpper();

            this.version = GetSmallestVersion();
            this.Name = "QRCode";
            this.Type = "BM";

            this.Width = 17 + 4 * version;
            this.Height = this.Width;
            this.HeaderOffset = 54;

            this.Taille = this.Width * this.Height * 3 + this.HeaderOffset;
            this.Bpp = 24;

            //Console.WriteLine(this.toString());


            string binary = GetData();
            binary += GetCorrection(binary);


            this.bindata = Utilities.ToBinTab(binary);
            //Utilities.ShowArray(this.bindata);
            GetMatrice();

            AddQuietZone();

            this.Resize(8); //Multiply by 8 the 
            //this.SaveFile("QRCode_mask_"+this.mask, true);
        }



        /// <summary>
        /// Computes version with the width and height parameter of the given image (
        /// </summary>
        /// <returns></returns>
        public int GetVersion()
        {
            if (this.Width == this.Height && this.Width > 21 && (this.Width - 21) % 4 == 0)
            {
                return ((this.Width - 17) / 4);
            }
            return -1;
        }

        #endregion

        //==================================================================================================================================================================================================================================================
        // DATA ENCODING
        //==================================================================================================================================================================================================================================================

        #region Data Encoding

        /// <summary>
        /// Determines the smallest version into which the string of data provide by the user can fit
        /// </summary>
        /// <returns>Returns the version as an int between 1 and 40</returns>
        public int GetSmallestVersion()   //Gets the smallest version of qrcode possible for the data length
        {
            int count = 0;
            int version = -1;
            int[] capacityL = new int[] { 25, 47, 77, 114, 154, 195, 224, 279, 335, 395, 468, 535, 619, 667, 758, 854, 938, 1046, 1153, 1249, 1352, 1460, 1588, 1704, 1853, 1990, 2132, 2223, 2369, 2520, 2677, 2840, 3009, 3183, 3351, 3537, 3729, 3927, 4087, 4296 };
            while (count < capacityL.Length)
            {
                count++;
                if (this.data.Length <= capacityL[count - 1])
                {
                    Utilities.GreenWriteLine("{+] Your QRCode version is " + count);
                    version = count;
                    break;
                }
            }
            return version;

        }



        /// <summary>
        /// Construct the binary data from the Mode indicator, the character count indicator, the encoded data in alphanumeric and the padding
        /// </summary>
        /// <returns></returns>
        public string GetData()
        {
            string binary = "";
            string modeind = "0010";
            binary = "";
            binary += modeind; //Add the mode indicator which is always 0010 for alphanumeric encoding
            binary += GetCharCountInd(); //Add the character count indicator
            binary += EncodedData();   //Add the encoded data in alphanumeric
            binary = AddPadding(binary);  //Add the padding, which consists of a terminator, a %8 padding, and pad bytes to fill up the matrix
            return binary;
        }



        /// <summary>
        /// Return the total data codewords after considering the current version
        /// </summary>
        /// <returns>returns the number of datacodewords available for this version as an int</returns>
        public int GetTotalDataCodeWord()  //Returns the total data code work, since qr code's codeword need to be full
        {
            int[] TotalDataCodeWord = new int[] { 19, 34, 55, 80, 108, 136, 156, 194, 232, 274, 324, 370, 428, 461, 523, 589, 647, 721, 795, 861, 932, 1006, 1094, 1174, 1276, 1370, 1468, 1531, 1631, 1735, 1843, 1955, 2071, 2191, 2306, 2434, 2566, 2702, 2812, 2956 };
            return TotalDataCodeWord[this.version - 1];
        }



        /// <summary>
        /// Add the padding to a binary string, adds terminator, %8 bits, and pad bytes until total data codewords
        /// </summary>
        /// <param name="binary">binary string to add padding to</param>
        /// <returns>binary data with its padding appended to it</returns>
        public string AddPadding(string binary)
        {
            int length = GetTotalDataCodeWord() * 8; //*8 to tranforms codewords into bits

            //Adds terminator, up to 4 '0'
            int terminator;
            if (length - binary.Length <= 4)
            {
                terminator = length - binary.Length;
            }
            else
            {
                terminator = 4;
            }
            for (int i = 0; i < terminator; i++)
            {
                binary += "0";
            }

            //Adds bits until binary is %8
            while (binary.Length % 8 != 0)
            {
                binary += "0";
            }


            //Complete codewords with 11101100 00010001 codewords
            int padbytes = (length / 8) - (binary.Length / 8);
            while (padbytes != 0)
            {
                binary += padbytes % 2 != 0 ? "11101100" : "00010001";
                padbytes--;
            }
            return binary;

        }



        /// <summary>
        /// Returns the message's data encoded as strings of binary values, considering their alphanumeric values
        /// </summary>
        /// <returns>a long binary string</returns>
        public string EncodedData()
        {
            //Break up into pairs 
            int x = data.Length % 2 == 0 ? data.Length / 2 : data.Length / 2 + 1;
            string[] pairs = new string[x];
            int count = 0;
            if (data.Length % 2 == 0)
            {
                for (int i = 0; i < data.Length; i += 2)
                {
                    pairs[count] = Convert.ToString((data[i] + "" + data[i + 1]));
                    count++;
                }
            }
            else
            {
                for (int i = 0; i < data.Length - 1; i += 2)
                {
                    pairs[count] = Convert.ToString((data[i] + "" + data[i + 1]));
                    count++;
                }
                pairs[pairs.Length - 1] = Convert.ToString(data[data.Length - 1] + " ");
            }

            //Create a binary number for each pair
            for (int i = 0; i < pairs.Length; i++)
            {
                if (data.Length % 2 != 0 && i == pairs.Length - 1) //If we are encoding an odd number of characters, take the numeric representation of the final character and convert it into a 6-bit binary string
                {
                    pairs[i] = Convert.ToString(Utilities.ToBin(GetAlphanumericValue(data[data.Length - 1]), 6));
                }
                else
                {
                    int number = (GetAlphanumericValue(pairs[i][0]) * 45) + GetAlphanumericValue(pairs[i][1]);
                    pairs[i] = Convert.ToString(Utilities.ToBin(number, 11)); // convert that number into an 11-bit binary string
                }
            }

            return String.Join("", pairs);

        }



        /// <summary>
        /// Return the alphanumeric value of a character
        /// </summary>
        /// <param name="input">the character to be encoded into its alphanumeric value</param>
        /// <returns>the alphanumeric value in int of the character</returns>
        static int GetAlphanumericValue(char input)
        {
            switch (input)
            {
                case 'A':
                    return 10;
                case 'B':
                    return 11;
                case 'C':
                    return 12;
                case 'D':
                    return 13;
                case 'E':
                    return 14;
                case 'F':
                    return 15;
                case 'G':
                    return 16;
                case 'H':
                    return 17;
                case 'I':
                    return 18;
                case 'J':
                    return 19;
                case 'K':
                    return 20;
                case 'L':
                    return 21;
                case 'M':
                    return 22;
                case 'N':
                    return 23;
                case 'O':
                    return 24;
                case 'P':
                    return 25;
                case 'Q':
                    return 26;
                case 'R':
                    return 27;
                case 'S':
                    return 28;
                case 'T':
                    return 29;
                case 'U':
                    return 30;
                case 'V':
                    return 31;
                case 'W':
                    return 32;
                case 'X':
                    return 33;
                case 'Y':
                    return 34;
                case 'Z':
                    return 35;
                case ' ':
                    return 36;
                case '$':
                    return 37;
                case '%':
                    return 38;
                case '*':
                    return 39;
                case '+':
                    return 40;
                case '-':
                    return 41;
                case '.':
                    return 42;
                case '/':
                    return 43;
                case ':':
                    return 44;
                default:
                    return Convert.ToInt32(input);
            }
        }



        /// <summary>
        /// Returns in a binary string the Character Count Indicator, in length 9 11 or 13 depending on the version
        /// </summary>
        /// <returns>the character count indicator in string</returns>
        public string GetCharCountInd()
        {
            if (this.version <= 9)
            {
                return Utilities.ToBin(this.data.Length, 9);
            }
            else
            {
                if (this.version <= 26)
                {
                    return Utilities.ToBin(this.data.Length, 11);
                }
                else
                {
                    return Utilities.ToBin(this.data.Length, 13);
                }
            }
        }

        #endregion

        //==================================================================================================================================================================================================================================================
        // ERROR CORRECTION CODING
        //==================================================================================================================================================================================================================================================

        #region Error correction Coding

        /// <summary>
        /// Create the reed solomon correction of a string of binary data, in other words, 
        /// returns Error correction codewords in string from the string of binary of the data codewords
        /// </summary>
        /// <param name="binary">the binary data</param>
        /// <returns></returns>
        public string GetCorrection(string binary)
        {
            byte[] messagepolynomial = Utilities.ToDecTab(binary);
            //Program.ShowByteTab(messagepolynomial);
            int n = GetECCodewords();
            //Program.ShowByteTab(ReedSolomonAlgorithm.Encode(messagepolynomial, n, ErrorCorrectionCodeType.QRCode));

            return String.Join("", Utilities.ToBinTab(ReedSolomonAlgorithm.Encode(messagepolynomial, n, ErrorCorrectionCodeType.QRCode), 8));
        }

        #endregion

        //==================================================================================================================================================================================================================================================
        //STRUCTURE FINAL MESSAGE
        //==================================================================================================================================================================================================================================================

        #region Structure final message

        /// <summary>
        /// Calculates and returns the the total number of error correction codewords, knowing the version
        /// </summary>
        /// <returns>the number of ECC</returns>
        public int GetECCodewords()
        {
            int[] CodewordsPerBlock = new int[] { 7, 10, 15, 20, 26, 18, 20, 24, 30, 18, 20, 24, 26, 30, 22, 24, 28, 30, 28, 28, 28, 28, 30, 30, 26, 28, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
            int[] NumberOfBlocks_Grp1 = new int[] { 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 4, 2, 4, 3, 5, 5, 1, 5, 3, 3, 4, 2, 4, 6, 8, 10, 8, 3, 7, 5, 13, 17, 17, 13, 12, 6, 17, 4, 20, 19 };
            int[] NumberOfData_Grp1 = new int[] { 19, 34, 55, 80, 108, 68, 78, 97, 116, 68, 81, 92, 107, 115, 87, 98, 107, 120, 113, 107, 116, 111, 121, 117, 106, 114, 122, 117, 116, 115, 115, 115, 115, 115, 121, 121, 122, 122, 117, 118 };
            int[] NumberOfBlocks_Grp2 = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 2, 0, 1, 1, 1, 5, 1, 4, 5, 4, 7, 5, 4, 4, 2, 4, 10, 7, 10, 3, 0, 1, 6, 7, 14, 4, 18, 4, 6 };
            int[] NumberOfData_Grp2 = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 69, 0, 93, 0, 116, 88, 99, 108, 121, 114, 108, 117, 112, 122, 118, 107, 115, 123, 118, 117, 116, 116, 0, 116, 116, 122, 122, 123, 123, 118, 119 };

            int n = (NumberOfBlocks_Grp1[this.version - 1] + NumberOfBlocks_Grp2[this.version - 1]) * CodewordsPerBlock[this.version - 1];

            return n;
        }

        #endregion

        //==================================================================================================================================================================================================================================================
        // MODULE PLACEMENT IN MATRIC
        //==================================================================================================================================================================================================================================================

        #region Create the QR Code

        /// <summary>
        /// Creates the QRCode image's pixel matrix, adds all the pattern (finder, alignment, timing...) and the data
        /// </summary>
        public void GetMatrice()
        {
            this.Matrice = new Pixel[this.Height, this.Width];
            //Color in red the matrix: this was first used to debug the matrix, but later kept as a feature since a lot of functions
            //Now use the red pixel as a comparable pixel for free space
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    this.Matrice[i, j] = new Pixel(0, 0, 255);
                }
            }

            //Add the differents pattern and data:
            AddFinderPattern();
            AddAlignmentPattern();
            AddTimingPattern();
            AddDarkModule();
            AddUnusableLines();
            AddData();

            this.mask = GetBestMask(); //Calculate the best mask

            AddMask(this.Matrice, this.mask); //Add the best mask
            AddInfo(this.Matrice, this.mask); //Add the version/mask info around the finder patterns
        }

        /// <summary>
        /// Creates a white pixel at the desired i,j, of the QRCode matrix(!) 
        /// (Not to be used by temp pixel matrix... learned this the hard way)
        /// </summary>
        /// <param name="i">the line coordinate</param>
        /// <param name="j">the column coordinate</param>
        public void Whiten(int i, int j)
        {
            this.Matrice[i, j] = new Pixel(255, 255, 255);
        }



        /// <summary>
        /// Creates a black pixel at the desired i,j, of the QRCode matrix
        /// </summary>
        /// <param name="i">the line coordinate</param>
        /// <param name="j">the column coordinate</param>
        public void Blacken(int i, int j)
        {
            //Console.WriteLine("Trying to add a black pixel at " + i + "," + j + " in the matrix " + this.Matrice.GetLength(0) + "," + this.Matrice.GetLength(1));
            this.Matrice[i, j] = new Pixel(0, 0, 0);
        }



        /// <summary>
        /// Add the quiet zone to the matrix: a 4 wide white quiet around the QRCode
        /// </summary>
        public void AddQuietZone()
        {
            this.Width += 8;
            this.Height += 8;
            this.Taille = this.Width * this.Height * 3 + this.HeaderOffset;

            Pixel[,] newmatrice = new Pixel[this.Width, this.Width];
            for (int i = 0; i < this.Width; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    if (i < 4 || j < 4 || i >= this.Width - 4 || j >= this.Width - 4)
                    {
                        newmatrice[i, j] = new Pixel(255, 255, 255);
                    }
                    else
                    {
                        newmatrice[i, j] = this.Matrice[i - 4, j - 4];
                    }
                }
            }
            this.Matrice = newmatrice;
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region Add finder,alignement and timing patterns + dark module and reserved areas

        /// <summary>
        /// Add the finder patterns to the matrix, calls the CreateFinderPattern function to the center of the 3 corners
        /// </summary>
        public void AddFinderPattern()
        {
            CreateFinderPattern(3, 3);
            CreateFinderPattern(this.Width - 4, 3);
            CreateFinderPattern(3, this.Width - 4);
        }



        /// <summary>
        /// Creates a finder around with its center at the x,y coordinate
        /// </summary>
        /// <param name="x">line coordinate of center of the pattern</param>
        /// <param name="y">column coordinate of center of the pattern</param>
        public void CreateFinderPattern(int x, int y)
        {

            for (int i = -4; i <= 4; i++)
            {
                for (int j = -4; j <= 4; j++)
                {
                    //If the coordinate if in the matrix
                    if (((x + i) >= 0 && (x + i) < this.Matrice.GetLength(0) && (y + j) >= 0 && (y + j) < this.Matrice.GetLength(1)))
                    {
                        /* IS THIS A CLEANER WAY? NEED TESTING
                            if ((i % 2 == 0 || j >= i) || (j % 2 == 0 || i >= j))
                            {
                                Whiten(x + i, y + j);
                            }
                            else
                            {
                                Blacken(x + i, y + j);
                                //Blacken(8 - i, 8 - j);
                            }
                        */

                        //If the coordinate correspond to a white pixel
                        if ((i % 4 == 0 && i != 0) || (j % 4 == 0 && j != 0) || (j == 2 && i >= -2 && i <= 2) || (j == -2 && i >= -2 && i <= 2) || (i == 2 && j >= -2 && j <= 2) || (i == -2 && j >= -2 && j <= 2))
                        {
                            Whiten(x + i, y + j);
                        }
                        else
                        {
                            Blacken(x + i, y + j);
                        }
                    }

                }
            }
            //Blacken the center pixel
            Blacken(x, y);
        }


        /// <summary>
        /// Add all the alignment patterns to the pixel matrix
        /// </summary>
        public void AddAlignmentPattern()
        {
            if (version >= 2)
            {
                int[,] locations = new int[,] {
                {0,0,0,0,0,0,0 },
                {6,18,0,0,0,0,0},
                {6,22,0,0,0,0,0},
                {6,26,0,0,0,0,0},
                {6,30,0,0,0,0,0},
                {6,34,0,0,0,0,0},
                {6,22,38,0,0,0,0},
                {6,24,42,0,0,0,0},
                {6,26,46,0,0,0,0},
                {6,28,50,0,0,0,0},
                {6,30,54,0,0,0,0},
                {6,32,58,0,0,0,0},
                {6,34,62,0,0,0,0},
                {6,26,46,66,0,0,0},
                {6,26,48,70,0,0,0},
                {6,26,50,74,0,0,0},
                {6,30,54,78,0,0,0},
                {6,30,56,82,0,0,0},
                {6,30,58,56,0,0,0},
                {6,34,62,90,0,0,0},
                {6,28,50,72,94,0,0},
                {6,26,50,74,98,0,0},
                {6,30,54,78,102,0,0},
                {6,28,54,80,106,0,0},
                {6,32,58,84,110,0,0},
                {6,30,58,86,114,0,0},
                {6,34,62,90,118,0,0},
                {6,26,50,74,98,122,0},
                {6,30,54,78,102,126,0},
                {6,26,52,78,104,130,0},
                {6,30,56,82,108,134,0},
                {6,34,60,86,112,138,0},
                {6,30,58,86,114,142,0},
                {6,34,62,90,118,146,0},
                {6,30,54,78,102,126,150},
                {6,24,50,76,102,128,154},
                {6,28,54,80,106,132,158},
                {6,32,58,84,110,136,162},
                {6,26,54,82,110,138,166},
                {6,30,58,86,114,142,170},};

                //We remove the zeros from the possibilities of location
                int count = 0;
                for (int i = 0; i < locations.GetLength(1); i++) //how many 0?
                {
                    if (locations[version - 1, i] == 0) { count++; }
                }

                int[] location = new int[7 - count];  //Cut down 0, example: location for version 1: {}, for version 2:{6,18}
                for (int i = 0; i < location.Length; i++)
                {
                    location[i] = locations[version - 1, i];
                }

                //For each intersection of the location, without the 3 corners where finder patterns are present
                for (int i = 0; i < location.Length; i++)
                {
                    for (int j = 0; j < location.Length; j++)
                    {
                        //Finder pattern already here?
                        if ((location[i] == 6 && (location[j] == 6 || j == location.Length - 1)) || ((i == location.Length - 1) && (location[j] == 6)))
                        {
                            continue;
                        }
                        //Adds an alignment pattern at  location[i],location[j]
                        CreateAlignmentPattern(location[i], location[j]);
                    }
                }
            }

        }



        /// <summary>
        /// Creates an alignment pattern at the desired coordinates
        /// </summary>
        /// <param name="x">the line coordinate of the center of the alignment pattern</param>
        /// <param name="y">the column coordinate of the center of the alignment pattern</param>
        public void CreateAlignmentPattern(int x, int y)
        {
            //Range of the pattern
            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    //This should not be useful, because alignment pattern never touch borders, but we never know
                    if (((x + i) >= 0 && (x + i) < this.Matrice.GetLength(0) && (y + j) >= 0 && (y + j) < this.Matrice.GetLength(1)))
                    {
                        if ((i % 2 == 0 && i != 0) || (j % 2 == 0 && j != 0))
                        {
                            Blacken(x + i, y + j);
                        }
                        else
                        {
                            Whiten(x + i, y + j);
                        }
                    }

                }
            }
            //Add a black pixel to the middle
            Blacken(x, y);
        }



        /// <summary>
        /// Adds the timing pattern's line and column 
        /// </summary>
        public void AddTimingPattern()
        {
            Pixel redpixel = new Pixel(0, 0, 255);
            for (int i = 8; i < Height - 8; i++)
            {
                //Redpixel is to be understood as a non-assigned pixel, to avoid rewriting the finder patterns
                if (i % 2 == 0 && Pixel.IsEqual(this.Matrice[i, 6], redpixel) && Pixel.IsEqual(this.Matrice[6, i], redpixel))
                {
                    Blacken(i, 6);
                    Blacken(6, i);
                }
                if (i % 2 != 0 && Pixel.IsEqual(this.Matrice[i, 6], redpixel) && Pixel.IsEqual(this.Matrice[6, i], redpixel))
                {
                    Whiten(i, 6);
                    Whiten(6, i);
                }
            }
        }



        /// <summary>
        /// Adds the dark module pixel
        /// </summary>
        public void AddDarkModule()
        {
            this.Matrice[(4 * this.version) + 9, 8] = new Pixel(0, 0, 0);
        }



        /// <summary>
        /// Color in blue the unusable lines around the finder pattern on the pixel matrix, so the data doesn't
        /// write itself on the version/mask info reserved space
        /// </summary>
        public void AddUnusableLines()
        {
            if (version < 7)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (Pixel.IsEqual(this.Matrice[8, i], new Pixel(0, 0, 255)))
                    {
                        this.Matrice[8, i] = new Pixel(255, 0, 0);
                    }
                    if (Pixel.IsEqual(this.Matrice[i, 8], new Pixel(0, 0, 255)))
                    {
                        this.Matrice[i, 8] = new Pixel(255, 0, 0);
                    }
                    if (Pixel.IsEqual(this.Matrice[8, this.Matrice.GetLength(1) - 8 + i], new Pixel(0, 0, 255)))
                    {
                        this.Matrice[8, this.Matrice.GetLength(1) - 8 + i] = new Pixel(255, 0, 0);
                    }
                    if (Pixel.IsEqual(this.Matrice[this.Matrice.GetLength(1) - 8 + i, 8], new Pixel(0, 0, 255)))
                    {
                        this.Matrice[this.Matrice.GetLength(1) - 8 + i, 8] = new Pixel(255, 0, 0);
                    }
                }
                this.Matrice[8, 8] = new Pixel(255, 0, 0);
            }
            else //Above the version 6, the mask/version info are stored in rectangle at the top right and bottom left corner next
            //Next to the finder patterns
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (Pixel.IsEqual(this.Matrice[Matrice.GetLength(0) - 11 + i, j], new Pixel(0, 0, 255)))
                        {
                            this.Matrice[Matrice.GetLength(0) - 11 + i, j] = new Pixel(255, 0, 0);
                        }
                        if (Pixel.IsEqual(this.Matrice[j, Matrice.GetLength(1) - 11 + i], new Pixel(0, 0, 255)))
                        {
                            this.Matrice[j, Matrice.GetLength(1) - 11 + i] = new Pixel(255, 0, 0);
                        }
                    }
                }
            }
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region Placement the Data Bits

        /// <summary>
        /// Add the data to the pixel matrix
        /// </summary>
        public void AddData()
        {
            string binary = String.Join("", this.bindata);
            Pixel redpixel = new Pixel(0, 0, 255);
            int timingcolpassed = 0;
            //For each pixel/binary number in data
            int count = 0;

            for (int j = this.Matrice.GetLength(1) - 1; j >= 1; j -= 2)
            {
                if ((j / 2 + timingcolpassed) % 2 == 0)  //Going upwards
                {
                    for (int i = this.Matrice.GetLength(0) - 1; i >= 0; i--)
                    {
                        if (Pixel.IsEqual(this.Matrice[i, j], redpixel) && count<binary.Length)
                        {
                            if (binary[count] == '0')
                            {
                                Whiten(i, j);
                            }
                            else
                            {
                                Blacken(i, j);
                            }
                            count++;
                        }

                        if (Pixel.IsEqual(this.Matrice[i, j - 1], redpixel) && count < binary.Length)
                        {

                            if (binary[count] == '0')
                            {
                                Whiten(i, j - 1);
                            }
                            else
                            {
                                Blacken(i, j - 1);
                            }
                            count++;
                        }



                    }
                }
                else   //Going downwards
                {
                    for (int i = 0; i < this.Matrice.GetLength(0); i++)
                    {
                        if (Pixel.IsEqual(this.Matrice[i, j], redpixel) && count < binary.Length)
                        {

                            if (binary[count] == '0')
                            {
                                Whiten(i, j);
                            }
                            else
                            {
                                Blacken(i, j);
                            }
                            count++;
                        }

                        if (Pixel.IsEqual(this.Matrice[i, j - 1], redpixel) && count < binary.Length)
                        {

                            if (binary[count] == '0')
                            {
                                Whiten(i, j - 1);
                            }
                            else
                            {
                                Blacken(i, j - 1);
                            }
                            count++;
                        }


                    }
                }
                if (j == 8)
                {
                    j--;
                    timingcolpassed++;
                }
            } 
                

            //If any pixel was not colored, add the white final padding (this is just to avoid having another 40 index table to copy by hand)
            for (int i = 0; i < this.Matrice.GetLength(0); i++)
            {
                for (int j = 0; j < this.Matrice.GetLength(1); j++)
                {
                    if (Pixel.IsEqual(this.Matrice[i, j], redpixel)) { Whiten(i, j); }
                }
            }
            
            
        }



        
        #endregion

        //==================================================================================================================================================================================================================================================
        // DATA MASKING
        //==================================================================================================================================================================================================================================================

        #region Determining the Best Mask

        /// <summary>
        /// Function that returns the best mask based on penalty calculations
        /// </summary>
        /// <returns>int of best mask, between 0 and 7</returns>
        public int GetBestMask()
        {
            int[] maskscore = new int[8];
            Pixel[,] temp = Utilities.Copymat(this.Matrice); //Deep copy to avoid touching our own matrix
            for (int i = 0; i < maskscore.Length; i++)
            {

                AddInfo(temp, i); //Add the info with the temporary mask
                AddMask(temp, i); //Add the temporary mask
                int penalty = 0; //Reset the penalty
                penalty += GetPenalty1(temp); //Add the 4 penalties
                //Console.WriteLine("Got penalty 1,  " + GetPenalty1(temp));
                penalty += GetPenalty2(temp);
                //Console.WriteLine("Got penalty 2,  " + GetPenalty2(temp));
                penalty += GetPenalty3(temp);
                //Console.WriteLine("Got penalty 3,  " + GetPenalty3(temp));
                penalty += GetPenalty4(temp);
                //Console.WriteLine("Got penalty 4,  " + GetPenalty4(temp));

                //Console.WriteLine("Total penalty for mask " + i + " is " + penalty + "\n\n");

                maskscore[i] = penalty; //Add penalty to the maskscore tab
                //Console.WriteLine( "\n\n\n"+this.MatricetoString());
                temp = Utilities.Copymat(this.Matrice); //Deepcopy
            }
            Utilities.GreenWriteLine("{+] The best mask for your QRCode is " + Array.IndexOf(maskscore, maskscore.Min()));
            return Array.IndexOf(maskscore, maskscore.Min()); //return the index of the minimum of the mask scores, so we get the best mask
        }
        #region penalty
        /// <summary>
        /// Computes the 1rst penalty
        /// </summary>
        /// <param name="mat">the matrix to calculate the penalty from</param>
        /// <returns>the value of the 1th penalty</returns>
        public int GetPenalty1(Pixel[,] mat)
        {
            int penalty = 0;
            int count = 1;
            Pixel lastpixel = new Pixel();
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                count = 1;
                lastpixel = mat[i, 0];
                for (int j = 1; j < mat.GetLength(1); j++)
                {
                    if (Pixel.IsEqual(mat[i, j], lastpixel)) //Last pixel is the same
                    {
                        count++;
                        //Console.WriteLine("i,j=" + i + "," + j + " pixel is the same as last pixel, count incremented: "+count+" pixels in a row!");

                    }
                    else
                    {
                        if (count >= 5)  //Count was bigger or equal than 5
                        {
                            penalty += count - 2;
                            count = 1;
                            //Console.WriteLine("i,j=" + i + "," + j + " pixel isnt the same as last pixel, penalty is now "+penalty);

                        }
                        count = 1;
                        //Console.WriteLine("i,j=" + i + "," + j + " pixel isnt the same as last pixel, count set to 1");

                    }
                    //Console.WriteLine(i + "th line, penalty is " + penalty);
                    //Console.ReadLine();
                    lastpixel = mat[i, j].Copy();
                }
                if (count >= 5)  //Count was bigger or equal than 5
                {
                    penalty += count - 2;
                    count = 1;
                    //Console.WriteLine("End of the line: penalty is now " + penalty);
                }
            }

            //For columns
            for (int j = 0; j < mat.GetLength(1); j++)
            {

                count = 1;
                lastpixel = mat[0, j];

                for (int i = 1; i < mat.GetLength(1); i++)
                {
                    if (Pixel.IsEqual(mat[i, j], lastpixel)) //Last pixel is the same
                    {
                        count++;
                    }
                    else
                    {
                        if (count >= 5)  //Count was bigger or equal than 5
                        {
                            penalty += count - 2;
                            count = 1;
                        }
                        count = 1;
                    }
                    lastpixel = mat[i, j].Copy();
                }
                if (count >= 5)  //Count was bigger or equal than 5
                {
                    penalty += count - 2;
                }

            }

            return penalty;
        }



        /// <summary>
        /// Computes the 2nd penalty
        /// </summary>
        /// <param name="mat">the matrix to calculate the penalty from</param>
        /// <returns>the value of the 2nd penalty</returns>
        public int GetPenalty2(Pixel[,] mat)
        {
            int penalty = 0;
            for (int i = 0; i < mat.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < mat.GetLength(1) - 1; j++)
                {
                    if (Pixel.IsEqual(mat[i, j], mat[i + 1, j]) && Pixel.IsEqual(mat[i, j], mat[i + 1, j + 1]) && Pixel.IsEqual(mat[i, j], mat[i, j + 1]))
                    {
                        penalty += 3;
                    }
                }
            }
            return penalty;
        }



        /// <summary>
        /// Computes the 3rd penalty
        /// </summary>
        /// <param name="mat">the matrix to calculate the penalty from</param>
        /// <returns>the value of the 3rd penalty</returns>
        public int GetPenalty3(Pixel[,] mat)
        {
            int penalty = 0;
            Pixel blackpixel = new Pixel(0, 0, 0);
            Pixel whitepixel = new Pixel(255, 255, 255);
            List<Pixel> penaltypattern1 = new List<Pixel> { blackpixel, whitepixel, blackpixel, blackpixel, blackpixel, whitepixel, blackpixel, whitepixel, whitepixel, whitepixel, whitepixel, };
            List<Pixel> penaltypattern2 = new List<Pixel> { whitepixel, whitepixel, whitepixel, whitepixel, blackpixel, whitepixel, blackpixel, blackpixel, blackpixel, whitepixel, blackpixel };

            //Checking for lines
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1) - 10; j++)
                {
                    List<Pixel> temp = new List<Pixel>();
                    for (int k = 0; k < 11; k++)
                    {
                        temp.Add(mat[i, j + k]);
                    }
                    if (Utilities.IsPixelListEqual(temp, penaltypattern1) || Utilities.IsPixelListEqual(temp, penaltypattern2))
                    {
                        //Console.WriteLine("Found a line pattern at " + i + "," + j);
                        penalty += 40;
                        //Console.WriteLine("penalty is " + penalty);
                    }
                }
            }
            //CHecking for columns
            for (int i = 0; i < mat.GetLength(0) - 10; i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    List<Pixel> temp = new List<Pixel>();
                    for (int k = 0; k < 11; k++)
                    {
                        temp.Add(mat[i + k, j]);
                    }
                    if (Utilities.IsPixelListEqual(temp, penaltypattern1) || Utilities.IsPixelListEqual(temp, penaltypattern2))
                    {
                        //Console.WriteLine("Found a col pattern at " + i + "," + j);
                        penalty += 40;
                        //Console.WriteLine("penalty is " + penalty);
                    }
                }
            }
            return penalty;
        }



        /// <summary>
        /// Computes the 4th penalty
        /// </summary>
        /// <param name="mat">the matrix to calculate the penalty from</param>
        /// <returns>the value of the 4th penalty</returns>
        public int GetPenalty4(Pixel[,] mat)
        {
            int score = 0;
            int total_moduls = 0;
            int dark_moduls = 0; //Count how many dark modules there are in the matrix
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    total_moduls++;//Count the total number of modules in the matrix
                    if (Pixel.IsEqual(mat[i, j], new Pixel(0, 0, 0))) { dark_moduls++; } //Count how many dark modules there are in the matrix
                }
            }

            dark_moduls = dark_moduls * 100; //mutliply by 100 to do the percent
            int percent = dark_moduls / total_moduls; //Calculate the percent of modules in the matrix that are dark: darkmodules / totalmodules

            //Determine the previous and next multiple of five of this percent
            int rest = percent % 5;
            int x = 0;
            while (rest % 5 != 0)
            {
                x++;
                rest++;
            }
            int next_multiple = percent + x;
            int previous_multiple = percent - (5 - x);

            //Subtract 50 from each of these multiples of five and take the absolute value of the result and divide each of these by five
            next_multiple = Math.Abs(next_multiple - 50) / 5;
            previous_multiple = Math.Abs(previous_multiple - 50) / 5;

            //Finally, take the smallest of the two numbers and multiply it by 10
            score = next_multiple > previous_multiple ? previous_multiple * 10 : next_multiple * 10;
            return score;
        }
        #endregion

        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region Add Mask

        /// <summary>
        /// Returns the mask pattern magic number applied to the pixel at x,y, we only need to switch pixels with magicnumber=0
        /// </summary>
        /// <param name="pattern">the pattern int</param>
        /// <param name="x">x coordinate of the pixel</param>
        /// <param name="y">x coordinate of the pixel</param>
        /// <returns>The mask pattern magic number</returns>
        public int MaskPattern(int pattern, int x, int y)
        {
            switch (pattern)
            {
                case 0:
                    return (x + y) % 2;
                case 1:
                    return x % 2;
                case 2:
                    return y % 3;
                case 3:
                    return (x + y) % 3;
                case 4:
                    return (this.Matrice.GetLength(1) * (x / 2) + this.Matrice.GetLength(1) * (y / 3)) % 2;
                case 5:
                    return ((x * y) % 2 + (x * y) % 3);
                case 6:
                    return ((x * y) % 2 + (x * y) % 3) % 2;
                case 7:
                    return ((x + y) % 2 + (x * y) % 3) % 2;
                default:
                    Utilities.RedWriteLine("Error, no correct mask found");
                    return 1;
            }
        }



        /// <summary>
        /// Add a mask to a pixel matrix
        /// </summary>
        /// <param name="matrice">the pixel matrix without the mask</param>
        /// <param name="mask">the int of the mask to be added</param>
        /// <returns>the pixel matrix with the mask</returns>
        public Pixel[,] AddMask(Pixel[,] matrice, int mask)
        {
            List<int[]> alignpatterncoo = GetAlignmentPatternCoo(); //We create a list of all the coordinates of the align. patterns pixels to avoid them

            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                for (int j = 0; j < matrice.GetLength(1); j++)
                {
                    if (MaskPattern(mask, i, j) == 0 && coordinatevalid(i, j, alignpatterncoo) == true) //If the coordinates is valid the pixel's magic number is 0, we need to switch is
                    {
                        if (Pixel.IsEqual(matrice[i, j], new Pixel(255, 255, 255)))
                        {
                            matrice[i, j] = new Pixel(0, 0, 0);
                        }
                        else
                        {
                            matrice[i, j] = new Pixel(255, 255, 255);
                        }
                    }
                }
            }
            return matrice;
        }



        /// <summary>
        /// Boolean functions that checks if a coordinate is valid for masking, to avoid masking any patterns
        /// </summary>
        /// <param name="x">the x coordinate of the pixel</param>
        /// <param name="y">the x coordinate of the pixel</param>
        /// <param name="alignpattern">a list of coordinates in format int tab[x,y] which belong to alignment patterns</param>
        /// <returns>returns if the coordinate is valid or not (false if it hits any pattern)</returns>
        public bool coordinatevalid(int x, int y, List<int[]> alignpattern)
        {
            //Console.WriteLine("Trying to see if x,y:" + x + "," + y + " are valid for masking");
            bool valid = true;
            if (x == 6) { return false; } //timing pattern?
            if (y == 6) { return false; }
            if ((x >= 0 && x <= 8) && (y >= 0 && y <= 8)) { return false; } //finder pattern?
            if ((x >= Matrice.GetLength(0) - 8 && x < Matrice.GetLength(0)) && (y >= 0 && y <= 8)) { return false; }
            if ((x >= 0 && x <= 8) && (y >= Matrice.GetLength(1) - 8 && y < Matrice.GetLength(1))) { return false; }
            if (x == (4 * this.version + 9) && y == 8) { return false; } //Dark module?
            int[] tab = new int[] { x, y };
            //See if the coordinate is inside the alignpattern coordinates
            for (int i = 0; i < alignpattern.Count; i++)
            {
                if (Utilities.IsTabSame(tab, alignpattern[i])) { return false; }
            }
            return valid;
        }



        /// <summary>
        /// Construct and returns a list of coordinates in format: int tab[x,y] which belong to alignment patterns
        /// </summary>
        /// <returns>a list of coordinates</returns>
        public List<int[]> GetAlignmentPatternCoo()
        {
            List<int[]> coordinates = new List<int[]>();
            if (version >= 2)
            {
                int[,] locations = new int[,] {
                {0,0,0,0,0,0,0 },
                {6,18,0,0,0,0,0},
                {6,22,0,0,0,0,0},
                {6,26,0,0,0,0,0},
                {6,30,0,0,0,0,0},
                {6,34,0,0,0,0,0},
                {6,22,38,0,0,0,0},
                {6,24,42,0,0,0,0},
                {6,26,46,0,0,0,0},
                {6,28,50,0,0,0,0},
                {6,30,54,0,0,0,0},
                {6,32,58,0,0,0,0},
                {6,34,62,0,0,0,0},
                {6,26,46,66,0,0,0},
                {6,26,48,70,0,0,0},
                {6,26,50,74,0,0,0},
                {6,30,54,78,0,0,0},
                {6,30,56,82,0,0,0},
                {6,30,58,56,0,0,0},
                {6,34,62,90,0,0,0},
                {6,28,50,72,94,0,0},
                {6,26,50,74,98,0,0},
                {6,30,54,78,102,0,0},
                {6,28,54,80,106,0,0},
                {6,32,58,84,110,0,0},
                {6,30,58,86,114,0,0},
                {6,34,62,90,118,0,0},
                {6,26,50,74,98,122,0},
                {6,30,54,78,102,126,0},
                {6,26,52,78,104,130,0},
                {6,30,56,82,108,134,0},
                {6,34,60,86,112,138,0},
                {6,30,58,86,114,142,0},
                {6,34,62,90,118,146,0},
                {6,30,54,78,102,126,150},
                {6,24,50,76,102,128,154},
                {6,28,54,80,106,132,158},
                {6,32,58,84,110,136,162},
                {6,26,54,82,110,138,166},
                {6,30,58,86,114,142,170},};

                //We remove the zeros from the possibilities of location
                int count = 0;
                for (int i = 0; i < locations.GetLength(1); i++) //how many 0?
                {
                    if (locations[version - 1, i] == 0) { count++; }
                }

                int[] location = new int[7 - count];  //Cut down 0, example: location for version 1: {}, for version 2:{6,18}
                for (int i = 0; i < location.Length; i++)
                {
                    location[i] = locations[version - 1, i];
                }

                //For each intersection of the location, without the 3 corners where finder patterns are present
                for (int i = 0; i < location.Length; i++)
                {
                    for (int j = 0; j < location.Length; j++)
                    {
                        if ((location[i] == 6 && (location[j] == 6 || j == location.Length - 1)) || ((i == location.Length - 1) && (location[j] == 6)))
                        {
                            continue;
                        }
                        for (int k = -2; k <= 2; k++)
                        {
                            for (int l = -2; l <= 2; l++)
                            {
                                coordinates.Add(new int[] { location[i] + k, location[j] + l });
                            }
                        }
                    }
                }
            }
            return coordinates;

        }

        #endregion

        //==================================================================================================================================================================================================================================================
        // FORMAT AND VERSION INFORMATION
        //==================================================================================================================================================================================================================================================

        #region Get Format and Version Information

        /// <summary>
        /// Returns the format info (in the case version if strictly lower than 7)
        /// </summary>
        /// <param name="MaskPattern">the mask pattern (0 to 7)</param>
        /// <returns>the binary to be added to the info bits</returns>
        public string GetFormatInfo(int MaskPattern)
        {
            switch (MaskPattern)
            {
                case 0:
                    return "111011111000100";
                case 1:
                    return "111001011110011";
                case 2:
                    return "111110110101010";
                case 3:
                    return "111100010011101";
                case 4:
                    return "110011000101111";
                case 5:
                    return "110001100011000";
                case 6:
                    return "110110001000001";
                case 7:
                    return "110100101110110";
                default:
                    return "Why are you even feeding me this number";
            }
        }



        /// <summary>
        /// Returns the format info (in the case version if strictly higher than 6)
        /// </summary>
        /// <param name="version">the version of the QRCode</param>
        /// <returns>the binary to be added to the info bits</returns>
        public string GetVersionInfo(int version)
        {
            switch (version)//Why not use this.version..?
            {
                case 7:
                    return "000111110010010100";
                case 8:
                    return "001000010110111100";
                case 9:
                    return "001001101010011001";
                case 10:
                    return "001010010011010011";
                case 11:
                    return "001011101111110110";
                case 12:
                    return "001100011101100010";
                case 13:
                    return "001101100001000111";
                case 14:
                    return "001110011000001101";
                case 15:
                    return "001111100100101000";
                case 16:
                    return "010000101101111000";
                case 17:
                    return "010001010001011101";
                case 18:
                    return "010010101000010111";
                case 19:
                    return "010011010100110010";
                case 20:
                    return "010100100110100110";
                case 21:
                    return "010101011010000011";
                case 22:
                    return "010110100011001001";
                case 23:
                    return "010111011111101100";
                case 24:
                    return "011000111011000100";
                case 25:
                    return "011001000111100001";
                case 26:
                    return "011010111110101011";
                case 27:
                    return "011011000010001110";
                case 28:
                    return "011100110000011010";
                case 29:
                    return "011101001100111111";
                case 30:
                    return "011110110101110101";
                case 31:
                    return "011111001001010000";
                case 32:
                    return "100000100111010101";
                case 33:
                    return "100001011011110000";
                case 34:
                    return "100010100010111010";
                case 35:
                    return "100011011110011111";
                case 36:
                    return "100100101100001011";
                case 37:
                    return "100101010000101110";
                case 38:
                    return "100110101001100100";
                case 39:
                    return "100111010101000001";
                case 40:
                    return "101000110001101001";
                default:
                    return "Why are you even feeding me this number";
            }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region Add Format and Version Information

        /// <summary>
        /// Add the info bits around or next to the finder pattern in a pixel matrix (depending on the version)
        /// </summary>
        /// <param name="mat">the pixel matrix which needs info to be added</param>
        /// <param name="mask">the mask that will be applied after</param>
        public void AddInfo(Pixel[,] mat, int mask)
        {
            if (version <= 6)
            {
                string info = GetFormatInfo(mask);

                int count = 0;
                for (int i = 0; i < 7; i++)
                {
                    mat[mat.GetLength(0) - 1 - i, 8] = info[count] == '1' ? new Pixel(0, 0, 0) : new Pixel(255, 255, 255);
                    count++;
                }
                for (int i = 0; i < 8; i++)
                {
                    mat[8, mat.GetLength(1) - 8 + i] = info[count] == '1' ? new Pixel(0, 0, 0) : new Pixel(255, 255, 255);
                    count++;
                }

                count = 0;
                for (int i = 0; i < 6; i++)
                {
                    mat[8, i] = info[count] == '1' ? new Pixel(0, 0, 0) : new Pixel(255, 255, 255);
                    mat[i, 8] = info[info.Length - count - 1] == '1' ? new Pixel(0, 0, 0) : new Pixel(255, 255, 255);
                    count++;
                }
                mat[8, 7] = info[6] == '1' ? new Pixel(0, 0, 0) : new Pixel(255, 255, 255);
                mat[8, 8] = info[7] == '1' ? new Pixel(0, 0, 0) : new Pixel(255, 255, 255);
                mat[7, 8] = info[8] == '1' ? new Pixel(0, 0, 0) : new Pixel(255, 255, 255);
            }
            else
            {
                string info = GetVersionInfo(this.version);
                int count = 0;
                int x = 0;
                int y = mat.GetLength(1) - 11;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        mat[x + j, y + i] = info[count] == '1' ? new Pixel(0, 0, 0) : new Pixel(255, 255, 255); ;
                        count++;
                    }
                }
                count = 0;
                x = mat.GetLength(1) - 11;
                y = 0;
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        mat[x + j, y + i] = info[count] == '1' ? new Pixel(0, 0, 0) : new Pixel(255, 255, 255);
                        count++;
                    }
                }
            }

        }

        #endregion
    }
}
