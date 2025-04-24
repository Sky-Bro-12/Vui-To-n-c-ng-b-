using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KhaiCanSieuKho : MonoBehaviour
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
        audioSource = GetComponent<AudioSource>();
        GenerateQuestion();
    }

    public void GenerateQuestion()
    {
        int degree = Random.Range(2, 5);              // Bậc căn: 2, 3 hoặc 4
        int radicand = Random.Range(2, 1000);         // Số cần khai căn: < 1000
        correctAnswer = Mathf.Pow(radicand, 1f / degree);  // Kết quả đúng (float)

        questionText.text = $"Căn bậc {degree} của {radicand} = ?";

        // Tạo các lựa chọn sai + đúng
        HashSet<float> options = new HashSet<float>();
        options.Add((float)System.Math.Round(correctAnswer, 6));

        while (options.Count < 4)
        {
            float wrong = correctAnswer + Random.Range(-2f, 2f);
            wrong = (float)System.Math.Round(wrong, 6);

            if (wrong > 0 && Mathf.Abs(wrong - correctAnswer) > 0.0001f)
                options.Add(wrong);
        }

        // Gán đáp án vào các nút
        List<float> answerList = new List<float>(options);
        Shuffle(answerList);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            float answer = answerList[i];
            answerButtons[i].GetComponentInChildren<Text>().text = answer.ToString("F6");
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerClicked(answer));
        }

        totalQuestions++;
    }

    void OnAnswerClicked(float selectedAnswer)
    {
        foreach (Button btn in answerButtons)
        {
            float btnValue = float.Parse(btn.GetComponentInChildren<Text>().text);
            Image btnImage = btn.GetComponent<Image>();

            bool isCorrect = Mathf.Abs(btnValue - correctAnswer) < 0.000001f;

            if (btnValue == selectedAnswer)
            {
                btnImage.color = isCorrect ? Color.green : Color.red;

                if (isCorrect)
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
