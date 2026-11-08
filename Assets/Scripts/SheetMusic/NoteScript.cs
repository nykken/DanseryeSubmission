using UnityEngine;

[RequireComponent(typeof(FallingItem))]
public class NoteScript : MonoBehaviour
{
    public SpriteRenderer fill;

    public void SetColor(Color color) => fill.color = color;
}
