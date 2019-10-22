<b>Object Replacement Tool</b>

A tool for quickly replacing multiple objects at once within the Unity Editor.

<b>Getting Started: Importing From a Local Repo</b>
- Clone or download the repo to your local machine
- Open up your Unity Project
- Open the Package Manager window by navigating to 'Window/Package Manager'
- Click on the '+' icon button and 'Add package from disk...'
- Navigate to wherever you installed the local repo and select the 'package.json' file.

<b>Getting Started: Importing via Git</b>
- Navigate to the root folder of your Unity Project in Finder (Mac) or Explorer (Windows).
- Open the 'Packages' folder and open the 'manifest.json' file.
- Add '"com.vulpes.objectreplacementtool": "https://github.com/VulpesSoftware/com.vulpes.objectreplacementtool.git#1.0.0",' to the dependencied list.
- Reopen your Unity Project and if all goes well the package should import.

<b>How do I use it?</b>
- Once added to your project the window can be accessed via the 'GameObject/Replace Object...' menu item.
- From the Object Replacement Tool Window you can search for and select the objects you want to replace or select them manually through the Scene or Hierarchy views.
- Then simply assign the Object you wish to replace the selection with to the 'Replacement Object' field.
- Additional behaviours such as transform offsets and hierarchy inheritance can be configured from the Object Replacement Tool window prior to replacing the selection.