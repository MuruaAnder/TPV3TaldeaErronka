namespace _2taldea
{
    partial class PlateraAddForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtIzena;
        private TextBox txtKategoria;
        private TextBox txtKantitatea;
        private TextBox txtPrezioa;
        private Button btnGorde;
        private Button btnUtzi;
        private Label labelTitle;
        private Label labelIzena;
        private Label labelKategoria;
        private Label labelKantitatea;
        private Label labelPrezioa;
        private PictureBox pictureBox;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            BackgroundImage = Image.FromFile("background.png");
            BackgroundImageLayout = ImageLayout.Stretch;
            txtIzena=new TextBox();
            txtKategoria=new TextBox();
            txtKantitatea=new TextBox();
            txtPrezioa=new TextBox();
            btnGorde=new Button();
            btnUtzi=new Button();
            labelTitle=new Label();
            labelIzena=new Label();
            labelKategoria=new Label();
            labelKantitatea=new Label();
            labelPrezioa=new Label();
            pictureBox=new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            SuspendLayout();
            // 
            // txtIzena
            // 
            txtIzena.BackColor=Color.White;
            txtIzena.ForeColor=Color.Black;
            txtIzena.Location=new Point(850, 400);
            txtIzena.Name="txtIzena";
            txtIzena.Size=new Size(350, 23);
            txtIzena.TabIndex=0;
            // 
            // txtKategoria
            // 
            txtKategoria.BackColor=Color.White;
            txtKategoria.ForeColor=Color.Black;
            txtKategoria.Location=new Point(850, 450);
            txtKategoria.Name="txtKategoria";
            txtKategoria.Size=new Size(350, 23);
            txtKategoria.TabIndex=1;
            // 
            // txtKantitatea
            // 
            txtKantitatea.BackColor=Color.White;
            txtKantitatea.ForeColor=Color.Black;
            txtKantitatea.Location=new Point(850, 500);
            txtKantitatea.Name="txtKantitatea";
            txtKantitatea.Size=new Size(350, 23);
            txtKantitatea.TabIndex=3;
            // 
            // txtPrezioa
            // 
            txtPrezioa.BackColor=Color.White;
            txtPrezioa.ForeColor=Color.Black;
            txtPrezioa.Location=new Point(850, 550);
            txtPrezioa.Name="txtPrezioa";
            txtPrezioa.Size=new Size(350, 23);
            txtPrezioa.TabIndex=2;
            txtPrezioa.TextChanged+=txtPrezioa_TextChanged;
            // 
            // btnGorde
            // 
            btnGorde.BackColor=Color.Green;
            btnGorde.FlatStyle=FlatStyle.Flat;
            btnGorde.Font=new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            btnGorde.ForeColor=Color.White;
            btnGorde.Location=new Point(500, 800);
            btnGorde.Name="btnGorde";
            btnGorde.Size=new Size(150, 50);
            btnGorde.TabIndex=5;
            btnGorde.Text="Gorde";
            btnGorde.UseVisualStyleBackColor=false;
            btnGorde.Click+=btnGorde_Click;
            // 
            // btnUtzi
            // 
            btnUtzi.BackColor=Color.Red;
            btnUtzi.FlatStyle=FlatStyle.Flat;
            btnUtzi.Font=new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            btnUtzi.ForeColor=Color.White;
            btnUtzi.Location=new Point(1300, 800);
            btnUtzi.Name="btnUtzi";
            btnUtzi.Size=new Size(150, 50);
            btnUtzi.TabIndex=6;
            btnUtzi.Text="Utzi";
            btnUtzi.UseVisualStyleBackColor=false;
            btnUtzi.Click+=btnUtzi_Click;
            // 
            // labelTitle
            // 
            labelTitle.AutoSize=true;
            labelTitle.BackColor=Color.Transparent;
            labelTitle.Font=new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point);
            labelTitle.ForeColor=Color.Gray;
            labelTitle.Location=new Point(850, 50);
            labelTitle.Name="labelTitle";
            labelTitle.Size=new Size(233, 45);
            labelTitle.TabIndex=7;
            labelTitle.Text="Platera Gehitu";
            // 
            // labelIzena
            // 
            labelIzena.AutoSize=true;
            labelIzena.BackColor=Color.Transparent;
            labelIzena.Font=new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            labelIzena.Location=new Point(600, 400);
            labelIzena.Name="labelIzena";
            labelIzena.Size=new Size(64, 25);
            labelIzena.TabIndex=8;
            labelIzena.Text="Izena:";
            // 
            // labelKategoria
            // 
            labelKategoria.AutoSize=true;
            labelKategoria.BackColor=Color.Transparent;
            labelKategoria.Font=new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            labelKategoria.Location=new Point(600, 450);
            labelKategoria.Name="labelKategoria";
            labelKategoria.Size=new Size(103, 25);
            labelKategoria.TabIndex=9;
            labelKategoria.Text="Kategoria:";
            // 
            // labelKantitatea
            // 
            labelKantitatea.AutoSize=true;
            labelKantitatea.BackColor=Color.Transparent;
            labelKantitatea.Font=new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            labelKantitatea.Location=new Point(600, 500);
            labelKantitatea.Name="labelKantitatea";
            labelKantitatea.Size=new Size(107, 25);
            labelKantitatea.TabIndex=12;
            labelKantitatea.Text="Kantitatea:";
            // 
            // labelPrezioa
            // 
            labelPrezioa.AutoSize=true;
            labelPrezioa.BackColor=Color.Transparent;
            labelPrezioa.Font=new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            labelPrezioa.Location=new Point(600, 550);
            labelPrezioa.Name="labelPrezioa";
            labelPrezioa.Size=new Size(83, 25);
            labelPrezioa.TabIndex=10;
            labelPrezioa.Text="Prezioa:";
            // 
            // pictureBox
            // 
            pictureBox.BackColor=Color.Transparent;
            pictureBox.Image=Properties.Resources.thebulls_logo;
            pictureBox.Location=new Point(50, 30);
            pictureBox.Name="pictureBox";
            pictureBox.Size=new Size(250, 200);
            pictureBox.SizeMode=PictureBoxSizeMode.Zoom;
            pictureBox.TabIndex=13;
            pictureBox.TabStop=false;
            // 
            // PlateraAddForm
            // 
            AutoScaleDimensions=new SizeF(7F, 15F);
            AutoScaleMode=AutoScaleMode.Font;
            BackColor=Color.FromArgb(243, 245, 248);
            BackgroundImageLayout=ImageLayout.Stretch;
            ClientSize=new Size(1920, 1061);
            Controls.Add(labelTitle);
            Controls.Add(txtIzena);
            Controls.Add(txtKategoria);
            Controls.Add(txtKantitatea);
            Controls.Add(txtPrezioa);
            Controls.Add(labelIzena);
            Controls.Add(labelKategoria);
            Controls.Add(labelKantitatea);
            Controls.Add(labelPrezioa);
            Controls.Add(btnGorde);
            Controls.Add(btnUtzi);
            Controls.Add(pictureBox);
            Name="PlateraAddForm";
            Text="Platera Gehitu";
            WindowState=FormWindowState.Maximized;
            Load+=PlateraAddForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}