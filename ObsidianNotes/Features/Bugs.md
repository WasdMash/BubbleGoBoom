Save system is nearly done now
- Lowkey forget to just do the same with the chests as I've done for the rooms and enemies
	- Make another class to save them so that newtonsoft and Unity don't both throw a hissy fit, arguing over how the chest info should be saved
- Because I haven't told newtonsoft how to serialise my LootableItem, it currently appears as an empty list in my file![[Pasted image 20260124110639.png|right|100]]
	- Turns out that loading in my LootableItems in my loadState() function really annoys Unity because it only likes to make new ScriptableObjects in a certain manner. Little princess
		- ![[Pasted image 20260124110907.png]]