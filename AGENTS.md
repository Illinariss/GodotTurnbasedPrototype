# Agent Instructions

Diese Hinweise erklären, wie das Projekt lokal ausgeführt wird und welche Abhängigkeiten benötigt werden.

## .NET SDK 8 installieren
Auf Ubuntu lässt sich das .NET SDK 8 direkt über die Paketquellen installieren:

```bash
sudo apt-get update -y
sudo apt-get install -y dotnet-sdk-8.0
```

Danach sollte `dotnet --version` die installierte Version (z. B. `8.0.117`) ausgeben.

## Godot 4.4.1 (Mono) Headless herunterladen
Die Headless-Version wird für automatisierte Tests benötigt. Der Download kann von GitHub erfolgen:

```bash
wget https://github.com/godotengine/godot-builds/releases/download/4.4.1-stable/Godot_v4.4.1-stable_mono_linux_x86_64.zip
unzip Godot_v4.4.1-stable_mono_linux_x86_64.zip
mv Godot_v4.4.1-stable_mono_linux_x86_64/Godot_v4.4.1-stable_mono_linux.x86_64 ./
cp -r Godot_v4.4.1-stable_mono_linux_x86_64/GodotSharp .
chmod +x Godot_v4.4.1-stable_mono_linux.x86_64
```

## Projekt bauen und testen

```bash
dotnet build 'Rundenbasiertes kampfsystem prototyp.sln'
dotnet test --verbosity minimal
```

## Headless starten

Das Projekt lässt sich headless über den folgenden Befehl ausführen (hier wird die Hauptszene direkt angegeben):

```bash
./Godot_v4.4.1-stable_mono_linux.x86_64 --headless --path . --main-scene game.tscn --quit -v
```
