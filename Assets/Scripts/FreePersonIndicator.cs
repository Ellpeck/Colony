using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreePersonIndicator : MonoBehaviour {

    private int currentlySelected = -1;

    public void OnPress() {
        var objects = FindObjectsOfType<Person>();
        for (var i = 0; i < objects.Length; i++) {
            var index = (this.currentlySelected + 1 + i) % objects.Length;
            var person = objects[index];
            if (person.IsBusy())
                continue;
            CameraController.Instance.CenterCameraOn(person.transform.position);
            SelectionManager.Instance.Select(person.GetComponent<Selectable>(), true);
            this.currentlySelected = index;
            return;
        }
        this.currentlySelected = -1;
    }

}