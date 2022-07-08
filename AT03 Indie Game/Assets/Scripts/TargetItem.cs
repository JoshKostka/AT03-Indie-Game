using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetItem : MonoBehaviour, IInteractable
{
    public GameObject findMagazineText;
    public GameObject runText;

    public delegate void ObjectiveDelegate();

    private static bool active = false;

    public static ObjectiveDelegate ObjectiveActivatedEvent = delegate { };


    // Start is called before the first frame update
    void Start()
    {
        active = false;
        runText.SetActive(false);
    }

    private void Awake()
    {
        ObjectiveActivatedEvent += ResetObjectiveEvent;
        PlayerController.CaughtPlayerEvent += ResetObjectiveEvent;
        active = false;
    }

    public void Activate()
    {
        if (active == false)
        {
            ui.SetDialogueText(dialogue, true);

            findMagazineText.SetActive(false);
            runText.SetActive(true);

            gameObject.SetActive(false);
            active = true;
            ObjectiveActivatedEvent.Invoke();
        }
    }

    public static void ResetObjectiveEvent()
    {
        ObjectiveActivatedEvent = delegate { };
    }

    public DialogueUI ui;
    [TextArea]
    public string dialogue;



    


}


