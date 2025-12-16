2. Implement a Save/Load Manager 

You need a central system responsible for managing the data persistence. This manager will interact with your game's file system or database. 

- **Serialization:** Convert your `ChestData` objects into a format that can be stored in a file, such as JSON, XML, or a binary format . JSON is often recommended for its readability.
- **Data Storage:** The manager saves a complete list of all `ChestData` objects to a single save file (e.g., `chests.sav`) . 

3. Integrate with Game Events

Hook up the save manager to key moments in your game's lifecycle:

- **Loading:** When the game world loads (or the player enters a specific area), the manager loads all saved `ChestData` from the file. It iterates through all active chests in the scene, finds the corresponding saved data using the `ChestID`, and populates the chest's in-game inventory .
- **Saving:** The game should save the state of all chests whenever a significant event occurs, such as:
    - The player closes the chest UI .
    - The player reaches a checkpoint or saves their game manually .
    - The player quits the game . 

4. Handle Item Manipulation In-Game

Whenever a player adds, removes, or moves items within a chest, the in-game data model for that specific chest must be updated immediately . It is only necessary to _save_ this updated data to permanent storage at the events mentioned in Step 3.