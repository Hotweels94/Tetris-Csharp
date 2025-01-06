using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tetris_C_.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Tetris_C_
{
    public partial class Form1 : Form
    {
        Shape currentShape;
        Shape nextShape;
        Timer timer = new Timer();
        Shape holdShape = null;
        public Form1()
        {
            InitializeComponent();

            loadCanvas();

            currentShape = getRandomShapeWithCenterAligned();
            nextShape = getNextShape();
            holdShape = getHoldShape();

            timer.Tick += Timer_Tick;
            timer.Interval = 500;
            timer.Start();

            this.KeyDown += Form1_KeyDown;
        }

        Bitmap canvasBitmap;
        Graphics canvasGraphics;
        int canvasWidth = 15;
        int canvasHeight = 20;
        Brush[,] canvasDotArray;

        int dotSize = 20;
        private void loadCanvas()
        {
            pictureBox1.Width = canvasWidth * dotSize;
            pictureBox1.Height = canvasHeight * dotSize;

            canvasBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            canvasGraphics = Graphics.FromImage(canvasBitmap);
            canvasGraphics.FillRectangle(Brushes.LightGray, 0, 0, canvasBitmap.Width, canvasBitmap.Height);
            pictureBox1.Image = canvasBitmap;

            canvasDotArray = new Brush[canvasWidth, canvasHeight];
            for (int i = 0; i < canvasWidth; i++)
            {
                for (int j = 0; j < canvasHeight; j++)
                {
                    canvasDotArray[i, j] = Brushes.LightGray;
                }
            }
        }


        int currentX;
        int currentY;
        private Shape getRandomShapeWithCenterAligned()
        {
            var shape = ShapesHandler.GetRandomShape();

            // Calculate the x and y values as if the shape lies in the center
            currentX = 7;
            currentY = -shape.Height;

            return shape;
        }

        Bitmap workingBitmap;
        Graphics workingGraphics;
        private void Timer_Tick(object sender, EventArgs e)
        {
            var isMoveSuccess = moveShapeIfPossible(moveDown: 1);

            // if shape reached bottom or touched any other shapes
            if (!isMoveSuccess)
            {
                // copy working image into canvas image
                canvasBitmap = new Bitmap(workingBitmap);

                updateCanvasDotArrayWithCurrentShape();

                // get next shape
                currentShape = nextShape;
                nextShape = getNextShape();

                clearFilledRowsAndUpdateScore();
            }
        }

        private void updateCanvasDotArrayWithCurrentShape()
        {
            for (int i = 0; i < currentShape.Width; i++)
            {
                for (int j = 0; j < currentShape.Height; j++)
                {
                    if (currentShape.Dots[j, i] == 1)
                    {
                        checkIfGameOver();

                        // Save the color of the block
                        canvasDotArray[currentX + i, currentY + j] = currentShape.Color;
                    }
                }
            }
        }


        private void checkIfGameOver()
        {
            if (currentY < 0)
            {
                timer.Stop();
                MessageBox.Show("Game Over");
                Application.Restart();
            }
        }

        // returns if it reaches the bottom or touches any other blocks
        private bool moveShapeIfPossible(int moveDown = 0, int moveSide = 0)
        {
            var newX = currentX + moveSide;
            var newY = currentY + moveDown;

            //Verify is the shape is out of the grid
            if (newX < 0 || newX + currentShape.Width > canvasWidth
                || newY + currentShape.Height > canvasHeight)
                return false;

            //Verify if the sahpe touch other shape
            for (int i = 0; i < currentShape.Width; i++)
            {
                for (int j = 0; j < currentShape.Height; j++)
                {
                    if (currentShape.Dots[j, i] == 1) //IF this dot is appart of the shape
                    {
                        int targetX = newX + i;
                        int targetY = newY + j;

                        //Verify that the space is blank
                        if (targetY >= 0 && canvasDotArray[targetX, targetY] != Brushes.LightGray)
                        {
                            return false;
                        }
                    }
                }
            }


            currentX = newX;
            currentY = newY;

            drawShape();

            return true;
        }


        private void drawShape()
        {
            workingBitmap = new Bitmap(canvasBitmap);
            workingGraphics = Graphics.FromImage(workingBitmap);

            for (int i = 0; i < currentShape.Width; i++)
            {
                for (int j = 0; j < currentShape.Height; j++)
                {
                    if (currentShape.Dots[j, i] == 1)
                        workingGraphics.FillRectangle(currentShape.Color, (currentX + i) * dotSize, (currentY + j) * dotSize, dotSize, dotSize);
                }
            }

            pictureBox1.Image = workingBitmap;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            var verticalMove = 0;
            var horizontalMove = 0;

            // calculate the vertical and horizontal move values
            // based on the key pressed
            switch (e.KeyCode)
            {
                // move shape left
                case Keys.Left:
                    verticalMove--;
                    break;

                // move shape right
                case Keys.Right:
                    verticalMove++;
                    break;

                // move shape down faster
                case Keys.Down:
                    horizontalMove++;
                    break;

                // rotate the shape clockwise
                case Keys.Up:
                    currentShape.turn();
                    break;

                // Hold a piece for later
                case Keys.C:
                    if (holdShape == null)
                    {
                        holdShape = currentShape;
                        currentShape = nextShape;
                        nextShape = getNextShape();
                    }
                    else
                    {
                        Shape temp = currentShape;
                        currentShape = holdShape;
                        holdShape = temp; ;
                    }
                    getHoldShape();
                    break;

                default:
                    return;
            }

            var isMoveSuccess = moveShapeIfPossible(horizontalMove, verticalMove);

            // if the player was trying to rotate the shape, but
            // that move was not possible - rollback the shape
            if (!isMoveSuccess && e.KeyCode == Keys.Up)
                currentShape.rollback();
        }

        int score;
        public void clearFilledRowsAndUpdateScore()
        {
            for (int i = canvasHeight - 1; i >= 0; i--)
            {
                bool isRowFull = true;

                // Verify is a line is completed
                for (int j = 0; j < canvasWidth; j++)
                {
                    if (canvasDotArray[j, i] == Brushes.LightGray)
                    {
                        isRowFull = false;
                        break;
                    }
                }

                //Delet a complet line
                if (isRowFull)
                {
                    score++;
                    label1.Text = "Score: " + score;
                    label2.Text = "Level: " + (score / 10);

                    timer.Interval = Math.Max(50, timer.Interval - 10);

                    // Make all the above lines down from 1 block
                    for (int k = i; k > 0; k--)
                    {
                        for (int j = 0; j < canvasWidth; j++)
                        {
                            canvasDotArray[j, k] = canvasDotArray[j, k - 1];
                        }
                    }

                    // Errase the first line
                    for (int j = 0; j < canvasWidth; j++)
                    {
                        canvasDotArray[j, 0] = Brushes.LightGray;
                    }

                    i++; // Verify the if the grid after the clear
                }
            }

            redrawCanvas();
        }


        private void redrawCanvas()
        {
            canvasGraphics = Graphics.FromImage(canvasBitmap);

            //Clear the canvas
            canvasGraphics.FillRectangle(Brushes.LightGray, 0, 0, canvasBitmap.Width, canvasBitmap.Height);

            // Draw each bloc of each sahpe with his color
            for (int i = 0; i < canvasWidth; i++)
            {
                for (int j = 0; j < canvasHeight; j++)
                {
                    if (canvasDotArray[i, j] != Brushes.LightGray) // If there is a block
                    {
                        canvasGraphics.FillRectangle(canvasDotArray[i, j], i * dotSize, j * dotSize, dotSize, dotSize);
                    }
                }
            }

            pictureBox1.Image = canvasBitmap;
        }



        Bitmap nextShapeBitmap;
        Graphics nextShapeGraphics;
        private Shape getNextShape()
        {
            var shape = getRandomShapeWithCenterAligned();

            // Codes to show the next shape in the side panel
            nextShapeBitmap = new Bitmap(6 * dotSize, 6 * dotSize);
            nextShapeGraphics = Graphics.FromImage(nextShapeBitmap);

            nextShapeGraphics.FillRectangle(Brushes.LightGray, 0, 0, nextShapeBitmap.Width, nextShapeBitmap.Height);

            // Find the ideal position for the shape in the side panel
            var startX = (6 - shape.Width) / 2;
            var startY = (6 - shape.Height) / 2;

            for (int i = 0; i < shape.Height; i++)
            {
                for (int j = 0; j < shape.Width; j++)
                {
                    nextShapeGraphics.FillRectangle(
                        shape.Dots[i, j] == 1 ? shape.Color : Brushes.LightGray,
                        (startX + j) * dotSize, (startY + i) * dotSize, dotSize, dotSize);
                }
            }

            pictureBox2.Size = nextShapeBitmap.Size;
            pictureBox2.Image = nextShapeBitmap;

            return shape;
        }

        Bitmap holdShapeBitmap;
        Graphics holdShapeGraphics;
        private Shape getHoldShape()
        {
            var shape = holdShape;

            // Codes to show the hold shape in the side panel
            holdShapeBitmap = new Bitmap(6 * dotSize, 6 * dotSize);
            holdShapeGraphics = Graphics.FromImage(holdShapeBitmap);

            holdShapeGraphics.FillRectangle(Brushes.LightGray, 0, 0, holdShapeBitmap.Width, holdShapeBitmap.Height);

            // Find the ideal position for the shape in the side panel
            if (shape != null)
            {
                var startX = (6 - shape.Width) / 2;
                var startY = (6 - shape.Height) / 2;

                for (int i = 0; i < shape.Height; i++)
                {
                    for (int j = 0; j < shape.Width; j++)
                    {
                        holdShapeGraphics.FillRectangle(
                            shape.Dots[i, j] == 1 ? shape.Color : Brushes.LightGray,
                            (startX + j) * dotSize, (startY + i) * dotSize, dotSize, dotSize);
                    }
                }

                pictureBox3.Size = holdShapeBitmap.Size;
                pictureBox3.Image = holdShapeBitmap;
            }
            return shape;
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}