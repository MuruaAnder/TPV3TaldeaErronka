﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NHibernate;

namespace _2taldea
{
    public partial class MesaDetallesForm : Form
    {
        private int mesaId;
        private ISessionFactory sessionFactory;
        private Dictionary<string, (int cantidad, float precio)> resumen = new Dictionary<string, (int cantidad, float precio)>();
        private int ultimoEskaeraZenb;

        public MesaDetallesForm(int mesaId, ISessionFactory sessionFactory)
        {
            InitializeComponent();
            this.mesaId = mesaId;

            if (mesaId <= 0)
            {
                throw new ArgumentException("El ID de la mesa debe ser mayor a 0.");
            }
            this.sessionFactory = sessionFactory;
            this.mesaLabel.Text = $"Mahaia: {mesaId}";
            this.tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            CargarPlatos("Edaria", tabControl.TabPages[0]);
            CargarPedidosGuardados();
        }

        private void MesaDetallesForm_Load(object sender, EventArgs e)
        {
            // Crear el TabControl si no existe
            TabControl tabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(tabControl);

            // Crear las pestañas
            TabPage bebidasTab = new TabPage("Edaria");
            TabPage primerPlatoTab = new TabPage("Lehenengo platera");
            TabPage segundoPlatoTab = new TabPage("Bigarren platera");

            tabControl.TabPages.AddRange(new[] { bebidasTab, primerPlatoTab, segundoPlatoTab });

            // Crear el Label para mostrar el número de la mesa
            Label mesaLabel = new Label
            {
                Text = $"Mahaia: {mesaId}",
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.DarkSlateGray,
                Location = new Point(this.ClientSize.Width - 150, 10), // Ajusta la ubicación
                Anchor = AnchorStyles.Top | AnchorStyles.Right // Fijar el Label a la derecha
            };

            // Asegurarse de que solo se agregue una vez
            if (!this.Controls.Contains(mesaLabel))
            {
                this.Controls.Add(mesaLabel);
            }

            // Cargar los platos en la primera pestaña
            CargarPlatos("Edaria", bebidasTab); // Se carga de inmediato
        }


        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            if (tabControl != null)
            {
                TabPage selectedTab = tabControl.SelectedTab;
                string selectedCategory = selectedTab.Text;

                // Evitar cargar los productos más de una vez
                if (selectedTab.Controls.Count == 0)
                {
                    // Usamos BeginInvoke para asegurarnos de que la carga de los platos se realice correctamente después del cambio de pestaña
                    selectedTab.BeginInvoke((Action)(() =>
                    {
                        CargarPlatos(selectedCategory, selectedTab);
                    }));
                }
            }
        }

        private void CargarPlatos(string kategoria, TabPage tabPage)
        {
            try
            {
                using (ISession session = sessionFactory.OpenSession())
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var platos = session.QueryOver<Platera>()
                                        .Where(p => p.Kategoria == kategoria)
                                        .List();

                    // *Panel principal (ocupa toda la TabPage)*
                    Panel mainPanel = new Panel
                    {
                        Dock = DockStyle.Fill,
                        BackgroundImage = Image.FromFile("background.png"),
                        BackgroundImageLayout = ImageLayout.Stretch
                    };

                    // *Panel con scroll para los platos*
                    Panel platosPanel = new Panel
                    {
                        Dock = DockStyle.Fill,
                        AutoScroll = true,
                        BackColor = Color.Transparent
                    };

                    // Sincronizar el fondo con mainPanel
                    platosPanel.BackgroundImage = mainPanel.BackgroundImage;
                    platosPanel.BackgroundImageLayout = mainPanel.BackgroundImageLayout;

                    // *Tabla para organizar los platos en 3 columnas*
                    TableLayoutPanel tablaPlatos = new TableLayoutPanel
                    {
                        AutoSize = true,
                        ColumnCount = 3,
                        Padding = new Padding(0),
                        Margin = new Padding(0),
                        BackColor = Color.Transparent
                    };

                    // Asegurar que las columnas se ajustan bien
                    for (int i = 0; i < tablaPlatos.ColumnCount; i++)
                        tablaPlatos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3F));

                    // *FlowLayoutPanel para centrar la tabla de platos*
                    FlowLayoutPanel contenedorCentrado = new FlowLayoutPanel
                    {
                        Dock = DockStyle.Top,
                        AutoSize = true,
                        FlowDirection = FlowDirection.LeftToRight,
                        Padding = new Padding(490, 0, 0, 0),
                        BackColor = Color.Transparent
                    };

                    contenedorCentrado.Controls.Add(tablaPlatos);
                    platosPanel.Controls.Add(contenedorCentrado);

                    int row = 0, col = 0;
                    foreach (var plato in platos)
                    {
                        Panel productoPanel = new Panel
                        {
                            Width = 225,
                            Height = 225,
                            Margin = new Padding(40),
                            BorderStyle = BorderStyle.FixedSingle,
                            BackColor = Color.SaddleBrown
                        };

                        Label lblNombre = new Label
                        {
                            Text = plato.Izena,
                            AutoSize = true,
                            Font = new Font("Segoe UI", 12, FontStyle.Bold),
                            ForeColor = Color.White,
                            Location = new Point(10, 10)
                        };
                        productoPanel.Controls.Add(lblNombre);

                        Label lblPrecio = new Label
                        {
                            Text = $"Prezioa: {plato.Prezioa:C2}",
                            AutoSize = true,
                            ForeColor = Color.White,
                            Location = new Point(10, 40)
                        };
                        productoPanel.Controls.Add(lblPrecio);

                        Button btnPlus = new Button
                        {
                            Text = "+",
                            Width = 50,
                            Height = 40,
                            Location = new Point(10, 70)
                        };
                        productoPanel.Controls.Add(btnPlus);

                        Button btnMinus = new Button
                        {
                            Text = "-",
                            Width = 50,
                            Height = 40,
                            Location = new Point(70, 70)
                        };
                        productoPanel.Controls.Add(btnMinus);

                        Label lblCantidad = new Label
                        {
                            Text = "0",
                            Width = 50,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Location = new Point(130, 70),
                            ForeColor = Color.White
                        };
                        productoPanel.Controls.Add(lblCantidad);

                        btnPlus.Click += (s, e) =>
                        {
                            int cantidad = int.Parse(lblCantidad.Text);
                            cantidad++;
                            lblCantidad.Text = cantidad.ToString();

                            if (!resumen.ContainsKey(plato.Izena))
                                resumen[plato.Izena] = (0, plato.Prezioa);

                            resumen[plato.Izena] = (cantidad, plato.Prezioa);
                        };

                        btnMinus.Click += (s, e) =>
                        {
                            int cantidad = int.Parse(lblCantidad.Text);
                            if (cantidad > 0)
                            {
                                cantidad--;
                                lblCantidad.Text = cantidad.ToString();

                                if (resumen.ContainsKey(plato.Izena))
                                    resumen[plato.Izena] = (cantidad, plato.Prezioa);
                            }
                        };

                        tablaPlatos.Controls.Add(productoPanel, col, row);

                        col++;
                        if (col >= 3)
                        {
                            col = 0;
                            row++;
                        }
                    }

                    // *Forzar el redibujado de platosPanel y tablaPlatos*
                    platosPanel.Invalidate();
                    tablaPlatos.Invalidate();

                    // *Panel para los botones (fijo en la parte inferior)*
                    Panel botonesPanel = new Panel
                    {
                        Dock = DockStyle.Bottom,
                        Height = 100,
                        BackColor = Color.Transparent
                    };

                    FlowLayoutPanel flowBotones = new FlowLayoutPanel
                    {
                        Dock = DockStyle.Fill,
                        FlowDirection = FlowDirection.LeftToRight,
                        AutoSize = true,
                        Padding = new Padding(490, 0, 0, 0),
                        BackColor = Color.Transparent
                    };

                    Button btnGuardar = new Button
                    {
                        Text = "Pedidoa gorde",
                        BackColor = Color.Green,
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        Width = 200,
                        Height = 50,
                        Margin = new Padding(20, 0, 20, 0)
                    };
                    btnGuardar.Click += BtnGuardar_Click;

                    Button btnResumen = new Button
                    {
                        Text = "Laburpena ikusi",
                        BackColor = Color.Blue,
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        Width = 200,
                        Height = 50,
                        Margin = new Padding(20, 0, 20, 0)
                    };
                    btnResumen.Click += BtnResumen_Click;

                    Button btnBorrar = new Button
                    {
                        Text = "Pedidoa ezabatu",
                        BackColor = Color.Red,
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        Width = 200,
                        Height = 50,
                        Margin = new Padding(20, 0, 20, 0)
                    };
                    btnBorrar.Click += BtnBorrar_Click;

                    Button btnAtzera = new Button
                    {
                        Text = "Atzera",
                        BackColor = Color.Gray,
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                        Width = 200,
                        Height = 50,
                        Margin = new Padding(20, 0, 20, 0)
                    };
                    btnAtzera.Click += (s, e) => { this.Close(); };

                    flowBotones.Controls.AddRange(new Control[] { btnGuardar, btnResumen, btnBorrar, btnAtzera });
                    botonesPanel.Controls.Add(flowBotones);

                    // *Añadir todo al panel principal*
                    mainPanel.Controls.Add(platosPanel);
                    mainPanel.Controls.Add(botonesPanel);

                    // *Añadir el mainPanel a la TabPage*
                    tabPage.Controls.Add(mainPanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kategoriako platerak kargatzean arazoak {kategoria}: {ex.Message}", "Arazoak", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }






        private void CargarPedidosGuardados()
        {
            try
            {
                using (ISession session = sessionFactory.OpenSession())
                {
                    var lastEskaera = session.QueryOver<Eskaera>()
                                              .Where(e => e.MesaId == mesaId && e.Activo == true)
                                              .OrderBy(e => e.EskaeraZenb).Desc
                                              .Take(1)
                                              .SingleOrDefault();

                    ultimoEskaeraZenb = lastEskaera?.EskaeraZenb ?? 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gordetako pedidoak ez dira kargatu: {ex.Message}", "Arazoak", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static int ObtenerNuevoEskaeraZenb(ISession session)
        {
            var lastEskaera = session.QueryOver<Eskaera>()
                                     .Where(e => e.Activo == true)
                                     .OrderBy(e => e.EskaeraZenb).Desc
                                     .Take(1)
                                     .SingleOrDefault();

            return (lastEskaera?.EskaeraZenb ?? 0) + 1;
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            KomandakKudeatzailea.GuardarEskaera(sessionFactory, mesaId, resumen);

            MessageBox.Show("Datuak ongi gorde dira", "Ongi!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void BtnBorrar_Click(object sender, EventArgs e)
        {
            KomandakKudeatzailea.BorrarPedidos(sessionFactory, mesaId);

            MessageBox.Show("Eskaria ezabatu da", "Ongi!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void BtnResumen_Click(object sender, EventArgs e)
        {
            try
            {
                string resumenTexto = KomandakKudeatzailea.CargarResumen(sessionFactory, mesaId);

                if (string.IsNullOrWhiteSpace(resumenTexto))
                {
                    MessageBox.Show("Ez dago informaziorik erakusteko.", "Laburpena", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(resumenTexto, "Laburpena", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errorea laburpena kargatzean: {ex.Message}", "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void bebidasTab_Click(object sender, EventArgs e)
        {

        }
    }
}
