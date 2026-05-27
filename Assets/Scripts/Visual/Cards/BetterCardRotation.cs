using UnityEngine;
using System.Collections;

/// <summary>
/// This script should be attached to the card game object to display card`s rotation correctly.
/// </summary>
public class BetterCardRotation : MonoBehaviour 
{
    [Header("Card Rotation")]
    [SerializeField, Tooltip("Parent game object for all the card face graphics")]
    private RectTransform CardFront;
    
    [SerializeField, Tooltip("Parent game object for all the card back graphics")]
    private RectTransform CardBack;

    [SerializeField, Tooltip("Point on the face of the card that we will raycast to determine if the card is facing towards the camera or not. " +
                             "It should be placed somewhere in the middle of the card`s face.")]
    private Transform targetFacePoint;
    
    [SerializeField, Tooltip("3d collider attached to the card (2d colliders like BoxCollider2D won`t work in this case)")]
    private Collider col;

    // if this is true, our players currently see the card Back
    private bool showingBack = false;

	// Update is called once per frame
	private void Update () 
    {
        // Raycast from Camera to a target point on the face of the card
        // If it passes through the card`s collider, we should show the back of the card
        RaycastHit[] hits;
        hits = Physics.RaycastAll(origin: Camera.main.transform.position, 
                                  direction: (-Camera.main.transform.position + targetFacePoint.position).normalized, 
            maxDistance: (-Camera.main.transform.position + targetFacePoint.position).magnitude) ;
        bool passedThroughColliderOnCard = false;
        foreach (RaycastHit h in hits)
        {
            if (h.collider == col)
                passedThroughColliderOnCard = true;
        }
        //Debug.Log("TotalHits: " + hits.Length); 
        if (passedThroughColliderOnCard!= showingBack)
        {
            // something changed
            showingBack = passedThroughColliderOnCard;
            if (showingBack)
            {
                // show the back side
                CardFront.gameObject.SetActive(false);
                CardBack.gameObject.SetActive(true);
            }
            else
            {
                // show the front side
                CardFront.gameObject.SetActive(true);
                CardBack.gameObject.SetActive(false);
            }

        }

	}
}
