﻿using System;
using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using ArcGISRuntimeXamarin.Managers;
using Android.Content;

namespace ArcGISRuntimeXamarin
{
    [Activity(Label = "ArcGIS Runtime SDK for Xamarin Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        List<TreeItem> _sampleCategories;

        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.CategoriesList);

            try
            {
                // Initialize the SampleManager and create the Sample Categories
                await SampleManager.Current.InitializeAsync(this);
                _sampleCategories = SampleManager.Current.GetSamplesAsTree();

                // Set up the custom ArrayAdapter for displaying the Categories.
                var categoriesAdapter = new CategoriesAdapter(this, _sampleCategories);
                ListView categoriesListView = FindViewById<ListView>(Resource.Id.categoriesListView);
                categoriesListView.Adapter = categoriesAdapter;

                categoriesListView.ItemClick += CategoriesItemClick;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void CategoriesItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            // Don't need this currently, but would be nice eventually to pass the category instead of the 
            // position. TBD since you can't pass complex types via Intents. 
            var category = _sampleCategories[e.Position];

            var samplesListActivity = new Intent(this, typeof(SamplesListActivity));

            // Pass the index of the selected category to the SamplesListActivity
            samplesListActivity.PutExtra("SelectedCategory", e.Position);
            StartActivity(samplesListActivity);
        }
    }
}

