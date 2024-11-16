using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private bool bCreateWanderers = true;

    [SerializeField] private Wanderer wandererPrefab;

    [SerializeField] private List<Sprite> sprites = new List<Sprite>();

    [SerializeField] private Transform Wanderers;

    public List<Wanderer> wandererList = new List<Wanderer>();
    public Wanderer match1;
    public Wanderer match2;

    public Image match1Sprite;
    public Image match2Sprite;

    private List<Wanderer> selectedWanderers = new List<Wanderer>();

    public TextMeshProUGUI timerText;
    public float timer = 60.0f;
    public float currentTime;

    public MenuManager menuManager;

    private void Start() {
        if (Instance == null) Instance = this;

        if (bCreateWanderers)
        {
            CreateWanderers();
            CreateMatch();
        }

        currentTime = timer;
    }

    private void CreateWanderers() {
        foreach (Sprite sprite in sprites) {
            float x = Random.Range(-wandererPrefab.areaSize.x / 2 + wandererPrefab.areaOffset.x, wandererPrefab.areaSize.x / 2 + wandererPrefab.areaOffset.x);
            float y = Random.Range(-wandererPrefab.areaSize.y / 2 + wandererPrefab.areaOffset.y, wandererPrefab.areaSize.y / 2 + wandererPrefab.areaOffset.y);
           
            Wanderer newWanderer = Instantiate(wandererPrefab, new Vector2(x, y), Quaternion.identity, Wanderers);
            newWanderer.spriteRenderer.sprite = sprite;

            wandererList.Add(newWanderer);
        }
    }

    private void CreateMatch()
    {
        int randomIndex = Random.Range(0, wandererList.Count);
        match1Sprite.sprite = wandererList[randomIndex].spriteRenderer.sprite;
        match1 = wandererList[randomIndex];

        // Get a random index
        randomIndex = Random.Range(0, wandererList.Count);
        match2Sprite.sprite = wandererList[randomIndex].spriteRenderer.sprite;
        match2 = wandererList[randomIndex];
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            HandleClick();
        }

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timerText.text = currentTime.ToString("F1");
        }
        else
        {
            menuManager.LossMenu();
        }
    }

    private void HandleClick() {
        // raycast from mouse to wanderer
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null) {
            Wanderer clickedWanderer = hit.collider.GetComponent<Wanderer>();
            if (clickedWanderer != null)
            {
                WandererClicked(clickedWanderer);
            }
        }
    }

    private void WandererClicked(Wanderer wanderer) {
        if (IsWandererSelected(wanderer)) {
            // Deselect wanderer

            wanderer.SetMovement(true);
            selectedWanderers.Remove(wanderer);

        }
        else {
            // Select wanderer
            selectedWanderers.Add(wanderer);

            wanderer.SetMovement(false);
        }

        // Check Win Condition
        if (selectedWanderers.Count == 2) CheckWin();

    }

    private void CheckWin() {
        if (selectedWanderers.Contains(match1) && selectedWanderers.Contains(match2)) {
            Debug.Log("WIN");
        }
        else {
            Debug.Log("YOU SUCK DUMBASS");
        }
    }


    // TODO: Finish this
    public bool IsWandererSelected(Wanderer wanderer) {
       return selectedWanderers.Contains(wanderer);
    }
}
