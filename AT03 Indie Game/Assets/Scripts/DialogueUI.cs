using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialoguePanel;
    public Text dialogueText;
    public float readTime = 3;

    private float timer = -1;

    // Start is called before the first frame update
    void Start()
    {
        SetDialogueText(null, false);   //will turn off the idialogue panel when the game starts
    }

    private void Update()
    {
        if (timer> -1)
        {
            timer += Time.deltaTime;
            if(timer >= readTime)
            {
                timer = -1;
                SetDialogueText(null, false);
            }
        }
    }

    public void SetDialogueText(string message, bool toggle)
    {
        if(toggle == true)
        {
            dialogueText.text = message;
            timer = 0;
        }
        dialoguePanel.SetActive(toggle);

    }
}
