using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWheatstone
{
    /// <summary>
    /// Class to represent a complex number and handle complex computations
    /// </summary>
    class Complex
    {
        //==================================================================================================================================================================================================================================================
        // CONSTRUCTOR
        //==================================================================================================================================================================================================================================================

        #region Constructor

        //a complex number can be written with a real and imaginary part
        private double re;
        private double im;



        /// <summary>
        /// Empty constructor
        /// </summary>
        public Complex() { }



        /// <summary>
        /// Simple constructor
        /// </summary>
        /// <param name="re">real part of the number</param>
        /// <param name="im">imaginary part of the number</param>
        public Complex(double re, double im)
        {
            this.re = re;
            this.im = im;
        }



        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="z">complex number to copy</param>
        public Complex(Complex z)
        {
            this.re = z.re;
            this.im = z.im;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region get and sets
        public double Re
        {
            get { return this.re; }
            set { this.re = value; }
        }
        public double Im
        {
            get { return this.im; }
            set
            {
                this.im = value;
            }
        }
        #endregion

        #endregion

        //==================================================================================================================================================================================================================================================
        // FUNCTIONS
        //==================================================================================================================================================================================================================================================

        #region Functions

        /// <summary>
        /// Multiply 2 complex number
        /// </summary>
        /// <param name="z1">the first complex number</param>
        /// <param name="z2">the second complex number</param>
        /// <returns>the result of the multiplication</returns>
        public static Complex ComplexMult(Complex z1, Complex z2)
        {
            return new Complex(z1.Re * z2.Re - z1.Im * z2.Im, z1.Re * z2.Im + z2.Re * z1.Im);
        }



        /// <summary>
        /// Add 2 complex number
        /// </summary>
        /// <param name="z1">the first complex number</param>
        /// <param name="z2">the second complex number</param>
        /// <returns>the result of the addition</returns>
        public static Complex ComplexAdd(Complex z1, Complex z2)
        {
            return new Complex(z1.Re + z2.Re, z1.Im + z2.Im);
        }



        /// <summary>
        /// Computes the module of a complex number, its distance from (0,0) in the complex plane
        /// </summary>
        /// <returns>the module of the complex number</returns>
        public double Module()
        {
            return Math.Sqrt(this.Re * this.Re + this.Im * this.Im);
        }



        /// <summary>
        /// Tostring method for a complex number
        /// </summary>
        /// <returns>returns the complex number like: (a+bi)</returns>
        public string toString()
        {
            return "(" + this.Re.ToString() + "," + this.Im.ToString() + "i)";
        }

        #endregion

    }
}
