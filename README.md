# Behavior Graph
A visual graph editor facilitating the creation of simple and complex AI behaviors

## Features
* A dynamic, user-friendly editor window
* Support for fundamental node transitions
* Easily create custom nodes and transitions
* Quick and easy integration to other `MonoBehaviour` components, enabling dynamic field modifications for nodes and transitions

## Editor Usage
### Using the graph editor
1. Create a new `BehaviorPanel` scriptable object using the context menu.
2. Double click scriptable object, or click `Window -> Behavior Graph Editor` and select scriptable object.
3. Press `Space`, or `Right click -> Create Nodes`, to open search window for nodes.
4. `Right click` any node for more options, like creating transitions.
5. Select any node or edge to see values in the inspector to the left.
### Integrating separate `MonoBehaviour` component for dynamic field updates
1. Create a new script and inherit `BehaviorData`.
2. Variables that can be accessed, and used by, behavior graph should be created using the generic `DataField<T>` class.
3. Add logic as usual.
4. Select the desired `BehaviorPanel` and drag in the new class, before clicking `Link Behavior Data`.
5. You can now see all the serialized fields in the blackboard of the graph editor!
6. To use fields, select the desired node or edge.
7. Click on a variable in the blackboard and then click on the variable you wish to link it to in the inspector.
8. You can unlink fields by just clicking them again.
### Creating custom nodes and node transitions
**Nodes**
1. In the context menu under `Behavior Graph` choose `Custom Node Script`.
2. This creates a script which inherits from the `Node` class.
3. `Node` contains a Start, Update and Exit method, use as necessary.
4. `Node` also contains an enum, `Statuses`, which contain Success, Running and Failure. Each `Node` has a `CurrentStatus`, there are base transitions which exits a node depending on whether or not the nodes `CurrentStatus` is in Success or Failure.
5. All fields that you wish to serialize to the inspector, that should be linked to the blackboard, should be of the generic type `DataField<T>`, variables that are not of this type will be shown in the inspector and can still be used within the node, but it cannot be linked to the blackboard fields.
6. Search for your new node in the graph editor and use as you please! *Note: If you want your node to be considered a base node by the system, use the `IBaseBehaviorElement` interface in your node*.

**Transitions**
1. In the context menu under `Behavior Graph` choose `Custom Transition Script`.
2. This creates a script which inherits from the `NodeTransitionObject` class.
3. The `Condition` method will tell the system to switch node when it returns `true`.
4. You also need to use `DataField<T>` here in the same way as you would for `Node` objects.
5. Most transitions should only be used once per node; however, if your new transition should be able to be used by the same node multiple times, then use the `IAllowMultiTransitionElement` interface.
6. Right click a node and you can find your new transition within the context menu, under the custom section! *Note: You can also use the `IBaseBehaviorElement` interface here to get your new transition to show up under the base section*.

## Runtime Usage
1. Add a `BehaviorTree` component to the game object, also add the class that derives `BehaviorData` if used for overrides.
2. Reference your `BehaviorPanel` in the `Panel` field of the `BehaviorTree`.

## License
This package is licensed under the MIT License. For more information read: `LICENSE`.

## Additional Notes
The tool is surprisingly performant, but it works using a lot of reflection, and cloning of `Scriptable Objects` in some cases, which is inherently quite costly.

The tool also has some serialization issues right now, which means that if you are unlucky the tool may just stop working for a panel. I am trying to fix this as fast as possible

Due to the fact that the system uses `Scriptable Objects` as nodes and transitions, you may have to use the `[NonSerialized]` attribute to avoid Unity from serializing the runtime data and thus overriding your default values for variables. This is not a problem for all fields, but if you notice the issue just use the attribute.

The use of [UPM Git Extension](https://github.com/mob-sakai/UpmGitExtension) is highly recommended, for easy updating of this package.
