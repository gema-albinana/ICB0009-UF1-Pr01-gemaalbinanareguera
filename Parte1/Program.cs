using System;
using System.Security.Cryptography;
using System.Text;

class RegistroLogin
{
    // Variables globales para almacenar el usuario, la contraseña cifrada y el "salt"
    static string UserName; // Nombre de usuario
    static string SecurePass; // Contraseña cifrada
    static string Salt; // "Salting" utilizado
    static int Iteraciones = 10000; // Número de iteraciones para reforzar el hashing

    // Método para generar un "salt" aleatorio, que se agregará a la contraseña antes de cifrarla
    static string GenerateSalt()
    {
        byte[] saltBytes = new byte[16]; // Array de 16 bytes para el salt
        using (var rng = RandomNumberGenerator.Create()) // Usamos un generador aleatorio seguro
        {
            rng.GetBytes(saltBytes); // Generamos bytes aleatorios para el salt
        }
        return Convert.ToBase64String(saltBytes); // Convertimos el salt a formato Base64
    }

    // Método para aplicar SHA-256 con salt y múltiples iteraciones (reforzando seguridad)
    static string HashPassword(string password, string salt, int iterations)
    {
        using (SHA256 sha256 = SHA256.Create()) // Creamos el objeto SHA-256 para el hashing
        {
            string saltedPassword = password + salt; // Concatenamos la contraseña con el salt
            byte[] hashBytes = Encoding.UTF8.GetBytes(saltedPassword); // Convertimos a bytes

            // Iteramos el hashing múltiples veces para hacerlo más seguro
            for (int i = 0; i < iterations; i++)
            {
                hashBytes = sha256.ComputeHash(hashBytes); // Aplicamos SHA-256 repetidamente
            }

            // Convertimos el resultado en una cadena hexadecimal legible
            StringBuilder builder = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                builder.Append(b.ToString("x2")); // Convertimos cada byte a hexadecimal
            }

            return builder.ToString(); // Retornamos el hash final
        }
    }

    // Método para realizar el registro del usuario
    static void Registro()
    {
        Console.Write("🧑 Introduce tu nombre de usuario: ");
        UserName = Console.ReadLine(); // Guardamos el nombre de usuario

        Console.Write("🔑 Introduce tu contraseña: ");
        string password = Console.ReadLine(); // Capturamos la contraseña en texto plano

        Salt = GenerateSalt(); // Generamos un salt aleatorio único
        SecurePass = HashPassword(password, Salt, Iteraciones); // Aplicamos hashing con salt iterado

        // Mostramos la información cifrada (esto normalmente no se mostraría en un sistema real)
        Console.WriteLine("✅ Registro completado con seguridad.");
        Console.WriteLine($"👤 Usuario registrado: {UserName}");
        Console.WriteLine($"🔒 Contraseña cifrada: {SecurePass}");
        Console.WriteLine($"🧂 Salt utilizado: {Salt}");
    }

    // Método principal, ejecutamos el proceso de registro
    static void Main()
    {
        Registro(); // Llamamos al método de registro
    }
}
