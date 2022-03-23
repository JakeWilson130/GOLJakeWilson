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

namespace GOLJakeWilson
{
    public partial class Form1 : Form
    {
        static int xAxis = 10;
        static int yAxis = 10;
        // The universe array
        bool[,] universe = new bool[xAxis, yAxis];
        bool[,] scratchPad = new bool[xAxis, yAxis];
        
        int alive;
        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;
        Color HUDColor = Color.Blue;
        Color countColor = Color.Black;
        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();
            
            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        // Calculate the next generation of cells
        
        private void NextGeneration()
        {
            for (int y = 0; y < scratchPad.GetLength(1); y++)
            {
                for (int x = 0; x < scratchPad.GetLength(0); x++)
                {
                    scratchPad[x, y] = false;
                }
            }
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int count = CountNeighborsFinite(x, y);

                    //apply rules
                    //rule 1
                    if (count < 2 && universe[x, y] == true)
                    {

                        scratchPad[x, y] = false;
                    }

                    //Rule 2
                    if (count > 3 && universe[x, y] == true)
                    {

                        scratchPad[x, y] = false;
                    }

                    //rule 3
                    if ((count == 2 || count == 3) && universe[x, y] == true)
                    {
                        scratchPad[x, y] = true;
                    }

                    //rule 4
                    if (count == 3 && universe[x, y] == false)
                    {
                        scratchPad[x, y] = true;
                    }
                    //turn it on/off on the scratchPad

                }
            }
            //copy from scratchPad to universe
            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;
            // Increment generation count
            generations++;
            CellsAlive();
            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            bool isAlive = false;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                    {
                        isAlive = true;
                    }
                }
            }
            
            //stop timer if no cells alive
            if (isAlive == false)
            {
                timer.Enabled = false;
            }
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
            graphicsPanel1.Invalidate();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            //make stuff floats
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    //RectangleF
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    int count = CountNeighborsFinite(x, y);
                    Brush neighbor = new SolidBrush(countColor);
                    e.Graphics.DrawString(count.ToString(), graphicsPanel1.Font, neighbor, cellRect);
                }
            }
            
                string gen = "Generations: " + generations;
                string size = "Universe size: " + xAxis + " x " + yAxis;
                string cells = "Cells Alive: " + alive;

                
                Brush brush = new SolidBrush(HUDColor);

                e.Graphics.DrawString(gen, graphicsPanel1.Font, brush, new PointF(0, 0));
                e.Graphics.DrawString(size, graphicsPanel1.Font, brush, new PointF(0, 10));
                e.Graphics.DrawString(cells, graphicsPanel1.Font, brush, new Point(0, 20));
            
            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }
        //close
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //next gen
        private void NextGen_Button(object sender, EventArgs e)
        {
            NextGeneration();
            graphicsPanel1.Invalidate();
        }
        //clear grid
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            graphicsPanel1.Invalidate();
        }
        private int CountNeighborsFinite(int x, int y)

        {

            int count = 0;

            int xLen = universe.GetLength(0);

            int yLen = universe.GetLength(1);

            for (int yOffset = -1; yOffset <= 1; yOffset++)

            {

                for (int xOffset = -1; xOffset <= 1; xOffset++)

                {

                    int xCheck = x + xOffset;

                    int yCheck = y + yOffset;

                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }

                    // if xCheck is less than 0 then continue
                    if (xCheck < 0)
                    {
                        continue;
                    }
                    // if yCheck is less than 0 then continue
                    if (yCheck < 0)
                    {
                        continue;
                    }

                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= xLen)
                    {
                        continue;
                    }

                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= yLen)
                    {
                        continue;
                    }


                    if (universe[xCheck, yCheck] == true) count++;

                }

            }

            return count;

        }
        //save as
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                writer.WriteLine("!This is my comment.");

                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(0); y++)
                {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;

                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(1); x++)
                    {
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.
                        if (universe[x, y] == true)
                        {
                            currentRow += 'O';
                        }
                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.
                        else 
                        {
                            currentRow += '.';
                        }
                    }

                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine(currentRow);
                }

                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }
        //open file
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();
                    

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    if (row.StartsWith("!"))
                    {
                        continue;
                    }
                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.
                    else
                    {
                        maxHeight++;
                        if (row.Length > maxWidth)
                        {
                            maxWidth = row.Length;
                        }
                    }
                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                }

                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.
                universe = new bool [maxWidth, maxHeight];
                scratchPad = new bool [maxWidth, maxHeight];
                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row.StartsWith ("!"))
                    {
                        continue;
                    }
                    int rowsRead = 0;
                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                    for (int xPos = 0; xPos < row.Length; xPos++)
                    {
                        // If row[xPos] is a 'O' (capital O) then
                        // set the corresponding cell in the universe to alive.
                        
                        
                        if (row[xPos] == 'O')
                        {
                            
                            universe[rowsRead,xPos] = true;
                            
                        }
                        // If row[xPos] is a '.' (period) then
                        // set the corresponding cell in the universe to dead.
                        if (row[xPos] == '.')
                        {
                            
                            universe[rowsRead, xPos] = false;
                            
                            
                        }
                    }
                    rowsRead++;
                }

                // Close the file.
                reader.Close();
            }
        }
        //play
        private void Play(object sender, EventArgs e)
        {
            timer.Enabled = true;
            
        }
        //pause
        private void Pause(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }
        //count alive cells
        private void CellsAlive()
        {
            alive = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                    {
                        alive++;
                    }
                }
            }
            toolStripStatusLabel2.Text = "Cells Alive = " + alive.ToString();
            graphicsPanel1.Invalidate();
        }


        //change grid x axis
        private void ChangeSizeX(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int x = Int32.Parse(toolStripTextBox1.Text);
                xAxis = x;
                universe = new bool[xAxis, yAxis];
                scratchPad = new bool[xAxis, yAxis];
            }
            NextGeneration();
        }
        //change grid y axis
        private void ChangeSizeY(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int x = Int32.Parse(toolStripTextBox2.Text);
                yAxis = x;
                universe = new bool[xAxis, yAxis];
                scratchPad = new bool[xAxis, yAxis];
            }
            NextGeneration();
        }

       //change cell color
        private void ChangeCellColor(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = cellColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color;
            }
        }
        //change background color
        private void ChangeBackgroundColor(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = graphicsPanel1.BackColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;
            }
        }
        //change grid color
        private void ChangeGridColor(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = gridColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridColor = dlg.Color;
            }
        }
        //change time of gen
        private void ControlTime(object sender, EventArgs e)
        {

        }
        //randomize universe from time
        private void RandomFromTime(object sender, EventArgs e)
        {
            Random rnd = new Random();
            
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    
                    int ran = rnd.Next(0,2);
                    if (ran == 0)
                    {
                        universe[y, x] = true;
                        
                    }
                    else
                    {
                        universe[y, x] = false;
                        
                    }
                }
            }
            graphicsPanel1.Invalidate();
        }

       
        //change hud color
        private void hUDToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = gridColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                HUDColor = dlg.Color;
            }
        }

        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = gridColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                countColor = dlg.Color;
            }
        }
    }  
}
