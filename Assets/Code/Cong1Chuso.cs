using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cong1ChuSo : MonoBehaviour
{
    public Text questionText; //Câu hỏi
    public Button[] answerButtons; //Ô trả lời

    private int correctAnswer; //Câu trả lời đúng

    public AudioClip correctSound; //Âm thanh trả lời đúng
    public AudioClip wrongSound; //Âm thanh trả lời sai
    private AudioSource audioSource; 

    public Text scoreText; //Điểm

    private int totalQuestions = 0; //Tổng điểm
    private int correctCount = 0; //Tổng số câu đúng

    void Start()
    {
        GenerateQuestion();
        audioSource = GetComponent<AudioSource>();
    }

    public void GenerateQuestion()
    {
        // Tạo phép cộng ngẫu nhiên
        int a = Random.Range(0, 10);
        int b = Random.Range(0, 10);
        correctAnswer = a + b;

        questionText.text = $"{a} + {b} = ?";

        // Tạo đáp án sai
        HashSet<int> options = new HashSet<int> { correctAnswer };
        while (options.Count < 4)
        {
            int wrongAnswer = Random.Range(correctAnswer - 5, correctAnswer + 5);
            if (wrongAnswer >= 0) options.Add(wrongAnswer);
        }

        // Xáo trộn và gán vào các nút
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
        // phần còn lại không đổi...
    }

    void OnAnswerClicked(int selectedAnswer)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            Button btn = answerButtons[i];
            int btnValue = int.Parse(btn.GetComponentInChildren<Text>().text);
            Image btnImage = btn.GetComponent<Image>();

            if (btnValue == selectedAnswer)
            {
                btnImage.color = (selectedAnswer == correctAnswer) ? Color.green : Color.red;

                // Phát âm thanh đúng/sai
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
            // Reset về trắng
            btn.GetComponent<Image>().color = Color.white;
            btn.interactable = true;
        }

        GenerateQuestion();
    }

}
