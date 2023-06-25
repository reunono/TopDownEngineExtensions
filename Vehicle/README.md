# Vehicle

**Add drivable vehicles to a Top-Down Engine (TDE) scene.**

Enter, drive with wheel suspension forces, and exit damage-aware vehicles.


# Usage 

## Simple

1. Extract UnityPackage into TDE scene
1. Add `Vehicle.prefab` to scene
1. Run game. 
    1. Player walk to vehicle, see indicator. Player press interact button. Player disappear, and vehicle enabled. Player drive vehicle. Player press interact button. Player appear, vehicle disabled.

## Detailed - build your own vehicle

1. Set up TDE project similar to `MinimalScene2D` or `MinimalScene3D`
1. Add an empty GameObject to become a vehicle. These steps will add components and child GameObjects to it.
    1. Set the Transform to be above the ground. For testing move it near to the Player's spawn point.
    1. Add a `BoxCollider` Unity component.
    1. Add a `RigidBody` Unity component. Configure properties:
        1. Enable "Is Kinematic"
        1. Set realistic physics, e.g. mass to 3000, drag to 1, angular drag to 8
    1. Add the `Vehicle` component
        1. Disable it by un-checking in the Inspector. NOTE that this will be enabled at runtime, when the Player enters.
        1. TRICKY: This will be configured more, once the following vehicle-build instructions are completed.       
    1. Add `VehicleController` component
        1. Disable it by un-checking in the Inspector. NOTE that this will be enabled at runtime, when the Player enters.
    1. Add a visible child mesh GameObject: 3D Object > Cube. Resize it to be long and wide, but not tall.
    1. Create several child GameObjects for lifting the body out of the ground. Typicaly add four of these.
        1. Add `Suspension` component to each
        1. Optionally configure properties of: height off ground, springing bounciness, and dampening compression.
        1. Set Transform position to the bottom corners of the vehicle for each. 
            1. TIP: For more stability, move and rotate these suspension objects slightly outside the model bounds, to emulate a very wide "grip". Small vehicles can easily fall sideways with larger, sudden forces.   
    1. Create a child GameObject to become the enter zone, optionally named `"EnterZone"`.
        1. Add `ButtonActivatedZone` TDE component
        1. Add (+) Action for OnActivation(): use the parent vehicle object, call `Vehicle.enabled` function, check `True`
        1. Add `BoxCollider` Unity components. Position and size each to be appropriate for: where the Player should be located when they enter and exit. This is typically on the sides of the vehicle, by its doors.
        1. Finish configuring this in the Vehicle script, next.
    1. Finish configuring the `Vehicle` component, added previously, for driving
        1. Add (+) action for OnEnableEvent() and OnDisableEvent():
            1. For the **enter zone**, use your `"EnterZone"` object, call GameObject.SetActive, and check `True` for OnEnableEvent(). Duplicate and check `False` for OnDisableEvent().
            1. If you want your vehicle to **damage** other entities
                1. Add a child GameObject to the vehicle, with the `DamageOnTouch` TDE component, and a box collider. 
                1. Then use the DamageOnTouch object for this (+) Vehicle action, call GameObject.SetActive, and check `True` for OnEnableEvent(). Duplicate and check `False` for OnDisableEvent()`
    1. If you want your vehicle to have **health**, add a `Health` TDE component to the vehicle. Bind it to the GameObject's model.



# Contents

* [Vehicle](Vehicle.unitypackage) Unity Package - Installs vehicle system: scripts, Prefab, and Scene
* [Vehicle](Scripts/Vehicle.cs) Component - Hides player, activates vehicle forces and input controller, configures vehicle health
* [Vehicle](Vehicle.prefab) Prefab - Preconfigured vehicle, drop one or more into a scene
* [VehicleDemo](VehicleDemo.unity) Scene - Demo with Prefab and Scripts
* [VehicleController](ThirdParty/Vehicle-Physics/Scripts/VehicleController.cs) (3rd party) Component - moves the vehicle in the chosen direction. Listens to user input, converts input to appropriate physics impulse.
* [Suspension](ThirdParty/Vehicle-Physics/Scripts/Suspension.cs) (3rd party) Component - lifts vehicle off ground. Required. Use multiple of these to balance a vehicle, e.g. one for each wheel of a car.


# How it works

## Concepts 

* Enter
    * `Vehicle.enabled`: Enabling the vehicle hides the Player, enables suspension off the ground, and listens to directional input
* Exit
    * Disabling the vehicle shows the Player, and drops the vehicle suspension
* Drive
    * Pushes the vehicle based upon `"Player1_Horizontal"` and `"Player1_Vertical"` input. (See TDE `StandaloneInputModule` Component.) 
    * Suspension prevents vehicle from touching the ground and stopping early.

## Steps

1. Vehicle's EnterZone listens for default Interaction button
1. Player walks to EnterZone and presses the action key
1. Player is hidden, and Vehicle is enabled 
1. Suspension(s) lift the vehicle body off the ground. VehicleController listens for input. Vehicle waits.
1. Player presses directional buttons. VehicleController calculates and imparts forces on body. Vehicle moves.
1. Player presses `"Space"`, is visible, and Vehicle is disabled.


# Tips

For vehicle stability, this system is dependent configurations of its components. When the vehicle shudders, flips, and moves undesirably, check:

* sizes and positions of its colliders (including inside DamageOnTouch) and Suspension objects: they should "fit" the model
* Rigidbody mass, drag, angular drag should be non-zero
