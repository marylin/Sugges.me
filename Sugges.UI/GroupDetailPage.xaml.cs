﻿using Sugges.UI.Common;
using Sugges.UI.Flyouts;
using Sugges.UI.Logic.Enumerations;
using Sugges.UI.Logic.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Group Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234229

namespace Sugges.UI
{
    /// <summary>
    /// A page that displays an overview of a single group, including a preview of the items
    /// within the group.
    /// </summary>
    public sealed partial class GroupDetailPage : Sugges.UI.Common.LayoutAwarePage
    {
        public GroupDetailPage()
        {
            this.InitializeComponent();
            this.btnDelete.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            GroupViewModel group = MainViewModel.GetGroup((Int32)navigationParameter);
            this.DefaultViewModel["Group"] = group;
            this.DefaultViewModel["Items"] = group.Items;
        }

        /// <summary>
        /// Invoked when an item is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            TripViewModel item = ((TripViewModel)e.ClickedItem);
            if (item.Identifier == -1)
            {
                if (!item.IsSuggestion)
                    this.btnAdd_Click(sender, e);
            }
            else
            {
                MainViewModel.SetSelectedTrip(item);
                this.Frame.Navigate(typeof(ItemDetailPage), item.Identifier);
            }
        }

        private void btnAdd_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.bottomAppBar.IsOpen = false;
            var trips = new SettingsFlyout();
            trips.ShowFlyout(new ManageTrip());
        }

        private void itemListView_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            foreach (object item in e.AddedItems)
            {
                if (((TripViewModel)item).Identifier == -1)
                {
                    this.itemListView.SelectedItems.Remove(item);
                    e.AddedItems.Remove(item);
                }
            }

            foreach (object item in e.AddedItems)
            {
                itemGridView.SelectedItems.Add(item);
                ((TripViewModel)item).IsSelected = true;
            }

            foreach (object item in e.RemovedItems)
            {
                itemGridView.SelectedItems.Remove(item);
                ((TripViewModel)item).IsSelected = false;
            }

            if (this.itemListView.SelectedItems.Count > 0)
                btnDelete.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else
                btnDelete.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void itemGridView_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            foreach (object item in e.AddedItems)
            {
                if (((TripViewModel)item).Identifier == -1)
                {
                    this.itemGridView.SelectedItems.Remove(item);
                    e.AddedItems.Remove(item);
                }
            }

            foreach (object item in e.AddedItems)
            {
                itemListView.SelectedItems.Add(item);
                ((TripViewModel)item).IsSelected = true;
            }

            foreach (object item in e.RemovedItems)
            {
                itemListView.SelectedItems.Remove(item);
                ((TripViewModel)item).IsSelected = false;
            }

            if (this.itemGridView.SelectedItems.Count > 0)
                btnDelete.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else
                btnDelete.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        async private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Create the message dialog and set its content and title
            var messageDialog = new MessageDialog("The seleted trips will be deleting permanently. Are your sure to do this action?", "Deleting trips");

            // Add commands and set their callbacks
            messageDialog.Commands.Add(new UICommand("Yes", (command) =>
            {
                MainViewModel.DeleteSelectedTripsAsync();
            }));

            messageDialog.Commands.Add(new UICommand("No, please cancel", (command) =>
            {
                //OK!
            }));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }
    }
}
