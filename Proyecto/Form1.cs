using System.CodeDom;
using System.Management;
using System.Net;
using System.Reflection;


namespace Proyecto
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            salida.Text= string.Empty;
            ObtenerUsuarioEquipo();
            ObtenerUnidad();
            ObeterIP();
            ComprobarPCEnCarga();
            ObtenerGestionMemoriaRAM();
        }

        private void ObtenerUsuarioEquipo()
        {
            string usuario = SystemInformation.UserName;
            string dominio = SystemInformation.UserDomainName;

            salida.Text = "Usuario: "+ usuario + Environment.NewLine
                +"Dominio/Equipo: "+ dominio;
        }

        private void ObtenerUnidad()
        {
            DriveInfo[] drives = DriveInfo
                .GetDrives()
                .Where(disco => disco.DriveType == DriveType.Fixed)
                .ToArray();
            foreach (DriveInfo drive in drives)
            {
                double espacioLibre = drive.TotalFreeSpace;
                double espacioTotal = drive.TotalSize;

                double espacioLibrePorcentaje = (espacioLibre/ espacioTotal) * 100;

                salida.Text += drive.Name + ": " + espacioLibrePorcentaje + " % "
                    + Environment.NewLine;
            }
        }
        private void ObeterIP()
        {
            IPAddress[] direcciones =
                Dns.GetHostAddresses(Dns.GetHostName())
                .Where(a => !a.IsIPv6LinkLocal)
                .ToArray();
            foreach(IPAddress direccion in direcciones)
            {
                salida.Text += Environment.NewLine + "IP: " + direccion.Address.ToString();
            }
        }
        private void ComprobarPCEnCarga()
        {
            Type pw = typeof(PowerStatus);

            PropertyInfo[]propiedades = pw.GetProperties();

            object? valor = propiedades[0].GetValue(SystemInformation.PowerStatus,null);

            salida.Text += valor.ToString();
        }

        private void ObtenerGestionMemoriaRAM()
        {
            ObjectQuery objectQuery= new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            
            ManagementObjectSearcher managementObject = new (objectQuery);

            ManagementObjectCollection collection = managementObject.Get();

            foreach(ManagementObject elemento in collection)
            {
                decimal memoriaTotal=
                    Math.Round(Convert.ToDecimal(elemento["TotalVisibleMemorySize"]) / (1024 * 1024),2);

                decimal memoriaLibre=
                    Math.Round(Convert.ToDecimal(elemento["FreePhysicalMemory"])/(1024 * 1024),2);

                salida.Text += Environment.NewLine +
                    "Memoria Total: " + memoriaTotal+"GB";

                salida.Text += Environment.NewLine +
                    "Memoria Libre: " + memoriaLibre+"GB";

                string arquitectura=elemento["OSArchitecture"].ToString();
                string fabricante = elemento["Manufacturer"].ToString();
                salida.Text += Environment.NewLine + arquitectura+fabricante; 

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}