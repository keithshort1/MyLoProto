namespace MyLoFacebookContextApp
{
    partial class MyLoForm1
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
            this.buttonFBlogin = new System.Windows.Forms.Button();
            this.buttonFBlogout = new System.Windows.Forms.Button();
            this.facebookPanel1 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnSaveStore = new System.Windows.Forms.Button();
            this.btnRunQuery1 = new System.Windows.Forms.Button();
            this.query1result = new System.Windows.Forms.TextBox();
            this.queryOutBox = new System.Windows.Forms.TextBox();
            this.facebookPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonFBlogin
            // 
            this.buttonFBlogin.Location = new System.Drawing.Point(17, 11);
            this.buttonFBlogin.Name = "buttonFBlogin";
            this.buttonFBlogin.Size = new System.Drawing.Size(153, 24);
            this.buttonFBlogin.TabIndex = 0;
            this.buttonFBlogin.Text = "Add Items from Facebook";
            this.buttonFBlogin.UseVisualStyleBackColor = true;
            this.buttonFBlogin.Click += new System.EventHandler(this.buttonFBlogin_Click);
            // 
            // buttonFBlogout
            // 
            this.buttonFBlogout.Location = new System.Drawing.Point(427, 11);
            this.buttonFBlogout.Name = "buttonFBlogout";
            this.buttonFBlogout.Size = new System.Drawing.Size(141, 24);
            this.buttonFBlogout.TabIndex = 1;
            this.buttonFBlogout.Text = "Logout from Facebook";
            this.buttonFBlogout.UseVisualStyleBackColor = true;
            this.buttonFBlogout.Click += new System.EventHandler(this.buttonFBlogout_Click);
            // 
            // facebookPanel1
            // 
            this.facebookPanel1.Controls.Add(this.buttonFBlogout);
            this.facebookPanel1.Controls.Add(this.buttonFBlogin);
            this.facebookPanel1.Location = new System.Drawing.Point(22, 27);
            this.facebookPanel1.Name = "facebookPanel1";
            this.facebookPanel1.Size = new System.Drawing.Size(571, 87);
            this.facebookPanel1.TabIndex = 2;
            // 
            // textBox1
            // 
            this.textBox1.AcceptsReturn = true;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(39, 70);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(554, 29);
            this.textBox1.TabIndex = 2;
            // 
            // btnSaveStore
            // 
            this.btnSaveStore.Location = new System.Drawing.Point(442, 533);
            this.btnSaveStore.Name = "btnSaveStore";
            this.btnSaveStore.Size = new System.Drawing.Size(151, 44);
            this.btnSaveStore.TabIndex = 3;
            this.btnSaveStore.Text = "Save MyLo Context To  DB";
            this.btnSaveStore.UseVisualStyleBackColor = true;
            this.btnSaveStore.Click += new System.EventHandler(this.btnSaveStore_Click);
            // 
            // btnRunQuery1
            // 
            this.btnRunQuery1.Location = new System.Drawing.Point(39, 144);
            this.btnRunQuery1.Name = "btnRunQuery1";
            this.btnRunQuery1.Size = new System.Drawing.Size(153, 34);
            this.btnRunQuery1.TabIndex = 4;
            this.btnRunQuery1.Text = "Run Query";
            this.btnRunQuery1.UseVisualStyleBackColor = true;
            this.btnRunQuery1.Click += new System.EventHandler(this.btnRunQuery1_Click);
            // 
            // query1result
            // 
            this.query1result.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.query1result.Location = new System.Drawing.Point(212, 145);
            this.query1result.Multiline = true;
            this.query1result.Name = "query1result";
            this.query1result.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.query1result.Size = new System.Drawing.Size(381, 103);
            this.query1result.TabIndex = 5;
            this.query1result.TextChanged += new System.EventHandler(this.query1result_TextChanged);
            // 
            // queryOutBox
            // 
            this.queryOutBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.queryOutBox.Location = new System.Drawing.Point(39, 264);
            this.queryOutBox.Multiline = true;
            this.queryOutBox.Name = "queryOutBox";
            this.queryOutBox.ReadOnly = true;
            this.queryOutBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.queryOutBox.Size = new System.Drawing.Size(553, 250);
            this.queryOutBox.TabIndex = 6;
            this.queryOutBox.TextChanged += new System.EventHandler(this.queryOutBox_TextChanged);
            // 
            // MyLoForm1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(628, 589);
            this.Controls.Add(this.queryOutBox);
            this.Controls.Add(this.query1result);
            this.Controls.Add(this.btnRunQuery1);
            this.Controls.Add(this.btnSaveStore);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.facebookPanel1);
            this.Name = "MyLoForm1";
            this.Text = "MyLo Facebook Context Application";
            this.Load += new System.EventHandler(this.MyLOForm1_Load);
            this.facebookPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonFBlogin;
        private System.Windows.Forms.Button buttonFBlogout;
        private System.Windows.Forms.Panel facebookPanel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnSaveStore;
        private System.Windows.Forms.Button btnRunQuery1;
        private System.Windows.Forms.TextBox query1result;
        private System.Windows.Forms.TextBox queryOutBox;
    }
}

