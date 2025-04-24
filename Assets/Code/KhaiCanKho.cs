using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KhaiCanKho : MonoBehaviour
{
    public Text questionText;
    public Button[] answerButtons;

    private long correctAnswer;

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
        int a = Random.Range(2, 9); // Bậc căn (2–8)
        long x = Random.Range(10, 100); // Cơ số (10–99)
        long b = Power(x, a); // b = x^a
        correctAnswer = x;

        questionText.text = $"Căn bậc {a} của {b} = ?";

        // Tạo đáp án sai
        HashSet<long> options = new HashSet<long> { correctAnswer };
        while (options.Count < 4)
        {
            long wrongAnswer = correctAnswer + Random.Range(-3, 4);
            if (wrongAnswer >= 0 && wrongAnswer != correctAnswer)
                options.Add(wrongAnswer);
        }

        // Gán đáp án
        List<long> answerList = new List<long>(options);
        Shuffle(answerList);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            long answer = answerList[i];
            answerButtons[i].GetComponentInChildren<Text>().text = answer.ToString();
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerClicked(answer));
        }

        totalQuestions++;
    }

    void OnAnswerClicked(long selectedAnswer)
    {
        foreach (Button btn in answerButtons)
        {
            long btnValue = long.Parse(btn.GetComponentInChildren<Text>().text);
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

    // Hàm tự tính lũy thừa cho kiểu long
    long Power(long baseVal, int exponent)
    {
        long result = 1;
        for (int i = 0; i < exponent; i++)
        {
            result *= baseVal;
        }
        return result;
    }
}
