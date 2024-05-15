## TopDown Engine Extensions

This repository contains community-created extensions for the TopDown Engine, More Mountains' bestselling top down games solution, [available on the Unity Asset Store](https://assetstore.unity.com/packages/templates/systems/topdown-engine-89636?aid=1011lKhG). These extensions can be anything, from an alternate take on an existing Character Ability, to brand new ones, or new ways to use the engine.

## Contents

* **AngleBasedAutoAim**, _by AlexanderGheorghe_ : extensions of WeaponAutoAim2D/3D that aim at the target with the smallest angle between the movement direction and the character-target direction instead of the closest target
* **AOEWeapon**, _by Sargorath_ : a weapon class aimed specifically at allowing the creation of various projectile weapons, from shotgun to machine gun, via plasma gun or rocket launcher
* **AStar Pathfinder3DAiAction**, _by ZUR1EL_ : Add this Ai Action to your enemy AiBrain to allow A* to control them.
* **AIAction2DPathFinding**, _by Gabriel Elkind_ : an A* Pathfinding Project integration
* **AIActionMoveTowardsPlayer2D**, _by Efreeti_ : makes the character move up to the specified MinimumDistance in the direction of the target.
* **AI Brain Extensions**, _by TheBitCave_ : These are hosted separately, at [https://github.com/thebitcave/ai-brain-extensions-for-topdown-engine](https://github.com/thebitcave/ai-brain-extensions-for-topdown-engine), and provide a way to interact with AI Brains using visual nodes.
* **AIDecisionPlayerIsInSameRoom**, _by 石田ぎがしー Gigacee_, an AI decision that returns true if the player is in one of the specified rooms, false otherwise.
* **AI Performance Manager**, _by Force_ : A system used to disable AIs based on distance to the player for better performance
* **BlockAbilities**, _by AlexanderGheorghe_: scripts for blocking selected character abilities while attacking
* **Boomerang**, _by AlexanderGheorghe_ : a component that makes Projectile decelerate and return to the owner when it stops. works in both 2D and 3D. includes 3D demo, unitypackage for easy installation and boomerang model from [The Base Mesh](https://thebasemesh.com/model-library)
* **AdvancedCharacterGridMovement**, _by [JCPhlux](https://github.com/jcphlux)_ : Inhanced version of CharacterGridMovement that allows for more control over the movement of the character. Single script that works for both 2D and 3D.
    Options include:
    * Diagonal Movement: When true allows for diagonal movement.
    * Ignore Cardinal obstacles: When true allows for diagonal movement even if blocked by Cardinal Direction Obstacles.
    *Ignore Cardinal obstacles is still a WIP for 3D movement but works for 2D movement.*
    * Ignore Cardinal obstacle Movement State: Configurable movement state to set the movement state to when moving through a cardinal direction obstacle. *ex. set to "Jumping" to set the state to "Jumping" when moving through a cardinal direction obstacle.*

    Report a bug or request a feature [here](https://github.com/jcphlux/TopDownEngineExtensions/issues)

* **CharacterInventoryWeaponChanger**, _by AlexanderGheorghe_ : this script allows you to use scroll wheel or number keys to change your weapon to any weapon from your MainInventory
* **CharacterMultiButtonActivation**, _by AlexanderGheorghe_ : extension of CharacterButtonActivation that lets characters interact with multiple ButtonActivated that they're touching
* **CharacterRunWhenFaceDirection3D**, _by Aitor F._: Extension of CharacterRun that only permit the Character to run when is facing the movement direction, allowing for a small angular threshold. Useful when using a Weapon with AutoAim or when AimControl is set to Secondary Movement. For example, you can run aiming towards an enemy, but if you aim while moving away you can't.
* **CharacterWeaponMagazineMemorizer**, _by AlexanderGheorghe_ : Memorize the remaining ammo on the weapon so when switching back to it, it retains the current ammo instead of a full clip.
* **CharacterOrientation3DExtensions**, _by AlexanderGheorghe_ : extension methods for forcing character orientation towards a transform. includes a usage example in CharacterForceLookAtZone
* **CharacterProne**, _by AlexanderGheorghe_ : duplicate of CharacterCrouch that allows you to have different animations (Animator parameters "Prone" and "ProneMoving") and different collider resizing, useful for making a character prone like in Metal Gear Solid, for example
* **ClimbingController3D**, _by AlexanderGheorghe_ : An extension of TopDownController3D which gives you the option (activated by default) to have CharacterDash3D climb slopes instead of stopping at obstacles and to move the character downwards while dashing, at a configurable speed. It uses CharacterController.Move(), which means you no longer need to put objects to avoid when dashing on the layers of the obstacles layer mask, because the CharacterController will be handling collisions.
* **Control Freak 2 Integration**, _by christougher_ : support for the input solution Control Freak 2, available on the Asset Store.
* **ConveyorBelts**, _by AlexanderGheorghe_, : 3D conveyor belts with implementations for moving Characters and Rigidbodies
* **DeflectProjectiles**, _by AlexanderGheorghe_, : put this component in your scene and it will make all projectiles deflect off of the objects they deal damage to
* **ExtendedDialogueZome**, _by AlexanderGheorghe_, : DialogueZone extension that adds an OnDialogueEnd action, executed after all dialogue lines have been displayed
* **GrapplingHook**, _by AlexanderGheorghe_, : 3D grappling hook implementation with [rope animation](https://youtu.be/tPtKNvifpj0). configurable to pull owner towards hit object or the other way around
* **GridPathfinding**, _by AlexanderGheorghe_, : grid versions of CharacterPathfinder3D and CharacterPathfindToMouse3D
* **HeldButtonZone**, _by Dougomite_ : a button activated zone for which the player needs to keep the button pressed for a certain duration to activate
* **InvertedMeshConeOfVision2D**, _by AlexanderGheorghe_ : version of MMConeOfVision2D with inverted mesh (mesh is where the character doesn't have vision)
* **MarbleCharacter**, _by AlexanderGheorghe_ : scripts that enable creating marble type characters. includes example character prefab
* **MMFeedbackLootDrops**, _by Dougomite_ : an MMFeedback that spawns "loot" (item pickers or any object you want) in a certain radius at weighted chances
* **MouseControls3D**, _by AlexanderGheorghe_ : a collection of extension scripts that implement Diablo-like mouse controls (double click to run, click and hold to make the character follow mouse position, click an enemy to pathfind to it and attack when in line of sight and in range). requires adding the _public float Range = 10f;_ declaration to Weapon.cs so the character knows when it is in range to attack, depending on the weapon he has equipped. includes unitypackage for easy installation and an example character prefab
* **MultiInventoryDetails**, _by men8_ : an addon to handle all active inventories on scene. See [this repo](https://github.com/men8/MultiInventoryDetails) for more info.
* **MultipleKeyOperatedZone**, _by AlexanderGheorghe_ : extension of KeyOperatedZone that allows for requiring multiple keys. useful for trading a number of inventory items for something, for example when used in conjunction with InventoryEngineChest for setting up a simple shop system, where the keys are coins
* **MultipleWeapons**, _by AlexanderGheorghe_ : two scripts, extensions of CharacterHandleWeapon and InputManager that go together to allow you to have your characters handle any number of weapons you want at once, from 0 to 1000000, the possibilities are infinite! includes an InputManager.asset with already configured bindings for the second weapon reload and third weapon input (2, 3 for reloading second and third weapon, middle mouse for shooting the third weapon)
* **PathfindingAvoidance**, _by septN_ : this CharacterPathfinder3D extension lets you have characters carve the navmesh and avoid each other. You'll need to add a NavMeshobstacle to your character(s), check "Carve" and uncheck "Carve Only Stationary"
* **Perspective**, _by AlexanderGheorghe_ : a collection of scripts that allows toggling between top down and first person perspectives. includes demo scene based on Loft3D and unitypackage
* **PickableAbilities**, _by AlexanderGheorghe_ : PickableItem extension that allows configuring a list of abilities to be enabled on the picking character
* **PlayerDamageMultiplier**, _by AlexanderGheorghe_ : a component that goes in your scene and multiplies all damage dealt by the player character. to change the multiplier you can either do `Player.DamageMultiplier = newMultiplier;` from a script or call one of `PlayerDamageMultiplier`'s `Set`, `Add` or `Multiply` from an UnityEvent or some visual scripting system
* **PredictiveAim3D**, _by ZUR1EL_ : Add this Ai Action to your enemy AiBrain in replace of AiActionShoot3D to allow your enemy to lead targets.
* **ProgressionSystem**, _by AlexanderGheorghe_ : a simple, extendable, scriptable object based progression system (get experience -> level up -> update stats). uses AnimationCurves for value management, as explained in [this video](https://youtu.be/Nc9x0LfvJhI)
* **ProjectileHoming**, _by AlexanderGheorghe_ : a component that gets a target on spawn and rotates Projectile's direction towards the target every frame, stopping if it misses the target. works in both 2D and 3D
* **ReferenceFrameCharacterMovement**, _by Necka_ : a specialized variant of the Character Movement ability that corrects for a reference frame camera
* **RewiredIntegration**, _by Tony Li [Pixel Crushers]_ : support for the input solution Rewired, available on the Unity Asset Store.
* **RoomPlayerTriggerEnter**, _by AlexanderGheorghe_ : component that does room enter setup when the player enters the room's trigger collider. useful if you don't want to use teleporters
* **RootMotionMovement**, _by AlexanderGheorghe_ : allows root motion animations to move the top down controller 3D
* **SaveIKHandlesPositionInPlayMode**, _by AlexanderGheorghe_ : a component that goes on the Weapon/WeaponModel GameObject in a prefab asset and saves the play mode changes to the local position and rotation of the IK handles to the prefab asset
* **SetBrainTargetToDamageInstigatorOwner**, _by AlexanderGheorghe_ : put this component in your scene and it will set the Target of characters' AIBrains to the damage instigator's owner when they get hit
* **SpecificInvulnerabilityHealth**, _by AlexanderGheorghe_ : an extension of Health that calculates invulnerability per instigator GameObject (DamageOnTouch usually). this allows you to have overlapping damage on touch all affect your characters and count down invulnerability individually
* **SpeedMultipliers**, _by AlexanderGheorghe_ : extension scripts that add speed multipliers. includes demo of slowing down all enemy characters while the player continues to move at normal speed
* **Stamina**, _by AlexanderGheorghe_ : simple stamina that gets consumed when running or dashing and stops either if it's too low
* **StatusEffectSystem**, _by AlexanderGheorghe_ : a simple, extendable, scriptable object and event based status system. includes 5 status effect examples, a demo scene and a .unitypackage for easy installation
* **TankControls**, _by AlexanderGheorghe_ : scripts for implementing tank controls (horizontal axis rotates player model, vertical moves the character along the direction it's facing). set Movement to Strict 2 Directions Vertical and replace CharacterOrientation3D with CharacterRotation
* **TemporaryEffects**, _by AlexanderGheorghe_ : scripts for implementing temporary effects. includes damage, projectile speed and weapon delay between use multipliers and projectile weapon spread changing. see demo scene for an example
* **ToggleCrouch**, _by AlexanderGheorghe_ : CharacterCrouch extension that makes the crouch button toggle instead of having to hold it to keep crouching
* **TriggerEventOnAreaClear**, _by AlexanderGheorghe_ : a script that triggers a UnityEvent when all the targets within an area (defined by a 2D or 3D collider) have been destroyed (useful for example in the room system to open doors/portals or spawn chests when all enemies have been killed). tag targets with the respective 2D/3D scripts included
* **TwinStickShooterHandleWeapon**, _by AlexanderGheorghe_ : an extension of CharacterHandleWeapon that gives you the option to shoot on receiving SecondaryMovement input, with configurable minimum magnitude of the input
* **TypedDamage**, _by AlexanderGheorghe_ : a collection of extension scripts that implement typed damage with scriptable objects, like explained in [this video](https://youtu.be/_q21rEaSlAs).
* **Vehicle**, _by AlexanderGheorghe_ : allows creating vehicles that the player character can enter, drive and exit. uses [Hayden Donnelly's Vehicle-Physics](https://github.com/hayden-donnelly/Vehicle-Physics).
* **WeaponAutoDestroyWhenEmpty**, _by AlexanderGheorghe_ : component that enables the Auto Destroy When Empty option to work without the Weapon Ammo component. goes on your magazine based, Auto Destroy When Empty weapon
* **WeaponCooldownProgressBar**, _by AlexanderGheorghe_ : put this component on a MMProgressBar and it will be updated with the weapon's cooldown progress
* **WeaponLaserSightEx**, _by Velsthinez_ : an extended version of the base WeaponLaserSight to create a widening laser when player turns

## Why aren't these in the engine directly?

Because they weren't created by Renaud, the creator of the TopDown Engine, because I want to keep the Engine simple to use and just pouring features into it (as cool as they may be) wouldn't be such a great idea, and because the Engine is meant to be extended upon, and these extensions are great examples of that.

## Do I need to pay something? Give credit?

No you don't, it's all free to use. Feel free to give credit to their creators though (they'll mention that in the class' comments if they want to).

## Are more extensions coming? Can I contribute?

Absolutely, contributions are always welcome. If you want to contribute, drop me a line using [the form on this page](https://topdown-engine.moremountains.com/topdown-engine-contact), or create a pull request on this very repository.

## Hey this extension is not working!

You'd need to contact the creator of the extension if they've put their contact info in the class somewhere. If not, not much I can do, I won't provide support for these.
