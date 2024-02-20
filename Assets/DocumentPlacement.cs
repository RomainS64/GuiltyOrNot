using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


enum Direction
{
    Left,Right,Up,Down
}

public struct DocumentPositions
{
    public Transform document;
    public Vector3 documentGroupedPosition;
    public Vector3 suspectGroupedPosition;
}
public class DocumentPlacement : MonoSingleton<DocumentPlacement>
{
    private List<DocumentPositions> documentPositions = new();
    [SerializeField] private Transform[] placementZones;

    [SerializeField] private float distance;
    [SerializeField] private float randomNoiseDistance;
    [SerializeField] private float randomNoiseLateral;
    private List<Direction> directions = new() { Direction.Left,Direction.Right, Direction.Down, Direction.Up };
    public void PlaceDocument(int id, List<Transform> documents)
    {
        ScenarioFlow.Shuffle(directions);
        documents[0].position = placementZones[id].position;
        for (int i = 1; i < 4; i++)
        {
            Vector3 displacement = Vector3.zero;
            switch (directions[i])
            {
                case Direction.Left:
                    displacement = new Vector3(
                        -distance + Random.Range(-randomNoiseDistance, randomNoiseDistance),
                        Random.Range(-randomNoiseLateral, randomNoiseLateral),0);
                    break;
                case Direction.Right:
                    displacement = new Vector3(
                        distance + Random.Range(-randomNoiseDistance, randomNoiseDistance),
                        Random.Range(-randomNoiseLateral, randomNoiseLateral),0);
                    break;
                case Direction.Up:
                    displacement = new Vector3(
                        Random.Range(-randomNoiseLateral, randomNoiseLateral),
                        distance + Random.Range(-randomNoiseDistance, randomNoiseDistance),
                        0);
                    break;
                case Direction.Down:
                    displacement = new Vector3(
                        Random.Range(-randomNoiseLateral, randomNoiseLateral),
                        -distance + Random.Range(-randomNoiseDistance, randomNoiseDistance),
                        0);
                    break;
            }
            documentPositions.Add(new DocumentPositions()
            {
                document = documents[i],
                documentGroupedPosition = placementZones[id].position+displacement,
                suspectGroupedPosition =  placementZones[id].position+displacement
            });
            documents[i].position = placementZones[id].position+displacement;
        }
    }

    public void GroupBySuspect()
    {
        foreach (var document in documentPositions)
        {
            StartCoroutine(MoveTo(document.document, document.suspectGroupedPosition, 1f));
        }
    }
    public void GroupByDocuments()
    {
        foreach (var document in documentPositions)
        {
            StartCoroutine(MoveTo(document.document, document.documentGroupedPosition, 1f));
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            GroupBySuspect();
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            GroupByDocuments();
        }
    }

    IEnumerator MoveTo(Transform toMove,Vector3 where,float time)
    {
        float precision = 100;
        Vector3 startPos = toMove.position;
        for (int i = 0; i < precision; i++)
        {
            toMove.position = Vector3.Lerp(startPos, where, 1f / (precision-i));
            yield return new WaitForSeconds(time / precision);
        }
    }
}
