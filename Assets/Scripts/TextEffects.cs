using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct Range {

    public int start;
    public int end;

    public Range(int item1, int item2) {
        start = item1;
        end = item2;
    }
}

public class TextEffects : MonoBehaviour
{
    [Header("GENERAL")] // ----------------------------------------------------------------------------
    public TMP_SubMeshUI childText;

    public List<Range> lengths = new();
    private Queue<Range> qLengths = new();
    public List<Range> regLengths = new();
    private Queue<Range> qRegLengths = new();
    public List<Range> richLengths = new();
    private Queue<Range> qRichLengths = new();

    private Mesh mesh;
    public Vector3[] totalVertices;
    public Color[] totalColors;
    public Vector3[] vertices;
    public Color[] colors;

    private Vector3[] convertedVerts;
    private Color[] convertedColors;
    private Vector3[] convertedChildVerts;
    private Color[] convertedChildColors;

    public Vector3[] childVertices;
    public Color[] childColors;

    [Header("WOBBLE")]

    public bool wobbleOn = false;

    public float wobbleSpeedX = 1.1f;
    public float wobbleSpeedY = 0.8f;

    public List<Range> wobbleRanges = new();

    [Header("GLITCH")]

    public bool glitchOn = false;

    public string possibleReplacements;

    public float colorSpeedMin;
    public float colorSpeedMax;

    public float flickerSpeedMin;
    public float flickerSpeedMax;

    public float normalHoldSpeedMin;
    public float normalHoldSpeedMax;

    public float glitchSpeedX;
    public float glitchSpeedY;

    public List<Range> glitchRanges = new();
    public List<Range> glitchPureRanges = new();

    public float flickerChance = 0.5f;

    public string glitchBackup;

    public Gradient gradient;

    private bool on = false;

    private void Update()
    {
        // Activates text effects if any are on
        if (glitchOn || wobbleOn) {
            on = true;
        } else {
            on = false;
        }
    }

    private void LateUpdate()
    {

        // If there is no dialogue being spoken, don't do anything
        if (!GameStateManager.dialogueManager.currentDialogueText) {
            return;
        }

        // Glitch
        if (on) {

            GameStateManager.dialogueManager.currentDialogueText.ForceMeshUpdate();
            mesh = GameStateManager.dialogueManager.currentDialogueText.mesh;
            vertices = mesh.vertices;
            colors = mesh.colors;

            UpdateMeshes(childText);

            if (glitchOn) {

                foreach (var range in glitchPureRanges) {

                    int parseStart = range.start;
                    int parseEnd = range.end;

                    // Applies glitch effect
                    for (int i = parseStart; i <= parseEnd; i += 4) {

                        float colorSpeed = UnityEngine.Random.Range(colorSpeedMin, colorSpeedMax);

                        totalColors[i] = gradient.Evaluate(Mathf.Repeat(Time.time + totalVertices[i].x * colorSpeed, 1f));
                        totalColors[i + 1] = gradient.Evaluate(Mathf.Repeat(Time.time + totalVertices[i + 1].x * colorSpeed, 1f));
                        totalColors[i + 2] = gradient.Evaluate(Mathf.Repeat(Time.time + totalVertices[i + 2].x * colorSpeed, 1f));
                        totalColors[i + 3] = gradient.Evaluate(Mathf.Repeat(Time.time + totalVertices[i + 3].x * colorSpeed, 1f));

                        Vector3 offset = Wobble(Time.fixedDeltaTime + i, glitchSpeedX, glitchSpeedY);

                        totalVertices[i] += offset;
                        totalVertices[i + 1] += offset;
                        totalVertices[i + 2] += offset;
                        totalVertices[i + 3] += offset;
                    }
                }
            }

            if (wobbleOn) {

                foreach (var range in wobbleRanges) {

                    int pStart = range.start;
                    int pEnd = range.end;

                    for (int i = pStart; i <= pEnd; i+=4) {

                        Vector3 offset = Wobble(Time.fixedDeltaTime + UnityEngine.Random.value * 2, wobbleSpeedX, wobbleSpeedY);

                        totalVertices[i] += offset;
                        totalVertices[i + 1] += offset;
                        totalVertices[i + 2] += offset;
                        totalVertices[i + 3] += offset;
                    }
                }
            }

            RevertMeshes(childText);

            mesh.colors = convertedColors;
            mesh.vertices = convertedVerts;

            GameStateManager.dialogueManager.currentDialogueText.canvasRenderer.SetMesh(mesh);
            
            if (childText) {
                childText.mesh.colors = convertedChildColors;
                childText.mesh.vertices = convertedChildVerts;
                
                childText.canvasRenderer.SetMesh(childText.mesh);
            }
        }
    }

    public void UpdateMeshes(TMP_SubMeshUI subMeshUI) {

        Mesh subMesh = null;

        if (subMeshUI != null) {
            subMesh = subMeshUI.mesh;
        }

        // If there are submeshes found (italics, bold), update child meshes
        if (subMesh) {
            childVertices = subMesh.vertices;
            childColors = subMesh.colors;
            totalVertices = new Vector3[childVertices.Length + vertices.Length];
            totalColors = new Color[childColors.Length + colors.Length];

            // Initializes regular vertices and colors
            int length = 0;
            for (int pair = 0; pair < lengths.Count; pair++) {

                // If this is the first pair range—
                if (pair == 0) {

                    // If this range doesn't start at index 0—
                    if (lengths[pair].start != 0) {

                        // Set the length of the regular vertices array to the length of the range
                        length = lengths[pair].start - 1;
                    } else {
                        length = 0;
                    }
                } else {
                    length += lengths[pair].start - lengths[pair - 1].end;
                }
            }

            qLengths = new(lengths);
            qRegLengths = new(regLengths);
            qRichLengths = new(richLengths);

            int childCounter = 0;
            int totalCounter = 0;
            int nonChildCounter = 0;

            // Loop through all the vertices—
            while (totalCounter < totalVertices.Length) {
                //Debug.Log("T: " + totalCounter + " | NC: " + nonChildCounter + " | C: " + childCounter);

                // If the current vertex is part of an effect—
                if (qLengths.Count > 0 && totalCounter >= qLengths.First().start && totalCounter <= qLengths.First().end) {

                    // Add 
                    for (int v = qLengths.First().start; v <= qLengths.First().end; v++) {
                        totalVertices[totalCounter] = vertices[nonChildCounter];
                        totalColors[totalCounter] = colors[nonChildCounter];

                        nonChildCounter++;
                        totalCounter++;
                    }

                    qLengths.Dequeue();
                    continue;
                } else if (qRichLengths.Count > 0 && totalCounter >= qRichLengths.First().start && totalCounter <= qRichLengths.First().end) {

                    for (int v = qRichLengths.First().start; v <= qRichLengths.First().end; v++) {
                        totalVertices[totalCounter] = childVertices[childCounter];
                        totalColors[totalCounter] = childColors[childCounter];
                        
                        childCounter++;
                        totalCounter++;
                    }

                    qRichLengths.Dequeue();
                    continue;
                } else if (totalCounter < (vertices.Length - 4 + childVertices.Length - 4)) {
                    totalVertices[totalCounter] = vertices[nonChildCounter];
                    totalColors[totalCounter] = colors[nonChildCounter];

                    nonChildCounter++;
                    totalCounter++;
                    continue;
                }
                totalCounter++;
                //Debug.Log(totalCounter + " | " + glitchCounter + " | " + childCounter);
            }

        } else {
            totalVertices = vertices;
            totalColors = colors;
        }
    }

    public void RevertMeshes(TMP_SubMeshUI subMeshUI) {

        convertedVerts = new Vector3[vertices.Length];
        convertedColors = new Color[colors.Length];

        Mesh subMesh = null;

        if (subMeshUI != null) {
            subMesh = subMeshUI.mesh;
        }

        if (subMesh) {

            convertedChildVerts = new Vector3[childVertices.Length];
            convertedChildColors = new Color[childColors.Length];

            qLengths = new(lengths);
            qRegLengths = new(regLengths);
            qRichLengths = new(richLengths);

            int childCounter = 0;
            int totalCounter = 0;
            int nonChildCounter = 0;

            // Loop through all the child vertices—
            while (totalCounter < totalVertices.Length) {
                //Debug.Log("REVERT - T: " + totalCounter + " | NC: " + nonChildCounter + " | C: " + childCounter);

                // If the current vertex is part of an effect—
                if (qLengths.Count > 0 && totalCounter >= qLengths.First().start && totalCounter <= qLengths.First().end) {

                    for (int v = qLengths.First().start; v <= qLengths.First().end; v++) {
                        convertedVerts[nonChildCounter] = totalVertices[totalCounter];
                        convertedColors[nonChildCounter] = totalColors[totalCounter];
                        
                        nonChildCounter++;
                        totalCounter++;
                    }

                    qLengths.Dequeue();
                    continue;
                } else if (qRichLengths.Count > 0 && totalCounter >= qRichLengths.First().start && totalCounter <= qRichLengths.First().end) {

                    for (int v = qRichLengths.First().start; v <= qRichLengths.First().end; v++) {
                        convertedChildVerts[childCounter] = totalVertices[totalCounter];
                        convertedChildColors[childCounter] = totalColors[totalCounter];
                        
                        childCounter++;
                        totalCounter++;
                    }

                    qRichLengths.Dequeue();
                    continue;
                } else if (totalCounter < totalVertices.Length - 4) {

                    convertedVerts[nonChildCounter] = totalVertices[totalCounter];
                    convertedColors[nonChildCounter] = totalColors[totalCounter];

                    nonChildCounter++;
                    totalCounter++;
                    continue;
                }
                totalCounter++;
            }
        } else {
            convertedVerts = totalVertices;
            convertedColors = totalColors;
        }
    }

    public void StartWobble(int start, int end, int spacesBefore, int spacesAfter, int tagLengthsBefore) {
        int wobbleStart = (start - spacesBefore - tagLengthsBefore) * 4;
        int wobbleEnd = ((end - spacesAfter - tagLengthsBefore) * 4) - 1;
        //Debug.Log(end + " | " + spacesAfter + " | " + tagLengthsBefore);

        if (!wobbleRanges.Contains(new(wobbleStart, wobbleEnd))) wobbleRanges.Add(new(wobbleStart, wobbleEnd));

        if (!lengths.Contains(new(wobbleStart, wobbleEnd))) lengths.Add(new(wobbleStart, wobbleEnd));

        TMP_SubMeshUI[] childMeshes = GameStateManager.dialogueManager.currentDialogueText.GetComponentsInChildren<TMP_SubMeshUI>();

        if (childMeshes.Length > 0) {
            childText = childMeshes[0];
        }

        wobbleOn = true;
    }

    private Vector3 Wobble(float time, float speedX, float speedY) {
        return new(Mathf.Sin(time * speedX), Mathf.Cos(time * speedY), 0);
    }

    public void StartGlitch(string sentence, int start, int end, int spacesBefore, int spacesAfter, int tagLengthsBefore) {
        glitchBackup = sentence;
        int glitchStart = start;
        int glitchEnd = end;
        int glitchPureStart = (glitchStart - spacesBefore - tagLengthsBefore) * 4;
        int glitchPureEnd = ((glitchEnd - spacesAfter - tagLengthsBefore) * 4) - 1;

        if (!glitchRanges.Contains(new(glitchStart, glitchEnd))) glitchRanges.Add(new(glitchStart, glitchEnd));
        if (!glitchPureRanges.Contains(new(glitchPureStart, glitchPureEnd))) glitchPureRanges.Add(new(glitchPureStart, glitchPureEnd));

        if (!lengths.Contains(new(glitchPureStart, glitchPureEnd))) lengths.Add(new(glitchPureStart, glitchPureEnd));

        TMP_SubMeshUI[] childMeshes = GameStateManager.dialogueManager.currentDialogueText.GetComponentsInChildren<TMP_SubMeshUI>();

        if (childMeshes.Length > 0) {
            childText = childMeshes[0];
        }

        // Start color glitch
        glitchOn = true;

        // For every glitched *letter*—
        foreach (var range in glitchRanges) {
            for (int i = range.start; i < range.end; i++) {

                // Start letter glitch
                StartCoroutine(Glitch(i));
            }
        }
    }

    private IEnumerator Glitch(int index) {

        while (glitchOn && GameStateManager.dialogueManager.currentDialogueText) {

            float rand = UnityEngine.Random.value;

            GameStateManager.dialogueManager.currentDialogueText.ForceMeshUpdate();
            mesh = GameStateManager.dialogueManager.currentDialogueText.mesh;
            vertices = mesh.vertices;

            if (convertedVerts == null || convertedVerts.Length <= 0) {
                convertedVerts = vertices;
            }

            // If roll succeeds—
            if (rand < flickerChance) {

                // ---------------------------------------------------------------------------------------- FLICKER

                // Shuffle all possible replacement letters
                List<char> shuffled = ShuffleList(possibleReplacements.ToList());

                // Copy down current dialogue sentence
                List<string> temp = new();
                foreach (char ch in GameStateManager.dialogueManager.currentDialogueText.text) {
                    temp.Add(ch.ToString());
                }

                // Don't replace spaces
                if (temp[index] == " ") {
                    continue;
                }
                
                // Choose replacement letter
                temp[index] = shuffled[UnityEngine.Random.Range(0, shuffled.Count - 1)].ToString();

                // Replace letter
                GameStateManager.dialogueManager.currentDialogueText.text = string.Concat(temp.ToArray()); 

                // ---------------------------------------------------------------------------------------- MOVE POSITION

                mesh.vertices = convertedVerts;
                GameStateManager.dialogueManager.currentDialogueText.canvasRenderer.SetMesh(mesh);

                yield return new WaitForSeconds(UnityEngine.Random.Range(flickerSpeedMin, flickerSpeedMax)); // WAIT --------------------------------

                // Copy down current dialogue sentence
                List<string> current = new();
                foreach (char ch in GameStateManager.dialogueManager.currentDialogueText.text) {
                    current.Add(ch.ToString());
                }

                // Revert letter back to original
                current[index] = glitchBackup[index].ToString();

                // Replace letter
                GameStateManager.dialogueManager.currentDialogueText.text = string.Concat(current.ToArray());

                // ---------------------------------------------------------------------------------------- MOVE POSITION BACK

                // Update vertices
                if (mesh) {
                    mesh.vertices = convertedVerts;
                    GameStateManager.dialogueManager.currentDialogueText.canvasRenderer.SetMesh(mesh);
                }
            }

            yield return new WaitForSeconds(UnityEngine.Random.Range(normalHoldSpeedMin, normalHoldSpeedMax));
        }
    }

    public void AddRichTextRange(Range range) {

        if (!richLengths.Contains(range)) {
            richLengths.Add(range);
        }
    }

    public void AddRegularTextRange(Range range) {

        if (!regLengths.Contains(range)) {
            regLengths.Add(range);
        }
    }

    public void ClearEffects() {
        on = false;
        lengths.Clear();
        regLengths.Clear();
        qLengths.Clear();
        qRegLengths.Clear();
        vertices = null;
        colors = null;
        childVertices = null;
        childColors = null;
        convertedVerts = null;
        convertedColors = null;
        convertedChildVerts = null;
        convertedChildColors = null;

        wobbleOn = false;
        wobbleRanges.Clear();

        glitchOn = false;
        glitchRanges.Clear();
        glitchPureRanges.Clear();
        glitchBackup = "";

        StopAllCoroutines();
        Debug.Log("CLEARED");
    }

    public List<char> ShuffleList(List<char> list) {

        List<char> newList = new(list);

        for(int i = newList.Count - 1; i > 0; i--) {
            int j = UnityEngine.Random.Range(0, i + 1);
            (newList[j], newList[i]) = (newList[i], newList[j]);
        }

        return newList;
    }
}