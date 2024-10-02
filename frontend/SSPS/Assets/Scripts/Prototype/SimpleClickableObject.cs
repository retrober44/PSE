using UnityEngine;

public class SimpleClickableObject : MonoBehaviour, IClickable
{
    // Test output for the sample scene.
    public void onClick()
    {
        Debug.Log("I, the greatest of all SimpleClickableObejcts, have been clicked. I shall be clicked again.");
    }
}
