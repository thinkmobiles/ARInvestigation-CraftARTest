using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExampleOffTracking : MonoBehaviour, CraftARSDK.CraftARSDKCallbacks, CraftARSDK.CraftARItemEvents {

	public string CollectionToken = "Put your token here";

	float sliderValue=0.0f;
	CraftARItemAR lastItemAdded;
	bool isUpdatingWithTracking=true;

	void Start () {
		//Set this app as Callback handler in the SDK.
		CraftARSDK.instance.setCraftARSDKCallbacksHandler(this);

		CraftARSDK.instance.setCraftARItemEventsHandler(this);
	}
	public void OnGUI()
	{
		if (lastItemAdded == null) {
			//When there are no ARItems added, don't show anything in the GUI.
			return;
		}

		GUILayout.BeginVertical ();
		//Button to toogle Off tracking
		if (GUILayout.Button("Toggle OFF-Tracking", GUILayout.Height (Screen.height / 8))) {
			isUpdatingWithTracking = !isUpdatingWithTracking;
		}
		lastItemAdded.SetUpdateWithTracking (isUpdatingWithTracking);
		//Add an slider to modify item's rotation in direction Vector3.up
		//The slider is enabled just when isUpdatingWithTracking = false;
		if (!isUpdatingWithTracking) {
			sliderValue = GUI.HorizontalSlider (new Rect (50, 200, 600, 100), sliderValue, 0.0f, 360.0f);
			if (lastItemAdded != null) { 
				Quaternion newRotation = Quaternion.AngleAxis (sliderValue, Vector3.up);
				lastItemAdded.contentInstance.transform.rotation = newRotation;
			}
		}
		GUILayout.EndVertical ();
	}
	
	//CraftARSDK events:
	void CraftARSDK.CraftARSDKCallbacks.CraftARReady() { 
		//#warning Configure the SDK to point to your servers:
        CraftARSDK.instance.setToken(CollectionToken);
	}

	void CraftARSDK.CraftARSDKCallbacks.SearchResults(List<CraftARSearchResult> results){
		if (results.Count <= 0) {
			//Nothing found!
			Debug.Log("Results is empty");
			return;
		}
		//Something was found! We stop the finder mode.
		CraftARSDK.instance.stopFinderMode();
		
		//As an example, we take only the best match (the first in the list of matches). 
		CraftARItem bestMatch = results [0].Item;
		switch (bestMatch.ItemType) {
		case CraftARItem.CraftARItemType.AUGMENTED_REALITY_ITEM:
			CraftARItemAR arItem = bestMatch as CraftARItemAR;
			Debug.Log ("Found AR Item with name:"+arItem.itemName);

			// Add it for tracking, this will trigger the CraftARSDK.CraftARItemEvents.ItemAdded callback on success
			CraftARSDK.instance.AddSceneARItem(arItem);
			break;
		case  CraftARItem.CraftARItemType.IMAGE_RECOGNITION_ITEM:
			Debug.Log ("Found IR Item with name:"+bestMatch.itemName);
			break;
		} 
	}

	void CraftARSDK.CraftARSDKCallbacks.SearchError(CraftARError error) {
		Debug.Log("Search error: "+error.errorMessage);
	}
	void CraftARSDK.CraftARSDKCallbacks.CollectionReady() {
		CraftARSDK.instance.startFinderMode ();
		Debug.Log("token OK");
	}
	void CraftARSDK.CraftARSDKCallbacks.TokenValidationError(CraftARError error) {
		Debug.Log("Token validation error: " + error.errorMessage);
	}

	void CraftARSDK.CraftARItemEvents.ItemAdded(CraftARItemAR item) {
		lastItemAdded = item;
		//Start tracking and download item contents. During download, we change the ARItem content for a text label we have pre-loaded.
		CraftARSDK.instance.startTracking();
		//Download the bundle from this ARItem, and automatically enable it when download finishes.
		CraftARSDK.instance.DownloadItemContents(item, true);
	}
	
	void CraftARSDK.CraftARItemEvents.AddItemError(CraftARItemAR item, CraftARError error) {
		Debug.Log("Error Adding item: " + item.itemName + " Message: " + error.errorMessage);
	}
	
	void CraftARSDK.CraftARItemEvents.TrackingStarted(CraftARItemAR item) {
		Debug.Log ("Tracking started: " + item.itemName);
	}

	void CraftARSDK.CraftARItemEvents.TrackingLost(CraftARItemAR item) {
		Debug.Log ("Tracking lost: " + item.itemName);
	}
				
	void CraftARSDK.CraftARSDKCallbacks.PictureTaken(Texture2D image) {
	}
	
	void CraftARSDK.CraftARSDKCallbacks.TakePictureFailed() {
	}
}
