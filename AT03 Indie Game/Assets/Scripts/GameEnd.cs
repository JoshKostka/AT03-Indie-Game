using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnd : MonoBehaviour
{
    private int sceneIndex4 = 3;
    private int SceneIndex1 = 1;

    [SerializeField] Enemy enemyRef;
    public DialogueUI ui;
    [TextArea]
    public string dialogue;

    private void OnTriggerEnter(Collider other)
    {
        if(enemyRef.ForceChaseTarget == true)
        {
            SceneManager.LoadScene(sceneIndex4);
            ui.SetDialogueText(dialogue, true);
        }
    }


}