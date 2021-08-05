using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ProyPadronSunat
{
    public partial class Form1 : Form
    {
        private static IWebDriver driver;

        //Ruta de Descarga
        static string path1_DWL = Environment.CurrentDirectory + @"\1-Downloads\";
        static string path2_Unzip = Environment.CurrentDirectory + @"\2-Unzip\";
        static string path3_txtFormat = Environment.CurrentDirectory + @"\3-TxtFormat\";

        public Form1()
        {
            InitializeComponent();

            createPaths();


            //option por default para descarga multiple
            var options = new ChromeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.automatic_downloads", 1);
            options.AddUserProfilePreference("download.default_directory", path1_DWL);

            options.AddUserProfilePreference("profile.managed_default_content_settings.insecure_content", 1);

            driver = new ChromeDriver(options);
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.ToString();

            string url = "https://www.sunat.gob.pe/descargaPRR/mrc137_padron_reducido.html";

            driver.Url = url;

            #region Descarga txt web sunat

            Thread.Sleep(2000);

            IWebElement iDownloadZip = driver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[2]/a"));
            iDownloadZip.Click();

            #endregion

            Thread.Sleep(30000);

            //Ruta donde se descomprimira los archivos ZIP

            #region Descomprimir ZIP

            string pathUnzip = Environment.CurrentDirectory + @"\2-Unzip\";

            if (!Directory.Exists(pathUnzip))
            { Directory.CreateDirectory(pathUnzip); }

            bool existeFile = false;

            while (existeFile == false)
            {
                DirectoryInfo di1 = new DirectoryInfo(path1_DWL);
                string name1;

                foreach (var fi1 in di1.GetFiles())
                {
                    name1 = fi1.Name.Substring(0, 6);

                    if (name1.Equals("padron"))
                    {
                        ZipFile.ExtractToDirectory(path1_DWL + fi1.Name, pathUnzip);

                        existeFile = true;
                    }
                    else
                    {
                        Thread.Sleep(10000);

                        existeFile = false;
                    }
                }
            }

            #endregion

            Thread.Sleep(5000);

            #region Formatear TXT

            DirectoryInfo di2 = new DirectoryInfo(path2_Unzip);

            foreach (var fi2 in di2.GetFiles())
            {
                reemplazarCaracter(fi2.Name);
            }

            #endregion

            Thread.Sleep(3000);

            cleanFiles("");

            label2.Text = DateTime.Now.ToString();
        }

        public void reemplazarCaracter(string nameFile) {

            string nameFile2 = nameFile.Substring(0, nameFile.Length - 4);

            string path1 = path2_Unzip + nameFile;
            string path2 = path3_txtFormat + nameFile2 + "_Res.txt";
            //string path3 = Environment.CurrentDirectory + @"\padron_reducido_ruc_Res_1.txt";

            string[] arr = File.ReadAllLines(path1, Encoding.Default);
            int large = arr.Length;

            string line;

            var stopwatch = new Stopwatch(); // 2) Nueva Linea

            stopwatch.Start();

            using (StreamWriter streamwriter1 = File.AppendText(path2))
            {
                for (int i = 0; i < large; i++)
                {
                    line = arr[i];
                    line = line.Replace("||", "|").Replace(@"\|", "").Replace(@"\", ".");

                    streamwriter1.WriteLine(line.Substring(0, line.Length - 1));
                }
            }
            stopwatch.Stop();

        }

        public void createPaths() {

            string path1 = Environment.CurrentDirectory + @"\1-Downloads\";
            string path2 = Environment.CurrentDirectory + @"\2-Unzip\";
            string path3 = Environment.CurrentDirectory + @"\3-TxtFormat\";

            if (!Directory.Exists(path1))
            {
                Directory.CreateDirectory(path1);
            }
            if (!Directory.Exists(path2))
            {
                Directory.CreateDirectory(path2);
            }
            if (!Directory.Exists(path3))
            {
                Directory.CreateDirectory(path3);
            }
        }

        public void cleanFiles(string all)
        {

            DirectoryInfo di11 = new DirectoryInfo(path1_DWL);

            foreach (var fi11 in di11.GetFiles())
            {
                File.Delete(path1_DWL + fi11.Name);
            }

            DirectoryInfo di12 = new DirectoryInfo(path2_Unzip);

            foreach (var fi12 in di12.GetFiles())
            {
                File.Delete(path2_Unzip + fi12.Name);
            }

            if (all.Equals("all"))
            {

                DirectoryInfo di13 = new DirectoryInfo(path3_txtFormat);

                foreach (var fi13 in di13.GetFiles())
                {
                    File.Delete(path3_txtFormat + fi13.Name);
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            driver.Close();
            this.Close();
        }

        private void BtnEliminarFiles_Click(object sender, EventArgs e)
        {
            cleanFiles("all");

            MessageBox.Show("Archivos eliminados correctamente");
        }
    }
}
