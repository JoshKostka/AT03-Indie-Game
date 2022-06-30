using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StunScript : MonoBehaviour
{
    public float distance;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") == true)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, distance) == true)
            {
                Debug.DrawRay(transform.position, transform.forward * distance, Color.red, 0.2f);
                if (hit.collider.TryGetComponent(out IInteractable interaction) == true)
                {
                    interaction.Activate();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.E) == true)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, distance) == true)
            {
                Debug.DrawRay(transform.position, transform.forward * distance, Color.red, 0.2f);
                if (hit.collider.TryGetComponent(out IInteractable interaction) == true)
                {
                    interaction.Activate();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
public interface IInteractable
{
    public void Activate();
}
public abstract class Interactable : MonoBehaviour, IInteractable
{
    public abstract void Activate();
}