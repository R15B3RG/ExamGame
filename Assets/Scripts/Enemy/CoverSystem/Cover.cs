using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    [Header("Cover points")]
    [SerializeField] private GameObject coverPointPrefab;
    [SerializeField] private List<CoverPoint> coverPoints = new List<CoverPoint>();
    [SerializeField] private float xOffset = 1;
    [SerializeField] private float yOffset = .2f;
    [SerializeField] private float zOffset = 1;

    private void Start()
    {
        GenerateCoverPoints();
    }

    private void GenerateCoverPoints()
    {
        Vector3[] localCoverPoints =
        {
            new Vector3 (0, yOffset, zOffset), //Front
            new Vector3 (0, yOffset, -zOffset), //Back
            new Vector3 (xOffset, yOffset, 0), //Right
            new Vector3 (-xOffset, yOffset, 0) //Left
        };

        foreach (Vector3 localPoint in localCoverPoints)
        {
            Vector3 worldPoint = transform.TransformPoint(localPoint);
            CoverPoint coverPoint = Instantiate(coverPointPrefab, worldPoint, Quaternion.identity, transform).GetComponent<CoverPoint>();

            coverPoints.Add(coverPoint);
        }
    }

    public List<CoverPoint> GetCoverPoints() => coverPoints;
}
