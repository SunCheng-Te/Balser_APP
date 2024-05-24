namespace DialDemo
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
            components = new System.ComponentModel.Container();
            btnStart = new Button();
            btnStop = new Button();
            pictureBox1 = new PictureBox();
            DailListView = new ListView();
            Coordinate = new ColumnHeader();
            PUDefectClass = new ColumnHeader();
            Conf = new ColumnHeader();
            timer1 = new System.Windows.Forms.Timer(components);
            Savebtn = new Button();
            listBox1 = new ListBox();
            label2 = new Label();
            numericUpDown2 = new NumericUpDown();
            label3 = new Label();
            label4 = new Label();
            numericUpDown3 = new NumericUpDown();
            label5 = new Label();
            label6 = new Label();
            label1 = new Label();
            listBox2 = new ListBox();
            label7 = new Label();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            label8 = new Label();
            numericUpDown1 = new NumericUpDown();
            label9 = new Label();
            comboBox1 = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(36, 41);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(150, 149);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(213, 41);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(150, 149);
            btnStop.TabIndex = 1;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.ButtonHighlight;
            pictureBox1.Location = new Point(36, 215);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(800, 591);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // DailListView
            // 
            DailListView.Columns.AddRange(new ColumnHeader[] { Coordinate, PUDefectClass, Conf });
            DailListView.Location = new Point(634, 41);
            DailListView.Name = "DailListView";
            DailListView.Size = new Size(681, 151);
            DailListView.TabIndex = 3;
            DailListView.UseCompatibleStateImageBehavior = false;
            DailListView.View = View.Details;
            // 
            // Coordinate
            // 
            Coordinate.Text = "Box";
            Coordinate.Width = 175;
            // 
            // PUDefectClass
            // 
            PUDefectClass.Text = "Class";
            PUDefectClass.Width = 250;
            // 
            // Conf
            // 
            Conf.Text = "Conf";
            Conf.Width = 250;
            // 
            // timer1
            // 
            timer1.Interval = 20000;
            timer1.Tick += timer1_Tick;
            // 
            // Savebtn
            // 
            Savebtn.Location = new Point(394, 41);
            Savebtn.Name = "Savebtn";
            Savebtn.Size = new Size(150, 149);
            Savebtn.TabIndex = 5;
            Savebtn.Text = "Save";
            Savebtn.UseVisualStyleBackColor = true;
            Savebtn.Click += Savebtn_Click;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 20;
            listBox1.Items.AddRange(new object[] { "A區(車修區)", "B區(胎面區)", "C區(重疊區)", "D區(內輪區)" });
            listBox1.Location = new Point(1011, 336);
            listBox1.Margin = new Padding(4, 4, 4, 4);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(148, 104);
            listBox1.TabIndex = 7;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(891, 260);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(105, 20);
            label2.TabIndex = 9;
            label2.Text = "半成品半徑：";
            // 
            // numericUpDown2
            // 
            numericUpDown2.Location = new Point(1011, 257);
            numericUpDown2.Margin = new Padding(4, 4, 4, 4);
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(61, 28);
            numericUpDown2.TabIndex = 10;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(1081, 260);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(39, 20);
            label3.TabIndex = 11;
            label3.Text = "mm";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(1081, 298);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(39, 20);
            label4.TabIndex = 14;
            label4.Text = "mm";
            // 
            // numericUpDown3
            // 
            numericUpDown3.Location = new Point(1011, 295);
            numericUpDown3.Margin = new Padding(4, 4, 4, 4);
            numericUpDown3.Name = "numericUpDown3";
            numericUpDown3.Size = new Size(61, 28);
            numericUpDown3.TabIndex = 13;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(891, 298);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(89, 20);
            label5.TabIndex = 12;
            label5.Text = "成品半徑：";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(891, 373);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(89, 20);
            label6.TabIndex = 15;
            label6.Text = "設定區域：";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(891, 481);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(89, 20);
            label1.TabIndex = 16;
            label1.Text = "瑕疵設定：";
            // 
            // listBox2
            // 
            listBox2.FormattingEnabled = true;
            listBox2.ItemHeight = 20;
            listBox2.Items.AddRange(new object[] { "黑點雜質", "氣泡", "牽絲", "貼合不良", "內輪不良" });
            listBox2.Location = new Point(1011, 453);
            listBox2.Margin = new Padding(4, 4, 4, 4);
            listBox2.Name = "listBox2";
            listBox2.Size = new Size(148, 104);
            listBox2.TabIndex = 17;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(891, 581);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(89, 20);
            label7.TabIndex = 18;
            label7.Text = "允收標準：";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(1013, 581);
            checkBox1.Margin = new Padding(4, 4, 4, 4);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(95, 24);
            checkBox1.TabIndex = 19;
            checkBox1.Text = "不可允收";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(1011, 616);
            checkBox2.Margin = new Padding(4, 4, 4, 4);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(143, 24);
            checkBox2.TabIndex = 20;
            checkBox2.Text = "可允收，範圍於";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(1231, 619);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(71, 20);
            label8.TabIndex = 22;
            label8.Text = "mm以下";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(1161, 616);
            numericUpDown1.Margin = new Padding(4, 4, 4, 4);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(61, 28);
            numericUpDown1.TabIndex = 21;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(891, 215);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(89, 20);
            label9.TabIndex = 23;
            label9.Text = "產品顏色：";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(1009, 216);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(111, 28);
            comboBox1.TabIndex = 24;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1499, 1039);
            Controls.Add(comboBox1);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(numericUpDown1);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(label7);
            Controls.Add(listBox2);
            Controls.Add(label1);
            Controls.Add(label6);
            Controls.Add(label4);
            Controls.Add(numericUpDown3);
            Controls.Add(label5);
            Controls.Add(label3);
            Controls.Add(numericUpDown2);
            Controls.Add(label2);
            Controls.Add(listBox1);
            Controls.Add(Savebtn);
            Controls.Add(DailListView);
            Controls.Add(pictureBox1);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Name = "Form1";
            Text = "Form1";
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnStart;
        private Button btnStop;
        private PictureBox pictureBox1;
        private ListView DailListView;
        private ColumnHeader Coordinate;
        private ColumnHeader PUDefectClass;
        private ColumnHeader Conf;
        private System.Windows.Forms.Timer timer1;
        private Button Savebtn;
        private ListBox listBox1;
        private Label label2;
        private NumericUpDown numericUpDown2;
        private Label label3;
        private Label label4;
        private NumericUpDown numericUpDown3;
        private Label label5;
        private Label label6;
        private Label label1;
        private ListBox listBox2;
        private Label label7;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private Label label8;
        private NumericUpDown numericUpDown1;
        private Label label9;
        private ComboBox comboBox1;
    }
}