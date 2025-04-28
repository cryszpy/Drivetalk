using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using Unity.VisualScripting;
using UnityEngine;

public enum ComparisonOperator {
    EQUAL, GREATER, LESSER
}

[System.Serializable]
public class KnotInfo {

    public string knotName;

    public bool seen = false;

    [Header("DIALOGUE REQUIREMENTS")] // ---------------------------------------------------------------------------------

    public List<KnotTagReq> requirements = new();

    public KnotInfo(string _knotName, bool _seen, List<KnotTagReq> _requirements = default) {
        knotName = _knotName;
        seen = _seen;
        if (!_requirements.IsUnityNull()) requirements = new(_requirements);
    }
}

[System.Serializable]
public class KnotTagReq {

    public string reqName;

    public int numberValue;

    public ComparisonOperator operation;

    public KnotTagReq(string _reqName, int _numberValue, ComparisonOperator _operation) {
        reqName = _reqName;
        numberValue = _numberValue;
        operation = _operation;
    }
}

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/StoryContainer")]
public class StoryContainer : ScriptableObject
{
    public TextAsset jsonFile;

    public Story story;

    public List<KnotInfo> knots = new();

    public static List<string> GetKnotAndStitches(Story story)
    {
        var output = new List<string>();
        var knots = story.mainContentContainer.namedContent.Keys;
        knots.ToList().ForEach((knot) =>
        {
            output.Add(knot);

            // Adds subknots
            /* var container = story.KnotContainerWithName(knot);
            var stitchKeys = container.namedContent.Keys;
            stitchKeys.ToList().ForEach((stitch) =>
            {
                output.Add(knot + "." + stitch);
            }); */
        });

        // Gets rid of "start" and "global decl"
        output.RemoveAt(0);
        //output.RemoveAt(output.Count - 1);

        return output;
    }

    public void SetStories() {

        // Update the story with any changes and rebind external functions
        story = new(jsonFile.text);
        GameStateManager.dialogueManager.BindExternalFunctions(story);

        GetKnots(story);
    }

    public void GetKnots(Story story) {

        List<string> allKnots = GetKnotAndStitches(story);

        // For every knot inside this character's Ink dialogue
        foreach (string knotName in allKnots) {

            KnotInfo knot = CreateKnot(story, knotName);

            // If the current knot does not exist in any dialogue then add it
            if (!knots.Exists(x => x.knotName == knotName)) {
                knots.Add(knot);
            } 
            // If the knot already exists, check to see if its info is updated
            else {
                int oldKnotIndex = knots.FindIndex(x => x.knotName == knotName);

                KnotInfo oldKnot = knots[oldKnotIndex];

                int counter = 0;

                // If there are unequal amounts of requirements, increment the counter
                if (knot.requirements.Count != oldKnot.requirements.Count) {
                    counter++;
                } 
                // Otherwise, check to see if requirements have changed
                else {

                    // For every requirement—
                    for (int i = 0; i < knot.requirements.Count; i++) {
                        
                        // Grab the currently selected requirements
                        KnotTagReq req = knot.requirements[i];
                        KnotTagReq oldReq = oldKnot.requirements[i];

                        // If any requirement info is out-of-date, increment counter
                        if (req.reqName != oldReq.reqName) counter++;
                        if (req.numberValue != oldReq.numberValue) counter++;
                        if (req.operation != oldReq.operation) counter++;
                    }
                }

                // If anything is out of date, replace the knot with the new updated knot
                if (counter > 0) {
                    knots[oldKnotIndex] = knot;
                    Debug.Log("Replaced knot: " + knot.knotName);
                }
            }
        }

        // If there are knots that need to be removed—
        if (knots.Count > allKnots.Count) {

            // Gathers all knots that are not inside the new updated version
            List<KnotInfo> queuedForRemoval = new();

            // For every knot in the current version—
            for (int i = 0; i < knots.Count; i++) {
                
                string knotName = knots[i].knotName;

                // If this knot does not exist in the updated version—
                if (!allKnots.Exists(x => x == knotName)) {

                    // Queue the knot for removal
                    queuedForRemoval.Add(knots[i]);
                }
            }

            // Remove knots that are queued for removal
            foreach (KnotInfo knot in queuedForRemoval) {
                knots.Remove(knot);
            }
        }
    }

    public KnotInfo CreateKnot(Story story, string knotName)  {

        List<KnotTagReq> tags = new();

        // Gets all tags in the knot
        List<string> unsortedTags = story.TagsForContentAtPath(knotName);

        // If this knot has any tags—
        if (unsortedTags != null && unsortedTags.Count > 0) {

            // For every tag this knot has—
            foreach (var item in unsortedTags) {

                // Split the tag into type and value
                List<string> splitTag = item.Split(":").ToList();

                KnotTagReq newReq;

                // Create a new Story Requirement from the tag
                if (splitTag[1].Contains("<")) newReq = new(splitTag[0], int.Parse(splitTag[1].Replace("<", "")), ComparisonOperator.LESSER);
                else if (splitTag[1].Contains(">")) newReq = new(splitTag[0], int.Parse(splitTag[1].Replace(">", "")), ComparisonOperator.GREATER);
                else if (int.TryParse(splitTag[1], out int result)) newReq = new(splitTag[0], result, ComparisonOperator.EQUAL);
                else {
                    Debug.LogError("Unknown character found in tag: '" + knotName + "' --> " + item);
                    return null;
                }

                // Adds new requirement tag to this knot's list of tags
                tags.Add(newReq);
            }
        }

        // Return a new KnotInfo knot
        return new(knotName, false, tags);
    }
}
