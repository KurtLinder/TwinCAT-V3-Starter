using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;

namespace TwinCAT_V3_Starter
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(this UIElement uiElement)
        {
            try
            {
                uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
            }
            catch (Exception exp)
            {
            }
        }
    }

    public partial class MainWindow : Window
    {

        string ProjektName = "";
        string ProjektPfad = "h:\\TwinCAT_V3";
        List<RadioButton> RadioButtonList = new List<RadioButton>();

        public MainWindow()
        {
            InitializeComponent();
            ProjekteLesen();
        }

        public void ProjekteLesen()
        {

            /*
            * Aufbau der Projektnamen (Ordner)
            * TwinCAT_V3_PLC_WEB_FUP_Linearachse
            * 
            * _PLC_ oder  _BUG_    
            * + _NC_
            * + _HMI
            * + _VISU_
            * + _FIO_
            * + _WEB_
            * 
            * _AWL_ oder _AS_ oder _FUP_ oder _KOP_ oder _SCL_ oder _ST_
            * 
            * */


            List<string> ProjektVerzeichnis = new List<string>();
            List<string> Projekte_PLC = new List<string>();
            List<string> Projekte_PLC_VISU = new List<string>();
            List<string> Projekte_PLC_FIO = new List<string>();
            List<string> Projekte_BUG = new List<string>();

            System.IO.DirectoryInfo ParentDirectory = new System.IO.DirectoryInfo("Projekte");

            foreach (System.IO.DirectoryInfo d in ParentDirectory.GetDirectories())
                ProjektVerzeichnis.Add(d.Name);

            ProjektVerzeichnis.Sort();

            foreach (string Projekt in ProjektVerzeichnis)
            {
                string Sprache = "";
                int StartBezeichnung = 0;

                if (Projekt.Contains("AS"))
                {
                    Sprache = " (AS/SFC)";
                    StartBezeichnung = 3 + Projekt.IndexOf("AS");
                }
                if (Projekt.Contains("AWL"))
                {
                    Sprache = " (AWL/IL)";
                    StartBezeichnung = 4 + Projekt.IndexOf("AWL");
                }
                if (Projekt.Contains("CFC"))
                {
                    Sprache = " (CFC)";
                    StartBezeichnung = 4 + Projekt.IndexOf("CFC");
                }
                if (Projekt.Contains("FUP"))
                {
                    Sprache = " (FUP/FBD)";
                    StartBezeichnung = 4 + Projekt.IndexOf("FUP");
                }
                if (Projekt.Contains("KOP"))
                {
                    Sprache = " (KOP/LD)";
                    StartBezeichnung = 4 + Projekt.IndexOf("KOP");
                }
                if (Projekt.Contains("ST"))
                {
                    Sprache = " (ST)";
                    StartBezeichnung = 3 + Projekt.IndexOf("ST");
                }


                RadioButton rdo = new RadioButton();
                rdo.GroupName = "TIA_PORTAL_V14_SP1";
                rdo.VerticalAlignment = VerticalAlignment.Top;
                rdo.Checked += new RoutedEventHandler(radioButton_Checked);
                rdo.FontSize = 14;



                if (Projekt.Contains("PLC"))
                {
                    if (Projekt.Contains("VISU"))
                    {
                        rdo.Content = Projekt.Substring(StartBezeichnung).Replace("_", " ") + Sprache;
                        rdo.Name = Projekt;
                        StackPanel_PLC_VISU.Children.Add(rdo);
                    }
                    else
                    {

                        if (Projekt.Contains("NC"))
                        {
                            rdo.Content = Projekt.Substring(StartBezeichnung).Replace("_", " ") + Sprache;
                            rdo.Name = Projekt;
                            StackPanel_PLC_NC.Children.Add(rdo);
                        }
                        else
                        {
                            // nur PLC und sonst nichts
                            rdo.Content = Projekt.Substring(StartBezeichnung).Replace("_", " ") + Sprache;
                            rdo.Name = Projekt;
                            StackPanel_PLC.Children.Add(rdo);
                        }
                    }
                }
                else
                {
                    // Es gibt momentan noch keine Gruppe bei den Bugs
                    rdo.Content = Projekt.Substring(StartBezeichnung).Replace("_", " ") + Sprache;
                    rdo.Name = Projekt;
                    StackPanel_BUG.Children.Add(rdo);
                }

                RadioButtonList.Add(rdo);
            }

        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            System.IO.DirectoryInfo ParentDirectory = new System.IO.DirectoryInfo("Projekte");

            DarstellungAendern(ProjektStarten_BUG, true, Colors.Green, "Projekt starten");
            DarstellungAendern(ProjektStarten_PLC, true, Colors.Green, "Projekt starten");
            DarstellungAendern(ProjektStarten_PLC_NC, true, Colors.Green, "Projekt starten");
            DarstellungAendern(ProjektStarten_PLC_VISU, true, Colors.Green, "Projekt starten");

            ProjektName = rb.Name;

            string DateiName = ParentDirectory.FullName + "\\" + rb.Name + "\\index.html";
            string HtmlSeite = System.IO.File.ReadAllText(DateiName);
            string LeereHtmlSeite = "<!doctype html>   </html >";

            Web_PLC.NavigateToString(LeereHtmlSeite);
            Web_PLC_VISU.NavigateToString(LeereHtmlSeite);
            Web_PLC_NC.NavigateToString(LeereHtmlSeite);
            Web_BUG.NavigateToString(LeereHtmlSeite);

            if (rb.Name.Contains("PLC"))
            {
                if (rb.Name.Contains("VISU"))
                    Web_PLC_VISU.NavigateToString(HtmlSeite);
                else
                {
                    if (rb.Name.Contains("NC")) Web_PLC_NC.NavigateToString(HtmlSeite);
                    else Web_PLC.NavigateToString(HtmlSeite);
                }
            }
            else
            {
                if (rb.Name.Contains("BUG")) Web_BUG.NavigateToString(HtmlSeite);
                //bei Bug gibt es keine Unterkategorien
            }
        }

        private void ProjektStarten(object sender, RoutedEventArgs e)
        {

            System.IO.DirectoryInfo ParentDirectory = new System.IO.DirectoryInfo("Projekte");
            string sourceDirectory = ParentDirectory.FullName + "\\" + ProjektName;


            DarstellungAendern(ProjektStarten_BUG, true, Colors.Yellow, "Ordner " + ProjektPfad + " löschen");
            DarstellungAendern(ProjektStarten_PLC, true, Colors.Yellow, "Ordner " + ProjektPfad + " löschen");
            DarstellungAendern(ProjektStarten_PLC_NC, true, Colors.Yellow, "Ordner " + ProjektPfad + " löschen");
            DarstellungAendern(ProjektStarten_PLC_VISU, true, Colors.Yellow, "Ordner " + ProjektPfad + " löschen");
            if (System.IO.Directory.Exists(ProjektPfad)) System.IO.Directory.Delete(ProjektPfad, true);

            DarstellungAendern(ProjektStarten_BUG, true, Colors.Yellow, "Ordner " + ProjektPfad + " erstellen");
            DarstellungAendern(ProjektStarten_PLC, true, Colors.Yellow, "Ordner " + ProjektPfad + " erstellen");
            DarstellungAendern(ProjektStarten_PLC_NC, true, Colors.Yellow, "Ordner " + ProjektPfad + " erstellen");
            DarstellungAendern(ProjektStarten_PLC_VISU, true, Colors.Yellow, "Ordner " + ProjektPfad + " erstellen");
            System.IO.Directory.CreateDirectory(ProjektPfad);

            DarstellungAendern(ProjektStarten_BUG, true, Colors.Yellow, "Alle Dateien kopieren");
            DarstellungAendern(ProjektStarten_PLC, true, Colors.Yellow, "Alle Dateien kopieren");
            DarstellungAendern(ProjektStarten_PLC_NC, true, Colors.Yellow, "Alle Dateien kopieren");
            DarstellungAendern(ProjektStarten_PLC_VISU, true, Colors.Yellow, "Alle Dateien kopieren");
            Copy(sourceDirectory, ProjektPfad);

            DarstellungAendern(ProjektStarten_BUG, true, Colors.LawnGreen, "Projekt mit TwinCAT V3 öffnen");
            DarstellungAendern(ProjektStarten_PLC, true, Colors.LawnGreen, "Projekt mit TwinCAT V3 öffnen");
            DarstellungAendern(ProjektStarten_PLC_NC, true, Colors.LawnGreen, "Projekt mit TwinCAT V3 öffnen");
            DarstellungAendern(ProjektStarten_PLC_VISU, true, Colors.LawnGreen, "Projekt mit TwinCAT V3 öffnen");
            Process proc = new Process();
            proc.StartInfo.FileName = ProjektPfad + "\\start.cmd";
            proc.StartInfo.WorkingDirectory = ProjektPfad;
            proc.Start();
        }


        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(System.IO.Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        private void TabControl_SelectionChanged(object sender, RoutedEventArgs e)
        {

            DarstellungAendern(ProjektStarten_BUG, false, Colors.Gray, "Projekt auswählen");
            DarstellungAendern(ProjektStarten_PLC, false, Colors.Gray, "Projekt auswählen");
            DarstellungAendern(ProjektStarten_PLC_NC, false, Colors.Gray, "Projekt auswählen");
            DarstellungAendern(ProjektStarten_PLC_VISU, false, Colors.Gray, "Projekt auswählen");
            AlleRadioButtonsDeaktivieren();


            string LeereHtmlSeite = "<!doctype html>   </html >";
            Web_PLC.NavigateToString(LeereHtmlSeite);
            Web_PLC_VISU.NavigateToString(LeereHtmlSeite);
            Web_PLC_NC.NavigateToString(LeereHtmlSeite);
            Web_BUG.NavigateToString(LeereHtmlSeite);
        }


        private void DarstellungAendern(Button Knopf, bool Enable, Color Farbe, string Text)
        {
            Knopf.IsEnabled = Enable;
            Knopf.Background = new SolidColorBrush(Farbe);
            Knopf.Content = Text;
            Knopf.Refresh();
        }

        private void AlleRadioButtonsDeaktivieren()
        {
            foreach (RadioButton R_Button in RadioButtonList)
            {
                if (R_Button.IsChecked == true) R_Button.IsChecked = false;
            }
        }

    }
}
