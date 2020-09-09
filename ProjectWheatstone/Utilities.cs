using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWheatstone
{   /// <summary>
    /// This class regroups many utilities used in input sanitization, tostrings for debug, converting
    /// </summary>
    public class Utilities
    {
        //==================================================================================================================================================================================================================================================
        // QRCODE UTILITIES FUNCTIONS
        //==================================================================================================================================================================================================================================================

        #region QRCode utilies functions

        /// <summary>
        /// Compares 2 Pixel lists and check if they are equal
        /// </summary>
        /// <param name="list1">first list</param>
        /// <param name="list2">second list</param>
        /// <returns>boolean, true if tabs are equal</returns>
        static public bool IsPixelListEqual(List<Pixel> list1, List<Pixel> list2) //utilities
        {
            bool isequal = true;
            if (list1.Count == list2.Count)
            {
                for (int i = 0; i < list1.Count; i++)
                {
                    isequal = (isequal && Pixel.IsEqual(list1[i], list2[i]));
                }
            }
            else
            {
                isequal = false;
            }
            return isequal;
        }

        /// <summary>
        /// Deep copy a pixel matrix
        /// </summary>
        /// <param name="mat">the original pixel matrix</param>
        /// <returns>the new copied pixel matrix</returns>
        public static Pixel[,] Copymat(Pixel[,] mat)
        {
            Pixel[,] ret = new Pixel[mat.GetLength(0), mat.GetLength(1)];
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {

                    ret[i, j] = new Pixel(mat[i, j]);
                }
            }
            return ret;
        }



        /// <summary>
        /// Checks if 2 tabs of int are the same
        /// </summary>
        /// <param name="tab1">1rst tab to compare</param>
        /// <param name="tab2">2nd tab to compare</param>
        /// <returns></returns>
        static public bool IsTabSame(int[] tab1, int[] tab2)
        {
            bool same = true;
            for (int i = 0; i < tab1.Length; i++)
            {
                if (tab1[i] != tab2[i]) { same = false; }
            }
            return same;
        }

        #endregion

        //==================================================================================================================================================================================================================================================
        // IMAGE UTILITIES FUNCTIONS
        //==================================================================================================================================================================================================================================================

        #region Image utilities functions

        /// <summary>
        /// Add an int number to a byte array, by first converting it into a little Endian byte array, at a specific offset
        /// </summary>
        /// <param name="array">The byte array to add to</param>
        /// <param name="number">the int number to be added</param>
        /// <param name="offset">the offset at which the number should be place</param>
        /// <param name="length">the length of the byte array tha tthe number should be converted to</param>
        public static void ArrayAddInt(byte[] array, int number, int offset, int length)
        {
            byte[] numberasendian = EndianFromInt(number, length);
            for (int i = 0; i < length; i++)
            {
                array[i + offset] = numberasendian[i];
            }
        }



        /// <summary>
        /// Add a array of bytes to another array of bytes, at a specific offset
        /// </summary>
        /// <param name="array">The byte array to add to</param>
        /// <param name="arraytoadd">The byte array to be added</param>
        /// <param name="offset">the offset at which the new byte array should be placed in the original array</param>
        public static void ArrayAddBytes(byte[] array, byte[] arraytoadd, int offset)
        {
            for (int i = 0; i < arraytoadd.Length; i++)
            {
                array[i + offset] = arraytoadd[i];
            }
        }



        /// <summary>
        /// Add pixel matrix to a byte array, by converting each pixel into a byte array first
        /// </summary>
        /// <param name="array">the original array</param>
        /// <param name="mat">the pixel matrix to be added to the array</param>
        /// <param name="offset">the offeset it should be added to</param>
        /// <param name="padding">the padding value of the current picture</param>
        public static void ArrayAddPixels(byte[] array, Pixel[,] mat, int offset, int padding)
        {
            int count = 0;
            for (int i = mat.GetLength(0) - 1; i >= 0; i--) //for each line (descending line to reorder properly)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    ArrayAddBytes(array, mat[i, j].toByteArray(), (offset + count)); //This could be prettier, change count with i*?+j*?

                    count += 3;


                    //Add Padding
                }
                if (padding != 0)
                {
                    for (int k = 0; k < padding; k++)
                    {
                        //Console.WriteLine("Added a paddin byte");
                        ArrayAddBytes(array, new byte[] { 0 }, (offset + count));
                        count += 1;
                    }
                }


            }
        }



        /// <summary>
        /// Gives back an array of bytes from the start to end index in the bytes array given
        /// </summary>
        /// <param name="bytes">the original byte array</param>
        /// <param name="start">start index</param>
        /// <param name="end">end index</param>
        /// <returns>a cropped byte array between start and end</returns>
        public static byte[] getArray(byte[] bytes, int start, int end)
        {
            byte[] b = new byte[(end - start + 1)];
            for (int i = start; i <= end; i++)
            {
                b[i - start] = bytes[i];
                //Console.WriteLine("i=" + i + " and bytes[i]=" + bytes[i]);
            }
            return b;
        }

        #endregion

        //==================================================================================================================================================================================================================================================
        // WRITE WITH COLORS
        //==================================================================================================================================================================================================================================================

        #region Write with colors

        /// <summary>
        /// Prints the data in dark cyan for a nice looking console
        /// </summary>
        /// <param name="data">the data to be printed</param>
        public static void GreenWriteLine(string data)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(data);
            Console.ResetColor();
        }



        /// <summary>
        /// Prints the data in red for erros
        /// </summary>
        /// <param name="data">the data to be printed</param>
        public static void RedWriteLine(string data)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(data);
            Console.ResetColor();
        }

        #endregion

        //==================================================================================================================================================================================================================================================
        // CONVERT INDIAN/INT
        //==================================================================================================================================================================================================================================================

        #region Convert Indian/Int

        /// <summary>
        /// Convert an endian byte sequence to int
        /// </summary>
        /// <param name="array"> Endian byte sequence to be converted</param>
        /// <returns> Int converted from endian </returns>
        public static int IntFromEndian(byte[] array)
        {
            int result = 0;
            //Console.WriteLine("Array length is " + array.Length);
            for (int i = array.Length - 1; i >= 0; i--)
            {
                //Console.WriteLine("result= "+result);
                result |= array[i] << i * 8;  //add the byte and shifts left i*8 bits (like multiply)   0001<<1=0010   /     1010|0100=1110
            }
            return result;
        }



        /// <summary>
        /// Convert an int to an endian byte sequence
        /// </summary>
        /// <param name="number">the int to be converted</param>
        /// <param name="arraylength">the desired length of the byte array</param>
        /// <returns> Endian converted from int </returns>
        static public byte[] EndianFromInt(int number, int arraylength)
        {
            byte[] result = new byte[arraylength];
            for (int i = 0; i < arraylength; i++)
            {
                result[i] = (byte)(((uint)number >> i * 8) & 0xFF);   //Shift right i*8 bits and add a 0 if necessary
            }
            return result;
        }

        #endregion

        //==================================================================================================================================================================================================================================================
        // CONVERT DECIMAL/BINARY
        //==================================================================================================================================================================================================================================================

        #region  Convert Decimal/Binary

        /// <summary>
        /// Recursive function to convert to binary but adds a 0 if needed to complete the 8 digit binary number
        /// </summary>
        /// <param name="value">the int value to be converted</param>
        /// <param name="len">the length of the binary for 0 padding</param>
        /// <returns>the binary as a string</returns>
        public static string ToBin(int value, int len)
        {
            string temp = "";
            if (len > 1)
            {
                temp = ToBin(value >> 1, len - 1);
            }
            else
            {
                temp = "";
            }
            temp += "01"[value & 1];
            return temp;
        }



        /// <summary>
        /// Converts a string of binary to a byte
        /// </summary>
        /// <param name="bin">the binary number as a string</param>
        /// <returns>the byte value of the binary</returns>
        public static byte ToDec(string bin)
        {
            byte decimalnumber = (byte)Convert.ToInt32(bin, 2);
            return decimalnumber;
        }



        /// <summary>
        /// convert binary data, after breaking it into 8 bits, into a byte array
        /// </summary>
        /// <param name="bindata">the binary data to be converted</param>
        /// <returns>the equivalent byte tab</returns>
        public static byte[] ToDecTab(string bindata)
        {
            //Left padding with 0
            while (bindata.Length % 8 != 0)
            {
                bindata = "0" + bindata;
            }
            byte[] poly = new byte[bindata.Length / 8];
            for (int i = 0; i < bindata.Length; i += 8)
            {
                string tempbin = "";
                for (int j = 0; j < 8; j++)
                {
                    tempbin += bindata[i + j];
                }
                poly[i / 8] = ToDec(tempbin);
            }

            return poly;
        }



        /// <summary>
        /// converts a tab of bytes to tab of binary in form of strings, with the binary being the specified length
        /// </summary>
        /// <param name="values">the byte tab to be converted into binary</param>
        /// <param name="len">the length of each individual binary number for left padding</param>
        /// <returns></returns>
        public static string[] ToBinTab(byte[] values, int len)
        {
            string[] temp = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                temp[i] = ToBin(values[i], len);
            }

            return temp;
        }



        /// <summary>
        /// Converts a long string of binary into a 8-bit long array of binary numbers as strings
        /// </summary>
        /// <param name="bin">the long binary data</param>
        /// <returns>the binary tab containing only 8 bits long binary numbers</returns>
        public static string[] ToBinTab(string bin)
        {
            while (bin.Length % 8 != 0)
            {
                bin = "0" + bin;
            }
            string[] temp = new string[bin.Length / 8];
            for (int i = 0; i < bin.Length; i += 8)
            {
                string tempbin = "";
                for (int j = 0; j < 8; j++)
                {
                    tempbin += bin[i + j];
                }
                temp[i / 8] = tempbin;
            }

            return temp;
        }

        #endregion

        //==================================================================================================================================================================================================================================================
        // SHOW FUNCTIONS (TO DEBUG)
        //==================================================================================================================================================================================================================================================

        #region Show functions (to debug)

        /// <summary>
        /// Shows a long string of bits with a space char at each 8 bits, for debug mainly
        /// </summary>
        /// <param name="bits">the long bit string</param>
        public static void ShowBitString(string bits)
        {
            for (int i = 0; i < bits.Length; i++)
            {
                Console.Write(bits[i]);
                if ((i + 1) % 8 == 0)
                {
                    Console.Write(" ");
                }

            }
        }



        /// <summary>
        /// Show a string tab, debug function
        /// </summary>
        /// <param name="var">the tab to be shown</param>
        public static void ShowArray(string[] var)
        {
            for (int i = 0; i < var.Length; i++)
            {
                Console.Write(var[i] + " ");
                if (i == 53) Console.Write("[HEADEREND]");
                if (i > 53 && i % 3 == 2)
                {
                    Console.Write(".");
                }
            }
        }



        /// <summary>
        /// Show a int tab, debug function
        /// </summary>
        /// <param name="tab">the tab to be shown</param>
        public static void ShowTab(int[] tab)
        {
            for (int i = 0; i < tab.Length; i++)
            {
                Console.Write(tab[i] + " ");
            }
            Console.WriteLine("");
        }



        /// <summary>
        /// Show a byte tab, debug function
        /// </summary>
        /// <param name="tab">the tab to be shown</param>
        public static void ShowByteTab(byte[] tab)
        {
            for (int i = 0; i < tab.Length; i++)
            {
                Console.Write(tab[i] + " ");
            }
            Console.WriteLine("");
        }

        #endregion

        //==================================================================================================================================================================================================================================================
        // SANITIZE
        //==================================================================================================================================================================================================================================================

        #region Sanitize

        /// <summary>
        /// Sanitize user input over a query for an int
        /// </summary>
        /// <param name="min">the minimum of the int queried</param>
        /// <param name="max">the maximum of the int queried</param>
        /// <returns>the int queried</returns>
        public static int IntQuery(int min, int max) //Méthode qui demande et vérifie si c'est bien un chiffre dans l'intervalle
        {
            int GoodInt = 0;
            bool IntNotGood = true; // Cet Méthode peut tourner infiniment si l'on ne choisit pas un chiffre
            while (IntNotGood)
            {

                string query = Console.ReadLine();
                if (query == "*")
                {
                    return 99999;
                }
                else
                {
                    bool IsQueryInt = Int32.TryParse(query, out int INT); //Error handling
                    if (IsQueryInt == true) //Error handling
                    {

                        GoodInt = INT;
                        if (GoodInt >= min)
                        {
                            if (GoodInt <= max)
                            {
                                IntNotGood = false;
                            }
                            else Utilities.RedWriteLine("[!] Woops too much, try again");

                        }
                        else Utilities.RedWriteLine("[!] Woops that's not enough probably, try again");

                    }
                    else
                    {
                        Utilities.RedWriteLine("[!] Are you sure that's even a number?");
                    }
                }
            }
            return GoodInt;
        }



        /// <summary>
        /// Sanitize user input over a query for a double
        /// </summary>
        /// <param name="min">the minimum of the double queried</param>
        /// <param name="max">the maximum of the double queried</param>
        /// <returns>the double queried</returns>
        public static double DoubleQuery(double min, double max) //Méthode qui demande et vérifie si c'est bien un chiffre dans l'intervalle
        {
            double GoodInt = 0;
            bool IntNotGood = true; // Cet Méthode peut tourner infiniment si l'on ne choisit pas un chiffre
            while (IntNotGood)
            {

                string query = Console.ReadLine();
                if (query == "*")
                {
                    return 99999;
                }
                else
                {
                    bool IsQueryInt = double.TryParse(query, out double INT); //Error handling
                    if (IsQueryInt == true) //Error handling
                    {

                        GoodInt = INT;
                        if (GoodInt > min)
                        {
                            if (GoodInt <= max)
                            {
                                IntNotGood = false;
                            }
                            else Utilities.RedWriteLine("[!] Woops too much, try again");

                        }
                        else Utilities.RedWriteLine("[!] Woops that's not enought probably, try again");

                    }
                    else
                    {
                        Utilities.RedWriteLine("[!] Are you sure that's even a number?");
                    }
                }
            }
            return GoodInt;
        }



        /// <summary>
        /// Ask the user for a bool, sanitize his input
        /// </summary>
        /// <returns>the bool inputed</returns>
        public static bool BoolQuery()
        {
            bool HasGivenGoodBool = false;
            bool returnedbool = false;
            while (!HasGivenGoodBool)
            {
                string query = Console.ReadLine().ToLower();
                List<string> Positive = new List<string>() { "oui", "yes", "y", "o", "true", "ouais", "yep", "yop", "t", "ye", "yeh", "yup", "ou", "oi", "oiu", "ys", "yse" };
                List<string> Negative = new List<string>() { "non", "no", "n", "false", "f", "nowtf", "on", "nn", "noo", "nooo", "noon", "nno", "flase", "fl", "fla", "flas", };
                if (Positive.Contains(query))
                {
                    returnedbool = true;
                    HasGivenGoodBool = true;
                }
                else
                {
                    if (Negative.Contains(query))
                    {
                        returnedbool = false;
                        HasGivenGoodBool = true;
                    }
                    else Utilities.RedWriteLine("[!] Are you sure this is a correct boolean? I'm really making efforts to understand you :c");
                }
            }
            return returnedbool;
        }

        #endregion

    }
}
