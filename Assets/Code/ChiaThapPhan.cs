using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChiaThapPhan : MonoBehaviour
{
    public Text questionText;
    public Button[] answerButtons;

    private float correctAnswer;

    public AudioClip correctSound;
    public AudioClip wrongSound;
    private AudioSource audioSource;

    public Text scoreText;

    private int totalQuestions = 0;
    private int correctCount = 0;

    void Start()
    {
        GenerateQuestion();
        audioSource = GetComponent<AudioSource>();
    }

    public void GenerateQuestion()
    {
        int a = Random.Range(1, 100);
        int b = Random.Range(1, 10);
        correctAnswer = (float)a / b;

        questionText.text = $"{a} : {b} = ?";

        // Tạo đáp án sai
        HashSet<float> options = new HashSet<float> { Mathf.Round(correctAnswer * 10f) / 10f };
        while (options.Count < 4)
        {
            float wrongAnswer = correctAnswer + Random.Range(-2f, 2f);
            wrongAnswer = Mathf.Round(wrongAnswer * 10f) / 10f;

            if (wrongAnswer >= 0f)
                options.Add(wrongAnswer);
        }

        List<float> answerList = new List<float>(options);
        Shuffle(answerList);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            float answer = answerList[i];
            answerButtons[i].GetComponentInChildren<Text>().text = answer.ToString("0.0");
            answerButtons[i].onClick.RemoveAllListeners();
            float captured = answer; // cần biến tạm để tránh lỗi closure
            answerButtons[i].onClick.AddListener(() => OnAnswerClicked(captured));
        }

        totalQuestions++;
    }

    void OnAnswerClicked(float selectedAnswer)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            Button btn = answerButtons[i];
            float btnValue = float.Parse(btn.GetComponentInChildren<Text>().text);
            Image btnImage = btn.GetComponent<Image>();

            if (Mathf.Approximately(btnValue, selectedAnswer))
            {
                bool isCorrect = Mathf.Approximately(selectedAnswer, Mathf.Round(correctAnswer * 10f) / 10f);
                btnImage.color = isCorrect ? Color.green : Color.red;

                if (isCorrect)
                {
                    correctCount++;
                    audioSource.PlayOneShot(correctSound);
                }
                else
                {
                    audioSource.PlayOneShot(wrongSound);
                }
            }

            btn.interactable = false;
        }

        StartCoroutine(ResetAndNextQuestion());
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }

    IEnumerator ResetAndNextQuestion()
    {
        yield return new WaitForSeconds(3f);
        scoreText.text = $"Điểm: {correctCount} / {totalQuestions}";

        foreach (Button btn in answerButtons)
        {
            btn.GetComponent<Image>().color = Color.white;
            btn.interactable = true;
        }

        GenerateQuestion();
    }
}
