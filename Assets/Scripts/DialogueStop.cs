using System.Collections;
using System.Linq;
using UnityEngine;

public class DialogueStop : Road
{
    private CarPointer carPointer;

    public bool shouldStop;
    public GameObject[] stopLights;
    void Start()
    {
        foreach (var stopLight in stopLights)
        {
            Transform lightGroup = stopLight.transform.GetChild(0).GetChild(0).GetChild(0);

            //Material greMat = lightGroup.GetChild(0).GetComponent<MeshRenderer>().material;
            Material redMat = lightGroup.GetChild(1).GetComponent<MeshRenderer>().material;
            //Material yelMat = lightGroup.GetChild(2).GetComponent<MeshRenderer>().material;

            redMat.EnableKeyword("_EMISSION");
            redMat.SetColor("_EmissionColor", Color.red * 4f);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {

        // If the car pointer has been collided with—
        if (collider.CompareTag("CarFrame") && GameStateManager.Gamestate == GAMESTATE.PLAYING)
        {

            // If the car pointer's script can be accessed
            if (collider.transform.parent.TryGetComponent<CarController>(out var script))
            {

                // Set the car pointer's script reference to the script pulled from collision
                carPointer = script.carPointer;
                carPointer.inIntersection = true;

                if (shouldStop) carPointer.atStopSign = true;

                // Spawns new procedural road tile
                //carPointer.SpawnRoadTile();

                // Set appropriate turn signal and wheel rotation only if at the appropriate intersection
                //if (carPointer.wheel && carPointer.turnSignal && carPointer.roadQueue.First().center.transform.parent == transform)
                //{
                //    StartCoroutine(carPointer.turnSignal.SignalClick(carPointer.currentSteeringDirection));
                //    StartCoroutine(carPointer.wheel.TurnWheel(carPointer.currentSteeringDirection));
                //
                //    if (carPointer.currentSteeringDirection != SteeringDirection.FORWARD)
                //{
                //    GameStateManager.audioManager.PlayRandomSoundByName("Blinker");
                //}
                //}

                // Sets car to stop at stop signs or traffic lights
                if (shouldStop)
                {
                    StartCoroutine(WaitAtStop());
                }

            }
            else
            {
                Debug.LogWarning("Could not find CarPointer script on car pointer!");
                return;
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {

        if (collider.CompareTag("CarFrame"))
        {

            if (carPointer)
            {

                // Reset turn signal and wheel rotation
                //if (carPointer.wheel && carPointer.turnSignal)
                //{
                //    StartCoroutine(carPointer.turnSignal.SignalClick(SteeringDirection.FORWARD));
                //    StartCoroutine(carPointer.wheel.TurnWheel(SteeringDirection.FORWARD));
                //    GameStateManager.audioManager.StopSoundByName("Blinker");
                //}

                carPointer.inIntersection = false;
                if (shouldStop) carPointer.atStopSign = false;
            }
        }
    }

    private IEnumerator WaitAtStop()
    {

        float prevSpeed = carPointer.agent.speed;

        carPointer.agent.speed = 0;

        while (GameStateManager.dialogueManager.currentStory.canContinue)//wait until all dialogue is done
        {
            yield return null;
            Debug.Log("Waiting for dialogue");
        }

        foreach (var stopLight in stopLights)//change traffic lights color
        {
            Transform lightGroup = stopLight.transform.GetChild(0).GetChild(0).GetChild(0);

            Material greMat = lightGroup.GetChild(0).GetComponent<MeshRenderer>().material;
            Material redMat = lightGroup.GetChild(1).GetComponent<MeshRenderer>().material;
            //Material yelMat = lightGroup.GetChild(2).GetComponent<MeshRenderer>().material;

            redMat.DisableKeyword("_EMISSION");
            redMat.SetColor("_EmissionColor", Color.red * 4f);

            greMat.EnableKeyword("_EMISSION");
            greMat.SetColor("_EmissionColor", Color.green * 4f);
        }

        yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 2.5f));

        carPointer.agent.speed = prevSpeed;

        DialogueStop[] stops = transform.parent.GetComponentsInChildren<DialogueStop>();//search for and disable the other dialogue stops in this tile

        foreach (DialogueStop stop in stops)
            stop.GetComponent<Collider>().enabled = false;
    }
}
