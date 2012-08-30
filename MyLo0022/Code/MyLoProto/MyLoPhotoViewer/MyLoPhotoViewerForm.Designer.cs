namespace MyLoPhotoViewerNS
{
    partial class MyLoPhotoViewer
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.MessageBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.queryResultsView = new System.Windows.Forms.DataGridView();
            this.query3 = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buildUsingIntervalGPSfit = new System.Windows.Forms.Button();
            this.loadPhotosButton = new System.Windows.Forms.Button();
            this.getTripItContextButton = new System.Windows.Forms.Button();
            this.getCalendarContextButton = new System.Windows.Forms.Button();
            this.catalogPhotosButton = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.getFacebookContextButton = new System.Windows.Forms.Button();
            this.queryBySelectedEvent = new System.Windows.Forms.Button();
            this.queryButton = new System.Windows.Forms.Button();
            this.timeListBox = new System.Windows.Forms.ListBox();
            this.locationListBox = new System.Windows.Forms.ListBox();
            this.eventListBox = new System.Windows.Forms.ListBox();
            this.peopleListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.signOutButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.displayMyLoMap = new System.Windows.Forms.Button();
            this.eventTreeView = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this.queryResultsView)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(328, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(158, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "Enter MyLo Account Id";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // MessageBox
            // 
            this.MessageBox.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.MessageBox.Location = new System.Drawing.Point(625, 12);
            this.MessageBox.Multiline = true;
            this.MessageBox.Name = "MessageBox";
            this.MessageBox.Size = new System.Drawing.Size(251, 42);
            this.MessageBox.TabIndex = 4;
            this.MessageBox.TextChanged += new System.EventHandler(this.MessageBox_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(492, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(127, 42);
            this.button1.TabIndex = 6;
            this.button1.Text = "Sign In";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // queryResultsView
            // 
            this.queryResultsView.AllowUserToAddRows = false;
            this.queryResultsView.AllowUserToDeleteRows = false;
            this.queryResultsView.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightSkyBlue;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.DodgerBlue;
            this.queryResultsView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.queryResultsView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.queryResultsView.Location = new System.Drawing.Point(203, 283);
            this.queryResultsView.Name = "queryResultsView";
            this.queryResultsView.ReadOnly = true;
            this.queryResultsView.Size = new System.Drawing.Size(340, 253);
            this.queryResultsView.TabIndex = 9;
            this.queryResultsView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.queryResultsView_CellContentClick);
            // 
            // query3
            // 
            this.query3.Location = new System.Drawing.Point(8, 264);
            this.query3.Name = "query3";
            this.query3.Size = new System.Drawing.Size(162, 27);
            this.query3.TabIndex = 12;
            this.query3.Text = "All Photos";
            this.query3.UseVisualStyleBackColor = true;
            this.query3.Click += new System.EventHandler(this.query3_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(560, 283);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(436, 253);
            this.flowLayoutPanel1.TabIndex = 13;
            this.flowLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanel1_Paint);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(328, 34);
            this.textBox2.Name = "textBox2";
            this.textBox2.PasswordChar = '*';
            this.textBox2.Size = new System.Drawing.Size(157, 20);
            this.textBox2.TabIndex = 14;
            this.textBox2.Text = "Enter Password";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.buildUsingIntervalGPSfit);
            this.panel1.Controls.Add(this.loadPhotosButton);
            this.panel1.Controls.Add(this.getTripItContextButton);
            this.panel1.Controls.Add(this.getCalendarContextButton);
            this.panel1.Controls.Add(this.catalogPhotosButton);
            this.panel1.Controls.Add(this.textBox3);
            this.panel1.Controls.Add(this.query3);
            this.panel1.Controls.Add(this.getFacebookContextButton);
            this.panel1.Location = new System.Drawing.Point(8, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(177, 523);
            this.panel1.TabIndex = 15;
            // 
            // buildUsingIntervalGPSfit
            // 
            this.buildUsingIntervalGPSfit.Location = new System.Drawing.Point(8, 234);
            this.buildUsingIntervalGPSfit.Name = "buildUsingIntervalGPSfit";
            this.buildUsingIntervalGPSfit.Size = new System.Drawing.Size(161, 25);
            this.buildUsingIntervalGPSfit.TabIndex = 16;
            this.buildUsingIntervalGPSfit.Text = "Build Index Interval and GPS";
            this.buildUsingIntervalGPSfit.UseVisualStyleBackColor = true;
            this.buildUsingIntervalGPSfit.Click += new System.EventHandler(this.buildUsingIntervalGPSfit_Click);
            // 
            // loadPhotosButton
            // 
            this.loadPhotosButton.Location = new System.Drawing.Point(8, 170);
            this.loadPhotosButton.Name = "loadPhotosButton";
            this.loadPhotosButton.Size = new System.Drawing.Size(162, 27);
            this.loadPhotosButton.TabIndex = 15;
            this.loadPhotosButton.Text = "Load Photos";
            this.loadPhotosButton.UseVisualStyleBackColor = true;
            this.loadPhotosButton.Click += new System.EventHandler(this.loadPhotosButton_Click);
            // 
            // getTripItContextButton
            // 
            this.getTripItContextButton.Location = new System.Drawing.Point(8, 326);
            this.getTripItContextButton.Name = "getTripItContextButton";
            this.getTripItContextButton.Size = new System.Drawing.Size(162, 26);
            this.getTripItContextButton.TabIndex = 14;
            this.getTripItContextButton.Text = "Get TripIt Context";
            this.getTripItContextButton.UseVisualStyleBackColor = true;
            // 
            // getCalendarContextButton
            // 
            this.getCalendarContextButton.Location = new System.Drawing.Point(8, 297);
            this.getCalendarContextButton.Name = "getCalendarContextButton";
            this.getCalendarContextButton.Size = new System.Drawing.Size(162, 23);
            this.getCalendarContextButton.TabIndex = 13;
            this.getCalendarContextButton.Text = "Get Calendar Context";
            this.getCalendarContextButton.UseVisualStyleBackColor = true;
            this.getCalendarContextButton.Click += new System.EventHandler(this.getCalendarContextButton_Click);
            // 
            // catalogPhotosButton
            // 
            this.catalogPhotosButton.Location = new System.Drawing.Point(8, 203);
            this.catalogPhotosButton.Name = "catalogPhotosButton";
            this.catalogPhotosButton.Size = new System.Drawing.Size(162, 26);
            this.catalogPhotosButton.TabIndex = 3;
            this.catalogPhotosButton.Text = "Build Index Simple";
            this.catalogPhotosButton.UseVisualStyleBackColor = true;
            this.catalogPhotosButton.Click += new System.EventHandler(this.catalogPhotosButton_Click);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(8, 3);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(162, 126);
            this.textBox3.TabIndex = 1;
            // 
            // getFacebookContextButton
            // 
            this.getFacebookContextButton.Location = new System.Drawing.Point(8, 135);
            this.getFacebookContextButton.Name = "getFacebookContextButton";
            this.getFacebookContextButton.Size = new System.Drawing.Size(162, 29);
            this.getFacebookContextButton.TabIndex = 0;
            this.getFacebookContextButton.Text = "Get Facebook Context";
            this.getFacebookContextButton.UseVisualStyleBackColor = true;
            this.getFacebookContextButton.Click += new System.EventHandler(this.getFacebookContextButton_Click);
            // 
            // queryBySelectedEvent
            // 
            this.queryBySelectedEvent.Location = new System.Drawing.Point(720, 249);
            this.queryBySelectedEvent.Name = "queryBySelectedEvent";
            this.queryBySelectedEvent.Size = new System.Drawing.Size(170, 28);
            this.queryBySelectedEvent.TabIndex = 16;
            this.queryBySelectedEvent.Text = "Get Photos By Selected Event";
            this.queryBySelectedEvent.UseVisualStyleBackColor = true;
            this.queryBySelectedEvent.Click += new System.EventHandler(this.queryBySelectedEvent_Click);
            // 
            // queryButton
            // 
            this.queryButton.Location = new System.Drawing.Point(203, 249);
            this.queryButton.Name = "queryButton";
            this.queryButton.Size = new System.Drawing.Size(193, 28);
            this.queryButton.TabIndex = 15;
            this.queryButton.Text = "Get Photos By Selected Dimension";
            this.queryButton.UseVisualStyleBackColor = true;
            this.queryButton.Click += new System.EventHandler(this.queryButton_Click);
            // 
            // timeListBox
            // 
            this.timeListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeListBox.FormattingEnabled = true;
            this.timeListBox.Location = new System.Drawing.Point(203, 83);
            this.timeListBox.Name = "timeListBox";
            this.timeListBox.Size = new System.Drawing.Size(139, 160);
            this.timeListBox.TabIndex = 16;
            this.timeListBox.SelectedIndexChanged += new System.EventHandler(this.timeListBox_SelectedIndexChanged);
            // 
            // locationListBox
            // 
            this.locationListBox.FormattingEnabled = true;
            this.locationListBox.Location = new System.Drawing.Point(348, 83);
            this.locationListBox.Name = "locationListBox";
            this.locationListBox.Size = new System.Drawing.Size(156, 160);
            this.locationListBox.TabIndex = 17;
            this.locationListBox.SelectedIndexChanged += new System.EventHandler(this.locationListBox_SelectedIndexChanged);
            // 
            // eventListBox
            // 
            this.eventListBox.FormattingEnabled = true;
            this.eventListBox.Location = new System.Drawing.Point(720, 83);
            this.eventListBox.Name = "eventListBox";
            this.eventListBox.Size = new System.Drawing.Size(276, 160);
            this.eventListBox.TabIndex = 18;
            this.eventListBox.SelectedIndexChanged += new System.EventHandler(this.eventListBox_SelectedIndexChanged);
            // 
            // peopleListBox
            // 
            this.peopleListBox.FormattingEnabled = true;
            this.peopleListBox.Location = new System.Drawing.Point(510, 83);
            this.peopleListBox.Name = "peopleListBox";
            this.peopleListBox.Size = new System.Drawing.Size(153, 160);
            this.peopleListBox.TabIndex = 19;
            this.peopleListBox.SelectedIndexChanged += new System.EventHandler(this.peopleListBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(232, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Time Periods";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(397, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Locations";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(827, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Events";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(537, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "People and Groups";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(686, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 16);
            this.label5.TabIndex = 24;
            this.label5.Text = "OR";
            // 
            // signOutButton
            // 
            this.signOutButton.Enabled = false;
            this.signOutButton.Location = new System.Drawing.Point(883, 12);
            this.signOutButton.Name = "signOutButton";
            this.signOutButton.Size = new System.Drawing.Size(113, 42);
            this.signOutButton.TabIndex = 25;
            this.signOutButton.Text = "Sign Out";
            this.signOutButton.UseVisualStyleBackColor = true;
            this.signOutButton.Click += new System.EventHandler(this.signOutButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Ravie", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(202, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 30);
            this.label6.TabIndex = 26;
            this.label6.Text = "MyLo ";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(204, 39);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(104, 13);
            this.label7.TabIndex = 27;
            this.label7.Text = "My Life ... Organized";
            // 
            // displayMyLoMap
            // 
            this.displayMyLoMap.Location = new System.Drawing.Point(403, 250);
            this.displayMyLoMap.Name = "displayMyLoMap";
            this.displayMyLoMap.Size = new System.Drawing.Size(82, 27);
            this.displayMyLoMap.TabIndex = 28;
            this.displayMyLoMap.Text = "Show Map";
            this.displayMyLoMap.UseVisualStyleBackColor = true;
            this.displayMyLoMap.Click += new System.EventHandler(this.displayMyLoMap_Click);
            // 
            // eventTreeView
            // 
            this.eventTreeView.Location = new System.Drawing.Point(1022, 83);
            this.eventTreeView.Name = "eventTreeView";
            this.eventTreeView.Size = new System.Drawing.Size(293, 160);
            this.eventTreeView.TabIndex = 29;
            this.eventTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.eventTreeView_AfterSelect);
            // 
            // MyLoPhotoViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1366, 559);
            this.Controls.Add(this.eventTreeView);
            this.Controls.Add(this.displayMyLoMap);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.signOutButton);
            this.Controls.Add(this.queryBySelectedEvent);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.queryButton);
            this.Controls.Add(this.eventListBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.peopleListBox);
            this.Controls.Add(this.locationListBox);
            this.Controls.Add(this.timeListBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.queryResultsView);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.MessageBox);
            this.Controls.Add(this.textBox1);
            this.Name = "MyLoPhotoViewer";
            this.Text = "MyLo Photo Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.queryResultsView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox MessageBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView queryResultsView;
        private System.Windows.Forms.Button query3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button catalogPhotosButton;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button getFacebookContextButton;
        private System.Windows.Forms.ListBox timeListBox;
        private System.Windows.Forms.ListBox locationListBox;
        private System.Windows.Forms.ListBox eventListBox;
        private System.Windows.Forms.ListBox peopleListBox;
        private System.Windows.Forms.Button queryButton;
        private System.Windows.Forms.Button getTripItContextButton;
        private System.Windows.Forms.Button getCalendarContextButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button queryBySelectedEvent;
        private System.Windows.Forms.Button loadPhotosButton;
        private System.Windows.Forms.Button signOutButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buildUsingIntervalGPSfit;
        private System.Windows.Forms.Button displayMyLoMap;
        private System.Windows.Forms.TreeView eventTreeView;
    }
}

