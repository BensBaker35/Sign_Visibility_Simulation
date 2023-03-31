# Sign Simulation Tool
## Summary
Created as a part of [Professor Brian Tomaszewski's NSF Civic Grant](https://www.rit.edu/gccis/geoinfosciencecenter/nsf-civic). A proof of concept for a tool to help emergency management create better communication strategies for Deaf and Hard of Hearing peoples. Only a small portion of downtown Rochester (The area bounded by the Genessee River and Inner Loop Expressway) is available. The project is split into two main scenes, Explorable and Editing. A demo version also exists which just includes the Explorable scene with preset objects.

### Explorable
Uses the ArcGISMapComponent to display a 3D explorable version of the enviorment. Users view the enviorment through a third-person camera, controlling an avatar. There are two different types of objects in the scene besides the player, obstacles and signs. Objects are dynamically loaded from real world coordinates and placed into the game world using the ArcGISLocationComponent. When in proximity to a sign, they preform a raycast to the player. If the player is the first object that is hit, then a line is drawn from the sign to the player green. Otherwise the line is drawn red.

### Editing
Much like the Explorable scene, this also makes uses of the same ArcGISMapComponent. However, this scene instead uses a top down camera to provide a better view of the area. Signs are placed by clicking with the mouse, the type can be selected from an editor on the right side of the screen. This editor also allows for fine tuning of the objects coordinates. Once done making placements, the user can save the edits and load the explorable experience. 

## Non-ArcGIS SDK Systems

### Service Locator
Simple dependency injection service operating as a Singleton. All services implement an interface that is or extends `IService`, for example the Serializer has the interface `ISerializationService`. Services should be registerd in the `Awake` function of each monobehavior. For any script that is then looking for a service should then request them in the `Start` method. Internally, the `ServiceLocator` uses a dictonary keyed by a type that implements IService and has values of type object. This means that objects must be casted to the correct service interface upon reciving them.

### Events
The `EventManager` does not operate as a service like most other highly used systems, it is also another Singleton. The `EventManager` makes use of C#'s delegates to send event data to multiple listeners. The Events namespaces has an enum `Event` that describe the different events that are available. Events can easily be added by adding another line to the Enum. The same conventions follow as the Service Locator, register in Awake. It is also important to remove listeners when the object is no longer valid. Use the Unity `OnDisable` function to remove a listener.

Registering a listener for an event is relatively simple, one needs to specify the specific event and a callback function with the following signature ```void HandlerName(object package) ```. The same process can be used to remove a listener from an event.

Any script can trigger an event. Events are triggered selecting an event and providing a package. The package can be null or anything that inherits from the C# object base class. Handlers will have to cast the object into more concrete types inorder to manipulate it.

### UI
The User Interface is provided by the `GeneralUIManager` which is registered as the `IUIService`. It listens for changes to the scenes. When there is a change in the scene it will then load the proper UI resources using the Unity Resource system. UI objects are stored as `UIInjector` scriptable objects. The `UIInjector` scriptable objects maintain a list of different Unity GameObjects/Prefabs. When the scene changes, the UI clears all objects currently in the scene. Gets the `UIInjector` object, and instatiates all the objects it contains.


### SceneBootstrap
`SceneBootStrap` is an important Monobehavior that is present in both Start and Start_Demo. Much like an OS bootstrap it sets up the files and resources for the rest of the tool to use. In the `Awake` function, it registers and creates many of the services with the Service Locator. It also makes sure that the UI scene is additively loaded in. 

#### Scenes
Unity provides two different ways to load/view scenes, single and additive. Scenes that are loaded as single can be the only scene loaded/viewed at one time. There can be multiple additive scenses active at one time. 

The current project structure is to use additive scenes, at anytime there are at most two scenes active. UI is seperated out into it's own scene and therefore it is always loaded and active. The start, loading, explorable, and editing scenes are alternated between using the scene managager. 

### Serialization
Three different interfaces are provided for accessing serialized data, binary, text, and Unity. The Unity serializer is simply a wrapper over the Unity resource system. Only created for completeness/homogenuity. 

## ArcGIS SDK Tips
* The ArcGISRebaseComponent can cause issues in objects that are without ArcGISLocationComponents. For small areas this isn't an issue. In larger areas due to the limits of Unity's floating point coordinate system there might be errors.
* RigidBody Components, or any GameObject that is dynamicaly moved, should not have an ArcGISLocationComponent. Instead there should be a parent gameobject that has an ArcGISLocationComponent, in the editing/explore scenes, this is the Physics Root. 
* When instantiating new objects with ArcGISLocationComponents, you must first get the component from the new game object, then set it to enabled. After that you can then update it's location. 
* As of ArcGIS SDK 1.1 the built in geometry functions do not provide any graphical output, based on readings from the forum there doesn't seem to be any plans to change that.