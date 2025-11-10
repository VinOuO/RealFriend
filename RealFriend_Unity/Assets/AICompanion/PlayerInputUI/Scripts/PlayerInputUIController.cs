using TMPro;
using UnityEngine;

public class PlayerInputUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_PlauerInputText;
    public static PlayerInputUIController Instance { get; private set; }
    public bool m_ServiceEnabled = false;
    public bool m_FinishedPlayerInput = true; public bool FinishedPlayerInput => m_FinishedPlayerInput;

    private void OnEnable()
    {
        Instance = this;
    }

    private void Start()
    {
        EndSevice();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) && m_ServiceEnabled && !m_FinishedPlayerInput)
        {
            m_FinishedPlayerInput = true;
        }
    }

    public void StartSevice()
    {
        SetVisibility(true);
        m_ServiceEnabled = true;
        m_PlauerInputText.text = "";
        m_FinishedPlayerInput = false;
    }

    public void EndSevice()
    {
        SetVisibility(false);
        m_PlauerInputText.text = "";
        m_ServiceEnabled = false;
        m_FinishedPlayerInput = true;
    }

    public string GetPlayerInput()
    {
        return m_PlauerInputText.text;
    }

    public void SetVisibility(bool show)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(show);
        }
    }
}
