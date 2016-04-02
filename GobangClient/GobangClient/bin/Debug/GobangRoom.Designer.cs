namespace GobangUserControl
{
    partial class GobangRoom
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.labelBlack = new System.Windows.Forms.Label();
            this.labelWhite = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBoxBlack = new System.Windows.Forms.PictureBox();
            this.pictureBoxWhite = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBlack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWhite)).BeginInit();
            this.SuspendLayout();
            // 
            // labelBlack
            // 
            this.labelBlack.AutoSize = true;
            this.labelBlack.Location = new System.Drawing.Point(14, 49);
            this.labelBlack.Name = "labelBlack";
            this.labelBlack.Size = new System.Drawing.Size(83, 12);
            this.labelBlack.TabIndex = 1;
            this.labelBlack.Text = "Name of Black";
            this.labelBlack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelWhite
            // 
            this.labelWhite.AutoSize = true;
            this.labelWhite.Location = new System.Drawing.Point(155, 50);
            this.labelWhite.Name = "labelWhite";
            this.labelWhite.Size = new System.Drawing.Size(83, 12);
            this.labelWhite.TabIndex = 3;
            this.labelWhite.Text = "Name of White";
            this.labelWhite.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::GobangUserControl.Properties.Resources.SmallBoard;
            this.pictureBox1.Location = new System.Drawing.Point(101, 14);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBoxBlack
            // 
            this.pictureBoxBlack.BackColor = System.Drawing.Color.Linen;
            this.pictureBoxBlack.Location = new System.Drawing.Point(41, 14);
            this.pictureBoxBlack.Name = "pictureBoxBlack";
            this.pictureBoxBlack.Size = new System.Drawing.Size(35, 32);
            this.pictureBoxBlack.TabIndex = 0;
            this.pictureBoxBlack.TabStop = false;
            this.pictureBoxBlack.Click += new System.EventHandler(this.pictureBoxBlack_Click);
            // 
            // pictureBoxWhite
            // 
            this.pictureBoxWhite.BackColor = System.Drawing.Color.Linen;
            this.pictureBoxWhite.Location = new System.Drawing.Point(175, 14);
            this.pictureBoxWhite.Name = "pictureBoxWhite";
            this.pictureBoxWhite.Size = new System.Drawing.Size(35, 32);
            this.pictureBoxWhite.TabIndex = 0;
            this.pictureBoxWhite.TabStop = false;
            this.pictureBoxWhite.Click += new System.EventHandler(this.pictureBoxWhite_Click);
            // 
            // GobangRoom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkOrchid;
            this.Controls.Add(this.labelWhite);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelBlack);
            this.Controls.Add(this.pictureBoxWhite);
            this.Controls.Add(this.pictureBoxBlack);
            this.Name = "GobangRoom";
            this.Size = new System.Drawing.Size(255, 79);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.GobangRoom_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBlack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWhite)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelBlack;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelWhite;
        private System.Windows.Forms.PictureBox pictureBoxBlack;
        private System.Windows.Forms.PictureBox pictureBoxWhite;
    }
}
