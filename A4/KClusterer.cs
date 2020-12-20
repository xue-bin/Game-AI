using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KClusterer
{
    const int K = 4;//TODO; set K to the optimal value that you've found via experimentation
    const int MAX_ATTEMPTS = 5000;//Maximum number of clustering attempts, you may want to use this
    const float threshold = 1f;//Threshold for cluster similarity, you may want to use this and alter it if so

    //TODO; fix this function
    public Dictionary<Datapoint, List<Datapoint>> Cluster(Datapoint[] datapoints)
    {

        Dictionary<Datapoint, List<Datapoint>> clustersByCenters = new Dictionary<Datapoint, List<Datapoint>>();
        Dictionary<Datapoint, List<Datapoint>> calculateClusters = new Dictionary<Datapoint, List<Datapoint>>();
        Dictionary<Datapoint, List<Datapoint>> calculatenewClusters = new Dictionary<Datapoint, List<Datapoint>>();
        List<Datapoint> newcentroids = new List<Datapoint>();        
        List<Datapoint> oldCentroids = new List<Datapoint>();


        //Select K random centers to start
        List<Datapoint> centers = new List<Datapoint>();

        while (centers.Count < K)
        {
            // Generate a random index less than the size of the array.  
            int randomIndex = Random.Range(0, datapoints.Length);
            Datapoint randomCenter = datapoints[randomIndex];

            if (!centers.Contains(randomCenter))
            {
                centers.Add(randomCenter);
            }
        }

        //Instantiate clusters by these centers
        foreach (Datapoint center in centers)
        {
            clustersByCenters.Add(center, new List<Datapoint>());
        }

        foreach (Datapoint pnt in datapoints)
            {
                Datapoint closestCenter = null;
                float minDistance = float.PositiveInfinity;

                foreach (Datapoint center in centers)
                {
                    float thisDistance = Distance(pnt, center);
                    if (thisDistance < minDistance)
                    {
                        closestCenter = center;
                        minDistance = thisDistance;
                    }
                }
                clustersByCenters[closestCenter].Add(pnt);
            }

        // oldcentroids = []
        //Debug.Log("diff:" + DifferenceBetweenCenters(centers.ToArray(), oldCentroids.ToArray()));
        int attempts = 0;
        //var abc = new HashSet<Datapoint>(centers);
        // while not centroids==oldcentroids:

        while ((DifferenceBetweenCenters(centers.ToArray(), oldCentroids.ToArray()) > threshold) && (attempts < MAX_ATTEMPTS))
        {
            attempts += 1;

            // oldcentroids = centroids
            oldCentroids.Clear();
            foreach (Datapoint d in centers)
            {
                oldCentroids.Add(d);
            }

            //clustersByCenters = new Dictionary<Datapoint, List<Datapoint>>();

            //calculateClusters();Map each datapoint to its closest center
            //Discover the new centroids
            
            
            //centers = new List<Datapoint>();
            foreach (Datapoint center in oldCentroids)
            {
                //Datapoint newCentroids = GetAverage(clustersByCenters[center].ToArray());
                //centers.Add(newCentroids);
                calculateClusters.Add(center, new List<Datapoint>());
            }

            foreach (Datapoint pnt in datapoints)
            {
                Datapoint closestCenter = null;
                float minDistance = float.PositiveInfinity;

                foreach (Datapoint center in oldCentroids)
                {
                    float thisDistance = Distance(pnt, center);
                    if (thisDistance < minDistance)
                    {
                        closestCenter = center;
                        minDistance = thisDistance;
                    }
                }
                calculateClusters[closestCenter].Add(pnt);
            }

            foreach (KeyValuePair<Datapoint, List<Datapoint>> i in calculateClusters)
            {
                calculatenewClusters[GetMedian(i.Value.ToArray())] = i.Value;
            }

            foreach (KeyValuePair<Datapoint, List<Datapoint>> i in calculatenewClusters)
            {
                newcentroids.Add(i.Key);
            }
            
            centers = newcentroids;
            newcentroids = new List<Datapoint>();
            calculatenewClusters = new Dictionary<Datapoint, List<Datapoint>>();
            clustersByCenters = calculateClusters;
        }
        return clustersByCenters;
    }

    //Calculate the difference between sets of centers
    private float DifferenceBetweenCenters(Datapoint[] centers1, Datapoint[] centers2)
    {
        List<Datapoint> mappedPoints = new List<Datapoint>();
        float totalDistance = 0;
        foreach(Datapoint c1 in centers1)
        {
            Datapoint minPoint = null;
            float minDistance = float.PositiveInfinity;

            foreach(Datapoint c2 in centers2)
            {
                if (!mappedPoints.Contains(c2))
                {
                    float thisDistance = Distance(c1, c2);

                    if (thisDistance < minDistance)
                    {
                        minDistance = thisDistance;
                        minPoint = c2;
                    }
                }
            }
            mappedPoints.Add(minPoint);
            totalDistance += minDistance;
        }
        
        return totalDistance;
    }

    //Calculate and returns the geometric median of the given datapoints
    public Datapoint GetMedian(Datapoint[] datapoints)
    {
        Datapoint medianPnt = null;
        float totalDistance = float.PositiveInfinity;

        for(int i = 0; i<datapoints.Length; i++)
        {
            float thisDistance = 0;
            for(int j=0; j<datapoints.Length; j++)
            {
                if (i != j)
                {
                    thisDistance += Distance(datapoints[i], datapoints[j]);
                }
            }

            if (thisDistance < totalDistance)
            {
                totalDistance = thisDistance;
                medianPnt = datapoints[i];
            }
        }

        return medianPnt;
    }

    //Calculate and returns the average of the given datapoints
    public Datapoint GetAverage(Datapoint[] datapoints)
    {
        Datapoint sumDatapoint = new Datapoint("", 0, 0, 0, 0, 0, 0, 0, 0);
        int churnedVal = 0;

        foreach(Datapoint d in datapoints)
        {
            sumDatapoint = new Datapoint("", sumDatapoint.HoursPlayed + d.HoursPlayed, sumDatapoint.Level + d.Level, sumDatapoint.PelletsEaten + d.PelletsEaten, sumDatapoint.FruitEaten + d.FruitEaten, sumDatapoint.GhostsEaten + d.GhostsEaten, sumDatapoint.AvgScore + d.AvgScore, sumDatapoint.MaxScore + d.MaxScore, sumDatapoint.TotalScore + d.TotalScore);

            if (d.Churned)
            {
                churnedVal += 1;
            }
            else
            {
                churnedVal -= 1;
            }
        }
        //Calculate averages
        int hoursPlayed = (int)(((float)sumDatapoint.HoursPlayed) / ((float)datapoints.Length));
        int level = (int)(((float)sumDatapoint.Level) / ((float)datapoints.Length));
        int pelletsEaten = (int)(((float)sumDatapoint.PelletsEaten) / ((float)datapoints.Length));
        int fruitEaten = (int)(((float)sumDatapoint.FruitEaten) / ((float)datapoints.Length));
        int ghostsEaten = (int)(((float)sumDatapoint.GhostsEaten) / ((float)datapoints.Length));
        float avgScore = (((float)sumDatapoint.AvgScore) / ((float)datapoints.Length));
        int maxScore = (int)(((float)sumDatapoint.MaxScore) / ((float)datapoints.Length));
        int totalScore = (int)(((float)sumDatapoint.MaxScore) / ((float)datapoints.Length));

        bool churned = false;
        if (churnedVal > 0)
        {
            churned = true;
        }

        return new Datapoint("", hoursPlayed, level, pelletsEaten, fruitEaten, ghostsEaten, avgScore, maxScore, totalScore, churned);
    }

    //TODO; alter this distance function
    //WARNING: DO NOT USE CHURNED AS PART OF THIS FUNCTION
    
    public static float Distance(Datapoint a, Datapoint b)
    {//HoursPlayed: 2143 Level: 334 Pellets Eaten: 509712 Fruit Eaten: 1319 Ghosts Eaten: 37927 Average Score: 2061 Max Score: 5010 Total Score: 359495
        float dist = 0;
        dist += (Mathf.Abs(a.HoursPlayed - b.HoursPlayed)) / Mathf.Max(a.HoursPlayed, b.HoursPlayed);
        dist += (Mathf.Abs(a.Level - b.Level)) / Mathf.Max(a.Level, b.Level);
        dist += (Mathf.Abs(a.PelletsEaten - b.PelletsEaten)) / Mathf.Max(a.PelletsEaten, b.pelletsEaten);
        dist += (Mathf.Abs(a.FruitEaten - b.FruitEaten)) / Mathf.Max(a.FruitEaten, b.FruitEaten);
        dist += (Mathf.Abs(a.GhostsEaten - b.GhostsEaten)) / Mathf.Max(a.GhostsEaten, b.GhostsEaten);
        dist += (Mathf.Abs(a.AvgScore - b.AvgScore)) / Mathf.Max(a.AvgScore, b.AvgScore);
        dist += (Mathf.Abs(a.MaxScore - b.MaxScore)) / Mathf.Max(a.MaxScore, b.MaxScore);
        dist += (Mathf.Abs(a.TotalScore - b.TotalScore)) / Mathf.Max(a.totalScore, b.totalScore);
        return dist;
    }

}

