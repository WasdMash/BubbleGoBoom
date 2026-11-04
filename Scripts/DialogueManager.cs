using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    [Tooltip("I'll need to store all .pngs of the heads of chatting characters in the game")]
    [SerializeField] Sprite[] characterHeads;
    [SerializeField] CSVReader cSVReader;
    public static DialogueManager Instance;
    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;
    private Queue<string[]> lines;
    public bool isDialogueActive = false;
    [Range(5f, 100f)]
    [SerializeField] float typingSpeed = 0.2f;
    public Animator animator;
    [SerializeField] GameObject dialogueUI;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        animator.updateMode = AnimatorUpdateMode.UnscaledTime; //I'mma need my animations to still be working when time is frozen
        if (!isDialogueActive) dialogueUI.SetActive(false);
        lines = new Queue<string[]>();
        cSVReader = GetComponent<CSVReader>();
    }

    public void ShowUI() { 
        dialogueUI.SetActive(true);
        animator.Play("Base Layer.show", 0, 0f);}
    public void HideUI() => animator.Play("Base Layer.hide", 0, 0f);


    //This function basically takes an unformatted name of a sprite image and removes extra characters to return a printable name
    string NameFormat(string spriteName)
    {
        //search for any camelCasing - if found, add a whitespace
        //search for _ - if found, convert into a whitespace
        string newName = "";
        for(int i=0;i<spriteName.Length;i++)
        {
            if(i < spriteName.Length - 1)
            {   //Using Unicode character mapping to spot upperCase characters / camelCasing and add the whitespace then
                if(char.IsUpper(spriteName[i+1])) newName += spriteName[i] + " ";
                else newName += spriteName[i];
            }
            else newName += spriteName[i];
            if(spriteName[i] == '_') newName += " ";
        }
        return newName;
    }
 
    public void StartDialogue(Dialogue dialogue)
    {
        //Should read the contents of the .csv file now, yeah?
        List<List<string>> dialogueLines = cSVReader.ParseCSV(dialogue.getFile());
        ShowUI();
        InputManager.Instance.MenuTime();
        Time.timeScale = 0f; //It's time to freeze all enemies - you get a break
        lines.Clear(); //In case there are any leftover lines from the previous conversation
 
        foreach (List<string> dialogueLine in dialogueLines) lines.Enqueue(dialogueLine.ToArray());
        Debug.Log(lines);
        DisplayNextDialogueLine();
    }

    //Overloading the function to use input context, so that menu button triggers the next line of dialogue
    public void DisplayNextDialogueLine(InputAction.CallbackContext context) => DisplayNextDialogueLine();
    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0){
            EndDialogue();
            return;
        }
 
        string[] currentLine = lines.Dequeue();
 
        //Here, should have stored the appropriate character sprites in an array above to fetch from now
        Sprite currentCharacter = characterHeads[int.Parse(currentLine[0])];
        characterIcon.sprite = currentCharacter;
        characterName.text = NameFormat(currentCharacter.name); //will need to format the name of the sprite before assigning to UI
 
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }
 
    IEnumerator TypeSentence(string[] dialogueLine)
    {
        dialogueArea.text = "";
        foreach (char letter in dialogueLine[1].ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSecondsRealtime(1/typingSpeed);
        }
    }
 
    void EndDialogue()
    {
        HideUI();
        InputManager.Instance.GameTime();
        Time.timeScale = 1f; //undoes the time freezing - don't want enemies moving when you can't
    }
}