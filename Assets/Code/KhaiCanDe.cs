using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KhaiCanDe : MonoBehaviour
{
    public Text questionText;
    public Button[] answerButtons;

    private int correctAnswer;

    public AudioClip correctSound;
    public AudioClip wrongSound;
    private AudioSource audioSource;

    public Text scoreText;

    private int totalQuestions = 0;
    private int correctCount = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GenerateQuestion();
    }

    public void GenerateQuestion()
    {
        int a = Random.Range(2, 9);           // Bậc căn (2 -> 4)
        int x = Random.Range(2, 10);          // Kết quả đúng (x)
        int b = (int)Mathf.Pow(x, a);         // b = x^a
        correctAnswer = x;

        questionText.text = $"Căn bậc {a} của {b} = ?";

        // Tạo đáp án sai
        HashSet<int> options = new HashSet<int> { correctAnswer };
        while (options.Count < 4)
        {
            int wrongAnswer = Random.Range(correctAnswer - 3, correctAnswer + 4);
            if (wrongAnswer >= 0 && wrongAnswer != correctAnswer)
                options.Add(wrongAnswer);
        }

        // Gán đáp án
        List<int> answerList = new List<int>(options);
        Shuffle(answerList);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int answer = answerList[i];
            answerButtons[i].GetComponentInChildren<Text>().text = answer.ToString();
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerClicked(answer));
        }

        totalQuestions++;
    }

    void OnAnswerClicked(int selectedAnswer)
    {
        foreach (Button btn in answerButtons)
        {
            int btnValue = int.Parse(btn.GetComponentInChildren<Text>().text);
            Image btnImage = btn.GetComponent<Image>();

            if (btnValue == selectedAnswer)
            {
                btnImage.color = (selectedAnswer == correctAnswer) ? Color.green : Color.red;

                if (selectedAnswer == correctAnswer)
                {
                    correctCount++;
                    if (audioSource && correctSound)
                        audioSource.PlayOneShot(correctSound);
                }
                else
                {
                    if (audioSource && wrongSound)
                        audioSource.PlayOneShot(wrongSound);
                }
            }

            btn.interactable = false;
        }

        StartCoroutine(ResetAndNextQuestion());
    }

    IEnumerator ResetAndNextQuestion()
    {
        yield return new WaitForSeconds(4f);
        scoreText.text = $"Điểm: {correctCount} / {totalQuestions}";

        foreach (Button btn in answerButtons)
        {
            btn.GetComponent<Image>().color = Color.white;
            btn.interactable = true;
        }

        GenerateQuestion();
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
}
