## 🔐 **Parte1. Registro y Login** ##  

## ⚙️ **Etapas del Programa**
### ✅ **Etapa 1: Completar el registro**
1. El programa solicita al usuario un **nombre de usuario** y lo almacena en la variable `UserName`.
2. Luego, solicita una **contraseña** y la cifra utilizando **SHA-512** y un `salt` aleatorio.
3. Se almacenan variables adicionales (`SecurePass`, `Salt`) que serán necesarias para el login.

### ✅ **Etapa 2: Realizar login**
1. Se solicita al usuario su **nombre de usuario** y **contraseña**.
2. Se compara el nombre de usuario con el registrado en `UserName`.
3. Se cifra nuevamente la contraseña ingresada y se compara con el hash original almacenado.
4. **Si la contraseña es correcta**, se genera un `true`, permitiendo el acceso.
5. **Si la contraseña es incorrecta**, se genera `false`, y después de 3 intentos fallidos, el acceso queda bloqueado temporalmente.

## 🔒 **Tecnologías y Seguridad Implementada**
✅ **SHA-512**: Algoritmo seguro para cifrar la contraseña.  
✅ **Salt aleatorio**: Protege el hash de la contraseña contra ataques de fuerza bruta.  
✅ **Límite de intentos en el login**: Evita múltiples intentos fallidos y bloquea el acceso tras 3 fallos.  

## 🎯 Salida esperada del sistema de registro: 
![alt text](image.png)
![alt text](image-1.png)
![alt text](image-2.png)

**🔐Explica el mecanismo de Registro / Login utilizado (máximo 5 líneas)**    
El sistema de Registro y Login usa SHA-512 para cifrar la contraseña con un salt aleatorio, protegiéndola contra ataques de fuerza bruta. Al registrarse, el usuario ingresa su nombre y contraseña, que se almacenan cifradas. En el login, la contraseña ingresada se cifra nuevamente con el mismo salt y se compara con el hash guardado. Si coincide, el acceso se concede; si falla tres veces, el sistema bloquea el intento por seguridad. 