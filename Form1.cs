using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

/*
 * 
 *      le fait est que dans la DLL (fichier flwin32.c)
 *      l'objet t_fuzzylogic - objet conteneur des données - est global
 *      ce qui induit qu'une seule instance est créée,
 *      d'où la nécessité d'appeler
 *          - fl_engine_init();
 *          - fl_engine_free();
 *      à chaque fois que l'on définit de nouvelles variables/regles
 * 
 *      à voir pour instancier cet objet plusieurs fois,
 *      et dès lors recuperer le pointeur sur l'objet via C# en retour de fonction,
 *      permettant de créer des noeuds d'objets ( voir cognitive map avec fuzzylogic inference )
 * 
 */

/*
 *      declaration des pointeurs sur fonctions pour le code " non managé "
 *      => les methodes de callback dans la DLL 
 */
[UnmanagedFunctionPointer(CallingConvention.Cdecl) ]
public delegate void callback_fl_result(string name, int sizename, [In]double value);

[UnmanagedFunctionPointer(CallingConvention.Cdecl) ]
public delegate void callback_fl_get_var(string vartype, string varname, string subsetwave, string subsetname, IntPtr ranges, IntPtr data);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void callback_fl_get_rules_mb(int idx, string name, string subset, double mb);



namespace FuzzyLogic
{
    //https://docs.microsoft.com/en-us/dotnet/framework/interop/marshalling-different-types-of-arrays?redirectedfrom=MSDN

    public partial class Form1 : Form
    {

        public static double Normalize(double val, double valmin, double valmax, double min, double max)
        {
            return (((val - valmin) / (valmax - valmin)) * (max - min)) + min;
        }

        [DllImport("FLDLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int fl_engine_init();

        [DllImport("FLDLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int fl_engine_test([In, Out] string[] stringArray, int s);

        [DllImport("FLDLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int fl_engine_variable_adds([In, Out] string[] stringArray, int s);

        [DllImport("FLDLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int fl_engine_rules_adds([In, Out] string[] stringArray, int s);

        [DllImport("FLDLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int fl_engine_variable_add([In, Out] string str, int s);

        [DllImport("FLDLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int fl_engine_rules_add([In, Out] string str, int s);

        [DllImport("FLDLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int fl_engine_fuzzyfy_var_by_name([In, Out] string str, double v);

        [DllImport("FLDLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int fl_engine_get_results( callback_fl_result r);

        [DllImport("FLDLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int fl_engine_free();

        [DllImport("FLDLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int fl_engine_get_vars_list(callback_fl_get_var r);

        [DllImport("FLDLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern double fl_engine_fuzzyfy_set([In, Out] string varname, [In, Out] string setname, double v);

        [DllImport("FLDLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern double fl_engine_get_rules_membership(callback_fl_get_rules_mb r);


        public Form1()
        {
            InitializeComponent();
          //  testTabControlgraphics();

            chart1.MouseClick += chart1_Click;

            initTestFL();

            //testPlot3D();
        }
        // pour initialiser le lancement de l'interface graphique & effectuer quelques simulations
        public void initTestFL()
        {
            string[] vars = { "I diff BS high 0,1 # ES low 0,3" ,
                
                "O test  TSK high diff * 2 # low 42"
            
            };
            string[] rules = {
                "diff{high} :test{high}",
                "diff{low} :test{low}"
               
            };
            tb_vars.Text =  String.Join(Environment.NewLine, vars);
            tb_rules.Text = String.Join(Environment.NewLine, rules);
        }
        Color[] arrColor = new Color[9] { Color.Blue, Color.Violet, Color.Red, Color.Orange, Color.YellowGreen, Color.Green, Color.Cyan, Color.BlueViolet, Color.Pink };
        public class subset
        {
            public string subsetname { get; set; }
            public string subsetwave { get; set; }
            public double[] range = new double[4];
            public double[] data = new double[4];
            public List<double> valuesfuzzed = new List<double>();
            public double degreeMembership { get; set; } = 0;
            public Series serie { get; set; } = null;

        }
        public class resultFL
        {
            public string name { get; set; }
            public double result { get; set; }
            public string vartype { get; set; }
            public List<subset> subsets = new List<subset>();
            public void fuzzyfy_sets(double resolution)
            {
                getMinAndMax();
                double i = min;
                while (i < max)
                {
                    foreach (subset s in subsets)
                    {
                        double r = fl_engine_fuzzyfy_set(name, s.subsetname, i);
                        s.valuesfuzzed.Add(r);
                    }
                    i += (resolution < 0 ? 1 : resolution);
                }
            }
            public double min { get; set; }
            public double max { get; set; }
            public void getMinAndMax()
            {
                min = +10000;
                max = -min;
                foreach (subset s in subsets)
                {
                    if (s.data[1] < min) min = s.data[1];
                    if (s.data[2] > max) max = s.data[2];
                    s.valuesfuzzed.Clear();
                }
            }
            public double settingSimulationValue { get; set; } = 0;

            public double degreeOfvar { get; set; } = 0;
            public Series serie { get; set; } = null;

            public double getVarDegree()
            {
                degreeOfvar = 0;
                foreach (subset s in subsets)
                {
                        degreeOfvar += s.degreeMembership ;
                }
                degreeOfvar = Normalize(degreeOfvar, 0 , subsets.Count, 0, 1);
                return degreeOfvar;
            }
            public void setSubsetMembershipDegree(string subsetname, double val)
            {
              
                foreach(subset s in subsets)
                {
                    if (subsetname == s.subsetname)
                    {

                        s.degreeMembership = val;
                        break;
                    }
                        
                }
            }
        }
        public resultFL getVarInListVarsByName(string name)
        {
            foreach (resultFL r in listvars)
            {
                if (name == r.name)
                    return r;
            }
            return null;
        }
  
        // classe pour generer le slider avec nombre floattant
        class FloatTrackBar : TrackBar
        {
            private float precision = 0.01f;

            public float Precision
            {
                get { return precision; }
                set
                {
                    precision = value;
                    // todo: update the 5 properties below
                }
            }
            public new float LargeChange
            { get { return base.LargeChange * precision; } set { base.LargeChange = (int)(value / precision); } }
            public new float Maximum
            { get { return base.Maximum * precision; } set { base.Maximum = (int)(value / precision); } }
            public new float Minimum
            { get { return base.Minimum * precision; } set { base.Minimum = (int)(value / precision); } }
            public new float SmallChange
            { get { return base.SmallChange * precision; } set { base.SmallChange = (int)(value / precision); } }
            public new float Value
            { get { return base.Value * precision; } set { base.Value = (int)(value / precision); } }
        }
        public static List<resultFL> listvalues = new List<resultFL>();
        public static List<resultFL> listvars = new List<resultFL>();

        bool flengine_loaded = false;
        bool changedSetByHand = false;

        double g_resolution = 0.1;

        ToolTip tt = null;
        Point tl = Point.Empty;

        public class RulesMembershipRendering 
        {
            PictureBox pb;
            Bitmap mbRender;
            Graphics g;
            Font font = new Font("Verdana", 8);
            int width = 0;
            int height = 0;
            int hweight = 0;
            int count = 0;

            public RulesMembershipRendering(int w, int h, int ct)
            {
                width = w;
                height = h ;
                hweight = h /ct;
                count = ct;
                mbRender = new Bitmap(width, height);
                pb = new PictureBox();
                pb.Size = new Size(width, height);
                Console.WriteLine(width + " " + height + " " + hweight + " " + ct);
            }
            public PictureBox getPictureBox() { return pb; }
            public void setLocation (Point p)
            {
                Point np = p;
                np.X = p.X - 50;
                np.Y = p.Y;
                pb.Location = np;
            }
            public void update()
            {
                g = Graphics.FromImage(mbRender);
                g.FillRectangle(Brushes.White, 0, 0, width, height);
                pb.Image = mbRender;
               // g.Dispose();
            }
            
            public void refresh(int idx, double alpha)
            {
                int toph = hweight * idx;

                int a = (int)Normalize(alpha, 0, 1, 0, 255);

                SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(a, 0, 0, 0));

                g.FillRectangle(semiTransBrush, 0, toph, width,  hweight );

                semiTransBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
                g.DrawString(string.Format("{0:0.000}", alpha), font, semiTransBrush, new RectangleF(0, toph, width, hweight));

                pb.Image = mbRender;
             //   g.Dispose();
            }
        }

        // rendu3D, ne fonctionne pas, revoir Skia (moteur rendu)
        public void testPlot3D()
        {
            PictureBox pb;
            Bitmap mbRender;
            Graphics g;
            Point3D p3;
            Random r;

            pb = pbtestrender;
            mbRender = new Bitmap(pb.Width, pb.Height);
     
            r = new Random();
            int i = 0;
            while (i < 10)
            {
                p3 = new Point3D(i, i, r.Next() );
                i++;
            }
            
            g = Graphics.FromImage(mbRender);
            pb.Image = mbRender;
            //g.DrawPath();
        }

        public static RulesMembershipRendering rmr = null;


        public void testTabControlgraphics()
        {
            Color c;
            Pen p;
            Graphics g;
            PictureBox pb;
            Bitmap bm;
            TabPage tp;
            int i = 1;

            tc_input_vars.TabPages.Clear();
            tc_output_vars.TabPages.Clear();

            while (i < 4)
            {
                tp = new TabPage();
                tp.Text = "mytab " + i.ToString();

                pb = new PictureBox();
                bm = new Bitmap(200,200);

                g = Graphics.FromImage(bm);
                
                c = Color.AliceBlue;

                p = new Pen(i == 1 ? Brushes.Blue : Brushes.Red);

                g.DrawLine(p, 0, 0, 200, 200);
                //g.FillRectangle(Brushes.Blue, 0, 0, 200, 200);

                /*
                GraphicsPath gp;

                gp = new GraphicsPath();

                g.DrawPath(p, gp);
                  */

                pb.Image = bm;
                g.Dispose();
                tp.Controls.Add(pb);
                tc_input_vars.TabPages.Add(tp);
                i++;
            }
        }
   
        public void testTabControlSerie()
        {
            tc_input_vars.TabPages.Clear();

            TabPage tp;
            Chart chart;
            Series s;
            int i = 1;
            while (i < 4)
            {
                tp = new TabPage();
                tp.Text = "mytab " + i.ToString();
   
                chart = new Chart();
                // chart.Size = new System.Drawing.Size(200, 200);
                // chart.Location = new System.Drawing.Point(100, 0);
                // chart.Text = "chart" + i.ToString();
                // chart.Name ="chartname_" + i.ToString();
                chart.Titles.Add("Test " + i.ToString());
                tp.Controls.Add(chart);

                tc_input_vars.TabPages.Add(tp);

                string seriename = "serie_" + i.ToString();
                s = chart.Series.Add(seriename);
                //s.Name = seriename;
                s.LegendText = "legend " + "serie " + i.ToString();
                s.ChartType = SeriesChartType.Line;
                chart.Series[s.Name].Points.AddXY(0, i);
                chart.Series[s.Name].Points.AddXY(1, i * 2);

                ChartArea ca = new ChartArea();
                chart.ChartAreas.Add(ca);

                Legend l = new Legend();
               
                chart.Legends.Add(l);
                i++;
            }
        }


        // creation interface graphique des variables d'entrées & de sorties
        // avec onglets
        // les subsets associés aux variables et leur representation
        public void createTabPageSubset(resultFL var, double resolution)
        {
            TabPage tp;
            Chart chart;
            Series s;
            FloatTrackBar ftb;
            Point p;
            TextBox tb;
            double i;
            int idx;
            string title = (var.vartype == "I" ? "input" : "output") + " - " + var.subsets[0].valuesfuzzed.Count + " samples in range  [ " + var.min + " ; " + var.max + " ]";

            tp = new TabPage();
            tp.Text = var.name;
            tp.Name = var.name;

            chart = new Chart();
            
            chart.Titles.Add(title);

            if (var.vartype == "I")
            {
                tb = new TextBox();
                tb.Name = var.name;
                tb.Text = "v";
                tb.KeyDown += new System.Windows.Forms.KeyEventHandler( this.textboxingInputValue);

                ftb = new FloatTrackBar();
                ftb.Name = var.name;
                ftb.Minimum = (float)var.min;
                ftb.Maximum = (float)var.max;
                ftb.Size = new Size(tc_input_vars.Size.Width - tb.Size.Width, 30);
                ftb.ValueChanged += slidingInputValue;
                p = ftb.Location;
                p.X += tb.Width;
                ftb.Location = p;
                tb.Text = ftb.Value.ToString();

                chart.Size = new Size(tc_input_vars.Size.Width, tc_input_vars.Size.Height - 26 - ftb.Size.Height);
                p = chart.Location;
                p.Y += ftb.Size.Height;
                chart.Location = p;

                tp.Controls.Add(tb);
                tp.Controls.Add(ftb);
                tc_input_vars.TabPages.Add(tp);
            }
            else
            {
                chart.Size = new Size(tc_output_vars.Size.Width, tc_output_vars.Size.Height - 26 );
                tc_output_vars.TabPages.Add(tp);
            }
            tp.Controls.Add(chart);

            string seriename;
            idx = 0;
            foreach (subset subs in var.subsets)
            {
                seriename =  var.name + "_"+ subs.subsetname; // do not modifiy , sensitive case to retrieve when click on tab
                s = chart.Series.Add(seriename);
                s.LegendText = subs.subsetname;
                s.ChartType = SeriesChartType.Area;
                Color c = arrColor[idx % 9];
                idx++;
                s.Color = Color.FromArgb(96, c);

                i = var.min;
                foreach (double d in subs.valuesfuzzed)
                {
                    chart.Series[s.Name].Points.AddXY(i, d);
                    
                    i += resolution;
                }
                subs.serie = s;
            }

            ChartArea ca = new ChartArea();
            chart.ChartAreas.Add(ca);

            Legend l = new Legend();
            chart.Legends.Add(l);

            chart.MouseClick += chart1_Click;
            chart.MouseMove += chart1_MouseMove;
        }

        // creation interface graphique pour la simulation associée à chacune des variables d'entrées
        public void createTabPageSimulationVars(resultFL var)
        {
            TabPage tp;
            TextBox tb;
            Button btn;

            tp = new TabPage();
            tp.Text = var.name;
            tp.Name = "tp_" + var.name;

            tb = new TextBox();
            tb.Text = "enter [start, end, resolution] to simulate " + var.name + " variable";
            tb.Size = new Size(tc_simulation.Size.Width, 20);
            tb.Name = "Field_" + var.name;
            btn = new Button();
            btn.Text = "run";
            btn.Name = var.name;
            btn.Location = new System.Drawing.Point(tb.Location.X, tb.Size.Height);

            btn.Click += new System.EventHandler(this.tab_btn_simulation_run);

            tp.Controls.Add(tb);
            tp.Controls.Add(btn);
            tc_simulation.TabPages.Add(tp);
        }

        // quand usager entre la valeur de la variable
        //  attention aux nombres flottants et la conversion a l'usage de la methode  Convert.ToDouble, 
        //  utiliser une ',' virgule et non pas un point '.' 
        //  "0,1"   => ok
        //  "0.1"   => ne fonctionne pas
        private void textboxingInputValue(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            
            TextBox tb = sender as TextBox;
            FloatTrackBar ftb;
            double value;

            try {
                value = Convert.ToDouble(tb.Text);
            } catch(Exception err)
            {
                return;
            }
            foreach (TabPage tp in tc_input_vars.TabPages)
            {
                foreach (Control c in tp.Controls)
                {
                    if (c is FloatTrackBar && c.Name == tb.Name)
                    {
                        ftb = c as FloatTrackBar;
                        
                        Console.WriteLine(ftb.Name + " " + ftb.Value);
                        if (value >= ftb.Minimum && value <= ftb.Maximum)
                            ftb.Value = (float)value;
                        if (value < ftb.Minimum)
                            ftb.Value = ftb.Minimum;
                        if (value > ftb.Maximum)
                            ftb.Value = ftb.Maximum;

                     //   slidingInputValue(ftb, null);
                       // ftb.Refresh();
                    }
                    else if (c is FloatTrackBar && c.Name == tb.Name)
                    {

                    }
                }
            }
           
          //  changingInputVarByHand();
        }

        // quand usager bouge le curseur associée a une valeur d'une variable
        // 
        private void slidingInputValue(object sender, EventArgs e)
        {
            FloatTrackBar tb = sender as FloatTrackBar;
            FloatTrackBar ftb;
            TextBox displaytb;
            resultFL r;

            r = getVarInListVarsByName(tb.Name);
            r.settingSimulationValue = (double)tb.Value;
            foreach (TabPage tp in tc_input_vars.TabPages)
            {
                foreach (Control c in tp.Controls)
                {
                    if (  c is FloatTrackBar && c.Name != tb.Name)
                    {
                        ftb = c as FloatTrackBar;
                        r = getVarInListVarsByName(ftb.Name);
                        r.settingSimulationValue = ftb.Value;
                    }
                    else if (c is TextBox && c.Name == tb.Name)
                    {
                        displaytb = c as TextBox;
                        displaytb.Text = tb.Value.ToString();
                    }
                }
            }
            changingInputVarByHand();
        }
        private void changingInputVarByHand()
        {
            resultFL item;
            if (changedSetByHand == false)
            {
                listvalues.Clear();
                changedSetByHand = true;
            }
            foreach (resultFL r in listvars)
            {
                if (r.vartype == "I")
                {
                    //fl_engine_fuzzyfy_var_by_name(r.name, r.settingSimulationValue);
                    foreach (subset subs in r.subsets)
                    {
                        subs.degreeMembership = fl_engine_fuzzyfy_set(r.name, subs.subsetname, r.settingSimulationValue);
                    }
                    item = new resultFL();
                    item.name = r.name;
                    item.result = r.settingSimulationValue;
                    listvalues.Add(item);
                }
            }

            fl_engine_get_results(getfl);
            rmr.update();
            fl_engine_get_rules_membership(getrulesmb);
            display_membership_degree_of_vars();
            display_full_simulation();
            if (listvalues.Count > 100)
            {
                foreach (resultFL r in listvars)
                {
                    listvalues.RemoveAt(0);
                    chart1.Series[r.name].Points.RemoveAt(0);
                }
            }  
        }



        //  creer le gradient de couleur [blanc  ->  noir] 
        //  fonction du pourcentage d'appartenance du subset
        public void display_membership_degree_of_vars()
        {
            double alpha;
            double mb;
            int idx;
            Chart ch;
            Series s;
            Color c;
            string seriename;
            TabControl tc;

            foreach (resultFL r in listvars)
            {
                foreach (subset subs in r.subsets)
                {
                    mb = 0;
                    alpha = 0;
                    seriename = r.name + "_" + subs.subsetname; //  sensitive case to retrieve when click on tab
                    idx = r.subsets.IndexOf(subs);
                    c = arrColor[idx % 9];
                    tc = (r.vartype == "I") ? tc_input_vars : tc_output_vars;

                    mb = subs.degreeMembership;

                    alpha = Normalize(mb, 0, 1, 25, 200);
                    alpha = alpha < 0 ? 0 : (alpha > 200 ? 200 : alpha);
                    subs.serie.Color = Color.FromArgb((int)alpha, c);
                    /*
                    foreach (TabPage tp in tc.TabPages)
                    {
                        if (tp.Name == r.name)
                        {
                            foreach (Control control in tp.Controls)
                            {
                                if (control.GetType() == typeof(Chart))
                                {
                                    ch = (control as Chart);
                                    s = ch.Series[seriename];

                                    mb = subs.degreeMembership;

                                    alpha = Normalize(mb, 0, 1, 96, 200);
                                    s.Color = Color.FromArgb((int)alpha, c);
                                }

                            }
                        }
                    }
                    */
                }
            }
        }


        // fonction de callback ( appelée par la DLL )
        // etablit le degrée d'appartenance d'un sous set
        public static void getrulesmb(int idx, string name, string subset, double mb)
        {
         //   Console.WriteLine(idx + " " + name + " " + subset + " " + mb);
            rmr.refresh(idx, mb);
            foreach (resultFL r in listvars)
            {
                if (r.name == name)
                    r.setSubsetMembershipDegree(subset,mb);
            }
        }

        private void tab_btn_simulation_run(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            TabPage tp;
            TextBox tb;
            Control c;
            int k = 0;
            int len = tc_simulation.TabPages.Count;
            int i = 0;

            Console.WriteLine("btn click " + btn.Name);

            while (i < len)
            {
                tp = tc_simulation.TabPages[i];
                k = 0;
                while (k < tp.Controls.Count)
                {
                    c = tp.Controls[k];
                    string []sp = c.Name.Split('_');
                    if (sp.Length == 2 && sp[1] == (btn.Name) && sp[0] == ("Field"))
                    {
                        tb = c as TextBox;
                        produceSimulation(tb.Text, btn.Name);
                        Console.WriteLine(c.Name + " " + tb.Text);
                        return;
                    }
                    k++;
                }
                i++;
            }
        }

        // fonction pour tester les differents appels
        // ajout de variables
        // ajout de regles
        // obtention des variables & enregistrement dans la liste
        public void test()
        { 
            int err;
            err = fl_engine_init();
            Console.WriteLine("init fl lib : " + err);

            string[] vars = { "I diff BS high 0,1 # ES vhigh 0,3 # ES vfan 1.5,1.6 # P hfan 0.3,0.4,1,1.6 # P lfan -10,-9.9,0.3,0.4" ,
                "O heater TR cut -1, 0, 1 #  TR high 254,255,256",
                "O fanheater TR high 127, 128, 129 # TR full 254,255,256",
                "O extractor TR low 10, 12, 14 # TR high 30,32,34"
            };
            string[] rules = {
                "diff{high} :heater{high}",
                "diff{vhigh} :heater{cut},extractor{high}",
                "diff{vfan} | diff{lfan} : fanheater{full}",
                "diff{hfan} :fanheater{high}",
                "diff{lfan} :extractor{low}"
            };

            err = fl_engine_test(vars, vars.Length);
            Console.WriteLine("fl_engine_test : " + err);
            
           //  err = fl_engine_variable_adds(vars, vars.Length);
            //   Console.WriteLine("fl_engine_variable_add : " + err);
           
            err = fl_engine_variable_add("I diff BS high 0,1 # ES vhigh 0,3 # ES vfan 1.5,1.6 # P hfan 0.3,0.4,1,1.6 # P lfan -10,-9.9,0.3,0.4", 0);
            Console.WriteLine("fl_engine_variable_add : " + err);
            err = fl_engine_variable_add("O heater TR cut -1, 0, 1 #  TR high 254,255,256", 0);
            Console.WriteLine("fl_engine_variable_add : " + err);
            err = fl_engine_variable_add("O fanheater TR high 127, 128, 129 # TR full 254,255,256", 0);
            Console.WriteLine("fl_engine_variable_add : " + err);
            err = fl_engine_variable_add("O extractor TR low 10, 12, 14 # TR high 30,32,34 # TSK test 5+5+diff", 0);
            Console.WriteLine("fl_engine_variable_add : " + err);
            
            //  err = fl_engine_rules_adds(rules, rules.Length);
            //          Console.WriteLine("fl_engine_rules_add : " + err);
            
            // err = fl_engine_rules_add("diff{high} : heater{high} , fanheater{high} , extractor{low}", 0);
            err = fl_engine_rules_add("diff{high} : heater{high}", 0);
            Console.WriteLine("fl_engine_rules_add : " + err);
            err = fl_engine_rules_add("diff{vhigh} : heater{cut},extractor{high}", 0);
            Console.WriteLine("fl_engine_rules_add : " + err);
            err = fl_engine_rules_add("diff{vfan} | diff{lfan} : fanheater{full}", 0);
            Console.WriteLine("fl_engine_rules_add : " + err);
            err = fl_engine_rules_add("diff{hfan} : fanheater{high}", 0);
            Console.WriteLine("fl_engine_rules_add : " + err);
            err = fl_engine_rules_add("diff{lfan} : extractor{low}", 0);
            Console.WriteLine("fl_engine_rules_add : " + err);
     
         //   fl_engine_fuzzyfy_var_by_name("diff", 0.5);
          
        //    fl_engine_get_results(getfl);

            fl_engine_get_vars_list(getflvars);

            run();


            fl_engine_free();
        }


        // fonction de test au debut du dev
        // methode de callback
        public void run()
        {
            resultFL item;
            double i = 0;
            double v = -10;

            listvalues = new List<resultFL>();
            while (i < 20)
            {
                v++;
                //   Console.WriteLine("==================== " + i + " ==== " + v);
                fl_engine_fuzzyfy_var_by_name("diff", v);

                item = new resultFL();
                item.name = "diff";
                item.result = v;
                listvalues.Add(item);

                fl_engine_get_results(getfl);
                //fl_engine_get_vars_list(getflvars);
                i += 0.1;
            }
            display_full_simulation();
        }

        // fonction de test au debut du dev
        // methode employée en callback ( appelée par la DLL ) pour recuperer les données de la simulation
        public static void getflvars(string vartype, string varname, string subsetwave, string subsetname, IntPtr ranges, IntPtr datas)
        {
            Console.WriteLine("vartype:" + vartype + " varname : " + varname + " subsetwave :  " + subsetwave + " subsetname : " + subsetname);
            int i = 0;
            double[] range = new double[4];

            Marshal.Copy(ranges, range, 0, 4);     
            while (i < 4)
            {
                Console.WriteLine(i + " = " + range[i]);
                i++;
            }
            /*
            0:value     <= if vartype == INPUT this is set to the degree , if vartype == OUTPUT it does get the value of expr TSK
            1:min
            2:max
            3:center
            */
            double[] data = new double[4];
            Marshal.Copy(datas, data, 0, 4);
            i = 0;
            while (i < 4)
            {
                Console.WriteLine(i + " = " + data[i]);
                i++;
            }
        }
        // fonction de test au debut du dev
        // methode de callback
        public static void getfl(string vname, int sizename, double v)
        {
                resultFL item = new resultFL();
                item.name = vname;
                item.result = v;
                listvalues.Add(item);
            //if (FLAG)
             //  Console.WriteLine("name : " + vname + " = " + v);
        }
        


        // quand on produit une simulation pour une variable donnée
        // le champ de texte se remplit tel que 
        //  [   nombre_au_commencement_simul      nombre_fin_simul    resolution  ]     
        //  soit 3 parametres séparés par un espace,
        //  attention aux nombres flottants et la conversion a l'usage de la methode  Convert.ToDouble, 
        //  utiliser une ',' virgule et non pas un point '.' 
        //  "0,1"   => ok
        //  "0.1"   => ne fonctionne pas
        public void produceSimulation(string txt, string invar)
        {
            if (flengine_loaded == false) return;

            string []data = txt.Split(' ');

            if (data.Length != 3) return;

            double i;
            double len;
            double resolution;
            resultFL item;

            try
            {
                i = Convert.ToDouble(data[0]);
                len = Convert.ToDouble(data[1]);
                resolution = Convert.ToDouble(data[2]);
            }
            catch(Exception e)
            {
                return;
            }

            //   tc_input_vars.TabPages.Clear();
            //   tc_output_vars.TabPages.Clear();

            listvalues.Clear();
            changedSetByHand = false;
            /*
            foreach (resultFL r in listvars)
            {
                r.fuzzyfy_sets(g_resolution);
                createTabPageSubset(r, g_resolution);
            }
            */
            while (i < len)
            {
                foreach (resultFL r in listvars)
                {
                    if (r.vartype == "I")
                    {
                        if (r.name == invar)
                        {
                            fl_engine_fuzzyfy_var_by_name(r.name, i);
                        }
                        else
                        {
                            fl_engine_fuzzyfy_var_by_name(r.name, r.settingSimulationValue);
                        }
                        item = new resultFL();
                        item.name = r.name;
                        item.result = (r.name == invar) ? i : r.settingSimulationValue;
                        listvalues.Add(item);
                    }
                }
                fl_engine_get_results(getfl);
                i += resolution;
            }
            display_full_simulation();
        }

        // commencement d'une simulation avec initialisation 
        // du moteur fl
        // ajout des variables & des regles
        private int fl_preambule()
        {
            int err;

            listvars.Clear();
            listvalues.Clear();

            err = fl_engine_init();
            if (err != 0)
            {
                flengine_loaded = false;
                return -42;
            }
            Console.WriteLine("init fl lib : " + err);

            String[] vars = tb_vars.Text.Split('\n');
            err = fl_engine_variable_adds(vars, vars.Length);
            Console.WriteLine("error in vars = " + err);
            if (err != 0)
            {
                fl_engine_free();
                flengine_loaded = false;
                return -1;
            }

            String[] rules = tb_rules.Text.Split('\n');
            err = fl_engine_rules_adds(rules, rules.Length);
            Console.WriteLine("error in rules = " + err);
            if (err != 0)
            {
                fl_engine_free();
                flengine_loaded = false;
                return -2;
            }
            flengine_loaded = true;
            fl_engine_get_vars_list(getAndFillVars);

            createRuleMembershipVisualisation();

            return 0;
        }


        // creer visuel - gradient & pourcentage - associés au déclenchement des règles
        // ( qui s'affiche quand on deplace le curseur d'entrée d'une variable )
        private void createRuleMembershipVisualisation()
        {
            Size size = TextRenderer.MeasureText(tb_rules.Text, tb_rules.Font);

            tb_rules.Height = size.Height + (size.Height / tb_rules.Lines.Length - 1);
            if (rmr != null)
            {
                this.Controls.Remove(rmr.getPictureBox());
            }
            rmr = new RulesMembershipRendering(40, tb_rules.Height, tb_rules.Lines.Length);
            rmr.setLocation(tb_rules.Location);
            rmr.update();
            PictureBox pb = rmr.getPictureBox();
            this.Controls.Add(pb);
        }

        private void btn_run_simulation_Click(object sender, EventArgs e)
        {
            baseSimulation();
        }

        // creer la simulation, fuzzify les variables
        // dans le domaine  [ min , max ] correspondant au minimum et au maximum des subsets
        // s1   ........[A.....B]...
        // s2   .....[C.....D]......
        // min = C
        // max = B 
        private void baseSimulation()
        {
            resultFL item;
            double min = 100000;
            double max = -min;

            listvalues.Clear();
            changedSetByHand = false;

            foreach (resultFL r in listvars)
            {
                if (r.vartype == "I")
                {
                    foreach (subset s in r.subsets)
                    {
                        if (s.data[1] < min) min = s.data[1];
                        if (s.data[2] > max) max = s.data[2];
                    }
                }
            }
            infos.Text = "Inputs domain [ " + min + " , " + max + " ]";

            double resolution = g_resolution;
            double i = min;
            while (i < max)
            {
                foreach (resultFL r in listvars)
                {
                    if (r.vartype == "I")
                    {
                        fl_engine_fuzzyfy_var_by_name(r.name, i);
                        item = new resultFL();
                        item.name = r.name;
                        item.result = i;
                        listvalues.Add(item);
                    }
                }
                fl_engine_get_results(getfl);
                i += resolution;
            }
            display_full_simulation();
        }

        // quand usager valide parametres {vars, rules}
        // creer l'interface graphique
        private void btn_validate_fl_Click(object sender, EventArgs e)
        {
            int err;
            Size size;

            if (flengine_loaded == true)
            {
                fl_engine_free();
            }
            err = fl_preambule();
            if (err != 0)
            {
                flengine_loaded = false;
                return;
            }
            tc_simulation.TabPages.Clear();
            tc_input_vars.TabPages.Clear();
            tc_output_vars.TabPages.Clear();

            foreach (resultFL r in listvars)
            {
                r.fuzzyfy_sets(g_resolution);
                createTabPageSubset(r, g_resolution);
                if (r.vartype == "I")
                    createTabPageSimulationVars(r);
            }
            baseSimulation();

            //resize the box
            size = TextRenderer.MeasureText(tb_rules.Text, tb_rules.Font);
            tb_rules.Height = size.Height + (size.Height / tb_rules.Lines.Length ) * 2;
        }

        // fonction de callback ( appelée par la dll ) , parametre de la fonction fl_engine_get_vars_list
        // ajoute à {listvars} le nouveau resultat de la simulation
        // ainsi que les subsets associés   
        public static void getAndFillVars(string vartype, string varname, string subsetwave, string subsetname, IntPtr ranges, IntPtr datas)
        {
            resultFL gelem = null;
            List<subset> subsets;
            subset newSubset;

            foreach (resultFL r in listvars)
            {
                if (r.name == varname)
                {
                    gelem = r;
                    break;
                }
            }
            if (gelem == null)
            {
                gelem = new resultFL();
                gelem.name = varname;
                gelem.vartype = vartype;
                listvars.Add(gelem);
            }
            subsets = gelem.subsets;
            foreach (subset s in subsets)
            {
                if (s.subsetname == subsetname)
                    return;
            }
            newSubset = new subset();
            newSubset.subsetname = subsetname;
            newSubset.subsetwave = subsetwave;
   
            Marshal.Copy(ranges, newSubset.range, 0, 4);
            Marshal.Copy(datas, newSubset.data, 0, 4);

            subsets.Add(newSubset);
        }
        
        // affichage sur le graph global
        // de l'ensemble des resultats associées a la simulation
        private void display_full_simulation()
        {
            Series s;
            int i;

            display_clear();
            i = 0;

            foreach (resultFL r in listvars)
            {
                s = chart1.Series.FindByName(r.name);
                if (s == null)
                {
                    s = chart1.Series.Add(r.name);
                    s.ChartType = SeriesChartType.Line;
                    
                    Color c = arrColor[i % 9];
                    s.Color = c; // Color.FromArgb(96, c);
                    s.BorderWidth = 3;
               
                    r.serie = s;
                    /*
                    r.getVarDegree();
                    double alpha = Normalize(r.degreeOfvar, 0, 1, 100, 255);
                    r.serie.Color = Color.FromArgb((int)alpha, c);
                    */
                    i++;
                }
            }
            i = 0;
            foreach (resultFL r in listvalues)
            {  
                chart1.Series[r.name].Points.AddY( r.result);
            }
        }


        // affichage meta-données quand le curseur est au-dessus d'une courbe
        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            Chart cchart = sender as Chart;

            if (tt == null) tt = new ToolTip();

            ChartArea ca = cchart.ChartAreas[0];

            if (InnerPlotPositionClientRectangle(cchart, ca).Contains(e.Location))
            {
                Axis ax = ca.AxisX;
                Axis ay = ca.AxisY;
                double x = ax.PixelPositionToValue(e.X);
                double y = ay.PixelPositionToValue(e.Y);

                if (e.Location != tl)
                {
                    int idx = -1;
                    string data = string.Format("{0:0.00} ; {1:0.00}\n", x, y);

                    if (cchart == chart1)
                    {
                        while (++idx < cchart.Series.Count)
                        {
                            Series s = cchart.Series[idx];
                            int ix;
                            //ix  = (int)Normalize(x, 0, cchart.Width, 0, s.Points.Count);
                            ix = (int)(x);
                            if (ix < 0)
                                ix = 0;
                            if (ix >= s.Points.Count)
                                ix = s.Points.Count - 1;
                            if (ix > -1)
                            {
                                var dpx = s.Points[ix];
                                data += string.Format("{0} {1:0.00}\n", s.Name, dpx.YValues[0]);
                            }
                        }
                    }
                    else
                    {
                        // we are on fuzzysubset chart!
                        while (++idx < cchart.Series.Count)
                        {
                            Series s = cchart.Series[idx];
                            int ix = (int)(x);
                            string subsetname = s.Name.Split('_')[1];
                            string varname = s.Name.Split('_')[0];

                            foreach (resultFL r in listvars)
                            {
                                if (varname == (r.name))
                                {
                                    foreach (subset sub in r.subsets)
                                    {
                                        if (subsetname == (sub.subsetname))
                                        {
                                            ix = (int)Normalize(x, r.min, r.max, 0, s.Points.Count);
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            if (ix < 0)
                                ix = 0;
                            if (ix >= s.Points.Count)
                                ix = s.Points.Count - 1;
                            if (ix > -1)
                            {
                                var dpx = s.Points[ix];
                                data += string.Format("{0} {1:0.00}\n", subsetname, dpx.YValues[0]);
                            }
                        }
                    }
                    tt.SetToolTip(cchart, data);
                }
                tl = e.Location;
            }
            else tt.Hide(cchart);
        }

        RectangleF ChartAreaClientRectangle(Chart chart, ChartArea CA)
        {
            RectangleF CAR = CA.Position.ToRectangleF();
            float pw = chart.ClientSize.Width / 100f;
            float ph = chart.ClientSize.Height / 100f;
            return new RectangleF(pw * CAR.X, ph * CAR.Y, pw * CAR.Width, ph * CAR.Height);
        }

        RectangleF InnerPlotPositionClientRectangle(Chart chart, ChartArea CA)
        {
            RectangleF IPP = CA.InnerPlotPosition.ToRectangleF();
            RectangleF CArp = ChartAreaClientRectangle(chart, CA);

            float pw = CArp.Width / 100f;
            float ph = CArp.Height / 100f;

            return new RectangleF(CArp.X + pw * IPP.X, CArp.Y + ph * IPP.Y,
                                    pw * IPP.Width, ph * IPP.Height);
        }

        // quand on click sur le nom d'un preset dans la légende
        // affiche / masque la courbe associée 
        private void chart1_Click(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Chart c = sender as Chart;
            HitTestResult result = c.HitTest(e.X, e.Y);
            if (result != null && result.Object != null)
            {
                LegendItem legendItem;
                Series getserie;
                Series s;
                Color co;
                int i = 0;

                // When user hits the LegendItem
                if (result.Object is LegendItem)
                {
                    // Legend item result
                    legendItem = (LegendItem)result.Object;  
                    getserie = c.Series.FindByName(legendItem.SeriesName);
                    i = 0;
                    while (i < c.Series.Count)
                    {
                        if (c.Series[i].Name == getserie.Name) break;
                        i++;
                    }
                    s = c.Series[i];
                    if (s.Color == Color.Transparent)
                    {
                        co = arrColor[i % 9];
                        s.Color = Color.FromArgb(96, co);
                    }
                    else
                        s.Color = Color.Transparent;
                }
            }
        }


        // efface les series des graphics (les barres representant les données)
        private void display_clear()
        {
            int i = 0;
            int len = chart1.Series.Count;
            while (i < len)
            {
                chart1.Series[i].Points.Clear();
                chart1.Series[i].Dispose();
                i++;
            }
            chart1.Series.Clear();
        }

        // change la resolution des graphics
        // pour disposer d'une resolution + ou - fine
        private void btn_resolution_Click(object sender, EventArgs e)
        {
            try
            {
                g_resolution = Convert.ToDouble(tb_resolution.Text);
                g_resolution = g_resolution < 0 ? 1 : g_resolution;
            }
            catch(Exception ex) { }
        }

        // efface les series des graphics (les barres representant les données)
        // ainsi que les valeurs enregistrées issuent de la simulation
        private void button1_Click(object sender, EventArgs e)
        {
            int i = 0;
            int len = chart1.Series.Count;
            while (i < len)
            {
                chart1.Series[i].Points.Clear();
                i++;
            }
            listvalues.Clear();
        }
    }
}
