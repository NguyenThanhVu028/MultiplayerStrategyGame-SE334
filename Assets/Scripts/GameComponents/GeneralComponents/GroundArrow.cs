using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class GroundArrow : MonoBehaviour
{
    private enum CursorPosition { Tip, Center}

    [Header("Properties")]
    [SerializeField] float arrowHeight = 1.15f;
    [SerializeField] float arrowWidth = 1;
    [SerializeField] float bodyWidth = 0.7f;
    [SerializeField] float groundY = 0;
    [SerializeField] float groundDistance = 0;
    [SerializeField] CursorPosition cursorPosition;

    [SerializeField] Vector3 fromPos;
    [SerializeField] Vector3 toPos;

    [SerializeField] GameObject topObject = null;
    [SerializeField] GameObject bottomObject = null;
    [SerializeField] GameObject bottomSprite = null;

    [SerializeField] bool followMouse;
    [SerializeField] LayerMask layerToDetectMousePosition;

    void Update()
    {
        if(topObject != null && bottomObject != null && fromPos != toPos)
        {
            if (followMouse)
            {
                toPos = GetMouseWorldPosition();
                groundY = toPos.y;
            }
            fromPos.y = groundY + groundDistance;
            toPos.y = groundY + groundDistance;
            Vector3 distance = toPos - fromPos; 

            //Adjust angle
            float angle = Mathf.Atan2(distance.z, distance.x);
            topObject.transform.rotation = Quaternion.Euler(0, 90 - angle * Mathf.Rad2Deg, 0);
            bottomObject.transform.rotation = Quaternion.Euler(0, 90 - angle * Mathf.Rad2Deg, 0);

            //Adjust scale
            bottomObject.transform.localScale = new Vector3(bodyWidth / this.transform.lossyScale.x, 1.0f / this.transform.lossyScale.y, bodyWidth / this.transform.lossyScale.z);
            if ((fromPos - toPos).magnitude <= arrowHeight)
            {
                topObject.transform.localScale = new Vector3(arrowWidth / this.transform.lossyScale.x, 1.0f / this.transform.lossyScale.y, (fromPos - toPos).magnitude) / this.transform.lossyScale.z;
                bottomSprite.GetComponent<SpriteRenderer>().size = new Vector2(1.0f, 0.0f);
            }
            else
            {
                topObject.transform.localScale = new Vector3(arrowWidth / this.transform.lossyScale.x, 1.0f / this.transform.lossyScale.y, arrowHeight / this.transform.lossyScale.z);
                bottomSprite.GetComponent<SpriteRenderer>().size = new Vector2(1.0f, (distance.magnitude -  arrowHeight) / bodyWidth);
            }

            //Adjust position
            Vector3 normalizedDistance = distance.normalized; //length = 1
            Vector3 TopPos = toPos - normalizedDistance * topObject.transform.lossyScale.z * 0.5f; TopPos.y = groundY + groundDistance;
            Vector3 BottomPos = (toPos - normalizedDistance * topObject.transform.lossyScale.z + fromPos) * 0.5f; BottomPos.y = groundY + groundDistance;
            topObject.transform.position = TopPos;
            bottomObject.transform.position = BottomPos;

            this.transform.position = new Vector3(0, 0, 0);
        }
    }
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, layerToDetectMousePosition))
        {
            return hitInfo.point;
        }
        else return Vector3.zero;
    }
    public void SetFromPos(Vector3 target)
    {
        target.y = 0;
        fromPos = target;
    }
}
