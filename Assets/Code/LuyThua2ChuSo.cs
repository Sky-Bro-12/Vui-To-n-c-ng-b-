using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class LuyThua2ChuSo : MonoBehaviour
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
        // Sinh cơ số và số mũ
        int a = Random.Range(10, 100);
        int b = Random.Range(2, 9);

        correctAnswer = PowInt(a, b);

        // Cập nhật câu hỏi
        questionText.text = $"{a} ^ {b} = ?";

        // Sinh các đáp án sai
        HashSet<long> options = new HashSet<long> { correctAnswer };
        while (options.Count < 4)
        {
            long wrongAnswer = correctAnswer + Random.Range(-5000, 5000);
            if (wrongAnswer >= 0) options.Add(wrongAnswer);
        }

        List<long> answerList = new List<long>(options);
        Shuffle(answerList);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            long answer = answerList[i];
            string formattedAnswer = answer.ToString("N0"); // có dấu phẩy

            answerButtons[i].GetComponentInChildren<Text>().text = formattedAnswer;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerClicked(answer));
        }

        totalQuestions++;
        scoreText.text = $"Câu {totalQuestions} - Điểm: {correctCount}";
    }

    void OnAnswerClicked(long selectedAnswer)
    {
        foreach (Button btn in answerButtons)
        {
            long btnValue = long.Parse(btn.GetComponentInChildren<Text>().text, NumberStyles.AllowThousands);
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
        yield return new WaitForSeconds(3f);
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

    // Hàm tính lũy thừa chính xác với số nguyên lớn
    long PowInt(int baseNum, int exponent)
    {
        long result = 1;
        for (int i = 0; i < exponent; i++)
        {
            result *= baseNum;
        }
        return result;
    }
}
