using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public enum GameMode {
    Normal = 0,
    SpeedDating = 1,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    [SerializeField] private Wanderer wandererPrefab;

    [SerializeField] private List<Sprite> sprites = new List<Sprite>();

    [HideInInspector] public List<Wanderer> wandererList = new List<Wanderer>();
    public Wanderer match1;
    public Wanderer match2;

    public Image match1Sprite;
    public Image match2Sprite;
    
    [SerializeField] private bool bCreateWanderers = true;
    [SerializeField] private GameMode gameMode = GameMode.Normal;
    [Header("Normal Dating")]
    [SerializeField] private float normalTimer = 60.0f;

    [Header("Speed Dating")]
    [SerializeField] private float speedTimer = 120.0f;
    [SerializeField] private float speedDateCooldown = 0.75f;

    private int numMatches = 0;

    private List<Wanderer> selectedWanderers = new List<Wanderer>();

    private float gameStartTimer = 2.4f;
    //private bool gameStartTimerOn = true;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public float currentTime;
    public bool timerOn = false;
    public MenuManager menuManager;
    public TextMeshProUGUI matchNumber;

    private Animator animator;

    private bool bCanClick = true;

    [SerializeField] private float spamPreventionTime = 1f;
    [SerializeField] private Transform match1EndPosition;
    [SerializeField] private Transform match2EndPosition;

    private bool bMatchesFound = false;

    [Header("Audio")]
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip matchMadeSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

    private bool bGameOver = false;

    private void Start() {
        if (Instance == null) Instance = this;

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (backgroundAudioSource == null) Debug.LogWarning("Background music not assigned");

        if (bCreateWanderers)
        {
            CreateWanderers();
            CreateMatch();
        }

        if (gameMode == GameMode.Normal) {
            // Normal Dating
            currentTime = normalTimer;

        }
        else {
            // Speed Dating
            currentTime = speedTimer;

        }
    }

    private void CreateWanderers() {
        foreach (Sprite sprite in sprites) {
            Vector2 spawnPosition;

            // Find a valid spawn position that's NOT in UI or out of bounds
            do {
                float x = Random.Range(
                    -wandererPrefab.areaSize.x / 2 + wandererPrefab.areaOffset.x,
                    wandererPrefab.areaSize.x / 2 + wandererPrefab.areaOffset.x
                );
                float y = Random.Range(
                    -wandererPrefab.areaSize.y / 2 + wandererPrefab.areaOffset.y,
                    wandererPrefab.areaSize.y / 2 + wandererPrefab.areaOffset.y
                );
                spawnPosition = new Vector2(x, y);
            } while (IsInsideBlockedArea(spawnPosition));

            Wanderer newWanderer = Instantiate(wandererPrefab, spawnPosition, Quaternion.identity, gameObject.transform);
            newWanderer.spriteRenderer.sprite = sprite;

            animator = newWanderer.GetComponent<Animator>();
            animator.Play(0, -1, Random.Range(0f, 1f)); // Randomize start offset

            wandererList.Add(newWanderer);
        }
    }

    private bool IsInsideBlockedArea(Vector2 position) {
        return IsInsideArea(position, wandererPrefab.matchAreaOffset, wandererPrefab.matchAreaSize) ||
               IsInsideArea(position, wandererPrefab.timerAreaOffset, wandererPrefab.timerAreaSize);
    }

    private bool IsInsideArea(Vector2 position, Vector2 areaOffset, Vector2 areaSize) {
        return position.x > areaOffset.x - areaSize.x / 2 &&
               position.x < areaOffset.x + areaSize.x / 2 &&
               position.y > areaOffset.y - areaSize.y / 2 &&
               position.y < areaOffset.y + areaSize.y / 2;
    }

    private void CreateMatch()
    {
        int randomIndex1 = Random.Range(0, wandererList.Count);
        match1Sprite.sprite = wandererList[randomIndex1].spriteRenderer.sprite;
        match1 = wandererList[randomIndex1];

        // Ensure no matches of same 2
        int randomIndex2 = Random.Range(0, wandererList.Count);
        while (randomIndex1 == randomIndex2) {
            randomIndex2 = Random.Range(0, wandererList.Count);
        }
        match2Sprite.sprite = wandererList[randomIndex2].spriteRenderer.sprite;
        match2 = wandererList[randomIndex2];
    }

    private void Update() {
        if (bGameOver) return;

        if (bMatchesFound) {
            if (match1.bAtTarget && match2.bAtTarget) {
                match1.GetComponent<Animator>().SetBool("Match", true);
                match2.GetComponent<Animator>().SetBool("Match", true);
                StartCoroutine(WinScreen());
            }

            return;
        }

        // REMOVE THIS
        if (Input.GetKeyDown(KeyCode.G)) ForceWin();

        if (Input.GetMouseButtonDown(0)) {
            HandleClick();
        }

        if (gameStartTimer > 0)
        {
            gameStartTimer -= Time.deltaTime;
            if (gameStartTimer <= 0) {
                backgroundAudioSource.Play();
            }
        }
        else {
            timerOn = true;
        }

        timerText.text = currentTime.ToString("F1");
        if (timerOn && currentTime > 0)
        {
            if (currentTime < 10) { timerText.color = Color.red; }
            currentTime -= Time.deltaTime;
        }
        else if (currentTime <= 0) {
            // Time out, game OVER
            foreach (Wanderer wanderer in wandererList) {
                if (wanderer == match1 || wanderer == match2) {
                    wanderer.SetMovement(false);
                    continue;
                }
                if (IsWandererSelected(wanderer)) DeselectWanderer(wanderer);

                wanderer.Scatter();
            }

            if (gameMode == GameMode.SpeedDating) {
                StartCoroutine(SpeedDatingEndScreen());
               
            }
            else {
                StartCoroutine(LossScreen());
            }

        }
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
            // Match Made
            PlaySoundClip(matchMadeSound);

            if (gameMode == GameMode.Normal) {
                // Game Won
                bMatchesFound = true;
                bCanClick = false;

                foreach (Wanderer wanderer in wandererList) {
                    if (wanderer == match1) {
                        wanderer.SetGoToPosition(match1EndPosition.transform.position);
                    }
                    else if (wanderer == match2) {
                        wanderer.SetGoToPosition(match2EndPosition.transform.position);
                    }
                    else {
                        wanderer.Scatter();
                    }
                }
            }
            else if (gameMode == GameMode.SpeedDating) {
                StartCoroutine(SpeedMatchCreated());
            }
        }
        else {
            StartCoroutine(WrongWanderersSelectedCoroutine());
        }
    }

    IEnumerator SpeedMatchCreated() {
        numMatches++;
        match1.GetComponent<Animator>().SetBool("Match", true);
        match2.GetComponent<Animator>().SetBool("Match", true);
        // Dom TODO: +1 anim to score?
        
        yield return new WaitForSeconds(speedDateCooldown);

        match1.GetComponent<Animator>().SetBool("Match", false);
        match2.GetComponent<Animator>().SetBool("Match", false);
        ClearAllSelectedWanderers();

        match1Sprite.GetComponent<Animator>().SetTrigger("Switch");
        match2Sprite.GetComponent<Animator>().SetTrigger("Switch");
        CreateMatch();

    }

    IEnumerator WrongWanderersSelectedCoroutine() {
        bCanClick = false;
        
        foreach (Wanderer wanderer in selectedWanderers) {
            if (wanderer == match1 || wanderer == match2) continue;

            wanderer.GetComponent<Animator>().SetTrigger("Rejected");

        }

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

    private void ClearAllSelectedWanderers() {
        for (int i = selectedWanderers.Count - 1; i >= 0; i--) {
            DeselectWanderer(selectedWanderers[i]);
        }
    }

    private void ForceWin() {
        ClearAllSelectedWanderers();

        SelectWanderer(match1);
        SelectWanderer(match2);

        CheckWin();
    }

    IEnumerator WinScreen()
    {
        PauseBackgroundMusic();
        bGameOver = true;
        yield return new WaitForSeconds(1f);
        menuManager.WinMenu();
        PlaySoundClip(winSound);
    }

    IEnumerator LossScreen()
    {
        PauseBackgroundMusic();
        bGameOver = true;
        yield return new WaitForSeconds(2f);
        menuManager.LossMenu();
        PlaySoundClip(loseSound);
    }

    IEnumerator SpeedDatingEndScreen() {
        PauseBackgroundMusic();
        bGameOver = true;
        yield return new WaitForSeconds(2f);
        matchNumber.text = numMatches.ToString();
        menuManager.SpeedDatingMenu();
        PlaySoundClip(winSound);
    }

    public void PlayBackgroundMusic() {
        if (backgroundAudioSource) {
            backgroundAudioSource.Play();
        }
    }

    public void PauseBackgroundMusic() {
        if (backgroundAudioSource) {
            backgroundAudioSource.Pause();
        }
    }
    private void PlaySoundClip(AudioClip clipToPlay) {
        if (audioSource != null && clipToPlay != null) audioSource.PlayOneShot(clipToPlay);
    }
}
