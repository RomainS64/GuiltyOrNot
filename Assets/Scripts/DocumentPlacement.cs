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
    public int documentTypeId;
    public Transform document;
    public Vector3 documentGroupedPosition;
    public Vector3 suspectGroupedPosition;
}
public class DocumentPlacement : MonoSingleton<DocumentPlacement>
{
    private List<DocumentPositions> documentPositions = new();
    [SerializeField] private Transform[] placementZones;
    [SerializeField] private Transform[] documentPlacementZones;
    [SerializeField] private Transform followPoint;
    [SerializeField] private Vector3[] documentDecal;
    [SerializeField] private float distance;
    [SerializeField] private float randomNoiseDistance;
    [SerializeField] private float randomNoiseLateral;
    private List<Direction> directions = new() { Direction.Left,Direction.Right, Direction.Down, Direction.Up };
    public void PlaceDocument(int id, List<Transform> documents)
    {
        ScenarioFlow.Shuffle(directions);
        documents[0].position = placementZones[id].position;
        documentPositions.Add(new DocumentPositions()
        {
            documentTypeId = 0,
            document = documents[0],
            documentGroupedPosition = SpiralPlacement(documentPlacementZones[0],id,0),
            suspectGroupedPosition =  placementZones[id].position
        });
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
            int documentId = i;
            documentPositions.Add(new DocumentPositions()
            {
                documentTypeId = documentId,
                document = documents[i],
                documentGroupedPosition = SpiralPlacement(documentPlacementZones[i],id,documentId),
                suspectGroupedPosition =  placementZones[id].position+displacement
            });
            documents[i].position = placementZones[id].position+displacement;
        }

        IsGroupedByDocument = false;
    }
    
    [SerializeField] private float[] spiralDistanceStart;
    [SerializeField] private float[] spiralAddition;
    [SerializeField] private float[] spiralAngle;
    [SerializeField] private float[] spiralAngleRdm;
    [SerializeField] private float[] spiralRdm;
    [SerializeField] private float[] xRdm;
    [SerializeField] private float[] yRdm;
     
    public Vector3 SpiralPlacement(Transform _placementPoint,int id,int docId)
    {
        Vector3 basePos =Vector3.zero;
        _placementPoint.rotation = Quaternion.identity;
        float distanceMultiplicator = spiralDistanceStart[docId];
        for (int i = 0; i < id; i++)
        {
            Vector3 up = _placementPoint.up * (distanceMultiplicator+Random.Range(-spiralRdm[docId],spiralRdm[docId]));
            distanceMultiplicator += spiralAddition[docId];
            _placementPoint.position += up;
            _placementPoint.Rotate(Vector3.forward,spiralAngle[docId]+Random.Range(-spiralAngleRdm[docId],spiralAngleRdm[docId]));
        }
        Vector3 position =  _placementPoint.position;
        _placementPoint.rotation = Quaternion.identity;
        _placementPoint.position = basePos+new Vector3(Random.Range(-xRdm[docId],xRdm[docId]),Random.Range(-yRdm[docId],yRdm[docId]),0);
        return position;
    }
    public void GroupBySuspect()
    {
        int i = 0;
        foreach (var document in documentPositions)
        {
            if (!document.document.gameObject.activeSelf) continue;
            if (Vector2.Distance(document.document.position, document.suspectGroupedPosition) > 1)
            {
                StartCoroutine(MoveTo(document.document, document.suspectGroupedPosition, 0.2f,i<4));
            }
            ++i;
        }
        IsGroupedByDocument = false;
    }
    public bool IsGroupedByDocument { get; private set; }
    public void GroupByDocuments(int documentTypeId = -1)
    {
        int i = 0;
        foreach (var document in documentPositions)
        {
            if (!document.document.gameObject.activeSelf) continue;
            if (documentTypeId != -1 &&
                document.documentTypeId != documentTypeId) continue;
            
            if (Vector2.Distance(document.document.position, document.documentGroupedPosition) > 1)
            {
                document.document.SetAsLastSibling();
                StartCoroutine(MoveTo(document.document,followPoint.position+document.documentGroupedPosition+documentDecal[document.documentTypeId], 0.2f,i<4));
            }

            ++i;
        }

        IsGroupedByDocument = true;
    }

    IEnumerator MoveTo(Transform targetTransform, Vector3 destination, float time,bool playSound)
    {
        if(playSound)AudioManager.instance.audioEvents[Random.Range(0,2)==0?"Object Grab":"Object Release"].Play();
        Vector3 startPos = targetTransform.position;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            targetTransform.position = Vector3.Lerp(startPos, destination, elapsedTime / time);
            
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
        targetTransform.position = destination;
    }
}
