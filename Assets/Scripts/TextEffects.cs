using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TextEffects : MonoBehaviour
{

    private Mesh mesh;
    public Vector3[] vertices;
    public Color[] colors;

    [Header("WOBBLE")]

    public bool wobbleOn = false;

    public float wobbleSpeedX = 1.1f;
    public float wobbleSpeedY = 0.8f;

    private int wobbleStart;
    private int wobbleEnd;

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

    private int glitchStart;
    private int glitchEnd;
    private int glitchJoinedStart;
    private int glitchJoinedEnd;

    public float flickerChance = 0.5f;

    public string glitchBackup;

    public Gradient gradient;

    private void LateUpdate()
    {

        // If there is no dialogue being spoken, don't do anything
        if (!GameStateManager.dialogueManager.currentDialogueText) {
            return;
        } 
        
        // Wobble
        if (wobbleOn) {

            for (int i = wobbleStart; i <= wobbleEnd; i++) {

                Vector3 offset = Wobble(Time.time + i, wobbleSpeedX, wobbleSpeedY);

                vertices[i] = vertices[i] + offset;
            }

            mesh.vertices = vertices;
            GameStateManager.dialogueManager.currentDialogueText.canvasRenderer.SetMesh(mesh);
        }

        // Glitch
        if (glitchOn) {

            GameStateManager.dialogueManager.currentDialogueText.ForceMeshUpdate();
            mesh = GameStateManager.dialogueManager.currentDialogueText.mesh;
            vertices = mesh.vertices;
            colors = mesh.colors;

            for (int i = glitchJoinedStart * 4; i <= glitchJoinedEnd * 4; i += 4) {

                /* // Color
                if (glitchingColors[i] > -1) {
                    GlitchColorTimer(i);
                } else {
                    glitchColorSpeed = Random.Range(glitchColorSpeedMin, glitchColorSpeedMax);
                    GlitchColor(i);
                } */

                float colorSpeed = Random.Range(colorSpeedMin, colorSpeedMax);

                colors[i] = gradient.Evaluate(Mathf.Repeat(Time.time + vertices[i].x * colorSpeed, 1f));
                colors[i + 1] = gradient.Evaluate(Mathf.Repeat(Time.time + vertices[i + 1].x * colorSpeed, 1f));
                colors[i + 2] = gradient.Evaluate(Mathf.Repeat(Time.time + vertices[i + 2].x * colorSpeed, 1f));
                colors[i + 3] = gradient.Evaluate(Mathf.Repeat(Time.time + vertices[i + 3].x * colorSpeed, 1f));

                Vector3 offset = Wobble(Time.time + i, glitchSpeedX, glitchSpeedY);

                vertices[i] += offset;
                vertices[i + 1] += offset;
                vertices[i + 2] += offset;
                vertices[i + 3] += offset;
            }

            mesh.colors = colors;
            mesh.vertices = vertices;
            GameStateManager.dialogueManager.currentDialogueText.canvasRenderer.SetMesh(mesh);
        }
    }

    public void StartWobble(int start, int end, float speedX, float speedY) {
        wobbleStart = start;
        wobbleEnd = end;
        wobbleSpeedX = speedX;
        wobbleSpeedY = speedY;

        wobbleOn = true;
    }

    private Vector3 Wobble(float time, float speedX, float speedY) {
        return new(Mathf.Sin(time * speedX), Mathf.Cos(time * speedY), 0);
    }

    public void StartGlitch(string sentence, int start, int end, int spacesBefore, int spacesAfter) {

        // Set glitch stats
        glitchBackup = sentence;
        glitchStart = start;
        glitchEnd = end;
        glitchJoinedStart = glitchStart - spacesBefore;
        glitchJoinedEnd = glitchEnd - spacesAfter;

        // Start color glitch
        glitchOn = true;

        /* Debug.Log(start + " | " + end);
        Debug.Log(glitchJoinedStart + " | " + glitchJoinedEnd);
        Debug.Log(spacesBefore + " | " + spacesAfter); */

        // For every glitched *letter*—
        for (int i = glitchStart; i <= glitchEnd; i++) {

            // Start letter glitch
            StartCoroutine(Glitch(i));
        }
    }

    /* private void GlitchColorTimer(int i) {

        glitchingColors[i] += Time.deltaTime;

        // Reset timer
        if (glitchingColors[i] > glitchColorSpeed) {
            glitchingColors[i] = -1;
        }
    } */

    /* private void GlitchLetterTimer(int i) {

        glitchingLetters[i] += Time.deltaTime;

        //mesh.vertices = vertices;
        //GameStateManager.dialogueManager.currentDialogueText.canvasRenderer.SetMesh(mesh);

        // Reset timer
        if (glitchingLetters[i] > glitchLetterSpeed) {
            
            // Go back to original letter
            char[] original = GameStateManager.dialogueManager.currentDialogueText.text.ToArray();
            original[i] = glitchBackup[i];

            // Revert letter
            GameStateManager.dialogueManager.currentDialogueText.text = string.Concat(original.ToArray());

            mesh.vertices = vertices;
            GameStateManager.dialogueManager.currentDialogueText.canvasRenderer.SetMesh(mesh);

            if (glitchingLetters[i] > glitchLetterRevertSpeed + glitchLetterSpeed) {
                glitchingLetters[i] = -1;
            }
        }
    } */

    private IEnumerator Glitch(int index) {

        while (glitchOn && GameStateManager.dialogueManager.currentDialogueText) {

            float rand = Random.value;

            GameStateManager.dialogueManager.currentDialogueText.ForceMeshUpdate();
            mesh = GameStateManager.dialogueManager.currentDialogueText.mesh;
            vertices = mesh.vertices;

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
                temp[index] = shuffled[Random.Range(0, shuffled.Count - 1)].ToString();

                // Replace letter
                GameStateManager.dialogueManager.currentDialogueText.text = string.Concat(temp.ToArray()); 

                // ---------------------------------------------------------------------------------------- MOVE POSITION

                /* Vector3 offset = Wobble(Time.time + index, 0, 100);
                vertices[index] += offset;
                vertices[index + 1] += offset;
                vertices[index + 2] += offset;
                vertices[index + 3] += offset; */

                mesh.vertices = vertices;
                GameStateManager.dialogueManager.currentDialogueText.canvasRenderer.SetMesh(mesh);

                yield return new WaitForSeconds(Random.Range(flickerSpeedMin, flickerSpeedMax)); // WAIT --------------------------------

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

                /* vertices[index] -= offset;
                vertices[index + 1] -= offset;
                vertices[index + 2] -= offset;
                vertices[index + 3] -= offset; */

                // Update vertices
                mesh.vertices = vertices;
                GameStateManager.dialogueManager.currentDialogueText.canvasRenderer.SetMesh(mesh);
            }

            yield return new WaitForSeconds(Random.Range(normalHoldSpeedMin, normalHoldSpeedMax));
        }
    }

    /* private void GlitchLetter(int index) {

        float rand = Random.value;

        if (rand <= replaceChance) {

            // Shuffle all possible replacement letters
            List<char> shuffled = ShuffleList(possibleReplacements.ToList());
            
            // Choose replacement letter
            char[] temp = GameStateManager.dialogueManager.currentDialogueText.text.ToArray();
            temp[index] = shuffled[Random.Range(0, shuffled.Count - 1)];

            // Replace letter
            GameStateManager.dialogueManager.currentDialogueText.text = string.Concat(temp.ToArray());

            mesh.vertices = vertices;
            GameStateManager.dialogueManager.currentDialogueText.canvasRenderer.SetMesh(mesh);
        }

        // Resets the timer (SHOULD BE THE LAST THING IN THIS FUNCTION)
        glitchingLetters[index] = 0;
    } */

    /* private void GlitchColor(int index) {
            
        // g1 = 4 | g2 = 19

        if (colors.Length != vertices.Length) {
            colors = mesh.colors;
        }

        float rand = Random.value;

        Color c = new(255, 0, 0);

        if (rand < 0.5) {
            c = new(255, 0, 0);
        } else {
            c = new(0, 0, 255);
        }
        
        colors[index] = c;
        colors[index + 1] = c;
        colors[index + 2] = c;
        colors[index + 3] = c;

        /* mesh.colors = colors;
        GameStateManager.dialogueManager.currentDialogueText.canvasRenderer.SetMesh(mesh);

        // Resets the timer (SHOULD BE THE LAST THING IN THIS FUNCTION)
        glitchingColors[index] = 0;
    } */

    public void ClearEffects() {
        wobbleOn = false;
        wobbleStart = 0;
        wobbleEnd = 0;

        glitchOn = false;
        glitchStart = 0;
        glitchEnd = 0;
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