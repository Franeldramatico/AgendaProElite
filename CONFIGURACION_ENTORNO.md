# 🛠️ Guía de Configuración del Entorno de Desarrollo

## 📋 Requisitos del Sistema

### Para Windows:
- Windows 10 versión 1809 o superior (64-bit)
- Mínimo 8 GB de RAM (16 GB recomendado)
- 50 GB de espacio libre en disco

### Para macOS:
- macOS 12.0 (Monterey) o superior
- Mínimo 8 GB de RAM (16 GB recomendado)
- 50 GB de espacio libre en disco

---

## 🚀 Paso 1: Instalar .NET 8.0 SDK

### Windows:

1. **Descarga el instalador:**
   - Ve a: https://dotnet.microsoft.com/download/dotnet/8.0
   - Descarga **".NET 8.0 SDK"** (no Runtime)
   - Elige la versión para **Windows x64**

2. **Ejecuta el instalador:**
   - Ejecuta el archivo `.exe` descargado
   - Sigue el asistente de instalación
   - Acepta los términos y condiciones

3. **Verifica la instalación:**
   - Abre **PowerShell** o **CMD**
   - Ejecuta:
     ```bash
     dotnet --version
     ```
   - Deberías ver: `8.0.x` o superior

### macOS:

1. **Descarga el instalador:**
   - Ve a: https://dotnet.microsoft.com/download/dotnet/8.0
   - Descarga **".NET 8.0 SDK"** para macOS
   - Elige la versión según tu Mac (Intel o Apple Silicon)

2. **Instala:**
   - Abre el archivo `.pkg` descargado
   - Sigue el asistente de instalación

3. **Verifica:**
   - Abre **Terminal**
   - Ejecuta:
     ```bash
     dotnet --version
     ```

---

## 🎨 Paso 2: Instalar Visual Studio 2022

### Windows:

1. **Descarga Visual Studio 2022 Community** (gratis):
   - Ve a: https://visualstudio.microsoft.com/es/downloads/
   - Descarga **"Visual Studio 2022 Community"**
   - Ejecuta el instalador

2. **Durante la instalación, selecciona estas cargas de trabajo:**
   - ✅ **Desarrollo de escritorio con .NET**
   - ✅ **Desarrollo para dispositivos móviles con .NET (MAUI)**
   - ✅ **Desarrollo de Android con .NET**

3. **Componentes individuales adicionales:**
   - ✅ **Android SDK** (se instala automáticamente con MAUI)
   - ✅ **Android SDK Platform-Tools**
   - ✅ **Android Emulator**

4. **Completa la instalación:**
   - Haz clic en "Instalar"
   - Espera a que termine (puede tardar 30-60 minutos)
   - Reinicia tu computadora si se solicita

### macOS:

1. **Descarga Visual Studio 2022 para Mac:**
   - Ve a: https://visualstudio.microsoft.com/es/vs/mac/
   - Descarga **"Visual Studio 2022 para Mac"**
   - Abre el archivo `.dmg` y arrastra Visual Studio a Aplicaciones

2. **Durante la instalación, selecciona:**
   - ✅ **.NET Multi-platform App UI (MAUI)**
   - ✅ **Desarrollo de Android**
   - ✅ **Desarrollo de iOS** (requiere Xcode)

3. **Instala Xcode (para desarrollo iOS):**
   - Abre **App Store**
   - Busca "Xcode"
   - Instala Xcode (es grande, ~15 GB)
   - Abre Xcode una vez para aceptar los términos
   - Ejecuta: `sudo xcode-select --switch /Applications/Xcode.app`

---

## 📱 Paso 3: Configurar Android SDK (Windows)

Si Visual Studio no instaló todo automáticamente:

1. **Abre Visual Studio 2022**
2. **Ve a:** Herramientas → Administrador de SDK de Android
3. **Instala:**
   - Android SDK Platform 33 (Android 13)
   - Android SDK Platform 34 (Android 14)
   - Android SDK Build-Tools
   - Android Emulator
   - Intel x86 Emulator Accelerator (HAXM)

---

## ✅ Paso 4: Verificar la Instalación

### Verificar .NET SDK:

```bash
dotnet --version
# Debería mostrar: 8.0.x
```

### Verificar MAUI workloads:

```bash
dotnet workload list
# Debería mostrar: maui-android, maui-ios (si estás en Mac)
```

Si no aparecen, instálalos:

```bash
dotnet workload restore
```

### Verificar Visual Studio:

1. Abre **Visual Studio 2022**
2. Ve a: Ayuda → Acerca de Microsoft Visual Studio
3. Verifica que aparezca: **".NET Multi-platform App UI development tools"**

---

## 🎯 Paso 5: Clonar y Abrir el Proyecto

### Opción A: Desde Visual Studio

1. **Abre Visual Studio 2022**
2. **Selecciona:** "Clonar un repositorio"
3. **Pega la URL:**
   ```
   https://github.com/Franeldramatico/AgendaProElite.git
   ```
4. **Elige una carpeta** donde guardar el proyecto
5. **Haz clic en "Clonar"**
6. **Espera** a que Visual Studio restaure los paquetes NuGet

### Opción B: Desde la Terminal/CMD

```bash
# Navega a donde quieres el proyecto
cd C:\Users\TuUsuario\Documents

# Clona el repositorio
git clone https://github.com/Franeldramatico/AgendaProElite.git

# Entra al proyecto
cd AgendaProElite

# Restaura los paquetes NuGet
dotnet restore

# Abre en Visual Studio
start AgendaProElite.csproj
```

---

## 🔧 Paso 6: Configurar el Emulador Android

### En Visual Studio:

1. **Ve a:** Herramientas → Administrador de dispositivos Android
2. **Crea un nuevo dispositivo virtual:**
   - Haz clic en el botón **"+"** (Nuevo dispositivo)
   - Elige un dispositivo (ej: Pixel 5)
   - Selecciona una imagen del sistema (ej: Android 13)
   - Haz clic en "Crear"

### O desde la línea de comandos:

```bash
# Lista los emuladores disponibles
emulator -list-avds

# Inicia un emulador
emulator -avd NombreDelEmulador
```

---

## ▶️ Paso 7: Compilar y Ejecutar el Proyecto

### Desde Visual Studio:

1. **Abre el proyecto** `AgendaProElite.csproj`
2. **Selecciona el dispositivo:**
   - En la barra superior, elige un emulador Android o dispositivo físico
3. **Presiona F5** o haz clic en el botón **"Ejecutar"** (▶️)
4. **Espera** a que compile (primera vez puede tardar 5-10 minutos)
5. **¡La app se abrirá en el emulador!**

### Desde la Terminal:

```bash
# Navega al proyecto
cd AgendaProElite

# Compila para Android
dotnet build -f net8.0-android

# Ejecuta en un emulador/dispositivo conectado
dotnet build -t:Run -f net8.0-android
```

---

## 🐛 Solución de Problemas Comunes

### Error: "Workloads no instalados"

```bash
dotnet workload restore
dotnet workload install maui
```

### Error: "Android SDK no encontrado"

1. Abre Visual Studio
2. Herramientas → Opciones → Xamarin → Configuración de Android
3. Verifica la ruta del SDK
4. O instala desde: Herramientas → Administrador de SDK de Android

### Error: "Emulador no inicia"

1. Verifica que **Hyper-V** esté habilitado (Windows)
2. O instala **Intel HAXM** desde el Administrador de SDK de Android
3. Reinicia tu computadora

### Error: "Paquetes NuGet no se restauran"

```bash
# Limpia la caché
dotnet nuget locals all --clear

# Restaura de nuevo
dotnet restore
```

### El proyecto no compila

1. **Verifica que tienes .NET 8.0:**
   ```bash
   dotnet --version
   ```

2. **Instala los workloads de MAUI:**
   ```bash
   dotnet workload install maui-android
   dotnet workload install maui-ios  # Solo en Mac
   ```

3. **Restaura los paquetes:**
   ```bash
   dotnet restore
   ```

---

## 📚 Recursos Adicionales

- **Documentación .NET MAUI**: https://learn.microsoft.com/dotnet/maui/
- **Documentación Visual Studio**: https://learn.microsoft.com/visualstudio/
- **Comunidad .NET MAUI**: https://github.com/dotnet/maui

---

## ✅ Checklist de Verificación

Antes de compilar, verifica que tienes:

- [ ] .NET 8.0 SDK instalado (`dotnet --version` muestra 8.0.x)
- [ ] Visual Studio 2022 con carga de trabajo MAUI
- [ ] Android SDK instalado (Windows) o Xcode instalado (Mac)
- [ ] Emulador Android configurado
- [ ] Proyecto clonado desde GitHub
- [ ] Paquetes NuGet restaurados (`dotnet restore`)

---

## 🎉 ¡Listo!

Una vez completados estos pasos, podrás:
- ✅ Compilar el proyecto AgendaPro Elite
- ✅ Ejecutarlo en emuladores Android
- ✅ Ejecutarlo en dispositivos físicos
- ✅ Desarrollar nuevas funcionalidades
- ✅ Depurar y probar la aplicación

**¡Feliz desarrollo!** 🚀

