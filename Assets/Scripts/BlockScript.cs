using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockScript : MonoBehaviour
{
    public GameObject textObject;
    Text textComponent;
    public int hitsToDestroy;
    public int points;
    void Start()
    {
        if (textObject != null)
        {
            textComponent = textObject.GetComponent<Text>();
            textComponent.text = hitsToDestroy.ToString();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        {
            hitsToDestroy--;
            if (hitsToDestroy == 0)
                Destroy(gameObject);
            else if (textComponent != null)
                textComponent.text = hitsToDestroy.ToString();
        }
    }
}