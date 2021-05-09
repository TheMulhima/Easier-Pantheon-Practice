# Easier Pantheon Practice

## Description:
A mod that makes practicing in HoG easier. Its main features are: allows you to press a key to reload bosses much quicker, matches the health and damage of bosses with P5, gives the ability to do fury practice in HoG. There are more but you can read the readme for that ¬‿¬.

## How to use
- Download this mod, the Modding API and SFCore from the modinstaller.
- To change the settings, pause the game in Hall of Gods and click on the buttons to increment the setting / bind a key. There isn't a way to decrement the counter so if you want to go back, just click on it until it turns to 0 (which will happen after it crosses the max value)
- To practice a Pantheon 5 boss, fight ascended. To Practice a Pantheon 1-4 boss, fight attuned.  
- If a key isn't registered using the GUI, go to the saves folder and add an [acceptable input](https://drive.google.com/file/d/1aebQ9DMngjk3ZO6x7XHk89D5I9q5armr/view?usp=sharing) in the respective keys to add a key bind. e.g.  `Key_teleport_around_HoG = "z"`.

## Features Explanation
- remove_health -> removed the specified value of health each time a boss scene in HoG is loaded. Useful for learning fury strats. 
  -  Fury can be setup on each boss load by setting remove_health to your max health - 1 and lifeblood to 4-5 (optional).
- lifeblood -> add the specified amount of lifeblood each time a boss scene in HoG is loaded.
- soul -> add the specified amount of soul each time a boss scene in HoG is loaded. The GUI will only allow adding 33 soul each time. if you want less (because you are using soul twister) you can do that from the settings file.
- hitless_practice -> makes you die in 1 hit to bosses. The advantage of using this over radiant fights is 1) bosses have same health as p5 2) ability to usse other features of the mod like adding soul or quickly reloading bosses.
### Each click on the button will increment the setting. so to get from remove health:0 to remove health:8, click on the button 8 times
- Key_Reload_Boss -> Press this key during a boss fight to quickly reload the boss. Probs should be done just as the boss dies. Note that after pressing the reload key and killing the boss, you won't be able to return normally. you will need to press the "Key_return_to_hog" or dreamgate.
  - Just for reference, using this takes 2 seconds to load a boss while doing it normally takes around 15 seconds. 
- Key_return_to_hog -> Equivalent to dream gating but much faster and doesn't burn your eyes by flashing a white screen
- Key_teleport_around_HoG -> Teleports you aound HoG. It alternates between the bench and gpz statue.
### If you click on the button on the GUI that corresponds to keys, it will give you the option to bind them. just click the button and then press the key you want to bind to.

 ## Common use cases:
- Practice fighting in pantheons with **Fury of the Fallen**.
- The hotkeys (espically the reload boss) can be used to lower waiting time between boss fights. (read below)
- It can be used to practice specific bosses in P5 such as **Collector**. (also compatible with bindings mod for AB practice)
- The hitless option allows practice for hitless Pantheons.

## Other:
- The mod only works in Hall of Gods and doesn't affect pantheons or other areas of the game.
- The mod affects bosses of health. Needless to say, it shouldn't be used with other modded boss. If you do want to, turn this mod off in the game settings.
- To unbind any key, press the button on the GUI and press esc to close the pause menu.
- If you find any bugs or have any suggestions, you can message me on discord at Mulhima#2695.

## Dependancies:
- This mod depends on Modding API (Obviously) and SFCore.

## Credits
- Thank you to everyone in #modding-development for helping me make this mod and also for having open source code for me to read ~~and copy~~.
