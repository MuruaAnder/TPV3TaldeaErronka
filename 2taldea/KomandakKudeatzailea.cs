using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2taldea
{
    internal class KomandakKudeatzailea
    {
        private readonly ISessionFactory sessionFactory;

        public KomandakKudeatzailea(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        // Método para obtener todas las mesas
        public static List<Mahaia> ObtenerMesas(ISessionFactory sessionFactory)
        {
            try
            {
                using (var session = sessionFactory.OpenSession())
                {
                    return session.CreateQuery("FROM Mahaia").List<Mahaia>().ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las mesas: {ex.Message}");
                return new List<Mahaia>();
            }
        }

        // Método para cargar platos por categoría
        public List<Platera> CargarPlatos(string categoria)
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                return session.QueryOver<Platera>()
                              .Where(p => p.Kategoria == categoria)
                              .List()
                              .ToList();
            }
        }

        // Método para verificar si una mesa tiene un pedido activo
        internal static bool TienePedidoActivo(int mesaId, ISessionFactory sessionFactory)
        {
            try
            {
                using (var session = sessionFactory.OpenSession())
                {
                    return session.QueryOver<Eskaera>()
                                  .Where(e => e.MesaId == mesaId && e.Activo)
                                  .RowCount() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar pedidos activos en la mesa {mesaId}: {ex.Message}");
                return false;
            }
        }


        // Método para guardar un pedido y activarlo
        public static void GuardarEskaera(ISessionFactory sessionFactory, int mesaId, Dictionary<string, (int cantidad, float precio)> resumen)
        {
            using (var session = sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var mesa = session.Get<Mahaia>(mesaId);
                    if (mesa == null)
                    {
                        throw new Exception($"No se encontró la mesa con ID {mesaId}.");
                    }

                    int nuevoEskaeraZenb = ObtenerNuevoEskaeraZenb(session);

                    foreach (var item in resumen)
                    {
                        var nombrePlato = item.Key;
                        var cantidad = item.Value.cantidad;
                        var precio = item.Value.precio;

                        var plato = session.QueryOver<Platera>()
                                           .Where(p => p.Izena == nombrePlato)
                                           .SingleOrDefault();

                        if (plato == null)
                        {
                            throw new Exception($"Plato '{nombrePlato}' no encontrado en la base de datos.");
                        }

                        var productos = session.QueryOver<Produktua>()
                                               .Where(p => p.Izena == plato.Izena)
                                               .List();

                        if (productos == null || productos.Count == 0)
                        {
                            throw new Exception($"No se encontró un producto asociado al plato '{nombrePlato}' en la tabla Produktua.");
                        }

                        int stockTotalDisponible = productos.Sum(p => p.Stock);

                        // 🚨 Si no hay suficiente stock, mostramos alerta y no seguimos con el pedido 🚨
                        if (stockTotalDisponible < cantidad)
                        {
                            Console.WriteLine($"⚠️ Ez dago stockik '{nombrePlato}'! Mesedez, aukeratu beste zerbait eta gerenteari abixatu! ⚠️");
                            return; // Salimos de la función sin procesar el pedido
                        }

                        int cantidadRestante = cantidad;

                        foreach (var producto in productos)
                        {
                            if (cantidadRestante == 0)
                                break;

                            int reducirEnEsteProducto = Math.Min(producto.Stock, cantidadRestante);
                            producto.Stock -= reducirEnEsteProducto;
                            cantidadRestante -= reducirEnEsteProducto;

                            session.Update(producto);

                            // 🚨 Si el producto llegó a 0, mostramos la alerta 🚨
                            if (producto.Stock == 0)
                            {
                                Console.WriteLine($"⚠️ Ez dago stockik '{producto.Izena}'! Mesedez, aukeratu beste zerbait eta gerenteari abixatu! ⚠️");
                            }
                        }

                        // Guardamos los pedidos
                        for (int i = 0; i < cantidad; i++)
                        {
                            Eskaera nuevaEskaera = new Eskaera
                            {
                                EskaeraZenb = nuevoEskaeraZenb,
                                Izena = nombrePlato,
                                Prezioa = precio,
                                MesaId = mesaId,
                                Activo = true
                            };

                            session.Save(nuevaEskaera);
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error al guardar el pedido y actualizar el stock: {ex.Message}");
                    throw;
                }
            }
        }



        // Método para desactivar los pedidos de una mesa
        public static void BorrarPedidos(ISessionFactory sessionFactory, int mesaId)
        {
            try
            {
                using (ISession session = sessionFactory.OpenSession())
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var pedidosParaActualizar = session.QueryOver<Eskaera>()
                                                        .Where(e => e.MesaId == mesaId && e.Activo)
                                                        .List();

                    foreach (var pedido in pedidosParaActualizar)
                    {
                        pedido.Activo = false;
                        session.Update(pedido);
                    }

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al desactivar los pedidos: {ex.Message}");
            }
        }

        // Método para obtener el siguiente número de pedido
        private static int ObtenerNuevoEskaeraZenb(ISession session)
        {
            var lastEskaera = session.QueryOver<Eskaera>()
                                     .OrderBy(e => e.EskaeraZenb).Desc
                                     .Take(1)
                                     .SingleOrDefault();

            return (lastEskaera?.EskaeraZenb ?? 0) + 1;
        }

        // Método para cargar el resumen de un pedido activo
        public static string CargarResumen(ISessionFactory sessionFactory, int mesaId)
        {
            try
            {
                using (ISession session = sessionFactory.OpenSession())
                {
                    var pedidosActivos = session.QueryOver<Eskaera>()
                                                .Where(e => e.MesaId == mesaId && e.Activo)
                                                .List();

                    if (pedidosActivos == null || pedidosActivos.Count == 0)
                    {
                        return "Ez dago komandarik mahai honetarako.";
                    }

                    Dictionary<string, (int cantidad, float precio)> resumen = new();

                    foreach (var pedido in pedidosActivos)
                    {
                        if (resumen.ContainsKey(pedido.Izena))
                        {
                            resumen[pedido.Izena] = (resumen[pedido.Izena].cantidad + 1, pedido.Prezioa);
                        }
                        else
                        {
                            resumen[pedido.Izena] = (1, pedido.Prezioa);
                        }
                    }

                    string resumenTexto = "Komandaren laburpena:\n\n";
                    float total = 0;

                    foreach (var item in resumen)
                    {
                        float subtotal = item.Value.cantidad * item.Value.precio;
                        resumenTexto += $"- {item.Key}: {item.Value.cantidad} x {item.Value.precio:C2} = {subtotal:C2}\n";
                        total += subtotal;
                    }

                    resumenTexto += $"\nTotala: {total:C2}";

                    return resumenTexto;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Errorea laburpena kargatzean: {ex.Message}", ex);
            }
        }
    }
}
