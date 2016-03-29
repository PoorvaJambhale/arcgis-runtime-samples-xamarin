// Copyright 2015 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Esri.ArcGISRuntime.Mapping;
using Xamarin.Forms;
using Esri.ArcGISRuntime.Xamarin.Forms.UI;
using System;
using System.Linq;

namespace ArcGISRuntimeXamarin.Samples.ChangeSublayerVisibility
{
	public partial class ChangeSublayerVisibility : ContentPage
	{


		// Controls that are global in scope for this page.
		StackLayout myStackLayout;
		Button myButton;
		MapView myMapView;
		Map myMap;

		public ChangeSublayerVisibility()
		{
			InitializeComponent();

			// Title of the code example being demonstrated.
			Title = "Change sub layer visibility";


			// Dynamically load all of the controls on the device surface for this code example. They will include: 
			
			// 1. A StackPanel with a vertical orientation.
			myStackLayout = new StackLayout();
			myStackLayout.VerticalOptions = LayoutOptions.FillAndExpand;
			Grid1.Children.Add(myStackLayout); // Grid1 is the only control defined in Xaml.

			// 2. A button. This will be used to load an ArcGISMapImageLayer in the Map and display some Switch controls to 
			// turn on/off the sublayers in the ArcGISMapImageLayer.
			myButton = new Button();
			myButton.Text = "Load the data";
			myButton.Clicked += myButton_Clicked; // Add a click event handler when the button is clicked.
			myStackLayout.Children.Add(myButton);

			// 3. A MapView with and associated Map.
			myMapView = new MapView();
			myMapView.HeightRequest = 200;
			myMapView.WidthRequest = 200;
			myMap = new Map();
			myMapView.Map = myMap;
			myStackLayout.Children.Add(myMapView);
		}

		private async void myButton_Clicked(object sender, EventArgs e)
		{
			// This function will display an ArcGISMapImageLayer in the Map and then display 3 Switch controls to turn
			// on/off the visibility of the sublayers.

			// The location of the map service.
			Uri myUri = new Uri("http://sampleserver6.arcgisonline.com/arcgis/rest/services/SampleWorldCities/MapServer");

			// Create a new ArcGISMapImageLayer from the Uri. This particular service has 3 sublayers (Cites - index 0, 
			// Continent - index 1, and World - index 2).
			ArcGISMapImageLayer myArcGISMapImageLayer = new ArcGISMapImageLayer(myUri);

			// Get the basemap of the map.  
			Basemap myBasemap = myMap.Basemap;

			// Get the base layers of the basemap.
			LayerCollection myLayerCollection = myBasemap.BaseLayers;

			// Add the ArcGISMapImageLayer to the base layers.
			myLayerCollection.Add(myArcGISMapImageLayer);

			// Refresh the layers in the map.
			await myMap.RetryLoadAsync();
			
			// Loop through all of the sublayers in the ArcGISMapImageLayer. There should be three of them for this service.
			foreach (var item in myArcGISMapImageLayer.Sublayers)
			{
				// Dynamically create a set of controls (one for each sublayer) that will enable the user to turn 
				// on /off the individual sublayers.
				StackLayout myStackLayout = new StackLayout();
				myStackLayout.Orientation = StackOrientation.Horizontal;
				Label myLabel = new Label();
				myLabel.Text = "SubLayer visibility " + item.Name; // Display the name of the sublayer.
				Switch mySwitch = new Switch();
				mySwitch.ClassId = item.Name; // Set the sublayer name to this property so it can be accessed by the toggled event handler.
				mySwitch.Toggled += MySwitch_Toggled; // Add a toggled event handler to turn on/off the sublayers.
				mySwitch.IsToggled = true;
				myStackLayout.Children.Add(myLabel);
				myStackLayout.Children.Add(mySwitch);
				this.myStackLayout.Children.Add(myStackLayout);
			}

			// Don't let the user click the button again to add the ArcGISMapImageLayer and other dynamically created controls.
			myButton.IsEnabled = false;
		}

		private void MySwitch_Toggled(object sender, ToggledEventArgs e)
		{

			// This function controls the turning on/off of the ArcGISMapImageLayer sublayers for each of the Switch controls.

			// Get the Switch control.
			Switch mySwitch = sender as Switch;

			// Get the name of the sublayer associated with each Switch.
			String switchSubLayerName = mySwitch.ClassId;

			// Get the first (there should be only one) ArcGISMapImageLayer from the Map.
			ArcGISMapImageLayer myArcGISMapImageLayer = myMap.AllLayers.FirstOrDefault() as ArcGISMapImageLayer;

			// Loop through each of the ArcGISMapImageLayer sublayer's.
			foreach (var item in myArcGISMapImageLayer.Sublayers)
			{
				// If the sublayer name (item.name) matches the name of the sublayer associated with the Switch 
				// control (switchSubLayerName) then toggle on/off the visibility of the sublayer.
				if (item.Name == switchSubLayerName)
				{
					item.IsVisible = e.Value;
				}
			}
		}
	}
}
