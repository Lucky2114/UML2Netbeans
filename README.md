# UML2Netbeans

This Programm converts UML-Classdiagrams into a Neatbeans Project using only an image of the class. 
Warning! I made this Converter based on the Class Diagrams we get from our School (BBB IT-School) which are made using Visual Paradigm. Using your own or other Class Diagrams is experimental and may not work.

This Converter is very basic.

How To Use:
First make sure your screenshot / image of the class is in a good quality. 

1. Load an Image of a CLASS. Not the whole diagram. A single class. You can do this by clicking "Bild auswählen", or by clicking on "Screenshot".
2. Select a Netbeans Project. Important: Select the ROOT Folder of your Netbeans Project.
3. Choose a Package. You can either choose a package that is listed, or type the name of a new one and it will create a new package.
4. Choose the language.
5. Hit convert.

Now there should be a .java File in your package with all the Variables, Methods etc.


This Converter does NOT support:
- Abstract Classes
- Interfaces
- Inheritance between Classes
- Abstract methods

Everything else should work.
