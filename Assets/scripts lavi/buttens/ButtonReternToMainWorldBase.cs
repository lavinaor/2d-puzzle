using UnityEngine;

public class ButtonReturnToMainWorld : MonoBehaviour
{
    public void OnClickReturnToMainWorld()
    {
        WorldManager.Instance.ReturnToMainWorldBase();
    }
}
