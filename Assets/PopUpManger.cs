using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UIManager;

public class PopUpManger : MonoBehaviour
{
    //���� �� �� �ui
    [SerializeField]
    private PopUp[] popUps = new PopUp[0];

    //���� �� ������ �����
    [SerializeField]
    private PopUpType currentPopUpType = PopUpType.NoneUI;

    //���� �� ����� �� ���� �����
    [SerializeField]
    private int MainManuNum;

    // ���� �� �� ������ �����
    void Start()
    {
        //����� �� ������ �����
        ChangeActiveUI();
    }

    //���� �� ���� ��� ���� �����
    public void ChangeUIState(int newState)
    {
        //����� �� ���� ���� ���� ����
        if (currentPopUpType == (PopUpType)newState)
        {
            currentPopUpType = PopUpType.NoneUI;
        }
        else
        {
            //���� �� �� ������
            currentPopUpType = (PopUpType)newState;
        }

        //����� �����
        ChangeActiveUI();
    }

    // ���� �� �Ui ������
    public void ChangeActiveUI()
    {
        //���� �� �� �������� ���� �� ��� ������ ������ �� �����
        foreach (PopUp popUp in popUps)
        {
            //���� �� ����
            if (popUp.popUpType == currentPopUpType)
            {
                //����� �� �����
                popUp.popUpGameObject.SetActive(true);

                //���� �� �� ��� �� ��
                if (popUp.isTimePase)
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
                popUp.popUpGameObject.SetActive(false);
            }

        }

        //�� ��� �� ����� ��� ����
        if (currentPopUpType == PopUpType.NoneUI) 
        {
            Time.timeScale = 1f;
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

    // ����� ����� ����
    public void GoToNewScene(int sceneNum)
    {
        //���� �� ����� �����
        SceneManager.LoadScene(sceneNum);

        //����� �� ���� ����
        Time.timeScale = 1f; // Resume the game
    }
}

//���� ������ �� ���� �� �������� ��� ��� ���� �� �� �� ��
[System.Serializable]
class PopUp
{
    //���� �� ��� �ui
    public PopUpType popUpType = PopUpType.NoneUI;

    //���� �� ��������
    public GameObject popUpGameObject;

    //���� ��� ���� �� 
    public bool isTimePase = false;
}

//����� �� Ui
public enum PopUpType
{
    NoneUI = 0,
    settingsUI = 1,
    storUI = 2,
    confirmation = 3,
    endScreen = 4,
}
