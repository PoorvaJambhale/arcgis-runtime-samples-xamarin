﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ArcGISRuntimeXamarin.Models;
using Android.App;

namespace ArcGISRuntimeXamarin.Managers
{   /// <summary>
    /// Single instance class to manage samples.
    /// </summary>
    public class SampleManager
    {
        private Assembly _samplesAssembly;
        private SampleStructureMap _sampleStructureMap;

        Activity context;

        #region Constructor and unique instance management

        // Private constructor
        private SampleManager() { }

        // Static initialization of the unique instance 
        private static readonly SampleManager SingleInstance = new SampleManager();

        public static SampleManager Current
        {
            get { return SingleInstance; }
        }

        public async Task InitializeAsync(Activity context)
        {
            // We now pass the Activity context so that we can open the groups.json
            // file using context.Assets.Open. Otherwise this functionality is only
            // available on an Activity.

            this.context = context;

            await CreateAllAsync();


            // TODO - Need to implement
            // Used for Removing samples
            // string filename = Path.GetFileName(GetType().Assembly.Location);
            // _samplesAssembly = Assembly.Load(filename);

            // RemoveEmptySamples(); 
        }

        #endregion // Constructor and unique instance management

        /// <summary>
        /// Gets or sets selected sample.
        /// </summary>
        public SampleModel SelectedSample { get; set; }

        /// <summary>
        /// Gets featured samples.
        /// </summary>
        /// <returns></returns>
        public List<FeaturedModel> GetFeaturedSamples()
        {
            return _sampleStructureMap.Featured;
        }

        /// <summary>
        /// Gets all samples as a tree.
        /// </summary>
        /// <returns>Return all categories, subcategories and samples.</returns>
        public List<TreeItem> GetSamplesAsTree()
        {
            var categories = new List<TreeItem>();

            try
            {
                List<SampleModel> sampleList = new List<SampleModel>();

                foreach (var category in _sampleStructureMap.Categories)
                {
                    var categoryItem = new TreeItem();
                    categoryItem.Name = category.Name;

                    foreach (var subCategory in category.SubCategories)
                    {
                        if (subCategory.ShowGroup)
                        {
                            var subCategoryItem = new TreeItem() { Name = subCategory.Name };
                            categoryItem.Items.Add(subCategoryItem);

                            if (subCategory.Samples != null)
                                foreach (var sample in subCategory.Samples)
                                {
                                    subCategoryItem.Items.Add(sample);
                                }
                        }
                        else
                        {
                            foreach (var sample in sampleList)
                                categoryItem.Items.Add(sample);
                        }
                    }
                    categories.Add(categoryItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return categories;
        }

        /// <summary>
        /// Creates a new control from sample.
        /// </summary>
        /// <param name="sampleModel">Sample that is transformed into a control</param>
        /// <returns>Sample as a control.</returns>
        //public Activity SampleToControl(SampleModel sampleModel)
        //{
        //	var fullTypeAsString = string.Format("{0}.{1}", sampleModel.SampleNamespace,
        //		sampleModel.GetSampleName());
        //	var sampleType = _samplesAssembly.GetType(fullTypeAsString);
        //	var item = Activator.CreateInstance(sampleType);
        //	return (UIViewController)item;
        //}

        /// <summary>
        /// Creates whole sample structure.
        /// </summary>
        private async Task CreateAllAsync()
        {
            // You can no longer check to see if groups.json exists on disk here. You have to 
            // open it and verify that it isn't null. 

            var groups = context.Assets.Open("groups.json");

            try
            {
                await Task.Run(() =>
                {
                    if (groups == null)
                        throw new NotImplementedException("groups.json file cannot be opened");
                    _sampleStructureMap = SampleStructureMap.Create("groups.json", context); // Passing the Activity context here again
                });
            }
            // This is thrown if even one of the files requires permissions greater 
            // than the application provides. 
            catch (UnauthorizedAccessException e)
            {
                throw; //TODO
            }
            catch (DirectoryNotFoundException e)
            {
                throw; //TODO
            }
            catch (Exception e)
            {
                throw; //TODO
            }
        }

        /// <summary>
        /// Remove samples that doesn't have a type registered i.e. cannot be shown.
        /// </summary>
        private void RemoveEmptySamples()
        {
            _sampleStructureMap.Featured.RemoveAll(x => !DoesSampleTypeExists(x.Sample));

            // Remove samples that are empty ie. doesn't have code files
            foreach (var category in _sampleStructureMap.Categories)
            {
                foreach (var subCategory in category.SubCategories)
                {
                    var notFoundSamples = subCategory.Samples.Where(x => !DoesSampleTypeExists(x)).ToList();
                    foreach (var sampleToRemove in notFoundSamples)
                    {
                        subCategory.Samples.Remove(sampleToRemove);
                        // subCategory.SampleNames.Remove(sampleToRemove.SampleName);
                    }
                }
            }
        }

        /// <summary>
        /// Check if the sample has a type registered.
        /// </summary>
        /// <param name="sampleModel">SampleModel that is checked.</param>
        /// <returns>Returns true if the type if found. False otherwise.</returns>
        private bool DoesSampleTypeExists(SampleModel sampleModel)
        {
            var fullTypeAsString = string.Format("{0}.{1}", sampleModel.SampleNamespace,
               sampleModel.GetSampleName());
            var sampleType = _samplesAssembly.GetType(fullTypeAsString);
            if (sampleType == null)
                return false;
            return true;
        }
    }
}