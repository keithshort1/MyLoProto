namespace MyLoCalendarContextReaderApp
{
    partial class MyLoCalendarReaderForm
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
            this.saveContextButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.getEventsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // saveContextButton
            // 
            this.saveContextButton.Location = new System.Drawing.Point(385, 293);
            this.saveContextButton.Name = "saveContextButton";
            this.saveContextButton.Size = new System.Drawing.Size(158, 49);
            this.saveContextButton.TabIndex = 0;
            this.saveContextButton.Text = "Save Context To Database";
            this.saveContextButton.UseVisualStyleBackColor = true;
            this.saveContextButton.Click += new System.EventHandler(this.saveContextButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(62, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(190, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "This simulates reading calendar events";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(65, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(217, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "which are then stored into a MyLo data store";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(68, 138);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(280, 85);
            this.textBox1.TabIndex = 3;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // getEventsButton
            // 
            this.getEventsButton.Location = new System.Drawing.Point(65, 90);
            this.getEventsButton.Name = "getEventsButton";
            this.getEventsButton.Size = new System.Drawing.Size(153, 42);
            this.getEventsButton.TabIndex = 4;
            this.getEventsButton.Text = "Get Events From Calendar";
            this.getEventsButton.UseVisualStyleBackColor = true;
            this.getEventsButton.Click += new System.EventHandler(this.getEventsButton_Click);
            // 
            // MyLoCalendarReaderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 372);
            this.Controls.Add(this.getEventsButton);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.saveContextButton);
            this.Name = "MyLoCalendarReaderForm";
            this.Text = "MyLo Calendar Reader";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button saveContextButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button getEventsButton;
    }
}

