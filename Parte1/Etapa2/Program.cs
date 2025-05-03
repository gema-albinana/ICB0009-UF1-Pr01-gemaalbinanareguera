using System;
using BCrypt.Net;

class RegistroLogin
{
    static string UserName;  // Nombre de usuario
    static string SecurePass;  // Contraseña cifrada con bcrypt
    static int intentosFallidos = 0; // Contador de intentos fallidos en el login
    static int maxIntentos = 3; // Máximo de intentos permitidos antes de bloquear

    // Método para registrar al usuario con seguridad usando bcrypt
    static void Registro()
    {
        Console.Write("🧑 Introduce tu nombre de usuario: ");
        UserName = Console.ReadLine();

        Console.Write("🔑 Introduce tu contraseña: ");
        string password = Console.ReadLine();

        // Ciframos la contraseña con bcrypt (salt automático, 12 rondas)
        SecurePass = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));

        Console.WriteLine("✅ Registro completado con seguridad.");
        Console.WriteLine($"👤 Usuario registrado: {UserName}");
        Console.WriteLine($"🔒 Contraseña cifrada con bcrypt: {SecurePass}");
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
            // Verificamos la contraseña con bcrypt
            if (BCrypt.Net.BCrypt.Verify(inputPass, SecurePass))
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
