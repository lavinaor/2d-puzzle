using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //���� �� �� �ui
    [SerializeField]
    private UI[] UIAray = new UI[0];

    //���� �� ������ �����
    [SerializeField]
    private UIType currentUIType = UIType.NoneUI;

    //���� �� ����� �� ���� �����
    [SerializeField]
    private int MainManuNum;

    //���� ������ �� ���� �� �������� ��� ��� ���� �� �� �� ��
    [System.Serializable]
    class UI
    {
        //���� �� ��� �ui
        public UIType UIType = UIType.NoneUI;

        //���� �� ��������
        public GameObject UIGameObject;

        //���� ��� ���� �� 
        public bool isTimePase = false;
    }

    //����� �� Ui
    public enum UIType
    {
        NoneUI = 0,
        settingsUI = 1,
        loseUI = 2,
        winUI = 3
    }

    // ���� �� �� ������ �����
    void Start()
    {
        //����� �� ������ �����
        ChangeActiveUI();
    }

    //���� �� ���� ��� ���� �����
    public void ChangeUIState(int newState)
    {
        //���� �� �� ������
        currentUIType = (UIType)newState;

        //����� �����
        ChangeActiveUI();
    }

    // ���� �� �Ui ������
    public void ChangeActiveUI()
    {
        //���� �� �� �������� ���� �� ��� ������ ������ �� �����
        foreach (UI ui in UIAray)
        {
            //���� �� ����
            if (ui.UIType == currentUIType)
            {
                //����� �� �����
                ui.UIGameObject.SetActive(true);

                //���� �� �� ��� �� ��
                if (ui.isTimePase)
                {
                    Time.timeScale = 0f;
                }
                else
                {
                    Time.timeScale = 1f;
                }
            }
            else
            {
                //���� �� ��� ����
                ui.UIGameObject.SetActive(false);
            }
        }
    }

    // ����� ���� �� ����� 
    public void RestartLevel()
    {
        //���� �� ����� ������� ������ ���� ����
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        //���� �� ���� ���� ����� ���� �� 
        Time.timeScale = 1f;
    }

    //���� �� �����
    public void ExitGame()
    {
        //���� ��� �������� �� ���� �� ����� ����� ������� �� ���� �� ��������� �����
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
    }

    // ����� ���� ������
    public void GoToMainManu()
    {
        //���� �� ��� ������ ��� �� ������
        SceneManager.LoadScene(MainManuNum);
        
        //����� �� ���� ����
        Time.timeScale = 1f; // Resume the game
    }
}
