﻿namespace MapPushpinPhotoDisplay
{
    partial class PushpinPhotos
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
            this.pushpinPhotosLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pushpinPhotosLayout
            // 
            this.pushpinPhotosLayout.AutoScroll = true;
            this.pushpinPhotosLayout.Location = new System.Drawing.Point(22, 39);
            this.pushpinPhotosLayout.Name = "pushpinPhotosLayout";
            this.pushpinPhotosLayout.Size = new System.Drawing.Size(560, 283);
            this.pushpinPhotosLayout.TabIndex = 0;
            this.pushpinPhotosLayout.Paint += new System.Windows.Forms.PaintEventHandler(this.pushpinPhotosLayout_Paint);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(507, 341);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // PushpinPhotos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 385);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.pushpinPhotosLayout);
            this.Name = "PushpinPhotos";
            this.Text = "Pushpin Photo Display";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel pushpinPhotosLayout;
        private System.Windows.Forms.Button btnClose;
    }
}

