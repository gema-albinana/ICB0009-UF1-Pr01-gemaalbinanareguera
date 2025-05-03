using System;
using System.Text;
using System.Security.Cryptography;
using ClaveSimetricaClass;
using ClaveAsimetricaClass;

namespace SimuladorEnvioRecepcion
{
    class Program
    {   
        static string UserName;  
        static string SecurePass;  
        static string Salt;  
        static ClaveAsimetrica Emisor = new ClaveAsimetrica();
        static ClaveAsimetrica Receptor = new ClaveAsimetrica();
        static ClaveSimetrica ClaveSimetricaEmisor = new ClaveSimetrica();
        static ClaveSimetrica ClaveSimetricaReceptor = new ClaveSimetrica();

        static byte[] Firma = Array.Empty<byte>();
        static byte[] ClaveSimetricaKeyCifrada = Array.Empty<byte>();
        static byte[] ClaveSimetricaIVCifrada = Array.Empty<byte>();
        static byte[] TextoCifrado = Array.Empty<byte>();

        static string TextoAEnviar = "Me he dado cuenta que incluso las personas que dicen que todo está predestinado y que no podemos hacer nada para cambiar nuestro destino igual miran antes de cruzar la calle. Stephen Hawking.";
        
        static void Main(string[] args)
        {
            /****PARTE 1****/
            Console.WriteLine("¿Deseas registrarte? (S/N)");
            string registro = Console.ReadLine();

            if (registro.ToUpper() == "S")
            {
                Registro();                
            }
            else
            {
                Console.WriteLine("👋 **Gracias por usar el sistema. Hasta luego.**");
                return; 
            }

            bool login = Login();

            if (!login)
            {
                return;
            }

            /***PARTE 2: Simulación de envío y recepción segura***/

            byte[] TextoAEnviar_Bytes = Encoding.UTF8.GetBytes(TextoAEnviar); 
            Console.WriteLine("Texto a enviar bytes: {0}", BytesToStringHex(TextoAEnviar_Bytes));    

            // LADO EMISOR
            Console.WriteLine("\n🚀 **Inicio de envío de mensaje seguro**");

            // 1️⃣ Firmar mensaje
            Firma = Emisor.FirmarMensaje(TextoAEnviar_Bytes);

            // 2️⃣ Cifrar mensaje con clave simétrica
            TextoCifrado = ClaveSimetricaEmisor.CifrarMensaje(TextoAEnviar_Bytes);

            // 3️⃣ Cifrar clave simétrica con criptografía asimétrica
            ClaveSimetricaKeyCifrada = Emisor.CifrarMensaje(ClaveSimetricaEmisor.Key);
            ClaveSimetricaIVCifrada = Emisor.CifrarMensaje(ClaveSimetricaEmisor.IV);

            // Datos enviados al receptor
            Console.WriteLine("Firma: {0}", BytesToStringHex(Firma));
            Console.WriteLine("Texto cifrado: {0}", BytesToStringHex(TextoCifrado));
            Console.WriteLine("Clave simétrica cifrada (Key): {0}", BytesToStringHex(ClaveSimetricaKeyCifrada));
            Console.WriteLine("Clave simétrica cifrada (IV): {0}", BytesToStringHex(ClaveSimetricaIVCifrada));

            // LADO RECEPTOR
            Console.WriteLine("\n📥 **Recepción del mensaje cifrado y proceso de descifrado**");

            // 4️⃣ Descifrar clave simétrica
            ClaveSimetricaReceptor.Key = Receptor.DescifrarMensaje(ClaveSimetricaKeyCifrada);
            ClaveSimetricaReceptor.IV = Receptor.DescifrarMensaje(ClaveSimetricaIVCifrada);

            // 5️⃣ Descifrar mensaje
            byte[] MensajeDescifrado = ClaveSimetricaReceptor.DescifrarMensaje(TextoCifrado);
            string MensajeFinal = Encoding.UTF8.GetString(MensajeDescifrado);

            // 6️⃣ Comprobar firma antes de mostrar el mensaje
            if (Emisor.ComprobarFirma(Firma, MensajeDescifrado))
            {
                Console.WriteLine("\n✅ **Firma verificada con éxito. Mostrando mensaje descifrado:**");
                Console.WriteLine(MensajeFinal);
            }
            else
            {
                Console.WriteLine("\n❌ **Error: La firma no es válida. Mensaje posiblemente alterado.**");
            }
        }

        public static void Registro()
        {
            Console.WriteLine("📌 **Etapa 1: Completar el registro**");

            Console.Write("🧑 Indica tu nombre de usuario: ");
            UserName = Console.ReadLine();

            Console.Write("🔑 Indica tu password: ");
            string passwordRegister = Console.ReadLine();

            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            Salt = Convert.ToBase64String(saltBytes);

            using (SHA512 sha512 = SHA512.Create())
            {
                string saltedPassword = passwordRegister + Salt;
                byte[] hashBytes = Encoding.UTF8.GetBytes(saltedPassword);
                hashBytes = sha512.ComputeHash(hashBytes);
                SecurePass = Convert.ToBase64String(hashBytes);
            }

            Console.WriteLine("✅ **Registro completado con seguridad**");
        }

        public static bool Login()
        {
            int intentosFallidos = 0;
            int maxIntentos = 3;
            bool auxlogin = false;

            do
            {
                Console.WriteLine("\n🔐 **Etapa 2: Realizar login**");

                Console.Write("🧑 Usuario: ");
                string inputUser = Console.ReadLine();

                Console.Write("🔑 Password: ");
                string inputPass = Console.ReadLine();

                if (inputUser != UserName)
                {
                    intentosFallidos++;
                    Console.WriteLine($"❌ **Usuario no encontrado**. Intentos restantes: {maxIntentos - intentosFallidos}");
                    continue;
                }

                using (SHA512 sha512 = SHA512.Create())
                {
                    string saltedPassword = inputPass + Salt;
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

            if (intentosFallidos >= maxIntentos)
            {
                Console.WriteLine("🔒 **Acceso bloqueado. Intenta más tarde.**");
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
