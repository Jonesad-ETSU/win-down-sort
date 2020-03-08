using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManagerTaskTray
{
    public partial class Wndw_Abt : Form
    {
        public Wndw_Abt()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Window_Abt_Title.Text = "BrAD's Download Manager";
            this.Window_Abt_Info.Text = "\tThis software reads text files from the cfg folder (" +
                "default location: C:\\Users\\UserNameHere\\Downloads\\DL_MGR\\cfg\\) and uses " +
                "Path's attached in them to categorize and sort files. Also, supports instructions via bracket syntax.";
        }
    }
}
