using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magazine2Script : MonoBehaviour, IInteractable
{
    private static bool active = false;

    private void Start()
    {
        active = false;
    }

    public void Activate()
    {
        if (active == false)
        {
            ui.SetDialogueText(dialogue, true);

            gameObject.SetActive(false);
            active = true;
        }
    }
    public DialogueUI ui;
    [TextArea]
    public string dialogue;
}

