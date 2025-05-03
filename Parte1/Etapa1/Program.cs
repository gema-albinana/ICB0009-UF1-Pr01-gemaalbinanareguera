using System;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

class RegistroLogin
{
    // Variables globales para almacenar el usuario, la contraseña cifrada y el "salt"
    static string UserName; // Nombre de usuario
    static string SecurePass; // Contraseña cifrada
    static string Salt; // "Salting" utilizado
    static int Iteraciones = 10000; // Número de iteraciones para reforzar el hashing
    static int intentosFallidos = 0; // Contador de intentos fallidos en el login
    static int maxIntentos = 3; // Máximo de intentos permitidos antes de bloquear

    // Método para generar un salt aleatorio
    static string GenerateSalt()
    {
        byte[] saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create()) // Generador seguro de salt
        {
            rng.GetBytes(saltBytes); // Generamos bytes aleatorios
        }
        return Convert.ToBase64String(saltBytes); // Convertimos a formato Base64
    }

    // Método para aplicar SHA-256 con salt y múltiples iteraciones (reforzando seguridad)
    static string HashPassword(string password, string salt, int iterations)
    {
        using (SHA256 sha256 = SHA256.Create()) // Usamos SHA-256 para hashing
        {
            string saltedPassword = password + salt; // Concatenamos password + salt
            byte[] hashBytes = Encoding.UTF8.GetBytes(saltedPassword); // Convertimos a bytes

            // Iteramos el hashing múltiples veces para hacerlo más seguro
            for (int i = 0; i < iterations; i++)
            {
                hashBytes = sha256.ComputeHash(hashBytes);
            }

            // Convertimos el resultado en una cadena hexadecimal
            StringBuilder builder = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                builder.Append(b.ToString("x2")); // Formato hexadecimal
            }

            return builder.ToString();
        }
    }

    // Método para registrar al usuario con seguridad
    static void Registro()
    {
        Console.Write("🧑 Introduce tu nombre de usuario: ");
        UserName = Console.ReadLine();

        Console.Write("🔑 Introduce tu contraseña: ");
        string password = Console.ReadLine();

        Salt = GenerateSalt(); // Generamos un salt aleatorio
        SecurePass = HashPassword(password, Salt, Iteraciones); // Ciframos la contraseña

        Console.WriteLine("✅ Registro completado con seguridad.");
        Console.WriteLine($"👤 Usuario registrado: {UserName}");
        Console.WriteLine($"🔒 Contraseña cifrada: {SecurePass}");
        Console.WriteLine($"🧂 Salt utilizado: {Salt}");
    }

    // Método para realizar el login con protección de intentos fallidos
    static bool Login()
    {
        if (intentosFallidos >= maxIntentos)
        {
            Console.WriteLine("🚫 Has excedido el número máximo de intentos. Intenta más tarde.");
            return false;
        }

        Console.Write("🧑 Introduce tu nombre de usuario: ");
        string inputUser = Console.ReadLine();

        Console.Write("🔑 Introduce tu contraseña: ");
        string inputPass = Console.ReadLine();

        // Comprobamos si el usuario existe
        if (inputUser == UserName) 
        {
            // Aplicamos el mismo hashing que en el registro
            string hashedInputPass = HashPassword(inputPass, Salt, Iteraciones);

            // Comparamos el hash con el almacenado
            if (hashedInputPass == SecurePass)
            {
                Console.WriteLine("✅ Login exitoso.");
                intentosFallidos = 0; // Reiniciamos el contador de intentos fallidos
                return true;
            }
            else
            {
                intentosFallidos++;
                Console.WriteLine($"❌ Contraseña incorrecta. Intentos restantes: {maxIntentos - intentosFallidos}");

                // Mensaje final cuando el usuario agota los intentos
                if (intentosFallidos >= maxIntentos)
                {
                    Console.WriteLine("🔒 Has alcanzado el número máximo de intentos fallidos. Bloqueando acceso.");
                }

                return false;
            }
        }
        else
        {
            intentosFallidos++;
            Console.WriteLine($"❌ Usuario no encontrado. Intentos restantes: {maxIntentos - intentosFallidos}");

            // Mensaje final cuando el usuario agota los intentos
            if (intentosFallidos >= maxIntentos)
            {
                Console.WriteLine("🔒 Has alcanzado el número máximo de intentos fallidos. Bloqueando acceso.");
            }

            return false;
        }
    }

    // Método principal: primero registra al usuario y luego permite probar el login
    static void Main()
    {
        Registro(); // Ejecutamos el proceso de registro
        Console.WriteLine("\n🔐 Ahora intenta iniciar sesión:\n");

        while (!Login()) // Ejecutamos el login hasta que sea exitoso o se bloqueen los intentos
        {
            if (intentosFallidos >= maxIntentos)
                break;
        }
    }
}
