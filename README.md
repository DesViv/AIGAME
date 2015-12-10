# DIODE v1.0

# Introduction: 
DIODE is a turn based strategy game created to facilitate the testing of AI algorithms. 

# Installation :
DIODE can be installed and built through source or downloaded as a prebuilt binary in the GitHub release page. At the moment the only build is windows 32 bit, but other platforms can be compiled by the user if needed. 

https://github.com/DesViv/AIGAME/releases

To run a prebuilt binary simply download DIODE.zip from the GitHub release page and unzip to a directory. The only supported prebuilt binaries are Windows 32 bit. 

To play DIODE run the DIODE.exe file and the game should run. To install additional AI scripts place the AI script files in the directory called AI in the same directory as the binary. To develop AI algorithms see the section AI Development. 

To download as source simply clone the git repo and open the project in Unity. The last guaranteed supported version of Unity is 5.2.3f1, although likely later versions should work as well. To run simply run in the Unity player or build your own binary. If running in the Unity editor nothing needs to be done it should work as is. There are cases where the build settings aren’t saved on Git so the player needs to be set to use .NET 2.0. This will be noted by an error in the Unity editor:
“Assets/Completed/Scripts/CSharp.cs(53,41): error CS0246: The type or namespace name `ICodeCompiler' could not be found. Are you missing a using directive or an assembly reference?”

To Fix this follow these instructions:
Open the menu File>Build Settings and click on player settings
Under the PC target tab go to Other Settings>Optimization>API Level and set it to .NET 2.0 and not .NET 2.0 subset

# Building DIODE:
To build DIODE make sure the API compatibility is set to .NET 2.0. Then follow these instructions:
Go the menu File>Build Settings
Select your platform, the only officially supported platforms are 32 and 64 bit Windows. 
Select the build directory, and give the exe a name. All files will be ready for distribution. 
Create the AI folder and place the AI scripts inside of it.

To run a binary of DIODE a install of Mono is required in the same directory as the binary. This is needed to do the dynamic compiling of the C# AI scripts. The player vs player mode will still work if the mono install is not there. Install mono from:
http://www.mono-project.com/download/
Copy the install directory for mono into the directory with the exe. 
To develop DIODE see the develop section. 

# AI Development: 
All AI scripts extend the AIBase class that can be found in the AIAPI.cs file in the source code. This code provides an interface to all the units in the AI player team, and units on the other team. It also gives an interface to the game mechanics to move the units in the game state. This is implemented through the ActionList variable. Adding AIActions to the action list 
