using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI; // using TMPro;

public class BlockScript : MonoBehaviour
{
    PlayerScript playerScript;

    public GameObject textObject;
    Text textComponent;
    public int hitsToDestroy;
    public int points;

    virtual public void Start()
    {
        if (textObject != null)
        {
            textComponent = textObject.GetComponent<Text>();
            textComponent.text = hitsToDestroy.ToString();
        }
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();

    }
    virtual public void OnCollisionEnter2D(Collision2D collision)
    {
        {
            if (collision.gameObject.tag == "Ball")
            {
                hitsToDestroy -= playerScript.force;
                if (hitsToDestroy <= 0)
                {
                    Destroy(gameObject);
                    playerScript.BlockDestroyed(points, this.gameObject.name, this.gameObject.transform.position);
                }
                else if (textComponent != null)
                    textComponent.text = hitsToDestroy.ToString();
            }
        }
    }
}