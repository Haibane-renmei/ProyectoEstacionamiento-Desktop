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
//using System.Drawing.Imaging.BitmapData;
using System.Reflection;
using System.IO;



namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        //AGREGAR REFERENCIAS
        //AGREGAR CONTROLES
        //DISEÑAR FORMULARIO
        // VideoSourcePlayer1 es el control donde se mostrará
        //la imagen de la webcam

        //MotionDetector Detector;
        //float NivelDeDeteccion;
        List<IntPoint> VERTICES2 = null;

        public Form1()
        {
            InitializeComponent();
        }
        //AGREGAR USING
        //VARIABLE PARA LISTA DE DISPOSITIVOS
        private FilterInfoCollection Dispositivos;
        //VARIABLE PARA FUENTE DE VIDEO
        private VideoCaptureDevice FuenteDeVideo;

        private void Form1_Load(object sender, EventArgs e)
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            //ESTABLECER EL DISPOSITIVO SELECCIONADO COMO FUENTE DE VIDEO
            FuenteDeVideo = new VideoCaptureDevice(Dispositivos[comboBox1.SelectedIndex].MonikerString);
            //INICIALIZAR EL CONTROL
            videoSourcePlayer1.VideoSource = FuenteDeVideo;
            //INICIAR RECEPCION DE IMAGENES
            videoSourcePlayer1.Start();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //DETENER RECEPCION DE IMAGENES
            videoSourcePlayer1.SignalToStop();
            PictureBox2.Image = null;
        }

        private void button3_Click_ori(object sender, EventArgs e)
        {
            //DIALOGO PARA SELECCIONAR LA RUTA PARA GUARDAR
            SaveFileDialog sf = new SaveFileDialog();
            //FILTO DE IMAGENES JPG
            sf.Filter = "Imagenes JPG | *.jpg";
            //MOSTRAR DIALOGO
            sf.ShowDialog();
            //ASEGURAR QUE TIENE UNA RUTA VALIDA
            if (sf.FileName != null)
            {
                //VARIBALE PARA LA IMAGEN
                Bitmap img = videoSourcePlayer1.GetCurrentVideoFrame();
                //GUARDAR IMAGEN EN LA RUTA
                img.Save(sf.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                //BORRAR IMAGEN DE MEMORIA
                img.Dispose();
                //PROBAR!
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string dir = "C:\\Users\\carlo\\Desktop\\aaa.jpg";
        
                //VARIBALE PARA LA IMAGEN
                Bitmap img = videoSourcePlayer1.GetCurrentVideoFrame();
                //GUARDAR IMAGEN EN LA RUTA
                img.Save(dir, System.Drawing.Imaging.ImageFormat.Jpeg);
                //BORRAR IMAGEN DE MEMORIA
                img.Dispose();
                //PROBAR!
            
        }

        private System.Drawing.Point[] ARRAYPUNTOS(List<IntPoint> PUNTOS)
        {
            System.Drawing.Point[] MIARRAY = new System.Drawing.Point[ (PUNTOS.Count()) ];
            for (int I = 0; (I < (PUNTOS.Count())); I++)
            {
                MIARRAY[I] = new System.Drawing.Point(PUNTOS[I].X, PUNTOS[I].Y);
            }

            return MIARRAY;
        }

        private void ButtonCAMARA_Click_1(object sender, System.EventArgs e)
        {
            // INICIA ESCANEO
            ButtonCAMARA.Visible = false;
            Timer1.Interval = 1000;
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
                FILTRO.Red = new IntRange(0, 128);
                FILTRO.Green = new IntRange(0, 128);
                FILTRO.Blue = new IntRange(0, 128);
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
                for (I = 0; (I<= (ELEMENTOSINFO.Length - 1)); I++)
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
                        }
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
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

        }
        

        //private void videoSourcePlayer1_NewFrame(object sender, Bitmap image)
        //{
        //    float nivelMinimo = 0.0030f;
        //    string dir = "C:\\Users\\Francisco Clotet\\Desktop\\aaa.jpg";

        //    NivelDeDeteccion = Detector.ProcessFrame(image);
        //    if (nivelMinimo >= NivelDeDeteccion)
        //    {
        //        Bitmap img = videoSourcePlayer1.GetCurrentVideoFrame();
        //        //GUARDAR IMAGEN EN LA RUTA
        //        img.Save(dir, System.Drawing.Imaging.ImageFormat.Jpeg);
        //        //BORRAR IMAGEN DE MEMORIA
        //        img.Dispose();
        //    }
        //}
    }
}