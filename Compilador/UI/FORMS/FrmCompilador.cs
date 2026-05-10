
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Windows.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Compilador.UI.Forms
{
    public partial class FrmCompilador : Form
    {
        private RichTextBox txtEditor;
        private Panel pnlLineNumbers;
        private bool isDarkTheme = false;

        public FrmCompilador()
        {
            InitializeComponent();
            InicializarEditor();
            AplicarTemaClaro();
            gridSimbolos.Rows.Clear();

            btnTema.Click += BtnTema_Click;
        }



        private void InicializarGrid()
        {
            gridSimbolos.Columns.Clear();

            gridSimbolos.Columns.Add("Linea", "Linea");
            gridSimbolos.Columns.Add("Tipo", "Tipo");
            gridSimbolos.Columns.Add("Lexema", "Lexema");

            gridSimbolos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }


        private void InicializarEditor()
        {
            pnlLineNumbers = new Panel
            {
                Dock = DockStyle.Left,
                Width = 50
            };
            pnlLineNumbers.Paint += PnlLineNumbers_Paint;

            txtEditor = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 11F),
                WordWrap = false,
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            txtEditor.VScroll += (s, e) => pnlLineNumbers.Invalidate();
            txtEditor.TextChanged += (s, e) => pnlLineNumbers.Invalidate();
            txtEditor.Resize += (s, e) => pnlLineNumbers.Invalidate();

            splitEditor.Panel1.Controls.Add(txtEditor);
            splitEditor.Panel1.Controls.Add(pnlLineNumbers);
        }


        private void PnlLineNumbers_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(pnlLineNumbers.BackColor);

            int firstLine = txtEditor.GetLineFromCharIndex(
                txtEditor.GetCharIndexFromPosition(new Point(0, 0)));

            int lastLine = txtEditor.GetLineFromCharIndex(
                txtEditor.GetCharIndexFromPosition(
                    new Point(0, txtEditor.Height)));

            int lineHeight = txtEditor.Font.Height;
            int y = 2;

            for (int i = firstLine; i <= lastLine + 1; i++)
            {
                string line = (i + 1).ToString();
                SizeF size = e.Graphics.MeasureString(line, txtEditor.Font);

                e.Graphics.DrawString(
                    line,
                    txtEditor.Font,
                    Brushes.Gray,
                    pnlLineNumbers.Width - size.Width - 5,
                    y
                );

                y += lineHeight;
            }
        }


        private void BtnTema_Click(object sender, EventArgs e)
        {
            isDarkTheme = !isDarkTheme;

            if (isDarkTheme)
                AplicarTemaOscuro();
            else
                AplicarTemaClaro();
        }

        private void AplicarTemaClaro()
        {
            txtEditor.BackColor = Color.White;
            txtEditor.ForeColor = Color.Black;
            txtTokens.BackColor = Color.White;
            txtTokens.ForeColor = Color.Black;
            txtEstatus.BackColor = Color.White;
            txtEstatus.ForeColor = Color.Black;
            gridSimbolos.BackgroundColor = Color.White;
            gridSimbolos.DefaultCellStyle.BackColor = Color.White;
            gridSimbolos.DefaultCellStyle.ForeColor = Color.Black;
            pnlLineNumbers.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void AplicarTemaOscuro()
        {
            txtEditor.BackColor = Color.FromArgb(30, 30, 30);
            txtEditor.ForeColor = Color.Gainsboro;
            txtTokens.BackColor = Color.FromArgb(30, 30, 30);
            txtTokens.ForeColor = Color.Gainsboro;
            txtEstatus.BackColor = Color.FromArgb(30, 30, 30);
            txtEstatus.ForeColor = Color.Gainsboro;
            gridSimbolos.BackgroundColor = Color.FromArgb(30, 30, 30);
            gridSimbolos.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            gridSimbolos.DefaultCellStyle.ForeColor = Color.Gainsboro;
            pnlLineNumbers.BackColor = Color.FromArgb(45, 45, 48);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            limpiar();
        }
        private void limpiar()
        {
            try
            {
                txtEditor.Clear();
                txtTokens.Clear();
                txtEstatus.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al limpiar los campos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    limpiar();
                    string filePath = openFileDialog1.FileName;
                    string fileContent = File.ReadAllText(filePath);
                    txtEditor.Text = fileContent;

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

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.Filter = "Archivos de texto (*.txt)|*.txt|Todos los archivos (*.*)|*.*";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog1.FileName;
                    File.WriteAllText(filePath, txtEditor.Text);
                    txtEstatus.AppendText("Archivo guardado: " + filePath + Environment.NewLine);
                }
                else
                {
                    txtEstatus.AppendText("Operación de guardado cancelada." + Environment.NewLine);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el archivo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCompilar_Click(object sender, EventArgs e)
        {

            try
            {
                // 1. Limpiar controles de salida
                txtTokens.Clear();
                txtEstatus.Clear();

                // 2. Mostrar mensaje inicial
                txtEstatus.Text = "Fase 1 [Léxico] INICIADO." + Environment.NewLine;

                // 3. Obtener texto del editor
                string textoFuente = txtEditor.Text;

                // 4. Crear instancia de Código Fuente
                CodigoFuente codigoFuente = CodigoFuente.DesdeTexto(textoFuente);

                // Confirmación visual
                txtEstatus.AppendText($"Total de líneas detectadas: {codigoFuente.NumeroLineas}" + Environment.NewLine);

                // --- AQUÍ ESTÁ LO NUEVO ---

                // 5. Ejecutar el análisis léxico
                AnalizadorLexico lexer = new AnalizadorLexico();
                ResultadoLexico resultado = lexer.Analizar(codigoFuente);

                // 6. Mostrar los avisos (Léxico iniciado, finalizado, errores, etc.)
                foreach (string aviso in resultado.Avisos)
                {
                    txtEstatus.AppendText(aviso + Environment.NewLine);
                }

                // 7. Mostrar tokens agrupados por línea
                var tokensAgrupados = resultado.Tokens
                    .GroupBy(t => t.Linea)
                    .OrderBy(g => g.Key);

                foreach (var grupo in tokensAgrupados)
                {
                    txtTokens.AppendText($"[Linea {grupo.Key}]  ");
                    foreach (var token in grupo)
                    {
                        txtTokens.AppendText($"[{token.Tipo}: {token.Lexema}] ");
                    }
                    txtTokens.AppendText(Environment.NewLine);
                }

                // 8. Fase Sintáctica (Fuera del bucle de tokens)
                txtEstatus.AppendText("Fase 2 [Sintáctico] INICIADO" + Environment.NewLine);
                var analizadorSintactico = new AnalizadorSintactico();
                analizadorSintactico.Parse(resultado.Tokens);
                
                if (analizadorSintactico.Errores.Count == 0)
                {
                    txtEstatus.AppendText("Análisis Sintáctico finalizado con éxito" + Environment.NewLine);
                }
                else
                {
                    foreach (var error in analizadorSintactico.Errores)
                    {
                        txtEstatus.AppendText(error + Environment.NewLine);
                    }
                }
            }






            catch (Exception ex)
            {
                MessageBox.Show("Error al compilar: " + ex.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void txtTokens_TextChanged(object sender, EventArgs e)
        {

        }

        private void gridSimbolos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }

}
    
