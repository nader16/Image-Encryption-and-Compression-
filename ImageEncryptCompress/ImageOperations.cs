using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Imaging;
///Algorithms Project
///Intelligent Scissors
///

namespace ImageQuantization
{
    public class Node
    {
        public int Value = 0;
        public long Frequency = 0;
        public Node Left = null;
        public Node Right = null;

        public Node(int val, int freq)
        {
            this.Value = val;
            this.Frequency = freq;
            this.Left = null;
            this.Right = null;
        }
    }
    public class Priority_Queue
    {
        public static void Min_Heapify(ref int h, List<Node> a, int i) // o(log N) 
        {
            int large_value = -1; // o(1) assignment
            int left = (2 * (i + 1)) - 1; // o(1) assignment
            int right = (2 * (i + 1)); // o(1) assignment
            if ((left < h) && (a[left].Frequency < a[i].Frequency)) // o(1) 
            {
                large_value = left; // o(1) assignment
            }
            else
            {
                large_value = i; // o(1) assignment
            }
            if ((right < h) && (a[right].Frequency < a[i].Frequency)) // o(1) 
            {
                large_value = right; // o(1) assignment
            }
            if (large_value != i) // o(1) assignment
            {
                Node temp = a[i]; // o(1) (assignment && retrive index in array)
                a[i] = a[large_value]; //o(1)(assignment && retrive index in array)
                a[large_value] = temp; //o(1)(assignment && retrive index in array)
                Min_Heapify(ref h, a, large_value); //T(2 N / 3) 
                                                    //T(N) <= T(2 N / 3) + θ (1)
            }

        }
        public static void Build_MinHeap(List<Node> a) // o(Nlog N)
        {
            int heap_size = a.Count(); //o(1)-Assignment
            for (int i = ((a.Count()) / 2); i >= 0; i--) //o(N)
            {
                Min_Heapify(ref heap_size, a, i); //O(log N)
            }
        }
        public static Node Extract_HeapMin(List<Node> a, ref int h)//o(log N)
        {
            Node Min = a[0]; //o(1)-(Assignment && reterive value from array
            a[0] = a[h - 1]; //o(1)-(Assignment && reterive value from array
            h = h - 1; //o(1)-(Assignment)
            Min_Heapify(ref h, a, 0); //o(log N)
            return Min; //o(1)
        }
        public static void Heap_Decrease_key(List<Node> a, int i, Node key) //o(log N)
        {
            a[i] = key; //o(1)-Assignment
            while ((i > 0) && (a[i / 2].Frequency > a[i].Frequency)) //o(Log N) AS IN EACH TIME WE DIVIE BY 2
            {
                Node temp = a[i]; //o(1)-(Assignment && reterive value from array
                a[i] = a[i / 2]; //o(1)-(Assignment && reterive value from array
                a[i / 2] = temp; //o(1)-(Assignment && reterive value from array
                i = i / 2;//o(1)-Assignment
            }
        }
        public static void Min_Heap_Insert(List<Node> a, Node key, ref int h) //o(log N)
        {
            h = h + 1; //o(1)-Assignment
            Heap_Decrease_key(a, h - 1, key); //o(log N)
        }
    }

    /// <summary>
    /// Holds the pixel color in 3 byte values: red, green and blue
    /// </summary>
    public struct RGBPixel
    {
        public byte red, green, blue;
    }

    /// <summary>
    /// Library of static functions that deal with images
    /// </summary>
    public class ImageOperations
    {
        /// <summary>
        /// Open an image and load it into 2D array of colors (size: Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of colors</returns>
        public static RGBPixel[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            RGBPixel[,] Buffer = new RGBPixel[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].red = p[2];
                            Buffer[y, x].green = p[1];
                            Buffer[y, x].blue = p[0];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }
        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }
        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }
        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        /// 
        public static void DisplayImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[2] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[0] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }



        public static void Get_Password(ref Int64 Seed, int Tape, int n) //θ(1)
        {
            Int64 newBit, TAPEv, MSBv, X = 1; //o(1)-Assignment
            for (int z = 0; z < 8; z++) //o(1)
            {
                MSBv = Seed; //o(1) -assignment
                MSBv = MSBv & (X << n); //o(1)-(assignment , shift left ,and)
                MSBv = MSBv >> n; //o(1)-shift right 
                TAPEv = Seed; //o(1) -assignment
                TAPEv = TAPEv & (X << Tape); //o(1)-(assignment , shift left,and)
                TAPEv = TAPEv >> Tape; //o(1)-(assignment , shift right)
                newBit = MSBv ^ TAPEv; //o(1)-(assignment , xor)
                Seed = Seed << 1; //o(1)-(assignment , shift left)
                Seed = Seed | newBit; //o(1)-(assignment , or)
            }
        }
        public static string AlphaNumericPW(string s)//θ(s.Length)
        {
            int chk = 0;    //o(1)-Assignment
            for (int i = 0; i < s.Length; i++)  //o(s.Length)
            {
                if (s.ElementAt(i).Equals('0') || s.ElementAt(i).Equals('1'))
                    chk++;  //o(1)
            }
            if (chk != s.Length)
            {
                string result = string.Empty;   //o(1)-Assignment
                foreach (char ch in s)  //o(s.Length)
                {
                    result += Convert.ToString((int)ch, 2); //o(1)
                }
                if (result.Length > 63)
                    result = result.Substring(result.Length - 63, 63);
                return result;
            }
            return s;
        }
        public static void Encrypt_Decrypt(ref RGBPixel[,] ImageMatrix, string seed, int tape)  // o(h*w) + o(s.length) 
        {
            seed = AlphaNumericPW(seed);    //o(s.length)
            Int64 se = Convert.ToInt64(seed, 2); //o(1) -(assignment , convert)
            int sLen = seed.Length - 1; //o(1) -Assignment
            for (int i = 0; i < GetHeight(ImageMatrix); i++) //o(h*w)
            {
                for (int j = 0; j < GetWidth(ImageMatrix); j++) //o(w) 
                {
                    //=========Encrypt/Decrypt Red Component=========

                    Get_Password(ref se, tape, sLen); //o(1)
                    Int64 w = se & 255; // o(1) -(Assignment && and operator)
                    ImageMatrix[i, j].red = (byte)((ImageMatrix[i, j].red) ^ w); //o(1) (Assignment && xor)
                    //=========Encrypt/Decrypt Green Component=========
                    Get_Password(ref se, tape, sLen); //o(1)
                    w = se & 255;  // o(1) -(Assignment && and operator)
                    ImageMatrix[i, j].green = (byte)((ImageMatrix[i, j].green) ^ w); //o(1) (Assignment && xor)
                    //   =========Encrypt/Decrypt Blue Component=========
                    Get_Password(ref se, tape, sLen); //o(1)
                    w = se & 255;  // o(1) -(Assignment && and operator)
                    ImageMatrix[i, j].blue = (byte)((ImageMatrix[i, j].blue) ^ w); //o(1) (Assignment && xor)
                }
            }
        }



        public static int red_length = 0, green_length = 0, blue_length = 0, Tape_Position = 0;
        public static int[] R, G, B;
        public static float matrix_dimintion = 0;
        public static long red_bytes = 0, green_bytes = 0, blue_bytes = 0, Total_Bits = 0;
        public static string Initial_Seed = "";
        public static string[] arr_red, arr_gr, arr_bl;
        public static byte[] arr, arr1, arr2;

        public static void str_1(RGBPixel[,] ImageMatrix)//O(h*w)
        {
            arr = new byte[red_bytes]; //O(1) (assignment && new].
            arr1 = new byte[green_bytes]; //O(1) (assignment && new].
            arr2 = new byte[blue_bytes]; //O(1) (assignment && new].
            int ar = 8, ag = 8, ab = 8; //O(1)-(assignment)
            int index_red = 0, index_green = 0, index_blue = 0; //o(1)-(assignment)
            string temp, temp1, temp2, rf, rf1, rf2;
            for (int i = 0; i < GetHeight(ImageMatrix); i++) //o(h*w)
            {
                for (int j = 0; j < GetWidth(ImageMatrix); j++) //o(w)
                {
                    temp = arr_red[ImageMatrix[i, j].red]; //o(1)-assignment
                    if (temp.Length < ar)//o(1) 
                    {
                        arr[index_red] <<= temp.Length; //o(1)-(put index in array && shift)
                        arr[index_red] += Convert.ToByte(temp, 2); //o(1)-(put index in array && assignment && addition && convert)
                        ar -= temp.Length; //o(1)-subtraction && assignment
                    }
                    else if (temp.Length == ar)
                    {
                        arr[index_red] <<= temp.Length; ////o(1)-(put index in array && shift)
                        arr[index_red] += Convert.ToByte(temp, 2); //o(1)-(put index in array && assignment && addition && convert)
                        index_red++; // o(1)-addition && assignment 
                        ar = 8; //o(1)-assignment
                    }
                    else
                    {
                        rf = temp.Substring(0, ar);  //o(1) - assignment && substring
                        arr[index_red] <<= ar; //o(1)-(put index in array && shift)
                        arr[index_red] += Convert.ToByte(rf, 2);  //o(1)-(put index in array && assignment && addition && convert)
                        index_red++; //o(1)-addition && assignment
                        temp = temp.Substring(ar, temp.Length - ar);//o(1) - assignment && substring

                        while (temp.Length >= 8) //o(1) AS temp size is limited to 32
                        {
                            rf = temp.Substring(0, 8); //o(1)-assignment && substring
                            arr[index_red] <<= 8; //o(1)-(put index in array && shift)
                            arr[index_red] += Convert.ToByte(rf, 2); //o(1)-(put index in array && assignment && addition && convert)
                            index_red++; //o(1)-addition && assignment
                            temp = temp.Substring(8, temp.Length - 8); //o(1) - assignment && substring
                        }
                        if (temp.Length != 0) //o(1)
                        {
                            arr[index_red] <<= temp.Length; //o(1)-(put index in array && shift)
                            arr[index_red] += Convert.ToByte(temp, 2); //o(1)-(put index in array && assignment && addition && convert)
                            ar = 8 - temp.Length; // o(1) - assignment 
                        }
                        else
                            ar = 8; //o(1) - assignment 
                    }
                    temp1 = arr_gr[ImageMatrix[i, j].green]; //o(1)-assignment
                    if (temp1.Length < ag) //o(1)
                    {
                        arr1[index_green] <<= temp1.Length; //o(1)-(put index in array && shift)
                        arr1[index_green] += Convert.ToByte(temp1, 2);//o(1)-(put index in array && assignment && addition && convert)
                        ag -= temp1.Length; //o(1)-subtraction && assignment
                    }
                    else if (temp1.Length == ag)
                    {
                        arr1[index_green] <<= temp1.Length; //o(1)-(put index in array && shift)
                        arr1[index_green] += Convert.ToByte(temp1, 2); //o(1)-(put index in array && assignment && addition && convert)
                        index_green++; //o(1)-addition && assignment 
                        ag = 8; //o(1)-assignment
                    }
                    else
                    {
                        rf1 = temp1.Substring(0, ag);//o(1) - assignment && substring
                        arr1[index_green] <<= ag; //o(1)-(put index in array && shift)
                        arr1[index_green] += Convert.ToByte(rf1, 2); //o(1)-(put index in array && assignment && addition && convert)
                        index_green++; // o(1)-addition && assignment 
                        temp1 = temp1.Substring(ag, temp1.Length - ag);//o(1) - assignment && substring
                        while (temp1.Length >= 8) //o(1) AS temp size is limited to 32
                        {
                            rf1 = temp1.Substring(0, 8);//o(1)-assignment && substring
                            arr1[index_green] <<= 8; //o(1)-(put index in array && shift)
                            arr1[index_green] += Convert.ToByte(rf1, 2);//o(1)-(put index in array && assignment && addition && convert)
                            index_green++; //o(1)-addition && assignment
                            temp1 = temp1.Substring(8, temp1.Length - 8); //o(1) - assignment && substring
                        }
                        if (temp1.Length != 0) //o(1)
                        {
                            arr1[index_green] <<= temp1.Length; //o(1)-(put index in array && shift)
                            arr1[index_green] += Convert.ToByte(temp1, 2); //o(1)-(put index in array && assignment && addition && convert)
                            ag = 8 - temp1.Length; // o(1) - assignment 
                        }
                        else
                            ag = 8; // o(1) - assignment 
                    }
                    temp2 = arr_bl[ImageMatrix[i, j].blue]; //o(1)-assignment
                    if (temp2.Length < ab) //o(1)
                    {
                        arr2[index_blue] <<= temp2.Length; //o(1)-(put index in array && shift)
                        arr2[index_blue] += Convert.ToByte(temp2, 2);//o(1)-(put index in array && assignment && addition && convert)
                        ab -= temp2.Length; //o(1)-subtraction && assignment
                    }
                    else if (temp2.Length == ab)
                    {
                        arr2[index_blue] <<= temp2.Length; //o(1)-(put index in array && shift)
                        arr2[index_blue] += Convert.ToByte(temp2, 2); //o(1)-(put index in array && assignment && addition && convert)
                        index_blue++;//o(1)-addition && assignment 
                        ab = 8; //o(1)-assignment
                    }
                    else
                    {
                        rf2 = temp2.Substring(0, ab);//o(1) - assignment && substring
                        arr2[index_blue] <<= ab;//o(1)-(put index in array && shift)
                        arr2[index_blue] += Convert.ToByte(rf2, 2); //o(1)-(put index in array && assignment && addition && convert)
                        index_blue++; //o(1)-addition && assignment 
                        temp2 = temp2.Substring(ab, temp2.Length - ab); //o(1) - assignment && substring

                        while (temp2.Length >= 8) //o(1) AS temp size is limited to 32
                        {
                            rf2 = temp2.Substring(0, 8); //o(1)-assignment && substring
                            arr2[index_blue] <<= 8;//o(1)-(put index in array && shift)
                            arr2[index_blue] += Convert.ToByte(rf2, 2);//o(1)-(put index in array && assignment && addition && convert)
                            index_blue++;//o(1)-addition && assignment
                            temp2 = temp2.Substring(8, temp2.Length - 8);//o(1) - assignment && substring
                        }
                        if (temp2.Length != 0) //o(1)
                        {
                            arr2[index_blue] <<= temp2.Length; //o(1)-(put index in array && shift)
                            arr2[index_blue] += Convert.ToByte(temp2, 2); //o(1)-(put index in array && assignment && addition && convert)
                            ab = 8 - temp2.Length; // o(1) - assignment 
                        }
                        else
                            ab = 8; // o(1) - assignment 
                    }
                }

            }

        }
        public static void Huffman_Code(ref RGBPixel[,] ImageMatrix)
        {
            matrix_dimintion = ((GetHeight(ImageMatrix)) * (GetWidth(ImageMatrix))) * 24;
            long rem = 0; //o(1)-Assignment
            List<Node> RedL = new List<Node>();
            List<Node> GreenL = new List<Node>();
            List<Node> BlueL = new List<Node>();
            Total_Bits = red_bytes = green_bytes = blue_bytes = 0;//o(1)-Assignment
            R = new int[256]; //o(1)-(Assignment - new)
            G = new int[256]; //o(1)-Assignment-new)
            B = new int[256]; //o(1)-Assignment-new
            for (int i = 0; i < GetHeight(ImageMatrix); i++) //o(h*w)
            {
                for (int j = 0; j < GetWidth(ImageMatrix); j++) //o(w)
                {
                    R[Convert.ToInt32(ImageMatrix[i, j].red)] += 1; //o(1)-(Assignment-convert-addition- 
                    G[Convert.ToInt32(ImageMatrix[i, j].green)] += 1; //o(1)-(Assignment-convert-addition- 
                    B[Convert.ToInt32(ImageMatrix[i, j].blue)] += 1; //o(1)-(Assignment-convert-addition- 
                }
            }
            for (int i = 0; i < 256; i++)//o(1)
            {
                if (R[i] != 0)
                { Node x = new Node(i, R[i]); RedL.Add(x); }    //o(1)-(Assignment - new) & (Add the end of list)
                if (G[i] != 0)
                { Node x = new Node(i, G[i]); GreenL.Add(x); }  //o(1)-(Assignment - new) & (Add the end of list)
                if (B[i] != 0)
                { Node x = new Node(i, B[i]); BlueL.Add(x); }   //o(1)-(Assignment - new) & (Add the end of list)

            }
            red_length = RedL.Count(); //o(1)-Assignment
            green_length = GreenL.Count(); //o(1)-Assignment
            blue_length = BlueL.Count(); //o(1)-Assignment
            arr_red = new string[256]; //o(1)-Assignment && new
            arr_gr = new string[256];//o(1)-Assignment && new
            arr_bl = new string[256];//o(1)-Assignment && new
            FileStream fs = new FileStream("RGB-Tree.txt", FileMode.Truncate);
            StreamWriter sw = new StreamWriter(fs);
            string BinVal = "";
            Node RTRoot = Build_Huffman(RedL); //o(Nlog N)
            Save_Tree(RTRoot, sw, BinVal, arr_red, ref red_bytes); //o(log n)
            rem = red_bytes % 8; //o(1)-assignment
            red_bytes = red_bytes / 8; //o(1)-assignment
            if (rem != 0)
            {
                red_bytes += 1; //o(1)-assignment
            }

            // temp.Clear();
            sw.WriteLine("=============================");
            Node GTRoot = Build_Huffman(GreenL);//o(Nlog N)
            Save_Tree(GTRoot, sw, BinVal, arr_gr, ref green_bytes);//o(log n)
            rem = green_bytes % 8;  //o(1)-assignment
            green_bytes = green_bytes / 8;  //o(1)-assignment
            if (rem != 0)
            {
                green_bytes += 1;   //o(1)-assignment
            }

            // temp.Clear();
            sw.WriteLine("=============================");
            Node BTRoot = Build_Huffman(BlueL);//o(Nlog N)
            Save_Tree(BTRoot, sw, BinVal, arr_bl, ref blue_bytes);//o(log n)
            rem = blue_bytes % 8;   //o(1)-assignment
            blue_bytes = blue_bytes / 8;    //o(1)-assignment
            if (rem != 0) //o(1)-assignment
            {
                blue_bytes += 1;    //o(1)-assignment
            }

            // temp.Clear();
            long res = (Total_Bits) / 8;    //o(1)-assignment
            sw.WriteLine(Convert.ToString(res));//o(1)(convert && writ in file)
            float ratio = (Total_Bits) / (ImageOperations.matrix_dimintion) * 100;  //o(1)-assignment
            sw.WriteLine(Convert.ToString(ratio) + " % ");//o(1)(convert && writ in file)
            sw.Close();
            fs.Close();
        }
        public static Node Build_Huffman(List<Node> Component) //o(Nlog N)
        {
            Priority_Queue.Build_MinHeap(Component);//o(log N)
            int C_Heap = Component.Count();//o(1)-Assignment
            int n = Component.Count() - 1;//o(1)-Assignment
            for (int i = 0; i < n; i++) //o(N) *o(logN)
            {
                Node temp = new Node(0, 0); //o(1)
                Node right = Priority_Queue.Extract_HeapMin(Component, ref C_Heap);//o(log N)
                Node left = Priority_Queue.Extract_HeapMin(Component, ref C_Heap);//o(log N)
                temp.Frequency = (left.Frequency + right.Frequency); //o(1)-Assignment
                temp.Left = left; //o(1)-Assignment
                temp.Right = right; //o(1)-Assignment
                Priority_Queue.Min_Heap_Insert(Component, temp, ref C_Heap);//o(log N)
            }
            return Priority_Queue.Extract_HeapMin(Component, ref C_Heap); //o(log N)
        }
        public static void Save_Tree(Node Root, StreamWriter sw, string BinaryVal, string[] arr, ref long length) //o(log N) as the recuersive relation is as T(N)=N/2+θ
        {
            if (Root.Right != null) Save_Tree(Root.Right, sw, BinaryVal + '1', arr, ref length);

            if (Root.Left != null) Save_Tree(Root.Left, sw, BinaryVal + '0', arr, ref length);

            if ((Root.Left == null) && (Root.Right == null))//o(1)
            {

                sw.WriteLine(Convert.ToString(Root.Value) + " " + BinaryVal + " " + Convert.ToString(Root.Frequency) + " " + Convert.ToString((BinaryVal.Length) * (Root.Frequency))); //o(1)
                Total_Bits += ((BinaryVal.Length) * (Root.Frequency)); //o(1)-(addition&& assignment)
                length += ((BinaryVal.Length) * (Root.Frequency)); //o(1)-(addition&& assignment)
                arr[Root.Value] = BinaryVal; //o(1)
            }
            


        }
        public static void compress_image(int[] red_freq, int[] green_freq, int[] blue_freq, byte[] red_com, byte[] green_com, byte[] blue_com, int tape, string seed, int w, int h)//o(Nlog N)
        {
            byte[] redd = new byte[1024];//o(1) (assignment)
            byte[] grenn = new byte[1024];//o(1) (assignment)
            byte[] bluee = new byte[1024];//o(1) (assignment)
            for (int i = 0; i < 256; i++)
            {
                Array.Copy(BitConverter.GetBytes(red_freq[i]), 0, redd, i * 4, 4);//o(nlon)(number of iterations*4 && copy to array)
                Array.Copy(BitConverter.GetBytes(green_freq[i]), 0, grenn, i * 4, 4);//o(nlon)(number of iterations*4 && copy to array)
                Array.Copy(BitConverter.GetBytes(blue_freq[i]), 0, bluee, i * 4, 4);//o(nlon)(number of iterations*4 && copy to array)
            }
            FileStream ffs = new FileStream("com.txt", FileMode.Truncate);
            StreamWriter ffss = new StreamWriter(ffs);
            ffss.WriteLine(red_com.Length);//o(1) (write in file)
            ffss.WriteLine(green_com.Length);//o(1) (write in file)
            ffss.WriteLine(blue_com.Length);//o(1) (write in file)
            ffss.Close();
            ffs.Close();

            FileStream ss = new FileStream("compressed.txt", FileMode.Truncate);
            BinaryWriter bwr = new BinaryWriter(ss);
            bwr.Write(redd);//o(1) (write in file)
            bwr.Write(grenn); //o(1) (write in file)
            bwr.Write(bluee);//o(1) (write in file)
            bwr.Write(red_com);//o(1) (write in file)
            bwr.Write(green_com);//o(1) (write in file)
            bwr.Write(blue_com);//o(1) (write in file)
            bwr.Write(seed);//o(1) (write in file)
            bwr.Write(tape);//o(1) (write in file)
            bwr.Write(w);//o(1) (write in file)
            bwr.Write(h);//o(1) (write in file)
            bwr.Close();
            ss.Close();
        }
        public static Node get_next_node(byte p, Node n)//o(1)return the Next node to get path
        {
            if (p == 0)//o(1) Assigment    
            { return n.Left; }//o(1)return
            else
                return n.Right;//o(1)return
        }
        public static List<int> decompress(byte[] c, Node root)//o(n) get the list of specfiec color     
        {
            List<int> temp = new List<int>();//o(1) define a new list that save the color values
            byte var = 128;//o(1) assigment to get the bits from the byte color
            int accu = 0;// o(1) assigment that count to 8 to get the value of one byte 
            Node f = root;// o(1) assigment that point to the top node in huffman
            for (int i = 0; i < c.Length;)//o(s.length)
            {
                while (accu < 8)//o(1) this loop count untill the byte read all 
                {
                    byte aa = (byte)(c[i] & var);//o(1) assigmrnt to get the spceific bit
                    Node check = get_next_node(aa, f);//o(1) call the function to get the next node according to the bit value
                    if (check.Left == null && check.Right == null)//o(1) check that the cuurent node is a leaf node
                    {
                        temp.Add(check.Value);// o(1) add to the list the value of the cuurent color
                        f = root;// O(1) make search start from the root or the huffman
                    }
                    else

                        f = check;//o(1)make the start node is the current node 

                    var /= 2;// o(1)  divide the var to get the next bit 
                    accu++;// o(1) read another 8 bits

                }
                i++;// o(1) go to the next list value to save on it
                accu = 0;// o(1) reset the counter
                var = 128;// o(1) reset the var to make it point to the first bit
            }
            return temp;//o(1) return the list that contain the colors values
        }
        public static RGBPixel[,] decompress_image()//o(h*w)
        {
            FileStream fo = new FileStream("com.txt", FileMode.Open);
            StreamReader ffo = new StreamReader(fo);
            int red_l = Convert.ToInt32(ffo.ReadLine());//o(1)read the red_bytes length
            int green_l = Convert.ToInt32(ffo.ReadLine());//o(1) read the green bytes length
            int blue_l = Convert.ToInt32(ffo.ReadLine());//o(1) read the blue bytes length 
            ffo.Close();
            fo.Close();
            FileStream fs = new FileStream("compressed.txt", FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            byte[] red_freq = br.ReadBytes(1024);
            byte[] green_freq = br.ReadBytes(1024);
            byte[] blue_freq = br.ReadBytes(1024);
            int[] red1 = new int[256];
            int[] green1 = new int[256];
            int[] blue1 = new int[256];
            List<Node> red_frq = new List<Node>();
            List<Node> green_frq = new List<Node>();
            List<Node> blue_frq = new List<Node>();

            for (int i = 0; i < 1024; i += 4)//o(1) this loop take 4 bytes and compress them to one int32 value
            {
                red1[i / 4] = BitConverter.ToInt32(red_freq, i);
                green1[i / 4] = BitConverter.ToInt32(green_freq, i);
                blue1[i / 4] = BitConverter.ToInt32(blue_freq, i);
            }
            for (int j = 0; j < 256; j++)//o(1) this loop add in the lists that it's value is not 0  
            {
                if (red1[j] != 0)
                { Node x = new Node(j, red1[j]); red_frq.Add(x); }
                if (green1[j] != 0)
                { Node x = new Node(j, green1[j]); green_frq.Add(x); }
                if (blue1[j] != 0)
                { Node x = new Node(j, blue1[j]); blue_frq.Add(x); }

            }
            Node r1 = Build_Huffman(red_frq);//o(n logn)
            Node r2 = Build_Huffman(green_frq);//o(n logn)
            Node r3 = Build_Huffman(blue_frq);//o(n logn)
            byte[] red_co = br.ReadBytes(red_l);//o(1) read from the file the red bytes compressed values
            byte[] green_co = br.ReadBytes(green_l);//o(1) read from the file the green bytes compressed values
            byte[] blue_co = br.ReadBytes(blue_l);//o(1) read from the file the blue bytes compressed values
            string seed = br.ReadString();// o(1) get the seed from the file 
            int TAPE = br.ReadInt32();//o(1 )get the tape from the file 
            int width = br.ReadInt32();// o(1) get the width from the file
            int heigth = br.ReadInt32();// o(1) get the heigth from the file 
            br.Close();
            fs.Close();
            Tape_Position = TAPE;//o(1) save it to he global value
            Initial_Seed = seed;//o(1) save it to the global value
            List<int> redd = decompress(red_co, r1);//o(n) call the function that get the acctual values of red 
            List<int> grenn = decompress(green_co, r2);//o(n)call the function that get the acctual values of green
            List<int> bluee = decompress(blue_co, r3);// o(n)call the function that get the acctual values of blue
            RGBPixel[,] com_image = new RGBPixel[heigth, width];// create a new RGBpixel that get the image values 
            int index = 0;
            // this nessted loop create the iamge that displayed to the user
            for (int i = 0; i < heigth; i++)//o(H)
            {
                for (int j = 0; j < width; j++)//o(W)
                {
                    com_image[i, j].red = (byte)redd[index];
                    com_image[i, j].green = (byte)grenn[index];
                    com_image[i, j].blue = (byte)bluee[index];
                    index++;
                }
            }

            return com_image;//o(1) retun the image to the user
        }
    }
}