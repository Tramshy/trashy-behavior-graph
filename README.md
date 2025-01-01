# Behavior Graph
A visualized graph editor, allowing for simple, and complex, AI creation

## Features
* A dynamic and easily usable editor window
* Base transitions between nodes
* Allows for custom nodes and node transitions to be easily created
* Quick and easy integration to other `MonoBehavior` components, allows for dynamic field changes for nodes and transitions

## Editor Usage
### Using the graph editor
1. Create a new `BehaviorPanel` scriptable object using context menu.
2. Double click scriptable object, or click `Window -> Behavior Graph Editor` and select scriptable object.
3. Press `Space`, or `Right click -> Create Nodes`, to open search window for nodes.
4. `Right click` any node for more options, like creating transitions.
5. Select any node or edge to see values in the inspector to the left.
### Integrating separate `MonoBehavior` component for dynamic field updates
1. Create a new script and inherit `BehaviorData`.
2. Variables that can be accessed, and used by, behavior graph should be created using the generic `DataField<T>` class.
3. Add logic as usual.
4. Select the desired `BehaviorPanel` and write the full class name in the `Data Component Name` field.
5. You can now see all the serialized fields in the blackboard of the graph editor!
6. To use fields, select desired node or edge.
7. Add a new element to the `Field Overrides` and input the node field name to override and the component field name to override with.
### Creating custom nodes and node transitions
**Nodes**
1. Create a new script and inherit `Node`.
2. `Node` contains a Start, Update and Exit method, use as necessary.
3. `Node` also contains an enum, `Statuses`, which contain Success, Running and Failure. Each `Node` has a `CurrentStatus`, there are base transitions which exits a node depending on whether or not the nodes `CurrentStatus` is in Success or Failure.
4. All fields that you wish to serialize to the inspector, and that can be overridden, should be of the generic type `DataField<T>`, variables that are not of this type will not be shown in the inspector, but can still be used within the node.
5. Search for your new node in the graph editor and use as you please! *Note: If you want your node to be considered a base node by the system, use the `IBaseBehaviorElement` interface in your node*.

**Transitions**
1. Create a new script and inherit `NodeTransitionObject`.
2. The `Condition` method will tell the system to switch node when it returns `true`.
3. You also need to use `DataField<T>` here in the same way as you would for `Node` objects.
4. Most transitions should only be used once per node; however, if your new transition should be able to be used by the same node multiple times, then use the `IAllowMultiTransitionElement` interface.
5. Right click a node and you can find your new transition within the context menu, under the custom section! *Note: You can also use the `IBaseBehaviorElement` interface here to get your new transition to show up under the base section*.

## Runtime Usage
1. Add a `BehaviorTree` component to the game object, also add the class that derives `BehaviorData` if used for overrides.
2. Reference your `BehaviorPanel` in the `Panel` field of the `BehaviorTree`.
3. Enjoy!

## Dependency: `TrashyTools`
This package depends on the `TrashyTools` package, which is an MIT Licensed utility library for Unity (Created by me :D). `TrashyTools` is automatically installed when you add the Behavior Graph Package.

For more information, visit the [Trashy Tools repository](https://github.com/Tramshy/trashy-tools-package)

## License
This package is licensed under the MIT License. For more information read: `LICENSE`.

## Additional Note
The use of [UPM Git Extension](https://github.com/mob-sakai/UpmGitExtension) is highly recommended, for easy updating of this package.
