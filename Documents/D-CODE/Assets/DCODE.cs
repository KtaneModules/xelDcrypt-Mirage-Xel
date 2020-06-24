using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using KModkit;
public class DCODE : MonoBehaviour {
 public KMBombModule module;
public KMSelectable[] digits;
public TextMesh[] texts;
public Color[] fontColours;
public KMBombInfo bomb;
public KMAudio sound;
int stageCounter = 1;
static int moduleidcounter = 1;
int moduleid;
bool moduleSolved;
    void Awake()
    {
        moduleid = moduleidcounter++;
        foreach (KMSelectable digit in digits) { 
            KMSelectable pressedDigit = digit;
            digit.OnInteract += delegate ()
            {
                pressDigit(pressedDigit); return false;
            };
        }
    }
    // Use this for initialization
    void Start () {
        foreach (TextMesh num in texts)
        {
            num.text = UnityEngine.Random.Range(0, 10).ToString();
        }
        Debug.LogFormat("D-CODE #{0}: The starting digits are {1}, {2}, and {3}.", moduleid, texts[0].text, texts[1].text, texts[2].text);
	}
	
	// Update is called once per frame
	void pressDigit (KMSelectable digit) {
        if (!moduleSolved) {
            digit.AddInteractionPunch(.5f);
            Debug.LogFormat("D-CODE #{0}: You pressed digit {1}  at {2}.", moduleid, Array.IndexOf(digits, digit), bomb.GetFormattedTime());
            if (Math.Floor(bomb.GetTime() % 60 % 10) != int.Parse(digit.GetComponentInChildren<TextMesh>().text))
            {
                Debug.LogFormat("D-CODE #{0}: That was incorrect.  Strike.", moduleid);
                module.HandleStrike();
            }
            else
            {
                Debug.LogFormat("D-CODE #{0}: That was correct.", moduleid);
                digit.Highlight.gameObject.SetActive(false);
                digit.GetComponentInChildren<TextMesh>().text = "";
                switch (stageCounter)
                {
                    case 3:
                        sound.PlaySoundAtTransform("SolveSound", transform);
                        Debug.LogFormat("D-CODE #{0}: Module solved.", moduleid);
                        module.HandlePass();
                        moduleSolved = true;
                        break;
                    default:
                        sound.PlaySoundAtTransform("StageSound", transform);
                        stageCounter++;
                        break;

                }
            }     
        }
	}
   
}