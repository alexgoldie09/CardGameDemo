using UnityEngine;
using System.Collections;

// place first and last elements in children array manually
// others will be placed automatically with equal distances between first and last elements
public class SameDistanceChildren : MonoBehaviour 
{

	[SerializeField, Tooltip("First and last elements in this array should be placed manually. " +
							 "The script will automatically place the other elements with equal distances between the first and last ones.")]
    private Transform[] Children;

	// Use this for initialization
	private void Awake () 
    {
	    if (Children == null || Children.Length == 0)
	    {
		    Debug.LogError("SameDistanceChildren: Children array is not assigned or empty. Please assign the first and last elements in the inspector.");
		    return;
	    }
	    
        Vector3 firstElementPos = Children[0].transform.position;
        Vector3 lastElementPos = Children[^1].transform.position;

        // dividing by Children.Length - 1 because for example: between 10 points that are 9 segments
        float xDist = (lastElementPos.x - firstElementPos.x)/(Children.Length - 1);
        float yDist = (lastElementPos.y - firstElementPos.y)/(Children.Length - 1);
        float zDist = (lastElementPos.z - firstElementPos.z)/(Children.Length - 1);

        Vector3 dist = new Vector3(xDist, yDist, zDist);

        for (int i = 1; i < Children.Length; i++)
            Children[i].transform.position = Children[i - 1].transform.position + dist;
	}
	
	
}
