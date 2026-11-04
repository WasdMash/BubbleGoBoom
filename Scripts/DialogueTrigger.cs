using System.Collections.Generic;
using UnityEngine;
 
[System.Serializable]
public class Dialogue
{
    [Tooltip("This is the name of the CSV file for this relevant dialogue")]
    [SerializeField] string dialogueFileName;
    //As said in Obsidian, each line is an array of [iconNumber, dialogueLine]
    [SerializeField] bool oneTimeTrigger;
    public List<string[]> dialogueLines = new List<string[]>();
    public string getFile() {return dialogueFileName;}
    public bool oneTime() {return oneTimeTrigger;} 
}
 
public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    
    //The player needs to enter a trigger to start the dialogue - use wisely
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            DialogueManager.Instance.StartDialogue(dialogue);
            if(dialogue.oneTime()) gameObject.GetComponent<Collider2D>().enabled = false;
        }
        
    }
}