using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private bool bCreateWanderers = true;

    [SerializeField] private Wanderer wandererPrefab;

    [SerializeField] private List<Sprite> sprites = new List<Sprite>();

    public List<Wanderer> wandererList = new List<Wanderer>();
    public Wanderer match1;
    public Wanderer match2;

    public Image match1Sprite;
    public Image match2Sprite;

    private List<Wanderer> selectedWanderers = new List<Wanderer>();

    private float gameStartTimer = 2.4f;
    private bool gameStartTimerOn = true;

    public TextMeshProUGUI timerText;
    public float timer = 60.0f;
    public float currentTime;
    public bool timerOn = false;

    public MenuManager menuManager;

    private Animator animator;

    private bool bCanClick = true;

    [SerializeField] private float spamPreventionTime = 1f;

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
           
            Wanderer newWanderer = Instantiate(wandererPrefab, new Vector2(x, y), Quaternion.identity, gameObject.transform);
            newWanderer.spriteRenderer.sprite = sprite;

            animator = newWanderer.GetComponent<Animator>();
            animator.Play(0, -1, Random.Range(0f, 1f)); // Randomize start offset

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
        // REMOVE THIS
        if (Input.GetKeyDown(KeyCode.G)) ForceWin();

        if (Input.GetMouseButtonDown(0)) {
            HandleClick();
        }

        if (gameStartTimer > 0)
        {
            gameStartTimer -= Time.deltaTime;
        }
        else { timerOn = true; }

        timerText.text = currentTime.ToString("F1");
        if (timerOn && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        else if (currentTime <= 0 && !timerOn) { menuManager.LossMenu(); }
    }

    private void HandleClick() {
        if (!bCanClick) return;

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
            DeselectWanderer(wanderer);
        }
        else {
            if (selectedWanderers.Count == 2) return;
            SelectWanderer(wanderer);
        }

        // Check Win Condition
        if (selectedWanderers.Count == 2) CheckWin();

    }

    private void DeselectWanderer(Wanderer wanderer) {
        wanderer.SetMovement(true);
        selectedWanderers.Remove(wanderer);
        wanderer.GetComponent<Animator>().SetBool("Selected", false);
    }

    private void SelectWanderer(Wanderer wanderer) {
        selectedWanderers.Add(wanderer);

        wanderer.SetMovement(false);
        wanderer.GetComponent<Animator>().SetBool("Selected", true);
    }

    private void CheckWin() {
        if (selectedWanderers.Contains(match1) && selectedWanderers.Contains(match2)) {
            bCanClick = false;
            
            foreach(Wanderer wanderer in wandererList) {
                if (wanderer == match1 || wanderer == match2) {
                    // TODO: go to center
                }
                else {
                    wanderer.Scatter();
                }
            }
        }
        else {
            StartCoroutine(WrongWanderersSelectedCoroutine());
        }
    }

    IEnumerator WrongWanderersSelectedCoroutine() {
        bCanClick = false;
        
        yield return new WaitForSeconds(spamPreventionTime);

        for (int i = selectedWanderers.Count - 1; i >= 0; i--) { 
            Wanderer wanderer = selectedWanderers[i];

            if (wanderer == match1 || wanderer == match2) continue;

            DeselectWanderer(wanderer);
        }

        bCanClick = true;
    
    }

    public bool IsWandererSelected(Wanderer wanderer) {
       return selectedWanderers.Contains(wanderer);
    }

    private void ForceWin() {
        for (int i = selectedWanderers.Count - 1; i >= 0; i--) {
            DeselectWanderer(selectedWanderers[i]);
        }

        SelectWanderer(match1);
        SelectWanderer(match2);

        CheckWin();
    }

}
