namespace GobangClient
{
    partial class PlayingTable
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
            //
            if (ShackleSpell == true)
            {
                System.Windows.Forms.MessageBox.Show("With the effect of Shackle Spell, no one can leave or tie");
                return;
            }
            //
            if (disposing && (components != null))
            {
                components.Dispose();        // This contains the effect the form itself being closed
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
            this.labelSideUp = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.labelSideDown = new System.Windows.Forms.Label();
            this.labelGo = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonAskTie = new System.Windows.Forms.Button();
            this.radioButtonMusic = new System.Windows.Forms.RadioButton();
            this.radioButtonSound = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.pictureBoxShackles = new System.Windows.Forms.PictureBox();
            this.pictureBoxFrozen = new System.Windows.Forms.PictureBox();
            this.pictureBoxIllusion = new System.Windows.Forms.PictureBox();
            this.pictureBoxDivision = new System.Windows.Forms.PictureBox();
            this.pictureBoxDown = new System.Windows.Forms.PictureBox();
            this.pictureBoxUp = new System.Windows.Forms.PictureBox();
            this.pictureBoxBoard = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxShackles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFrozen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIllusion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDivision)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBoard)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelSideUp
            // 
            this.labelSideUp.AutoSize = true;
            this.labelSideUp.Location = new System.Drawing.Point(86, 25);
            this.labelSideUp.Name = "labelSideUp";
            this.labelSideUp.Size = new System.Drawing.Size(35, 12);
            this.labelSideUp.TabIndex = 2;
            this.labelSideUp.Text = "Black";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(13, 53);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(163, 201);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // labelSideDown
            // 
            this.labelSideDown.AutoSize = true;
            this.labelSideDown.Location = new System.Drawing.Point(86, 278);
            this.labelSideDown.Name = "labelSideDown";
            this.labelSideDown.Size = new System.Drawing.Size(35, 12);
            this.labelSideDown.TabIndex = 5;
            this.labelSideDown.Text = "White";
            // 
            // labelGo
            // 
            this.labelGo.AutoSize = true;
            this.labelGo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.labelGo.Location = new System.Drawing.Point(30, 7);
            this.labelGo.Name = "labelGo";
            this.labelGo.Size = new System.Drawing.Size(89, 12);
            this.labelGo.TabIndex = 5;
            this.labelGo.Text = "Whose turn now";
            this.labelGo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.panel1.Controls.Add(this.labelGo);
            this.panel1.Location = new System.Drawing.Point(18, 310);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(158, 28);
            this.panel1.TabIndex = 6;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(10, 359);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 7;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonAskTie
            // 
            this.buttonAskTie.Location = new System.Drawing.Point(112, 359);
            this.buttonAskTie.Name = "buttonAskTie";
            this.buttonAskTie.Size = new System.Drawing.Size(75, 23);
            this.buttonAskTie.TabIndex = 7;
            this.buttonAskTie.Text = "Ask Tie";
            this.buttonAskTie.UseVisualStyleBackColor = true;
            this.buttonAskTie.Click += new System.EventHandler(this.buttonAskTie_Click);
            // 
            // radioButtonMusic
            // 
            this.radioButtonMusic.AutoSize = true;
            this.radioButtonMusic.Location = new System.Drawing.Point(248, 371);
            this.radioButtonMusic.Name = "radioButtonMusic";
            this.radioButtonMusic.Size = new System.Drawing.Size(119, 16);
            this.radioButtonMusic.TabIndex = 8;
            this.radioButtonMusic.TabStop = true;
            this.radioButtonMusic.Text = "Background Music";
            this.radioButtonMusic.UseVisualStyleBackColor = true;
            this.radioButtonMusic.CheckedChanged += new System.EventHandler(this.radioButtonMusic_CheckedChanged);
            // 
            // radioButtonSound
            // 
            this.radioButtonSound.AutoSize = true;
            this.radioButtonSound.Location = new System.Drawing.Point(377, 371);
            this.radioButtonSound.Name = "radioButtonSound";
            this.radioButtonSound.Size = new System.Drawing.Size(89, 16);
            this.radioButtonSound.TabIndex = 8;
            this.radioButtonSound.TabStop = true;
            this.radioButtonSound.Text = "Chess Sound";
            this.radioButtonSound.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 397);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "Talk:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(58, 394);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(395, 21);
            this.textBox1.TabIndex = 10;
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(462, 392);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(75, 23);
            this.buttonSend.TabIndex = 11;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // pictureBoxShackles
            // 
            this.pictureBoxShackles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pictureBoxShackles.Location = new System.Drawing.Point(419, 4);
            this.pictureBoxShackles.Name = "pictureBoxShackles";
            this.pictureBoxShackles.Size = new System.Drawing.Size(116, 96);
            this.pictureBoxShackles.TabIndex = 12;
            this.pictureBoxShackles.TabStop = false;
            this.pictureBoxShackles.Click += new System.EventHandler(this.pictureBoxShackles_Click);
            // 
            // pictureBoxFrozen
            // 
            this.pictureBoxFrozen.BackColor = System.Drawing.Color.Cyan;
            this.pictureBoxFrozen.Location = new System.Drawing.Point(281, 4);
            this.pictureBoxFrozen.Name = "pictureBoxFrozen";
            this.pictureBoxFrozen.Size = new System.Drawing.Size(121, 96);
            this.pictureBoxFrozen.TabIndex = 12;
            this.pictureBoxFrozen.TabStop = false;
            this.pictureBoxFrozen.Click += new System.EventHandler(this.pictureBoxFrozen_Click);
            // 
            // pictureBoxIllusion
            // 
            this.pictureBoxIllusion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.pictureBoxIllusion.Location = new System.Drawing.Point(142, 4);
            this.pictureBoxIllusion.Name = "pictureBoxIllusion";
            this.pictureBoxIllusion.Size = new System.Drawing.Size(123, 96);
            this.pictureBoxIllusion.TabIndex = 12;
            this.pictureBoxIllusion.TabStop = false;
            this.pictureBoxIllusion.Click += new System.EventHandler(this.pictureBoxIllusion_Click);
            // 
            // pictureBoxDivision
            // 
            this.pictureBoxDivision.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.pictureBoxDivision.Location = new System.Drawing.Point(8, 4);
            this.pictureBoxDivision.Name = "pictureBoxDivision";
            this.pictureBoxDivision.Size = new System.Drawing.Size(118, 96);
            this.pictureBoxDivision.TabIndex = 12;
            this.pictureBoxDivision.TabStop = false;
            this.pictureBoxDivision.Click += new System.EventHandler(this.pictureBoxDivision_Click);
            // 
            // pictureBoxDown
            // 
            this.pictureBoxDown.Image = global::GobangClient.Properties.Resources.whiteChess;
            this.pictureBoxDown.Location = new System.Drawing.Point(49, 274);
            this.pictureBoxDown.Name = "pictureBoxDown";
            this.pictureBoxDown.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxDown.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxDown.TabIndex = 4;
            this.pictureBoxDown.TabStop = false;
            // 
            // pictureBoxUp
            // 
            this.pictureBoxUp.Image = global::GobangClient.Properties.Resources.blackChess;
            this.pictureBoxUp.Location = new System.Drawing.Point(49, 21);
            this.pictureBoxUp.Name = "pictureBoxUp";
            this.pictureBoxUp.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxUp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxUp.TabIndex = 1;
            this.pictureBoxUp.TabStop = false;
            // 
            // pictureBoxBoard
            // 
            this.pictureBoxBoard.Image = global::GobangClient.Properties.Resources.GobangBoard;
            this.pictureBoxBoard.Location = new System.Drawing.Point(194, 21);
            this.pictureBoxBoard.Name = "pictureBoxBoard";
            this.pictureBoxBoard.Size = new System.Drawing.Size(343, 343);
            this.pictureBoxBoard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxBoard.TabIndex = 0;
            this.pictureBoxBoard.TabStop = false;
            this.pictureBoxBoard.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxBoard_Paint);
            this.pictureBoxBoard.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxBoard_MouseDown);
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::GobangClient.Properties.Resources._1167909243_81472122;
            this.panel2.Controls.Add(this.pictureBoxShackles);
            this.panel2.Controls.Add(this.pictureBoxDivision);
            this.panel2.Controls.Add(this.pictureBoxFrozen);
            this.panel2.Controls.Add(this.pictureBoxIllusion);
            this.panel2.Location = new System.Drawing.Point(-3, 421);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(549, 109);
            this.panel2.TabIndex = 13;
            // 
            // PlayingTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::GobangClient.Properties.Resources._5_120601095138;
            this.ClientSize = new System.Drawing.Size(547, 529);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radioButtonSound);
            this.Controls.Add(this.radioButtonMusic);
            this.Controls.Add(this.buttonAskTie);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelSideDown);
            this.Controls.Add(this.pictureBoxDown);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.labelSideUp);
            this.Controls.Add(this.pictureBoxUp);
            this.Controls.Add(this.pictureBoxBoard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "PlayingTable";
            this.Text = "PlayingTable";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PlayingTable_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PlayingTable_FormClosed);
            this.Load += new System.EventHandler(this.PlayingTable_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxShackles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFrozen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIllusion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDivision)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBoard)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxBoard;
        private System.Windows.Forms.PictureBox pictureBoxUp;
        private System.Windows.Forms.Label labelSideUp;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label labelSideDown;
        private System.Windows.Forms.PictureBox pictureBoxDown;
        private System.Windows.Forms.Label labelGo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonAskTie;
        private System.Windows.Forms.RadioButton radioButtonMusic;
        private System.Windows.Forms.RadioButton radioButtonSound;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.PictureBox pictureBoxDivision;
        private System.Windows.Forms.PictureBox pictureBoxIllusion;
        private System.Windows.Forms.PictureBox pictureBoxFrozen;
        private System.Windows.Forms.PictureBox pictureBoxShackles;
        private System.Windows.Forms.Panel panel2;
    }
}