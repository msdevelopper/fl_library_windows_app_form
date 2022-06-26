namespace FuzzyLogic
{
    partial class Form1
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tb_vars = new System.Windows.Forms.TextBox();
            this.tb_rules = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_validate_fl = new System.Windows.Forms.Button();
            this.btn_run_simulation = new System.Windows.Forms.Button();
            this.infos = new System.Windows.Forms.Label();
            this.tc_input_vars = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.tb_resolution = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_resolution = new System.Windows.Forms.Button();
            this.tc_simulation = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.tc_output_vars = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.pbtestrender = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.tc_input_vars.SuspendLayout();
            this.tc_simulation.SuspendLayout();
            this.tc_output_vars.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbtestrender)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(381, 465);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(797, 329);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            this.chart1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseMove);
            // 
            // tb_vars
            // 
            this.tb_vars.Location = new System.Drawing.Point(64, 50);
            this.tb_vars.Multiline = true;
            this.tb_vars.Name = "tb_vars";
            this.tb_vars.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tb_vars.Size = new System.Drawing.Size(306, 166);
            this.tb_vars.TabIndex = 1;
            this.tb_vars.WordWrap = false;
            // 
            // tb_rules
            // 
            this.tb_rules.Location = new System.Drawing.Point(64, 277);
            this.tb_rules.Multiline = true;
            this.tb_rules.Name = "tb_rules";
            this.tb_rules.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tb_rules.Size = new System.Drawing.Size(306, 186);
            this.tb_rules.TabIndex = 2;
            this.tb_rules.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Variables:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(61, 261);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Rules";
            // 
            // btn_validate_fl
            // 
            this.btn_validate_fl.Location = new System.Drawing.Point(64, 218);
            this.btn_validate_fl.Name = "btn_validate_fl";
            this.btn_validate_fl.Size = new System.Drawing.Size(306, 40);
            this.btn_validate_fl.TabIndex = 5;
            this.btn_validate_fl.Text = "Validate Variables and Rules";
            this.btn_validate_fl.UseVisualStyleBackColor = true;
            this.btn_validate_fl.Click += new System.EventHandler(this.btn_validate_fl_Click);
            // 
            // btn_run_simulation
            // 
            this.btn_run_simulation.Location = new System.Drawing.Point(764, 69);
            this.btn_run_simulation.Name = "btn_run_simulation";
            this.btn_run_simulation.Size = new System.Drawing.Size(361, 28);
            this.btn_run_simulation.TabIndex = 6;
            this.btn_run_simulation.Text = "Run Simulation";
            this.btn_run_simulation.UseVisualStyleBackColor = true;
            this.btn_run_simulation.Click += new System.EventHandler(this.btn_run_simulation_Click);
            // 
            // infos
            // 
            this.infos.AutoSize = true;
            this.infos.Location = new System.Drawing.Point(382, 136);
            this.infos.Name = "infos";
            this.infos.Size = new System.Drawing.Size(73, 13);
            this.infos.TabIndex = 9;
            this.infos.Text = "Inputs domain";
            // 
            // tc_input_vars
            // 
            this.tc_input_vars.Controls.Add(this.tabPage1);
            this.tc_input_vars.Location = new System.Drawing.Point(381, 163);
            this.tc_input_vars.Name = "tc_input_vars";
            this.tc_input_vars.SelectedIndex = 0;
            this.tc_input_vars.Size = new System.Drawing.Size(400, 300);
            this.tc_input_vars.TabIndex = 10;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(392, 274);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tb_resolution
            // 
            this.tb_resolution.Location = new System.Drawing.Point(827, 36);
            this.tb_resolution.Name = "tb_resolution";
            this.tb_resolution.Size = new System.Drawing.Size(100, 20);
            this.tb_resolution.TabIndex = 11;
            this.tb_resolution.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(761, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Resolution:";
            // 
            // btn_resolution
            // 
            this.btn_resolution.Location = new System.Drawing.Point(933, 33);
            this.btn_resolution.Name = "btn_resolution";
            this.btn_resolution.Size = new System.Drawing.Size(100, 23);
            this.btn_resolution.TabIndex = 13;
            this.btn_resolution.Text = "change resolution";
            this.btn_resolution.UseVisualStyleBackColor = true;
            this.btn_resolution.Click += new System.EventHandler(this.btn_resolution_Click);
            // 
            // tc_simulation
            // 
            this.tc_simulation.Controls.Add(this.tabPage2);
            this.tc_simulation.Location = new System.Drawing.Point(381, 49);
            this.tc_simulation.Name = "tc_simulation";
            this.tc_simulation.SelectedIndex = 0;
            this.tc_simulation.Size = new System.Drawing.Size(361, 84);
            this.tc_simulation.TabIndex = 14;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(353, 58);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(378, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Simulation property";
            // 
            // tc_output_vars
            // 
            this.tc_output_vars.Controls.Add(this.tabPage4);
            this.tc_output_vars.Location = new System.Drawing.Point(778, 163);
            this.tc_output_vars.Name = "tc_output_vars";
            this.tc_output_vars.SelectedIndex = 0;
            this.tc_output_vars.Size = new System.Drawing.Size(400, 300);
            this.tc_output_vars.TabIndex = 16;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(392, 274);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(764, 110);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 17;
            this.button1.Text = "Clear Graph";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pbtestrender
            // 
            this.pbtestrender.Location = new System.Drawing.Point(83, 496);
            this.pbtestrender.Name = "pbtestrender";
            this.pbtestrender.Size = new System.Drawing.Size(300, 300);
            this.pbtestrender.TabIndex = 18;
            this.pbtestrender.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1273, 807);
            this.Controls.Add(this.pbtestrender);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tc_output_vars);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tc_simulation);
            this.Controls.Add(this.btn_resolution);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tb_resolution);
            this.Controls.Add(this.tc_input_vars);
            this.Controls.Add(this.infos);
            this.Controls.Add(this.btn_run_simulation);
            this.Controls.Add(this.btn_validate_fl);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb_rules);
            this.Controls.Add(this.tb_vars);
            this.Controls.Add(this.chart1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.tc_input_vars.ResumeLayout(false);
            this.tc_simulation.ResumeLayout(false);
            this.tc_output_vars.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbtestrender)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.TextBox tb_vars;
        private System.Windows.Forms.TextBox tb_rules;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_validate_fl;
        private System.Windows.Forms.Button btn_run_simulation;
        private System.Windows.Forms.Label infos;
        private System.Windows.Forms.TabControl tc_input_vars;
        private System.Windows.Forms.TabPage tabPage1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.TextBox tb_resolution;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_resolution;
        private System.Windows.Forms.TabControl tc_simulation;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabControl tc_output_vars;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pbtestrender;
    }
}

