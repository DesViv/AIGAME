# Introduction:
DIODE is a turn based strategy game created to facilitate the testing of AI algorithms. DIODE is played between two players. DIODE uses basic game mechanics and an AI programming interface to allow developers to easily plug and play new AI algorithms and test their effectiveness.

# Game Rules:
The game rules are simple. Each player as a set of units, each unit can move a certain number of steps or choose to attack and adjacent unit of the opposing player. The game ends when one player runs out of units.
Combat is decided by the stats of units involved in the combat. Each unit has a certain amount of health points. When these points fall to zero than the unit is removed from the game. Each time combat occurs a the attacked unit loses health points equivalent to the value of the attack stat in the attacking unit.

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

# AI Development:
Currently AI scripting is implemented in C#. The AI scripts are stored in a subdirectory “AI” of the directory that stores the game executable.When the game is run and the AI selector screen is reached, the code in that directory is compiled and linked against the running game.  
This design decision was driven by the choice to unify all our code under one language and the ease of interfacing the scripts with the rest of the game.
Unfortunately there are some serious security flaws related to running giving full access to user scripts to our code. Also the game lags significantly when loading the AI selector screen as the code compiles.  Currently the scripting API is stable and feature complete, but it is not future proof. In future versions of DIODE the scripting interface may change to a scripting language or a more limited compilation for C# to prevent security and speed up loading. For DIODE v1.0 C# with dynamic compilation is standard.

# AI API:
All AI scripts extend the AIBase class that can be found in the AIAPI.cs file in the source code. This code provides an interface to all the units in the AI player team, and units on the other team. It also gives an interface to the game mechanics to move the units in the game state. At the start of the AI players turn the method onTurn is called to calculate what moves the AI player is to take in this turn. The AI player takes its state information stored in the lists of the AI Players units and the opponent's units to determine a list of moves to make. That list of moves is stored in actions, which is then used by the game to move units and attack.
 Below is the interface for the AIBase class that will be used in all AI Scripts:
 ```java
public abstract class AIBase
{
    //The list of the other teams units. Note the following fields are public due to how the
    //Unity environment interacts with scripts in the editor. It makes it easier to interface
    //with Unity prefabs.
    public List<MovingObject> other;
    //List of Units belonging to the AI.
    public List<Enemy> self;
    //The actions the AI agent is to do
    protected List<AIAction> actions;
    //This is the main interface for writing user scripts this is called at the start of an AI
    //player's turn. The purpose of this method is to fill the actions list with AIActions
    //that are determined by the AI algorithm.
    public virtual void onTurn();
    //Sets up the AI class with the appropriate unit lists. This is implemented in the
    //base class and does not need to be overwritten.
    public void init(List<Enemy> s,List<MovingObject> o);
}
```
AIAction is a data structure that stores data about how the unit in the game should act. This is currently limited to either moving or attacking due to the rules of the game. The AIAction simply tells the game the who,what, and where of moving the game unit (the game mechanics take care of the when). Below is the code defining the AIAction class in AIAPI.cs. Not this code never has to be touched, and normally isn’t even available. In the client code just call the constructor and add it to the AIAction list:
```java
public class AIAction
{
    //Who should do the action
    public Enemy obj;
    //What actions can be done
    public enum Actions
    {
        Move,Attack
    }
    //Choose which action to do
    public Actions action;
    //Where to do the action
    public Vector3 pos;

    public AIAction(Enemy o,Actions a,Vector3 p)
    {
        obj = o;
        action = a;
        pos = p;
    }
}
```

Note certain C# namespaces are needed so that the code compiles correctly, and not all are available if running from binary install. Notably the UnityEngine namespace, and Completed namespace are two DIODE libraries that are needed for the base class to link to the game backend. Unfortunately at the moment the compiler is not configured to accept outside libraries, such as premade AI libraries.  

# Example AI Script:
Below is an example of a C# AI script. Note that the only function that is needed to be implemented is the onTurn method that is called at the start of every turn. The important parts that interface with the game are hidden behind the AIBase class and the Game Manager in the DIODE backend.
The onTurn method can be implemented in any way that the developer wants as long as it assigns actions to the action list.
using UnityEngine;
using Completed;

```java
public class AISimple : AIBase {
    /*
    A very simple AI that moves to the top left, and attacks anything along the way.
    Not very useful for anything except examples
    */
    public override void onTurn()
    {
        //Call the superclass
        base.onTurn();
        //Calculate action for each Unit in the AI player
        foreach (Enemy e in self)
        {

            Vector3 cur = e.currentPos;
            bool fin = false;
            e.showValidAttack();
            //Look in the areas that can attack
            foreach (GameObject g in e.validAttack)
            {
                //If any of the opposing units are in these squares attack them
                foreach (MovingObject mo in other)
                {
                    if (!fin)
                    {
                        if (g.transform.position.Equals(mo.currentPos)) { }
                        actions.Add(new AIAction(e, AIAction.Actions.Attack,  g.transform.position));
                        fin = true;
                    }
                }
            }
            //If no one to attack then move towards the corner.
            if(!fin)
                actions.Add(new AIAction(e, AIAction.Actions.Move, cur + new Vector3(1, 1, 0)));
        }
    }
}
```
