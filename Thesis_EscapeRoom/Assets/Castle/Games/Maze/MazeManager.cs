using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MazeManager : MonoBehaviour
{
    [Header("Events")]
    [Space(1), SerializeField] private UnityEvent onStartGame;
    [Space(1), SerializeField] private UnityEvent onEndGame;

    [Header("References")]
    [SerializeField] private GameObject mazeBall;
    //[SerializeField] private Transform ballSpawnPos;
    [SerializeField] private Transform mazeTransform;
    //[SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject rotateTool;
    [SerializeField] private GameObject questionUI;

    public Image questionImg;
    public TextMeshProUGUI questionText;
    public List<Button> questionButtons;
    //[SerializeField] private List<TextMeshProUGUI> buttonsText;
    [SerializeField] private int currentQuestion;

    [SerializeField] private Animator floor;

    [SerializeField] private List<Question> questions = new();

    [Space(1), SerializeField] private GameObject mazeCollider;
    [SerializeField] private List<GameObject> outsideBalls;

    [SerializeField] GameObject cinematicCamera;

    [SerializeField] NotificationManager notificationManager;

    [SerializeField] private GameObject[] questionTriggers = new GameObject[0];

    private static MazeManager mazeManager;

    private void Awake()
    {
        mazeManager = this;
    }

    private void Start()
    {
        currentQuestion = -1;
        TripleShuffleQuestions();
    }

    private void Update()
    {
        /*if (Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.D))
        {
            EndGame();
        }*/
    }

    void TripleShuffleQuestions()
    {
        questions = questions.OrderBy(item => new System.Random().Next()).ToList();
        questions = questions.OrderBy(item => new System.Random().Next()).ToList();
        questions = questions.OrderBy(item => new System.Random().Next()).ToList();
    }

    /*
    void ShuffleQuestions()
    {
        Debug.Log("Questions: " + questions.Count);
        Debug.Log("questionsText: " + questionsText.Count);
        if (questions.Count != 0)
        {
            questions = questions.OrderBy(item => new System.Random().Next()).ToList();
            currentQuestion = 0;
        }
        else
        {
            Debug.Log("Question is null");
            questionsText = questionsText.OrderBy(item => new System.Random().Next()).ToList();
            currentQuestion = 0;
        }
    }
    */

    public void CollisionEnter()
    {
        for (int i = 0; i < outsideBalls.Count; i++)
        {
            outsideBalls[i].SetActive(false);
        }

        mazeCollider.SetActive(false);
        rotateTool.SetActive(true);
        mazeBall.SetActive(true);
        onStartGame?.Invoke();
    }

    /*
    public void GetQuestion()
    {
        if (currentQuestion == questions.Count - 1 || currentQuestion == questionsText.Count - 1)
        {
            ShuffleQuestions();
        }

        //FirstPersonController.instance.currentPlayerState = PlayerStates.paused;

        mazeBall.GetComponent<Rigidbody>().useGravity = false;
        questionUI.SetActive(true);

        if (questions.Count != 0)
        {
            questionText.enabled = false;

            for (int i = 0; i < buttonsText.Count; i++)
            {
                buttonsText[i].enabled = false;
            }

            questionImg.sprite = questions[currentQuestion].question;

            questions[currentQuestion].answers = questions[currentQuestion].answers.OrderBy(item => new System.Random().Next()).ToList();

            for (int i = 0; i < questionButtons.Count; i++)
            {
                questionButtons[i].sprite = questions[currentQuestion].answers[i];
            }
        }
        else
        {
            questionImg.enabled = false;

            for (int i = 0; i < questionButtons.Count; i++)
            {
                questionButtons[i].enabled = false;
            }

            questionText.text = questionsText[currentQuestion].questionText;

            questionsText[currentQuestion].answersText = questionsText[currentQuestion].answersText.OrderBy(item => new System.Random().Next()).ToList();

            for (int i = 0; i < questionButtons.Count; i++)
            {
                buttonsText[i].text = questionsText[currentQuestion].answersText[i];
            }
        }
    }
    */

    public void GetQuestion()
    {
        currentQuestion++;
        if (currentQuestion == questions.Count)
        {
            currentQuestion = 0;
            TripleShuffleQuestions();
        }

        questionUI.SetActive(true);
        questions[currentQuestion].SetQuestion();
        mazeBall.GetComponent<Rigidbody>().useGravity = false;
    }

    public void CheckAnswer(bool correct)
    {
        if (correct)
        {
            notificationManager.SetMessage("Podes continuar!", notificationManager.GetColorRight());
            mazeBall.GetComponent<Rigidbody>().useGravity = true;
            questionUI.SetActive(false);

            if (cinematicCamera != null)
            {
                FirstPersonController.instance.currentPlayerState = PlayerStates.playing;
            }
        }
        else
        {
            notificationManager.SetMessage("Tenta Novamente!", notificationManager.GetColorWrong());
            GetQuestion();
        }
    }

    /*
    public void CheckAnswer(Image givenAnswer)
    {
        if (questions[currentQuestion].correctAnswer.name == givenAnswer.sprite.name)
        {
            notificationManager.SetMessage("Podes continuar!", notificationManager.GetColorRight());
            mazeBall.GetComponent<Rigidbody>().useGravity = true;
            questionUI.SetActive(false);
            currentQuestion++;

            if (cinematicCamera != null)
            {
                FirstPersonController.instance.currentPlayerState = PlayerStates.playing;
            }
        }
        else
        {
            notificationManager.SetMessage("Tenta Novamente!", notificationManager.GetColorWrong());
        }
    }

    public void CheckAnswerText(TextMeshProUGUI givenAnswer)
    {
        Debug.Log("CHECK");
        for (int i = 0; i < questionsText[currentQuestion].correctAnswerText.Count; i++)
        {
            Debug.Log("questionsText[currentQuestion].correctAnswerText[i]: " + questionsText[currentQuestion].correctAnswerText[i]);
            Debug.Log("givenAnswer.text: " + givenAnswer.text);
            if (questionsText[currentQuestion].correctAnswerText[i] == givenAnswer.text)
            {
                notificationManager.SetMessage("Podes continuar!", notificationManager.GetColorRight());
                mazeBall.GetComponent<Rigidbody>().useGravity = true;
                questionUI.SetActive(false);

                currentQuestion++;

                if (cinematicCamera != null)
                {
                    FirstPersonController.instance.currentPlayerState = PlayerStates.playing;
                }
            }
            else
            {
                notificationManager.SetMessage("Tenta Novamente!", notificationManager.GetColorWrong());
            }
        }
    }
    */

    public void EndGame()
    {
        for (int i = 0; i < questionTriggers.Length; i++)
        {
            if (questionTriggers[i].activeSelf)
            {
                return;
            }
        }

        if (cinematicCamera != null)
        {
            mazeBall.SetActive(false);
            cinematicCamera.SetActive(true);
        }
        else
        {
            transform.localEulerAngles = Vector3.right * 90;
            onEndGame?.Invoke();
        }
    }

    [Serializable]
    private class Question
    {
        [SerializeField] private string text = string.Empty;
        [SerializeField] private Sprite sprite = null;

        [SerializeField] private List<Answer> answers = new();

        public void SetQuestion()
        {
            if (text.Equals("<random>"))
            {
                mazeManager.questionText.text = RandomText();
            }
            else
            {
                mazeManager.questionText.text = text;
            }
            mazeManager.questionImg.sprite = sprite;

            SetAnswers();
        }

        const string glyphs = " !\"#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        private string RandomText()
        {
            string randomString = string.Empty;
            int charAmount = UnityEngine.Random.Range(30, 76);
            for (int i = 0; i < charAmount; i++)
            {
                randomString += glyphs[UnityEngine.Random.Range(0, glyphs.Length)];
            }
            return randomString;
        }


        void TripleShuffleAnswers()
        {
            answers = answers.OrderBy(item => new System.Random().Next()).ToList();
            answers = answers.OrderBy(item => new System.Random().Next()).ToList();
            answers = answers.OrderBy(item => new System.Random().Next()).ToList();
        }

        public void SetAnswers()
        {
            TripleShuffleAnswers();

            List<Button> buttons = mazeManager.questionButtons;
            Button button0 = buttons[0];
            int difference = buttons.Count - answers.Count;
            if (difference > 0)
            {
                for (int i = 0; i < difference; i++)
                {
                    Destroy(buttons[^1]);
                }
                buttons.Clear();
                buttons.Add(button0);
            }
            else if (difference < 0)
            {
                difference = Mathf.Abs(difference);
                Transform parent = button0.transform.parent;
                for (int i = 0; i < difference; i++)
                {
                    buttons.Add(Instantiate(button0, parent));
                    buttons[^1].name = "Option " + (i + 1); 
                }
            }
            mazeManager.questionButtons = buttons;

            for (int i = 0; i < answers.Count; i++)
            {
                answers[i].Get(buttons[i]);
            }
        }
    }

    [Serializable]
    private class Answer
    {
        [SerializeField] private Sprite sprite = null;
        [SerializeField] private string text = string.Empty;
        [SerializeField] private bool correct = false;

        public void Get(Button option)
        {
            option.GetComponentInChildren<TextMeshProUGUI>().text = text;
            option.GetComponentsInChildren<Image>()[1].sprite = sprite;
            option.onClick.RemoveAllListeners();
            option.onClick.AddListener(delegate { mazeManager.CheckAnswer(correct); });
        }
    }

}

[Serializable]
public class Questions
{
    public Sprite question;
    public string questionText;

    public List<Sprite> answers;
    public List<string> answersText;
    public Sprite correctAnswer;
    public List<string> correctAnswerText;

    public Questions(Sprite question, List<Sprite> answers, Sprite correctAnswer)
    {
        this.question = question;
        this.answers = answers;
        this.correctAnswer = correctAnswer;
    }

    public Questions(string questionText, List<string> answersText, List<string> correctAnswerText)
    {
        this.questionText = questionText;
        this.answersText = answersText;
        this.correctAnswerText = correctAnswerText;
    }
}