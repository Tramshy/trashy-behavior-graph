# Behavior Graph
A visual graph editor facilitating the creation of simple and complex AI behaviors

## Features
* A dynamic, user-friendly editor window
* Support for fundamental node transitions
* Easily create custom nodes and transitions
* Quick and easy integration to other `MonoBehaviour` components, enabling dynamic field modifications for nodes and transitions

## Installation
This repository is installed as a package for Unity.
1. `Open Window` > `Package Manager`.
2. Click `+`.
3. Select Add Package from git URL.
4. Paste `https://github.com/Tramshy/trashy-behavior-graph.git`.
5. Click Add.

NOTE: To do this you need Git installed on your computer.

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
4. `Node` also contains an enum, `Statuses`, which contain Success, Running and Failure. Each `Node` has a `CurrentStatus`, there are base transitions which exits a node depending on whether or not the node's `CurrentStatus` is in Success or Failure.
5. All fields that you wish to serialize to the inspector, that should be linked to the blackboard, should be of the generic type `DataField<T>`, variables that are not of this type will be shown in the inspector and can still be used within the node, but it cannot be linked to the blackboard fields.
6. Search for your new node in the graph editor and use as you please! *Note: If you want your node to be considered a base node by the system, use the `BaseBehaviorElement` attribute in your node*.

**Transitions**
1. In the context menu under `Behavior Graph` choose `Custom Transition Script`.
2. This creates a script which inherits from the `NodeTransitionObject` class.
3. The `Condition` method will tell the system to switch nodes when it returns `true`.
4. You also need to use `DataField<T>` here in the same way as you would for `Node` objects.
5. Transitions can by default only be used once per node; however, if your new transition should be able to be used by the same node multiple times, then use the `AllowMultipleTransition` attribute.
6. Right click a node and you can find your new transition within the context menu, under the custom section! *Note: You can also use the `BaseBehaviorElement` attribute here to get your new transition to show up under the base section*.

**Triggers**
1. In the context menu under `Behavior Graph` choose `Custom Trigger Script`.
2. This creates a script which inherits from the `TriggerTransition` class.
3. The `Condition` method will tell the system to switch nodes when it returns `true`.
4. Triggers are not checked automatically, instead you will have to use the `CallTrigger` method when you want to attempt a switch. This will then switch if the `Condition` returns true.
5. The `BaseBehaviorElement` and `AllowMultipleTransition` attributes can be used for Triggers as well.

## Runtime Usage
1. Add a `BehaviorTree` component to the game object, also add the class that derives `BehaviorData` if linking between blackboard is used.
2. Reference your `BehaviorPanel` in the `Panel` field of the `BehaviorTree`.

### Using Triggers
Triggers act very similar to regular transitions, the big difference is that triggers will not be checked automatically. This means you will need to get a reference to the triggers you wish to call. For this you can use the `GetNode` method in a `Panel` and then the `GetTrigger`, or `GetTriggers`, method in the specific node. When you have your reference just use the `CallTrigger` method to attempt a switch.

There are several base triggers that you can use by default. Most of them are self explanitory, but the `SimpleTrigger` can be quite unclear what it does without looking in the code. Its `Condition` always returns true. This means that whenever you use the `CallTrigger` method, it will switch.

## License
This package is licensed under the MIT License. For more information read: `LICENSE`.

## Additional Notes
The tool is surprisingly performant, but it works using a lot of reflection, and cloning of `Scriptable Objects` in some cases, which is inherently quite costly.

You can use the `ReadOnlyInspector` attribute on the fields you intend to link, this will make it very clear what fields you need to link.

When you try to link integers, floats and similar fields, you may run into an issue that prevents you from clicking the field in the inspector. If you try to click on the field in the inspector, it will instead result in you dragging the value of the field and will not link the fields. To fix this, all you need to do is click at the very edge of the field or use the `ReadOnlyInspector` attribute.

Due to the fact that the system uses `Scriptable Objects` as nodes and transitions, you may have to use the `[NonSerialized]` attribute to avoid Unity from serializing the runtime data and thus overriding your default values for variables. This is not a problem for all fields, but if you notice the issue just use the attribute.
