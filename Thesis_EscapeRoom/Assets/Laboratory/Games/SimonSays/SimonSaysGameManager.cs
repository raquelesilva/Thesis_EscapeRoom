using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SimonSaysGameManager : MonoBehaviour
{
    //keeping track if puzzle is done
    [SerializeField] bool isDoorOpen = false;
    //the door that shall be activated
    [SerializeField] UnityEvent door;


    //Counterpart that must communicate answer to
    [SerializeField] SimonSaysGameManager twin;
    //Uma batota
    [SerializeField] Keypad_Input beeper;
    //text that indicates number of code inputs inserted
    [SerializeField] TextMeshPro inputCountDisplay;
    //renderers to make buttons blink
    [SerializeField] LazyBlinker BLINKER;


    //answer to the puzzle
    [SerializeField] SimonCode answer;
    //saved inputs
    [SerializeField] List<string> recordedInputs = new List<string>();


    //codigo a dar display, é a resposta correta para o SimonSays alternativo
    SimonCode answerToDisplay;
    //spawn place for the code hint
    [SerializeField] GameObject displayPlace;

    public void ToggleVisibility(bool decision)
    {
        List<BoxCollider> colls = gameObject.GetComponentsInChildren<BoxCollider>().ToList();
        List<MeshRenderer> meshRenders = gameObject.GetComponentsInChildren<MeshRenderer>().ToList();

        foreach (var item in colls)
        {
            item.enabled = decision;
        }
        foreach (var item in meshRenders)
        {
            item.enabled = decision;
        }

        displayPlace.SetActive(decision);
    }

    private void Start()
    {
        answer = tempAnswer;
        //GenerateSimonCode(6);
        twin.SetDisplayAnswer(answer);

        //reset text
        inputCountDisplay.text = "";
        for (int i = 0; i < answer.colors.Count; i++)
        {
            inputCountDisplay.text += "-";
        }
    }


    public void AddInput(string title)
    {
        if (isDoorOpen) { return; }
        Debug.Log("input added -> " + title);
        string newTxt = "[]";
        for (int i = 0; i < inputCountDisplay.text.Length - 1; i++)
        {
            newTxt += inputCountDisplay.text[i];
        }
        inputCountDisplay.text = newTxt;

        
        //add input to recorded combination
        recordedInputs.Add(title);
        //see if number is bigger than code list
        if (recordedInputs.Count == answer.colors.Count)
        {//if we reach the limit test answer
            TestAnswer();
        }
        else
        {
            //beep
            beeper.PlayClickPadSound();
        }
    }

    public void ResetInputs()
    {
        if (isDoorOpen) { return; }
        recordedInputs.Clear();
        inputCountDisplay.text = "";
        for (int i = 0; i < answer.colors.Count; i++)
        {
            inputCountDisplay.text += "-";
        }
        //insert neutral beep here
        beeper.PlayClickPadSound();
    }

    void TestAnswer()
    {
        //test combination validity
        bool thereAreNoErrors = true;
        for (int i = 0; i < answer.colors.Count; i++)
        {
            Debug.Log(recordedInputs[i] + " =? " + answer.colors[i].name);
            if (recordedInputs[i] != answer.colors[i].name)
            {
                thereAreNoErrors = false;
                break;
            }
        }
        Debug.Log("thereAreNoErrors = " + thereAreNoErrors);
        if (thereAreNoErrors)
        {
            //if answer correct, light up all buttons
            FlashButtons(true);
            //open door
            OpenNeNoor();
            isDoorOpen = true;
            Debug.Log("CONGRALATION U WEEN");
        }
        else
        {
            //if incorrect, reset puzzle
            recordedInputs.Clear();
            inputCountDisplay.text = "";
            for (int i = 0; i < answer.colors.Count; i++)
            {
                inputCountDisplay.text += "-";
            }

            //make an error buzzing sound
            FlashButtons(false);
            Debug.Log("CONGRALATION U FEIL");
        }
    }





    //opens ne noor, OPEN NE NOOR!
    void OpenNeNoor()
    {
        //activates the door through a unity event; you can add more stuff if you wish
        door?.Invoke();
    }

    //Generates a new random sequence with a random number of colors from 5-9 (by default, if colorNum = -1),
    //colorNum sets a fixed number of colors for the new random code
    void GenerateSimonCode(int colorNum = -1)
    {
        List<SimonColor> newColorCombo = new List<SimonColor>();

        //default setting: get random number of colors from 5-9, uses input number otherwise
        if (colorNum == -1) { colorNum = Random.Range(5, 10); } Debug.Log("Color number for new code is: " + colorNum);

        //pick at random from the ALL_COLORS options
        Debug.Log("new answer");
        for (int i = 0; i < colorNum; i++)
        {
            int rnd = Random.Range(0, ALL_COLORS.colors.Count);
            newColorCombo.Add(new SimonColor(ALL_COLORS.colors[rnd].name, ALL_COLORS.colors[rnd].color));

            Debug.Log(ALL_COLORS.colors[rnd].name + " + " + ALL_COLORS.colors[rnd].color );
        }

        answer = new SimonCode(newColorCombo);

        //send answer to the other simon says to display it
        twin.SetDisplayAnswer(answer);
    }



    //receive answerToDisplay from the other simon says
    public void SetDisplayAnswer(SimonCode twinsAnswer)
    {
        answerToDisplay = twinsAnswer;

        //kill children
        for (int i = 0; i < displayPlace.transform.childCount; i++)
        {
            Destroy(displayPlace.transform.GetChild(i));
        }

        //spawn colors 1 by one at a regular offset
        float offset = 0; float offsetAmount = -0.1f;
        Vector3 dPTP = displayPlace.transform.position;
        Debug.Log(twinsAnswer.colors.Count);
        for (int i = 0; i < twinsAnswer.colors.Count; i++)
        {
            //get number from title
            Debug.Log("color " + twinsAnswer.colors[i].name);
            string currTitNum = twinsAnswer.colors[i].name[(twinsAnswer.colors[i].name.Length - 3)] + "";

            Debug.Log("currTitNum " + currTitNum);
            //convert string to int
            int.TryParse(currTitNum, out int cTId);
            Debug.Log("ID " + cTId);
            //instantiate prefab corresponding to that index
            Object.Instantiate(SYMBOLS[cTId-1], new Vector3(dPTP.x, dPTP.y - offset, dPTP.z), Quaternion.identity, displayPlace.transform);
            offset += offsetAmount;
            //dinamically turn material emission here
            //NOPE Unity wouldn't do it, so now we got a lazy blinker
        }

        if (PlayerManager.instance.GetPlayer() == 1)
        {
            displayPlace.SetActive(false);
        }
    }


    void FlashButtons(bool isPositiveSignal)
    {
        //if isPositiveSIGNALl, all buttons glow for a second and a beep is played
        if (isPositiveSignal)
        {
            //good beep here
            beeper.PlayWinPadSound();
            //animate blinking
            BLINKER.FlashBlinkers(1, 0.56f, false);
        }
        else //otherwise, all buttons blink 3 times and an error buzzer noise plays
        {
            //bad beep here
            beeper.PlayLosePadSound();
            //animate blinking
            BLINKER.FlashBlinkers(3, 0.21f, true);
        }


    }




    [System.Serializable]
    public struct SimonCode
    {
        public SimonCode(List<SimonColor> newColors)
        {
            this.colors = newColors;
        }
        public List<SimonColor> colors;

    }
    [System.Serializable]
    public struct SimonColor
    {
        public SimonColor(string title, Color colorCode)
        {
            this.name = title;
            this.color = colorCode;
        }
        public string name;
        public Color color;

    }




    //reminder de todas as cores
    [SerializeField]
    SimonCode ALL_COLORS = new SimonCode(new List<SimonColor>{
        new SimonColor("Sigma-1-?", new Color(22,20,183)),
        new SimonColor("Delta-2-?", new Color(242,104,0)),
        new SimonColor("Psi-3-?", new Color(243,243,22)),
        new SimonColor("Lamda-4-?", new Color(82,41,16)),
        new SimonColor("Phi-5-?", new Color(231,27,27)),
        new SimonColor("Xi-6-?", new Color(143,212,143)),
        new SimonColor("Omega-7-?", new Color(0,0,0)),
        new SimonColor("Alpha-8-?", new Color(255,255,255)),
        new SimonColor("Theta-9-?", new Color(132,132,132))
    });
    //e os prefabs
    [SerializeField]
    List<GameObject> SYMBOLS = new List<GameObject>();

    [SerializeField]
    SimonCode tempAnswer;
}
