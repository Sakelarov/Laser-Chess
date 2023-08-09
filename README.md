## Laser Chess
This is a game developed in Unity using C# as a test challenge for 8 days. All the 3D models, UI sprites and Particle Effects are Free assets
downloaded from the Unity Asset Store. All of those are described in the Credits section of the game. Most of those assets were edited in Photoshop
to fit the requirements of the project. Below is the description of the game.

## [Click to play the game on itch.io in your browser or download executable for Mac or Windows](https://sakelarov.itch.io/laser-chess)

The game is played on an 8x8 grid.  
It is a 2 player game, with a human player and an AI player.  
Each player takes turns, starting with the human player.  
During a turn, a player may move any or all of his pieces.  
The AI pieces must follow certain behaviour rules which normally require them to move.  
### The pieces
Each piece exclusively occupies a single space on the grid.  
Pieces may not move through other pieces, unless it uses the knight move as in Chess.  
Pieces block shooting.  
Each piece has hit points ­ the amount of damage it can take before it is destroyed and removed from the grid.  
Each piece has an attack power, which is the amount of hitpoints taken from a target enemy piece in an attack.  
Each piece can move and then attack.  
### The Human pieces

| Human Pieces  | Health Points | Attack Points |
| :------------:|:-------------:|:-------------:|
| Grunt         | 2             | 1             |
| Jumpship      | 2             | 2             |
| Tank          | 4             | 2             |

### Grunt
* Moves 1 space orthogonally
* Shoots once, diagonally at any range.

### Jumpship
* Moves like the knight in Chess
* Attacks all enemy pieces in the 4 orthogonally adjacent spaces simultaneously.

### Tank
* Moves like the Queen in chess, up to a maximum of 3 spaces.
* Shoots once, orthogonally at any range.

### The AI pieces

| AI Pieces     | Health Points | Attack Points |
| :------------:|:-------------:|:-------------:|
| Drone         | 2             | 1             |
| Dreadnought   | 5             | 2             |
| Command Unit  | 5             | 0             |

### Drone
* Moves forward 1 space from its side of the board (like a pawn, but never moves diagonally).
* Shoots once, diagonally at any range.
* Behaviour: Drones move before any other AI piece. They must all move and attack if possible. They must shoot at a target if possible after attempting to move
### Dreadnought
* Moves 1 space in any direction:
* Attacks all adjacent enemy units;
* Behaviour: Dreadnoughts move after all drones have moved. It must move 1 space, if possible, and must move towards the nearest enemy unit. It must try to attack after moving.

If the path to the nearest enemy is blocked by other enemy pieces then the Dreadnought (D) is using a simple implementation of A Star PAthfinder to get the closest path to the target. 

<img src="https://github.com/Sakelarov/Laser-Chess/blob/main/Assets/ReadMeImages/dreadnoughtMovement.png" width="400" height="400" />

In the case where more than 1 player units can be reached by the Dreadnought he will always chose to go to the square reaching the most player units applying maximum damage.

<img src="https://github.com/Sakelarov/Laser-Chess/blob/main/Assets/ReadMeImages/dreadnoughtAttack.png" width="400" height="400" /><img src="https://github.com/Sakelarov/Laser-Chess/blob/main/Assets/ReadMeImages/dreadnoughtMove3.png" width="400" height="400" />


### Command Unit
* The Command Unit must move after Dreadnoughts have moved.
* It can only move 1space in two possible directions ­ parallel to the AIs side of the board (i.e. it stays the same distance from the enemy side of the board).
* It cannot shoot or attack.
* Behaviour: It must avoid getting hit, if possible, so it must make the best move out of the three options available (move one way, move the other way, or stay still).

Command Units detect all player units that are able to attack them one the next turn taking into account their movement as well.  

If you take the Jumpship for example. In the case where the Jumpship (JS) is close to the Command Unit (CU) - the CU is trying to block the movement position of
the JS because in that way the JS will be rendered harmless this turn in relation to the Command Unit. If the Jumpship is at a distance more than 3 Manhattan distance then he will be ignored by the Command Unit. CU will take into account the JS only if he is or will be able to attack him in his next turn. Otherwise CU will ignore JS  

<img src="https://github.com/Sakelarov/Laser-Chess/blob/main/Assets/ReadMeImages/CommandUnitMovement1.png" width="400" height="400" />

Command Unit (CU) versus the Tank (T). The Tank has a very big reach and often it is impossible for CU to avoid incomming attacks. If it is possible to avoid it then CU will do its best to get away. However if there is no way of escaping the Tanks next movement + attack then the CU will ignore the Tank.  

<img src="https://github.com/Sakelarov/Laser-Chess/blob/main/Assets/ReadMeImages/CommandUnitMovement3.png" width="400" height="400" /><img src="https://github.com/Sakelarov/Laser-Chess/blob/main/Assets/ReadMeImages/CommandUnitMovement4.png" width="400" height="400" />

Finally the interaction of the CU with the Grunt (GR). CU can escape the Grunt by avoiding his attacking diagonals. If the CU is unable to avoid the attacking diagonals of the Grunt then he will ignore this player unit.

<img src="https://github.com/Sakelarov/Laser-Chess/blob/main/Assets/ReadMeImages/commandUnitMovement5.png" width="400" height="400" /><img src="https://github.com/Sakelarov/Laser-Chess/blob/main/Assets/ReadMeImages/commandUnitMovement6.png" width="400" height="400" />

After evaluating all the close player pieces to the CU the script weights all incoming dangers as attacks from Jumpship and Tank are weighten more than attacks from Grunt due to the damage difference dealt by those pieces.

### User Interface

When clicking on a cell containing a player unit this unit will be selected and the possible move locations will be displayed:  

<img src="https://github.com/Sakelarov/Laser-Chess/blob/main/Assets/ReadMeImages/playerMovement.png" width="400" height="200" /><img src="https://github.com/Sakelarov/Laser-Chess/blob/main/Assets/ReadMeImages/playerMovement2.png" width="400" height="200" />

Consecutive click on the same player will switch between attacking and moving mode as long as the player can move and can attack in that turn.

<img src="https://github.com/Sakelarov/Laser-Chess/blob/main/Assets/ReadMeImages/playerMovement2.png" width="400" height="200" /><img src="https://github.com/Sakelarov/Laser-Chess/blob/main/Assets/ReadMeImages/playerAttack.png" width="400" height="200" />

The top left GUI will also display the currently selected Character with information regarding his current health, attack points, position on the board and currenlty available actions

<img align="left" src="https://github.com/Sakelarov/Laser-Chess/blob/main/Assets/ReadMeImages/playerUnitsUI.png" width="231" height="280" />   

The green Move and Attack buttons also give the player the options to switch between move and attack actions when they are both available.  

There are little circular icons on the smaller portraits of the player units - those also indicateif the unit can move and attack at that specific moment.  

This means that if a unit hasnt attacked yet during his turn but has no available enemy targets then his attack indicator will be deactivated.  

However later during the same turn his line of sight towards an enemy unit could be opened and in that moment the indication will light up showing that this unit can attack. 

The same applied to the movement indicator.   

<br clear="left"/>  

<img align="right" src="https://github.com/Sakelarov/Laser-Chess/blob/main/Assets/ReadMeImages/enemyUnitsUI.png" width="231" height="280" />     
The case is similar regarding the enemy UI. However the indicator for moving and attacking here only show if the current unit has moved/attacked during this turn.   

They do not indicate if the enemy unit can / can't move / attack.  

All elements of the UI are updated in realtime - positions, remaining health of the units and portraits disappear for killed units.  

<br clear="right"/>  




### Victory determination
The human player wins if all the Command Units are destroyed.
The AI player wins if all human units are killed, or one of the drones reaches the 8th row.  
### Game set up
Human units are deployed on the first 2 rows from the human player’s side of the board.
AI units are deployed with the first 4 rows from the AI side of the board.
### Game levels
There are 3 basic levels of increasing difficulty which can be found in the main menu.   
Additionally there are custom levels and a Level Generator option.    
The composition of forces and starting positions of all pieces is predefined for the Easy, Medium, Hard and the Custom levels.  
When using the Level Generator keep in mind that you need to have at least one Command Unit and at least one Player Unit.  
The player pieces are deplayed in the first two rows from the player's side of the board therefore there is a limitation of maximum 16 pieces for Player Units.  
The enemy pieces are deployed in the first four rows from the AI's side of the board therefore the limitation for AI pieces is 32.   
Although possible it is not recommended to use more than 16 AI pieces because the UI gets crowded and the user experience may be worse.    

### Game Improvement Ideas

All aspects of the game can be improved.  
* A tutorial is required so that players understand how to move and attack each of their own pieces.
  Need to inform the player of the victory and lose conditions and possibly the attack and movement of the enemy units.
* AI movement calculations can be improved by thinking one move ahead. For example the Command Unit can calculate the possible next locations of the player pieces in his vicinity
  and from those pieces to calculate next movements. This will increase the data the units evaluates to decide in which direction to move.  
  Dreadnoughts are trying to get to the closest enemy and also try to get to as many enemies as possible. The improvement here could be that the script could know the borders of
  the game board and use that to his advantage to persue some pieces like Jumpship or Grunt (who cant easily run away from him) and force them to get trapped in the edges.
  
* Not all units have proper animations for all states required by the game i.e. attacking, receiving damage, movement, etc. The current animations need to be improved as well.
* Sounds need to be added for all kind of interactions of the units. This will make the game more immersive.
* UI can be improved to be more responsive and informative. For example hovering over a unit can show its possible move/attack locations. Selecting an enemy unit should highlight
  the enemy on the board so that it is easier for the player to see the exact location of the piece.

* Adding more viariation of player and enemy units will make the game more challenging and will enable the developers to create more content for the players.
* There could be some kind of Story driven and difficulty increasing progression in the game. Along the way some resources can be won after each battle and the player units could be upgradable.
 
