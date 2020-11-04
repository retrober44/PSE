# So you have chosen frontend.
Good choice. Very courageous of you to build something customer-facing, when facing anyone is currently to be considered dangerous.

## Getting started
If you don't have one already, obtain a Unity student plan subscription. As a student, you get Unity Pro for free!
You simply have to link your Github Student account.  
https://store.unity.com/de/academic/unity-student

In case you have never worked with Unity before, Unity themselves have a learning portal: https://learn.unity.com/  
You will be able to find all kinds of resources, from 2D to 3D there. Keep in mind that **this is a 2D project**.

Unity's scripting language is C# (Mono). I highly recommend using Jetbrains Rider to write code, it's integrated with Unity
and - in my opinion - the best C# IDE available.


## Recommendations
The actual design of the game's implementation is entirely up to you. This is just a procedure that I personally like to use, and I think
it will be helpful for a team, by defining a contract / common interface you'd like to use and then designing smaller components.
It might make integration easier.

Prototype gameplay elements. Build an element, for example the character, in itself by for example creating a prototype scene,
putting a rectancle in it and then designing the MonoBehaviour(s) (i.e. the element's logic).
For a player, that would mean "walking" up/down/left/right for one "field" (which could be one unit, for example).
When that works, add the ability to restrict movement, as a character should not be able to move backwards when another character is standing behind it.
The more defined the elements get, the easier it will be to integrate them with the work of teammates later.
A character that can have its movement restricted will tie in nicely with a system that checks the fields around the player to see if they are free (for example a "board").
An event system will be a nice way to send notifications to other MonoBehaviours without having to call GetComponent everywhere, for example, a central GameManager
that handles networking as well could send an event like "Character3-3 is dead!". Character3-3, like all the characters, subscribes to the GameManagers "Characters" event channel.
This way, the character will immediately know it's dead, can play an animation and then disable itself (its GameObject), for example. 

## Further information
We will be building for WebGL, so this game can be integrated into an Ionic app easily.
Should we run into any issues with WebGL not supporting certain things that we need, we'll find possible solutions as a team.
