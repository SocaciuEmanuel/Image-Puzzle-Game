using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class frmPuzzleGame : Form
    {
        int nullSliceIndex, moves = 0; // Index of the empty slice and move counter
        List<Bitmap> lstOriginalPictureList = new List<Bitmap>(); // List to hold the original images
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch(); // Timer to track elapsed time

        public frmPuzzleGame()
        {
            InitializeComponent();
            // Adding images to the list (including the null slice)
            lstOriginalPictureList.AddRange(new Bitmap[] { Properties.Resources._1, Properties.Resources._2, Properties.Resources._3, Properties.Resources._4, Properties.Resources._5, Properties.Resources._6, Properties.Resources._7, Properties.Resources._8, Properties.Resources._9, Properties.Resources._null });
            lblMovesMade.Text += moves; // Initialize move counter display
            lblTimeElapsed.Text = "00:00:00"; // Initialize timer display
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Shuffle(); // Shuffle the puzzle on form load
        }

        void Shuffle()
        {
            do
            {
                int j;
                List<int> Indexes = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 9 }); // Indices for shuffling
                Random r = new Random();
                for (int i = 0; i < 9; i++)
                {
                    // Randomly select an index and set the image for each PictureBox
                    Indexes.Remove((j = Indexes[r.Next(0, Indexes.Count)]));
                    ((PictureBox)gbPuzzleBox.Controls[i]).Image = lstOriginalPictureList[j];
                    if (j == 9) nullSliceIndex = i; // Store the index of the empty slice
                }
            } while (CheckWin()); // Ensure the puzzle is not already solved after shuffling
        }

        private void btnShuffle_Click(object sender, EventArgs e)
        {
            DialogResult YesOrNo = new DialogResult();
            if (lblTimeElapsed.Text != "00:00:00")
            {
                YesOrNo = MessageBox.Show("Are you sure you want to restart?", "Puzzle", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            if (YesOrNo == DialogResult.Yes || lblTimeElapsed.Text == "00:00:00")
            {
                Shuffle(); // Shuffle the puzzle
                timer.Reset(); // Reset the timer
                lblTimeElapsed.Text = "00:00:00"; // Reset timer display
                moves = 0; // Reset move counter
                lblMovesMade.Text = "Moves Made : 0"; // Update move counter display
            }
        }

        private void AskPermissionBeforeQuite(object sender, FormClosingEventArgs e)
        {
            DialogResult YesOrNO = MessageBox.Show("Are you sure you want to quit ?", "Puzzle", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (sender as Button != btnQuit && YesOrNO == DialogResult.No) e.Cancel = true; // Cancel closing if user says no
            if (sender as Button == btnQuit && YesOrNO == DialogResult.Yes) Environment.Exit(0); // Exit if user confirms quit
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            AskPermissionBeforeQuite(sender, e as FormClosingEventArgs); // Ask for confirmation before quitting
        }

        private void SwitchPictureBox(object sender, EventArgs e)
        {
            if (lblTimeElapsed.Text == "00:00:00")
                timer.Start(); // Start timer if not already started
            int inPictureBoxIndex = gbPuzzleBox.Controls.IndexOf(sender as Control);
            if (nullSliceIndex != inPictureBoxIndex)
            {
                // Calculate the indices of neighboring slices
                List<int> FourBrothers = new List<int>(new int[] { ((inPictureBoxIndex % 3 == 0) ? -1 : inPictureBoxIndex - 1), inPictureBoxIndex - 3, (inPictureBoxIndex % 3 == 2) ? -1 : inPictureBoxIndex + 1, inPictureBoxIndex + 3 });
                if (FourBrothers.Contains(nullSliceIndex))
                {
                    // Swap images between clicked slice and empty slice
                    ((PictureBox)gbPuzzleBox.Controls[nullSliceIndex]).Image = ((PictureBox)gbPuzzleBox.Controls[inPictureBoxIndex]).Image;
                    ((PictureBox)gbPuzzleBox.Controls[inPictureBoxIndex]).Image = lstOriginalPictureList[9];
                    nullSliceIndex = inPictureBoxIndex; // Update empty slice index
                    lblMovesMade.Text = "Moves Made : " + (++moves); // Increment and update move counter
                    if (CheckWin()) // Check if puzzle is solved
                    {
                        timer.Stop(); // Stop the timer
                        (gbPuzzleBox.Controls[8] as PictureBox).Image = lstOriginalPictureList[8]; // Show the last piece
                        MessageBox.Show("Congratulations!\nTime Elapsed : " + timer.Elapsed.ToString().Remove(8) + "\nTotal Moves Made : " + moves, "Puzzle"); // Congratulate the user
                        moves = 0; // Reset move counter
                        lblMovesMade.Text = "Moves Made : 0"; // Update move counter display
                        lblTimeElapsed.Text = "00:00:00"; // Reset timer display
                        timer.Reset(); // Reset timer
                        Shuffle(); // Shuffle the puzzle for a new game
                    }
                }
            }
        }

        bool CheckWin()
        {
            int i;
            for (i = 0; i < 8; i++)
            {
                // Check if each PictureBox has the correct image
                if ((gbPuzzleBox.Controls[i] as PictureBox).Image != lstOriginalPictureList[i]) break;
            }
            if (i == 8) return true; // If all pieces are in place, return true
            else return false; // Otherwise, return false
        }

        private void UpdateTimeElapsed(object sender, EventArgs e)
        {
            if (timer.Elapsed.ToString() != "00:00:00")
                lblTimeElapsed.Text = timer.Elapsed.ToString().Remove(8); // Update the timer display
            if (timer.Elapsed.ToString() == "00:00:00")
                btnPause.Enabled = false; // Disable pause button if timer is at zero
            else
                btnPause.Enabled = true; // Enable pause button otherwise
            if (timer.Elapsed.Minutes.ToString() == "2")
            {
                timer.Reset(); // Reset timer if one minute has passed
                lblMovesMade.Text = "Moves Made : 0"; // Reset move counter display
                lblTimeElapsed.Text = "00:00:00"; // Reset timer display
                moves = 0; // Reset move counter
                btnPause.Enabled = false; // Disable pause button
                MessageBox.Show("Time is up\nTry again", "Puzzle"); // Notify user that time is up
                Shuffle(); // Shuffle the puzzle for a new game
            }
        }

        private void lblTimeElapsed_Click(object sender, EventArgs e)
        {

        }

        private void PauseOrResume(object sender, EventArgs e)
        {
            if (btnPause.Text == "Pause")
            {
                timer.Stop(); // Stop the timer
                gbPuzzleBox.Visible = false; // Hide the puzzle
                btnPause.Text = "Resume"; // Change button text to "Resume"
            }
            else
            {
                timer.Start(); // Start the timer
                gbPuzzleBox.Visible = true; // Show the puzzle
                btnPause.Text = "Pause"; // Change button text to "Pause"
            }
        }
    }
}
