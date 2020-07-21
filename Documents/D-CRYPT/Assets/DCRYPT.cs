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
    public Color initial;
    public KMBombInfo bomb;
    public KMBombModule module;
    public KMAudio sound;
    string[] wordList = new string[52] { "ABOUT", "ALIEN", "BOMBS", "BRAIN", "CLAMP", "CROWN", "DEATH", "DEMON", "ETHER", "EVERY", "FLASH", "FORCE", "GIANT", "GHOST", "HEART", "HUMOR", "IMAGE", "INDIA", "JELLY", "JOKES", "KANJI", "KAZOO", "LEMON", "LOVER", "MAKER", "MEDIA", "NIGHT", "NADIR", "OASIS", "OCHRE", "PENNY", "PSALM", "QUEEN", "QUOTA", "RAZOR", "ROOST", "SALVE", "SIGMA", "TASTE", "TROLL", "UMBRA", "UNITY", "VIRUS", "VOMIT", "WHITE", "WOMAN", "XEROX", "XRAYS", "YACHT", "YEAST", "ZEBRA", "ZOOMS" };
    string[] alphabet = new string[26]
       { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
      "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
    string startingWord;
    string answerWord;
    int stageCounter = 1;
    bool breakCondition = false;
    static int moduleIDCounter = 1;
    int moduleID;
    void Awake()
    {
        moduleID = moduleIDCounter++;
        foreach (KMSelectable letter in letters)
        {
            KMSelectable pressedLetter = letter;
            letter.OnInteract += delegate () { pressLetter(pressedLetter); return false; };
            letter.OnInteractEnded += delegate () { releaseLetter(pressedLetter); };
        }
    }
    // Use this for initialization
    void Start() {
        Debug.Log(wordList.Length);
        startingWord = wordList[UnityEngine.Random.Range(0, 52)];
        answerWord = wordList[(Array.IndexOf(wordList, startingWord) + bomb.GetBatteryCount() + bomb.GetSerialNumberNumbers().Last()) % 52];
        if (startingWord == answerWord)
        {
            answerWord = wordList[Array.IndexOf(wordList, startingWord) + 7];
        }
        Debug.LogFormat("D-CRYPT #{0}: The starting word is {1}. ", moduleID, startingWord);
        Debug.LogFormat("D-CRYPT #{0}: The answer word is {1}. ", moduleID, answerWord);
        int textCounter = 0;
        foreach (TextMesh text in texts) {
            text.color = initial;
            text.text = startingWord.ToCharArray()[textCounter].ToString();
            textCounter++;
        }
    }

    // Update is called once per frame
    void pressLetter(KMSelectable letter) {
        Debug.Log(letter.GetComponentInChildren<TextMesh>().color.g);
        if (letter.GetComponentInChildren<TextMesh>().color.g > 0.6)
        {
            StartCoroutine(cycleLetters(letter.GetComponentInChildren<TextMesh>()));
        }
    }
    void releaseLetter(KMSelectable letter)
    {
        if (letter.GetComponentInChildren<TextMesh>().color.g > 0.6)
        {
            letter.GetComponentInChildren<TextMesh>().color = locked;
            sound.PlaySoundAtTransform("StageSound", transform);
            StopCoroutine(cycleLetters(letter.GetComponentInChildren<TextMesh>()));
            breakCondition = true;

            stageCounter++;
            if (stageCounter == 6)
            {
                checkSolution();
            }
        }
    }
    void checkSolution()
    {
        int index = 0;

        string loggingWord = "";
        foreach (TextMesh text in texts)
        {
            loggingWord = loggingWord + text.text;
        }
        Debug.LogFormat("D-CRYPT #{0}: You submitted {1}. ", moduleID, loggingWord);
         foreach (TextMesh text in texts)
        {
            if (text.text != answerWord.ToCharArray()[index].ToString())
            {
                module.HandleStrike();
                stageCounter = 1;
                Start();
                foreach (TextMesh tex in texts)
                {
                    tex.color = initial;
                }
                index++;
                Debug.LogFormat("D-CRYPT #{0}: That was incorrect. Strike. ", moduleID);
                return;
            }
            module.HandlePass();
            sound.PlaySoundAtTransform("SolveSound", transform);
        }
    } 
        IEnumerator cycleLetters(TextMesh letter)
        {
            int textIndex = 0;
            List<string> alphabetShuffle = alphabet.ToList().Shuffle();
            breakCondition = false;
            while (true)
            {
            if (breakCondition)
            {
                break;
            }
            letter.text = alphabetShuffle[textIndex];

                ++textIndex;

                if (textIndex >= alphabet.Count())
                    textIndex = 0;

                yield return new WaitForSeconds(1f);

            }

        }
    }
