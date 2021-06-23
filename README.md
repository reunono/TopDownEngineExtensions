## TopDown Engine Extensions
This repository contains community-created extensions for the TopDown Engine, More Mountains' bestselling top down games solution, [available on the Unity Asset Store](https://assetstore.unity.com/packages/templates/systems/topdown-engine-89636?aid=1011lKhG). These extensions can be anything, from an alternate take on an existing Character Ability, to brand new ones, or new ways to use the engine.

## Contents
* **CharacterProne**, _by AlexanderGheorghe_ : duplicate of CharacterCrouch that allows you to have different animations (Animator parameters "Prone" and "ProneMoving") and different collider resizing, useful for making a character prone like in Metal Gear Solid, for example
* **TwinStickShooterHandleWeapon**, _by AlexanderGheorghe_ : an extension of CharacterHandleWeapon that gives you the option to shoot on receiving SecondaryMovement input, with configurable minimum magnitude of the input
* **MultipleWeapons**, _by AlexanderGheorghe_ : two scripts, extensions of CharacterHandleWeapon and InputManager that go together to allow you to have your characters handle any number of weapons you want at once, from 0 to 1000000, the possibilities are infinite! includes an InputManager.asset with already configured bindings for the second weapon reload and third weapon input (2, 3 for reloading second and third weapon, middle mouse for shooting the third weapon)
* **AI Brain Extensions**, _by TheBitCave_ : These are hosted separately, at [https://github.com/thebitcave/ai-brain-extensions-for-topdown-engine](https://github.com/thebitcave/ai-brain-extensions-for-topdown-engine), and provide a way to interact with AI Brains using visual nodes.
* **AIDecisionPlayerIsInSameRoom**, _by 石田ぎがしー Gigacee_, an AI decision that returns true if the player is in one of the specified rooms, false otherwise.
* **AI Performance Manager**, _by Force_ : A system used to disable AIs based on distance to the player for better performance
* **ClimbingController3D**, _by AlexanderGheorghe_ : An extension of TopDownController3D which gives you the option (activated by default) to have CharacterDash3D climb slopes instead of stopping at obstacles and to move the character downwards while dashing, at a configurable speed. It uses CharacterController.Move(), which means you no longer need to put objects to avoid when dashing on the layers of the obstacles layer mask, because the CharacterController will be handling collisions.
* **Character8WayGridMovement**, _by @jcphlux_ : adds support for 8-way grid movement.
* **Control Freak 2 Integration**, _by christougher_ : support for the input solution Control Freak 2, available on the Asset Store.
* **HeldButtonZone**, _by Dougomite_ : a button activated zone for which the player needs to keep the button pressed for a certain duration to activate
* **MMFeedbackLootDrops**, _by Dougomite_ : an MMFeedback that spawns "loot" (item pickers or any object you want) in a certain radius at weighted chances
* **MultiInventoryDetails**, _by men8_ : an addon to handle all active inventories on scene. See [this repo](https://github.com/men8/MultiInventoryDetails) for more info.
* **PathfindingAvoidance**, _by septN_ : this CharacterPathfinder3D extension lets you have characters carve the navmesh and avoid each other. You'll need to add a NavMeshObstacle to your character(s), check "Carve" and uncheck "Carve Only Stationary"
* **ReferenceFrameCharacterMovement**, _by Necka_ : a specialized variant of the Character Movement ability that corrects for a reference frame camera

## Why aren't these in the engine directly?
Because they weren't created by Renaud, the creator of the TopDown Engine, because I want to keep the Engine simple to use and just pouring features into it (as cool as they may be) wouldn't be such a great idea, and because the Engine is meant to be extended upon, and these extensions are great examples of that.

## Do I need to pay something? Give credit?
No you don't, it's all free to use. Feel free to give credit to their creators though (they'll mention that in the class' comments if they want to).

## Are more extensions coming? Can I contribute?
Absolutely, contributions are always welcome. If you want to contribute, drop me a line using [the form on this page](https://topdown-engine.moremountains.com/topdown-engine-contact), or create a pull request on this very repository.

## Hey this extension is not working!
You'd need to contact the creator of the extension if they've put their contact info in the class somewhere. If not, not much I can do, I won't provide support for these.
