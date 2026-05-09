# Green city builder 

This repository contains the project 'Green city builder', an educational city building simulator game that features environmental elements. It was developed as part of the Comp 3000 project module 
# Overview 
## Game Overview 
The game is 2d topdown game with a pixel art style that was made as a part of the city simulation genre. It features city building aspects, dynamic NPCs, public transport, dynamic world generation, multiple game modes, trading and more. <br/> 
What sets it apart though is the environmental focus of the game, where the city is affected by differant aspects of the environment. For example building various buildings that produce air pollution while removing lots of greenery will cause the worlds air pollution to increase, this can lead to increased chances of NPCs getting sick which reduces NPC happines and may result in NPCs deciding to leave the city. <br/>
The player is given information as they play on how their actions are effecting the city, educating them on the real ways city building can effect the environment. This was created with education in mind aiming to educate the people who play the game about the environment. For more details please see the game guide later in this read me
## Repository overview 
This repository contains various items from the project. <br/>
A key one of these is the source code for the project which can be seen in the code folder.  Here you can find all the files that were used in the production of the game. This included unity file data, c# code/visual studio files, the games various sprites and animations and all other data that was required for the games development.<br/>
In addition there are project managment and research folders. The project management folder stores snapshots from tracking sprints aswell as meeting notes allowing for the planning history of the project to be maintained. The research folder contains pdf versions of any research done throughout the project. <br/> 
Finnally there is a build folder which contains an exe version of the game not reliant on the production environment to run
# Running the code/GameGuide 
This section details a full guide to playing the game. This can also be found in the project report.
## How to play/run
To run this game, and play for yourself, please open the build folder and run GreenCityBuilder.exe. This will run the game with all its functionality without reliance on the development environment 
##  Main menu and Save files
When the player opens the game, the first thing they will see is the main menu, this is a simple screen with the games logo and the start button. Clicking this will load a screen with more options, one of these being the save menu . Clicking this will load save files, which presents the user with all the save files currently made or the option to make a new one.
![Main menu screen](MDImages/F1.png)
#### Main menu screen<br/>
![Hub Screen](MDImages/F2.png)
#### Hub screen<br/>
![Save selection](MDImages/F3.png)
#### Save selection screen<br/><br/>
In addition, the credits and tutorial can be seen from here
![Credits screen](MDImages/F4.png)
#### Credits screen<br/>
![Tutorial screen](MDImages/F5.png)
#### Tutorial screen<br/>

### Creating save file 
When a player clicks the create new save button on one of the save slots, a pop up will appear which presents them with various options for creating a save file. <br/>
Here the player can choose the game mode (for details on game modes please see Game guide section 4),map size and the save file name. 
When the player has set the name and mode they want they can click the create button generating a new game map (for more information on this please see section 5 of world generation). If the player has not added anything to the save file name field the save file will prompt the player to add a name and not allow the save file to be created until they do
![New game](MDImages/F6.png)
#### New save file screen<br/>
![New game](MDImages/F7.png)
#### New save file input prompt<br/>

### Loading and deleting save file
On the main save screen, the player has the option to load a save file. When a save slot is occupied and the player clicks the save slot, the game will be loaded using the data from that save slot to generate the city in line with the save file. For more details on this please see section 5.2 of the player guide
In addition the player can delete existing files. Clicking the clear files button will remove the save files allowing players to start fresh when they are done with a save file 
## Game UI  
### Main UI 
The main UI seen in the game has various sections for various functionalities. <br/>
The starting UI that is seen most commonly has selections for tile editing, building editing, transport editing and displays general information about the game at the top of the screen. <br/>
The Information header at the top of the screen displays how much money the city is generating/losing, how much power the city is generating /losing, the amount of money and power the player has currently and the number of NPCs currently in the city. 
![Main game UI](MDImages/F8.png)
#### Main game UI<br/>
![Info UI](MDImages/F9.png)
#### Info header<br/><br/>
Next to the information header is the section that displays the current city rating, this is a 0-100 scale where 0 is terrible and 100 is the best possible city that is essentially flawless. When this is clicked, more information is displayed to the player on how they can improve their city rating 
![Rating UI](MDImages/F10.png)
#### Rating header<br/><br/>
![Rating Info](MDImages/F11.png)
#### rating info<br/><br/>
Finally there are the 3 buttons for editing and interacting with the world. 
![Core UI](MDImages/F12.png)
#### Core UI buttons<br/><br/>
### Building editing 
The first button from the core UI will open the building menu, which allows for the player to select, place and remove buildings. 
![Core building UI](MDImages/F13.png)
#### Core building UI buttons<br/><br/>

### Transport placement editing 
By opening the transport UI, the player can edit bus stops, train stations and rail placement depending on the UI open. This is where the player can set up routes for public transport 
![rail building UI](MDImages/F14.png)
#### rail building UI buttons<br/><br/>
![bus building UI](MDImages/F15.png)
#### bus building UI buttons<br/><br/>
### Tile editing 
When clicking the layout button the tile editing UI will open with options for editing tiles. For more information on this functionality please see later in this guide
![Layout editor UI](MDImages/F16.png)
#### layout editor UI <br/><br/>

### Transport Routes UI
Selecting the set routes button for either rail or bus transport UI will open the route setting UI. From here you can click the display routes button which can be used to view existing routes and cancel routes 
![Route creation UI](MDImages/F17.png)
#### route creation UI <br/><br/>

![Route viewing UI](MDImages/F18.png)
#### route Viewing UI <br/><br/>
### Pause menu UI
Clicking the escape key opens the pause menu. This menu will allow the player to return to the main menu, save the game or open settings. When the settings button is clicked the settings UI is opened, here the player can adjust game volume and accessibility options
![settings UI](MDImages/F19.png)
#### settings UI <br/><br/>
![Pause UI](MDImages/F20.png)
#### Pause UI <br/><br/> 

## NPCs 
A key part of the game was NPCs which act as the city's citizen population. These citizens can partake in a wide variety of actions which are chosen dynamically based on various factors 
### NPC Decision making and stats 
Each NPC has various stats that they take into account when deciding on a new action. These stats are tiredness, sickness and boredom and they can all be affected by different buildings.<br/> 
When making a decision, the NPC can choose between the following:
- Wandering(Walk to a random position,increases tiredness boredom and sickness while walking )
- Go home (Going home reduces npc tiredness and sickness but increases boredom, chance of going home is increased when NPC tiredness is higher)
- Going to the shop
- Go to hospital(Being in hospital reduces NPC sickness and tiredness but increases boredom, chance of going to hospital is higher when NPC has high levels of sickness)
- Partake in entertainment (Being in entertainment reduces boredom but increases sickness and tiredness, chance of going to entertainment building increases with NPC boredom)<br/>
In addition, if an NPC is quite unhappy, they may choose to leave the city. Likewise if your city is doing well, new NPCs may join your city

### NPC Movement
When NPCs select a position they want to move to, they will calculate if a route is possible and find the best route. This is done via a breadth first search, where valid tiles(grass,road and greenery tiles) are progressively checked until the target is found.  If the target is successfully found the NPC will have that position set as their movement target and will save the route to begin walking that way. <br.>
When routing, NPCs will see public transport as fast options, no matter the distance travelled by a train/bus, NPCs will see this as one square of movement due to its increased speed 

### NPC buildings interaction
Certain NPC actions involve them moving into buildings (for example going home, going shopping, doing something entertaining or being treated at a hospital). <br/> 
When an NPC selects this action, they will attempt to find a route to the nearest building of the specified type and if the route is possible they will set the building as their target. When the NPC arrives in the building they will go inside (assuming the player hasn't removed it). While inside, certain stats of the NPC will improve (e.g entertainment reduces boredom, hospital reduces sickness etc). When they enter the building a random value based on the building type will be selected, and this will determine how long they remain in the building. When the counter runs out, the NPC will leave the building and select a new action.<br/> 
While an NPC is in a building, that building cannot be removed
### NPC homelessness 
A key aspect of NPCs is that you must provide enough homes for them. If you have more NPCs than available housing then some NPCs will not be assigned a home and marked as homeless. These NPCs will be much less happy than NPCs with a home and having too many homeless NPCs will affect your city rating meaning it is important to watch the available housing carefully.
## Game modes
To allow for players with different playstyles to enjoy the game, multiple game modes were included
### Sandbox 
Sandbox mode is the mode for players not interested in keeping track of money and power. In this mode they can build to their hearts content while never running low on money or power
### Standard 
Standard mode is for the players who want to play the game like a real functioning city. In this mode they have to keep track of their use of money and power. If money runs out, nothing can be built, if power runs out, buildings will stop functioning, decreasing NPC happiness and causing them to start leaving at an increased rate
## World generation 
Various features appear in the world the city is set in
###  Map features on world creation
When a new map is created,a random map will be created. The map size can be set by the player on the main menu and will adjust the features of the map created. The key map features that are created are the following. 
Game Guide 5.1.1 River generation
The key thing generated onto the map is rivers, these use perlin noise to create simulated hills for the water to flow down to have semi believable rivers 

Game guide figure 19: water world generation
Game Guide 5.2.2 Greenery generation 
Another key feature generated is the greenery placed around the map. This is something that must be removed carefully when terraforming as removing too many will reduce the environmental rating. These are placed in random spots in various patterns 

Game guide Figure 20: greenery example

Game guide 5.2 Loading from save
When a player loads a save file, the details of that save file are retrieved and used to generate the game world. When the world generates, it will match exactly how it was in the save file allowing players to save their city and come back later
Game guide 6 World editing 
To allow the player to build and design their city, various options for editing the city and the world around it are present
Game guide 6.1 Building editing 
By opening the buildings UI and selecting a building, the player can click a position on the map and place a building. In standard mode they will only be able to do that if they have enough money to afford this and no matter the game mode, buildings can only be placed if the target position is not occupied by another building or invalid tile. When placing a building squares will be highlighted to show where the placed building would go. 

Game guide figure 21 Building placing before 

Game guide figure 22 Building placing after
In addition the player can remove buildings by using the remove button on the same page, if a building is in use(e.g by an npc or a train station on a train route), it cannot be removed until it has concluded being used
Game guide 6.2 Tile editing 
Tiles around the map can also be edited and can be turned into grass, road, water, greenery or bus stops.  This can be edited by clicking the layout button then selecting a certain tile type. Not all tiles can be placed on each other, for example a river would have to be turned to grass before you can build a road on that tile. In addition bus stops can only be placed on road tiles and are actually separate and placed from the bus UI. In standard mode, placing a tile costs money
Game guide 7 Environmental features 
As it was a key point and focus of the game, environmental features are a key part of the game. The following are examples of this 
If too many buildings emit air pollution with not enough greenery to deal with it, air quality will decrease and NPCs will have higher chances of getting sick
Building too many buildings with high water pollution will pollute rivers, reducing NPC happiness
Buildings create wastage, without building proper wastage facilities this will not be handled and will cause pollution in turn reducing the city rating 
Various power plant types exist, building the more environmentally friendly options reduce environmental impact and increase city rating 
Various wastage facility types exist, building the more environmentally friendly options reduce environmental impact and increase city rating 
The specific details of a city and its impact on the environment can be seen any time in the rating info box, this details exactly what the NPC can do at any time to improve the environment in their city
Game guide 8 City ratings
Based on all the different aspects of the city, a city rating is calculated and constantly displayed in the top left of the screen. This includes rating 
Whether there is enough hospitals
Whether there is enough shops
Whether there is enough roads
Whether there is enough power
Whether there is enough entertainment 
Whether most the buildings are close enough to a power plant to be powered
Whether there is enough public transport
Whether there is enough homes
And most significantly, the various environmental features in the city 
By following the advice given on the rating UI these ratings can be improved and the cities score can increase
Game guide 9 Transport 
The player can set up bus routes or train routes for their citizens to use. These allow for increased NPC movement around the city and ensure for better access to different points in the city 
Game guide 9.1 Train routes Creating 
When the player opens the train routes menu, they will be presented with buttons to set the starting train station and ending train station on the route. Clicking these buttons will display a UI allowing them to click train stations which in doing so will set that stop as the start/end bus stop for the route. When the player clicks confirm, if both route positions have been set, the positions are different from each other and the selections have a route to each other via train tracks, a new route will be confirmed and created. From here a train will run back and forth between this 2 stations

Game guide figure 23:Train route creation UI

Game guide figure 24:Train station selecting UI
Game guide 9.2 Bus routes creation
When the player opens the bus routes menu, they will be presented with buttons to set the starting bus stop and ending bus stop on the route. Clicking these buttons will display a UI allowing them to click bus stops which in doing so will set that stop as the start/end bus stop for the route. When the player clicks confirm, if both route positions have been set, the positions are different from each other and the selections have a route to each other, a new route will be confirmed and created. From here a bus will run back and forth between this 2 points 

Game guide figure 25: Bus route creation UI
Game guide 9.3 Route deletion
When a player decides they no longer want a certain route running, they have the option to shutdown the running of the route. 
This can be done by clicking transport, either bus or rail depending on the route type the player wants to remove, set route then finally see routes. This will open up a page where clicking a tile containing a route will show the full route and display its information on the UI. Here the player can click the cancel route button which will set the route to cancelled. 
When a route is marked as cancelled, the bus/train on that route will finish moving if it is currently in the process of moving. Then when all movement is complete, the route is fully removed, checking all NPCs and rerouting them/ selecting new actions for them if their current route included the removed transport route


# Asset/package usage credits 
Various assetts and packages, including sprites, animation, sound effects and more were used in the development of this game. Below you can find details on all assets/software used in the development game<br/> 
These can also be seen in the game and the project report
##  Packages/software credits
- Unity 6 (Game engine) -  https://unity.com/ <br/>
- Visual studio 2022 ( IDE) -  https://visualstudio.microsoft.com/downloads/ <br/>
- Github (GUI for git repository) -  https://github.com/ <br/>
- Piskel (Pixel asset development) - https://www.piskelapp.com/ <br/>
- Itch.io (Pre existing assets browser) -  https://itch.io/ <br/>
- Figma (Diagram maker)  - www.figma.com <br/>
- Google sheets (spreadsheets for project planning)-  https://docs.google.com/spreadsheets/ <br/>
- Google docs (research writeups) - https://docs.google.com/document <br/>
- Epidemic sound(music and sound effects searching)   https://www.epidemicsound.com/ <br/>
- Programming languages and packages <br/>
- C# (core programming language) - https://learn.microsoft.com/en-us/dotnet/csharp/  <br/>
- ShaderLab + CGPROGRAM (shader programming language for colour blind modes)- https://docs.unity3d.com/560/Documentation/Manual/SL-Shader.html  <br/>
- sqlite4Unity3d (Unity package for db operations) - https://github.com/robertohuertasm/SQLite4Unity3d  <br/>
## Sound effect credits 
- Ui Click: https://www.epidemicsound.com/saved/27887854/ <br/>
- Shop place : https://www.epidemicsound.com/sound-effects/tracks/6caad476-d3e4-44ad-afa1-1e9187c46bd0/ <br/>
- Building remove: https://www.epidemicsound.com/sound-effects/search?term=buildinf%20damage  <br/>
- Building place: https://www.epidemicsound.com/sound-effects/search?term=Hammer <br/>
- Bus running : https://www.epidemicsound.com/sound-effects/tracks/1d2a910c-11ae-4bb6-b7a1-72519ee4cdc5/ <br/>
- Train running: https://www.epidemicsound.com/sound-effects/tracks/fe9bbc35-9158-4743-bdc8-188082a917dd/ <br/>
- City AMbience: https://www.epidemicsound.com/sound-effects/tracks/8d637c99-8043-4cbe-a50c-ae2eac8cefaa/ <br/>
- Tile editing : https://www.epidemicsound.com/sound-effects/tracks/4dc122e1-f2c4-4547-a4a7-549008b5eaa3/  <br/>
##  Music credits
- Gameplay background Music : https://www.zapsplat.com/music/game-day-rhythmic-upbeat-electronic-sports-game-instrumental-with-piano-chords-and-synths/ <br/>
- Main menu music: https://www.epidemicsound.com/music/tracks/e8438738-fb07-479c-97c2-09d60eb04539/ <br/>
## Sprite credits 
### UI
- Money ICON: https://xflomasterx.itch.io/coins-free <br/>
- City background for main menu: https://free-game-assets.itch.io/free-city-backgrounds-pixel-art <br/>
- Buttons and most UI elements: https://crusenho.itch.io/complete-ui-essential-pack <br/>
- Game Font: https://www.1001fonts.com/arcadeclassic-font.html  <br/>
### Tiles
- Grass tiles: https://cardinalzebra.itch.io/grass-road-tiles <br/>
- Road tiles (edited to have pathway): https://kubigames.itch.io/road-tiles/download/eyJleHBpcmVzIjoxNzczMjY1NTE3LCJpZCI6MzkwMjY1fQ%3d%3d.WJny6AaDSPHNmF3wyioaGVCdeuk%3d  <br/>
- Water tile/animation (grass overlay was added by me):https://zrodfects.itch.io/16x16-water-tiles-animated-tileset-1-starter-pack <br/>
## Buildings
- Hospital sprite:https://dai420.itch.io/hospital <br/>
- Town hall sprite https://avkov.itch.io/city-tilemap-32x32 <br/>  
- Shop sprite,wastage center and recycling center(edited to fit style of game slightly) : https://avkov.itch.io/city-tilemap-32x32?download <br/>
## Assorted items/packages
- Plants: https://shubibubi.itch.io/nature-things <br/>
- Trees :https://karsiori.itch.io/free-pixel-art-tree-pack <br/>
- NPCs https://free-game-assets.itch.io/free-townspeople-cyberpunk-pixel-art <br/>

# Game guide 
