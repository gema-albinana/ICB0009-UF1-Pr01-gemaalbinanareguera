using System;
using System.Text;
using System.Security.Cryptography;
using ClaveSimetricaClass;
using ClaveAsimetricaClass;

namespace SimuladorEnvioRecepcion
{
    class Program
    {   
        static string UserName;  // Nombre de usuario
        static string SecurePass;  // Contraseña cifrada con SHA-512 y salt
        static string Salt;  // Salt aleatorio generado en el registro
        static ClaveAsimetrica Emisor = new ClaveAsimetrica();
        static ClaveAsimetrica Receptor = new ClaveAsimetrica();
        static ClaveSimetrica ClaveSimetricaEmisor = new ClaveSimetrica();
        static ClaveSimetrica ClaveSimetricaReceptor = new ClaveSimetrica();

        static string TextoAEnviar = "Me he dado cuenta que incluso las personas que dicen que todo está predestinado y que no podemos hacer nada para cambiar nuestro destino igual miran antes de cruzar la calle. Stephen Hawking.";
        
        static void Main(string[] args)
        {
            /****PARTE 1****/
            //Login / Registro
            Console.WriteLine("¿Deseas registrarte? (S/N)");
            string registro = Console.ReadLine();

            if (registro.ToUpper() == "S")
            {
                //Realizar registro del cliente con seguridad
                Registro();                
            }
            else
            {
                Console.WriteLine("👋 **Gracias por usar el sistema. Hasta luego.**");
                return; // Finalizar el programa sin intentar login
            }

            //Realizar login solo si el usuario se registró
            bool login = Login();

            /***FIN PARTE 1***/

            if (login)
            {                  
                byte[] TextoAEnviar_Bytes = Encoding.UTF8.GetBytes(TextoAEnviar); 
                Console.WriteLine("Texto a enviar bytes: {0}", BytesToStringHex(TextoAEnviar_Bytes));    

                //LADO EMISOR
                //Firmar mensaje
                //Cifrar mensaje con la clave simétrica
                //Cifrar clave simétrica con la clave pública del receptor

                //LADO RECEPTOR
                //Descifrar clave simétrica
                //Descifrar clave simétrica
                //Descifrar mensaje con la clave simétrica
                //Comprobar firma
            }
        }

        public static void Registro()
        {
            Console.WriteLine("📌 **Etapa 1: Completar el registro**");

            // Pedimos el nombre de usuario y lo almacenamos
            Console.Write("🧑 Indica tu nombre de usuario: ");
            UserName = Console.ReadLine();

            // Pedimos la contraseña y la guardamos de forma segura
            Console.Write("🔑 Indica tu password: ");
            string passwordRegister = Console.ReadLine();

            /***PARTE 1***/
            /*Añadir el código para poder almacenar el password de manera segura*/

            // Generamos el salt aleatorio
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes); // Generamos bytes aleatorios
            }
            Salt = Convert.ToBase64String(saltBytes); // Convertimos el salt a texto legible

            // Aplicamos SHA-512 con salt
            using (SHA512 sha512 = SHA512.Create())
            {
                string saltedPassword = passwordRegister + Salt; // Concatenamos password + salt
                byte[] hashBytes = Encoding.UTF8.GetBytes(saltedPassword);
                hashBytes = sha512.ComputeHash(hashBytes);
                SecurePass = Convert.ToBase64String(hashBytes); // Convertimos el resultado a Base64
            }

            Console.WriteLine("✅ **Registro completado con seguridad**");
            Console.WriteLine($"👤 **Usuario registrado:** {UserName}");
            Console.WriteLine($"🔒 **Contraseña cifrada:** {SecurePass}");
            Console.WriteLine($"🧂 **Salt utilizado:** {Salt}");
        }

        public static bool Login()
        {
            bool auxlogin = false;
            int intentosFallidos = 0;  // Contador de intentos fallidos
            int maxIntentos = 3;  // Máximo de intentos permitidos

            do
            {
                Console.WriteLine("\n🔐 **Etapa 2: Realizar login**");

                Console.Write("🧑 Usuario: ");
                string inputUser = Console.ReadLine();

                Console.Write("🔑 Password: ");
                string inputPass = Console.ReadLine();

                /***PARTE 1***/
                /*Modificar esta parte para que el login se haga teniendo en cuenta que el registro se realizó con SHA512 y salt*/

                // Comparación del nombre de usuario
                if (inputUser != UserName)
                {
                    intentosFallidos++;
                    Console.WriteLine($"❌ **Usuario no encontrado**. Intentos restantes: {maxIntentos - intentosFallidos}");
                    continue;
                }

                // Verificación de la contraseña cifrada con SHA-512 y salt
                using (SHA512 sha512 = SHA512.Create())
                {
                    string saltedPassword = inputPass + Salt; // Aplicamos el mismo salt generado en el registro
                    byte[] hashBytes = Encoding.UTF8.GetBytes(saltedPassword);
                    hashBytes = sha512.ComputeHash(hashBytes);
                    string hashedInputPass = Convert.ToBase64String(hashBytes);

                    if (hashedInputPass == SecurePass)
                    {
                        Console.WriteLine("✅ **Login exitoso**.");
                        auxlogin = true;
                    }
                    else
                    {
                        intentosFallidos++;
                        Console.WriteLine($"❌ **Contraseña incorrecta**. Intentos restantes: {maxIntentos - intentosFallidos}");
                    }
                }

            } while (!auxlogin && intentosFallidos < maxIntentos);

            // Mensaje final si se alcanzó el límite de intentos
            if (intentosFallidos >= maxIntentos)
            {
                Console.WriteLine("🔒 **Has excedido el número máximo de intentos. Acceso bloqueado.**");
                Console.WriteLine("⏳ **Por seguridad, deberás intentarlo más tarde.**");
            }

            return auxlogin;
        }

        static string BytesToStringHex(byte[] result)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (byte b in result)
                stringBuilder.AppendFormat("{0:x2}", b);

            return stringBuilder.ToString();
        }        
    }
}
