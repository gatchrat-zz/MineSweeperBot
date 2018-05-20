using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace MineSweeperBot
{  
    public partial class Form1 : Form
    {

        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);
        [DllImport("user32.dll",CharSet = CharSet.Auto, CallingConvention= CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        /**
         *https://stackoverflow.com/questions/1483928/how-to-read-the-color-of-a-screen-pixel answer from Bitterblue
         */
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr window);
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern uint GetPixel(IntPtr dc, int x, int y);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ReleaseDC(IntPtr window, IntPtr dc);
        public static Color GetColorAt(int x, int y)
        {
            IntPtr desk = GetDesktopWindow();
            IntPtr dc = GetWindowDC(desk);
            int a = (int)GetPixel(dc, x, y);
            ReleaseDC(desk, dc);
            return Color.FromArgb(255, (a >> 0) & 0xff, (a >> 8) & 0xff, (a >> 16) & 0xff);
        }
        int sizeX = 9;
        int[] field = new int[9 * 9];


        public Form1()
        {
            InitializeComponent();
        }
        private int[] readField(int[] field)
        {
            // 1  = y, x = 
            //Closed = 87,131,228 = 8
            //Flag = 135,146,180 = 9
            // Empty =192,203,225 = 0
            // 1 =63,80,186
            // 2 = 23,107,0
            // 3 = 174,4,4
            // 4 = 0,3,126
            int sizeX = 9;
            int sizeY = 9;
            
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {

                    int r, g, b;
                    Color c = GetColorAt(598 + i * (81) + 51, 178 + j * (81) + 62);
                    r = c.R;
                    g = c.G;
                    b = c.B;
                    if (field[j * sizeX + i] == 9)
                    {
                        //bomb
                    }
                    else if (r < 70 && r > 55 && g > 70 && g < 90 && b > 175 && b < 195)
                    {
                        //1
                        field[j * sizeX + i] = 1;
                    }
                    else if (r > 15 && r < 30)
                    {
                        //2
                        field[j * sizeX + i] = 2;
                    }
                    else if (r > 160 && r < 180 && g < 15)
                    {
                        //3
                        field[j * sizeX + i] = 3;
                    }
                    else if (r < 10)
                    {
                        //4
                        field[j * sizeX + i] = 4;
                    }
                    else if (g > 150 && r > 150 && b > 150)
                    {
                        field[j * sizeX + i] = 0;
                        //empty
                    }
                    else
                    {
                        field[j * sizeX + i] = 8;
                        //unkown
                    }

                }
            }
           
            return field;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            int totalfound = 0;

            while (totalfound < 10)
            {
                Thread.Sleep(100);
                field = readField(field);
                bool progress = false;
                //CHeck for known Bombs
                for (int i = 0; i < sizeX * sizeX; i++)
                {
                    if (field[i] > 4)
                    {

                    }
                    else
                    {


                        //count
                        int[] free = new int[8];
                        int[] bombs = new int[8];
                        int bombCount = 0;
                        int freeCount = 0;
                        int currrent = field[i];
                        //left
                        if ((i + 1) % sizeX != 1)
                        {
                            if (field[i - 1] == 9)
                            {
                                bombs[bombCount] = i - 1;
                                bombCount++;
                            }
                            if (field[i - 1] == 8)
                            {
                                free[freeCount] = i - 1;
                                freeCount++;
                            }
                        }
                        //right
                        if ((i + 1) % sizeX != 0)
                        {
                            if (field[i + 1] == 9)
                            {
                                bombs[bombCount] = i + 1;
                                bombCount++;
                            }
                            if (field[i + 1] == 8)
                            {
                                free[freeCount] = i + 1;
                                freeCount++;
                            }
                        }
                        //Top
                        if (i >= sizeX)
                        {
                            if (field[i - sizeX] == 9)
                            {
                                bombs[bombCount] = i - sizeX;
                                bombCount++;
                            }
                            if (field[i - sizeX] == 8)
                            {
                                free[freeCount] = i - sizeX;
                                freeCount++;
                            }
                        }
                        //Down
                        if (i < sizeX * (sizeX - 1))
                        {
                            if (field[i + sizeX] == 9)
                            {
                                bombs[bombCount] = i + sizeX;
                                bombCount++;
                            }
                            if (field[i + sizeX] == 8)
                            {
                                free[freeCount] = i + sizeX;
                                freeCount++;
                            }
                        }
                        //TopRight
                        if ((i + 1) % sizeX != 0 && i >= sizeX)
                        {
                            if (field[i + 1 - sizeX] == 9)
                            {
                                bombs[bombCount] = i + 1 - sizeX;
                                bombCount++;
                            }
                            if (field[i + 1 - sizeX] == 8)
                            {
                                free[freeCount] = i + 1 - sizeX;
                                freeCount++;
                            }
                        }
                        //DownRight
                        if ((i + 1) % sizeX != 0 && i < sizeX * (sizeX - 1))
                        {
                            if (field[i + 1 + sizeX] == 9)
                            {
                                bombs[bombCount] = i + 1 + sizeX;
                                bombCount++;
                            }
                            if (field[i + 1 + sizeX] == 8)
                            {
                                free[freeCount] = i + 1 + sizeX;
                                freeCount++;
                            }
                        }
                        //TopLeft
                        if ((i + 1) % sizeX != 1 && i >= sizeX)
                        {
                            if (field[i - 1 - sizeX] == 9)
                            {
                                bombs[bombCount] = i - 1 - sizeX;
                                bombCount++;
                            }
                            if (field[i - 1 - sizeX] == 8)
                            {
                                free[freeCount] = i - 1 - sizeX;
                                freeCount++;
                            }
                        }
                        //DownLeft
                        if ((i + 1) % sizeX != 1 && i < sizeX * (sizeX - 1))
                        {
                            if (field[i - 1 + sizeX] == 9)
                            {
                                bombs[bombCount] = i - 1 + sizeX;
                                bombCount++;
                            }
                            if (field[i - 1 + sizeX] == 8)
                            {
                                free[freeCount] = i - 1 + sizeX;
                                freeCount++;
                            }
                        }
                        //Click all fields
                        if (field[i] - bombCount == 0 && freeCount > 0)
                        {
                            for (int k = 0; k < freeCount; k++)
                            {
                                progress = true;
                                //click 
                                int x = free[k] % sizeX;
                                int y = (free[k] - ((free[k]) % sizeX)) / sizeX;
                                Console.WriteLine("Free Detected");
                                SetCursorPos(598 + x * (81) + 51, 178 + y * (81) + 62);
                                Thread.Sleep(200);
                                mouse_event(0x0002, 0, 0, 0, 0);
                                mouse_event(0x0004, 0, 0, 0, 0);
                                mouse_event(0x0002, 0, 0, 0, 0);
                                mouse_event(0x0004, 0, 0, 0, 0);
                            }
                            i = 999999;
                        }
                        //Mark all fields
                        else if (field[i] - bombCount == freeCount && freeCount > 0)
                        {
                            for (int k = 0; k < freeCount; k++)
                            {
                                progress = true;
                                //click 
                                Console.WriteLine("Bomb Detected");
                                field[free[k]] = 9;
                                totalfound++;
                            }
                            i = 999999;
                        }
                    }

                }
                if (!progress)
                {
                    for (int i = 0; i < sizeX * sizeX; i++)
                    {
                        if (field[i] == 8)
                        {
                            int x = i % sizeX;
                            int y = (i - ((i) % sizeX)) / sizeX;
                            Console.WriteLine("X: " + x + " Y: " + y);
                            SetCursorPos(598 + x * (81) + 51, 178 + y * (81) + 62);
                            Thread.Sleep(200);
                            mouse_event(0x0002, 0, 0, 0, 0);
                            mouse_event(0x0004, 0, 0, 0, 0);
                            i = 999999;
                        }
                    }
                }
                Console.WriteLine("---------------------");
                for (int x = 0; x < sizeX; x++)
                {
                    Console.WriteLine(field[x * sizeX] + " " + field[x * sizeX + 1] + " " + field[x * sizeX + 2] + " " + field[x * sizeX + 3] + " " + field[x * sizeX + 4] + " " + field[x * sizeX + 5] + " " + field[x * sizeX + 6] + " " + field[x * sizeX + 7] + " " + field[x * sizeX + 8]);
                }

            }
            //Mark all Found
            for (int i = 0; i < sizeX * sizeX; i++)
            {
                if (field[i] == 9)
                {

                    int x = i % sizeX;
                    int y = (i - ((i) % sizeX)) / sizeX;
                    Console.WriteLine("X: " + x + " Y: " + y);
                    SetCursorPos(598 + x * (81) + 51, 178 + y * (81) + 62);
                    Thread.Sleep(200);
                    mouse_event(0x0008, 0, 0, 0, 0);
                    mouse_event(0x00010, 0, 0, 0, 0);
                }
                if (field[i] == 8)
                {

                    int x = i % sizeX;
                    int y = (i - ((i) % sizeX)) / sizeX;
                    Console.WriteLine("X: " + x + " Y: " + y);
                    SetCursorPos(598 + x * (81) + 51, 178 + y * (81) + 62);
                    Thread.Sleep(200);
                    mouse_event(0x0002, 0, 0, 0, 0);
                    mouse_event(0x0004, 0, 0, 0, 0);
                }
            }
        }
    }
}
