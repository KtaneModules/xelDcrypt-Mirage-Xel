using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KModkit;
using System.Linq;


public class DCRYPT : MonoBehaviour
{
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
    bool TwitchPlaysActive;
    static int moduleIDCounter = 1;
    int moduleID;
    public struct Cell
    {
        public char character;
        public int X;
        public int Y;

        public Cell(char _character, int _X, int _Y)
        {
            this.character = _character;
            this.X = _X;
            this.Y = _Y;
        }
    }
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
    void Start()
    {
        Debug.Log(wordList.Length);
        startingWord = wordList[UnityEngine.Random.Range(0, 52)];
        answerWord = wordList[(Array.IndexOf(wordList, startingWord) + bomb.GetBatteryCount() + bomb.GetSerialNumberNumbers().Last()) % 52];
        if (startingWord == answerWord)
        {
            answerWord = wordList[Array.IndexOf(wordList, startingWord) + 7];
        }
        Debug.LogFormat("[D-CRYPT #{0}] The starting word is {1}. ", moduleID, startingWord);
        Debug.LogFormat("[D-CRYPT #{0}] The answer word is {1}. ", moduleID, answerWord);
        int textCounter = 0;
        foreach (TextMesh text in texts)
        {
            text.color = initial;
            text.text = startingWord.ToCharArray()[textCounter].ToString();
            textCounter++;
        }
    }

    // Update is called once per frame
    void pressLetter(KMSelectable letter)
    {
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
            StopAllCoroutines();
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

        string loggingWord = "";
        foreach (TextMesh text in texts)
        {
            loggingWord = loggingWord + text.text;
        }
        Debug.LogFormat("[D-CRYPT #{0}] You submitted {1}. ", moduleID, loggingWord);
        if (loggingWord == answerWord)
        {
            module.HandlePass();
            sound.PlaySoundAtTransform("SolveSound", transform);
            Debug.LogFormat("[D-CRYPT #{0}] That was correct. Module solved. ", moduleID);
        }
        else
        {
            module.HandleStrike();
            Debug.LogFormat("[D-CRYPT #{0}] That was incorrect. Strike! ", moduleID);
            foreach (TextMesh text in texts)
            {
                text.color = initial;
            }
            stageCounter = 1;
            Start();
        }
    }

    IEnumerator cycleLetters(TextMesh letter)
    {
        
        breakCondition = false;
        int textIndex = 0;
        List<string> textOptions = new List<string> { };
        string candidateLetter;
        textOptions.Add(answerWord[Array.IndexOf<TextMesh>(texts, letter)].ToString());
        for (int i = 0; i < 5; i++)
        {
            do
            {
                candidateLetter = alphabet[UnityEngine.Random.Range(0, 26)];
            } while (textOptions.Contains(candidateLetter));
            textOptions.Add(candidateLetter);
        };
        if  (TwitchPlaysActive){ textOptions = alphabet.ToList().Shuffle(); }
            else{textOptions = textOptions.Shuffle();}
        
        while (true)
        {
            if (breakCondition)
            {
                break;
            }
            letter.text = textOptions[textIndex];

            ++textIndex;

            if (textIndex >= textOptions.Count())
                textIndex = 0;
            if (TwitchPlaysActive)
            {
                yield return new WaitForSeconds(0.01f);
            }
            else
            {
                yield return new WaitForSeconds(0.2f);
            }
        }

    }
#pragma warning disable 414
    private string TwitchHelpMessage = "use e.g. '!{0} submit ABCDE' to submit the answer word.";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant();
        string validcmds = "abcdefghijklmnopqrstuvwxyz ";
        string[] commandArray = command.Split(' ');
        if (commandArray.Length != 2 || commandArray[0] != "submit" || commandArray[1].Length != letters.Length)
        {
            yield return "sendtochaterror @{0}, invalid command.";
            yield break;
        }
        else
        {
            for (int i = 0; i < command.Length; i++)
            {
                if (!validcmds.Contains(command[i]))
                {
                    yield return "sendtochaterror Invalid command.";
                    yield break;
                }
            }
            for (int i = 0; i < letters.Length; i++)
            {
                yield return null;
                letters[i].OnInteract();
                while (letters[i].GetComponentInChildren<TextMesh>().text != commandArray[1][i].ToString().ToUpperInvariant())
                {
                    yield return "trycancel [message]";
                }
                letters[i].OnInteractEnded();
            }
        }
    }
    IEnumerator TwitchHandleForcedSolve()
    {
           for (int i = 0; i<letters.Length; i++)
            {
                yield return null;
                letters[i].OnInteract();
                while (letters[i].GetComponentInChildren<TextMesh>().text != answerWord[i].ToString())
                {
                    yield return null;
                }
                letters[i].OnInteractEnded();
            }
    }
}
