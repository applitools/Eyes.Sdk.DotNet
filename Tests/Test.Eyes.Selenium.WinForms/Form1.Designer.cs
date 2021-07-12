
namespace Applitools.Selenium.Tests.WinForms
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbUrl = new System.Windows.Forms.TextBox();
            this.btnTestClassic = new System.Windows.Forms.Button();
            this.btnTestUFG = new System.Windows.Forms.Button();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbUrl
            // 
            this.tbUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbUrl.Location = new System.Drawing.Point(12, 12);
            this.tbUrl.Name = "tbUrl";
            this.tbUrl.Size = new System.Drawing.Size(349, 23);
            this.tbUrl.TabIndex = 0;
            this.tbUrl.Text = "https://applitools.com";
            // 
            // btnTestClassic
            // 
            this.btnTestClassic.Location = new System.Drawing.Point(12, 46);
            this.btnTestClassic.Name = "btnTestClassic";
            this.btnTestClassic.Size = new System.Drawing.Size(75, 24);
            this.btnTestClassic.TabIndex = 1;
            this.btnTestClassic.Text = "Test Classic";
            this.btnTestClassic.UseVisualStyleBackColor = true;
            this.btnTestClassic.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnTestUFG
            // 
            this.btnTestUFG.Location = new System.Drawing.Point(93, 46);
            this.btnTestUFG.Name = "btnTestUFG";
            this.btnTestUFG.Size = new System.Drawing.Size(75, 24);
            this.btnTestUFG.TabIndex = 2;
            this.btnTestUFG.Text = "Test UFG";
            this.btnTestUFG.UseVisualStyleBackColor = true;
            this.btnTestUFG.Click += new System.EventHandler(this.btnTestUFG_Click);
            // 
            // tbOutput
            // 
            this.tbOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbOutput.Location = new System.Drawing.Point(12, 77);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ReadOnly = true;
            this.tbOutput.Size = new System.Drawing.Size(349, 223);
            this.tbOutput.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 312);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.btnTestUFG);
            this.Controls.Add(this.btnTestClassic);
            this.Controls.Add(this.tbUrl);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Eyes Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbUrl;
        private System.Windows.Forms.Button btnTestClassic;
        private System.Windows.Forms.Button btnTestUFG;
        private System.Windows.Forms.TextBox tbOutput;
    }
}

