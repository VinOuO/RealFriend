using TMPro;
using UnityEngine;

public class DialogueUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_CharacterNameText;
    [SerializeField] TextMeshProUGUI m_DialogueText;
    public static DialogueUIController Instance { get; private set; }

    private void OnEnable()
    {
        Instance = this;
    }

    private void Start()
    {
        SetVisibility(false);
    }

    public void UpdateDialogue(string characterName, string dialogue)
    {
        m_CharacterNameText.text = characterName;
        m_DialogueText.text = dialogue;
    }

    public void SetVisibility(bool show)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(show);
        }
    }
}
