using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    
    [Header("SCRIPT REFERENCES")] // --------------------------------------------------------------------------------

    public Camera mainCam;
    
    [Tooltip("Reference to the car pointer's script component.")]
    public CarPointer carPointer;
    
    [Tooltip("Reference to the car's script component.")]
    public CarController car;

    [Tooltip("Reference to the flashing indicator on the GPS.")]
    public GameObject gpsIndicator;

    [Tooltip("Reference to the transcript log object.")]
    public TranscriptLog transcriptLog;

    private Toggle autoDialogueToggle;

    [Tooltip("Reference to the demo over screen.")]
    public GameObject demoOverScreen;

    [Header("DIALOGUE SCRIPT REFERENCES")] // ---------------------------------------------------------------------------------

    [SerializeField] private GameObject dialoguePivot;

    [SerializeField] private GameObject dialogueElement;

    public GameObject currentElement;

    public Canvas dialogueCanvas;

    [Tooltip("Reference to the name box.")]
    public GameObject nameBox;
    private TMP_Text nameBoxText;

    [Tooltip("Reference to the dialogue text element.")]
    public TMP_Text currentDialogueText;

    public Queue<GameObject> activeDialogueBlocks = new();
    [HideInInspector] public List<GameObject> activeDialogueTracker = new();

    [Header("CHOICES")] // ---------------------------------------------------------------------------------------------------

    [Tooltip("Reference to the invisible horizontal bar where choice button UI pops up.")]
    public GameObject choicesBar;

    [Tooltip("Reference to the prefab for a UI choice button.")]
    public GameObject choicePrefab;

    [Tooltip("List of all choice buttons in a choice branch.")]
    [SerializeField] private List<GameObject> choiceButtonsList;

    [Tooltip("Boolean flag; Checks if the current dialogue piece is a choice branch.")]
    public bool playingChoices = false;

    [Header("DIALOGUE VARIABLES")] // --------------------------------------------------------------------------------------------

    public string currentKnot;

    public Story currentStory;

    public StoryContainer CurrentStoryContainer { get; private set; }

    public bool playingDialogue = false;

    public bool stopDialogue = false;

    public bool waitForSkip = false;

    [Space(10)]

    [Tooltip("The maximum number of dialogue boxes allowed on screen at once.")]
    public int maxDialogueElements;

    [Tooltip("The amount of time to wait after typing a punctuation mark.")]
    public float punctuationWaitTime;

    [Tooltip("The amount of time to wait after finishing typing a sentence, before starting the next one.")]
    public float shortPauseTime;

    [Tooltip("How long it takes for static to fully build up at the beginning of a time loop.")]
    public float staticIncreaseTime;

    [Tooltip("The list of valid characters that count as 'punctuation'.")]
    public List<string> punctuationList = new();

    [Tooltip("Boolean flag; Checks if a sentence is being typed out.")]
    [HideInInspector] public bool typingSentence = false;

    [Tooltip("Static boolean flag; Checks whether automatic dialogue playing is enabled.")]
    public bool autoDialogue = false;

    public bool textLineDone = false;
    public bool voiceLineDone = false;

    private float expressionTimer = 0;
    private bool expressionTimerRunning = false;

    [HideInInspector] public bool startingExpressionDone = false;

    private List<int> hi = new();
    private List<int> hi2 = new();

    [Header("VFX")] // --------------------------------------------------------------------------------------------

    public TextEffects textEffects;

    public VolumeProfile volumeProfile;

    public float vignetteDefault;
    public float vignetteHallucinating;

    [Header("INK TAGS")] // -----------------------------------------------------------------------------------------------------

    public PassengerExpression currentExpression;
    public const string EMOTION_TAG = "emotion";

    public PassengerExpression currentPreExpression;
    public const string PRE_EMOTION_TAG = "pre_emotion";

    public const string NAME_TAG = "name";

    public float currentPauseLength = 0;
    public const string PAUSE_TAG = "pause";
    
    public bool spawnedDestination = false;
    public const string SPAWN_DEST_TAG = "spawn_dest";

    public GameObject currentGift;
    public const string GIFT_TAG = "gift";

    public bool isHallucinating = false;
    public const string HALLUCINATION_TAG = "hallucinating";

    public bool kickedOut = false;
    public const string KICKED_OUT_TAG = "kick_out";

    public bool timeLoop = false;
    public const string TIME_LOOP_TAG = "time_loop";

    public AudioClip currentVox = null;
    public const string VOICE_TAG = "voice";

    [Tooltip("Boolean flag; Checks whether the passenger is waiting for the player to select a destination.")]
    public bool waitForRouting = false;
    public const string GREETING_END_TAG = "end_greeting";

    public bool waitForDropoff = false;
    public const string DROPOFF_START_TAG = "dropoff";

    public const string REQUESTS_START_TAG = "requests_start";
    public const string REQUESTS_END_TAG = "requests_end";

    public const string MOOD_TAG = "mood";

    [Header("SAVABLE WORDS")]

    [SerializeField] private GameObject heldWordPrefab;

    public string currentlyHeldWord = "";

    public bool holdingOnWord = false;

    public List<GameObject> activeHeldWords = new();
    
    private void OnEnable() {
        GameStateManager.EOnDestinationSet += TalkAfterRouting;
        ClickableTextHandler.EOnLinkClick += ClickedWord;
    }

    private void OnDisable() {
        GameStateManager.EOnDestinationSet -= TalkAfterRouting;
        ClickableTextHandler.EOnLinkClick -= ClickedWord;
    }

    public void ClickedWord(string keyword) {
        holdingOnWord = true;
        currentlyHeldWord = keyword;

        // Spawn new held word
        Vector2 pos = GameStateManager.instance.ConvertToCanvasSpace(Input.mousePosition, dialogueCanvas);
        GameObject prefab = Instantiate(heldWordPrefab, pos, Quaternion.identity);

        // Assign canvas to new held word
        if (prefab.TryGetComponent<HeldWord>(out var wordScript)) {
            wordScript.canvas = dialogueCanvas;
        } else {
            throw new System.Exception("HeldWord script could not be found on object!");
        }

        // Make sure held word spawns with correct transforms
        Vector3 scale = prefab.transform.localScale;
        prefab.transform.SetParent(dialoguePivot.transform, true);
        prefab.transform.localRotation = Quaternion.identity;
        prefab.transform.localScale = scale;

        // Assign held word text
        prefab.GetComponentInChildren<TMP_Text>().text = currentlyHeldWord;
        activeHeldWords.Add(prefab);
    }

    public void BindExternalFunctions(Story story) {

        story.BindExternalFunction("UpdateCurrentKnot", (string knotName) =>
            UpdateCurrentKnot(knotName)
        );

        story.BindExternalFunction("GetRideNumber", () =>
            GetRideNumber()
        );

        story.BindExternalFunction("PostDropoff", () =>
            PostDropoff()
        );

        story.BindExternalFunction("GetMood", () =>
            GetMood()
        );
    }

    public void UpdateCurrentKnot(string knotName) {
        currentKnot = knotName;
    }

    public Path GetRideNumber() {
        int index = CarController.PassengersDrivenIDs.IndexOf(car.currentPassenger.id);

        return new("_" + CarController.PassengersDrivenRideNum[index]);
    }

    public float GetMood() {
        return GameStateManager.comfortManager.currentComfortability;
    }

    private void Awake()
    {
        if (volumeProfile.TryGet<Vignette>(out var vignette)) {
            vignette.intensity.value = vignetteDefault;
        }

        if (!mainCam) mainCam = Camera.main;

        // Reloads all dialogue files into Ink Stories upon loading the game
        foreach (Passenger passenger in car.passengerList.storyPassengers) {
            if (passenger.storyContainer != null) passenger.storyContainer.SetStories();
        }
    }

    private void Update() {

        activeDialogueTracker = activeDialogueBlocks.ToList();

        // Reset held word
        if (Input.GetMouseButtonUp(0) && holdingOnWord) {
            holdingOnWord = false;
            
            // Check if word is held above the notebook
            currentlyHeldWord = "";

            foreach (var word in activeHeldWords) {
                Destroy(word);
            }
            activeHeldWords.Clear();
        }

        // Auto/Non-auto dialogue
        if (autoDialogue) {
            waitForSkip = false;

            // If both text and voice line have finished, continue to the next dialogue piece
            if (textLineDone && voiceLineDone) {
                textLineDone = false;
                voiceLineDone = false;
                ContinueDialogue();
            }
        }

        if (waitForRouting) {
            // TODO: Add silly passenger quips about not moving
        }

        // Switch default expressions
        if (expressionTimerRunning && car.currentPassenger) {
            expressionTimer += Time.deltaTime;

            if (expressionTimer > UnityEngine.Random.Range(car.currentPassenger.minSwitchTime, car.currentPassenger.maxSwitchTime) && !typingSentence) {
                expressionTimerRunning = false;
                expressionTimer = 0;

                float rand = UnityEngine.Random.value;

                if (rand < 0.5f) {
                    SwitchExpression(car.currentPassenger.expressions[0]);
                } else {
                    expressionTimerRunning = true;
                }
            } else if (typingSentence) {
                expressionTimerRunning = false;
                expressionTimer = 0;
            }
        } else {
            expressionTimer = 0;
        }
    }

    // Assigns any missing script references
    public void FindReferences() {
        StopAllCoroutines();

        if (!car) {
            if (GameObject.FindGameObjectWithTag("Car").TryGetComponent<CarController>(out var carScript)) {
                car = carScript;
                carPointer = carScript.carPointer;
            } else {
                throw new System.Exception("Could not find CarController component!");
            }
        }

        if (!dialoguePivot) {
            dialoguePivot = GameObject.FindGameObjectWithTag("DialoguePivot");
            Debug.LogWarning("DialoguePivot is null! Reassigned.");
        }

        if (!gpsIndicator) {
            gpsIndicator = GameObject.FindGameObjectWithTag("GPSIndicator");
        }
        gpsIndicator.SetActive(false);

        if (!transcriptLog) {
            transcriptLog = GameObject.FindGameObjectWithTag("MainCanvas").GetComponentInChildren<TranscriptLog>();
        }

        /* if (!autoDialogueToggle) {
            autoDialogueToggle = GameObject.FindGameObjectWithTag("AutoDialogueToggle").GetComponent<Toggle>();
            autoDialogueToggle.onValueChanged.AddListener(delegate { SetAutoDialogue(autoDialogueToggle.isOn); } );
            autoDialogue = true;
            autoDialogueToggle.gameObject.SetActive(false);
        } */
        //autoDialogue = true;

        if (!nameBoxText) {

            if (!nameBox) {
                nameBox = GameObject.FindGameObjectWithTag("NameBox");
            }

            nameBox.SetActive(false);

            nameBoxText = nameBox.GetComponentInChildren<TMP_Text>();

            Debug.LogWarning("nameBox reference on: " + name + " is null! Reassigned.");
        }
    }

    public void TalkAfterRouting() {
        StartCoroutine(WaitAfterRouting());
    }

    private IEnumerator WaitAfterRouting() {

        // Wait for the passenger-specific time to start talking
        yield return new WaitForSeconds(car.currentPassenger.waitAfterRouteTime);

        // Allows passenger to continue dialogue
        waitForRouting = false;
    }
    
    // Starts a piece of dialogue
    public void StartDialogue(StoryContainer container) {
        playingDialogue = true;

        // Assigns any missing script references
        FindReferences();

        // Set the current dialogue
        CurrentStoryContainer = container;
        currentStory = container.story;

        stopDialogue = false;

        RemoveButtons();

        ContinueDialogue();
    }

    public void ContinueButton() {

        if (playingDialogue && !autoDialogue) {

            // Skip typing on click
            if (typingSentence) {
                currentDialogueText.maxVisibleCharacters = currentDialogueText.text.Length;
                typingSentence = false;
            } 
            // If sentence is typed out, play next sentence on click
            else if (waitForSkip) {
                waitForSkip = false;
                StopCoroutine(StartVoiceLine()); // Stops any voice line waiting
                StopCoroutine(WaitAfterRouting()); // Stops any text line waiting
                ContinueDialogue();
            }
        }
    }
    
    public void ContinueDialogue() {

        if (currentStory == null || stopDialogue) return;

        if (currentStory.canContinue) {
            StartCoroutine(TypeSentence(currentStory.Continue()));
        }
    }

    private void RemoveButtons() {

        // Remove existing buttons
        if (choiceButtonsList.Count > 0) {

            // For every active choice button—
            foreach (GameObject button in choiceButtonsList) {

                // Deletes the button
                Destroy(button);
            }

            // Clears choice buttons list
            choiceButtonsList.Clear();
        }
    }
    
    // Shows the choices at a choice branch
    public void ShowChoices() {

        if (currentStory.currentChoices.Count > 0) {
            playingChoices = true;

            RemoveButtons();

            // Enables the choices bar in preparation for displaying choice buttons
            choicesBar.SetActive(true);

            int index = 0;

            // For every choice in the current list—
            foreach (Choice choice in currentStory.currentChoices) {

                // Create button in UI
                TMP_Text choiceText = Instantiate(choicePrefab, choicesBar.transform).GetComponentInChildren<TMP_Text>();

                // Set UI button text to the choice text
                choiceText.text = choice.text;

                // Add button object to a list for future destruction
                choiceButtonsList.Add(choiceText.transform.parent.gameObject);

                int _index = ++index;

                // Add on-click button action (triggers certain dialogue)
                if (choiceText.transform.parent.TryGetComponent<Button>(out var button)) {
                    button.onClick.AddListener(() => ChoiceFollowUp(_index - 1));
                }
            }
        }
    }

    // Function that tells each choice what to do when clicked
    public void ChoiceFollowUp(int index) {

        // Switches from the "playing choices" state
        playingChoices = false;

        RemoveButtons();

        // Affect mood meter with appropriate change
        //GameStateManager.comfortManager.currentComfortability += choice.moodChange;

        /* // If choice response exists, play it
        if (choice.lines.Length > 0) {
            StartDialogue(choice);
        } 
        // Else, go to next dialogue piece
        else if (currentDialogue.nextDialogue) {
            StartDialogue(currentDialogue.nextDialogue);
        } */
        currentStory.ChooseChoiceIndex(index);
        ContinueDialogue();
    }

    // Read the current dialogue line's tags
    private void ReadTags(List<string> currentTags) {

        // Reset per-line variables
        currentPreExpression = null;
        currentPauseLength = 0;
        spawnedDestination = false;
        currentGift = null;
        kickedOut = false;
        timeLoop = false;
        currentVox = null;
        waitForRouting = false;
        waitForDropoff = false;
        voiceLineDone = true;

        // For every tag this line has—
        foreach (string unreadTag in currentTags) {

            // Split the tag into type and value
            List<string> splitTag = unreadTag.Split(":").ToList();

            string tag = splitTag[0].Trim().ToLower();
            string tagValue = 1 < splitTag.Count ? splitTag[1].Trim() : null;

            switch (tag.ToLower()) {

                case NAME_TAG:

                    car.currentPassenger.currentName = tagValue;

                    nameBoxText.text = tagValue;

                    // Only set name color if the name is the passenger's correct name
                    if (tagValue == car.currentPassenger.passengerName) {

                        // Sets name box color
                        nameBoxText.color = car.currentPassenger.nameColor;
                    }
                    
                    break;
                case EMOTION_TAG:
                    
                    // If the expression exists—
                    if (car.currentPassenger.expressions.Exists(x => x.name.Trim().ToLower() == tagValue)) {

                        // Grab the appropriate expression
                        currentExpression = car.currentPassenger.expressions.Find(x => x.name.Trim().ToLower() == tagValue);
                    } else {
                        throw new System.Exception("The requested expression: " + tagValue + " could not be found on this passenger!");
                    }
                    break;
                case PRE_EMOTION_TAG:

                    // If the expression exists—
                    if (car.currentPassenger.expressions.Exists(x => x.name.Trim().ToLower() == tagValue)) {

                        // Grab the appropriate expression
                        currentPreExpression = car.currentPassenger.expressions.Find(x => x.name.Trim().ToLower() == tagValue);
                    } else {
                        throw new System.Exception("The requested expression: " + tagValue + " could not be found on this passenger!");
                    }
                    break;
                case PAUSE_TAG:
                    currentPauseLength = float.Parse(tagValue);
                    break;
                case SPAWN_DEST_TAG:
                    spawnedDestination = true;
                    break;
                case GIFT_TAG:

                    if (int.TryParse(tagValue, out int giftIndex)) {
                        currentGift = car.currentPassenger.gifts[giftIndex];
                    } else {
                        throw new System.Exception("Unreadable gift index for passenger: " + car.currentPassenger.passengerName);
                    }
                    break;
                case HALLUCINATION_TAG:

                    switch (tagValue) {
                        case "off":
                            isHallucinating = false;
                            break;
                        case "on":
                            isHallucinating = true;
                            break;
                        default:
                            throw new System.Exception("Hallucination status can't be read!");
                    }
                    break;
                case KICKED_OUT_TAG:
                    kickedOut = true;
                    break;
                case TIME_LOOP_TAG:
                    timeLoop = true;
                    break;
                case VOICE_TAG:

                    // Finds the voice line with the right number
                    currentVox = car.currentPassenger.voicelines.Find(x => x.name.Split("_")[^1] == tagValue);
                    voiceLineDone = false;

                    break;
                case GREETING_END_TAG:
                    waitForRouting = true;
                    gpsIndicator.SetActive(true);
                    break;
                case DROPOFF_START_TAG:
                    waitForDropoff = true;
                    break;
                case REQUESTS_START_TAG:
                    GameStateManager.comfortManager.comfortabilityRunning = true;

                    car.moodMeterAnimator.SetTrigger("FadeIn");
                    break;
                case REQUESTS_END_TAG:
                    GameStateManager.comfortManager.comfortabilityRunning = false;

                    car.moodMeterAnimator.SetTrigger("FadeOut");
                    break;
                case MOOD_TAG:

                    if (float.TryParse(tagValue, out float mood)) {
                        GameStateManager.comfortManager.currentComfortability += mood;
                    } else {
                        throw new System.Exception("Unrecognized characters in MOOD_TAG!");
                    }
                    
                    break;
                default:
                    throw new System.Exception("Could not read this tag: " + tag + "!");
            }
        }
    }

    // Visually types the current sentence
    public IEnumerator TypeSentence(string line) {
        textLineDone = false;
        voiceLineDone = false;

        // Start fading previous dialogue line element
        if (activeDialogueBlocks.Count >= maxDialogueElements) {
            GameObject deadBlock = activeDialogueBlocks.Dequeue();

            if (deadBlock.TryGetComponent<DialogueUIElement>(out var deadScript)) {

                // Starts destruction of dialogue block
                deadScript.animator.SetTrigger("Out");
            }
        }

        // Don't type empty lines
        if (line.Length <= 1) {
            ContinueDialogue();
            yield break;
        }

        // Clear all text effects
        textEffects.ClearEffects();

        // Waits until GPS has been routed to continue
        while (waitForRouting) {
            yield return null;
        }

        // Parses through this dialogue line's tags
        ReadTags(currentStory.currentTags);

        // Waits until car has arrived at destination to continue
        while (waitForDropoff) {
            yield return null;
        }

        // Wait for the appropriate amount of long pause time before continuing with this line
        yield return new WaitForSeconds(currentPauseLength);

        // If there is a pre-expression to wait for, wait until it is done before typing out sentence
        if (currentPreExpression != null) {
            startingExpressionDone = false;

            SwitchExpression(currentPreExpression);
            
            while (!startingExpressionDone) {
                yield return null;
            }
        }

        // Prevents dialogue from continuing while transcript log is shown
        while (transcriptLog.gameObject.activeInHierarchy) {
            yield return null;
        }

        // Start voice line and short pause time
        if (currentVox) {
            StartCoroutine(StartVoiceLine());
        } else {
            voiceLineDone = true;
        }

        // Set whether destination is ready to spawn
        if (spawnedDestination) carPointer.readyToSpawnDest = true;

        // Play sound effect audio file if there is one
        /* if (currentLine.audioToPlay != null) {
            GameStateManager.audioManager.PlaySoundByFile(currentLine.audioToPlay);

            yield return new WaitForSeconds(currentLine.audioToPlay.length);
        } */

        // Spawn new dialogue element
        if (dialogueElement.TryGetComponent<DialogueUIElement>(out var blockScript)) {
            currentElement = (GameObject)blockScript.Create(dialogueElement, dialoguePivot.transform, car);
            currentElement.transform.SetAsFirstSibling();
        }

        activeDialogueBlocks.Enqueue(currentElement);

        DialogueUIElement dScript = null;

        // Set dialogue element text to the line's text
        if (currentElement.TryGetComponent<DialogueUIElement>(out var script)) {
            dScript = script;
            currentDialogueText = script.elementText;
        } else {
            throw new System.Exception("Could not find DialogueUIElement component on this dialogue element!");
        }

        // Set appropriate expression before talking
        if (currentExpression != null) {
            SwitchExpression(currentExpression);
        }

        // If the current sentence doesn't have any effects—
        if (!line.Contains("<glitch>") && !line.Contains("<wobble>")) {

            // Log appropriate name and message to transcript
            transcriptLog.LogText(line, car.currentPassenger.currentName);
        }

        // Spawn dashboard object if it exists
        if (currentGift) {

            if (car.dashboardGiftSpawns.Count > 0) {

                GameObject spawnedObject = null;

                // Iterate through all dashboard gift spawnpoints to check for first available
                for (int i = 0; i < car.dashboardGiftSpawns.Count; i++){

                    if (!car.dashboardGiftSpawns[i].taken) {
                        spawnedObject = Instantiate(currentGift, car.dashboardGiftSpawns[i].spawnPoint.transform);
                        car.dashboardGiftSpawns[i].taken = true;
                        break;
                    }
                }

                if (spawnedObject == null) {
                    Debug.LogWarning("Could not find available dashboard gift spawnpoint!");
                }
                currentGift = null;
            }

            /* switch (gift.location) {
                case GiftLocation.DASHBOARD:

                    if (car.dashboardGiftSpawns.Count > 0) {

                        GameObject spawnedObject = null;

                        // Iterate through all dashboard gift spawnpoints to check for first available
                        for (int i = 0; i < car.dashboardGiftSpawns.Count; i++){

                            if (!car.dashboardGiftSpawns[i].taken) {
                                spawnedObject = Instantiate(currentLine.dashboardObject, car.dashboardGiftSpawns[i].spawnPoint.transform);
                                car.dashboardGiftSpawns[i].taken = true;
                                break;
                            }
                        }

                        if (spawnedObject == null) {
                            Debug.LogWarning("Could not find available dashboard gift spawnpoint!");
                        }

                        break;
                    }
                    break;
                case GiftLocation.REARVIEW:

                    if (!car.rearviewGiftSpawn.taken) {
                        GameObject spawnedObject = Instantiate(currentLine.dashboardObject, car.rearviewGiftSpawn.spawnPoint.transform);
                        car.rearviewGiftSpawn.taken = true;
                        break;
                    } else {
                        Debug.LogWarning("Rearview gift spawn already taken!");
                        break;
                    }
            } */
        }
 
        string message = null;

        // Enable name box if hidden
        if (!nameBox.activeInHierarchy) {
            nameBox.SetActive(true);
        }

        nameBoxText.text = car.currentPassenger.currentName;

        // If name is already revealed, use revealed name
        if (car.currentPassenger.currentName == car.currentPassenger.passengerName) {

            // Sets name box color
            nameBoxText.color = car.currentPassenger.nameColor;
        } else {
            nameBoxText.color = Color.white;
        }

        // Separate everything but the name
        string concat = line;

        // String together message including name
        message = concat;

        Debug.Log(nameBoxText.text + ": " + message);

        // Indicates that a sentence is being typed out
        typingSentence = true;

        // Trigger vignette effect if line is hallucination
        if (isHallucinating) {

            if (volumeProfile.TryGet<Vignette>(out var vignette)) {

                StartCoroutine(FadeVignette(vignette, true));

            } else {
                throw new System.Exception("Could not get Vignette component on global volume!");
            }
        } else if (volumeProfile.TryGet<Vignette>(out var vignette)) {
            if (vignette.intensity.value != vignetteDefault) {
                StartCoroutine(FadeVignette(vignette, false));
            }
        }

        // Trigger speaking if line isn't silence or a hallucination
        if (line.Trim() != "..." && !isHallucinating) {
            car.currentPassenger.animator.SetBool("Speak", true);
        }

        // Clear dialogue box
        currentDialogueText.text = string.Empty;

        // --------------------------------------------------------- BEGIN PARSING ---------------------------------------------------------

        // Split dialogue into sections separated by < and >.
        // Even numbers are valid text, odd numbers are tags
        string[] subTexts = message.Split('<', '>');

        string displayText = "";

        // ------------------------------------------ REMOVE CUSTOM TAGS FROM DIALOGUE ------------------------------------------------

        // Removes custom tags from dialogue, but keeps standard rich text tags
        for (int i = 0; i < subTexts.Length; i++)
        {
            // Characters (even number)
            if (i % 2 == 0)
                displayText += subTexts[i];
            // Non-custom tags (odd number)
            else if (!isCustomTag(subTexts[i].Replace("/", "")))
                displayText += $"<{subTexts[i]}>";
        }
        
        char[] bruh = displayText.ToArray();

        // Check to see if tag is custom tag
        bool isCustomTag(string tag)
        {
            return tag.StartsWith("wobble") || tag.StartsWith("glitch");
        }

        // Set current dialogue to the dialogue without custom tags
        currentDialogueText.text = displayText;

        // Hide all dialogue visibility
        currentDialogueText.maxVisibleCharacters = 0;

        // ------------------------------------------ PARSE TO FIND CUSTOM TAGS AND ACTIVATE THEM --------------------------------------------
        
        int spaceCounter = 0;
        int characterTracker = 0;
        int tagTracker = 0;
        Queue<int> tagLengths = new();
        Queue<int> richTagLengths = new();

        // For every subsection split—
        for (int element = 0; element < subTexts.Length; element++) {

            // If the subsection is a tag—
            if (element % 2 == 1) {

                switch (isCustomTag(subTexts[element].Replace("/", ""))) {

                    // If the subsection is a default rich text tag—
                    case false:

                        // Mark this section's length as irrelevant
                        richTagLengths.Enqueue(tagTracker);

                        // Increment tag tracker by the length of the tag
                        tagTracker += subTexts[element].Length + 2;
                        break;
                    
                    // If the subsection is a custom tag—
                    case true:

                        // We don't care about the custom closing tag
                        /* if (subTexts[element].Contains("/")) {
                            break;
                        } */

                        // Mark this section as starting at the correct index
                        tagLengths.Enqueue(tagTracker);
                        break;
                }
            }
        }

        hi = new(tagLengths);
        hi2 = new(richTagLengths);

        int dequeued = 0;
        int richDequeued = 0;

        int spaces = 0;

        // ------------------------------------------------------------------------- REALLY BADLY OPTIMIZED CODE --------------------

        // For each subsection split—
        for (int sectionNum = 0; sectionNum < subTexts.Length; sectionNum++) {

            // Subsection split is an odd number, therefore is a tag
            if (sectionNum % 2 == 1)
            {
                // Get the amount of spaces in the text with the effect
                spaces = subTexts[sectionNum + 1].Count(x => x.ToString() == " ");

                // If this tag is a standard rich-text tag, include it in character counter
                if (!isCustomTag(subTexts[sectionNum].Replace("/", ""))) {
                    characterTracker += subTexts[sectionNum].Length + 2;
                    richDequeued = richTagLengths.Dequeue();
                }

                // If this tag is the START of a custom tag—
                if (isCustomTag(subTexts[sectionNum].Replace(" ", ""))) {
                    dequeued = tagLengths.Dequeue();
                    tagLengths.Dequeue(); // Remove closing tag as well, but don't use it
                }

                // If this tag is the START of a standard rich-text tag, add its text section to the list
                if (!isCustomTag(subTexts[sectionNum].Replace(" ", "")) && !subTexts[sectionNum].Contains("/")) {

                    int start = (characterTracker - spaceCounter - (richDequeued + subTexts[sectionNum].Length + 2)) * 4;
                    int end = ((characterTracker + subTexts[sectionNum + 1].Length - (spaceCounter + spaces) - (richDequeued + subTexts[sectionNum].Length + 2)) * 4) - 1;

                    textEffects.AddRichTextRange(new(start, end));
                }

                // If this tag is the opening tag of any kind—
                if (!subTexts[sectionNum].Contains("/")) {

                    // If the previous section exists, is not a tag, and is not just a space—
                    if (sectionNum - 1 >= 0 && sectionNum - 1 % 2 != 1 && subTexts[sectionNum - 1].Replace(" ", "").Length > 0) {

                        int start = (characterTracker - spaceCounter - dequeued) * 4;
                        int end = ((characterTracker - (spaceCounter + spaces) - dequeued) * 4) - 1;
                        
                        textEffects.AddRegularTextRange(new(start, end));
                    }
                }
            }
            // Subsection split is an even number, therefore made up of words
            else
            {

                // For each character in this subsection
                for (int charNum = 0; charNum < subTexts[sectionNum].Length; ++charNum) {

                    // The current iterated character
                    string character = subTexts[sectionNum][charNum].ToString();

                    // If the character is a space, increment the number of spaces
                    if (character == " ") {
                        spaceCounter++;
                    }

                    characterTracker++;
                    /* onTextReveal.Invoke(subTexts[subCounter][visibleCounter]); */
                }
            }
        }

        // RESET

        spaceCounter = 0;
        characterTracker = 0;
        tagTracker = 0;

        tagLengths = new(hi);
        richTagLengths = new(hi2);

        dequeued = 0;
        richDequeued = 0;

        spaces = 0;

        // ------------------------------------------------------------------------- TRIGGER EFFECTS --------------------

        // For each subsection split—
        for (int sectionNum = 0; sectionNum < subTexts.Length; sectionNum++) {

            // Subsection split is an odd number, therefore is a tag
            if (sectionNum % 2 == 1)
            {
                // Get the amount of spaces in the text with the effect
                spaces = subTexts[sectionNum + 1].Count(x => x.ToString() == " ");

                // If this tag is a standard rich-text tag, include it in character counter
                if (!isCustomTag(subTexts[sectionNum].Replace("/", ""))) {
                    characterTracker += subTexts[sectionNum].Length + 2;
                    richDequeued = richTagLengths.Dequeue();
                }

                // If this tag is the START of a custom tag—
                if (isCustomTag(subTexts[sectionNum].Replace(" ", ""))) {
                    dequeued = tagLengths.Dequeue();
                    tagLengths.Dequeue(); // Remove closing tag as well, but don't use it
                }

                // Trigger the tag effect
                yield return EvaluateTag(subTexts[sectionNum].Replace(" ", ""), characterTracker, characterTracker + subTexts[sectionNum + 1].Length, spaceCounter, spaceCounter + spaces, dequeued);

            }
            // Subsection split is an even number, therefore made up of words
            else
            {

                // For each character in this subsection
                for (int charNum = 0; charNum < subTexts[sectionNum].Length; ++charNum) {

                    // The current iterated character
                    string character = subTexts[sectionNum][charNum].ToString();

                    // If the character is a space, increment the number of spaces
                    if (character == " ") {
                        spaceCounter++;
                    }

                    characterTracker++;
                    /* onTextReveal.Invoke(subTexts[subCounter][visibleCounter]); */
                }
            }
        }
        yield return null;

        // ----------------------------------------------------- REVEAL TEXT  ---------------------------------------------------------

        // For every character in ONLY THE MESSAGE WITHOUT THE NAME
        foreach (char letter in currentDialogueText.text) {

            if (typingSentence) {

                // Types the character and adds it to the current sentence display
                currentDialogueText.maxVisibleCharacters++;

                // Wait after typing any sort of punctuation
                if (punctuationList.Contains(currentDialogueText.text.ToArray()[currentDialogueText.maxVisibleCharacters - 1].ToString())) {
                    yield return new WaitForSeconds(punctuationWaitTime);
                }

                // Waits for the typing speed time
                yield return new WaitForSeconds(car.currentPassenger.textCPS / CarController.TextSpeedMult);

            } else {
                Debug.Log("Skipped typing!");
                break;
            }
        }

        typingSentence = false;

        if (dScript) {
            dScript.finished = true;
        }

        // Reset speaking boolean in Animator
        car.currentPassenger.animator.SetBool("Speak", false);

        // Switches back to regular expression after done talking
        if (currentExpression != null) {
            SwitchExpression(currentExpression);
        }

        // Start time loop
        if (timeLoop) {
            StartCoroutine(StartTimeLoop());
            yield break;
        }

        // Initiate an early dropoff if needed
        if (kickedOut) {
            PostDropoff();

            GameStateManager.EOnPassengerDropoff?.Invoke();
            yield break;
        }

        // Display any choices for this dialogue line
        if (currentStory.currentChoices.Count > 0) {

            // Remove any voice lines
            voiceLineDone = false;
            currentVox = null;

            ShowChoices();
        }
        // Play the next dialogue line only if auto-dialogue is enabled, there isn't currently a starting expression playing, and
        // there is no voice line to be played. 
        else if (autoDialogue && !currentVox && !stopDialogue) {

            // Starts countdown to fade dialogue away
            StartCoroutine(WaitBeforeNextSentence());
            yield break;
        }

        textLineDone = true;
        waitForSkip = true;
    }

    private WaitForSeconds EvaluateTag(string tag, int start, int end, int spacesBefore, int spacesAfter, int tagLengthsBefore)
    {
        if (tag.Length > 0)
        {
            /* if (tag.StartsWith("speed="))
            {
                speed = float.Parse(tag.Split('=')[1]);
            }
            else if (tag.StartsWith("pause="))
            {
                return new WaitForSeconds(float.Parse(tag.Split('=')[1]));
            }
            else if (tag.StartsWith("emotion="))
            {
                onEmotionChange.Invoke((Emotion)System.Enum.Parse(typeof(Emotion), tag.Split('=')[1]));
            }
            else  */
            if (tag == "wobble"/* tag.StartsWith("action=") */) {
                //Debug.Log(start + " | " + end + " | " + spacesBefore + " | " + spacesAfter);
                //string action = tag.Split('=')[1];

                //onAction.Invoke(tag.Split('=')[1]);
                //SetAction(action);

                Debug.Log("Wobble detected!");
                textEffects.StartWobble(start, end, spacesBefore, spacesAfter, tagLengthsBefore);
            } else if (tag == "glitch") {

                Debug.Log("Glitch detected!");
                textEffects.StartGlitch(currentDialogueText.text, start, end, spacesBefore, spacesAfter, tagLengthsBefore);
            }
        }
        return null;
    }

    private IEnumerator FadeVignette(Vignette vignette, bool value) {

        switch (value) {

            case true:
                
                float Tcurrent = vignette.intensity.value;

                while (Tcurrent < vignetteHallucinating) {
                    Tcurrent += 0.01f;

                    vignette.intensity.value = Tcurrent;
                    yield return new WaitForSeconds(0.005f);
                }
                break;
            case false:
                float Fcurrent = vignette.intensity.value;

                while (Fcurrent > vignetteDefault) {
                    Fcurrent -= 0.01f;

                    vignette.intensity.value = Fcurrent;
                    yield return new WaitForSeconds(0.005f);
                }
                break;
        }
    }

    private IEnumerator StartTimeLoop() {

        // Lock radio channel knob to static
        foreach (var obj in car.radio.radioChannels.objectsToOutline) {
            obj.layer = 0;
        }

        // Turn dial to static
        car.radio.SetRadioStatic();
        car.radio.radioChannels.enabled = false;

        // Slowly increase static volume for a certain amount of time
        float timer = 0;
        while (timer < staticIncreaseTime) {
            timer += Time.deltaTime;

            if (car.radio.audioSource.volume < 1) {
                car.radio.audioSource.volume += Time.deltaTime / 4;
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }

        // Replace with glitched sprite
        car.currentPassenger.animator.SetTrigger("Glitch");

        yield return new WaitForSeconds(0.75f);

        // Play glitch effect and swap passengers
        GlitchPassenger();

        // Get the next passenger in the queue
        Passenger passenger = car.passengerList.storyPassengers[0];

        // Spawn passenger at stop
        GameObject character = Instantiate(passenger.gameObject, car.passengerSeats[0].transform.position, Quaternion.identity);

        // Exhaust / remove passenger from the queue
        car.passengerList.ExhaustPassenger(passenger, PassengerRarity.STORY);

        passenger.storyContainer.SetStories();

        // Pick up passenger in the car
        car.PickUpPassenger(character);
    }

    private IEnumerator StartVoiceLine() {
        voiceLineDone = false;

        AudioClip playedVoiceline = currentVox;

        // Plays voice line
        GameStateManager.audioManager.PlayVoiceLine(currentVox, car.currentPassenger.gameObject);

        // Waits until after voice line is done, and the next sentence is ready to be said
        yield return new WaitForSeconds(currentVox.length + shortPauseTime);

        // Checks if the player has skipped this voiceline while the coroutine is still in session
        if (currentVox != playedVoiceline) {
            yield break;
        }

        // Remove completed voice line
        currentVox = null;

        // Sets boolean flag to done
        voiceLineDone = true;
    }

    public IEnumerator WaitBeforeNextSentence() {

        // Waits for the passenger's hold-dialogue-on-screen time
        yield return new WaitForSeconds(shortPauseTime);

        // Displays next sentence if available
        textLineDone = true;
    }

    // Called AFTER a passenger's dropoff dialogue has concluded
    public void PostDropoff() {
        Debug.Log("Finished all dialogue!");

        // Removes second Quinton ride 2 if Quinton ride 1 was completed
        if (car.currentPassenger.id == 5) {
            Passenger quinton = car.passengerList.storyPassengers.Find(x => x.id == 5);
            car.passengerList.storyPassengers.Remove(quinton);
        }

        // Prevent ContinueDialogue() calls
        stopDialogue = true;
        playingDialogue = false;
        waitForSkip = false;

        // Gets the index number of the current passenger
        int index = CarController.PassengersDrivenIDs.IndexOf(car.currentPassenger.id);

        // Hide the name box
        nameBox.SetActive(false);

        // Clear current dialogue
        currentStory.ResetState();
        CurrentStoryContainer = null;
        currentStory = null;
        currentKnot = "";

        // Unparent passenger from car
        car.currentPassenger.transform.parent = null;

        // Reset dropoff dialogue status
        //car.currentPassenger.salutes[CarController.PassengersDrivenRideNum[index] - 1].playingSalute = false;

        // Set passenger position to the destination stop
        car.currentPassenger.transform.position = car.dropoffPosition.transform.position;

        StartCoroutine(DeletePassenger(car.currentPassenger.gameObject));

        // Clear current passenger
        car.currentPassenger = null;

        carPointer.readyToSpawnDest = false;

        // Allows car to continue driving
        car.arrivedAtDest = false;

        // If the player has completed the last ride of the day—
        if (car.currentRideNum >= car.totalRideNum) {

            Debug.Log("Completed day's shift!");
            // TODO: Put after-day summary here!

        }
    }

    public void ResetDialogue() {
        playingDialogue = false;
        stopDialogue = true;
        typingSentence = false;
        waitForSkip = false;

        if (currentElement) {
            Destroy(currentElement);
        }
        
        nameBox.SetActive(false);

        // Clear current dialogue
        if (currentStory) {
            currentStory.ResetState();
            currentStory = null;
        }
        if (CurrentStoryContainer) CurrentStoryContainer = null;
        currentKnot = "";
    }

    private IEnumerator DeletePassenger(GameObject passenger) {

        yield return new WaitForSeconds(10f);

        Destroy(passenger);
    }

    public void GlitchPassenger() {
        Debug.Log("Static switched passenger!");

        // Prevent ContinueDialogue() calls
        stopDialogue = true;

        // Hide the name box
        nameBox.SetActive(false);

        // Clear current dialogue
        currentStory.ResetState();
        CurrentStoryContainer = null;
        currentStory = null;
        currentKnot = "";

        // Unparent passenger from car
        car.currentPassenger.transform.parent = null;

        // Set passenger position to the destination stop
        GameObject passenger = car.currentPassenger.gameObject;

        // Clear current passenger
        car.currentPassenger = null;
        Destroy(passenger);

        carPointer.readyToSpawnDest = false;

        // Allows car to continue driving
        car.arrivedAtDest = false;
    }

    public void SwitchExpression(PassengerExpression expression) {

        expressionTimerRunning = expression.runExpressionTimer switch
        {
            true => true,
            false => false
        };

        car.currentPassenger.animator.SetTrigger(expression.animatorTrigger.ToString());
    }
}