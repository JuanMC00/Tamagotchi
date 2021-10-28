using System;
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
using System.Windows.Shapes;

namespace Tamagotchi
{
    /// <summary>
    /// Lógica de interacción para ventanaBienvenido.xaml
    /// </summary>
    public partial class ventanaBienvenido : Window
    {
        MainWindow pantallaPrincipal;
        public ventanaBienvenido(MainWindow pantallaPrincipal)
        {
            InitializeComponent();
            this.pantallaPrincipal = pantallaPrincipal;
        }

        private void enviarNombre(object sender, RoutedEventArgs e)
        {
            if (txtNombreTamagotchi.Text != "")
            {
                if (txtNombreTamagotchi.Text.ToString().Contains(" "))
                {
                    lblError.Visibility = Visibility.Visible;
                }

                else
                {
                    pantallaPrincipal.setNombre(txtNombreTamagotchi.Text);
                    this.Close();
                }
            }
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            pantallaPrincipal.setNombre("");
            pantallaPrincipal.Close();
        }
    }
}
