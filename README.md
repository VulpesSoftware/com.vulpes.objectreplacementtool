# Object Replacement Tool

A tool for quickly replacing multiple objects at once within the Unity Editor.

What's this?
- It's a Unity Editor Tool that can be used to quickly replace multiple objects at once.

What can I do with the code?
- You can pretty much do whatever you want with the code provided here, just don't go trying to sell it, that's kind of a shitty thing to do given that I've released it here for free.
- You're welcome to integrate it into larger projects and/or frameworks if you want, you're not expected to release your entire code base as a result of integrating this into a larger project or collection of code, that would be a bit silly.

How do I use it?
- Download 'ObjectReplacementTool.cs' or clone the repo if you like.
- Add 'ObjectReplacementTool.cs' to your Unity project within a folder called 'Editor'.
- Once added to your project the window can be accessed via the 'GameObject/Replace Object...' menu item.
- From the Object Replacement Tool Window you can search for and select the objects you want to replace or select them manually through the Scene or Hierarchy views.
- Then simply assign the Object you wish to replace the selection with to the 'Replacement Object' field.
- Additional behaviours such as transform offsets and hierarchy inheritance can be configured from the Object Replacement Tool window prior to replacing the selection.

Plans for future updates?
- Eventually I want to allow the user to have objects be offset relative to the instance(s) they are replacing rather than just using world offsets.
