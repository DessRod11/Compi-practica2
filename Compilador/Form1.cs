namespace Compilador
{
    public partial class frmCompilador : Form
    {
        public frmCompilador()
        {
            InitializeComponent();
        }

        private void frmSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmNuevo_Click(object sender, EventArgs e)
        {
            limpiar();
        }

        private void limpiar()
        {
            try
            {
                txtFuente.Clear();
                txtTokens.Clear();
                txtEstatus.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al limpiar los campos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmAbrir_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    limpiar();
                    string filePath = openFileDialog1.FileName;
                    string fileContent = File.ReadAllText(filePath);
                    txtFuente.Text = fileContent;

                    txtEstatus.AppendText("Archivo abierto: " + filePath + Environment.NewLine);
                }
                else
                {
                    txtEstatus.AppendText("Operación de apertura cancelada." + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el archivo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog1.FileName;
                    File.WriteAllText(filePath, txtFuente.Text);
                    txtEstatus.AppendText("Archivo guardado: " + filePath + Environment.NewLine);
                }
                else
                {
                    txtEstatus.AppendText("Operación de guardado cancelada." + Environment.NewLine);
                }


            } catch (Exception ex) {
                MessageBox.Show("Error al guardar el archivo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
    }
}
