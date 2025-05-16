# Test_D
I have not used ChatGPT or any AI tool to implement this assignment.

Used Unity Version: 2021.3.45f1

Open ‘Assets/Scenes/SampleScene.unity’ scene to test.

There are 6 managers I have implemented.
1) Image Manager:
	To handle loading of sprite runtime
2) Level Manager:
	To manage and handle game+UI flow
3) Save Manager:
	To manage save and load game level data
4) Score Manager:
	To manage score of game level
5) Sound Manager:
	To manage sound SFX
6) UI Manager:
To manage different UI

There are 3 UIs I have implemented.
1) Grid: 
	Card’s grid
2) HUD:
	Game HUD Elements(score, best score, home button, etc..)
3) Menu:
	Menu UI to load the particular level

I created ‘Assets/LevelDataXMLs/LevelData.xml’ to manage details of level and created its scriptable object ‘Assets/Resources/ScriptableObjects/LevelData/LevelData.asset’. 
Use the “LevelData->Check N Create Asset” menu option to generate a scriptable object after changing data in leveldata xml.
