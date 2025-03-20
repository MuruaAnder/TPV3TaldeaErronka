using NHibernate;

namespace _2taldea
{
    public static class PlateraKudeatzailea
    {
        public static string PlateraAdd(ISessionFactory sessionFactory, string izena, string kategoria, int kantitatea, float prezioa)
        {
            try
            {
                using (ISession session = sessionFactory.OpenSession())
                using (ITransaction transaction = session.BeginTransaction())
                {
                    // 1️⃣ Guardar el nuevo plato en la tabla Platera
                    Platera plato = new Platera
                    {
                        Izena = izena,
                        Kategoria = kategoria,
                        Kantitatea = kantitatea,
                        Prezioa = prezioa
                    };

                    session.Save(plato);

                    // 2️⃣ Guardar el mismo plato como un producto en la tabla Produktua
                    Produktua producto = new Produktua
                    {
                        Izena = izena,   // Mismo nombre del plato
                        Stock = kantitatea,  // Stock inicial igual a la cantidad del plato
                        Prezioa = prezioa,  // Mismo precio que el plato
                        Max = 100000,   // Valor arbitrario de stock máximo
                        Min = 1      // Valor arbitrario de stock mínimo
                    };

                    session.Save(producto);

                    // Confirmamos la transacción
                    transaction.Commit();

                    return "true"; // Éxito
                }
            }
            catch (Exception ex)
            {
                return "Error al guardar el plato y producto: " + ex.Message;
            }
        }
    }
}
