using UnityEngine;

public interface Interactable
{
    public void Interact(GameObject player)
    {

    }

    public string GetName();

    public bool IsHoldable();

    public void OnRelease();
}
