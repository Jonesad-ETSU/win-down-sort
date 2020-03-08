namespace FileManagerTaskTray
{
    partial class Wndw_Abt
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Wndw_Abt));
            this.Window_Abt_Title = new System.Windows.Forms.TextBox();
            this.Window_Abt_Info = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Window_Abt_Title
            // 
            this.Window_Abt_Title.BackColor = System.Drawing.SystemColors.Desktop;
            this.Window_Abt_Title.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Window_Abt_Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Window_Abt_Title.ForeColor = System.Drawing.SystemColors.Window;
            this.Window_Abt_Title.Location = new System.Drawing.Point(123, 12);
            this.Window_Abt_Title.Multiline = true;
            this.Window_Abt_Title.Name = "Window_Abt_Title";
            this.Window_Abt_Title.ReadOnly = true;
            this.Window_Abt_Title.Size = new System.Drawing.Size(512, 61);
            this.Window_Abt_Title.TabIndex = 0;
            this.Window_Abt_Title.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Window_Abt_Info
            // 
            this.Window_Abt_Info.BackColor = System.Drawing.SystemColors.Desktop;
            this.Window_Abt_Info.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Window_Abt_Info.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Window_Abt_Info.ForeColor = System.Drawing.SystemColors.Window;
            this.Window_Abt_Info.Location = new System.Drawing.Point(12, 79);
            this.Window_Abt_Info.Multiline = true;
            this.Window_Abt_Info.Name = "Window_Abt_Info";
            this.Window_Abt_Info.ReadOnly = true;
            this.Window_Abt_Info.Size = new System.Drawing.Size(756, 276);
            this.Window_Abt_Info.TabIndex = 1;
            // 
            // Wndw_Abt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.ClientSize = new System.Drawing.Size(780, 450);
            this.Controls.Add(this.Window_Abt_Info);
            this.Controls.Add(this.Window_Abt_Title);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Wndw_Abt";
            this.Text = "DL_MGR- About";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Window_Abt_Title;
        private System.Windows.Forms.TextBox Window_Abt_Info;
    }
}

