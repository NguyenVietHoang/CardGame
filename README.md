# CardGame
This game was create for the interview of NFQ Asia

Author: NGUYEN Viet Hoang


Reference: all game asset was downloaded from the internet.

#############################

DESIGN

All the game was designed base on MVC design pattern. This game doesn't need to get data from a scene to a scene, so i do not implement the Singleton pattern into the Controler, so all controler here is scene local Controler.

The main controler is RuleControl, which control the flow of the game. I design this game flow as turn base game. For each turn, there are 3 phases: Summon phase (will be ignore if the 2 monster are not killed on the plate), Battle phase and End phase. All the card will be controlled by the Rule Event : onCardClicked. This event will be changed base on the game phase, so i will never pay too much attention on the UI, all will be controlled here, inside the RuleControl.

The game card data was stocked at the folder Card Data. I used ScriptableObject for it, so it will be very easy to change the data without touching the code.

#############################


