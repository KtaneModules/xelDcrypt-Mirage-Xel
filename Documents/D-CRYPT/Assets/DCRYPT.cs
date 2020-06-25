using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KModkit;
using System.Linq;


public class DCRYPT : MonoBehaviour {
    public KMSelectable[] letters;
    public TextMesh[] texts;
    public Color locked;
    public KMBombInfo bomb;
    public KMBombModule module;
    public KMAudio sound;
    string[] wordList = { "ABOUT", "ALIEN", "BOMBS", "BRAIN", "CLAMP", "CROWN", "DEATH", "DEMON", "ETHER", "EVERY", "FLASH", "FORCE", "GIANT", "GHOST", "HEART", "HUMOR", "IMAGE", "INDIA", "JELLY", "JOKES", "KANJI", "KAZOO", "LEMON", "LOVER", "MAKER", "MEDIA", "NIGHT", "NADIR", "OASIS", "OCHRE", "PENNY", "PSALM", "QUEEN", "QUOTA", "RAZOR", "ROOST", "SALVE", "SIGMA", "TASTE", "TROLL", "UMBRA", "UNITY", "VIRUS", "VOMIT", "WHITE", "WOMAN", "XEROX", "XRAYS", "ZEBRA", "ZOOMS" };
    string[] alphabet = new string[26]
       { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
      "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
    string startingWord;
    string answerWord;
    static int moduleIDCounter = 1;
    int moduleID;
    void Awake()
    {
        moduleID = moduleIDCounter++;
        foreach (KMSelectable letter in letters)
        {
            KMSelectable pressedWord = letter;
            letter.OnInteract += delegate () { pressLetter(pressedWord); return false; };
        }
    }
    // Use this for initialization
    void Start () {
        startingWord = wordList[UnityEngine.Random.Range(0, 53)];
        answerWord = wordList[Array.IndexOf(wordList, startingWord) + bomb.GetBatteryCount() + bomb.GetSerialNumberNumbers().Last() % 52];
        if (startingWord == answerWord)
        {
            answerWord = wordList[Array.IndexOf(wordList, startingWord) + 7];
        }
        Debug.LogFormat("D-CRYPT #{0}: The starting word is {1}. ", moduleID, startingWord);
        Debug.LogFormat("D-CRYPT #{0}: The answer word is {1}. ", moduleID, answerWord);
        int textCounter = 0;
        foreach (TextMesh text in texts) {
            text.text = startingWord.ToCharArray()[textCounter].ToString();
            textCounter++;
        }
    }
	
	// Update is called once per frame
	void pressLetter (KMSelectable letter) {
        if (letter.GetComponentInChildren<TextMesh>().color != locked)
        {
            StartCoroutine(cycleLetters(letter.GetComponentInChildren<TextMesh>()));
        }
	}
    IEnumerator cycleLetters(TextMesh letter)
    {
        while (true)
        {
            letter.text = (alphabet[Array.IndexOf(alphabet,letter.text) + 1 % 25]);
            yield return new WaitForSeconds(2f);
        }
    }
}
