using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChiaDu : MonoBehaviour
{
    public Text questionText;
    public Button[] answerButtons;

    private int correctQuotient;
    private int correctRemainder;

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
        int b = Random.Range(2, 10); // tránh b = 1 cho hay hơn
        int quotient = Random.Range(1, 9);
        int remainder = Random.Range(0, b);

        int a = b * quotient + remainder;

        correctQuotient = quotient;
        correctRemainder = remainder;

        questionText.text = $"{a} : {b} = ?";

        HashSet<string> options = new HashSet<string>();
        options.Add($"{quotient} dư {remainder}");

        // Tạo thêm đáp án sai
        while (options.Count < 4)
        {
            int wrongQ = Random.Range(0, 9);
            int wrongR = Random.Range(0, b);

            if (wrongQ != quotient || wrongR != remainder)
                options.Add($"{wrongQ} dư {wrongR}");
        }

        List<string> answerList = new List<string>(options);
        Shuffle(answerList);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            string answer = answerList[i];
            answerButtons[i].GetComponentInChildren<Text>().text = answer;
            answerButtons[i].onClick.RemoveAllListeners();

            string captured = answer; // tránh lỗi closure
            answerButtons[i].onClick.AddListener(() => OnAnswerClicked(captured));
        }

        totalQuestions++;
    }

    void OnAnswerClicked(string selectedAnswer)
    {
        string correctAnswer = $"{correctQuotient} dư {correctRemainder}";

        for (int i = 0; i < answerButtons.Length; i++)
        {
            Button btn = answerButtons[i];
            string btnText = btn.GetComponentInChildren<Text>().text;
            Image btnImage = btn.GetComponent<Image>();

            if (btnText == selectedAnswer)
            {
                bool isCorrect = (btnText == correctAnswer);
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
