﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Vector3 startPosition;
    public Vector3 backPosition;
    public Vector3 positionone;

    public static GameObject itemBeingDragged;

    private Transform startParent;

    private Vector3 initial;

    public void Awake()
    {
        positionone = transform.position;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        initial = this.transform.position;
        itemBeingDragged = gameObject;
        startPosition = transform.position;
        backPosition = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        AudioManager.instance.soundAudioSource.clip = AudioManager.instance.soundClip[0];
        AudioManager.instance.soundAudioSource.Play();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // transform.position = Input.mousePosition;

        Vector3 currentPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, initial.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(currentPosition);
        this.transform.position = worldPosition;    //worldPosition
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemBeingDragged = null;
        transform.position = startPosition;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        AudioManager.instance.soundAudioSource.clip = AudioManager.instance.soundClip[1];
        AudioManager.instance.soundAudioSource.Play();
    }

    public void MoveBack()
    {

        transform.SetParent(startParent);
        transform.position = positionone;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

    }
}
