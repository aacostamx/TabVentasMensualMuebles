using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Threading;
///////////////////////////////////////////////////////////////////////////////////////////////////////////
// Elaboro : Antonio Acosta Murillo
// Objetivo: Generar tabulado de ventas mensuales de la Cartera Muebles Total, Coppel y Negocios Afiliados
// Fecha   : 12/Ene/2014
///////////////////////////////////////////////////////////////////////////////////////////////////////////
// Modifico: Antonio Acosta Murillo
// Razon: Se agrego clase BackgroudWorker para manejar la interfaz en diferentes hilos
// Fecha: 15/Ago/2012
///////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace AF0097_TabVentasMensualMuebles
{
    public partial class FrmPrincipal : Form
    {
        #region Variables Globales
        ConexionSQL con = new ConexionSQL();
        DataTable dtFecha = new DataTable();
        DataTable dtTotal = new DataTable();
        DataTable dtCoppel = new DataTable();
        DataTable dtNegAfiliado = new DataTable();
        ReportDocument rptCMVtaMuebles = new ReportDocument();
        //declaras los BackgroundWorkers y el delegado
        private BackgroundWorker Worker = new BackgroundWorker();
        private BackgroundWorker Worker2 = new BackgroundWorker();
        public delegate void Fhandler();


        #endregion

        #region Constructor
        public FrmPrincipal()
        {
            InitializeComponent();
            //Inicias el backgoundWorker y asignas verdadero el soporte de cancelacion
            InitializeBackgoundWorker();
            Worker.WorkerSupportsCancellation = true;
            Worker2.WorkerSupportsCancellation = true;
            CheckForIllegalCrossThreadCalls = false; 

        }
        #endregion

        #region Evento Click
        private void bIniciar_Click(object sender, EventArgs e)
        {
            bIniciar.Enabled = false;
            lbHoraInicialUsuario.Text = "--";
            lbHoraFinalUsuario.Text = "--";
            tssInformacion.Text = "--";
            lbTimeTranscurridoUsuario.Text = "--";
            Cursor = Cursors.WaitCursor;
            tssInformacion.Text = "Proceso en ejecución";

            //aquí debes de asignar la hora a la hora de inicio 
            lbHoraInicialUsuario.Text = DateTime.Now.ToLongTimeString();
            //corres los workers
            Worker2.RunWorkerAsync();
            Worker.RunWorkerAsync();
        }
        #endregion

        #region Logica
        private void GeneraTabuladosVTAsMueble()
        {
            //es importante que el worker este dentro de un try catch
            try
            {
                //cambiar el cursor a espera y el boton iniciar se vuelve inactivo
                Cursor = Cursors.WaitCursor;
                bIniciar.Enabled = false;

                //limpia las labels de la hora de inicia y hora final del usuario
                lbHoraInicialUsuario.Text = "--";
                lbHoraFinalUsuario.Text = "--";

                //asigna la hora que inicio el programa
                lbHoraInicialUsuario.Text = DateTime.Now.ToShortTimeString();
                Refresh();

                //variables que se utilizaran durante el proceso
                string conexion58Carteras, sentencia, fecha, fechaarchivo, droptable;
                //selecciona la fecha actual del catfechas
                sentencia = "select * from catFechas";
                Refresh();

                //verifica que exista el archivo de conexion
                if (!File.Exists("C:/Sys/Exe/Conexion/Conexion58Carteras.txt"))
                {
                    MessageBox.Show("El archivo de texto de la conexión no existe", "Advertencia");
                    bIniciar.Enabled = true;
                    Cursor = Cursors.Default;
                    return;
                }
                //se conecta a la base de datos carteras
                conexion58Carteras = con.LeeArchivo("C:/Sys/Exe/Conexion/Conexion58Carteras.txt");
                Conexion conexion = new Conexion(conexion58Carteras);
                conexion.LlenarDataTable(ref dtFecha, sentencia);

                //convierte la fecha a una cadena
                fecha = dtFecha.Rows[0][0].ToString();
                DateTime dtimeFecha = new DateTime();
                dtimeFecha = Convert.ToDateTime(fecha);
                //fecha = dtimeFecha.ToString("yyyyMMdd");
                fecha = dtimeFecha.ToString("dd/MM/yyyy");
                fechaarchivo = dtimeFecha.ToString("dd-MMM-yyyy");
                fechaarchivo = fechaarchivo.ToUpper();

                //ejecuta el procedimiento almacenado
                sentencia = "exec AF0097_TabVentasMensualMuebles";
                conexion.EjecutarSentencia(sentencia);

                //llena los data tables con la informacion de vtas muebles total, coppel y negocios afiliados
                conexion.LlenarDataTable(ref dtTotal, "select Fecha, Saldo, SaldoenMiles, Vencido, VencidoenMiles, NumCuentas from tmp_VtasMensualMueblesFinal order by anio desc, mes desc");
                conexion.LlenarDataTable(ref dtCoppel, "select Fecha, Saldo, SaldoenMiles, Vencido, VencidoenMiles, NumCuentas from tmp_VtasMensualMueblesFinal_Coppel order by anio desc, mes desc");
                conexion.LlenarDataTable(ref dtNegAfiliado, "select Fecha, Saldo, SaldoenMiles, Vencido, VencidoenMiles, NumCuentas from tmp_VtasMensualMueblesFinal_NegAfiliado order by anio desc, mes desc ");

                //se suma el vencido y vencido en miles del mes actual al mes anterior, y se deja en cero el vencido del mes actual
                SumaVencidoMesAnterior(ref dtTotal);
                SumaVencidoMesAnterior(ref dtCoppel);
                SumaVencidoMesAnterior(ref dtNegAfiliado);

                //se suma los ultimos meses en el ultimo mes que se mostrara en el informe
                SumaUltimosMeses(ref dtTotal);
                SumaUltimosMeses(ref dtCoppel);
                SumaUltimosMeses(ref dtNegAfiliado);

                //Genera reporte total
                rptCMVtaMuebles.FileName = @"rassdk://C:\sys\crystal\AF0097_TabVentasMensualMuebles.rpt";
                rptCMVtaMuebles.DataSourceConnections[0].SetConnection(con.IP, con.DB, con.USER, con.PASS);
                rptCMVtaMuebles.Refresh();
                rptCMVtaMuebles.SetDataSource(dtTotal);
                rptCMVtaMuebles.SetParameterValue("fechacorte", fecha);
                rptCMVtaMuebles.SetParameterValue("fechaarchivo", fechaarchivo);
                rptCMVtaMuebles.SetParameterValue("tipotabulado", "TOTAL");
                rptCMVtaMuebles.SetParameterValue("nombrearchivo", "CMVtaMueblesTotal");
                rptCMVtaMuebles.SetDatabaseLogon(con.USER, con.PASS);
                rptCMVtaMuebles.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
                rptCMVtaMuebles.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.DefaultPaperOrientation;
                rptCMVtaMuebles.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "AF0097_CMVtaMueblesTotal_" + fechaarchivo + ".pdf");
                //rptCMVtaMuebles.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.Excel, "AF0097_CMVtaMueblesTotal_" + fechaarchivo + ".xls");

                //Genera reporte Coppel
                rptCMVtaMuebles.FileName = @"rassdk://C:\sys\crystal\AF0097_TabVentasMensualMuebles.rpt";
                rptCMVtaMuebles.DataSourceConnections[0].SetConnection(con.IP, con.DB, con.USER, con.PASS);
                rptCMVtaMuebles.Refresh();
                rptCMVtaMuebles.SetDataSource(dtCoppel);
                rptCMVtaMuebles.SetParameterValue("fechacorte", fecha);
                rptCMVtaMuebles.SetParameterValue("fechaarchivo", fechaarchivo);
                rptCMVtaMuebles.SetParameterValue("tipotabulado", "COPPEL");
                rptCMVtaMuebles.SetParameterValue("nombrearchivo", "CMVtaMueblesCoppel");
                rptCMVtaMuebles.SetDatabaseLogon(con.USER, con.PASS);
                rptCMVtaMuebles.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
                rptCMVtaMuebles.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.DefaultPaperOrientation;
                rptCMVtaMuebles.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "AF0097_CMVtaMueblesCoppel_" + fechaarchivo + ".pdf");
                //rptCMVtaMuebles.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.Excel, "AF0097_CMVtaMueblesCoppel_" + fechaarchivo + ".xls");

                //Genera reporte Negocios Afiliados
                rptCMVtaMuebles.FileName = @"rassdk://C:\sys\crystal\AF0097_TabVentasMensualMuebles.rpt";
                rptCMVtaMuebles.DataSourceConnections[0].SetConnection(con.IP, con.DB, con.USER, con.PASS);
                rptCMVtaMuebles.Refresh();
                rptCMVtaMuebles.SetDataSource(dtNegAfiliado);
                rptCMVtaMuebles.SetParameterValue("fechacorte", fecha);
                rptCMVtaMuebles.SetParameterValue("fechaarchivo", fechaarchivo);
                rptCMVtaMuebles.SetParameterValue("tipotabulado", "NEGOCIOS AFILIADOS");
                rptCMVtaMuebles.SetParameterValue("nombrearchivo", "CMVtaMueblesNegAfiliado");
                rptCMVtaMuebles.SetDatabaseLogon(con.USER, con.PASS);
                rptCMVtaMuebles.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
                rptCMVtaMuebles.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.DefaultPaperOrientation;
                rptCMVtaMuebles.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "AF0097_CMVtaMueblesNegAfiliado_" + fechaarchivo + ".pdf");
                //rptCMVtaMuebles.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.Excel, "AF0097_CMVtaMueblesNegAfiliado_" + fechaarchivo + ".xls");

                //limpia data tables
                dtFecha.Clear();
                dtTotal.Clear();
                dtCoppel.Clear();
                dtNegAfiliado.Clear();

                //elimina tablas temporales de la base de datos Carteras
                droptable = "if exists(select * from sysobjects where name = 'tmp_VtasMensualMueblesFinal') drop table tmp_VtasMensualMueblesFinal";
                conexion.EjecutarSentencia(droptable);
                droptable = "if exists(select * from sysobjects where name = 'tmp_VtasMensualMueblesFinal_Coppel') drop table tmp_VtasMensualMueblesFinal_Coppel";
                conexion.EjecutarSentencia(droptable);
                droptable = "if exists(select * from sysobjects where name = 'tmp_VtasMensualMueblesFinal_NegAfiliado') drop table tmp_VtasMensualMueblesFinal_NegAfiliado";
                conexion.EjecutarSentencia(droptable);

                //para finalizar se utiliza el delegado
                BeginInvoke((Delegate)new FrmPrincipal.Fhandler(Finaliza));
                //para cancelar los workers
                Worker.CancelAsync();
                Worker2.CancelAsync();
            }
            catch (Exception ex)
            {
                //finaliza el worker
                BeginInvoke((Delegate)new FrmPrincipal.Fhandler(this.Finaliza));
                //para cancelar el worker
                Worker.CancelAsync();
                Worker2.CancelAsync();
                int num = (int)MessageBox.Show("Error: " + ((object)ex.Message).ToString() + "\nSource: " + ((object)ex.Source).ToString() + "\nMetodo: " + ex.TargetSite.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        #endregion

        #region Metodos
        public void SumaVencidoMesAnterior(ref DataTable dt)
        {
            try
            {
                //se suma el vencido, vencido en miles, y numero de operaciones de los ultimos meses
                Int32 vencido = Convert.ToInt32(dt.Rows[0][3].ToString()) + Convert.ToInt32(dt.Rows[1][3].ToString());
                Int32 vencidoenmiles = Convert.ToInt32(dt.Rows[0][4].ToString()) + Convert.ToInt32(dt.Rows[1][4].ToString());
                dt.Rows[0][3] = 0;
                dt.Rows[1][3] = vencido;
                dt.Rows[0][4] = 0;
                dt.Rows[1][4] = vencidoenmiles;
            }
            catch (Exception ex) { throw ex; }
        }

        public void SumaUltimosMeses(ref DataTable dt)
        {
            try
            {
                //se suma el vencido, vencido en miles, y numero de operaciones de los ultimos meses
                Int32 Vencido, VencidoenMiles, NumeroCuentas;
                for (int i = 24; i <= dt.Rows.Count - 1; i++)
                {
                    Vencido = Convert.ToInt32(dt.Rows[i][3]);
                    VencidoenMiles = Convert.ToInt32(dt.Rows[i][4]);
                    NumeroCuentas = Convert.ToInt32(dt.Rows[i][5]);

                    dt.Rows[23][3] = Convert.ToInt32(dt.Rows[23][3].ToString()) + Vencido;
                    dt.Rows[23][4] = Convert.ToInt32(dt.Rows[23][4].ToString()) + VencidoenMiles;
                    dt.Rows[23][5] = Convert.ToInt32(dt.Rows[23][5].ToString()) + NumeroCuentas;
                }

                //se eliminan los renglones mayores a 24
                for (int i = 24; i <= dt.Rows.Count - 1; i++)
                    dt.Rows[i].Delete();

                dt.AcceptChanges();
            }
            catch (Exception ex) { throw ex; }
        }

        //metodo para inicializar el backgroundworker 
        private void InitializeBackgoundWorker()
        {
            Worker.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            Worker2.DoWork += new DoWorkEventHandler(backgroundWorker2_DoWork);
            Worker2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker2_RunWorkerCompleted);
        }

        //metodo de la interfaz del worker 1
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Transcurrido();
        }

        //metodo que llama el procedimiento del worker 2
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            GeneraTabuladosVTAsMueble();
        }

        //si el worker 1 termina revisa que no tenga errores
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
                return;
            int num = (int)MessageBox.Show(e.Error.Message);
        }

        //si el worker dos termina revisa que no tenga erroes
        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
                return;
            int num = (int)MessageBox.Show(e.Error.Message);
        }

        //Metodo que invoca el metodo tiempo transcurrido
        public void Transcurrido()
        {
            try
            {
                MethodInvoker methodInvoker = new MethodInvoker(TiempoTranscurrido);
                while (!Worker.CancellationPending)
                {
                    BeginInvoke((Delegate)methodInvoker);
                    Thread.Sleep(500);
                }
            }
            catch (ThreadInterruptedException ex)
            {
                MessageBox.Show(ex.ToString(), "Aviso");
            }
        }

        //Calcula el tiempo transcurrido 
        private void TiempoTranscurrido()
        {
            //lbTime.Text = ("Transcurrido " + Convert.ToString((object)(DateTime.Now - Convert.ToDateTime(lbHoraInicio.Text)))).Substring(0, 21);
            //lbTimeTranscurrido.Text = Convert.ToString(((object)(DateTime.Now - Convert.ToDateTime(lbHoraInicialUsuario.Text)))).Substring(0, 21);
            lbTimeTranscurridoUsuario.Text = Convert.ToString(DateTime.Now - Convert.ToDateTime(lbHoraInicialUsuario.Text)).Substring(0, 8);
            //lbTimeTranscurrido.Text = DateTime.Now.Second.ToString();
            Refresh();
        }

        //si se cierra la forma principal se cancelan los workers
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Worker.CancelAsync();
            Worker2.CancelAsync();
        }

        public void Finaliza()
        {
            Cursor = Cursors.Default;
            lbHoraFinalUsuario.Text = DateTime.Now.ToLongTimeString();
            tssInformacion.Text = "Proceso Finalizado";
            bIniciar.Enabled = true;

            Refresh();
            Worker2.CancelAsync();
        }
        #endregion

        #region Evento Key
        private void FrmPrincipal_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    GeneraTabuladosVTAsMueble();
                    break;
                case Keys.Escape:
                    this.Dispose();
                    this.Close();
                    break;
            }
        }
        #endregion
    }
}
