If the current system of using multiple dialogue classes to store sentences is long, **which for writing a whole story into thousands of digested paragraphs with accompanying images can get long**, I can use the following .CSV file format setup to make my life so much easier:

***To allow for commas and the such in dialogue spoken, I cannot use a comma as my delimiter, so I will settle to use the vertical bar | instead for more safety.***

<h2>First line </h2>
* Store the names of the head sprites of all characters that I know are going to be talking in this particular dialogue - *they should all be saved as PNG sprites, so I can append /slice the .png ending if needed*
	* E.g ['Tom', 'KingOlay'] can represent the player head and King Olay head sprites accordingly - this would mean that I would only expect these two heads to be speaking
	* To keep the same functionality as before, I may have to do some text processing after fetching the *sprite to ensure that sprite name == displayed character name in UI*
		* Such as spacing out the camelCase in KingOlay and replacing  *_* in  `Pew_pew_pew_slime` with whitespaces when printing their name

This would be helpful to fetch the appropriate images to memory and **store them in an ordered list (preferably alphabetical)** so that I don't need to manually save their images to each line of their dialogue.

<h2> Every other line </h2>
* For the first line to be optimal, every other line would need to be ordered as following:
	* `characterHeadSprite` , `their dialogue line/paragraph`
		* Where characterHeadSprite is the index in the array of images loaded that ***corresponds to the face of the character currently talkiing***
		* Obviously, the rest is what they actually say
			* If I think that their current line is too long, I can split this line and add another line below with the same index and the rest of the sentence

<h2>Example between Tom (0) and King Olay (1)</h2>
* 1|Man, a new room! How sick is this?
* 0|You're not ready for us, big man!
* 1|Yeah, I am. Bring it!
* 1|On second thought, maybe I'm not. Oh well, too late!