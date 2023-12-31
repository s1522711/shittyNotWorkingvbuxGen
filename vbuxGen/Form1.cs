using System.Net.Http.Headers;
using System.Net.Http;
using System.Windows.Forms;
using System.Text.Json.Nodes;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.VisualBasic;

namespace vbuxGen
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class Datasend
        {
            public string partnerUserId { get; set; }
        }

        public class jsonDeserializeToken
        {
            public string token { get; set; }
        }

        static class generatedLink
        {
            public static string gennedLink = "";
        }

        private async void genLink()
        {
            // prepare textboxes and labels
            saveIndicatorLabel.Visible = false;
            copyLabel.Text = "";
            OutputBox.Text = "LOADING... PLEASE WAIT!";
            OutputBox.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            OutputBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            OutputBox.ScrollBars = ScrollBars.None;
            OutputBox.BackColor = System.Drawing.SystemColors.Control;

            Random rnd = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[5];
            string finalString = "";

            for (int i = 0; i < 5; i++)
            {

                for (int j = 0; j < stringChars.Length; j++)
                {
                    stringChars[j] = chars[rnd.Next(chars.Length)];
                }

                if (i == 4)
                {
                    finalString += new string(stringChars);
                }
                else
                {
                    finalString += new string(stringChars) + "-";
                }
                
            }

            OutputBox.Text = finalString;

            // save request if checkbox is checked
            if (fileLocation.saveToFile && saveTimerToFile.Checked)
            {
                using (StreamWriter stream = File.AppendText(fileLocation.path))
                {
                    stream.WriteLine(OutputBox.Text);
                }
                saveIndicatorLabel.Text = "Saved Successfully!";
                saveIndicatorLabel.ForeColor = Color.PaleGreen;
                saveIndicatorLabel.Visible = true;
            }
        }
    


        private void GenBtn_Click(object sender, EventArgs e)
        {
            genLink();
        }



        private void copyBtn_Click(object sender, EventArgs e)
        {
            // copy button states
            if (OutputBox.Text != "\r\n\r\nPRESS THE GENERATE BUTTON TO GENERATE A KEY!" && OutputBox.Text != "LOADING... PLEASE WAIT!")
            {
                Clipboard.SetText(OutputBox.Text);
                copyLabel.ForeColor = Color.DarkGreen;
                copyLabel.Text = "COPIED SUCCESSFULLY!";
            }
            else if (OutputBox.Text == "LOADING... PLEASE WAIT!")
            {
                copyLabel.ForeColor = Color.DarkRed;
                copyLabel.Text = "PLEASE WAIT, GENERATING LINK!";
            }
            else
            {
                copyLabel.ForeColor = Color.DarkRed;
                copyLabel.Text = "PLEASE GENERATE A LINK!";
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            copyLabel.Text = "";
            OutputBox.Text = "\r\n\r\nPRESS THE GENERATE BUTTON TO GENERATE A KEY!";
        }

        private void debugCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            // debug label
            if (debugCheckbox.Checked)
            {
                timerInput.Minimum = 0.01M;
            }
            else
            {
                timerInput.Minimum = 0.5M;
            }
        }

        static class timerTime
        {
            // public var to keep track of the thats time left until next generation
            public static double timeLeft = 0.00;
            public static double setTime = 0.00; // this is the time that was set by the user, updated when the timer is enabled and on every generation, used to calculate the progressbar value without it being changed by the user
            public static int genCount = 0; // this is the amount of generations that were made
        }

        static class fileLocation
        {
            // public vars that save the full filepath to the file where to save the links and bool to keep track if the program should save links to file
            public static string path = @"NOTSET";
            public static bool saveToFile = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // run every 100 milliseconds
            // if timeleft is higher than 0 decrease it by 0.01 and update label
            if (timerTime.timeLeft > 0.00)
            {
                timerTime.timeLeft = timerTime.timeLeft - 0.01;
                infoTimerLabel.Invoke(new Action(() => infoTimerLabel.Text = string.Format("time left until next generation: {0:F2}", timerTime.timeLeft)));
                //double progress = ((double)timerInput.Value - timerTime.timeLeft) / (double)timerInput.Value * 1000;
                try
                {
                    timerProgressBar.Value = (int)((timerTime.setTime - timerTime.timeLeft) / timerTime.setTime * 1000) + 1;
                    timerProgressBar.Value -= 2;
                    timerProgressBarSmall.Value = (int)((timerTime.setTime - timerTime.timeLeft) / timerTime.setTime * 1000) + 1;
                    timerProgressBarSmall.Value -= 1;
                }
                catch (System.ArgumentOutOfRangeException)
                {
                    timerProgressBar.Value = 100;
                }
            }
            else
            {
                // else generate new link and reset timeleft
                genLink();
                timerTime.timeLeft = (double)timerInput.Value;
                timerTime.setTime = (double)timerInput.Value;
                timerProgressBar.Value = 0;
                timerProgressBarSmall.Value = 0;
            }
        }

        private void timerBox_CheckedChanged(object sender, EventArgs e)
        {
            // enable and disable the timer
            if (timerBox.Checked)
            {
                timerTime.timeLeft = (double)timerInput.Value;
                timerTime.setTime = (double)timerInput.Value;
                GenBtn.Enabled = false;
                timer1.Enabled = true;
                infoTimerLabel.Text = string.Format("time left until next generation: {0:F2}", timerTime.timeLeft);
                infoTimerLabel.Visible = true;
                timerProgressBar.Visible = true;
                timerProgressBarSmall.Visible = true;
                timer1.Start();
            }
            else
            {
                timer1.Stop();
                timerTime.timeLeft = 0.00;
                infoTimerLabel.Text = "time left until next generation: 0.00";
                GenBtn.Enabled = true;
                timer1.Enabled = false;
                infoTimerLabel.Visible = false;
                timerProgressBar.Visible = false;
                timerProgressBarSmall.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // when file is selected via dialog save it to variable and auto-tick the saving checkbox
            fileLocation.path = saveFileDialog1.FileName;
            saveTimerToFile.Checked = true;
            selectedFileLabel.Text = Path.GetFileName(fileLocation.path);
            // create the file if it doesnt exists
            if (!File.Exists(fileLocation.path))
            {
                File.Create(fileLocation.path).Close();
            }
            
        }

        private void saveTimerToFile_CheckedChanged(object sender, EventArgs e)
        {
            // if a file was selected and the checkbox it ticked set saveToFile to true
            if (saveTimerToFile.Checked && fileLocation.path != "NOTSET")
            {
                fileLocation.saveToFile = true;
            }
            // else if a file wasnt selected and the checkbox is ticked untick it and show error msgbox
            else if (saveTimerToFile.Checked && fileLocation.path == "NOTSET")
            {
                saveTimerToFile.Checked = false;
                fileLocation.saveToFile = false;
                MessageBox.Show("Please select a file to save to!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                fileLocation.saveToFile = false;
            }
        }
    }
}
