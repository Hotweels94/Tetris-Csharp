﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris_C_.Shapes
{
    internal class Shape
    {
        public int Width;
        public int Height;
        public int[,] Dots;
        public Brush Color;


        private int[,] backupDots;




        public void turn()
        {
            backupDots = Dots;

            Dots = new int[Width, Height];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Dots[i, j] = backupDots[Height - 1 - j, i];
                }
            }

            var temp = Width;
            Width = Height;
            Height = temp;
        }

        public void rollback()
        {
            Dots = backupDots;
            var temp = Width;
            Width = Height;
            Height = temp;
        }
    }
}
