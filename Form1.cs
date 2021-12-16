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

namespace Game_Of_Life_Program_COS119 {
    public partial class Form1 : Form {

        int universeSize = 25;

        // The universe array
        bool[,] universe;

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Red;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        bool isShowingGrid = true;
        bool isShowingNums = true;

        public Form1() {
            InitializeComponent();
            universe = new bool[universeSize, universeSize];
            // Setup the timer
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
            toolStripStatusLabelGenerations.Text = "Generation: " + generations.ToString() +
                "    |    Cells Alive: " + CountTotalAliveCells().ToString() +
                "    |    Universe Size: " + (this.universeSize * this.universeSize);
            // timer.Enabled = true; // start timer running
        }

        public void changeCellColor(String newColor) {
            switch (newColor.ToUpper()) {
                case "RED":
                    cellColor = Color.Red;
                    break;
                case "BLUE":
                    cellColor = Color.Blue;
                    break;
                case "GREEN":
                    cellColor = Color.Green;
                    break;
                case "BLACK":
                    cellColor = Color.Black;
                    break;
                case "PURPLE":
                    cellColor = Color.Purple;
                    break;
            }
            graphicsPanel1.Invalidate();
        }

        public void start() {
            timer.Enabled = true;
        }

        public void pause() {
            timer.Enabled = false;
        }

        public void next() {
            NextGeneration();
        }

        public void clear() {
            pause();
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generation: " + generations.ToString() +
                "    |    Cells Alive: " + CountTotalAliveCells().ToString() + 
                "    |    Universe Size: " + (this.universeSize * this.universeSize);
            universe = new bool[universeSize, universeSize];
            graphicsPanel1.Invalidate();
        }

        public void randomize() {
            clear();
            Random rand = new Random();
            for (int i = 0; i < universe.GetLength(1); i++) {
                for (int x = 0; x < universe.GetLength(0); x++) {
                    bool currCellState = false;
                    if (rand.NextDouble() >= 0.5) {
                        currCellState = true;
                    } else {
                        currCellState = false;
                    }
                    universe[i, x] = currCellState;
                }
            }
            toolStripStatusLabelGenerations.Text = "Generation: " + generations.ToString() +
                "    |    Cells Alive: " + CountTotalAliveCells().ToString() +
                "    |    Universe Size: " + (this.universeSize * this.universeSize);
            graphicsPanel1.Invalidate();
        }

        // Calculate the next generation of cells
        private void NextGeneration() {
            // Increment generation count
            generations++;
            // Update status strip generations
            //toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            bool[,] tempUniverse = universe;
            // Generations Cell Logic
            for (int y = 0; y < universe.GetLength(1); y++) {
                for (int x = 0; x < universe.GetLength(0); x++) {
                    int numberOfNeighbors = CountAliveNeighbors(x, y);
                    if (numberOfNeighbors == 2) {
                        tempUniverse[x, y] = universe[x, y];
                    } else 
                    if (numberOfNeighbors == 3) {
                        tempUniverse[x, y] = true;
                    } else {
                        tempUniverse[x, y] = false;
                    }
                }
            }
            for (int y = 0; y < universe.GetLength(1); y++) {
                for (int x = 0; x < universe.GetLength(0); x++) {
                    universe[x, y] = tempUniverse[x, y];
                }
            }
            toolStripStatusLabelGenerations.Text = "Generation: " + generations.ToString() +
                "    |    Cells Alive: " + CountTotalAliveCells().ToString() +
                "    |    Universe Size: " + (this.universeSize * this.universeSize); 
            graphicsPanel1.Invalidate();
        }

        public int CountTotalAliveCells() {
            int numAlive = 0;
            for (int y = 0; y < universe.GetLength(1); y++) {
                for (int x = 0; x < universe.GetLength(0); x++) {
                    bool currCell = universe[x, y];
                    if (currCell == true) numAlive++;
                }
            }
            return numAlive;
         }

        public int CountAliveNeighbors(int x, int y) {
            int numberOfNeighbors = 0;

            if (x-1 >= 0) {
                bool neighbor = universe[x - 1, y];
                if (neighbor == true) numberOfNeighbors++;
            }

            if (x + 1 < universe.GetLength(0)) {
                bool neighbor = universe[x + 1, y];
                if (neighbor == true) numberOfNeighbors++;
            }

            if (y - 1 >= 0) {
                bool neighbor = universe[x, y - 1];
                if (neighbor == true) numberOfNeighbors++;
            }

            if (y + 1 < universe.GetLength(1)) {
                bool neighbor = universe[x, y + 1];
                if (neighbor == true) numberOfNeighbors++;
            }

            // Diagonal

            if (y - 1 >= 0 && x - 1 >= 0) {
                bool neighbor = universe[x - 1, y - 1];
                if (neighbor == true) numberOfNeighbors++;
            }

            if (y - 1 >= 0 && x + 1 < universe.GetLength(0)) {
                bool neighbor = universe[x + 1, y - 1];
                if (neighbor == true) numberOfNeighbors++;
            }

            if (y + 1 < universe.GetLength(1) && x - 1 >= 0) {
                bool neighbor = universe[x - 1, y + 1];
                if (neighbor == true) numberOfNeighbors++;
            }

            if (y + 1 < universe.GetLength(1) && x + 1 < universe.GetLength(0)) {
                bool neighbor = universe[x + 1, y + 1];
                if (neighbor == true) numberOfNeighbors++;
            }

            return numberOfNeighbors;
        }

        public void saveUniverse() {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                StreamWriter writer = new StreamWriter(saveFileDialog1.OpenFile());
                string output = "";
                for (int y = 0; y < universe.GetLength(1); y++) {
                    for (int x = 0; x < universe.GetLength(0); x++) {
                        int newChar = 0;
                        if (this.universe[x, y] == true) newChar = 1;
                        output += newChar + " ";
                    }
                    Console.WriteLine("Saving To File: \n" + output);
                    writer.WriteLine(output);
                    output = "";
                }
                Console.WriteLine("File written!");
                writer.Dispose();
                writer.Close();
            }
        }

        public void loadUniverse() {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                try {
                    string[] lines = System.IO.File.ReadAllLines(openFileDialog1.FileName);
                    universeSize = lines.Length;
                    universe = new bool[universeSize, universeSize];

                    for (int y = 0; y < lines.Length; y++) {
                        Console.WriteLine("Line " + (y + 1) + ": " + lines[y]);
                        string[] colmns = lines[y].Split(' ');

                        for (int x = 0; x < colmns.Length; x++) {
                            string currChar = colmns[x];
                            Console.WriteLine("currChar (" + x + ", " + y + ") : " + currChar);

                            if (x < universeSize) {
                                if (currChar == "1") {
                                    universe[x, y] = true;
                                } else {
                                    universe[x, y] = false;
                                }
                            }

                        }

                    }
                    toolStripStatusLabelGenerations.Text = "Generation: " + generations.ToString() +
                        "    |    Cells Alive: " + CountTotalAliveCells().ToString() +
                        "    |    Universe Size: " + (this.universeSize * this.universeSize);
                    graphicsPanel1.Invalidate();
                } catch {
                    MessageBox.Show("ERROR! Please check passphrase and do not attempt to edit cipher text");
                }
            }
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e) {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e) {
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
            for (int y = 0; y < universe.GetLength(1); y++) {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++) {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;
                    int fontSize = cellHeight / 3;
                    if (fontSize == 0) fontSize = 1;
                    Font font = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Regular);

                    int neighborCount = CountAliveNeighbors(x, y);

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true) {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        if (isShowingNums == true) e.Graphics.DrawString(neighborCount.ToString(), font, Brushes.White, cellRect, sf);
                    } else {
                        if (isShowingNums == true) e.Graphics.DrawString(neighborCount.ToString(), font, Brushes.Black, cellRect, sf);
                    }

                    // Outline the cell with a pen
                    if (isShowingGrid == true) e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e) {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left) {
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

        public void changeSize() {
            this.pause();
            int newSize = ShowDialogInt(
                "Enter the game board size: ", 
                "Change Game Size", universeSize);
            universeSize = newSize;
            this.clear();
        }

        public void changeSpeed() {
            this.pause();
            int newSpeed = ShowDialogInt(
                "Enter the amount of delay you want between generations below (in miliseconds):",
                "Change Game Speed", timer.Interval);
            timer.Interval = newSpeed;
        }

        public static int ShowDialogInt(string text, string caption, int initalValue) {
            Form prompt = new Form();
            prompt.Width = 500;
            prompt.Height = 200;
            prompt.Text = caption;
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            NumericUpDown inputBox = new NumericUpDown() { Left = 50, Top = 50, Width = 400 };
            inputBox.Value = initalValue;
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70 };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.ShowDialog();
            return (int)inputBox.Value;
        }
    }
}
