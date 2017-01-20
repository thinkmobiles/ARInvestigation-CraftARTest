// TestNewApp is free software. You may use it under the MIT license, which is copied
// below and available at http://opensource.org/licenses/MIT
//
// Copyright (c) 2014 Catchoom Technologies S.L.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
// Software, and to permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CraftARTest : MonoBehaviour, CraftARSDK.CraftARSDKCallbacks, CraftARSDK.CraftARItemEvents{

	public string CollectionToken = "Put your token here";

	bool restartCameraOnSearchResults = false;

	void Start () {
		//Set this app as Callback handler in the SDK.
		CraftARSDK.instance.setCraftARSDKCallbacksHandler(this);
		CraftARSDK.instance.setCraftARItemEventsHandler(this);

	}
	
	public void OnGUI()
	{
		//Example buttons performing calls to the CraftARSDK:
		GUILayout.BeginVertical ();
		GUIStyle buttonStyle = new GUIStyle ("button");
		buttonStyle.fontSize = 40;
		
		if (GUILayout.Button ("Start Finding",buttonStyle,GUILayout.Width(Screen.width/3), GUILayout.Height(Screen.height/8))){
			//Starts the finder mode. This will make 2 request every second 
			CraftARSDK.instance.startFinderMode();	
		}
		if (GUILayout.Button ("Stop Finding",buttonStyle, GUILayout.Width(Screen.width/3), GUILayout.Height(Screen.height/8))){
			//Stop the finder mode
			CraftARSDK.instance.stopFinderMode();
		}
		if (GUILayout.Button ("Single Shot Search",buttonStyle, GUILayout.Width(Screen.width/3), GUILayout.Height(Screen.height/8))){
			//Perform a single-shot search
			CraftARSDK.instance.singleShotSearch();
			restartCameraOnSearchResults = true;
		}
		GUILayout.EndVertical ();
	}

	//CraftARSDK events:
	void CraftARSDK.CraftARSDKCallbacks.CraftARReady() { 
		//#warning Set your collection token!
		CraftARSDK.instance.setToken(CollectionToken);
	}

	//The token has been set and validated.
	void CraftARSDK.CraftARSDKCallbacks.CollectionReady() {
		Debug.Log("Token OK");
	}

	//The token coould not be validated.
	void CraftARSDK.CraftARSDKCallbacks.TokenValidationError(CraftARError error) {
		Debug.Log("Token validation error: " + error.errorMessage);
	}

	void CraftARSDK.CraftARSDKCallbacks.SearchResults(List<CraftARSearchResult> results){

		if (restartCameraOnSearchResults) {
			//If we performed a single-shot search, we have to restart the camera.
			CraftARSDK.instance.restartCapture ();
			restartCameraOnSearchResults = false;
		}

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
			Debug.Log ("Found AR Item with name: "+bestMatch.itemName);
			CraftARItemAR arItem = bestMatch as CraftARItemAR;
			//Add item to the ARScene, start tracking and load item content.
			CraftARSDK.instance.AddSceneARItem(arItem);

			break;
		case  CraftARItem.CraftARItemType.IMAGE_RECOGNITION_ITEM:
			Debug.Log ("Found IR Item with name: "+bestMatch.itemName);
			break;
		} 
	}

	//Some error was produced during the search
	void CraftARSDK.CraftARSDKCallbacks.SearchError(CraftARError error) {
		if (restartCameraOnSearchResults) {
			CraftARSDK.instance.restartCapture ();
			restartCameraOnSearchResults = false;
		}

		Debug.Log("Search error: "+error.errorMessage);
	}

	void CraftARSDK.CraftARSDKCallbacks.PictureTaken(Texture2D image) {
		//Callback for the CraftARSDK.instance.takePicture() call, with the image as a Texture2D. Not used in this example
	}

	void CraftARSDK.CraftARSDKCallbacks.TakePictureFailed() {
		//Callback for the CraftARSDK.instance.takePicture() call, when the snapshot could not be taken. Not used in this example
	}
		
	// Item events:
	void CraftARSDK.CraftARItemEvents.ItemAdded(CraftARItemAR item) { 
		//AR item has been succesfully added for tracking.

		//Download item contents and start tracking
		CraftARSDK.instance.DownloadItemContents(item, true);
		CraftARSDK.instance.startTracking();
	}

	void CraftARSDK.CraftARItemEvents.AddItemError(CraftARItemAR item, CraftARError error) {
		//AR item could not be added for tracking.
		Debug.Log("Error Adding item: " + item.itemName + " Message: " + error.errorMessage);
	}
		
	void CraftARSDK.CraftARItemEvents.TrackingStarted(CraftARItemAR item) {
		Debug.Log("TrackingStarted: "+ item.itemName);
	}

	void CraftARSDK.CraftARItemEvents.TrackingLost(CraftARItemAR item) {
		Debug.Log("TrackingLost: "+ item.itemName);
	}

}