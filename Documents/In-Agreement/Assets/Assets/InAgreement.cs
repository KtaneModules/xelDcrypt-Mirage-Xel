using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAgreement : MonoBehaviour
{
    public TextMesh display;
    public KMSelectable button;
    int textIndex = 0;
    bool breakCondition = false;
    List<string> texts = new List<string> { "Yes", "Okay", "That", "No" };
    // Use this for initialization
   void Awake()
    {
        button.OnInteract += delegate () { pressButton(); return false; };
    }
    void Start()
    {
            StartCoroutine(cycleText(display));
        
    }


    public IEnumerator cycleText(TextMesh text)
    {
        while (true)
        {

            text.text = texts[textIndex];

            ++textIndex;

            if (textIndex >= texts.Count)
                textIndex = 0;

            yield return new WaitForSeconds(1f);

            if (breakCondition) break;
        }
    }
    void pressButton()
    {
        breakCondition = false;
    }
}