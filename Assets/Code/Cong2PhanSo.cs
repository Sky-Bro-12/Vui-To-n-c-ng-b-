using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cong2PhanSo : MonoBehaviour
{
    public Text questionText;
    public Button[] answerButtons;

    private string correctAnswer;

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
        // Tạo hai phân số ngẫu nhiên
        int tu1 = Random.Range(1, 10);
        int mau1 = Random.Range(1, 10);
        int tu2 = Random.Range(1, 10);
        int mau2 = Random.Range(1, 10);

        // Tính tổng
        int tu = tu1 * mau2 + tu2 * mau1;
        int mau = mau1 * mau2;

        int gcd = UCLN(tu, mau);
        int rutgonTu = tu / gcd;
        int rutgonMau = mau / gcd;

        correctAnswer = $"{rutgonTu}/{rutgonMau}";

        questionText.text = $"{tu1}/{mau1} + {tu2}/{mau2} = ?";

        // Tạo đáp án sai
        HashSet<string> options = new HashSet<string> { correctAnswer };
        while (options.Count < 4)
        {
            int wrongTu1 = Random.Range(1, 10);
            int wrongMau1 = Random.Range(1, 10);
            int wrongTu2 = Random.Range(1, 10);
            int wrongMau2 = Random.Range(1, 10);

            int wrongTu = wrongTu1 * wrongMau2 + wrongTu2 * wrongMau1;
            int wrongMau = wrongMau1 * wrongMau2;

            int wrongGCD = UCLN(wrongTu, wrongMau);
            string wrongAns = $"{wrongTu / wrongGCD}/{wrongMau / wrongGCD}";

            options.Add(wrongAns);
        }

        // Xáo trộn và gán vào các nút
        List<string> answerList = new List<string>(options);
        Shuffle(answerList);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            string answer = answerList[i];
            answerButtons[i].GetComponentInChildren<Text>().text = answer;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerClicked(answer));
        }

        totalQuestions++;
    }

    void OnAnswerClicked(string selectedAnswer)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            Button btn = answerButtons[i];
            string btnValue = btn.GetComponentInChildren<Text>().text;
            Image btnImage = btn.GetComponent<Image>();

            if (btnValue == selectedAnswer)
            {
                btnImage.color = (selectedAnswer == correctAnswer) ? Color.green : Color.red;

                if (selectedAnswer == correctAnswer)
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

    int UCLN(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
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
        yield return new WaitForSeconds(5f);
        scoreText.text = $"Điểm: {correctCount} / {totalQuestions}";

        foreach (Button btn in answerButtons)
        {
            btn.GetComponent<Image>().color = Color.white;
            btn.interactable = true;
        }

        GenerateQuestion();
    }
}
