using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClassSelectionUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject selectionPanel;

    [Header("Dialogue")]
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button nextButton;

    [Header("Class Buttons")]
    [SerializeField] private Button strikerButton;
    [SerializeField] private Button arcanistButton;
    [SerializeField] private Button astraCharmButton;

    private AstraShard currentShard;
    private int dialogueIndex = 0;
    private string[] dialogues =
    {
        "นี่มันายเลือดแห่งเผ่าแมวบิน ข้าเจอแล้ว... ในที่สุด",
        "ผู้สืบทอดสายเลือดเอ๋ย",
        "ข้าจะมอบพลังให้เจ้าเอง จงเลือกเส้นทางของเจ้าซะ....1"
    };

    void Start()
    {
        dialoguePanel.SetActive(false);
        selectionPanel.SetActive(false);

        nextButton.onClick.AddListener(OnNextDialogue);
        strikerButton.onClick.AddListener(() => OnClassSelected(ClassType.Striker));
        arcanistButton.onClick.AddListener(() => OnClassSelected(ClassType.Arcanist));
        astraCharmButton.onClick.AddListener(() => OnClassSelected(ClassType.AstraCharm));

    }

    public void ShowSelection(AstraShard shard, ClassType[] availableClasses)
    {
        currentShard = shard;
        dialogueIndex = 0;

        PausePlayer(true);

        dialoguePanel.SetActive(true);
        speakerText.text = "Astra Shard";
        dialogueText.text = dialogues[0];
    }

    void OnNextDialogue()
    {
        dialogueIndex++;

        if (dialogueIndex < dialogues.Length)
        {
            dialogueText.text = dialogues[dialogueIndex];
        }
        else
        {
            dialoguePanel.SetActive(false);
            selectionPanel.SetActive(true);
        }
    }

    void OnClassSelected(ClassType classType)
    {
        Debug.Log($"✨ [UI] เลือกคลาส: {classType}");

        selectionPanel.SetActive(false);

        Player player = FindObjectOfType<Player>();
        if (currentShard != null && player != null)
        {
            currentShard.GrantPower(player, classType);
        }

        PausePlayer(false);
    }

    void PausePlayer(bool pause)
    {
        PlayerController controller = FindObjectOfType<PlayerController>();
        if (controller != null)
        {
            controller.enabled = !pause;
        }

        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null && pause)
                rb.velocity = Vector2.zero;
        }
    }
}