using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Tamagotchi
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string nombre;
        bool gameOver = false;

        //Temporizadores
        DispatcherTimer timer;
        double decremento = 2.5;
        int puntuacion = 0;
        int timeraux = 0;
        int timeraux2 = 0;

        //Jugabilidad
        bool duplicarPuntuacion = false;
        bool [] logros = new bool[3];

        public MainWindow()
        {
            InitializeComponent();
            ventanaBienvenido pantallaInicio = new ventanaBienvenido(this);
            pantallaInicio.ShowDialog();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000.0);
            timer.Tick += new EventHandler(reloj);
            timer.Start();
            logros[0] = false;
            logros[1] = false;
            logros[2] = false;

            leerFichero();

            if (nombre != "")
            {
                StreamWriter fichero;
                fichero = File.AppendText("Jugadores.txt");
                fichero.Write(nombre);
                fichero.Close();
            }

        }

        private void leerFichero() {
            StreamReader fichero;
            List<Jugador> jugadores = new List<Jugador>();
            string linea;

            fichero = File.OpenText("Jugadores.txt");

            do
            {
                Jugador jugador = new Jugador();
                linea = fichero.ReadLine();

                if (linea == null)
                    break;

                jugador.nombre = linea.Split(' ')[0];
                jugador.score = linea.Split(' ')[1];
                jugadores.Add(jugador);

            } while (linea != null);

            jugadores = jugadores.OrderBy(x => x.score).Reverse().ToList();

            foreach (Jugador element in jugadores)
            {
                TextBlock item = new TextBlock();
                item.FontSize = 14;
                item.FontFamily = new FontFamily("Kristen ITC");
                item.Text = element.score.ToString() + "\t" + element.nombre.ToString();
                spnRanking.Children.Add(item);
            }

            fichero.Close();
        }

        private void reloj(object sender, EventArgs e)
        {
            timeraux++;

            if (timeraux2 > 10)
            {
                timeraux2 = 0;
                duplicarPuntuacion = false;
            }

            puntuacion++;
            if (duplicarPuntuacion)
            {
                puntuacion++;
                timeraux2++;
            }

            this.pbEnergia.Value -= 1;
            this.pbCansancio.Value -= 1;
            this.pbDiversion.Value -= decremento;

            if (pbEnergia.Value <= 0 || pbCansancio.Value <= 0 || pbDiversion.Value <= 0)
            {
                timer.Stop();
                game_over();
            }

            //Logros
            TextBlock logro1 = new TextBlock();
            if (timeraux == 60) {
                logro1.Text = "Has conseguido superar\nel minuto de juego\n";
                logro1.FontSize = 12;
                logro1.FontFamily = new FontFamily("Kristen ITC");
                spnLogros.Children.Add(logro1);

                //Habilidad barras al máximo
                imgFull.IsEnabled = true;
                imgFull.Visibility = Visibility.Visible;
            }

            TextBlock logro2 = new TextBlock();
            if (puntuacion/5 + 10 == 40)
            {
                logro2.Text = "Has conseguido superar\nel record del creador\n";
                logro2.FontSize = 12;
                logro2.FontFamily = new FontFamily("Kristen ITC");
                spnLogros.Children.Add(logro2);
            }

            //Habilidad aleatorio
            if (timeraux % 20 == 0)
            {
                imgRandom.IsEnabled = true;
                imgRandom.Visibility = Visibility.Visible;
            }

            //Habilidad duplicar score
            if (timeraux % 30 == 0) {
                imgx2.IsEnabled = true;
                imgx2.Visibility = Visibility.Visible;
            }

            if (timeraux % 70 == 0){
                imgFreeze.IsEnabled = true;
                imgFreeze.Visibility = Visibility.Visible;
            }

            //Aumentamos dificultad
            if (timeraux % 50 == 0)
                decremento += 5;
        }

        private void game_over(){
            this.lblGameOver.Visibility = Visibility.Visible;
            this.btnComer.IsEnabled = false;
            this.btnDormir.IsEnabled = false;
            this.btnJugar.IsEnabled = false;
            this.lblPuntuacionTitulo.Visibility = Visibility.Visible;
            this.lblPuntuacion.Visibility = Visibility.Visible;
            this.lblPuntuacion.Content = ((puntuacion/ 5 + 10)).ToString();
            gameOver = true;

            if (nombre != "")
            {
                StreamWriter fichero;
                fichero = File.AppendText("Jugadores.txt");
                fichero.WriteLine(" " + (puntuacion / 5 + 10));
                fichero.Close();
            }

            //Animaciones:
            //Animacion hambre:
            pbEnergia.Foreground = Brushes.Yellow;
            elipgrande.Visibility = Visibility.Hidden;
            elipmediana.Visibility = Visibility.Hidden;
            elippequeña.Visibility = Visibility.Hidden;
            imgPizza.Visibility = Visibility.Hidden;

            //Aniamcion triste:
            boca.Visibility = Visibility.Visible;
            bocaTriste.Visibility = Visibility.Hidden;


        }

        private void btnComer_Click(object sender, RoutedEventArgs e)
        {
            pbEnergia.Value += 10;
            decremento += 2.5;

            //Animacion masticar:
            btnComer.IsEnabled = false;
            Storyboard staux = (Storyboard)this.Resources["animacionComer"];
            staux.Completed += new EventHandler(animacionMasticarEnd);
            staux.Begin();
        }

        private void animacionMasticarEnd(object sender, EventArgs e)
        {            
            if(!gameOver)
                btnComer.IsEnabled = true;
        }

        private void btnDormir_Click(object sender, RoutedEventArgs e)
        {
            pbCansancio.Value += 10;
            decremento += 2.5;

            //Animaacion de cerrar ojos:
            btnDormir.IsEnabled = false;

            DoubleAnimation cerrarOjoDcho = new DoubleAnimation();
            cerrarOjoDcho.To = 5;
            cerrarOjoDcho.Duration = new Duration(TimeSpan.FromSeconds(1));
            cerrarOjoDcho.AutoReverse = true;
            cerrarOjoDcho.Completed += new EventHandler(cerrarOjoDchoEnd);
            ojoDcho.BeginAnimation(Ellipse.HeightProperty, cerrarOjoDcho);

            DoubleAnimation cerrarOjoIzq = new DoubleAnimation();
            cerrarOjoIzq.To = 5;
            cerrarOjoIzq.Duration = new Duration(TimeSpan.FromSeconds(1));
            cerrarOjoIzq.AutoReverse = true;
            ojoIzq.BeginAnimation(Ellipse.HeightProperty, cerrarOjoIzq);

        }

        private void cerrarOjoDchoEnd(object sender, EventArgs e)
        {
            if (!gameOver)
                btnDormir.IsEnabled = true;
        }

        private void btnJugar_Click(object sender, RoutedEventArgs e)
        {
            pbDiversion.Value += 10;
            if (decremento > 1)
            {
                decremento += 1;
            }

            this.pbEnergia.Value -= decremento+4;
            this.pbCansancio.Value -= decremento+4;

            //Animacion pelota:
            btnJugar.IsEnabled = false;
            Storyboard staux = (Storyboard)this.Resources["animacionJugar"];
            staux.Completed += new EventHandler(animacionJugarEnd);
            staux.Begin();

            //Animacion pierna:
            Storyboard staux2 = (Storyboard)this.Resources["animacionMoverPierna"];
            staux2.Completed += new EventHandler(animacionJugarEnd);
            staux2.Begin();


        }

        private void animacionJugarEnd(object sender, EventArgs e)
        {
            if (!gameOver)
                btnJugar.IsEnabled = true;
        }

        private void pbEnergia_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (pbEnergia.Value >= 50)
            {
                pbEnergia.Foreground = Brushes.Green;
                elipgrande.Visibility = Visibility.Hidden;
                elipmediana.Visibility = Visibility.Hidden;
                elippequeña.Visibility = Visibility.Hidden;
                imgPizza.Visibility = Visibility.Hidden;
            }

            if (pbEnergia.Value < 50 && pbEnergia.Value >= 25)
            {
                pbEnergia.Foreground = Brushes.Yellow;
                elipgrande.Visibility = Visibility.Hidden;
                elipmediana.Visibility = Visibility.Hidden;
                elippequeña.Visibility = Visibility.Hidden;
                imgPizza.Visibility = Visibility.Hidden;
            }

            if (pbEnergia.Value < 25)
            {
                pbEnergia.Foreground = Brushes.Red;
                elipgrande.Visibility = Visibility.Visible;
                elipmediana.Visibility = Visibility.Visible;
                elippequeña.Visibility = Visibility.Visible;
                imgPizza.Visibility = Visibility.Visible;
            }

        }

        private void pbCansancio_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (pbCansancio.Value >= 50)
            {
                pbCansancio.Foreground = Brushes.Green;
            }

            if (pbCansancio.Value < 50 && pbCansancio.Value >= 25)
            {
                pbCansancio.Foreground = Brushes.Yellow;

            }

            if (pbCansancio.Value < 25)
            {
                pbCansancio.Foreground = Brushes.Red;
            }

        }

        private void pbDiversion_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (pbDiversion.Value >= 50)
            {
                pbDiversion.Foreground = Brushes.Green;
                boca.Visibility = Visibility.Visible;
                bocaTriste.Visibility = Visibility.Hidden;
            }

            if (pbDiversion.Value < 50 && pbDiversion.Value >= 25)
            {
                pbDiversion.Foreground = Brushes.Yellow;
                boca.Visibility = Visibility.Visible;
                bocaTriste.Visibility = Visibility.Hidden;
            }

            if (pbDiversion.Value < 25)
            {
                pbDiversion.Foreground = Brushes.Red;
                boca.Visibility = Visibility.Hidden;
                bocaTriste.Visibility = Visibility.Visible;
            }

        }

        private void cambiarFondo(object sender, MouseButtonEventArgs e)
        {
            if (!logros[1])
            {
                TextBlock logro = new TextBlock();
                logro.Text = "Has cambiado el fondo\n";
                logro.FontSize = 12;
                logro.FontFamily = new FontFamily("Kristen ITC");
                spnLogros.Children.Add(logro);
                logros[1] = true;
            }

            this.imgFondo.Source = ((Image)sender).Source;
        }

        

        private void acercaDe(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult resultado = MessageBox.Show("Programa realizdo por:\nJuan Muñoz\n\n¿Salir de la aplicación?", "Acerca de", MessageBoxButton.YesNo);

            if (resultado==MessageBoxResult.Yes) //Los botones por defecto cierran el MessageBox por lo que no hace falta programar el no.
                this.Close();
                    
        }

        public void setNombre(string nombre)
        {
            this.nombre = nombre;
            lblNombre.Content = "Bienvenido, " + nombre;
        }

        private void inicioArrastrarSombrero(object sender, MouseButtonEventArgs e)
        {
            DataObject datos = new DataObject((Image)sender);
            DragDrop.DoDragDrop((Image)sender, datos, DragDropEffects.Move);
        }

        private void inicioArrastrarLogo(object sender, MouseButtonEventArgs e)
        {
            DataObject datos = new DataObject((Image)sender);
            DragDrop.DoDragDrop((Image)sender, datos, DragDropEffects.Move);
        }

        private void ocultarLogo(object sender, MouseButtonEventArgs e)
        {
            imgPizzaPlanet.Visibility = Visibility.Hidden;
        }

        private void ocultarSombrero(object sender, MouseButtonEventArgs e)
        {
            imgSombrero.Visibility = Visibility.Hidden;
        }

        private void colocarColeccionable(object sender, DragEventArgs e)
        {
            Image imgAux = (Image)e.Data.GetData(typeof(Image));

            if (!logros[2])
            {
                TextBlock logro = new TextBlock();
                logro.Text = "Has utilizado un cosmético\n";
                logro.FontSize = 12;
                logro.FontFamily = new FontFamily("Kristen ITC");
                spnLogros.Children.Add(logro);
                logros[2] = true;
            }

            switch (imgAux.Name) {
                case "imgSombreroMini":
                    imgSombrero.Visibility = Visibility.Visible;
                    break;

                case "imgPizzaPlanetMini":
                    imgPizzaPlanet.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (btnStop.Content.Equals("Parar"))
            {
                timer.Stop();
                btnStop.Content = "Reanudar";
            }
            else
            {
                timer.Start();
                btnStop.Content = "Parar";
            }
            
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            gameOver = false;

            decremento = 2.5;
            puntuacion = 0;
            timeraux = 0;
            timeraux2 = 0;

            pbEnergia.Value = 90;
            pbCansancio.Value = 90;
            pbDiversion.Value = 90;

            duplicarPuntuacion = false;
            logros[0] = false;
            logros[1] = false;
            logros[2] = false;

            spnLogros.Children.Clear();
            lblPuntuacion.Visibility = Visibility.Hidden;
            lblPuntuacionTitulo.Visibility = Visibility.Hidden;

            btnComer.IsEnabled = true;
            btnDormir.IsEnabled = true;
            btnJugar.IsEnabled = true;

            StreamWriter fichero;
            fichero = File.AppendText("Jugadores.txt");
            fichero.Write(nombre);
            fichero.Close();

            timer.Start();
        }

        private void usarHabilidad(object sender, MouseButtonEventArgs e)
        {
            String habilidad = ((Image)sender).Source.ToString();
            String[] aux = habilidad.Split('/');

            if (!logros[0])
            {
                TextBlock logro = new TextBlock();
                logro.Text = "Has usado una habilidad\n";
                logro.FontSize = 12;
                logro.FontFamily = new FontFamily("Kristen ITC");
                spnLogros.Children.Add(logro);
                logros[0] = true;
            }

            switch (aux[aux.Length - 1]) {
                case "Rayo.png":
                    pbEnergia.Value = 100;
                    pbCansancio.Value = 100;
                    pbDiversion.Value = 100;

                    imgFull.IsEnabled = false;
                    imgFull.Visibility = Visibility.Hidden;
                    break;

                case "Reloj.png":
                    decremento = 1;
                    imgFreeze.IsEnabled = false;
                    imgFreeze.Visibility = Visibility.Hidden;
                    break;

                case "x2.png":
                    duplicarPuntuacion = true;
                    imgx2.IsEnabled = false;
                    imgx2.Visibility = Visibility.Hidden;
                    break;

                case "Random.png":
                    var rand = new Random();
                    pbEnergia.Value = rand.Next(10, 101);
                    pbCansancio.Value = rand.Next(10, 101);
                    pbDiversion.Value = rand.Next(10, 101);
                    imgRandom.IsEnabled = false;
                    imgRandom.Visibility = Visibility.Hidden;
                    break;

            }

        }

        private void salir(object sender, EventArgs e)
        {
            if (!gameOver)
            {
                game_over();
            }
        }
    }

    public class Jugador
    {
        public string nombre { get; set; }
        public string score { get; set; }

    }
}
