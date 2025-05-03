using System;
using System.Security.Cryptography;
using System.Text;

class RegistroLogin
{
    static string UserName;
    static string SecurePass;
    static string Salt;

    // Método para generar un "salting" aleatorio
    static string GenerateSalt()
    {
        byte[] saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    // Método para cifrar la contraseña con SHA-256 y salting
    static string HashPassword(string password, string salt)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            string saltedPassword = password + salt;
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    // Método para registrar al usuario
    static void Registro()
    {
        Console.Write("Introduce tu nombre de usuario: ");
        UserName = Console.ReadLine();

        Console.Write("Introduce tu contraseña: ");
        string password = Console.ReadLine();

        Salt = GenerateSalt(); // Generamos un salt aleatorio
        SecurePass = HashPassword(password, Salt); // Ciframos la contraseña

        Console.WriteLine("Registro completado con seguridad.");
        Console.WriteLine($"Usuario registrado: {UserName}");
        Console.WriteLine($"Contraseña cifrada: {SecurePass}");
        Console.WriteLine($"Salt utilizado: {Salt}");
    }

    // Método principal para ejecutar el registro
    static void Main()
    {
        Registro(); // Solo probamos el registro por ahora
    }
}
