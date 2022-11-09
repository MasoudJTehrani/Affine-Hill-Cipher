using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Affine_Hill_Cipher
{
    public partial class Form1 : Form
    {
        private IconButton currentBtn;
        private Panel bottomBorderBtn;
        byte[] theInput;
        byte[] thePtext;
        byte[] theCtext;
        int[] thehackedkey = new int[6];
        int[] Pinverse = new int[9];
        byte[] theP = new byte[6];
        byte[] theC = new byte[6];
        static byte[] theOutput = new byte[1];
        int[] H;
        int[] Hinverse;
        static int theb1;
        static int theb2;
        StringBuilder sb;
        public Form1()
        {
            InitializeComponent();
            bottomBorderBtn = new Panel();
            bottomBorderBtn.Size = new Size(665, 7);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HackPANEL.Location = EncDecPANEL.Location;
        }

        // Interface design \\
        private void ActivateButton(object senderBtn, Color color)
        {
            if (senderBtn != null)
            {
                DisableButton();
                //Button
                currentBtn = (IconButton)senderBtn;
                currentBtn.BackColor = Color.FromArgb(20, 55, 55);
                currentBtn.Controls.Add(bottomBorderBtn);
                //bottom border button
                bottomBorderBtn.BackColor = color;
                bottomBorderBtn.Location = new Point(2, 53);
                bottomBorderBtn.Visible = true;
                bottomBorderBtn.BringToFront();

            }
        }
        private void DisableButton()
        {
            if (currentBtn != null)
            {
                currentBtn.BackColor = Color.FromArgb(0, 68, 69);
            }
        }
        // \\ end of design methods \\
        //
        // Interface methods \\
        private void EncDecBTN_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, Color.LawnGreen);
            EncDecPANEL.Visible = true;
            HackPANEL.Visible = false;
        }

        private void HackBTN_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, Color.Gold);
            EncDecPANEL.Visible = false;
            HackPANEL.Visible = true;
        }
        // Opening a file
        private byte[] openFile()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                byte[] fileBytes = File.ReadAllBytes(fileName);
                return fileBytes;
            }
            else
            {
                return null;
            }
        }

        // Downloading a file
        private void downloadFile(RichTextBox prevName)
        {
            if (prevName.Text == "")
            {
                MessageBox.Show("There is nothing to save");
                return;
            }

            saveFileDialog1.Filter = "Text files (*.txt)|*.txt|Image file(*.jpg)|*.jpg|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.Title = "Save The Selected text";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(saveFileDialog1.FileName, theOutput);
            }
        }

        private void h1_ValueChanged(object sender, EventArgs e)
        {
            k1.Text = h1.Value.ToString();
            k2.Text = h2.Value.ToString();
            k3.Text = b1.Value.ToString();
            k4.Text = h3.Value.ToString();
            k5.Text = h4.Value.ToString();
            k6.Text = b2.Value.ToString();
            checkTheKey();
        }

        private void DLFileBTN_Click(object sender, EventArgs e)
        {
            if(OutputTXTBOX.Text == "")
            {
                MessageBox.Show("There is nothing to save");
                return;
            }
            downloadFile(OutputTXTBOX);
        }

        private void ptextopenBTN_Click(object sender, EventArgs e)
        {
            thePtext = openFile();
            if (thePtext == null)
                return;
            sb = new StringBuilder();
            thePtext.ToList().ForEach(i => sb.Append(i + ","));
            ptextTXTBOX.Text = sb.ToString();
        }

        private void ctextopenBTN_Click(object sender, EventArgs e)
        {
            theCtext = openFile();
            if (theCtext == null)
                return;
            sb = new StringBuilder();
            theCtext.ToList().ForEach(i => sb.Append(i + ","));
            ctextTXTBOX.Text = sb.ToString();
        }

        private void hackkeyBTN_Click(object sender, EventArgs e)
        {
            // error handing
            if (ctextTXTBOX.Text == "" || ptextTXTBOX.Text == "")
            {
                MessageBox.Show("First insert both the plain text and the cipher text");
                return;
            }
            //
            int half = thePtext.Length / 2;
            for (int i = 0; i < half; i++)
            {
                theP[0] = thePtext[2 * i];
                theP[3] = thePtext[2 * i + 1];
                theP[1] = thePtext[2 * i + 2];
                theP[4] = thePtext[2 * i + 3];
                theP[2] = thePtext[2 * i + 4];
                theP[5] = thePtext[2 * i + 5];

                theC[0] = theCtext[2 * i];
                theC[3] = theCtext[2 * i + 1];
                theC[1] = theCtext[2 * i + 2];
                theC[4] = theCtext[2 * i + 3];
                theC[2] = theCtext[2 * i + 4];
                theC[5] = theCtext[2 * i + 5];
                if (calcinverse3())
                    return;
            }
            MessageBox.Show("Couldn't find a key for your inputs");
        }

        private void OpenFileBTN_Click(object sender, EventArgs e)
        {
            theInput = openFile();
            if (theInput == null)
                return;
            sb = new StringBuilder();
            theInput.ToList().ForEach(i => sb.Append(i + ","));
            InputTXTBOX.Text = sb.ToString();
        }

        private void encryptBTN_Click(object sender, EventArgs e)
        {
            // error handing
            if (InputTXTBOX.Text == "")
            {
                MessageBox.Show("First insert the file please");
                return;
            }
            if (checkICON.Visible != true)
            {
                MessageBox.Show("Your key is not valid please enter a valid key");
                return;
            }
            //
            addPadding();
            Transformation enc = new Transformation(theInput.Length);
            enc.transform(theInput, "enc", H);

            StringBuilder sb = new StringBuilder();
            theOutput.ToList().ForEach(i => sb.Append(i + ","));
            OutputTXTBOX.Text = sb.ToString();
        }

        private void decryptBTN_Click(object sender, EventArgs e)
        {
            // error handing
            if (InputTXTBOX.Text == "")
            {
                MessageBox.Show("First insert the file first");
                return;
            }
            if (checkICON.Visible != true)
            {
                MessageBox.Show("Your key is not valid please enter a valid key");
                return;
            }
            //
            Hinverse = calcinverse(H);
            Transformation dec = new Transformation(theInput.Length);
            dec.transform(theInput, "dec", Hinverse);
            removePadding();

            StringBuilder sb = new StringBuilder();
            theOutput.ToList().ForEach(i => sb.Append(i + ","));
            OutputTXTBOX.Text = sb.ToString();
        }

        // \\ end of Interface methods //
        //
        // programming methods \\
        private void checkTheKey()
        {
            H = new int[] { Convert.ToInt32(h1.Value), Convert.ToInt32(h2.Value),
            Convert.ToInt32(h3.Value), Convert.ToInt32(h4.Value)};

            if (isInvertible())
            {
                checkICON.Visible = true;
                notcheckICON.Visible = false;
                validLABEL.Visible = true;
                invalidLABEL.Visible = false;
            }
            else
            {
                checkICON.Visible = false;
                notcheckICON.Visible = true;
                validLABEL.Visible = false;
                invalidLABEL.Visible = true;
            }
        }
        private bool isInvertible()
        {
            int determinant = positiveMaker((H[0] * H[3]) - (H[1] * H[2])) % 256;

            if(determinant == 0 || gcd(determinant, 256) != 1)
            {
                return false;
            }
            return true;
        }

        private int gcd(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b;
        }
        private int modInverse(int a, int n)
        {
            int i = n, v = 0, d = 1;
            while (a > 0)
            {
                int t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= n;
            if (v < 0) v = (v + n) % n;
            return v;
        }

        private void RandKeyBTN_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            do
            {
                h1.Value = rnd.Next(0, 255);
                h2.Value = rnd.Next(0, 255);
                h3.Value = rnd.Next(0, 255);
                h4.Value = rnd.Next(0, 255);
                H = new int[] { Convert.ToInt32(h1.Value), Convert.ToInt32(h2.Value),
            Convert.ToInt32(h3.Value), Convert.ToInt32(h4.Value)};

            } while(!isInvertible());

            b1.Value = rnd.Next(0, 255);
            b2.Value = rnd.Next(0, 255);
            theb1 = Convert.ToInt32(b1.Value);
            theb2 = Convert.ToInt32(b2.Value);

            checkICON.Visible = true;
            notcheckICON.Visible = false;
            validLABEL.Visible = true;
            invalidLABEL.Visible = false;
        }

        public class Transformation
        {
            public int Length;
            public int halfLength;
            public List<byte> tempOutput = new List<byte>();
            byte[] block = new byte[2];

            public Transformation(int Length)
            {
                this.Length = Length;
                this.halfLength = Length / 2;
                theOutput = new byte[Length];
                
            }

            public void transform(byte[] inputSource, string transformType, int[] inputKey)
            {
                Block_reader(inputSource, transformType, inputKey);

                theOutput = tempOutput.ToArray();

                void Block_reader(byte[] src, string type, int[] key)
                {
                    for(int i = 0; i < halfLength; i++)
                    {
                        block[0] = src[2 * i];
                        block[1] = src[2 * i + 1];
                        Block_transformer(block, key, type);
                    }
                }

                void Block_transformer(byte[] S, int[] theKey, string theType)
                {
                    byte[] Cblock = new byte[2];

                    if (theType == "enc")
                    {
                        Cblock[0] = Convert.ToByte(( (theKey[0] * S[0]) + (theKey[1] * S[1]) + theb1) % 256);
                        Cblock[1] = Convert.ToByte(( (theKey[2] * S[0]) + (theKey[3] * S[1]) + theb2) % 256);
                    }
                    else
                    {
                        int newS0 = S[0] - theb1;
                        if(newS0 < 0)
                        {
                            newS0 += 256;
                        }
                        int newS1 = S[1] - theb2;
                        if (newS1 < 0)
                        {
                            newS1 += 256;
                        }
                        Cblock[0] = Convert.ToByte(( (theKey[0] * newS0) + (theKey[1] * newS1) ) % 256);
                        Cblock[1] = Convert.ToByte(( (theKey[2] * newS0) + (theKey[3] * newS1) ) % 256);
                    }
                    Block_writer(Cblock);
                }

                void Block_writer(byte[] B)
                {
                    tempOutput.Add(B[0]);
                    tempOutput.Add(B[1]);
                }
            }

        }

        private void addPadding()
        {
            if (theInput.Length % 2 == 0)
                return;
            var tempList = theInput.ToList();
            tempList.Add(1);
            theInput = tempList.ToArray();
        }

        private void removePadding()
        {
            if(theOutput[theOutput.Length - 1] == 1 && theOutput[theOutput.Length - 2] != 1)
            {
                var tempList = theOutput.ToList();
                tempList.RemoveAt(theOutput.Length - 1);
                theOutput = tempList.ToArray();
            }
        }

        
        private int[] calcinverse(int[] H)
        {
            int determinant = positiveMaker((H[0] * H[3]) - (H[1] * H[2])) % 256;

            int detInverse = modInverse(determinant, 256);
            int[] Result = new int[4];

            Result[0] = (H[3] * detInverse) % 256;
            Result[3] = (H[0] * detInverse) % 256;
            Result[1] = ((256 - H[1]) * detInverse) % 256;
            Result[2] = ((256 - H[2]) * detInverse) % 256;

            return Result;
        }

        
        private bool calcinverse3()
        {
            int determinant3 = positiveMaker((theP[0] * (theP[4] - theP[5])) - (theP[1] * (theP[3] - theP[5])) + (theP[2] * (theP[3] - theP[4]))) % 256;

            if ( determinant3 == 0 || gcd(determinant3, 256) != 1)
                return false;
            int detinverse3 = modInverse(determinant3, 256);

            // p inverse

            Pinverse[0] = positiveMaker((theP[4] - theP[5]) * detinverse3) % 256;
            Pinverse[1] = positiveMaker((theP[2] - theP[1]) * detinverse3) % 256;
            Pinverse[2] = positiveMaker(((theP[1] * theP[5]) - (theP[2] * theP[4])) * detinverse3) % 256;
            Pinverse[3] = positiveMaker((theP[5] - theP[3]) * detinverse3) % 256;
            Pinverse[4] = positiveMaker((theP[0] - theP[2]) * detinverse3) % 256;
            Pinverse[5] = positiveMaker(((theP[2] * theP[3]) - (theP[0] * theP[5])) * detinverse3) % 256;
            Pinverse[6] = positiveMaker((theP[3] - theP[4]) * detinverse3) % 256;
            Pinverse[7] = positiveMaker((theP[1] - theP[0]) * detinverse3) % 256;
            Pinverse[8] = positiveMaker(((theP[0] * theP[4]) - (theP[1] * theP[3])) * detinverse3) % 256;

            // calculating key
            thehackedkey[0] = ((Pinverse[0] * theC[0]) + (Pinverse[3] * theC[1]) + (Pinverse[6] * theC[2])) % 256;
            thehackedkey[1] = ((Pinverse[1] * theC[0]) + (Pinverse[4] * theC[1]) + (Pinverse[7] * theC[2])) % 256;
            thehackedkey[2] = ((Pinverse[2] * theC[0]) + (Pinverse[5] * theC[1]) + (Pinverse[8] * theC[2])) % 256;
            thehackedkey[3] = ((Pinverse[0] * theC[3]) + (Pinverse[3] * theC[4]) + (Pinverse[6] * theC[5])) % 256;
            thehackedkey[4] = ((Pinverse[1] * theC[3]) + (Pinverse[4] * theC[4]) + (Pinverse[7] * theC[5])) % 256;
            thehackedkey[5] = ((Pinverse[2] * theC[3]) + (Pinverse[5] * theC[4]) + (Pinverse[8] * theC[5])) % 256;

            // replacement 
            hh1.Text = thehackedkey[0].ToString();
            hh2.Text = thehackedkey[1].ToString();
            hh3.Text = thehackedkey[3].ToString();
            hh4.Text = thehackedkey[4].ToString();
            hb1.Text = thehackedkey[2].ToString();
            hb2.Text = thehackedkey[5].ToString();
            return true;
        }

        private int positiveMaker(int n)
        {
            while(n < 0)
            {
                n += 256;
            }
            return n;
        }
    }
}
