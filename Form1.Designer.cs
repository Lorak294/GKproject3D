namespace GKproject3D
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.imageBox = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.phongShadingRB = new System.Windows.Forms.RadioButton();
            this.gouraudShadingRB = new System.Windows.Forms.RadioButton();
            this.staticShadingRB = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.zTargetBox = new System.Windows.Forms.TextBox();
            this.yTargetBox = new System.Windows.Forms.TextBox();
            this.xTargetBox = new System.Windows.Forms.TextBox();
            this.grpbox = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.zPosBox = new System.Windows.Forms.TextBox();
            this.yPosBox = new System.Windows.Forms.TextBox();
            this.xPosBox = new System.Windows.Forms.TextBox();
            this.behindCameraRBtn = new System.Windows.Forms.RadioButton();
            this.staticCameraRBtn = new System.Windows.Forms.RadioButton();
            this.followingCameraRBtn = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.animationStartBtn = new System.Windows.Forms.Button();
            this.animationStopBtn = new System.Windows.Forms.Button();
            this.animationTimer = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.grpbox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 302F));
            this.tableLayoutPanel1.Controls.Add(this.imageBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1060, 638);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // imageBox
            // 
            this.imageBox.BackColor = System.Drawing.Color.White;
            this.imageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageBox.Location = new System.Drawing.Point(3, 3);
            this.imageBox.Name = "imageBox";
            this.imageBox.Size = new System.Drawing.Size(752, 632);
            this.imageBox.TabIndex = 0;
            this.imageBox.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(761, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(296, 632);
            this.panel1.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.phongShadingRB);
            this.groupBox3.Controls.Add(this.gouraudShadingRB);
            this.groupBox3.Controls.Add(this.staticShadingRB);
            this.groupBox3.Location = new System.Drawing.Point(24, 378);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(255, 109);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Shading";
            // 
            // phongShadingRB
            // 
            this.phongShadingRB.AutoSize = true;
            this.phongShadingRB.Location = new System.Drawing.Point(6, 72);
            this.phongShadingRB.Name = "phongShadingRB";
            this.phongShadingRB.Size = new System.Drawing.Size(60, 19);
            this.phongShadingRB.TabIndex = 2;
            this.phongShadingRB.Text = "Phong";
            this.phongShadingRB.UseVisualStyleBackColor = true;
            // 
            // gouraudShadingRB
            // 
            this.gouraudShadingRB.AutoSize = true;
            this.gouraudShadingRB.Location = new System.Drawing.Point(6, 47);
            this.gouraudShadingRB.Name = "gouraudShadingRB";
            this.gouraudShadingRB.Size = new System.Drawing.Size(71, 19);
            this.gouraudShadingRB.TabIndex = 1;
            this.gouraudShadingRB.Text = "Gouraud";
            this.gouraudShadingRB.UseVisualStyleBackColor = true;
            // 
            // staticShadingRB
            // 
            this.staticShadingRB.AutoSize = true;
            this.staticShadingRB.Checked = true;
            this.staticShadingRB.Location = new System.Drawing.Point(6, 22);
            this.staticShadingRB.Name = "staticShadingRB";
            this.staticShadingRB.Size = new System.Drawing.Size(54, 19);
            this.staticShadingRB.TabIndex = 0;
            this.staticShadingRB.TabStop = true;
            this.staticShadingRB.Text = "Static";
            this.staticShadingRB.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.grpbox);
            this.groupBox2.Controls.Add(this.behindCameraRBtn);
            this.groupBox2.Controls.Add(this.staticCameraRBtn);
            this.groupBox2.Controls.Add(this.followingCameraRBtn);
            this.groupBox2.Location = new System.Drawing.Point(24, 129);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(255, 243);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Camera ";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.zTargetBox);
            this.groupBox4.Controls.Add(this.yTargetBox);
            this.groupBox4.Controls.Add(this.xTargetBox);
            this.groupBox4.Location = new System.Drawing.Point(139, 97);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(110, 140);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Target";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 15);
            this.label4.TabIndex = 17;
            this.label4.Text = "Z";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 62);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 15);
            this.label5.TabIndex = 16;
            this.label5.Text = "Y";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 15);
            this.label6.TabIndex = 15;
            this.label6.Text = "X";
            // 
            // zTargetBox
            // 
            this.zTargetBox.Location = new System.Drawing.Point(37, 88);
            this.zTargetBox.Name = "zTargetBox";
            this.zTargetBox.Size = new System.Drawing.Size(56, 23);
            this.zTargetBox.TabIndex = 14;
            // 
            // yTargetBox
            // 
            this.yTargetBox.Location = new System.Drawing.Point(37, 59);
            this.yTargetBox.Name = "yTargetBox";
            this.yTargetBox.Size = new System.Drawing.Size(56, 23);
            this.yTargetBox.TabIndex = 13;
            // 
            // xTargetBox
            // 
            this.xTargetBox.Location = new System.Drawing.Point(37, 30);
            this.xTargetBox.Name = "xTargetBox";
            this.xTargetBox.Size = new System.Drawing.Size(56, 23);
            this.xTargetBox.TabIndex = 12;
            // 
            // grpbox
            // 
            this.grpbox.Controls.Add(this.label3);
            this.grpbox.Controls.Add(this.label2);
            this.grpbox.Controls.Add(this.label1);
            this.grpbox.Controls.Add(this.zPosBox);
            this.grpbox.Controls.Add(this.yPosBox);
            this.grpbox.Controls.Add(this.xPosBox);
            this.grpbox.Location = new System.Drawing.Point(6, 97);
            this.grpbox.Name = "grpbox";
            this.grpbox.Size = new System.Drawing.Size(110, 140);
            this.grpbox.TabIndex = 6;
            this.grpbox.TabStop = false;
            this.grpbox.Text = "Position";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 15);
            this.label3.TabIndex = 11;
            this.label3.Text = "Z";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "Y";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "X";
            // 
            // zPosBox
            // 
            this.zPosBox.Location = new System.Drawing.Point(32, 88);
            this.zPosBox.Name = "zPosBox";
            this.zPosBox.Size = new System.Drawing.Size(56, 23);
            this.zPosBox.TabIndex = 8;
            // 
            // yPosBox
            // 
            this.yPosBox.Location = new System.Drawing.Point(32, 59);
            this.yPosBox.Name = "yPosBox";
            this.yPosBox.Size = new System.Drawing.Size(56, 23);
            this.yPosBox.TabIndex = 7;
            // 
            // xPosBox
            // 
            this.xPosBox.Location = new System.Drawing.Point(32, 30);
            this.xPosBox.Name = "xPosBox";
            this.xPosBox.Size = new System.Drawing.Size(56, 23);
            this.xPosBox.TabIndex = 0;
            // 
            // behindCameraRBtn
            // 
            this.behindCameraRBtn.AutoSize = true;
            this.behindCameraRBtn.Location = new System.Drawing.Point(6, 72);
            this.behindCameraRBtn.Name = "behindCameraRBtn";
            this.behindCameraRBtn.Size = new System.Drawing.Size(101, 19);
            this.behindCameraRBtn.TabIndex = 4;
            this.behindCameraRBtn.TabStop = true;
            this.behindCameraRBtn.Text = "Behind the car";
            this.behindCameraRBtn.UseVisualStyleBackColor = true;
            this.behindCameraRBtn.CheckedChanged += new System.EventHandler(this.CameraRBtn_CheckedChanged);
            // 
            // staticCameraRBtn
            // 
            this.staticCameraRBtn.AutoSize = true;
            this.staticCameraRBtn.Checked = true;
            this.staticCameraRBtn.Location = new System.Drawing.Point(6, 22);
            this.staticCameraRBtn.Name = "staticCameraRBtn";
            this.staticCameraRBtn.Size = new System.Drawing.Size(96, 19);
            this.staticCameraRBtn.TabIndex = 2;
            this.staticCameraRBtn.TabStop = true;
            this.staticCameraRBtn.Text = "Static camera";
            this.staticCameraRBtn.UseVisualStyleBackColor = true;
            this.staticCameraRBtn.CheckedChanged += new System.EventHandler(this.CameraRBtn_CheckedChanged);
            // 
            // followingCameraRBtn
            // 
            this.followingCameraRBtn.AutoSize = true;
            this.followingCameraRBtn.Location = new System.Drawing.Point(6, 47);
            this.followingCameraRBtn.Name = "followingCameraRBtn";
            this.followingCameraRBtn.Size = new System.Drawing.Size(99, 19);
            this.followingCameraRBtn.TabIndex = 3;
            this.followingCameraRBtn.TabStop = true;
            this.followingCameraRBtn.Text = "Follow the car";
            this.followingCameraRBtn.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.animationStartBtn);
            this.groupBox1.Controls.Add(this.animationStopBtn);
            this.groupBox1.Location = new System.Drawing.Point(24, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(263, 114);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Animation";
            // 
            // animationStartBtn
            // 
            this.animationStartBtn.Location = new System.Drawing.Point(6, 22);
            this.animationStartBtn.Name = "animationStartBtn";
            this.animationStartBtn.Size = new System.Drawing.Size(88, 28);
            this.animationStartBtn.TabIndex = 1;
            this.animationStartBtn.Text = "Start";
            this.animationStartBtn.UseVisualStyleBackColor = true;
            this.animationStartBtn.Click += new System.EventHandler(this.animationStartBtn_Click);
            // 
            // animationStopBtn
            // 
            this.animationStopBtn.Location = new System.Drawing.Point(6, 56);
            this.animationStopBtn.Name = "animationStopBtn";
            this.animationStopBtn.Size = new System.Drawing.Size(88, 28);
            this.animationStopBtn.TabIndex = 4;
            this.animationStopBtn.Text = "Stop";
            this.animationStopBtn.UseVisualStyleBackColor = true;
            this.animationStopBtn.Click += new System.EventHandler(this.animationStopBtn_Click);
            // 
            // animationTimer
            // 
            this.animationTimer.Interval = 50;
            this.animationTimer.Tick += new System.EventHandler(this.animationTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 638);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.grpbox.ResumeLayout(false);
            this.grpbox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private PictureBox imageBox;
        private Panel panel1;
        private RadioButton followingCameraRBtn;
        private RadioButton staticCameraRBtn;
        private Button animationStartBtn;
        private GroupBox groupBox1;
        private Button animationStopBtn;
        private System.Windows.Forms.Timer animationTimer;
        private GroupBox groupBox2;
        private RadioButton behindCameraRBtn;
        private GroupBox groupBox4;
        private Label label4;
        private Label label5;
        private Label label6;
        private TextBox zTargetBox;
        private TextBox yTargetBox;
        private TextBox xTargetBox;
        private GroupBox grpbox;
        private Label label3;
        private Label label2;
        private Label label1;
        private TextBox zPosBox;
        private TextBox yPosBox;
        private TextBox xPosBox;
        private GroupBox groupBox3;
        private RadioButton phongShadingRB;
        private RadioButton gouraudShadingRB;
        private RadioButton staticShadingRB;
    }
}