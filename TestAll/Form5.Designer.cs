namespace TestAll
{
    partial class Form5
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form5));
            this.button1 = new System.Windows.Forms.Button();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.listBox1 = new All.Control.Metro.ListBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(498, 622);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 47;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 12;
            this.listBox2.Items.AddRange(new object[] {
            resources.GetString("listBox2.Items"),
            resources.GetString("listBox2.Items1"),
            resources.GetString("listBox2.Items2"),
            resources.GetString("listBox2.Items3"),
            resources.GetString("listBox2.Items4"),
            resources.GetString("listBox2.Items5"),
            resources.GetString("listBox2.Items6"),
            resources.GetString("listBox2.Items7"),
            resources.GetString("listBox2.Items8"),
            resources.GetString("listBox2.Items9"),
            resources.GetString("listBox2.Items10"),
            resources.GetString("listBox2.Items11"),
            resources.GetString("listBox2.Items12"),
            resources.GetString("listBox2.Items13"),
            resources.GetString("listBox2.Items14"),
            resources.GetString("listBox2.Items15"),
            resources.GetString("listBox2.Items16"),
            resources.GetString("listBox2.Items17"),
            resources.GetString("listBox2.Items18"),
            resources.GetString("listBox2.Items19"),
            resources.GetString("listBox2.Items20"),
            resources.GetString("listBox2.Items21"),
            resources.GetString("listBox2.Items22"),
            resources.GetString("listBox2.Items23"),
            resources.GetString("listBox2.Items24"),
            resources.GetString("listBox2.Items25"),
            resources.GetString("listBox2.Items26"),
            resources.GetString("listBox2.Items27"),
            resources.GetString("listBox2.Items28"),
            "这里填写显示标题这里填写显示标题这里填写显示标题这里填写显示标题这里填写显示标题这里填写显示标题这里填写显示标题这里填写显示标题这里填写显示标题这里填写显示标题这" +
                "里填写显示标题这里填这里填写显示标题",
            "这里填写显示标题这里填写显示标题这里填写显示标题这里填写显示标题这里填写显示标题这里填写显示标题这里填写显示标题这里填写显示标题这里填写显示标题这里填写显示标题这" +
                "里填写显示标题这里填写显示标题这里填写显示标题这里填写显示标题这里填写显示标题这里填写显示标题"});
            this.listBox2.Location = new System.Drawing.Point(478, 24);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(833, 688);
            this.listBox2.TabIndex = 46;
            this.listBox2.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox2_DrawItem);
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.Black;
            this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Icon = null;
            this.listBox1.IconStyle = All.Control.Metro.ListBox.IconStyleList.三角形;
            this.listBox1.ItemColor = new System.Drawing.Color[] {
        System.Drawing.Color.Red,
        System.Drawing.Color.Maroon};
            this.listBox1.ItemHeight = 30;
            this.listBox1.Items.AddRange(new object[] {
            "字符串集合编辑器",
            "字符串集合编辑器",
            "字符串集合编辑器",
            "字符串集合编辑器",
            "字符串集合编辑器",
            "字符串集合编辑器",
            "字符串集合编辑器",
            "字符串集合编辑器"});
            this.listBox1.Location = new System.Drawing.Point(40, 36);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(415, 484);
            this.listBox1.TabIndex = 48;
            this.listBox1.Tail = true;
            this.listBox1.TitleColor = System.Drawing.Color.WhiteSmoke;
            this.listBox1.TitleFont = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            // 
            // Form5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BoardColor = System.Drawing.Color.Red;
            this.ClientSize = new System.Drawing.Size(1349, 765);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBox2);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "Form5";
            this.Text = "Form5";
            this.Load += new System.EventHandler(this.Form5_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Button button1;
        private All.Control.Metro.ListBox listBox1;
    }
}