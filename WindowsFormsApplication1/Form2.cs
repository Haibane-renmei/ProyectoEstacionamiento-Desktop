using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Vision.Motion;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System.Drawing.Imaging;
using openalprnet;
using System.Reflection;
using System.IO;
using System.Threading;

namespace AlprNetGuiTest
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        //AGREGAR USING
        //VARIABLE PARA LISTA DE DISPOSITIVOS
        private FilterInfoCollection Dispositivos;
        //VARIABLE PARA FUENTE DE VIDEO
        private VideoCaptureDevice FuenteDeVideo;

        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            //LISTAR DISPOSITIVOS DE ENTRADA DE VIDEO
            Dispositivos = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            //CARGAR TODOS LOS DISPOSITIVOS AL COMBO
            foreach (FilterInfo x in Dispositivos)
            {
                comboBox1.Items.Add(x.Name);
            }
            comboBox1.SelectedIndex = 0;

            FuenteDeVideo = new VideoCaptureDevice();

            PictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ESTABLECER EL DISPOSITIVO SELECCIONADO COMO FUENTE DE VIDEO
            FuenteDeVideo = new VideoCaptureDevice(Dispositivos[comboBox1.SelectedIndex].MonikerString);
            //INICIALIZAR EL CONTROL
            videoSourcePlayer1.VideoSource = FuenteDeVideo;
            //INICIAR RECEPCION DE IMAGENES
            videoSourcePlayer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //DETENER RECEPCION DE IMAGENES
            videoSourcePlayer1.SignalToStop();
            PictureBox2.Image = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string placa = " ";
            string dir = "C:\\Users\\carlo\\Desktop\\aaa.jpg";

            //VARIBALE PARA LA IMAGEN
            Bitmap img = videoSourcePlayer1.GetCurrentVideoFrame();
            //GUARDAR IMAGEN EN LA RUTA
            img.Save(dir, System.Drawing.Imaging.ImageFormat.Jpeg);
            //BORRAR IMAGEN DE MEMORIA
            img.Dispose();
            //PROBAR!


            var region = "us";
            //var region = "ve";
            String config_file = Path.Combine(AssemblyDirectory, "openalpr.conf");
            String runtime_data_dir = Path.Combine(AssemblyDirectory, "runtime_data");

            //using (var alpr = new Alpr(region, config_file, runtime_data_dir))
            using (var alpr = new AlprNet(region, config_file, runtime_data_dir))
            {
                //alpr.Initialize();

                if (!alpr.IsLoaded())
                {
                    //lbxPlates.Items.Add("Error initializing OpenALPR");
                    return;
                }

                var results = alpr.Recognize(dir);
                if (results.Plates.Count > 0) {

                    System.Console.WriteLine(results.Plates[0].BestPlate.Characters);
                    placa = results.Plates[0].BestPlate.Characters;

                    if (Busqueda.getplaca(placa) == 1)
                    {
                        System.Console.WriteLine("correto");
                        Busqueda.EnUni(placa);
                        //int milliseconds = 3000;
                      //  Thread.Sleep(milliseconds);
                    }

                    

                }
                


            }
        }

        private System.Drawing.Point[] ARRAYPUNTOS(List<IntPoint> PUNTOS)
        {
            System.Drawing.Point[] MIARRAY = new System.Drawing.Point[(PUNTOS.Count())];
            for (int I = 0; (I < (PUNTOS.Count())); I++)
            {
                MIARRAY[I] = new System.Drawing.Point(PUNTOS[I].X, PUNTOS[I].Y);
            }

            return MIARRAY;
        }

        private void ButtonCAMARA_Click(object sender, EventArgs e)
        {
            // INICIA ESCANEO
            ButtonCAMARA.Visible = false;
            Timer1.Interval = 3000;
            Timer1.Start();
        }

        private void Timer1_Tick_1(object sender, System.EventArgs e)
        {
            PictureBox2.Image = videoSourcePlayer1.GetCurrentVideoFrame();
            try
            {
                Bitmap BMP = new Bitmap(PictureBox2.Image);
                // PONE LA IMAGEN EN UN BITMAP
                Rectangle RECTANGULO = new Rectangle(0, 0, BMP.Width, BMP.Height);
                // AREA DE LA IMAGEN
                BitmapData BMPDATOS = BMP.LockBits(RECTANGULO, ImageLockMode.ReadWrite, BMP.PixelFormat);
                // OBTIENE LAS CARACTERISTICAS DE LA IMAGEN Y LA PONE EN MEMORIA
                // PONE EL FONDO EN NEGRO
                ColorFiltering FILTRO = new ColorFiltering();
                FILTRO.Red = new IntRange(0, 192);
                FILTRO.Green = new IntRange(0, 192);
                FILTRO.Blue = new IntRange(0, 192);
                FILTRO.FillOutsideRange = false;
                FILTRO.ApplyInPlace(BMPDATOS);
                // BUSCA LOS ELEMENTOS
                BlobCounter ELEMENTOS = new BlobCounter();
                ELEMENTOS.FilterBlobs = true;
                ELEMENTOS.MinHeight = 15;
                // ALTURA MINIMA
                ELEMENTOS.MinWidth = 15;
                // ANCHURA MINIMA
                ELEMENTOS.ProcessImage(BMPDATOS);
                Blob[] ELEMENTOSINFO = ELEMENTOS.GetObjectsInformation();
                BMP.UnlockBits(BMPDATOS);
                SimpleShapeChecker BUSCADOR = new SimpleShapeChecker();
                // PARA DETERMINAR LA FORMA DE LOS ELEMENTOS ENCONTRADOS
                Graphics DIBUJO = Graphics.FromImage(BMP);
                // PARA DIBUJAR LOS ELEMENTOS
                //Pen CIRCULOS = new Pen(CheckBoxCIRCULOS.ForeColor, NumericUpDownGROSOR.Value);
                // TRAZO CIRCULOS
                //Pen TRIANGULOS = new Pen(CheckBoxTRIANGULOS.ForeColor, NumericUpDownGROSOR.Value);
                // TRAZO TRIANGULOS
                Pen CUADRILATEROS = new Pen(Color.Red, 10);
                // TRAZO CUADRILATEROS
                Pen TRAZO = null;
                int I;
                for (I = 0; (I <= (ELEMENTOSINFO.Length - 1)); I++)
                {
                    List<IntPoint> PUNTOS = ELEMENTOS.GetBlobsEdgePoints(ELEMENTOSINFO[I]);
                    // OBTIENE LOS PUNTOS DE LA FORMA
                    AForge.Point CENTRO;
                    // CENTRO DEL CIRCULO
                    float RADIO;
                    // RADIO DEL CIRCULO
                    //if (BUSCADOR.IsCircle(PUNTOS, CENTRO, RADIO))
                    //{
                    //    // SI ES UN CIRCULO....
                    //    if (CheckBoxCIRCULOS.Checked)
                    //    {
                    //        DIBUJO.DrawEllipse(CIRCULOS, float.Parse((CENTRO.X - RADIO)), float.Parse((CENTRO.Y - RADIO)), float.Parse((RADIO * 2)), float.Parse((RADIO * 2)));
                    //        // DIBUJA EL CIRCULO
                    //    }

                    //}
                    //else {
                    List<IntPoint> VERTICES = null;
                    if (BUSCADOR.IsConvexPolygon(PUNTOS, out VERTICES))
                    {

                        //  SI ES UN TRIANGULO O UN CUADRILATERO.....
                        if ((VERTICES.Count == 4))
                        {
                            /* if (VERTICES == VERTICES2)
                             {
                                 button3.PerformClick();
                             }
                             VERTICES2 = VERTICES;*/
                            //  ES UN CUADRILATERO
                            //if (CheckBoxCUADRILATEROS.Checked)
                            //{
                            TRAZO = CUADRILATEROS;
                            DIBUJO.DrawPolygon(TRAZO, this.ARRAYPUNTOS(VERTICES));
                            button3.PerformClick();


                            //else {
                            //    //  ES UN TRIANGULO
                            //    if (CheckBoxTRIANGULOS.Checked)
                            //    {
                            //        TRAZO = TRIANGULOS;
                            //        DIBUJO.DrawPolygon(TRAZO, this.ARRAYPUNTOS(VERTICES));
                            //    }

                            //}

                        }

                    }

                    //}

                    PictureBox2.Image = BMP;
                    // PONE LA IMAGEN RESULTANTE EN EL PICTUREBOX
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
           
            BdComun.ObtenerConexion();
            MessageBox.Show("conectado");
          
        }
    }
}
